using Dapper;
using Dapper.FastCrud;
using DbLib.DbEntities;
using ExportLib.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportLib.Infranstructures
{
    public class DistResultRepository : IExportDataRepository<TBDISTEntity>
    {
        private IEnumerable<DistResultKey> _targetIds;

        public DistResultRepository(IEnumerable<DistResultKey> targetIds)
        {
            _targetIds = targetIds;
        }

        public void FixData(IDbTransaction tr, IEnumerable<long> keys)
        {
            string sql = Sql.Format<TBDISTEntity>($"update {nameof(TBDISTEntity):T} set {nameof(TBDISTEntity.DTSENDDTDIST):C}=getdate(), {nameof(TBDISTEntity.UpdatedAt):C}=getdate() where {nameof(TBDISTEntity.IDDIST):C} = @id");
            foreach (var id in keys)
            {
                tr.Connection.Execute(sql, new { id }, transaction: tr);
            }
        }

        public IEnumerable<TBDISTEntity> GetTargetData(IDbTransaction tr)
        {
            var where = string.Join(" or ", _targetIds.Select(x => $@"(DT_DELIVERY='{x.DtDelivery.ToString("yyyyMMdd")}' 
                and CD_DIST_GROUP='{x.CdDistGroup}')"));

            return tr.Connection!.Find<TBDISTEntity>(s => s
                .AttachToTransaction(tr)
                .Include<TBDISTMAPPINGEntity>(j => j.InnerJoin())
                .Where($"{where}")
                .OrderBy($@"{nameof(TBSTOWAGEEntity.DTDELIVERY):of TB_DIST}, 
                    {nameof(TBDISTEntity.CDJUCHUBIN):of TB_DIST},
                    {nameof(TBDISTEntity.CDSHUKKABATCH):of TB_DIST},
                    {nameof(TBDISTEntity.CDKYOTEN):of TB_DIST},
                    {nameof(TBDISTEntity.CDHAISHOBIN):of TB_DIST},
                    {nameof(TBDISTEntity.CDCOURSE):of TB_DIST},
                    {nameof(TBDISTEntity.CDROUTE):of TB_DIST},
                    {nameof(TBDISTEntity.CDTOKUISAKI):of TB_DIST}"));
        }
    }
}
