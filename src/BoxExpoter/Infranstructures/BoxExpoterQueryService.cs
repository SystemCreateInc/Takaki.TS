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
                var sql = $@"select 
                    CD_DIST_GROUP,
                    max(NM_DIST_GROUP) NM_DIST_GROUP,
                    count(distinct case when tdunitaddrcode='' then null else CD_TOKUISAKI end) CustomerCount,
                    count(distinct case when tdunitaddrcode='' then null else CD_BLOCK+tdunitaddrcode end) SeatCount,
                    count(distinct case when FG_SSTATUS = @statusReady then CD_BLOCK+tdunitaddrcode else null end) UncompletedCount,
                    count(distinct case when FG_SSTATUS = @statusCompleted then CD_BLOCK+tdunitaddrcode else null end) CompletedCount,
                    count(distinct case when DT_SENDDT_STOWAGE is null then null else CD_BLOCK+tdunitaddrcode end) SendedCount,
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
                        x.SeatCount,
                        x.UncompletedCount,
                        x.CompletedCount,
                        x.OverCount
                    ));
            }
        }
    }
}