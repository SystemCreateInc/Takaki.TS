namespace SatoPrintLib
{
    // デバイスステータス
    public enum PrinterDeviceState
    {
        OnlineBatteryNearEndST4 = 0x21,

        OfflineReady = 0x30,
        OfflineBatteryNearEnd,
        OfflineBufferFull,
        OfflineBatteryNearEndAndBufferFull,

        OnlineReady = 0x41,
        OnlineBatteryNearEnd,
        OnlineBufferFull,
        OnlineBatteryNearEndAndBufferFull,

        OnlinePrintingReady = 0x47,
        OnlinePrintingBatteryNearEnd,
        OnlinePrintingBufferFull,
        OnlinePrintingBatteryNearEndAndBufferFull,

        OnlineWaitingReady = 0x4d,
        OnlineWaitingBatteryNearEnd,
        OnlineWaitingBufferFull,
        OnlineWaitingBatteryNearEndAndBufferFull,

        OnlineAnalyzingReady = 0x53,
        OnlineAnalyzingBatteryNearEnd,
        OnlineAnalyzingBufferFull,
        OnlineAnalyzingBatteryNearEndAndBufferFull,

        BufferOver = 0x61,
        NotUsed62,
        PaperEnd,
        BatteryError,

        ConnectFail = 0x100,
        Timeout,
        ServerError,
        Unknown,
    }

}
