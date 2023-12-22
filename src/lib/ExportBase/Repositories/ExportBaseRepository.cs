using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using ExportBase.Models;
using ExportLib.Models;
using ImTools;
using InterfaceTimingLib;
using LogLib;
using System.Data;

namespace ExportBase.Repositories
{
    public class ExportBaseRepository : IDisposable
    {
        public IDbConnection Connection { get; set; }
        public IDbTransaction Transaction { get; set; }

        private bool _comitted = false;

        public ExportBaseRepository()
        {
            Connection = DbFactory.CreateConnection();
            Transaction = Connection.BeginTransaction();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_comitted)
                {
                    Rollback();
                }

                Transaction.Dispose();
                Connection.Close();
                Connection.Dispose();
            }
        }

        public void Commit()
        {
            Transaction.Commit();
            _comitted = true;
        }

        public void Rollback()
        {
            Transaction.Rollback();
        }

        internal void DeleteExpiredLogs()
        {
            var expDate = GetExpiredDate();
            if (expDate is null)
            {
                Syslog.Debug("Skip delete interface logs");
                return;
            }

            Connection.BulkDelete<InterfaceLogsEntity>(s => s
                .AttachToTransaction(Transaction)
                .Where($"{nameof(InterfaceLogsEntity.CreatedAt):C} < @expDate")
                .WithParameters(new { expDate }));
        }

        private DateTime? GetExpiredDate()
        {
            var expdays = new Settings(Transaction).GetInt("expdays", 0);
            if (expdays == 0)
            {
                return null;
            }

            return DateTime.Now.Date.AddDays(-expdays);
        }

        internal static IEnumerable<Log> GetAllLogs(IEnumerable<DataType> dataTypes)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<InterfaceLogsEntity>(s => s
                    .Where($"{nameof(InterfaceLogsEntity.DataType):C} in @dataTypes")
                    .OrderBy($"{nameof(InterfaceLogsEntity.Id):C} desc")
                    .WithParameters(new { dataTypes }))
                    .Select(x => new Log(x.CreatedAt, x.Status, x.Name, x.RowCount, x.Terminal, x.Comment ?? ""));
            }
        }

        internal void Save(InterfaceFile interfaceFile)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var entity = con.Get(new InterfaceFileEntity { DataType = (short)interfaceFile.DataType });
                if (entity is null)
                {
                    return;
                }

                entity.FileName = interfaceFile.FileName;
                con.Update(entity);
            }
        }
    }
}
