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
                var sql = $@"select CD_DIST_GROUP, sum(NU_MAGICHI) SeatCount from"
                        + "(select distinct CD_DIST_GROUP, CD_BLOCK, Tdunitaddrcode, NU_MAGICHI from TB_STOWAGE"
                        + " inner join TB_STOWAGE_MAPPING on TB_STOWAGE.ID_STOWAGE = TB_STOWAGE_MAPPING.ID_STOWAGE"
                        + " where DT_DELIVERY = @dtDelivery"
                        + " )t1"
                        + " group by CD_DIST_GROUP";

                var r2 = con.Query(sql, new { dtDelivery = dtDelivery.ToString("yyyyMMdd") })
                    .Select(x => new GroupStowage
                    (
                       0,
                        x.CD_DIST_GROUP,
                        "",
                        0,
                        x.SeatCount,
                        0,
                        0,
                        0
                    ));

                sql = $@"select 
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
                    where DT_DELIVERY = @dtDelivery
                    group by t2.CD_DIST_GROUP
                ";

                return con.Query(sql, new { statusReady = Status.Ready, statusCompleted = Status.Completed, dtDelivery = dtDelivery.ToString("yyyyMMdd") })
                    .Select(x => new GroupStowage
                    (
                        x.SendedCount,
                        x.CD_DIST_GROUP,
                        x.NM_DIST_GROUP,
                        x.CustomerCount,
                        (r2.FirstOrDefault(s => s.CdDistGroup == x.CD_DIST_GROUP) ?? new GroupStowage(0, "", "", 0, 0, 0, 0,0)).SeatCount,
                        x.UncompletedCount,
                        x.CompletedCount,
                        x.OverCount
                    ));
            }

        }
    }
}