using StowageSvr.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StowageSvr.HostRequest
{
    public class GetDistGroupListResponse
    {
        public IEnumerable<ListRow> DistGroups { get; set; } = Enumerable.Empty<ListRow>();
    }
}
