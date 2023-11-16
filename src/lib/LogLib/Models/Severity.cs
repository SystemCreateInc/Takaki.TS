using System.ComponentModel;

namespace SyslogCS.Models
{
    public enum Severity : int
    {
        [Description("致命的")]
        Emerg = 0,

        [Description("警戒")]
        Alert = 1,

        [Description("危機的")]
        Crit = 2,

        [Description("エラー")]
        Err = 3,

        [Description("警告")]
        Warn = 4,

        [Description("通知")]
        Notice = 5,

        [Description("情報")]
        Info = 6,

        [Description("デバッグ")]
        Debug = 7,
    }
}
