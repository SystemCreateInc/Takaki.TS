using DistListPrint.Models;

namespace DistListPrint.Reports
{
    public class ReportCreator
    {
        // 得意先別仕分リスト作成
        public static IEnumerable<CustomerReportViewModel> CreateCustomerReport(IEnumerable<long> ids,
            SearchConditionType searchConditionType, string cdDistGroup, string nmDistGroup, string dtDelivery, int chunkSize)
        {
            var viewModels = new List<CustomerReportViewModel>();

            // パラメーターエラーを回避するために印刷単位毎に再読み込み
            var groupIdss = ids
                .Select((v, i) => new { v, i })
                .GroupBy(x => x.i / chunkSize)
                .Select(g => g.Select(x => x.v));

            var records = new List<Models.DistListPrint>();
            foreach (var groupIds in groupIdss)
            {
                records.AddRange(DistListPrintLoader.GetReports(groupIds));
            }

            // 出荷バッチ、コース、配送順、取引先でグループ化
            // 出荷バッチ、コース、配送順、取引先、品番でソート
            var groups = records
                .GroupBy(x => new { x.CdShukkaBatch, x.CdCourse, x.CdRoute, x.CdTokuisaki })
                .OrderBy(x => x.Key.CdShukkaBatch).ThenBy(x => x.Key.CdCourse).ThenBy(x => x.Key.CdRoute).ThenBy(x => x.Key.CdTokuisaki).ThenBy(x => x.Select(xx => xx.CdHimban));

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
            SearchConditionType searchConditionType, string cdDistGroup, string nmDistGroup, string dtDelivery, int chunkSize)
        {
            var viewModels = new List<ItemReportViewModel>();

            // パラメーターエラーを回避するために印刷単位毎に再読み込み
            var groupIdss = ids
                .Select((v, i) => new { v, i })
                .GroupBy(x => x.i / chunkSize)
                .Select(g => g.Select(x => x.v));

            var records = new List<Models.DistListPrint>();
            foreach (var groupIds in groupIdss)
            {
                records.AddRange(DistListPrintLoader.GetReports(groupIds));
            }

            // 品番、Jan、品名、入数でグループ化
            // 品番、Jan、品名、入数、出荷バッチ、コース、配送順、得意先でソート
            var groups = records
                .GroupBy(x => new { x.CdHimban, x.CdGtin13, x.NmHinSeishikimei, x.NuBoxunit })
                .OrderBy(x => x.Key.CdHimban).ThenBy(x => x.Key.CdGtin13).ThenBy(x => x.Key.NmHinSeishikimei).ThenBy(x => x.Key.NuBoxunit)
                .ThenBy(x => x.Select(x => x.CdShukkaBatch)).ThenBy(x => x.Select(x => x.CdCourse)).ThenBy(x => x.Select(x => x.CdRoute))
                .ThenBy(x => x.Select(x => x.CdTokuisaki));

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

        // 得意先別総個数集計リスト作成
        public static IEnumerable<CustomerUnitReportViewModel> CreateCustomerUnitReport(IEnumerable<long> ids, string cdDistGroup,
            string nmDistGroup, string dtDelivery, int chunkSize)
        {
            var viewModels = new List<CustomerUnitReportViewModel>();

            // パラメーターエラーを回避するために印刷単位毎に再読み込み
            var groupIdss = ids
                .Select((v, i) => new { v, i })
                .GroupBy(x => x.i / chunkSize)
                .Select(g => g.Select(x => x.v));

            var records = new List<Models.DistListPrint>();
            foreach (var groupIds in groupIdss)
            {
                records.AddRange(DistListPrintLoader.GetReports(groupIds));
            }

            // 出荷バッチ、ブロック、コース、配送順、得意先でグループ化
            var customerUnits = records
                .GroupBy(x => new { x.CdShukkaBatch, x.CdBlock, x.CdCourse, x.CdRoute, x.CdTokuisaki })
                .Select(x => new CustomerUnit
                {
                    CdShukkaBatch = x.Key.CdShukkaBatch,
                    NmShukkaBatch = x.Max(xx => xx.NmShukkaBatch) ?? string.Empty,
                    CdBlock = x.Key.CdBlock,
                    CdCourse = x.Key.CdCourse,
                    CdRoute = x.Key.CdRoute,
                    CdTokuisaki = x.Key.CdTokuisaki,
                    NmTokuisaki = x.Max(xx => xx.NmTokuisaki) ?? string.Empty,
                    TotalItemCount = x.Select(xx => xx.CdHimban).Distinct().Count(),
                    TotalNuOps = x.Sum(xx => xx.NuOps),
                });

            // 出荷バッチ、ブロック、コースでグループ化
            // 出荷バッチ、ブロック、コース、配送順、得意先でソート
            var groups = customerUnits
                .GroupBy(x => new { x.CdShukkaBatch, x.CdBlock, x.CdCourse })
                .OrderBy(x => x.Key.CdShukkaBatch).ThenBy(x => x.Key.CdBlock).ThenBy(x => x.Key.CdCourse)
                .ThenBy(x => x.Select(x => x.CdRoute)).ThenBy(x => x.Select(x => x.CdTokuisaki));

            foreach (var group in groups)
            {
                // 28行毎に印刷
                var pageGroups = group.OrderBy(x => x.CdRoute).ThenBy(x => x.CdTokuisaki)
                    .Select((v, idx) => new { v, idx }).GroupBy(x => x.idx / 28);

                if (pageGroups == null)
                {
                    continue;
                }

                var pageMax = pageGroups.Max(x => x.Key);
                viewModels.AddRange(pageGroups.Select((x, idx) =>
                {
                    return new CustomerUnitReportViewModel()
                    {
                        Page = idx + 1,
                        PageMax = pageMax + 1,
                        CdDistGroup = cdDistGroup,
                        NmDistGroup = nmDistGroup,
                        DtDelivery = dtDelivery,
                        CdShukkaBatch = pageGroups.Max(x => x.Max(xx => xx.v.CdShukkaBatch)) ?? string.Empty,
                        NmShukkaBatch = pageGroups.Max(x => x.Max(xx => xx.v.NmShukkaBatch)) ?? string.Empty,
                        CdBlock = pageGroups.Max(x => x.Max(xx => xx.v.CdBlock)) ?? string.Empty,
                        CdCourse = pageGroups.Max(x => x.Max(xx => xx.v.CdCourse)) ?? string.Empty,
                        Reports = x.Select(xx => xx.v).ToArray(),
                    };
                }));
            }

            return viewModels;
        }
    }
}
