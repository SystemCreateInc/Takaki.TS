using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLib.Defs
{
    public enum TdUnitType
    {
        [Description("天吊")]
        TdCeiling = 5,
        [Description("棚")]
        TdRack = 6,
        [Description("天吊ｽﾀｰﾄBOX")]
        TdCeilingBox = 7,
        [Description("棚ｽﾀｰﾄBOX")]
        TdRackBox = 8,
    }
}
