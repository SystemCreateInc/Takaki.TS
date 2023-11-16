using System.ComponentModel;

namespace DbLib.Defs
{
    public enum Status
    {
        [Description(" ")]
        Ready = 0,

        [Description("処理中")]
        Inprog,

        [Description("完了")]
        Completed,
    }
}
