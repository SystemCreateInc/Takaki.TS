using Picking.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picking.Models
{
    public class ReportShain
    {
        public string CdShain { get; set; } = string.Empty;
        public string NmShain { get; set; } = string.Empty;
        public DateTime? DtWorkStart { get; set; } = null;
        public DateTime? DtWorkEnd { get; set; } = null;
        public double WorkTime { get; set; } = 0;
        public double IdleTime { get; set; } = 0;
        public int ItemCnt { get; set; } = 0;
        public int ShopCnt { get; set; } = 0;
        public int DistCnt { get; set; } = 0;
        public int CheckCnt { get; set; } = 0;
        public double CheckTime { get; set; } = 0;
        public DateTime? DtNowStart { get; set; } = null;
        public int DistWorkMode { get; set; } = 0;
        public List<string> addrs { get; set; } = new List<string>();
    }
}
