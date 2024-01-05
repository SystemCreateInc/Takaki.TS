using Dapper;
using DbLib;
using DbLib.Defs;
using SearchBoxLib;

namespace TakahataDistListPrint.Models
{
    public class TakahataDistListPrintLoader
    {
        public static IEnumerable<TakahataDistListPrint> Get(string dtDelivery, SearchQueryParam searchQueryParam)
        {
            using (var con = DbFactory.CreateConnection())
            {
                // 前回検索時のパラメータ取得
                var prm = searchQueryParam.GetSearchParameters();

                // 座席マッピングステータスが0のものが対象
                var mapStatus = Status.Ready;
                prm.Add("mapStatus", (int)mapStatus);
                prm.Add("dtDelivery", dtDelivery);

                // 名称は各マスターの適用日範囲内のupdatedAtの最新の一行を取得する
                var sql = "select "
                    + "ID_DIST, "
                    + "CD_SHUKKA_BATCH, "
                    + "CD_COURSE, "
                    + "CD_ROUTE, "
                    + "TB_DIST.CD_TOKUISAKI, "
                    + "v1.NM_TOKUISAKI, "
                    + "TB_DIST.CD_HIMBAN, "
                    + "CD_GTIN13, "
                    + "v2.NM_HIN_SEISHIKIMEI, "
                    + "NU_BOXUNIT, "
                    + "NU_OPS, "
                    + "NU_DRPS "
                    + "from TB_DIST "
                    + "left join (select * from "
                        + "(select CD_TOKUISAKI, NM_TOKUISAKI, row_number() over(partition by CD_TOKUISAKI order by updatedAt desc) no "
                        + "from TB_MTOKUISAKI where TB_MTOKUISAKI.DT_TEKIYOKAISHI <= @dtDelivery and @dtDelivery < TB_MTOKUISAKI.DT_TEKIYOMUKO) t1 "
                        + "where no = 1) v1 on TB_DIST.CD_TOKUISAKI = v1.CD_TOKUISAKI "
                    + "left join (select * from "
                        + "(select CD_HIMBAN, NM_HIN_SEISHIKIMEI, row_number() over(partition by CD_HIMBAN order by updatedAt desc) no "
                        + "from TB_MHIMMOKU where TB_MHIMMOKU.DT_TEKIYOKAISHI <= @dtDelivery and @dtDelivery < TB_MHIMMOKU.DT_TEKIYOMUKO) t2 "
                        + "where no = 1) v2 on TB_DIST.CD_HIMBAN = v2.CD_HIMBAN "
                    + $"where TB_DIST.FG_MAPSTATUS = @mapStatus and TB_DIST.DT_DELIVERY = @dtDelivery ";

                var whereSql = searchQueryParam?.GetSearchWhere();
                whereSql = !string.IsNullOrEmpty(whereSql) ? "and " + whereSql : " and 1=1";
                sql += whereSql;

                return con.Query(sql, prm)
                    .Select(q => new TakahataDistListPrint
                    {
                        IdDist = q.ID_DIST,
                        CdShukkaBatch = q.CD_SHUKKA_BATCH,
                        CdCourse = q.CD_COURSE,
                        CdRoute = q.CD_ROUTE,
                        CdTokuisaki = q.CD_TOKUISAKI,
                        NmTokuisaki = q.NM_TOKUISAKI,
                        CdHimban = q.CD_HIMBAN,
                        CdGtin13 = q.CD_GTIN13,
                        NmHinSeishikimei = q.NM_HIN_SEISHIKIMEI,
                        NuBoxunit = q.NU_BOXUNIT,
                        BoxOps = q.NU_BOXUNIT == 0 ? 0 : q.NU_OPS / q.NU_BOXUNIT,
                        BaraOps = q.NU_BOXUNIT == 0 ? 0 : q.NU_OPS % q.NU_BOXUNIT,
                        NuOps = q.NU_OPS,
                        BoxRps = q.NU_BOXUNIT == 0 ? 0 : q.NU_DRPS / q.NU_BOXUNIT,
                        BaraRps = q.NU_BOXUNIT == 0 ? 0 : q.NU_DRPS % q.NU_BOXUNIT,
                        NuDrps = q.NU_DRPS,
                        BoxRemainingPs = q.NU_BOXUNIT == 0 ? 0 : (q.NU_OPS - q.NU_DRPS) / q.NU_BOXUNIT,
                        BaraRemainingPs = q.NU_BOXUNIT == 0 ? (q.NU_OPS - q.NU_DRPS) : (q.NU_OPS - q.NU_DRPS) % q.NU_BOXUNIT,
                        TotalRemainingPs = q.NU_OPS - q.NU_DRPS,
                    });
            }
        }

        public static IEnumerable<TakahataDistListPrint> GetReports(string dtDelivery, IEnumerable<long> distIds)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var sql = "select "
                    + "ID_DIST, "
                    + "TB_DIST.CD_SHUKKA_BATCH, "
                    + "v3.NM_SHUKKA_BATCH, "
                    + "CD_COURSE, "
                    + "CD_ROUTE, "
                    + "TB_DIST.CD_TOKUISAKI, "
                    + "v1.NM_TOKUISAKI, "
                    + "TB_DIST.CD_HIMBAN, "
                    + "CD_GTIN13, "
                    + "v2.NM_HIN_SEISHIKIMEI, "
                    + "NU_BOXUNIT, "
                    + "NU_OPS, "
                    + "NU_DRPS "
                    + "from TB_DIST "
                    + "left join (select * from "
                        + "(select CD_TOKUISAKI, NM_TOKUISAKI, row_number() over(partition by CD_TOKUISAKI order by updatedAt desc) no "
                        + "from TB_MTOKUISAKI where TB_MTOKUISAKI.DT_TEKIYOKAISHI <= @dtDelivery and @dtDelivery < TB_MTOKUISAKI.DT_TEKIYOMUKO) t1 "
                        + "where no = 1) v1 on TB_DIST.CD_TOKUISAKI = v1.CD_TOKUISAKI "
                    + "left join (select * from "
                        + "(select CD_HIMBAN, NM_HIN_SEISHIKIMEI, row_number() over(partition by CD_HIMBAN order by updatedAt desc) no "
                        + "from TB_MHIMMOKU where TB_MHIMMOKU.DT_TEKIYOKAISHI <= @dtDelivery and @dtDelivery < TB_MHIMMOKU.DT_TEKIYOMUKO) t2 "
                        + "where no = 1) v2 on TB_DIST.CD_HIMBAN = v2.CD_HIMBAN "
                    + "left join(select* from (select CD_SHUKKA_BATCH, NM_SHUKKA_BATCH, row_number() over(partition by CD_SHUKKA_BATCH order by updatedAt desc) no "
                        + "from TB_MSHUKKA_BATCH where TB_MSHUKKA_BATCH.DT_TEKIYOKAISHI <= @dtDelivery and @dtDelivery < TB_MSHUKKA_BATCH.DT_TEKIYOMUKO) t3 "
                        + "where no = 1) v3 on TB_DIST.CD_SHUKKA_BATCH = v3.CD_SHUKKA_BATCH "
                    + $"where TB_DIST.ID_DIST in @distIds ";

                return con.Query(sql, new { distIds, dtDelivery })
                    .Select(q => new TakahataDistListPrint
                    {
                        IdDist = q.ID_DIST,
                        CdShukkaBatch = q.CD_SHUKKA_BATCH,
                        NmShukkaBatch = q.NM_SHUKKA_BATCH,
                        CdCourse = q.CD_COURSE,
                        CdRoute = q.CD_ROUTE,
                        CdTokuisaki = q.CD_TOKUISAKI,
                        NmTokuisaki = q.NM_TOKUISAKI,
                        CdHimban = q.CD_HIMBAN,
                        CdGtin13 = q.CD_GTIN13,
                        NmHinSeishikimei = q.NM_HIN_SEISHIKIMEI,
                        NuBoxunit = q.NU_BOXUNIT,
                        BoxOps = q.NU_BOXUNIT == 0 ? 0 : q.NU_OPS / q.NU_BOXUNIT,
                        BaraOps = q.NU_BOXUNIT == 0 ? 0 : q.NU_OPS % q.NU_BOXUNIT,
                        NuOps = q.NU_OPS,
                        BoxRps = q.NU_BOXUNIT == 0 ? 0 : q.NU_DRPS / q.NU_BOXUNIT,
                        BaraRps = q.NU_BOXUNIT == 0 ? 0 : q.NU_DRPS % q.NU_BOXUNIT,
                        NuDrps = q.NU_DRPS,
                        BoxRemainingPs = q.NU_BOXUNIT == 0 ? 0 : (q.NU_OPS - q.NU_DRPS) / q.NU_BOXUNIT,
                        BaraRemainingPs = q.NU_BOXUNIT == 0 ? (q.NU_OPS - q.NU_DRPS) : (q.NU_OPS - q.NU_DRPS) % q.NU_BOXUNIT,
                        TotalRemainingPs = q.NU_OPS - q.NU_DRPS,
                    });
            }
        }
    }
}
