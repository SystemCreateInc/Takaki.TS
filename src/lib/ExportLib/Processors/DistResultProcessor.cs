using DbLib.DbEntities;
using DbLib.Defs;
using ExportLib.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportLib.Processors
{
    public class DistResultProcessor : ExportProcessor
    {
        private readonly IExportDataRepository<TBDISTEntity> _repository;

        public DistResultProcessor(IExportDataRepository<TBDISTEntity> repo)
            : base(DataType.HakoResult, AppLockKeys.DIST_RESULT)
        {
            _repository = repo;
        }

        // 送信可能件数取得
        public override void UpdateAvailableExportCount(IDbTransaction tr)
        {
            AvailableExportCount = 0;
        }

        // 送信前処理
        public override void PreExport(ExportContext ctxt)
        {
            _exportedIds.Clear();
        }

        // 送信データ書き込み
        public override void ExportData(ExportContext ctxt)
        {
            var recs = _repository.GetTargetData(ctxt.Transaction);

            foreach (var rec in recs)
            {
                ctxt.CancellationToken.ThrowIfCancellationRequested();

                var map = rec.TBDISTMAPPING!.First();

                var list = new[]
                {
                    rec.DTDELIVERY,
                    rec.CDSHUKKABATCH,
                    rec.CDKYOTEN,
                    map.CDBLOCK,
                    map.Tdunitaddrcode,
                    rec.CDHAISHOBIN,
                    rec.CDCOURSE,
                    rec.CDROUTE.ToString(),
                    rec.CDJUCHUBIN,
                    rec.CDTOKUISAKI,
                    rec.CDHIMBAN,
                    rec.CDGTIN13,
                    rec.CDGTIN14,
                    rec.NUDOPS.ToString(),
                    rec.NUDRPS.ToString(),
                    rec.DTTOROKUNICHIJI,
                    rec.DTKOSHINNICHIJI,
                    rec.CDHENKOSHA,
                };

                ctxt.StreamWriter.Write(string.Join(FieldSeparator, list) + LineSeparator);
                _exportedIds.Add(rec.IDDIST);
                ++ctxt.ExportedCount;

                if ((ctxt.ExportedCount % 33) == 0)
                {
                    ctxt.Service.NotifyProgress("送信中", ctxt.ExportedCount, recs.Count(), 0);
                }
            }
        }

        // 送信後処理
        // send_waitatをnullにして送信済み時刻をセットする
        public override void PostExport(ExportContext ctxt)
        {
            _repository.FixData(ctxt.Transaction, _exportedIds);
        }

        private readonly IList<long> _exportedIds = new List<long>();
    }
}
