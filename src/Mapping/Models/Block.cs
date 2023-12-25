using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapping.Models
{
    public class Block
    {
        public string CdKyoten = string.Empty;
        public string CdBlock = string.Empty;
        public int StTdUnitType = 5;
        public int NuTdUnitCnt = 0;
        public decimal NuThreshold = 0;

        public List<Addr> addrs = new List<Addr>();
    }
}
