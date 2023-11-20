using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using System.Collections.Generic;
using System.Linq;

namespace DispShop.Models
{
    public class DistLoaders
    {
        public static IEnumerable<Dist> Get(string dt_delivdt, string cd_dist_group)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var sql = @"select "
                    + " TB_DIST.tdunitaddrcode,"
                    + " TB_DIST.DT_DELIVERY,"
                    + " TB_DIST.CD_KYOTEN,"
                    + " TB_DIST.CD_DIST_GROUP,"
                    + " TB_DIST.CD_SUM_TOKUISAKI,"
                    + " NM_TOKUISAKI,"
                    + " TB_DIST.CD_COURSE,"
                    + " TB_DIST.CD_ROUTE,"
                    + " sum(NU_OPS) ops,"
                    + " sum(NU_DRPS) rps,"
                    + " sum(box0) box0,"
                    + " sum(box1) box1,"
                    + " sum(box2) box2,"
                    + " sum(box3) box3"
                    + " from TB_DIST"
                    + " left join TB_MTOKUISAKI on "
                    + " TB_DIST.CD_SUM_TOKUISAKI=TB_MTOKUISAKI.CD_TOKUISAKI "
                    + " and @dt_delivdt between TB_MTOKUISAKI.DT_TOROKU_NICHIJI and TB_MTOKUISAKI.DT_KOSHIN_NICHIJI"
                    + " left join TB_STOWAGE on "
                    + " TB_DIST.DT_DELIVERY=TB_STOWAGE.DT_DELIVERY"
                    + " and TB_DIST.CD_KYOTEN=TB_STOWAGE.CD_KYOTEN"
                    + " and TB_DIST.CD_DIST_GROUP=TB_STOWAGE.CD_DIST_GROUP"
                    + " and TB_DIST.CD_SUM_TOKUISAKI=TB_STOWAGE.CD_TOKUISAKI"
                    + " left join (select "
                    + "	ID_STOWAGE,"
                    + "	(case ST_BOXTYPE when 0 then sum(NU_OBOXCNT) else 0 end) box0,"
                    + "	(case ST_BOXTYPE when 1 then sum(NU_OBOXCNT) else 0 end) box1,"
                    + "	(case ST_BOXTYPE when 2 then sum(NU_OBOXCNT) else 0 end) box2,"
                    + "	(case ST_BOXTYPE when 3 then sum(NU_OBOXCNT) else 0 end) box3"
                    + "	from TB_STOWAGE_BOX group by ID_STOWAGE,ST_BOXTYPE) TB_STOWAGE_BOX on "
                    + " TB_STOWAGE.ID_STOWAGE=TB_STOWAGE_BOX.ID_STOWAGE"
                    + " where TB_DIST.DT_DELIVERY = @dt_delivdt and TB_DIST.CD_DIST_GROUP = @cd_dist_group"
                    + " group by "
                    + " TB_DIST.tdunitaddrcode,"
                    + " TB_DIST.DT_DELIVERY,"
                    + " TB_DIST.CD_KYOTEN,"
                    + " TB_DIST.CD_DIST_GROUP,"
                    + " TB_DIST.CD_SUM_TOKUISAKI,"
                    + " NM_TOKUISAKI,"
                    + " TB_DIST.CD_COURSE,"
                    + " TB_DIST.CD_ROUTE";

                return con.Query(sql, new
                {
                    @dt_delivdt = dt_delivdt,
                    @cd_dist_group = cd_dist_group,
                })
                     .Select(q => new Dist
                     {
                         TdUnitAddrCode = q.tdunitaddrcode,
                         DT_DELIVERY = q.DT_DELIVERY,
                         CD_KYOTEN = q.CD_KYOTEN,
                         CD_DIST_GROUP = q.CD_DIST_GROUP,
                         CD_TOKUISAKI = q.CD_SUM_TOKUISAKI,
                         NM_TOKUISAKI = q.NM_TOKUISAKI,
                         CD_COURSE = q.CD_COURSE,
                         CD_ROUTE = q.CD_ROUTE,
                         Ops = q.ops,
                         Rps = q.rps,
                         Box0 = q.box0 ?? 0,
                         Box1 = q.box1 ?? 0,
                         Box2 = q.box2 ?? 0,
                         Box3 = q.box3 ?? 0,
                     }).ToList();
            }
        }
    }
}
