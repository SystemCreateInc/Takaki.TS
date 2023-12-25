using ControlzEx.Standard;
using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using System.Runtime.Intrinsics.X86;
using System.Windows.Controls;

namespace Mapping.Models
{
    public class DistGroupInfoLoader
    {
        // 仕分けグループ一覧
        public static IEnumerable<DistGroupInfo> Get(string DT_DELIVERY)
        {
            using (var con = DbFactory.CreateConnection())
            {
                // 仕分グループ名称取得
                var r = con.Find<TBDISTGROUPEntity>(s => s
                    .Where($"{nameof(TBDISTGROUPEntity.DTTEKIYOKAISHI):C} <= @DT_DELIVERY and @DT_DELIVERY < {nameof(TBDISTGROUPEntity.DTTEKIYOMUKO):C}")
                    .WithParameters(new { DT_DELIVERY })
                    .OrderBy($"{nameof(TBDISTGROUPEntity.CDDISTGROUP)}"))
                    .Select((x, index) => new DistGroupInfo
                    {
                        DtDelivdt = DT_DELIVERY,
                        CdDistGroup = x.CDDISTGROUP,
                        NmDistGroup = x.NMDISTGROUP,
                    }).ToList();

                if (r.Count()!=0)
                {
                    foreach (var p in r)
                    {
                        // 各仕分グループ件数取得
                        var sql = "select CD_DIST_GROUP"
                            + ", count(distinct CD_BLOCK+tdunitaddrcode) shopcnt"
                            + ",count(distinct case when FG_MAPSTATUS = 1 then CD_TOKUISAKI else null end) overshopcnt"
                            + " from TB_DIST"
                            + " inner join TB_DIST_MAPPING on TB_DIST_MAPPING.ID_DIST = TB_DIST.ID_DIST"
                            + " where DT_DELIVERY = @dtdelivdt"
                            + " and  CD_DIST_GROUP = @cddistgroup"
                            + " group by CD_DIST_GROUP";

                        var result = con.Query(sql, new
                        {
                            @dtdelivdt = DT_DELIVERY,
                            @cddistgroup = p.CdDistGroup,
                        })
                             .Select(q => new DistGroupInfo
                             {
                                 OverShopCnt = q.overshopcnt ?? 0,
                                 ShopCnt = q.shopcnt ?? 0,
                             }).FirstOrDefault();
                        if (result != null)
                        {
                            p.Select = false;
                            p.OverShopCnt = result.OverShopCnt;
                            p.ShopCnt = result.ShopCnt;
                        }

                        sql = "select sum(NU_MAGUCHI) maguchi"
                                    + " from(select"
                                    + " max(NU_MAGICHI) NU_MAGUCHI"
                                    + ",(case when max(FG_LSTATUS) <> min(FG_LSTATUS) then @status_inprog else max(FG_LSTATUS) end) lstatus"
                                    + ",(case when max(FG_DSTATUS)<> min(FG_DSTATUS) then @status_inprog else max(FG_DSTATUS) end) dstatus"
                                    + " from TB_DIST"
                                    + " inner join TB_DIST_MAPPING on TB_DIST_MAPPING.ID_DIST = TB_DIST.ID_DIST"
                                    + " where DT_DELIVERY = @dtdelivdt"
                                    + " and  CD_DIST_GROUP = @cddistgroup"
                                    + " and NU_MAGICHI<>0"
                                    + " and FG_MAPSTATUS=@mapstatus"
                                    + " group by CD_DIST_GROUP, tdunitaddrcode) d";

                        result = con.Query(sql, new
                        {
                            @dtdelivdt = DT_DELIVERY,
                            @cddistgroup = p.CdDistGroup,
                            @status_inprog = (int)DbLib.Defs.Status.Inprog,
                            mapstatus = (int)DbLib.Defs.Status.Completed,
                        })
                             .Select(q => new DistGroupInfo
                             {
                                 LocCnt = q.maguchi ?? 0,
                                 LStatus = q.lstatus ?? 0,
                                 DStatus = q.dstatus ?? 0,
                             }).FirstOrDefault();
                        if (result != null)
                        {
                            p.LocCnt = result.LocCnt;
                            p.LStatus = result.LStatus;
                            p.DStatus = result.DStatus;
                        }
                    }
                }

                return r;
            }
        }
    }
}
