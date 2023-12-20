using DbLib.DbEntities;
using DbLib;
using SearchBoxLib;
using Dapper.FastCrud;
using DbLib.Defs;
using System.Runtime.InteropServices;

namespace DistLargePrint.Models
{
    public class DistLargePrintLoader
    {
        public static IEnumerable<DistLargePrint> Get(SearchConditionType searchConditionType, string cdLargeGroup, string dtDelivery, SearchQueryParam searchQueryParam)
        {
            using (var con = DbFactory.CreateConnection())
            {
                // 前回検索時のパラメータ取得
                var prm = searchQueryParam.GetSearchParameters();
                var mapStatus = Status.Completed;
                prm.Add("mapStatus", mapStatus);
                prm.Add("dtDelivery", dtDelivery);
                prm.Add("cdLargeGroup", cdLargeGroup);

                var whereSql = searchQueryParam?.GetSearchWhere();
                whereSql = !string.IsNullOrEmpty(whereSql) ? "and " + whereSql : " and 1=1";

                // 全件、未処理絞り込み
                if (searchConditionType == SearchConditionType.Uncompleted)
                {
                    whereSql += Sql.Format<TBDISTEntity>($" and {nameof(TBDISTEntity.FGLSTATUS):C} < {nameof(searchConditionType):P}");
                    prm.Add("searchConditionType", Status.Completed);
                }

                return con.Find<TBDISTEntity>(s => s
                .Include<TBDISTMAPPINGEntity>(j => j.InnerJoin())
                .Where(@$"{nameof(TBDISTEntity.FGMAPSTATUS):C}={nameof(mapStatus):P}
                    and {nameof(TBDISTEntity.DTDELIVERY):C}={nameof(dtDelivery):P}
                    and {nameof(TBDISTMAPPINGEntity.CDLARGEGROUP):of TB_DIST_MAPPING}={nameof(cdLargeGroup):P} {whereSql}")
                .WithParameters(prm))
                    .GroupBy(x => new { x.CDHIMBAN, x.CDGTIN13, x.TBDISTMAPPING?.FirstOrDefault()?.NMHINSEISHIKIMEI, x.NUBOXUNIT, x.TBDISTMAPPING?.FirstOrDefault()?.CDBLOCK })
                    .Select(x => new DistLargePrint
                    {
                        CdHimban = x.Key.CDHIMBAN,
                        CdJan = x.Key.CDGTIN13,
                        NmHinSeishikimei = x.Key.NMHINSEISHIKIMEI,
                        NuBoxunit = x.Key.NUBOXUNIT,
                        CdBlock = x.Key.CDBLOCK,
                        BoxOps = x.Key.NUBOXUNIT == 0 ? 0 : x.Sum(x => x.NULOPS) / x.Key.NUBOXUNIT,
                        BaraOps = x.Key.NUBOXUNIT == 0 ? 0 : x.Sum(x => x.NULOPS) % x.Key.NUBOXUNIT,
                        NuLops = x.Sum(x => x.NULOPS),
                        BoxRps = x.Key.NUBOXUNIT == 0 ? 0 : x.Sum(x => x.NULRPS) / x.Key.NUBOXUNIT,
                        BaraRps = x.Key.NUBOXUNIT == 0 ? 0 : x.Sum(x => x.NULRPS) % x.Key.NUBOXUNIT,
                        NuLrps = x.Sum(x => x.NULRPS),
                        BoxRemainingPs = x.Key.NUBOXUNIT == 0 ? 0 : (x.Sum(x => x.NULOPS) - x.Sum(x => x.NULRPS)) / x.Key.NUBOXUNIT,
                        BaraRemainingPs = x.Key.NUBOXUNIT == 0 ? (x.Sum(x => x.NULOPS) - x.Sum(x => x.NULRPS)) : (x.Sum(x => x.NULOPS) - x.Sum(x => x.NULRPS)) % x.Key.NUBOXUNIT,
                        TotalRemainingPs = x.Sum(x => x.NULOPS) - x.Sum(x => x.NULRPS),
                        DtWorkdtLarge =x.Max(x => x.DTWORKDTLARGE),
                        NMShainLarge = x.Max(x => x.NMSHAINLARGE),
                    });
            }
        }
    }
}
