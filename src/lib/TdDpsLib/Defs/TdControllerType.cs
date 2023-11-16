using System.ComponentModel;

namespace TdDpsLib.Defs
{
    /// 表示器コントーラータイプ(Dllと同順)
    public enum TdControllerType
    {
        [Description("無線")]
        Wireless = 0,

        [Description("有線")]
        Wired,

        [Description("無線(NEC)")]
        Wireless_Nec,

        [Description("有線LAN")]
        Wired_Lan,

        [Description(" ")]
        None,
    }
}
