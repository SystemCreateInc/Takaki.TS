using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLib.Defs
{
    public enum DataType
    {
        [Description("ピッキングデータ")]
        Dist = 0,

        [Description("実績データ")]
        Result = 10,
    }

}
