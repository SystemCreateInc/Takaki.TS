using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BoxExpoter.ViewModels;
using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using DbLib.Extensions;
using DryIoc;
using ImTools;

namespace BoxExpoter.Infranstructures
{
    internal class BoxExpoterQueryService
    {
        internal static IEnumerable<GroupStowage> GetGroupList(DateTime dtDelivery)
        {
            using (var con = DbFactory.CreateConnection())
            {
                // 座席数取得
                var sql = $@"select DT_DELIVERY, CD_DIST_GROUP, sum(NU_MAGICHI) SeatCount from"
                        + "(select distinct DT_DELIVERY, CD_DIST_GROUP, CD_BLOCK, Tdunitaddrcode, NU_MAGICHI from TB_STOWAGE"
                        + " inner join TB_STOWAGE_MAPPING on TB_STOWAGE.ID_STOWAGE = TB_STOWAGE_MAPPING.ID_STOWAGE"
                        + " where DT_DELIVERY >= @dtDelivery"
                        + " )t1"
                        + " group by DT_DELIVERY, CD_DIST_GROUP";

                var r2 = con.Query(sql, new { dtDelivery = dtDelivery.ToString("yyyyMMdd") })
                    .Select(x => new GroupStowage
                    (
                       0,
                        x.DT_DELIVERY,
                        x.CD_DIST_GROUP,
                        "",
                        0,
                        x.SeatCount,
                        0,
                        0,
                        0
                    ))
                    .ToArray();

                sql = $@"select 
                    DT_DELIVERY,
                    CD_DIST_GROUP,
                    max(NM_DIST_GROUP) NM_DIST_GROUP,
                    count(distinct case when tdunitaddrcode='' then null else CD_TOKUISAKI end) CustomerCount,
                    0 SeatCount,
                    count(case when FG_SSTATUS = @statusReady and (0<NU_OBOXCNT or 0<NU_RBOXCNT) then 1 else null end) UncompletedCount,
                    count(case when FG_SSTATUS = @statusCompleted and (0<NU_OBOXCNT or 0<NU_RBOXCNT) then 1 else null end) CompletedCount,
                    count(DT_SENDDT_STOWAGE) SendedCount,
                    count(distinct case when tdunitaddrcode='' then CD_TOKUISAKI else null end) OverCount
                    from TB_STOWAGE t1
                    inner join TB_STOWAGE_MAPPING t2 on t1.ID_STOWAGE = t2.ID_STOWAGE
                    where DT_DELIVERY >= @dtDelivery
                    group by t1.DT_DELIVERY, t2.CD_DIST_GROUP
                    order by (case when count(DT_SENDDT_STOWAGE)=0 then 0 else 1 end),t1.DT_DELIVERY, t2.CD_DIST_GROUP
                ";

                return con.Query(sql, new { statusReady = Status.Ready, statusCompleted = Status.Completed, dtDelivery = dtDelivery.ToString("yyyyMMdd") })
                    .Select(x => new GroupStowage
                    (
                        x.SendedCount,
                        x.DT_DELIVERY,
                        x.CD_DIST_GROUP,
                        x.NM_DIST_GROUP,
                        x.CustomerCount,
                        GetSeatCount(r2, x.DT_DELIVERY, x.CD_DIST_GROUP),
                        x.UncompletedCount,
                        x.CompletedCount,
                        x.OverCount
                    ))
                    .Where(x => !x.IsSended || x.DtDelivery >= DateTime.Today)
                    .ToArray();
            }

        }

        private static int GetSeatCount(IEnumerable<GroupStowage> r2, string dtDelivery, string cdDistGroup)
        {
            return r2.FirstOrDefault(s => s.DtDelivery == dtDelivery.ParseNonSeparatedDate(DateTime.Today) && s.CdDistGroup == cdDistGroup)?.SeatCount ?? 0;
        }
    }
}