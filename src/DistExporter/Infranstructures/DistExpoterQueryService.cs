using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DistExpoter.ViewModels;
using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using DryIoc;
using ImTools;

namespace DistExpoter.Infranstructures
{
    internal class DistExpoterQueryService
    {
        internal static IEnumerable<GroupDist> GetGroupList()
        {
            using (var con = DbFactory.CreateConnection())
            {
                var sql = $@"select 
                    DT_DELIVERY,
                    CD_DIST_GROUP,
                    max(NM_DIST_GROUP) NM_DIST_GROUP,
                    count(distinct case when tdunitaddrcode='' then null else CD_TOKUISAKI end) CustomerCount,
                    count(distinct case when tdunitaddrcode='' then null else CD_BLOCK+tdunitaddrcode end) SeatCount,
                    count(case when FG_MAPSTATUS = @statusReady then CD_BLOCK+tdunitaddrcode else null end) UncompletedCount,
                    count(case when FG_MAPSTATUS = @statusCompleted then CD_BLOCK+tdunitaddrcode else null end) CompletedCount,
                    count(case when DT_SENDDT_DIST is not null then null else CD_TOKUISAKI end) SendCount,
                    count(case when DT_SENDDT_DIST is null then null else CD_TOKUISAKI end) SendedCount,
                    count(distinct case when tdunitaddrcode='' then CD_TOKUISAKI else null end) OverCount
                    from TB_DIST t1
                    inner join TB_DIST_MAPPING t2 on t1.ID_DIST = t2.ID_DIST
                    group by t1.DT_DELIVERY, t2.CD_DIST_GROUP
                    order by t1.DT_DELIVERY, t2.CD_DIST_GROUP
                ";

                return con.Query(sql, new { statusReady = Status.Ready, statusCompleted = Status.Completed })
                    .Select(x => new GroupDist
                    (
                        x.SendCount,
                        x.SendedCount,
                        x.DT_DELIVERY,
                        x.CD_DIST_GROUP,  
                        x.NM_DIST_GROUP,
                        x.CustomerCount,
                        x.SeatCount,
                        x.UncompletedCount,
                        x.CompletedCount,
                        x.OverCount
                    ))
                    .Where(x => !x.IsSended || x.DtDelivery >= DateTime.Today)
                    .ToArray();
            }
        }
    }
}