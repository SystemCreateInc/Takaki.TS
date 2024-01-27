using DbLib.Defs;
using ExportLib.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace ExportLib.Processors
{
    public abstract class ExportProcessor : IExportProcessor
    {
        public string Name { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DataType DataType { get; set; }
        public int? IntervalSec { get; set; }
        public DateTime? NextExportTime { get; set; }
        public int AvailableExportCount { get; set; }
        public string LockKey { get; set; }
        public IEnumerable<TimeSpan> SpecifiedTimings { get; set; } = Enumerable.Empty<TimeSpan>();
        public bool EnableInterval { get; set; }
        public bool EnableTiming { get; set; }
        public string FieldSeparator { get; set; } = ",";
        public string LineSeparator { get; set; } = "\r\n";

        public DateTime? LastExportedTime { get; set; }
        public int? ExpDays { get; set; }
        public string HulftId { get; set; } = string.Empty;

        public ExportProcessor(DataType dataType, string lockKey)
        {
            DataType = dataType;
            LockKey = lockKey;
        }

        public InterfaceFile GetInterfaceFile()
        {
            return new InterfaceFile(Name, FileName, DataType, EnableInterval, EnableTiming, IntervalSec, ExpDays, SpecifiedTimings, HulftId);
        }

        // 送信対象時間、待機時間後を対象にする
        public DateTime GetExportTargetTime(bool exportAll)
        {
            return DateTime.Now;
        }

        public abstract void UpdateAvailableExportCount(IDbTransaction tr);

        public virtual void PreExport(ExportContext ctxt)
        {
        }

        public abstract void ExportData(ExportContext ctxt);

        public virtual void PostExport(ExportContext ctxt)
        {
        }

        public virtual string GetLogComment(ExportContext ctxt)
        {
            return ctxt.IsForce ? "強制" : "";
        }
    }
}
