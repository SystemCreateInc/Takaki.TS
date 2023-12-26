using DbLib.Extensions;
using Mapping.Models;
using System.Linq;
using System.Windows.Media.TextFormatting;
using System.Xaml;

namespace Mapping.Reports
{
    public class ReportCreator
    {
        // ロケーション一覧作成
        private const int ROWCOUNT = 20;
        public static List<LocReportViewModel> GetLocList(List<LocInfo> distItems, string cdDistGroup, string nmDistGroup, string dtDelivery)
        {
            var viewModels = new List<LocReportViewModel>();

            var shopGroups = distItems.GroupBy(x => new { x.CdBlock });

            var maxPage = shopGroups.Select(x => (x.Count() / ROWCOUNT) + 1).Sum();
            var page = 0;

            foreach (var shops in shopGroups)
            {
                var pageGroups = shops.OrderBy(x => x.Tdunitaddrcode).Select((value, idx) => new { value, idx }).GroupBy(x => x.idx / ROWCOUNT);

                viewModels.AddRange(pageGroups.Select(datas =>
                {
                    page++;
                    return new LocReportViewModel()
                    {
                        Page = page,
                        PageMax = maxPage,
                        Title = "ロケーション一覧",
                        CdDistGroup = cdDistGroup,
                        NmDistGroup = nmDistGroup,
                        DtDelivery = StringExtensions.GetDate(dtDelivery),

                        CdBlock = shops.FirstOrDefault()?.CdBlock ?? string.Empty,


                        Reports = datas.Select(xx => xx.value).ToList(),
                    };
                }));
            }

            return viewModels;
        }
        public static List<OverReportViewModel> GetOverList(List<OverInfo> distItems)
        {
            var viewModels = new List<OverReportViewModel>();

            var shopGroups = distItems.GroupBy(x => new
            {
                x.CdDistGroup,
                x.CdShukkaBatch,
                x.CdCourse,
                x.CdRoute,
                x.CdTokuisaki,
            })
                .OrderBy(x => x.Key.CdDistGroup)
                    .ThenBy(x => x.Key.CdShukkaBatch)
                    .ThenBy(x => x.Key.CdCourse)
                    .ThenBy(x => x.Key.CdRoute)
                    .ThenBy(x => x.Key.CdTokuisaki);

            var maxPage = shopGroups.Select(x => (x.Count() / ROWCOUNT) + 1).Sum();
            var page = 0;

            foreach (var shops in shopGroups)
            {
                var pageGroups = shops.OrderBy(x => x.CdHimban).Select((value, idx) => new { value, idx }).GroupBy(x => x.idx / ROWCOUNT);

                viewModels.AddRange(pageGroups.Select(datas =>
                {
                    page++;
                    return new OverReportViewModel()
                    {
                        Page = page,
                        PageMax = maxPage,
                        Title = "あふれ得意先仕分一覧",
                        CdDistGroup = shops.FirstOrDefault()?.CdDistGroup ?? string.Empty,
                        NmDistGroup = shops.FirstOrDefault()?.NmDistGroup ?? string.Empty,
                        DtDelivery = StringExtensions.GetDate(shops.FirstOrDefault()?.DtDelivery ?? string.Empty),

                        CdShukkaBatch = shops.FirstOrDefault()?.CdShukkaBatch ?? string.Empty,
                        NmShukkaBatch = shops.FirstOrDefault()?.NmShukkaBatch ?? string.Empty,
                        CdCourse = shops.FirstOrDefault()?.CdCourse ?? string.Empty,
                        CdRoute = shops.FirstOrDefault()?.CdRoute ?? 0,
                        CdTokuisaki = shops.FirstOrDefault()?.CdTokuisaki ?? string.Empty,
                        NmTokuisaki = shops.FirstOrDefault()?.NmTokuisaki ?? string.Empty,
                        LargeBox = shops.FirstOrDefault()?.LargeBox ?? 0,
                        SmallBox = shops.FirstOrDefault()?.SmallBox ?? 0,
                        BlueBox = shops.FirstOrDefault()?.BlueBox ?? 0,

                        Reports = datas.Select(xx => xx.value).ToList(),
                    };
                }));
            }

            return viewModels;
        }
    }
}
