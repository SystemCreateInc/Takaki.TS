using DbLib.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportLib.Models
{
    public record InterfaceFile(string Name, string FileName, DataType DataType, bool EnableInterval, bool EnableTiming, int? IntervalSec, int? ExpDays, IEnumerable<TimeSpan> Timings, string? HulftId);
}
