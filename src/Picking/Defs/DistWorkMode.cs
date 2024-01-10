using System.ComponentModel;

namespace Picking.Defs
{
    public enum DistWorkMode
    {
        [Description("")]
        Dist = 0,

        [Description("欠品")]
        Check,

        [Description("抜取")]
        Extraction,
    }
}
