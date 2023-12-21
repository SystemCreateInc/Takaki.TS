using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using ExportLib.Models;
using ExportLib.Repository;
using System.Data;
using static DbLib.AppLock;

namespace ExportLib.Infranstructures
{
    public class ExportRepository : IExportRepository
    {
        public IDbTransaction Transaction { get; }
        public IDbConnection Connection { get; private set; }

        private bool _commited = false;

        public ExportRepository()
        {
            Connection = DbFactory.CreateConnection();
            Transaction = Connection.BeginTransaction();
        }

        public AppLock Lock(string resource, LockMode lockMode = LockMode.Exclusive, LockOwner lockOwner = LockOwner.Session)
        {
            return new AppLock(Connection, Transaction, resource, lockMode, lockOwner);
        }

        public void Commit()
        {
            Transaction?.Commit();
            _commited = true;
        }

        public void Rollback()
        {
            Transaction?.Rollback();
        }

        public void Dispose()
        {
            if (!_commited)
            {
                Transaction?.Rollback();
            }

            Connection?.Dispose();
        }

        public InterfaceFile? GetInterfaceFile(DataType dataType)
        {
            var dao = Connection.Get(new InterfaceFileEntity { DataType = (short)dataType }, s => s
                .AttachToTransaction(Transaction));

            if (dao == null)
            {
                return null;
            }

            return new InterfaceFile
            (
                dao.Name,
                dao.FileName,
                (DataType)dao.DataType,
                false,
                false,
                null,
                dao.Expdays,
                Enumerable.Empty<TimeSpan>()
            );
        }

        public void Save(InterfaceFile ifile)
        {
            var dao = Connection.Get(new InterfaceFileEntity { DataType = (short)ifile.DataType }, s => s
                .AttachToTransaction(Transaction));

            if (dao is null)
            {
                throw new Exception("インターフェース定義情報を取得できませんでした");
            }

            dao.FileName = ifile.FileName;
            dao.Expdays = ifile.ExpDays;
            Connection.Update(dao, s => s.AttachToTransaction(Transaction));
        }

        public void InsertLog(InterfaceLogsEntity log)
        {
            Connection.Insert(log, s => s.AttachToTransaction(Transaction));
        }
    }
}
