using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using ImTools;
using LargeDist.Models;
using LargeDist.ViewModels;
using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LargeDist.Infranstructures
{
    public class LargeDistQueryService
    {
        public static ItemProgress GetProgress(LargeDistGroup group)
        {
            var sql = @"select count(*) total, count(case when diff = 0 then 1 else null end) completed
                from 
                (
                select t1.CD_HIMBAN, sum(t1.NU_LOPS) - sum(t1.NU_LRPS) diff
                from TB_DIST t1
                inner join TB_DIST_MAPPING t2 on t1.ID_DIST = t2.ID_DIST
                where t2.CD_LARGE_GROUP=@group
                group by t1.CD_HIMBAN
                ) tt1";

            using (var con = DbFactory.CreateConnection())
            {
                var entity = con.Query(sql, new { group = group.CdLargeGroup })
                    .First();
                return new ItemProgress
                {
                    Total = entity.total,
                    Completed = entity.completed,
                };
            }
        }

        internal static IEnumerable<LargeDistItem> FindItem(LargeDistGroup group, string scancode, bool cancelMode)
        {
            Syslog.Debug($"FindItem: {group.NmLargeGroup} {scancode}");

            using (var con = DbFactory.CreateConnection())
            {
                var recs = con.Find<TBDISTEntity>(s => s
                    .Include<TBDISTMAPPINGEntity>(ss => ss.InnerJoin())
                    .Where($@"{nameof(TBDISTMAPPINGEntity.CDLARGEGROUP):of TB_DIST_MAPPING} = {nameof(group.CdLargeGroup):P}
                        and {nameof(TBDISTEntity.CDGTIN13):C} = {nameof(scancode):P} or {nameof(TBDISTEntity.CDHIMBAN):C} = {nameof(scancode):P}")
                    .WithParameters(new { group.CdLargeGroup, scancode }))
                    .Select(x => CreateDistItem(x))
                    .ToArray();

                var items = recs
                    .GroupBy(x => new LargeDistItemKey(group.CdLargeGroup, x.CdHimban, x.CdJuchuBin, x.CdShukkaBatch),
                        (key, value) => new LargeDistItem(group, value))
                    .ToArray();

                if (items.Length == 0)
                {
                    Syslog.Debug($"Not found");
                    throw new Exception("商品が見つかりませんでした");
                }

                if (cancelMode)
                {
                    // 配分が始まった商品はキャンセルできない
                    if (items.Any(x => x.DistStatus != Status.Ready))
                    {
                        Syslog.Debug($"already processed picking");
                        throw new Exception("仕分済みのため取り消しできません");
                    }

                    // キャンセルモードは実績のあるもの
                    items = items.Where(x => x.Result.Total > 0).ToArray();

                    if (items.Length == 0)
                    {
                        Syslog.Debug($"all item not completed");
                        throw new Exception("未処理の商品です");
                    }
                }
                else
                {
                    // 配分モードは残りのあるもの
                    items = items.Where(x => x.Remain.Total > 0).ToArray();

                    if (items.Length == 0)
                    {
                        Syslog.Debug($"all item completed");
                        throw new Exception("大仕分は終了しています");
                    }
                }

                return items;
            }
        }

        internal static IEnumerable<LargeDistItem> GetItemsByLargeDist(LargeDistGroup group, bool uncompletedOnly)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var extraWhere = uncompletedOnly ? " and NU_LRPS=0" : " and 0=0";

                var recs = con.Find<TBDISTEntity>(s => s
                    .Include<TBDISTMAPPINGEntity>(ss => ss.InnerJoin())
                    .Where($@"{nameof(TBDISTMAPPINGEntity.CDLARGEGROUP):of TB_DIST_MAPPING}={nameof(group.CdLargeGroup):P} {extraWhere}")
                    .WithParameters(new { group.CdLargeGroup }))
                    .Select(x => CreateDistItem(x))
                    .ToArray();

                return recs
                    .GroupBy(x => new LargeDistItemKey(group.CdLargeGroup, x.CdHimban, x.CdJuchuBin, x.CdShukkaBatch))
                    .Select(x => new LargeDistItem(group, x))
                    .OrderBy(x => x.Status)
                    .ThenBy(x => x.DistStatus)
                    .ThenBy(x => x.CdHimban)
                    .ToArray();
            }
        }

        private static DistItem CreateDistItem(TBDISTEntity entity)
        {
            var mapping = entity.TBDISTMAPPING!.First();
            return new DistItem(entity.IDDIST,
                entity.DTDELIVERY,
                entity.CDGTIN13,
                entity.CDHIMBAN,
                entity.CDJUCHUBIN,
                entity.CDCOURSE,
                entity.CDROUTE,
                mapping.NMHINSEISHIKIMEI,
                mapping.Tdunitaddrcode,
                entity.STBOXTYPE,
                entity.NUBOXUNIT,
                entity.NULOPS,
                entity.NULRPS,
                entity.NUDOPS,
                entity.NUDRPS,
                entity.FGDSTATUS,
                entity.CDTOKUISAKI,
                mapping.NMTOKUISAKI,
                mapping.CDBLOCK,
                mapping.CDDISTGROUP,
                mapping.NMDISTGROUP,
                entity.CDSHUKKABATCH,
                mapping.NMSHUKKABATCH);
        }
    }
}