using System.ComponentModel;

namespace SyslogCS.Models
{
    public enum Truncate
    {
        [Description("削除しない")]
        None = 0,

        [Description("削除")]
        Trunc,
    }
}
