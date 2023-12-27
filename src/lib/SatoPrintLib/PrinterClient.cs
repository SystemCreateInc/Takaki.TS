using LogLib;
using System.Net.Sockets;
using System.Text;

namespace SatoPrintLib
{
    public class PrinterClient
    {
        private const int _port = 1024;

        private CancellationToken _cancellationToken;
        private readonly object _lock = new object();
        private TcpClient? _socket;
        private NetworkStream? _stream;

        public string Address { get; }
        private PrinterStatus _deviceStatus = new PrinterStatus();
        public PrinterStatus Status
        {
            get
            {
                lock (_lock)
                {
                    return _deviceStatus;
                }
            }
            private set
            {
                lock (_lock)
                {
                    _deviceStatus = value;
                }
            }
        }

        public bool IsConnected => _socket != null;

        public void Disconnect()
        {
            if (_socket == null)
            {
                return;
            }

            lock (_lock)
            {
                Status = Status with
                {
                    Message = "",
                    ConnectionState = ConnectionState.Disconnected,
                    Valid = false,
                };

                _stream?.Close();
                _stream?.Dispose();
                _stream = null;
                _socket.Close();
                _socket.Dispose();
                _socket = null;
                Syslog.Debug($"{Address}: Disconnected");
            }
        }

        public PrinterClient(string address, CancellationToken token)
        {
            Address = address;
            _cancellationToken = token;
        }

        public void ClearStatus()
        {
            Status = new PrinterStatus();
        }

        public void GetStatus()
        {
            Syslog.Debug($"{Address}: Start request staus");

            try
            {
                Connect();
                GetStatusFromDevice();
            }
            catch (PrintException e)
            {
                Syslog.Warn($"{Address}: PrintServer EXCEPTION: {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                Syslog.Warn($"{Address}: EXCEPTION: {e.Message}");
                Status = new PrinterStatus
                {
                    Message = e.Message,
                    ConnectionState = ConnectionState.Disconnected,
                    DeviceState = PrinterDeviceState.ServerError,
                    Valid = false,
                };
                throw;
            }
            finally 
            { 
                Disconnect();
                Syslog.Debug($"{Address}: End request status");
            }

        }

        public void Print(string data)
        {
            Syslog.Debug($"{Address}: Start request print");

            try
            {
                Connect();
                WaitPrinterReady();
                SendData(data);
            }
            catch (PrintException e)
            {
                Syslog.Warn($"{Address}: PrintServer EXCEPTION: {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                Syslog.Warn($"{Address}: EXCEPTION: {e.Message}");
                Status = new PrinterStatus
                {
                    Message = e.Message,
                    ConnectionState = ConnectionState.Disconnected,
                    DeviceState = PrinterDeviceState.ServerError,
                    Valid = false,
                };
                throw;
            }
            finally 
            { 
                Disconnect();
                Syslog.Debug($"{Address}: End request print");
            }
        }

        public void Connect()
        {
            try
            {
                Syslog.Debug($"{Address}: Connecting...");
                Status = Status with
                {
                    ConnectionState = ConnectionState.Connecting,
                };
                _socket = new TcpClient();
                _socket.Connect(Address, _port);

                Syslog.Debug($"Connected!");
                _stream = _socket.GetStream();
                _stream.ReadTimeout = 5000;

                Status = Status with
                {
                    ConnectionState = ConnectionState.Connected,
                };
            }
            catch (Exception e)
            {
                Syslog.Warn($"Connect FAIL!!!");
                Status = new PrinterStatus
                {
                    Message = e.Message,
                    ConnectionState = ConnectionState.ConnectFail,
                    DeviceState = PrinterDeviceState.ConnectFail,
                    Valid = false,
                };
                throw new PrintException(e.Message, e);
            }
        }

        public void GetStatusFromDevice()
        {
            Syslog.Debug($"{Address}: Request Status");
            Send("\x05");
            var data = ReadResponse();
            if (!ParseStatus(data))
            {
                Syslog.Warn($"{Address}: Unrecognized status");
            }
            else
            {
                Syslog.Debug($"{Address}: Status is {Status.DeviceState}");
            }
        }

        public void Send(string data)
        {
            var bytes = Encoding.GetEncoding("SJIS").GetBytes(data);
            _stream?.Write(bytes, 0, bytes.Length);
        }

        public byte[] ReadResponse()
        {
            var buffer = new byte[32];
            var data = new List<byte>();
            bool run = true;

            while (run)
            {
                var readedBytes = _stream?.Read(buffer, 0, buffer.Length) ?? 0;
                var lfpos = Array.FindIndex(buffer, 0, readedBytes, c => c == 0x03);
                if (lfpos != -1)
                {
                    run = false;
                    readedBytes = lfpos + 1;
                }

                data.AddRange(new Span<byte>(buffer.ToArray()).Slice(0, readedBytes).ToArray());
            }

            return data.ToArray();
        }

        public void WaitPrinterReady()
        {
            while (true)
            {
                GetStatusFromDevice();

                if (Status.DeviceState == PrinterDeviceState.OnlineReady
                    || Status.DeviceState == PrinterDeviceState.OnlineAnalyzingReady
                    || Status.DeviceState == PrinterDeviceState.OnlinePrintingReady
                    || Status.DeviceState == PrinterDeviceState.OnlineWaitingReady
                    || Status.DeviceState == PrinterDeviceState.OnlineBatteryNearEnd
                    || Status.DeviceState == PrinterDeviceState.OnlinePrintingBatteryNearEnd
                    || Status.DeviceState == PrinterDeviceState.OnlineWaitingBatteryNearEnd
                    || Status.DeviceState == PrinterDeviceState.OnlineAnalyzingBatteryNearEnd
                    || Status.DeviceState == PrinterDeviceState.OnlineBatteryNearEndST4)
                {
                    return;
                }

                Syslog.Debug($"{Address}: Printer status not ready {Status.DeviceState}");
                _cancellationToken.WaitHandle.WaitOne(1000);
                _cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private bool ParseStatus(byte[] data)
        {
            Syslog.Debug($"{Address}: Status: {string.Join("-", data.Select(x => x.ToString("X2")))}");
            var valid = false;

            if (data.Length == 11)
            {
                try
                {
                    var span = new Span<byte>(data);
                    Status = new PrinterStatus
                    {
                        Message = "",
                        ConnectionState = ConnectionState.Connected,
                        DeviceState = (PrinterDeviceState)span[3],
                        PrintCount = int.Parse(Encoding.ASCII.GetString(span.Slice(5, 4).ToArray())),
                        Valid = true,
                    };
                    valid = true;
                }
                catch (Exception e)
                {
                    Syslog.Warn($"FAIL Parse status {e.Message}");
                }
            }

            if (!valid)
            {
                Status = Status with { Valid = false, DeviceState = PrinterDeviceState.Unknown, Message = "" };
            }

            return valid;
        }

        public void SendData(string data)
        {
            Syslog.Debug($"{Address}: Send data {data.Length} bytes");
            Send(data);

            // ACKが帰る
            var buffer = new byte[1];
            var readedBytes = _stream?.Read(buffer, 0, buffer.Length);
            Syslog.Debug($"{Address}: response: {buffer[0].ToString("X2")}");
        }
    }
}
