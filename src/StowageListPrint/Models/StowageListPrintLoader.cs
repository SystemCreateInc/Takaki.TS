using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using SearchBoxLib;

namespace StowageListPrint.Models
{
    public class StowageListPrintLoader
    {
        public static IEnumerable<StowageListPrint> Get(string cdDistGroup, string dtDelivery, SearchQueryParam searchQueryParam)
        {
            using (var con = DbFactory.CreateConnection())
            {
                // 前回検索時のパラメータ取得
                var prm = searchQueryParam.GetSearchParameters();
                prm.Add("dtDelivery", dtDelivery);
                prm.Add("cdDistGroup", cdDistGroup);

                var whereSql = searchQueryParam?.GetSearchWhere();
                whereSql = !string.IsNullOrEmpty(whereSql) ? "and " + whereSql : " and 1=1";

                return con.Find<TBSTOWAGEEntity>(s => s
                .Include<TBSTOWAGEMAPPINGEntity>(j => j.InnerJoin())
                .Where(@$"{nameof(TBSTOWAGEEntity.DTDELIVERY):C}={nameof(dtDelivery):P}
                        and {nameof(TBSTOWAGEMAPPINGEntity.CDDISTGROUP):of TB_STOWAGE_MAPPING}={nameof(cdDistGroup):P} {whereSql}")
                .WithParameters(prm))
                    .GroupBy(x => new
                    {
                        x.TBSTOWAGEMAPPING?.FirstOrDefault()?.CDBLOCK,
                        x.TBSTOWAGEMAPPING?.FirstOrDefault()?.Tdunitaddrcode,
                        x.CDSHUKKABATCH,
                        x.CDCOURSE,
                        x.CDROUTE,
                        x.CDTOKUISAKI,
                        x.TBSTOWAGEMAPPING?.FirstOrDefault()?.NMTOKUISAKI
                    })
                    .Select(x =>
                    {
                        // 数量は実績数があれば実績を優先し、数量が0の場合は予定数を表示する
                        var largeBoxOps = x.Where(x => (BoxType)x.STBOXTYPE == BoxType.LargeBox).Sum(xx => xx.NUOBOXCNT);
                        var largeBoxRps = x.Where(x => (BoxType)x.STBOXTYPE == BoxType.LargeBox).Sum(xx => xx.NURBOXCNT);
                        var smallBoxOps = x.Where(x => (BoxType)x.STBOXTYPE == BoxType.SmallBox).Sum(xx => xx.NUOBOXCNT);
                        var smallBoxRps = x.Where(x => (BoxType)x.STBOXTYPE == BoxType.SmallBox).Sum(xx => xx.NURBOXCNT);
                        var blueBoxOps = x.Where(x => (BoxType)x.STBOXTYPE == BoxType.BlueBox).Sum(xx => xx.NUOBOXCNT);
                        var blueBoxRps = x.Where(x => (BoxType)x.STBOXTYPE == BoxType.BlueBox).Sum(xx => xx.NURBOXCNT);
                        var etcBoxOps = x.Where(x => (BoxType)x.STBOXTYPE == BoxType.EtcBox).Sum(xx => xx.NUOBOXCNT);
                        var etcBoxRps = x.Where(x => (BoxType)x.STBOXTYPE == BoxType.EtcBox).Sum(xx => xx.NURBOXCNT);

                        var stowageListPrint = new StowageListPrint
                        {
                            IdStowages = x.Select(xx => xx.IDSTOWAGE).ToList(),
                            CdBlock = x.Key.CDBLOCK ?? string.Empty,
                            Tdunitcode = x.Key.Tdunitaddrcode ?? string.Empty,
                            CdShukkaBatch = x.Key.CDSHUKKABATCH,
                            CdCourse = x.Key.CDCOURSE,
                            CdRoute = x.Key.CDROUTE,
                            CdTokuisaki = x.Key.CDTOKUISAKI,
                            NmTokuisaki = x.Key.NMTOKUISAKI ?? string.Empty,
                            LargeBoxOps = largeBoxOps,
                            LargeBoxRps = largeBoxRps,
                            DispLargeBoxPs = largeBoxRps != 0 ? largeBoxRps : largeBoxOps,
                            SmallBoxOps = smallBoxOps,
                            SmallBoxRps = smallBoxRps,
                            DispSmallBoxPs = smallBoxRps != 0 ? smallBoxRps : smallBoxOps,
                            BlueBoxOps = blueBoxOps,
                            BlueBoxRps = blueBoxRps,
                            DispBlueBoxPs = blueBoxRps != 0 ? blueBoxRps : blueBoxOps,
                            EtcBoxOps = etcBoxOps,
                            EtcBoxRps = etcBoxRps,
                            DispEtcBoxPs = etcBoxRps != 0 ? etcBoxRps : etcBoxOps,
                            DtWorkdtStowage = x.Max(x => x.DTWORKDTSTOWAGE),
                            HenkoshaCode = x.Max(x => x.CDHENKOSHA) ?? string.Empty,
                        };

                        return stowageListPrint;
                    })
                    .OrderBy(x => x.CdBlock)
                    .ThenBy(x => x.Tdunitcode)
                    .ThenBy(x => x.CdShukkaBatch)
                    .ThenBy(x => x.CdCourse)
                    .ThenBy(x => x.CdRoute)
                    .ThenBy(x => x.CdTokuisaki);
            }
        }
    }
}
