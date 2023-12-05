namespace DistLargePrint.Models
{
    public class DistLargePrintLoader
    {
        public static IEnumerable<DistLargePrint> Get()
        {
            // fixme:読み込み機能
            return new DistLargePrint[]
            {
                new DistLargePrint
                {
                    CdHimban = "227577",
                    CdJan = "4904730002302",
                    NmHinSeishikimei = "ミルクフランス",
                    QtSet = 14,
                    CdBlock = "1",
                    Obox = 3,
                    OBara = 0,
                    TotalOps = 42,
                    RemainingBox = 0,
                    RemainingBara = 0,
                    TotalRemainingps = 0,
                    Rbox = 3,
                    RBara = 0,
                    TotalRps = 42,
                    DtTorokuNichiji = DateTime.Now,
                    NmHenkosha = "佐藤一郎",
                },
                new DistLargePrint
                {
                    CdHimban = "227577",
                    CdJan = "4904730002302",
                    NmHinSeishikimei = "ミルクフランス",
                    QtSet = 14,
                    CdBlock = "2",
                    Obox = 2,
                    OBara = 3,
                    TotalOps = 31,
                    RemainingBox = 0,
                    RemainingBara = 0,
                    TotalRemainingps = 0,
                    Rbox = 2,
                    RBara= 3,
                    TotalRps = 31,
                    DtTorokuNichiji= DateTime.Now,
                    NmHenkosha = "佐藤一郎",
                },
                new DistLargePrint
                {
                    CdHimban = "000002",
                    CdJan = "4900000000002",
                    NmHinSeishikimei = "品名",
                    QtSet = 12,
                    CdBlock = "6",
                    Obox = 0,
                    OBara = 10,
                    TotalOps = 10,
                    RemainingBox = 0,
                    RemainingBara = 10,
                    TotalRemainingps = 10,
                    Rbox = 0,
                    RBara= 0,
                    TotalRps = 0,
                },
                new DistLargePrint
                {
                    CdHimban = "000003",
                    CdJan = "4900000000003",
                    NmHinSeishikimei = "品名",
                    QtSet = 12,
                    CdBlock = "2",
                    Obox = 0,
                    OBara = 10,
                    TotalOps = 10,
                    RemainingBox = 0,
                    RemainingBara = 10,
                    TotalRemainingps = 10,
                    Rbox = 0,
                    RBara= 0,
                    TotalRps = 0,
                },
                new DistLargePrint
                {
                    CdHimban = "000003",
                    CdJan = "4900000000003",
                    NmHinSeishikimei = "品名",
                    QtSet = 12,
                    CdBlock = "5",
                    Obox = 1,
                    OBara = 8,
                    TotalOps = 21,
                    RemainingBox = 1,
                    RemainingBara = 8,
                    TotalRemainingps = 21,
                    Rbox = 0,
                    RBara= 0,
                    TotalRps = 0,
                }
            };
        }
    }
}
