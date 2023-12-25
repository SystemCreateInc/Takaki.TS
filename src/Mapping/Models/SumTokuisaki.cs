using DbLib.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapping.Models
{
    public class SumTokuisaki
    {
        public long IdSumTokuisaki = 0;
        public string CdKyoten = string.Empty;
        public string CdSumTokuisaki = string.Empty;
        public string NmSumTokuisaki = string.Empty;
        public string CdSumCourse = string.Empty;
        public int CdSumRoute = 0;
        public List<string> SumTokuisakiChilds = new List<string>();
    }
}
