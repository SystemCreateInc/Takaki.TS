namespace SatoPrintLib
{
    public struct PrinterStatus
    {
        public PrinterStatus()
        {
        }

        public bool Valid { get; set; } = false;

        public ConnectionState ConnectionState { get; set; } = ConnectionState.Disconnected;

        public PrinterDeviceState DeviceState { get; set; } = PrinterDeviceState.Unknown;

        public int PrintCount { get; set; } = 0;

        public string Message { get; set; } = string.Empty;

        // ステータス取得時間
        public DateTime GetStateTime { get; private set; } = DateTime.Now;

        // 直近ステータス
        public PrinterDeviceState RecentlyState
        {
            get
            {
                // Redyステータスのみ
                if (DeviceState == PrinterDeviceState.OnlineReady)
                {
                    var elapsedTime = DateTime.Now - GetStateTime;

                    // 30秒以上経過した保持ステータスを無効
                    if (elapsedTime.TotalSeconds > 30)
                    {
                        return PrinterDeviceState.Unknown;
                    }
                }

                return DeviceState;
            }
        }
    }
}
