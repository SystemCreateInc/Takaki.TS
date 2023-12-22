using System;
using System.Collections.Generic;
using System.Linq;
using BoxExpoter.ViewModels;
using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;

namespace BoxExpoter.Infranstructures
{
    internal class BoxExpoterQueryService
    {
        internal static IEnumerable<GroupStowage> GetGroupList(DateTime dtDelivery)
        {
            var sql = $@"select 
                CD_DIST_GROUP,
                max(NM_DIST_GROUP) NM_DIST_GROUP,
                count(distinct CD_TOKUISAKI) CustomerCount,
                count(distinct Tdunitaddrcode) SeatCount,
                count(case when FG_SSTATUS = @statusReady then 1 else null end) UncompletedCount,
                count(case when FG_SSTATUS = @statusCompleted then 1 else null end) CompletedCount,
                count(DT_SENDDT_STOWAGE) SendedCount
                from TB_STOWAGE t1
                inner join TB_STOWAGE_MAPPING t2 on t1.ID_STOWAGE = t2.ID_STOWAGE
                where DT_DELIVERY = @dtDelivery
                group by t2.CD_DIST_GROUP
                ";

            using (var con = DbFactory.CreateConnection())
            {
                return con.Query(sql, new { statusReady = Status.Ready, statusCompleted = Status.Completed, dtDelivery = dtDelivery.ToString("yyyyMMdd") })
                    .Select(x => new GroupStowage
                    (
                        x.SendedCount,
                        x.CD_DIST_GROUP,  
                        x.NM_DIST_GROUP,
                        x.CustomerCount,
                        x.SeatCount,
                        x.UncompletedCount,
                        x.CompletedCount
                    ));
            }
        }
    }
}