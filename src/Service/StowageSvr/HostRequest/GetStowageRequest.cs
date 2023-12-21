using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StowageSvr.HostRequest
{
    public class GetStowageRequest
    {
        public string Block { get; set; } = string.Empty;
        public string DistGroup { get; set; } = string.Empty;
        public string TdCode { get; set; } = string.Empty;
        public string DeliveryDate { get; set; } = string.Empty;

        public string? Batch { get; set; }
        public string? Customer { get; set; }

    }
}
