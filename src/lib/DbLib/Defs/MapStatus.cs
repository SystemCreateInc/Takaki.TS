using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLib.Defs
{
    public enum MapStatus
    {
        [Description("未処理")]
        Ready,

        [Description("あふれ")]
        Over,

        [Description("マッピング完了")]
        Completed,
    }
}
