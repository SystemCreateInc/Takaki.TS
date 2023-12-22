namespace StowageListPrint.Reports
{
    public class StowageReportCreator
    {
        public static IEnumerable<StowageReportViewModel> Create(string cdDistGroup, string nmDistGroup, string dtDelivery,
            IEnumerable<Models.StowageListPrint> records)
        {
            var viewModels = new List<StowageReportViewModel>();

            // 20行毎に印刷
            var pageGroups = records.OrderBy(x => x.CdShukkaBatch).ThenBy(x => x.CdCourse).ThenBy(x => x.CdRoute).ThenBy(x => x.CdTokuisaki)
                .Select((v, idx) => new { v, idx }).GroupBy(x => x.idx / 20);

            var maxPage = pageGroups.Max(x => x.Key);
            viewModels.AddRange(pageGroups.Select((x, idx) =>
            {
                return new StowageReportViewModel()
                {
                    Page = idx + 1,
                    PageMax = maxPage + 1,
                    CdDistGroup = cdDistGroup,
                    NmDistGroup = nmDistGroup,
                    DtDelivery = dtDelivery,
                    Reports = x.Select(xx => xx.v).ToArray(),
                };
            }));


            return viewModels;
        }
    }
}
