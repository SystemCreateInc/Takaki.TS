using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
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
                prm.Add("mapStatus", mapStatus);
                prm.Add("dtDelivery", dtDelivery);

                var whereSql = searchQueryParam?.GetSearchWhere();
                whereSql = !string.IsNullOrEmpty(whereSql) ? "and " + whereSql : " and 1=1";

                return con.Find<TBDISTEntity>(s => s
                .Include<TBDISTMAPPINGEntity>(j => j.InnerJoin())
                .Where(@$"{nameof(TBDISTEntity.FGMAPSTATUS):C}={nameof(mapStatus):P}
                    and {nameof(TBDISTEntity.DTDELIVERY):C}={nameof(dtDelivery):P} {whereSql}")
                .WithParameters(prm))
                    .Select(x => new TakahataDistListPrint
                    {
                        IdDist = x.IDDIST,
                        CdShukkaBatch = x.CDSHUKKABATCH,
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
                    });
            }
        }

        public static IEnumerable<TakahataDistListPrint> GetReports(IEnumerable<long> distIds)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBDISTEntity>(s => s
                .Include<TBDISTMAPPINGEntity>(j => j.InnerJoin())
                .Where(@$"{nameof(TBDISTEntity.IDDIST):of TB_DIST} in {nameof(distIds):P}")
                .WithParameters(new { distIds }))
                    .Select(x => new TakahataDistListPrint
                    {
                        IdDist = x.IDDIST,
                        CdShukkaBatch = x.CDSHUKKABATCH,
                        NmShukkaBatch = x.TBDISTMAPPING?.FirstOrDefault()?.NMSHUKKABATCH ?? string.Empty,
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
                    });
            }
        }
    }
}
