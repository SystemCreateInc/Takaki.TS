using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceTimingLib
{
    public interface IInterfaceTiming
    {
        string Name { get; set; }
        bool EnableInterval { get; set; }
        bool EnableTiming { get; set; }
        int? IntervalSec { get; set; }
        DateTime? NextExportTime { get; set; }
        IEnumerable<TimeSpan> SpecifiedTimings { get; set; }
        DateTime? LastExportedTime { get; set; }
    }
}
