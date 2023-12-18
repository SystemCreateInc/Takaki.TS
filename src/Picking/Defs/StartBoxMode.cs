using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picking.Defs
{
    public enum StartBoxMode
    {
        [Description("    IN")]
        In = 0,

        [Description("    GO")]
        Go,

        [Description("")]
        Off,

        SpaceCnt = 1,
    }
}
