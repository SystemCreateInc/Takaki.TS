using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using DbLib.Defs.DbLib.Defs;
using ImportLib.Models;
using ImTools;
using LogLib;
using System.Data;

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


        public void Insert(TBDISTEntity entity)
        {
            Connection.Insert(entity, s => s.AttachToTransaction(Transaction));
        }
        public void Insert(TBSTOWAGEEntity entity)
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

        internal void DeleteKyotenData()
        {
            Connection.BulkDelete<TBMKYOTENEntity>(s => s
                .AttachToTransaction(Transaction));
        }

        internal void DeleteIssueData()
        {
            Connection.BulkDelete<TBMSHAINEntity>(s => s
                .AttachToTransaction(Transaction));
        }

        internal void DeleteTokuisakiData()
        {
            Connection.BulkDelete<TBMTOKUISAKIEntity>(s => s
                .AttachToTransaction(Transaction));
        }

        internal void DeleteExpiredHimmokuData()
        {
            var sql = Sql.Format<TBMHIMMOKUEntity>($"TRUNCATE TABLE {nameof(TBMHIMMOKUEntity):T}");
            Connection.Execute(sql, transaction: Transaction);
        }

        internal void DeleteExpiredShukkabatchData()
        {
            Connection.BulkDelete<TBMSHUKKABATCHEntity>(s => s
                .AttachToTransaction(Transaction));
        }

        internal void DeleteExpiredKoteimeishoData()
        {
            Connection.BulkDelete<TBMKOTEIMEISHOEntity>(s => s
                .AttachToTransaction(Transaction));
        }

        internal void DeleteExpiredShainData()
        {
            Connection.BulkDelete<TBMSHAINEntity>(s => s
                .AttachToTransaction(Transaction));
        }

        internal void DeleteExpiredDistData()
        {
            var expDate = GetExpiredDate();
            if (expDate is null)
            {
                Syslog.Debug("Skip delete dist data");
                return;
            }

            Connection.BulkDelete<TBDISTEntity>(s => s
                .AttachToTransaction(Transaction)
                .Where($"{nameof(TBDISTEntity.CreatedAt):C} < @expDate")
                .WithParameters(new { expDate }));
        }

        internal void DeleteExpiredStowageData()
        {
            var expDate = GetExpiredDate();
            if (expDate is null)
            {
                Syslog.Debug("Skip delete stowage data");
                return;
            }

            Connection.BulkDelete<TBSTOWAGEEntity>(s => s
                .AttachToTransaction(Transaction)
                .Where($"{nameof(TBSTOWAGEEntity.CreatedAt):C} < @expDate")
                .WithParameters(new { expDate }));
        }

        internal void DeleteSameDistData(IEnumerable<SameDistInfo> deleteDistInfos)
        {
            if (!deleteDistInfos.Any())
            {
                return;
            }

            Connection.BulkDelete<TBDISTEntity>(s => s
                .AttachToTransaction(Transaction)
                .Where($"{GetSameDistWhereSql(deleteDistInfos)}"));
        }

        internal void DeleteStowageData(IEnumerable<SameDistInfo> deleteDistInfos)
        {
            if (!deleteDistInfos.Any())
            {
                return;
            }

            Connection.BulkDelete<TBSTOWAGEEntity>(s => s
                .AttachToTransaction(Transaction)
                .Where($"{GetSameDistWhereSql(deleteDistInfos)}"));
        }

        // 納品日&バッチ一致リスト取得
        internal IEnumerable<SameDistInfo> GetDeleteSameDistDatas(IEnumerable<SameDistInfo> sameDistInfos)
        {
            return Connection.Find<TBDISTEntity>(s => s
                .AttachToTransaction(Transaction)
                .Where($"{GetSameDistWhereSql(sameDistInfos)}"))
                    .GroupBy(x => new { x.DTDELIVERY, x.CDSHUKKABATCH })
                    .Select(q => new SameDistInfo
                    {
                        DtDelivery = q.Key.DTDELIVERY,
                        ShukkaBatch = q.Key.CDSHUKKABATCH,
                        IsWork = q.Any(x => x.FGDSTATUS != (short)Status.Ready || x.FGDSTATUS != (short)Status.Ready),
                    });
        }

        internal IEnumerable<SameDistInfo> GetDeleteSameStowageDatas(IEnumerable<SameDistInfo> sameDistInfos)
        {
            return Connection.Find<TBSTOWAGEEntity>(s => s
                .AttachToTransaction(Transaction)
                .Where($"{GetSameDistWhereSql(sameDistInfos)}"))
                    .GroupBy(x => new { x.DTDELIVERY, x.CDSHUKKABATCH })
                    .Select(q => new SameDistInfo
                    {
                        DtDelivery = q.Key.DTDELIVERY,
                        ShukkaBatch = q.Key.CDSHUKKABATCH,
                        IsWork = q.Any(x => x.FGSSTATUS != (short)Status.Ready),
                    }); ;
        }

        internal string GetBulkInsertFileDirectoryPath()
        {
            return new Settings(Transaction).Get("BulkInsertFileDirectoryPath");
        }

        private string GetSameDistWhereSql(IEnumerable<SameDistInfo> sameDistInfos)
        {
            // HACK:列名を属性から取得
            return string.Join(" or ", sameDistInfos.Select(x => $"(DT_DELIVERY = {x.DtDelivery} and CD_SHUKKA_BATCH = {x.ShukkaBatch})"));
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
