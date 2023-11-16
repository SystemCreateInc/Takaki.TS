using System.ComponentModel;

namespace TdDpsLib.Defs
{
    /// <summary>
    /// ポートNo指定
    /// </summary>
    public enum TdPort
    {
        [Description("Stno")]
        Stno = 0,

        [Description("ポート")]
        Port,

        [Description("ボーレート")]
        Baudrate,

        [Description("IPアドレス")]
        Ipaddr,

        [Description("コントローラタイプ 0:無線 1:有線 2:無線NEC 3:有線LAN")]
        ControllerType,
    }

    public enum TdPortStatus
    {
        [Description("")]
        Ready = 0,

        [Description("異常")]
        Error,

        [Description("接続")]
        Connect,

        [Description("切断")]
        DisConnect,
    }

    public enum TdDisplayUnitStatus
    {
        [Description("")]
        Ready = 0,

        [Description("正常")]
        Ack,

        [Description("エラー")]
        Error,

        [Description("LOW")]
        Low,

        [Description("MID")]
        Mid,

        [Description("HIG")]
        High,

        [Description("NAK")]
        Nak,

        [Description("OFF")]
        Off,

        [Description("RCN")]
        Rcn,
    }
}
