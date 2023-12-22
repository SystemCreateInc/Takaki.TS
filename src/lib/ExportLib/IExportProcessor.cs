using DbLib.Defs;
using ExportLib.Models;
using InterfaceTimingLib;
using System;
using System.Collections.Generic;
using System.Data;

namespace ExportLib
{
    public interface IExportProcessor : IInterfaceTiming
    {
        string FileName { get;  set; }
        DataType DataType { get;  set; }
        int AvailableExportCount { get;  set; }
        string LockKey { get;  set; }
        int? ExpDays { get; set; }

        void UpdateAvailableExportCount(IDbTransaction tr);

        void PreExport(ExportContext ctxt);

        void ExportData(ExportContext ctxt);

        void PostExport(ExportContext ctxt);

        string GetLogComment(ExportContext ctxt);

        void SetFolder(string path)
        {
            var filename = Path.GetFileName(FileName);
            FileName = Path.Combine(path, filename);
        }

        InterfaceFile GetInterfaceFile();
    }
}
