using System.ComponentModel;

namespace Mapping.Defs
{
    public enum MStatus
    {
        [Description(" ")]
        Ready = 0,

        [Description("実行済")]
        Run,

        [Description("決定済")]
        Decision,
    }
}
