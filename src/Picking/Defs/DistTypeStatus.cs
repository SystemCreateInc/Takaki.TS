using System.ComponentModel;

namespace Picking.Defs
{
    public enum DistTypeStatus
    {
        [Description("")]
        Ready = 0,

        [Description("欠品")]
        Inprog,

        [Description("完了")]
        Completed,

        [Description("準備中")]
        DistWait,

        [Description("仕分中")]
        DistWorking,

        [Description("検品中")]
        CheckWorking,

    }
}
