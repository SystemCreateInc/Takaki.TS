using System.ComponentModel;

namespace WindowLib.Defs
{
    public enum DisplayStatus
    {
        [Description(" ")]
        Ready = 0,

        [Description("処理中")]
        Inprog,

        [Description("保留")]
        Holded,

        [Description("完了")]
        Completed,

        [Description("強制終了")]
        ForceCompleted,

        [Description("その他")]
        Other,
    }
}
