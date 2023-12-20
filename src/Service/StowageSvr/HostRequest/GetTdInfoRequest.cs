using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StowageSvr.HostRequest
{
    public class GetTdInfoRequest
    {
        public string Block { get; set; } = string.Empty;
        public string DistGroup { get; set; } = string.Empty;
        public string TdCode { get; set; } = string.Empty;
    }
}
