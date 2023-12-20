using DistListPrint.Models;
using System.Windows.Media.TextFormatting;
using System.Xaml;

namespace DistListPrint.Reports
{
    public class ReportCreator
    {
        // 得意先別仕分リスト作成
        public static IEnumerable<CustomerReportViewModel> CreateCustomerReport(IEnumerable<long> ids,
            SearchConditionType searchConditionType, string cdDistGroup, string nmDistGroup, string dtDelivery)
        {
            var viewModels = new List<CustomerReportViewModel>();
            var records = DistListPrintLoader.GetReports(ids);
            var groups = records.GroupBy(x => new { x.CdShukkaBatch, x.CdCourse, x.CdRoute, x.CdTokuisaki });

            foreach (var group in groups)
            {
                // 20行毎に印刷
                var pageGroups = group.OrderBy(x => x.CdHimban).ThenBy(x => x.CdGtin13).ThenBy(x => x.NmHinSeishikimei)
                    .Select((v, idx) => new { v, idx }).GroupBy(x => x.idx / 20);

                if (pageGroups == null)
                {
                    continue;
                }

                var maxPage = pageGroups.Max(x => x.Key);
                viewModels.AddRange(pageGroups.Select((x, idx) =>
                {
                    return new CustomerReportViewModel()
                    {
                        Title = searchConditionType == SearchConditionType.All ? "得意先別仕分リスト" : "得意先別欠品リスト",
                        Page = idx + 1,
                        PageMax = maxPage + 1,
                        CdDistGroup = cdDistGroup,
                        NmDistGroup = nmDistGroup,
                        DtDelivery = dtDelivery,
                        CdShukkaBatch = pageGroups.FirstOrDefault()?.FirstOrDefault()?.v.CdShukkaBatch ?? string.Empty,
                        NmShukkaBatch = pageGroups.Max(x => x.Max(xx => xx.v.NmShukkaBatch)) ?? string.Empty,
                        CdCourse = pageGroups.FirstOrDefault()?.FirstOrDefault()?.v.CdCourse ?? string.Empty,
                        CdRoute = pageGroups.Max(x => x.Max(xx => xx.v.CdRoute)),
                        CdTokuisaki = pageGroups.FirstOrDefault()?.FirstOrDefault()?.v.CdTokuisaki ?? string.Empty,
                        NmTokuisaki = pageGroups.FirstOrDefault()?.FirstOrDefault()?.v.NmTokuisaki ?? string.Empty,
                        Reports = x.Select(xx => xx.v).ToArray(),
                    };
                }));
            }

            return viewModels;
        }

        // 商品別仕分リスト作成
        public static IEnumerable<ItemReportViewModel> CreateItemReport(IEnumerable<long> ids,
            SearchConditionType searchConditionType, string cdDistGroup, string nmDistGroup, string dtDelivery)
        {
            var viewModels = new List<ItemReportViewModel>();
            var records = DistListPrintLoader.GetReports(ids);
            var groups = records.GroupBy(x => new { x.CdHimban, x.CdGtin13, x.NmHinSeishikimei, x.NuBoxunit });

            foreach (var group in groups)
            {
                // 20行毎に印刷
                var pageGroups = group.OrderBy(x => x.CdShukkaBatch).ThenBy(x => x.CdCourse).ThenBy(x => x.CdRoute).ThenBy(x => x.CdTokuisaki)
                    .Select((v, idx) => new { v, idx }).GroupBy(x => x.idx / 20);

                if (pageGroups == null)
                {
                    continue;
                }

                var pageMax = pageGroups.Max(x => x.Key);
                viewModels.AddRange(pageGroups.Select((x, idx) =>
                {
                    return new ItemReportViewModel()
                    {
                        Title = searchConditionType == SearchConditionType.All ? "商品別仕分リスト" : "商品別欠品リスト",
                        Page = idx + 1,
                        PageMax = pageMax + 1,
                        CdDistGroup = cdDistGroup,
                        NmDistGroup = nmDistGroup,
                        DtDelivery = dtDelivery,
                        CdHimban = pageGroups.Max(x => x.Max(xx => xx.v.CdHimban)) ?? string.Empty,
                        CdGtin13 = pageGroups.Max(x => x.Max(xx => xx.v.CdGtin13)) ?? string.Empty,
                        NmHinSeishikimei = pageGroups.Max(x => x.Max(xx => xx.v.NmHinSeishikimei)) ?? string.Empty,
                        NuBoxunit = pageGroups.Max(x => x.Max(xx => xx.v.NuBoxunit)),
                        Reports = x.Select(xx => xx.v).ToArray(),
                    };
                }));
            }

            return viewModels;
        }
    }
}
