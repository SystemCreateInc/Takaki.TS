using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLib.Defs
{
    public enum BoxType
    {
        [Description("その他")]
        EtcBox,

        [Description("薄箱")]
        SmallBox,

        [Description("厚箱")]
        LargeBox,

        [Description("青箱")]
        BlueBox,
    }
}
