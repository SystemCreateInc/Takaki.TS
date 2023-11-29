using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using DbLib.Defs.DbLib.Defs;
using ImportLib.Engines;
using ImportLib.Models;
using ImTools;
using LogLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImportLib.Repositories
{
    public class ImportRepository : IDisposable
    {
        public IDbConnection Connection { get; set; }
        public IDbTransaction Transaction { get; set; }

        private bool _comitted = false;

        public ImportRepository()
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

        public InterfaceFile? GetInterfaceFileByDataType(DataType dataType)
        {
            return Connection.Find<InterfaceFileEntity>(s => s
                .AttachToTransaction(Transaction)
                .Where($"{nameof(InterfaceFileEntity.DataType):C} = {nameof(dataType):P}")
                .WithParameters(new { dataType }))
                .Select(x => new InterfaceFile((DataType)x.DataType, x.Name, x.SortOrder, x.FileName, x.Expdays))
                .FirstOrDefault();
        }

        public void Insert(TBMKYOTENEntity entity)
        {
            Connection.Insert(entity, s => s.AttachToTransaction(Transaction));
        }

        public void Insert(TBMSHAINEntity entity)
        {
            Connection.Insert(entity, s => s.AttachToTransaction(Transaction));
        }

        public void Insert(TBMTOKUISAKIEntity entity)
        {
            Connection.Insert(entity, s => s.AttachToTransaction(Transaction));
        }

        public void Insert(TBMSHUKKABATCHEntity entity)
        {
            Connection.Insert(entity, s => s.AttachToTransaction(Transaction));
        }

        public void Insert(TBMKOTEIMEISHOEntity entity)
        {
            Connection.Insert(entity, s => s.AttachToTransaction(Transaction));
        }

        public void Insert(InterfaceLogsEntity entity)
        {
            Connection.Insert(entity, s => s.AttachToTransaction(Transaction));
        }

        public void Insert(TBMHIMMOKUEntity log)
        {
            Connection.Insert(log, s => s.AttachToTransaction(Transaction));
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

        internal void DeleteExpiredKyotenData()
        {
            var expDate = GetExpiredDate();
            if (expDate is null)
            {
                Syslog.Debug("Skip delete Kyoten data");
                return;
            }

            Connection.BulkDelete<TBMKYOTENEntity>(s => s
                .AttachToTransaction(Transaction)
                .Where($"{nameof(TBMKYOTENEntity.CreatedAt):C} < @expDate")
                .WithParameters(new { expDate }));
        }

        internal void DeleteExpiredIssueData()
        {
            var expDate = GetExpiredDate();
            if (expDate is null)
            {
                Syslog.Debug("Skip delete shain data");
                return;
            }

            Connection.BulkDelete<TBMSHAINEntity>(s => s
                .AttachToTransaction(Transaction)
                .Where($"{nameof(TBMSHAINEntity.CreatedAt):C} < @expDate")
                .WithParameters(new { expDate }));
        }

        internal void DeleteExpiredTokuisakiData()
        {
            var expDate = GetExpiredDate();
            if (expDate is null)
            {
                Syslog.Debug("Skip delete tokuisaki data");
                return;
            }

            Connection.BulkDelete<TBMTOKUISAKIEntity>(s => s
                .AttachToTransaction(Transaction)
                .Where($"{nameof(TBMTOKUISAKIEntity.CreatedAt):C} < @expDate")
                .WithParameters(new { expDate }));
        }

        internal void DeleteExpiredHimmokuData()
        {
            var expDate = GetExpiredDate();
            if (expDate is null)
            {
                Syslog.Debug("Skip delete himmoku data");
                return;
            }

            Connection.BulkDelete<TBMHIMMOKUEntity>(s => s
                .AttachToTransaction(Transaction)
                .Where($"{nameof(TBMHIMMOKUEntity.CreatedAt):C} < @expDate")
                .WithParameters(new { expDate }));
        }

        internal void DeleteExpiredshukkabatchData()
        {
            var expDate = GetExpiredDate();
            if (expDate is null)
            {
                Syslog.Debug("Skip delete shukkabatch data");
                return;
            }

            Connection.BulkDelete<TBMSHUKKABATCHEntity>(s => s
                .AttachToTransaction(Transaction)
                .Where($"{nameof(TBMSHUKKABATCHEntity.CreatedAt):C} < @expDate")
                .WithParameters(new { expDate }));
        }

        internal void DeleteExpiredKoteimeishoData()
        {
            var expDate = GetExpiredDate();
            if (expDate is null)
            {
                Syslog.Debug("Skip delete koteimeisho data");
                return;
            }

            Connection.BulkDelete<TBMKOTEIMEISHOEntity>(s => s
                .AttachToTransaction(Transaction)
                .Where($"{nameof(TBMKOTEIMEISHOEntity.CreatedAt):C} < @expDate")
                .WithParameters(new { expDate }));
        }

        //internal void DeleteItemData()
        //{
        //    Connection.BulkDelete<ItemEntity>(s => s
        //        .AttachToTransaction(Transaction));
        //}

        private DateTime? GetExpiredDate()
        {
            var expdays = new Settings(Transaction).GetInt("expdays", 0);
            if (expdays == 0)
            {
                return null;
            }

            return DateTime.Now.Date.AddDays(-expdays);
        }

        private DateTime? GetInventoryLogExpiredDate()
        {
            var expdays = new Settings(Transaction).GetInt("InventoryLog_expdays", 365 * 5);
            if (expdays == 0)
            {
                return null;
            }

            return DateTime.Now.Date.AddDays(-expdays);
        }

        internal static IEnumerable<Log> GetAllLogs(List<DataType> dataTypes)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<InterfaceLogsEntity>(s => s
                    .Where($"{nameof(InterfaceLogsEntity.DataType):C} in @dataTypes")
                    .OrderBy($"{nameof(InterfaceLogsEntity.Id):C} desc")
                    .WithParameters(new { dataTypes }))
                    .Select(x => new Log(x.CreatedAt!.Value, x.Status, x.Name, x.RowCount, x.Terminal, x.Comment ?? ""));
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
