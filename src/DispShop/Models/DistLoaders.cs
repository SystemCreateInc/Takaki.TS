using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using ImTools;
using Microsoft.Identity.Client.Extensions.Msal;
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
                    + " TB_DIST_MAPPING.NM_SUM_TOKUISAKI,"
                    + " TB_DIST_MAPPING.CD_SUM_COURSE,"
                    + " TB_DIST_MAPPING.CD_SUM_ROUTE,"
                    + " sum(NU_OPS) ops,"
                    + " sum(NU_DRPS) rps,"
                    + " sum(box0) box0,"
                    + " sum(box1) box1,"
                    + " sum(box2) box2,"
                    + " sum(box3) box3"
                    + " from TB_DIST"
                    + " inner join TB_DIST_MAPPING on "
                    + " TB_DIST.ID_DIST=TB_DIST_MAPPING.ID_DIST "
                    + "left join"
                    + " (select DT_DELIVERY, CD_KYOTEN, CD_DIST_GROUP, CD_SUM_TOKUISAKI"
                    + ",(case ST_BOXTYPE when 0 then sum(NU_OBOXCNT) else 0 end) box0"
                    + ",(case ST_BOXTYPE when 1 then sum(NU_OBOXCNT) else 0 end) box1"
                    + ",(case ST_BOXTYPE when 2 then sum(NU_OBOXCNT) else 0 end) box2"
                    + ",(case ST_BOXTYPE when 3 then sum(NU_OBOXCNT) else 0 end) box3"
                    + " from"
                    + " TB_STOWAGE"
                    + " inner join TB_STOWAGE_MAPPING on TB_STOWAGE.ID_STOWAGE = TB_STOWAGE_MAPPING.ID_STOWAGE"
                    + " group by DT_DELIVERY,CD_KYOTEN,CD_DIST_GROUP,CD_SUM_TOKUISAKI,ST_BOXTYPE"
                    + ") STOWAGE on TB_DIST.DT_DELIVERY = STOWAGE.DT_DELIVERY and TB_DIST.CD_KYOTEN = STOWAGE.CD_KYOTEN"
                    + " and TB_DIST_MAPPING.CD_DIST_GROUP = STOWAGE.CD_DIST_GROUP and TB_DIST_MAPPING.CD_SUM_TOKUISAKI = STOWAGE.CD_SUM_TOKUISAKI"
                    + " where TB_DIST.DT_DELIVERY = @dt_delivdt and TB_DIST_MAPPING.CD_DIST_GROUP = @cd_dist_group"
                    + " group by "
                    + " TB_DIST_MAPPING.tdunitaddrcode,"
                    + " TB_DIST.DT_DELIVERY,"
                    + " TB_DIST.CD_KYOTEN,"
                    + " TB_DIST_MAPPING.CD_DIST_GROUP,"
                    + " TB_DIST_MAPPING.CD_SUM_TOKUISAKI,"
                    + " TB_DIST_MAPPING.NM_SUM_TOKUISAKI,"
                    + " TB_DIST_MAPPING.CD_SUM_COURSE,"
                    + " TB_DIST_MAPPING.CD_SUM_ROUTE";

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
                         CdSumTokuisaki = q.CD_SUM_TOKUISAKI,
                         NmSumTokuisaki = q.NM_SUM_TOKUISAKI,
                         CdSumCource = q.CD_SUM_COURSE,
                         CdSumRoute = q.CD_SUM_ROUTE,
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
