using ControlzEx.Standard;
using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using Mapping.Defs;
using System.Drawing;
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

                if (r.Count() != 0)
                {
                    // 未処理の１件目に選択チェックする場合はtrue うを設定
                    bool InitSelect = true;

                    foreach (var p in r)
                    {
                        // 各仕分グループ件数取得
                        var sql = "select CD_DIST_GROUP"
                            //+ ", count(distinct case when tdunitaddrcode='' then null else CD_BLOCK+tdunitaddrcode end) shopcnt"
                            + ", count(distinct case when tdunitaddrcode='' then null else CD_TOKUISAKI end) shopcnt"
                            + ", count(distinct case when tdunitaddrcode='' then null else CD_BLOCK+tdunitaddrcode end) loccnt"
                            + ",count(distinct case when FG_MAPSTATUS = 1 then CD_TOKUISAKI else null end) overshopcnt"
                            + " from TB_DIST"
                            + " inner join TB_DIST_MAPPING on TB_DIST_MAPPING.ID_DIST = TB_DIST.ID_DIST"
                            + " where DT_DELIVERY = @dtdelivdt"
                            + " and CD_DIST_GROUP = @cddistgroup"
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
                                 LocCnt = q.loccnt ?? 0,
                             }).FirstOrDefault();
                        if (result != null)
                        {
                            p.OverShopCnt = result.OverShopCnt;
                            p.ShopCnt = result.ShopCnt;
                            p.LocCnt = result.LocCnt;
                            p.MStatus = result.ShopCnt == 0 ? Defs.MStatus.Ready : Defs.MStatus.Decision;
                        }
                        else
                        {
                            // 未処理の１件目に選択チェック
                            p.Select = InitSelect;
                            InitSelect = false;
                        }

                        sql = "select sum(NU_MAGUCHI) maguchi"
                                    + ",(case when max(lstatus) <> min(lstatus) then @status_inprog else max(lstatus) end) lstatus"
                                    + ",(case when max(dstatus)<> min(dstatus) then @status_inprog else max(dstatus) end) dstatus"
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
                                    + " group by CD_DIST_GROUP, CD_BLOCK, tdunitaddrcode) d";

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
        public static List<LocInfo> GetLoc(DistGroupInfo? distgroupinfo)
        {
            if (distgroupinfo == null)
                return new List<LocInfo>();

            using (var con = DbFactory.CreateConnection())
            {
                var sql = "select CD_BLOCK,tdunitaddrcode,CD_SUM_TOKUISAKI,NM_SUM_TOKUISAKI,CD_SUM_COURSE,CD_SUM_ROUTE,CD_BIN_SUM,NU_MAGICHI"
                        + ",min(CD_TOKUISAKI) mintokuisaki"
                        + ",max(CD_TOKUISAKI) maxtokuisaki"
                        + " from TB_DIST"
                        + " inner join TB_DIST_MAPPING on TB_DIST.ID_DIST = TB_DIST_MAPPING.ID_DIST"
                        + " where DT_DELIVERY = @DtDelivery and CD_DIST_GROUP = @CdDistGroup"
                        + " and FG_MAPSTATUS = @MapStatus"
                        + " group by CD_BLOCK,tdunitaddrcode,CD_SUM_TOKUISAKI,NM_SUM_TOKUISAKI,CD_SUM_COURSE,CD_SUM_ROUTE,CD_BIN_SUM,NU_MAGICHI"
                        + " order by CD_BLOCK, tdunitaddrcode";

                return con.Query(sql, new
                {
                    @DtDelivery = distgroupinfo.DtDelivdt,
                    @CdDistGroup = distgroupinfo.CdDistGroup,
                    MapStatus = (int)DbLib.Defs.Status.Completed,
                })
                     .Select(q => new LocInfo
                     {
                         CdBlock = q.CD_BLOCK,
                         Tdunitaddrcode = q.tdunitaddrcode,
                         CdTokuisaki = q.CD_SUM_TOKUISAKI,
                         NmTokuisaki = q.NM_SUM_TOKUISAKI,
                         CdCourse = q.CD_SUM_COURSE,
                         CdRoute = q.CD_SUM_ROUTE.ToString(),
                         CdBinSum = q.CD_BIN_SUM == (int)BinSumType.Yes ? "●" : "",
                         CdSumTokuisaki = q.mintokuisaki != q.maxtokuisaki ? "●" : "",
                         Maguchi = q.NU_MAGICHI.ToString(),
                     }).ToList();
            }
        }
    }
}
