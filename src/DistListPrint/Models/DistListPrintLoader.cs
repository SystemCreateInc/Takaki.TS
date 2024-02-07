using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using SearchBoxLib;
using SelDistGroupLib.Models;

namespace DistListPrint.Models
{
    public class DistListPrintLoader
    {
        public static IEnumerable<DistListPrint> Get(SearchConditionType searchConditionType, string cdDistGroup, string dtDelivery, SearchQueryParam searchQueryParam)
        {
            using (var con = DbFactory.CreateConnection())
            {
                // 前回検索時のパラメータ取得
                var prm = searchQueryParam.GetSearchParameters();
                var mapStatus = Status.Completed;
                prm.Add("mapStatus", mapStatus);
                prm.Add("dtDelivery", dtDelivery);
                prm.Add("cdDistGroup", cdDistGroup);

                var whereSql = searchQueryParam?.GetSearchWhere();
                whereSql = !string.IsNullOrEmpty(whereSql) ? "and " + whereSql : " and 1=1";

                // 全件、未処理絞り込み
                if (searchConditionType == SearchConditionType.Uncompleted)
                {
                    whereSql += Sql.Format<TBDISTEntity>($" and {nameof(TBDISTEntity.FGDSTATUS):C} < {nameof(searchConditionType):P}");
                    prm.Add("searchConditionType", Status.Completed);
                }

                // 出荷バッチ、ブロック、コース、配送順、得意先、品番でソート
                return con.Find<TBDISTEntity>(s => s
                .Include<TBDISTMAPPINGEntity>(j => j.InnerJoin())
                .Where(@$"{nameof(TBDISTEntity.FGMAPSTATUS):C}={nameof(mapStatus):P}
                    and {nameof(TBDISTEntity.DTDELIVERY):C}={nameof(dtDelivery):P}
                    and {nameof(TBDISTMAPPINGEntity.CDDISTGROUP):of TB_DIST_MAPPING}={nameof(cdDistGroup):P} {whereSql}")
                .OrderBy($"{nameof(TBDISTEntity.CDSHUKKABATCH):C}, {nameof(TBDISTMAPPINGEntity.CDBLOCK):of TB_DIST_MAPPING}, {nameof(TBDISTEntity.CDCOURSE):C}, {nameof(TBDISTEntity.CDROUTE):C}, {nameof(TBDISTEntity.CDTOKUISAKI):C}, {nameof(TBDISTEntity.CDHIMBAN):C}")
                .WithParameters(prm))
                    .Select(x => new DistListPrint
                    {
                        IdDist = x.IDDIST,
                        CdShukkaBatch = x.CDSHUKKABATCH,
                        CdBlock = x.TBDISTMAPPING?.FirstOrDefault()?.CDBLOCK ?? string.Empty,
                        CdCourse = x.CDCOURSE,
                        CdRoute = x.CDROUTE,
                        CdTokuisaki = x.CDTOKUISAKI,
                        NmTokuisaki = x.TBDISTMAPPING?.FirstOrDefault()?.NMTOKUISAKI ?? string.Empty,
                        CdHimban = x.CDHIMBAN,
                        CdGtin13 = x.CDGTIN13,
                        NmHinSeishikimei = x.TBDISTMAPPING?.FirstOrDefault()?.NMHINSEISHIKIMEI ?? string.Empty,
                        NuBoxunit = x.NUBOXUNIT,
                        BoxOps = x.NUBOXUNIT == 0 ? 0 : x.NUOPS / x.NUBOXUNIT,
                        BaraOps = x.NUBOXUNIT == 0 ? 0 : x.NUOPS % x.NUBOXUNIT,
                        NuOps = x.NUOPS,
                        BoxRps = x.NUBOXUNIT == 0 ? 0 : x.NUDRPS / x.NUBOXUNIT,
                        BaraRps = x.NUBOXUNIT == 0 ? 0 : x.NUDRPS % x.NUBOXUNIT,
                        NuDrps = x.NUDRPS,
                        BoxRemainingPs = x.NUBOXUNIT == 0 ? 0 : (x.NUOPS - x.NUDRPS) / x.NUBOXUNIT,
                        BaraRemainingPs = x.NUBOXUNIT == 0 ? (x.NUOPS - x.NUDRPS) : (x.NUOPS - x.NUDRPS) % x.NUBOXUNIT,
                        TotalRemainingPs = x.NUOPS - x.NUDRPS,
                        DtWorkdtDist = x.DTWORKDTDIST,
                        NmShainDist = x.NMSHAINDIST,
                    });
            }
        }

        public static IEnumerable<DistListPrint> GetReports(IEnumerable<long> distIds)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBDISTEntity>(s => s
                .Include<TBDISTMAPPINGEntity>(j => j.InnerJoin())
                .Where(@$"{nameof(TBDISTEntity.IDDIST):of TB_DIST} in {nameof(distIds):P}")
                .WithParameters(new { distIds }))
                    .Select(x => new DistListPrint
                    {
                        IdDist = x.IDDIST,
                        CdShukkaBatch = x.CDSHUKKABATCH,
                        NmShukkaBatch = x.TBDISTMAPPING?.FirstOrDefault()?.NMSHUKKABATCH ?? string.Empty,
                        CdBlock = x.TBDISTMAPPING?.FirstOrDefault()?.CDBLOCK ?? string.Empty,
                        CdCourse = x.CDCOURSE,
                        CdRoute = x.CDROUTE,
                        CdTokuisaki = x.CDTOKUISAKI,
                        NmTokuisaki = x.TBDISTMAPPING?.FirstOrDefault()?.NMTOKUISAKI ?? string.Empty,
                        CdHimban = x.CDHIMBAN,
                        CdGtin13 = x.CDGTIN13,
                        NmHinSeishikimei = x.TBDISTMAPPING?.FirstOrDefault()?.NMHINSEISHIKIMEI ?? string.Empty,
                        NuBoxunit = x.NUBOXUNIT,
                        BoxOps = x.NUBOXUNIT == 0 ? 0 : x.NUOPS / x.NUBOXUNIT,
                        BaraOps = x.NUBOXUNIT == 0 ? 0 : x.NUOPS % x.NUBOXUNIT,
                        NuOps = x.NUOPS,
                        BoxRps = x.NUBOXUNIT == 0 ? 0 : x.NUDRPS / x.NUBOXUNIT,
                        BaraRps = x.NUBOXUNIT == 0 ? 0 : x.NUDRPS % x.NUBOXUNIT,
                        NuDrps = x.NUDRPS,
                        BoxRemainingPs = x.NUBOXUNIT == 0 ? 0 : (x.NUOPS - x.NUDRPS) / x.NUBOXUNIT,
                        BaraRemainingPs = x.NUBOXUNIT == 0 ? (x.NUOPS - x.NUDRPS) : (x.NUOPS - x.NUDRPS) % x.NUBOXUNIT,
                        TotalRemainingPs = x.NUOPS - x.NUDRPS,
                        DtWorkdtDist = x.DTWORKDTDIST,
                        NmShainDist = x.NMSHAINDIST,
                    });
            }
        }
    }
}
