using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StowageSvr.HostRequest
{
    public class GetDistGroupRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Block { get; set; } = string.Empty;
        public string DeliveryDate { get; set; } = string.Empty;
    }
}
