using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistBlock.Models
{
    public class SameDistBlock
    {
        public long DistBlockId { get; set; }
        public string CdKyoten { get; set; } = string.Empty;
        public string CdDistGroup { get; set; } = string.Empty;
        public string CdBlock { get; set; } = string.Empty;
        public string CdAddrFrom { get; set; } = string.Empty;
        public string CdAddrTo { get; set; } = string.Empty;

        public string Tekiyokaishi { get; set; } = string.Empty;
        public string TekiyoMuko { get; set; } = string.Empty;

        public string PadBlock => CdBlock.PadLeft(2, '0');
        public string PadAddrFrom => CdAddrFrom.PadLeft(4, '0');
        public string PadAddrTo => CdAddrTo.PadLeft(4, '0');
    }
}
