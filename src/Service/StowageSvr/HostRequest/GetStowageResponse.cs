using StowageSvr.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StowageSvr.HostRequest
{
    public class GetStowageResponse
    {
        public string TdCode { get; set; } = string.Empty;
        public IEnumerable<ListRow> Batchs { get; set; } = Enumerable.Empty<ListRow>();
        public IEnumerable<ListRow> Customers { get; set; } = Enumerable.Empty<ListRow>();

        public bool IsLastCustomer { get; set; }
        public IEnumerable<long> StowageIds { get; set; } = Enumerable.Empty<long>();
        public int LargeBoxPs { get; set; }
        public int SmallBoxPs { get; set; }
        public int OtherPs { get; set; }
        public int BlueBoxPs { get; set; }
    }
}
