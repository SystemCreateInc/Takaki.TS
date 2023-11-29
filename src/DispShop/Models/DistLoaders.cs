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
                    + " TB_DIST_MAPPING.tdunitaddrcode,"
                    + " TB_DIST.DT_DELIVERY,"
                    + " TB_DIST.CD_KYOTEN,"
                    + " TB_DIST_MAPPING.CD_DIST_GROUP,"
                    + " TB_DIST_MAPPING.CD_SUM_TOKUISAKI,"
                    + " TB_DIST_MAPPING.NM_TOKUISAKI,"
                    + " TB_DIST.CD_COURSE,"
                    + " TB_DIST.CD_ROUTE,"
                    + " sum(NU_OPS) ops,"
                    + " sum(NU_DRPS) rps,"
                    + " sum(box0) box0,"
                    + " sum(box1) box1,"
                    + " sum(box2) box2,"
                    + " sum(box3) box3"
                    + " from TB_DIST"
                    + " inner join TB_DIST_MAPPING on "
                    + " TB_DIST.ID_DIST=TB_DIST_MAPPING.ID_DIST "
                    + " left join TB_STOWAGE on "
                    + " TB_DIST.DT_DELIVERY=TB_STOWAGE.DT_DELIVERY"
                    + " and TB_DIST.CD_KYOTEN=TB_STOWAGE.CD_KYOTEN"
                    + " inner join TB_STOWAGE_MAPPING on TB_STOWAGE.ID_STOWAGE=TB_STOWAGE_MAPPING.ID_STOWAGE"
                    + " and TB_DIST_MAPPING.CD_DIST_GROUP=TB_STOWAGE_MAPPING.CD_DIST_GROUP"
                    + " and TB_DIST_MAPPING.CD_SUM_TOKUISAKI=TB_STOWAGE_MAPPING.CD_SUM_TOKUISAKI"
                    + " left join (select "
                    + "	ID_STOWAGE,"
                    + "	(case ST_BOXTYPE when 0 then sum(NU_OBOXCNT) else 0 end) box0,"
                    + "	(case ST_BOXTYPE when 1 then sum(NU_OBOXCNT) else 0 end) box1,"
                    + "	(case ST_BOXTYPE when 2 then sum(NU_OBOXCNT) else 0 end) box2,"
                    + "	(case ST_BOXTYPE when 3 then sum(NU_OBOXCNT) else 0 end) box3"
                    + "	from TB_STOWAGE_BOX group by ID_STOWAGE,ST_BOXTYPE) TB_STOWAGE_BOX on "
                    + " TB_STOWAGE.ID_STOWAGE=TB_STOWAGE_BOX.ID_STOWAGE"
                    + " where TB_DIST.DT_DELIVERY = @dt_delivdt and TB_DIST_MAPPING.CD_DIST_GROUP = @cd_dist_group"
                    + " group by "
                    + " TB_DIST_MAPPING.tdunitaddrcode,"
                    + " TB_DIST.DT_DELIVERY,"
                    + " TB_DIST.CD_KYOTEN,"
                    + " TB_DIST_MAPPING.CD_DIST_GROUP,"
                    + " TB_DIST_MAPPING.CD_SUM_TOKUISAKI,"
                    + " TB_DIST_MAPPING.NM_TOKUISAKI,"
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
                         DtDelivery = q.DT_DELIVERY,
                         CdKyoten = q.CD_KYOTEN,
                         CdDistGroup = q.CD_DIST_GROUP,
                         CdTokuisaki = q.CD_SUM_TOKUISAKI,
                         NmTokuisaki = q.NM_TOKUISAKI,
                         CdCource = q.CD_COURSE,
                         CdRoute = q.CD_ROUTE,
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
