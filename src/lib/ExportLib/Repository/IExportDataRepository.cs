using System;
using System.Collections.Generic;
using System.Data;

namespace ExportLib.Repository
{
    public interface IExportDataRepository<T>
        where T : class
    {
        IEnumerable<T> GetTargetData(IDbTransaction tr);

        void FixData(IDbTransaction tr, IEnumerable<long> ids);
    }
}
