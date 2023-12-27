using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapping.Models
{
    public class ShukkaBatch
    {
        public int NuShukkaBatchSeq = 0;
        public string CdShukkaBatch = string.Empty;
        public string NmShukkaBatch = string.Empty;
        public string CdLargeGroup = string.Empty;
        public string NmLargeGroup = string.Empty;
    }
    public class DistBlockSeq
    {
        public string CdBlock = string.Empty;
        public string CdAddrFrom = string.Empty;
        public string CdAddrTo = string.Empty;
    }
    public class DistLargeGroup
    {
        public string CdLargeGroup = string.Empty;
        public string NmLargeGroup = string.Empty;
    }

    public class DistGroup
    {
        public string CdKyoten = string.Empty;
        public string NmKyoten = string.Empty;
        public long IdDistGroup = 0;
        public string CdDistGroup = string.Empty;
        public string NmDistGroup = string.Empty;
        public int CdBinSum = 0;
        public List<ShukkaBatch> ShukkaBatchs = new List<ShukkaBatch>();
        public List<DistBlockSeq> DistBlockSeqs = new List<DistBlockSeq>();
        public List<string> Courses = new List<string>();

        // 予定データ
        public List<Dist> dists = new List<Dist>();
        public List<Dist> stowages = new List<Dist>();

        // 実績
        public List<Dist> mappings = new List<Dist>();
    }
}
