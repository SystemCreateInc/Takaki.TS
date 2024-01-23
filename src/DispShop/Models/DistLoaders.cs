using ControlzEx.Standard;
using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using ImTools;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client.Extensions.Msal;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace DispShop.Models
{
    public class DistLoaders
    {
        public static IEnumerable<Dist> Get(string dt_delivdt, string cd_dist_group,string cd_block, int tdunityupe)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var sql = @"select tdunitaddrcode from tdunitaddr"
                    + " where usageid = @tdunityupe"
                    + " order by tdunitaddrcode";

                var locs = con.Query(sql, new
                {
                    @tdunityupe = tdunityupe,
                })
                     .Select(q => new Dist
                     {
                         TdUnitAddrCode = q.tdunitaddrcode,
                     }).ToList();

                sql = @"select "
                    + " TB_DIST_MAPPING.tdunitaddrcode,"
                    + " TB_DIST.DT_DELIVERY,"
                    + " TB_DIST.CD_KYOTEN,"
                    + " TB_DIST_MAPPING.CD_DIST_GROUP,"
                    + " TB_DIST_MAPPING.CD_SUM_TOKUISAKI,"
                    + " TB_DIST_MAPPING.NM_SUM_TOKUISAKI,"
                    + " TB_DIST_MAPPING.CD_SUM_COURSE,"
                    + " TB_DIST_MAPPING.CD_SUM_ROUTE,"
                    + " sum(NU_OPS) ops,"
                    + " sum(NU_DRPS) rps"
                    + " from TB_DIST"
                    + " inner join TB_DIST_MAPPING on "
                    + " TB_DIST.ID_DIST=TB_DIST_MAPPING.ID_DIST "
                    + " where TB_DIST.DT_DELIVERY = @dt_delivdt and TB_DIST_MAPPING.CD_DIST_GROUP = @cd_dist_group"
                    + " and TB_DIST.FG_MAPSTATUS=@mapstatus"
                    + " and CD_BLOCK=@cdblock"
                    + " group by "
                    + " TB_DIST_MAPPING.tdunitaddrcode,"
                    + " TB_DIST.DT_DELIVERY,"
                    + " TB_DIST.CD_KYOTEN,"
                    + " TB_DIST_MAPPING.CD_DIST_GROUP,"
                    + " TB_DIST_MAPPING.CD_SUM_TOKUISAKI,"
                    + " TB_DIST_MAPPING.NM_SUM_TOKUISAKI,"
                    + " TB_DIST_MAPPING.CD_SUM_COURSE,"
                    + " TB_DIST_MAPPING.CD_SUM_ROUTE";

                var dists = con.Query(sql, new
                {
                    @dt_delivdt = dt_delivdt,
                    @cd_dist_group = cd_dist_group,
                    @mapstatus = (int)DbLib.Defs.Status.Completed,
                    @cdblock = cd_block,
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
                         CdSumRoute = q.CD_SUM_ROUTE.ToString(),
                         Ops = q.ops,
                         Rps = q.rps,
                     }).ToList();

                sql = @"select DT_DELIVERY, CD_KYOTEN, CD_DIST_GROUP, CD_SUM_TOKUISAKI, sum(box0) box0 , sum(box1) box1,sum(box2) box2,sum(box3) box3 from "
                    + " (select DT_DELIVERY, CD_KYOTEN, CD_DIST_GROUP, CD_SUM_TOKUISAKI"
                    + ",(case ST_BOXTYPE when 0 then case when FG_SSTATUS=@completed then sum(NU_RBOXCNT) else sum(NU_OBOXCNT) end else 0 end) box0"
                    + ",(case ST_BOXTYPE when 1 then case when FG_SSTATUS=@completed then sum(NU_RBOXCNT) else sum(NU_OBOXCNT) end else 0 end) box1"
                    + ",(case ST_BOXTYPE when 2 then case when FG_SSTATUS=@completed then sum(NU_RBOXCNT) else sum(NU_OBOXCNT) end else 0 end) box2"
                    + ",(case ST_BOXTYPE when 3 then case when FG_SSTATUS=@completed then sum(NU_RBOXCNT) else sum(NU_OBOXCNT) end else 0 end) box3"
                    + " from"
                    + " TB_STOWAGE"
                    + " inner join TB_STOWAGE_MAPPING on TB_STOWAGE.ID_STOWAGE = TB_STOWAGE_MAPPING.ID_STOWAGE"
                    + " where TB_STOWAGE.DT_DELIVERY = @dt_delivdt and TB_STOWAGE_MAPPING.CD_DIST_GROUP = @cd_dist_group"
                    + " and CD_BLOCK=@cdblock"
                    + " group by DT_DELIVERY,CD_KYOTEN,CD_DIST_GROUP,CD_SUM_TOKUISAKI,ST_BOXTYPE,FG_SSTATUS"
                    + " ) STOWAGE"
                    + " group by DT_DELIVERY,CD_KYOTEN,CD_DIST_GROUP,CD_SUM_TOKUISAKI";

                var stowages =  con.Query(sql, new
                {
                    @dt_delivdt = dt_delivdt,
                    @cd_dist_group = cd_dist_group,
                    @cdblock = cd_block,
                    @completed = (int)DbLib.Defs.Status.Completed,
                })
                     .Select(q => new Dist
                     {
                         TdUnitAddrCode = q.tdunitaddrcode,
                         DtDelivery = q.DT_DELIVERY,
                         CdKyoten = q.CD_KYOTEN,
                         CdDistGroup = q.CD_DIST_GROUP,
                         CdSumTokuisaki = q.CD_SUM_TOKUISAKI,
                         NmSumTokuisaki = q.NM_SUM_TOKUISAKI,
                         Box0 = q.box0 ?? 0,
                         Box1 = q.box1 ?? 0,
                         Box2 = q.box2 ?? 0,
                         Box3 = q.box3 ?? 0,
                     }).ToList();

                foreach (var loc in locs)
                {
                    var dist = dists.Find(x => x.TdUnitAddrCode == loc.TdUnitAddrCode);
                    if (dist != null)
                    {
                        loc.DtDelivery = dist.DtDelivery;
                        loc.CdKyoten = dist.CdKyoten;
                        loc.CdDistGroup = dist.CdDistGroup;
                        loc.CdSumTokuisaki = dist.CdSumTokuisaki;
                        loc.NmSumTokuisaki = dist.NmSumTokuisaki;
                        loc.CdSumCource = dist.CdSumCource;
                        loc.CdSumRoute = dist.CdSumRoute;
                        loc.Ops = dist.Ops;
                        loc.Rps = dist.Rps;

                        var stowage = stowages.Find(x => x.CdKyoten == dist.CdKyoten
                                                && x.CdDistGroup == dist.CdDistGroup
                                                && x.CdSumTokuisaki == dist.CdSumTokuisaki);
                        if (stowage != null)
                        {
                            loc.Box0 = stowage.Box0;
                            loc.Box1 = stowage.Box1;
                            loc.Box2 = stowage.Box2;
                            loc.Box3 = stowage.Box3;
                        }
                    }

                }

                return locs;

            }
        }
    }
}
