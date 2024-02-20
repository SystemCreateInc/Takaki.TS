using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;

namespace DistProg.Models
{
    public class DistProgLoader
    {
        public static IEnumerable<DistProg> Get(string dtDelivery)
        {
            // TB_PCを親にしてTB_DIST_GROUP_PROGRESSの最新の一行を表示する
            var sql = "select "
                + "TB_PC.ID_PC, "
                + "v1.CD_BLOCK, "
                + "v1.NM_SHAIN, "
                + "v1.CD_DIST_GROUP, "
                + "v1.NM_DIST_GROUP, "
                + "v1.DT_DELIVERY, "
                + "v1.DT_START, "
                + "v1.DT_END, "
                + "v1.NU_RITEMCNT, "
                + "v1.NU_OITEMCNT, "
                + "v1.NU_RPS, "
                + "v1.NU_OPS "
                + "from TB_PC(nolock) "
                + "left join("
                    + "select * from(select ID_PC, CD_BLOCK, NM_SHAIN, CD_DIST_GROUP, NM_DIST_GROUP, DT_DELIVERY, DT_START, DT_END, NU_RITEMCNT, NU_OITEMCNT, NU_RPS, NU_OPS, row_number() over(partition by ID_PC order by updatedAt desc) no "
                    + "from TB_DIST_GROUP_PROGRESS(nolock) "
                    + "where DT_DELIVERY=@dtDelivery) t1 where no = 1) v1 "
                + "on TB_PC.ID_PC = v1.ID_PC "
                + "where TB_PC.ID_PC<>0";

            using (var con = DbFactory.CreateConnection())
            {
                return con.Query(sql, new { dtDelivery })
                    .Select(q => new DistProg
                    {
                        IdPc = q.ID_PC,
                        CdBlock = q.CD_BLOCK,
                        NmShain = q.NM_SHAIN,
                        CdDistGroup = q.CD_DIST_GROUP,
                        NmDistGroup = q.NM_DIST_GROUP,
                        DtDelivery = q.DT_DELIVERY ?? string.Empty,
                        DtStart = q.DT_START,
                        DtEnd = q.DT_END,
                        NuRitemcnt = q.NU_RITEMCNT,
                        NuOitemcnt = q.NU_OITEMCNT,
                        NuRps = q.NU_RPS,
                        NuOps = q.NU_OPS,
                    })
                    .OrderBy(x => x.IdPc)
                    .ThenBy(x => x.CdBlock);
            }
        }

        // 仕分未完了リスト取得
        public static IEnumerable<DistProg> GetUncompleteds(string dtDelivery)
        {
            var completed = Status.Completed;
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBDISTGROUPPROGRESSEntity>(s => s
                .Where(@$"{nameof(TBDISTGROUPPROGRESSEntity.DTDELIVERY):C}={nameof(dtDelivery):P}
                            and {nameof(TBDISTGROUPPROGRESSEntity.FGDSTATUS):C}<{nameof(completed):P}")
                .WithParameters(new { dtDelivery, completed }))
                    .Select(s => new DistProg
                    {
                        CdBlock = s.CDBLOCK,
                        NmShain = s.NMSHAIN,
                        CdDistGroup = s.CDDISTGROUP,
                        NmDistGroup = s.NMDISTGROUP,
                        DtDelivery = s.DTDELIVERY,
                        DtStart = s.DTSTART,
                        DtEnd = s.DTEND,
                        NuRitemcnt = s.NURITEMCNT,
                        NuOitemcnt = s.NUOITEMCNT,
                        NuRps = s.NURPS,
                        NuOps = s.NUOPS,
                    })
                    .OrderBy(x => x.CdBlock)
                    .ThenBy(x => x.CdDistGroup);
            }
        }

        public static IEnumerable<DistProg> GetCompleteds(string dtDelivery)
        {
            var completed = Status.Completed;
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBDISTGROUPPROGRESSEntity>(s => s
                .Where(@$"{nameof(TBDISTGROUPPROGRESSEntity.DTDELIVERY):C}={nameof(dtDelivery):P}
                            and {nameof(TBDISTGROUPPROGRESSEntity.FGDSTATUS):C}={nameof(completed):P}")
                .WithParameters(new { dtDelivery, completed }))
                    .Select(s => new DistProg
                    {
                        IdPc = s.IDPC,
                        CdBlock = s.CDBLOCK,
                        NmShain = s.NMSHAIN,
                        CdDistGroup = s.CDDISTGROUP,
                        NmDistGroup = s.NMDISTGROUP,
                        DtDelivery = s.DTDELIVERY,
                        DtStart = s.DTSTART,
                        DtEnd = s.DTEND,
                        NuRitemcnt = s.NURITEMCNT,
                        NuOitemcnt = s.NUOITEMCNT,
                        NuRps = s.NURPS,
                        NuOps = s.NUOPS,
                    })
                    .OrderBy(x => x.IdPc)
                    .ThenBy(x => x.CdBlock)
                    .ThenBy(x => x.CdDistGroup);
            }
        }
    }
}
