using System.ComponentModel;

namespace WorkReport.Reports
{
    public class ReportCreator
    {
        public static IEnumerable<ReportViewModel> Create(DateTime startDate, DateTime endDate, IEnumerable<Models.WorkReport> workReports)
        {
            var viewModels = new List<ReportViewModel>();
            var pageGroups = new List<List<Group>> { new List<Group>() };
            var records = Get(workReports);

            foreach (var record in records)
            {
                foreach (var (detail, index) in record.Details.Select((detail, index) => (detail, index)))
                {
                    // 最後の空白行で1行分を足す
                    var pageRowCount = pageGroups.LastOrDefault()?.Sum(x => x.Details.Count() + 2) ?? 0;

                    // レコードが最初の場合、納品日、作業日付、仕分グループ、ブロック、作業開始、作業終了、作業時間、休憩時間の行を作成する
                    if (index == 0)
                    {
                        // ヘッダーが最後に来る場合、改ページする
                        if (pageRowCount >= 19)
                        {
                            pageGroups.Add(new List<Group>());
                            pageRowCount = 0;
                        }

                        pageGroups.Last().Add(CreateHeader(record));
                        pageRowCount++;
                    }

                    // ページ行数が超えた場合、改ページする
                    if (pageRowCount >= 20)
                    {
                        pageGroups.Add(new List<Group>());
                        pageGroups.Last().Add(CreateHeader(record));
                    }

                    pageGroups.Last().Last().Details.Add(new Detail
                    {
                        NmSyain = detail.NmSyain,
                        NmWorktime = detail.NmWorktime,
                        NmItemcnt = detail.NmItemcnt,
                        Shopcnt = detail.Shopcnt,
                        NmDistcnt = detail.NmDistcnt,
                        NmCheckcnt = detail.NmCheckcnt,
                        NmChecktime = detail.NmChecktime,
                    });
                }
            }

            var maxPage = pageGroups.Count;

            viewModels.AddRange(pageGroups.Select(x =>
            {
                return new ReportViewModel()
                {
                    Page = viewModels.Count + 1,
                    PageMax = maxPage,
                    StartDate = startDate,
                    EndDate = endDate,
                    Groups = x.Select(xx => xx).ToArray(),
                };
            }));

            return viewModels;
        }

        private static List<Group> Get(IEnumerable<Models.WorkReport> workReports)
        {
            // 納品日、作業日付、仕分グループ、ブロックでグループ化
            return workReports
                .GroupBy(g => new { g.DtDelivery, g.WorkDate, g.CdDistGroup, g.CdBlock })
                .OrderBy(o => o.Key.DtDelivery).ThenBy(t => t.Key.WorkDate).ThenBy(t => t.Key.CdDistGroup).ThenBy(t => t.Key.CdBlock)
                .Select(s => new Group
                {
                    DtDelivery = s.Key.DtDelivery,
                    DtStart = s.Min(x => x.DtStart),
                    CdDistGroup = s.Key.CdDistGroup,
                    CdBlock = s.Key.CdBlock,
                    DtEnd = s.Max(x => x.DtEnd),
                    NmIdle = s.Max(x => x.NmIdle),
                    Details = s.Select(ss => new Detail
                    {
                        NmSyain = ss.NmSyain,
                        NmWorktime = ss.NmWorktime,
                        NmItemcnt = ss.NmItemcnt,
                        Shopcnt = ss.Shopcnt,
                        NmDistcnt = ss.NmDistcnt,
                        NmCheckcnt = ss.NmCheckcnt,
                        NmChecktime = ss.NmChecktime,
                    }).ToList()
                }).ToList();
        }

        private static Group CreateHeader(Group record)
        {
            return new Group
            {
                DtDelivery = record.DtDelivery,
                DtStart = record.DtStart,
                CdDistGroup = record.CdDistGroup,
                CdBlock = record.CdBlock,
                DtEnd = record.DtEnd,
                NmIdle = record.NmIdle,
            };
        }
    }
}
