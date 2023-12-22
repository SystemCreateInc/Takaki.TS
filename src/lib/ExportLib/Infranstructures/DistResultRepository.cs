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
            //  fixme: 抽出条件を記述
            var where = "";

            return tr.Connection!.Find<TBDISTEntity>(s => s
                .AttachToTransaction(tr)
                .Include<TBDISTMAPPINGEntity>(j => j.InnerJoin())
                .Where($"{where}")
                .OrderBy($"{nameof(TBDISTEntity.IDDIST):of TB_DIST}"));
        }
    }
}
