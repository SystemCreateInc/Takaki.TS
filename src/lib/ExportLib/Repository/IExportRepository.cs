using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using ExportLib.Models;
using System.Data;
using static DbLib.AppLock;

namespace ExportLib.Repository
{
    public interface IExportRepository : IDisposable
    {
        IDbTransaction Transaction { get; }

        AppLock Lock(string resouce, LockMode lockMode = LockMode.Exclusive, LockOwner lockOwner = LockOwner.Session);

        void Commit();

        void Rollback();

        InterfaceFile? GetInterfaceFile(DataType dataType);

        void Save(InterfaceFile ifile);

        void InsertLog(InterfaceLogsEntity log);
    }
}
