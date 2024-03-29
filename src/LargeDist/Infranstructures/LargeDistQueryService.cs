﻿using Dapper;
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
using System.Transactions;

namespace LargeDist.Infranstructures
{
    public class LargeDistQueryService
    {
        public static ItemProgress GetProgress(DateTime dtDelivery, LargeDistGroup group)
        {
            var sql = @"select count(*) total, count(case when diff = 0 then 1 else null end) completed
                from 
                (
                select t1.CD_HIMBAN, sum(t1.NU_LOPS) - sum(t1.NU_LRPS) diff
                from TB_DIST t1
                inner join TB_DIST_MAPPING t2 on t1.ID_DIST = t2.ID_DIST
                where t1.DT_DELIVERY=@dtDelivery and t2.CD_LARGE_GROUP=@group
                group by t1.CD_HIMBAN
                ) tt1";

            using var con = DbFactory.CreateConnection();
            var entity = con.Query(sql, new { group = group.CdLargeGroup, dtDelivery = dtDelivery.ToString("yyyyMMdd") })
                .First();
            return new ItemProgress
            {
                Total = entity.total,
                Completed = entity.completed,
            };
        }

        internal static IEnumerable<LargeDistItem> FindItem(DateTime dtDelivery, LargeDistGroup group, string scancode, bool cancelMode)
        {
            Syslog.Debug($"FindItem: {group.NmLargeGroup} {scancode}");

            if (scancode.Length < 9)
            {
                scancode = scancode.PadLeft(9, '0');
            }

            using var con = DbFactory.CreateConnection();
            var recs = con.Find<TBDISTEntity>(s => s
                .Include<TBDISTMAPPINGEntity>(ss => ss.InnerJoin())
                .Where($@"{nameof(TBDISTEntity.DTDELIVERY):C} = @dtDelivery
                        and {nameof(TBDISTMAPPINGEntity.CDLARGEGROUP):of TB_DIST_MAPPING} = {nameof(group.CdLargeGroup):P}
                        and ({nameof(TBDISTEntity.CDGTIN13):C} = {nameof(scancode):P} or {nameof(TBDISTEntity.CDHIMBAN):C} = {nameof(scancode):P})")
                .WithParameters(new { dtDelivery = dtDelivery.ToString("yyyyMMdd"), group.CdLargeGroup, scancode }))
                .Select(x => CreateDistItem(x))
                .ToArray();

            if (recs.Length == 0)
            {
                Syslog.Debug($"Not found");
                throw new Exception("商品が見つかりませんでした");
            }

            if (cancelMode)
            {
                // 配分が始まった商品はキャンセルできない
                if (recs.Any(x => x.FgDStatus != Status.Ready))
                {
                    Syslog.Debug($"already processed picking");
                    throw new Exception("仕分済みのため取り消しできません");
                }

                // キャンセルモードは実績のあるもの
                var items = recs
                    .Where(x => x.ResultPiece > 0)
                    .GroupBy(x => new LargeDistItemKey(group.CdLargeGroup, x.CdHimban),
                        (key, value) => new LargeDistItem(group, value))
                    .ToArray();

                if (items.Length == 0)
                {
                    Syslog.Debug($"all item not completed");
                    throw new Exception("未処理の商品です");
                }

                return items;
            }
            else
            {
                // 配分モードは残りのあるもの
                var items = recs
                    .Where(x => x.RemainPiece > 0)
                    .GroupBy(x => new LargeDistItemKey(group.CdLargeGroup, x.CdHimban),
                        (key, value) => new LargeDistItem(group, value))
                    .ToArray();

                items = items.Where(x => x.Remain.Total > 0).ToArray();

                if (items.Length == 0)
                {
                    Syslog.Debug($"all item completed");
                    throw new Exception("大仕分は終了しています");
                }

                return items;
            }
        }

        internal static IEnumerable<LargeDistItem> GetItemsByLargeDist(DateTime dtDelivery, LargeDistGroup group, bool uncompletedOnly)
        {
            using var con = DbFactory.CreateConnection();
            var extraWhere = uncompletedOnly ? " and NU_LRPS=0" : " and 0=0";

            var recs = con.Find<TBDISTEntity>(s => s
                .Include<TBDISTMAPPINGEntity>(ss => ss.InnerJoin())
                .Where($@"{nameof(TBDISTEntity.DTDELIVERY):C} = @dtDelivery and  {nameof(TBDISTMAPPINGEntity.CDLARGEGROUP):of TB_DIST_MAPPING}={nameof(group.CdLargeGroup):P} {extraWhere}")
                .WithParameters(new { dtDelivery = dtDelivery.ToString("yyyyMMdd"), group.CdLargeGroup }))
                .Select(x => CreateDistItem(x))
                .ToArray();

            return recs
                .GroupBy(x => new LargeDistItemKey(group.CdLargeGroup, x.CdHimban))
                .Select(x => new LargeDistItem(group, x))
                .OrderBy(x => x.Status)
                .ThenBy(x => x.DistStatus)
                .ThenBy(x => x.CdHimban)
                .ToArray();
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
        public static void UpdateBoxUnit(string cdhimban, int boxunit, DateTime dtDelivery, LargeDistGroup group)
        {
            using var con = DbFactory.CreateConnection();
            var sql = $@"update TB_DIST 
                set NU_BOXUNIT=@boxunit, TB_DIST.updatedAt=getdate()
                from TB_DIST inner join TB_DIST_MAPPING on TB_DIST.ID_DIST=TB_DIST_MAPPING.ID_DIST
                where DT_DELIVERY=@dtdelivery
                and CD_LARGE_GROUP=@largegroup
                and CD_HIMBAN=@cdhimban";

            con.Execute(sql, new
            {
                largegroup = group.CdLargeGroup,
                dtdelivery = dtDelivery.ToString("yyyyMMdd"),
                cdhimban,
                boxunit,
            });
        }
    }
}