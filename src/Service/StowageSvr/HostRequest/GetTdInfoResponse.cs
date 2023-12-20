using StowageSvr.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StowageSvr.HostRequest
{
    public class GetTdInfoResponse
    {
        public string TdCode { get; set; } = string.Empty;
        public IEnumerable<ListRow> Batchs { get; set; } = Enumerable.Empty<ListRow>();
        public IEnumerable<ListRow> Customers { get; set; } = Enumerable.Empty<ListRow>();
        public int ThickBoxPs { get; set; }
        public int WeakBoxPs { get; set; }
        public int OtherPs { get; set; }
        public int BlueBoxPs { get; set; }
    }
}
