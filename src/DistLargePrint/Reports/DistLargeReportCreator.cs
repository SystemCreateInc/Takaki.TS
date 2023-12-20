namespace DistLargePrint.Reports
{
    public class DistLargeReportCreator
    {
        public static IEnumerable<DistLargeListViewModel> Create(string cdLargeGroup, string dtDelivery, IEnumerable<Models.DistLargePrint> distLargePrints)
        {
            var viewModels = new List<DistLargeListViewModel>();
            var records = Get(distLargePrints);
            var pageGroups = new List<List<Group>> { new List<Group>() };

            foreach (var record in records)
            {
                foreach (var (detail, index) in record.Details.Select((detail, index) => (detail, index)))
                {
                    // 最後の空白行で1行分を足す
                    var pageRowCount = pageGroups.LastOrDefault()?.Sum(x => x.Details.Count() + 2) ?? 0;

                    // レコードが最初の場合、品番、Jan、品名、総数の行を作成する
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
                        CdBlock = detail.CdBlock,
                        BoxOps = detail.BoxOps,
                        BaraOps = detail.BaraOps,
                        NuLops = detail.NuLops,
                        BoxRps = detail.BoxRps,
                        BaraRps = detail.BaraRps,
                        NuLrps = detail.NuLrps,
                        BoxRemainingPs = detail.BoxRemainingPs,
                        BaraRemainingPs = detail.BaraRemainingPs,
                        TotalRemainingPs = detail.TotalRemainingPs,
                        DtWorkdtLarge = detail.DtWorkdtLarge,
                        NMShainLarge = detail.NMShainLarge,
                    });
                }
            }

            var maxPage = pageGroups.Count;

            viewModels.AddRange(pageGroups.Select(x =>
            {
                return new DistLargeListViewModel()
                {
                    Page = viewModels.Count + 1,
                    PageMax = maxPage,
                    CdLargeGroup = cdLargeGroup,
                    DtDelivery = dtDelivery,
                    Groups = x.Select(xx => xx).ToArray(),
                };
            }));

            return viewModels;
        }

        private static List<Group> Get(IEnumerable<Models.DistLargePrint> distLargePrints)
        {
            // 品番、Jan、品名、入数、ブロックでグループ化する
            return distLargePrints
                .GroupBy(q => new { q.CdHimban, q.CdJan, q.NmHinSeishikimei, q.NuBoxunit, })
                .Select(x => new Group
                {
                    CdHimban = x.Key.CdHimban,
                    CdJan = x.Key.CdJan,
                    NmHinSeishikimei = x.Key.NmHinSeishikimei ?? string.Empty,
                    NuBoxunit = x.Key.NuBoxunit,
                    TotalPs = x.Sum(x => x.NuLops),
                    Details = x.Select(xx => new Detail
                    {
                        CdBlock = xx.CdBlock ?? string.Empty,
                        BoxOps = xx.NuBoxunit == 0 ? 0 : xx.NuLops / xx.NuBoxunit,
                        BaraOps = xx.NuBoxunit == 0 ? 0 : xx.NuLops % xx.NuBoxunit,
                        NuLops = xx.NuLops,
                        BoxRps = xx.NuBoxunit == 0 ? 0 : xx.NuLrps / xx.NuBoxunit,
                        BaraRps = xx.NuBoxunit == 0 ? 0 : xx.NuLrps % xx.NuBoxunit,
                        NuLrps = xx.NuLrps,
                        BoxRemainingPs = xx.NuBoxunit == 0 ? 0 : (xx.NuLops - xx.NuLrps) / xx.NuBoxunit,
                        BaraRemainingPs = xx.NuBoxunit == 0 ? (xx.NuLops - xx.NuLrps) : (xx.NuLops - xx.NuLrps) % xx.NuBoxunit,
                        TotalRemainingPs = xx.NuLops - xx.NuLrps,
                        DtWorkdtLarge = xx.DtWorkdtLarge,
                        NMShainLarge = xx.NMShainLarge,
                    }).ToList()
                }).ToList();
        }

        private static Group CreateHeader(Group record)
        {
            return new Group
            {
                CdHimban = record.CdHimban,
                CdJan = record.CdJan,
                NmHinSeishikimei = record.NmHinSeishikimei,
                NuBoxunit = record.NuBoxunit,
                TotalPs = record.TotalPs,
                TotalBoxPs = record.NuBoxunit == 0 ? 0 : record.TotalPs / record.NuBoxunit,
                TotalBaraPs = record.NuBoxunit == 0 ? 0 :record.TotalPs % record.NuBoxunit,
            };
        }
    }
}
