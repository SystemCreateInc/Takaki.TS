namespace TakahataDistListPrint.Models
{
    public class TakahataDistListPrintLoader
    {
        public static IEnumerable<TakahataDistListPrint> Get()
        {
            // fixme:読み込み機能
            return new TakahataDistListPrint[]
            {
                new TakahataDistListPrint
                {
                    CdShukkaBatch = "02001",
                    CdCourse = "Y61",
                    CdRoute = 1,
                    CdTokuisaki = "227577",
                    NmTokuisaki = "小谷SA売店",
                    CdHimban = "000022499",
                    CdJan = "4900000000001",
                    NmHinSeishikimei = "ミルクフランス",
                    QtSet = 14,
                    Obox = 5,
                    OBara = 3,
                    TotalOps = 73,
                    RemainingBox = 0,
                    RemainingBara = 0,
                    TotalRemainingps = 0,
                    Rbox = 5,
                    RBara = 3,
                    TotalRps = 73,
                },
                new TakahataDistListPrint
                {
                    CdShukkaBatch = "02001",
                    CdCourse = "Y61",
                    CdRoute = 2,
                    CdTokuisaki = "000002",
                    NmTokuisaki = "得意先名",
                    CdHimban = "000022499",
                    CdJan = "4900000000001",
                    NmHinSeishikimei = "ミルクフランス",
                    QtSet = 14,
                    Obox = 0,
                    OBara = 1,
                    TotalOps = 1,
                    RemainingBox = 0,
                    RemainingBara = 1,
                    TotalRemainingps = 1,
                    Rbox = 0,
                    RBara = 0,
                    TotalRps = 0,
                },
            };
        }
    }
}
