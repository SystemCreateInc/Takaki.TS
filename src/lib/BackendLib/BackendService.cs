using LogLib;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Text;

namespace BackendLib
{
    public class BackendService : IDisposable
    {
        public const int BE_ERROR_OK = 0;
        public const int BE_ERROR_INVALIDPARAM				= 1;
        public const int BE_ERROR_CONNECT					= 2;
        public const int BE_ERROR_SEND						= 3;
        public const int BE_ERROR_READ						= 4;
        public const int BE_ERROR_TIMEOUT					= 5;
        public const int BE_ERROR_UNKNOWN					= 6;
        public const int BE_ERROR_OUTOFMEMORY				= 7;
        public const int BE_ERROR_UNKNOWNCLIENT				= 8;
        public const int BE_ERROR_UNKNOWNMESSAGE			= 9;
        public const int BE_ERROR_CANCELED					= 10;
        public const int BE_ERROR_CHECKTIMEOUT				= 11;

        public const int BEM_ECHO = 0;
        public const int BEM_REGIST = 1;
        public const int BEM_REGISTERED = 2;
        public const int BEM_UNREGISTERED = 3;
        public const int BEM_LISTREGIST = 5;
        public const int BEM_LISTCLIENTS = 6;
        public const int BEM_LISTPROCESSORS = 7;
        public const int BEM_NOTIFYMASK = 8;
        public const int BEM_SENDTOCAPABILITY = 9;
        public const int BEM_CORE = 0x10000000;

        public const int BEMS_NONEEDRESULT = (0);
        public const int BEMS_NEEDRESULT = (1 << 0);
        public const int BEMS_RESULT = (1 << 1);
        public const int BEMS_ERROR = (1 << 2);
        public const int BEMS_BROADCAST = (1 << 3);


        private readonly IntPtr _bes;

        public BackendService(ClientType clientType, string name, string capability, string user, string passwd)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("common.json", true, true)
                .Build();

            var section = config.GetSection("backend");
            var backendAddress = section["address"] ?? "localhost";

            BackendApi.BackendInit(ref _bes, (int)clientType, name, capability, user, passwd, backendAddress);
            Console.WriteLine("BackendInit");
        }

        public void Dispose()
        {
            BackendApi.BackendTerm(_bes);
            Console.WriteLine("BackendTerm");
        }

        public int CallByName(string capability, int message, byte[] sendbuf, out byte[] recvbuf)
        {
            int rc = BackendApi.BackendCallByName(_bes, capability, message, sendbuf.Length, sendbuf, out int recvsize, out recvbuf);
            return rc;
        }

        public int CallByName(string capability, int message, string sendText, out string recvText, Encoding? encoding = null)
        {
            Syslog.Debug($"Backend:S:CallByName: ({sendText.Length}) {sendText}");

            if (encoding == null)
            {
                encoding = Encoding.GetEncoding("SJIS");
            }

            var sendBytes = encoding.GetBytes(sendText);
            int rc = CallByName(capability, BackendService.BEM_CORE, sendBytes, out byte[] recivedBytes);

            if (recivedBytes != null)
            {
                recvText = encoding.GetString(recivedBytes);
            }
            else
            {
                recvText = "";
            }

            Syslog.Debug($"Backend:R:CallByName: ({recvText.Length}) {recvText}");

            return rc;
        }

        public class Message
        {
            public int rc;
            public int fromClientId;
            public int message;
            public byte[] data = Array.Empty<byte>();
        };

        public Message CurrentMessage { get; set; } = new Message();

        public Message GetMessage()
        {
            int rc = BackendApi.BackendGetMessage(_bes, out int from_clientid, out int message, out int size, out IntPtr buffer);
            var data = new byte[size];

            if (size > 0)
            {
                Marshal.Copy(buffer, data, 0, size);
            }

            Marshal.FreeCoTaskMem(buffer);

            return new Message
            {
                rc = rc,
                fromClientId = from_clientid,
                message = message,
                data = data,
            };
        }

        public void Cancel()
        {
            BackendApi.BackendCancel(_bes);
        }

        public int PostMessage(int handle, int message, byte[]? sendbuf, short status = 0)
        {
            return BackendApi.BackendPostData(
                    _bes,
                    handle,
                    status,
                    message,
                    sendbuf != null ? sendbuf.Length : 0,
                    sendbuf
            );
        }

        public int PostMessageByName(string capability, int message, byte[] sendbuf, short status = 0)
        {
            return BackendApi.BackendPostDataByName(
                    _bes,
                    capability,
                    status,
                    message,
                    sendbuf.Length,
                    sendbuf
            );
        }

        public static string GetErrorMessage(int err)
        {
            const string prefix = "BACKEND: ";
            switch (err)
            {
                case BE_ERROR_OK:
                    return "";

                case BE_ERROR_INVALIDPARAM:
                    return prefix + "パラメータが不正です";

                case BE_ERROR_CONNECT:
                    return prefix + "接続できませんでした";

                case BE_ERROR_SEND:
                    return prefix + "送信できませんでした";

                case BE_ERROR_READ:
                    return prefix + "受信できませんでした";

                case BE_ERROR_TIMEOUT:
                    return prefix + "タイムアウト";

                default:
                case BE_ERROR_UNKNOWN:
                    return prefix + "不明なエラーです";

                case BE_ERROR_OUTOFMEMORY:
                    return prefix + "メモリー不足です";

                case BE_ERROR_UNKNOWNCLIENT:
                    return prefix + "不明なクライアントです";

                case BE_ERROR_UNKNOWNMESSAGE:
                    return prefix + "不明なメッセージです";

                case BE_ERROR_CANCELED:
                    return prefix + "中断しました";

                case BE_ERROR_CHECKTIMEOUT:
                    return prefix + "タイムアウトになりました";
            }
        }

        public static void ThrowIf(int err)
        {
            if (err != BE_ERROR_OK)
            {
                throw new BackendException(GetErrorMessage(err));
            }
        }

    }
}
