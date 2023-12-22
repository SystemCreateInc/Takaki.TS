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
    public class BoxResultRepository : IExportDataRepository<TBSTOWAGEEntity>
    {
        private IEnumerable<BoxResultKey> _targetIds;

        public BoxResultRepository(IEnumerable<BoxResultKey> targetIds)
        {
            _targetIds = targetIds;
        }

        public void FixData(IDbTransaction tr, IEnumerable<long> keys)
        {
            string sql = Sql.Format<TBSTOWAGEEntity>($"update {nameof(TBSTOWAGEEntity):T} set {nameof(TBSTOWAGEEntity.DTSENDDTSTOWAGE):C}=getdate(), {nameof(TBSTOWAGEEntity.UpdatedAt):C}=getdate() where {nameof(TBSTOWAGEEntity.IDSTOWAGE):C} = @id");
            foreach (var id in keys)
            {
                tr.Connection.Execute(sql, new { id }, transaction: tr);
            }
        }

        public IEnumerable<TBSTOWAGEEntity> GetTargetData(IDbTransaction tr)
        {
            var where = string.Join(" or ", _targetIds.Select(x => $"(DT_DELIVERY='{x.DtDelivery.ToString("yyyyMMdd")}' and CD_DIST_GROUP='{x.CdDistGroup}')"));

            return tr.Connection!.Find<TBSTOWAGEEntity>(s => s
                .AttachToTransaction(tr)
                .Include<TBSTOWAGEMAPPINGEntity>(j => j.InnerJoin())
                .Where($"{where}")
                .OrderBy($"{nameof(TBSTOWAGEEntity.IDSTOWAGE):of TB_STOWAGE}"));
        }
    }
}
