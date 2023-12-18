using DbLib;

namespace LargeDistLabelLib
{
    public class LargeDistLabelRepository : ILabelRepository
    {
        public LargeDistLabel Get()
        {
            // fixme:読み込み機能
            return new LargeDistLabel
            {
                DtDelivery = "2018/10/15",
                CdJuchuBin = "001",
                CdBlock = "6",
                CdDistGroup = "02001",
                NmDistGroup = "広島1便(通常)",
                CdShukkaBatch = "02001",
                NmShukkaBatch = "広島常温1便",
                CdHimban = "000022499",
                CdJan = "4904730002302",
                NmHinSeishikimei = "ミルクフランス",
                NuBoxUnit = 12,
                BoxPs = 1,
                BaraPs = 2,
                TotalPs = 14,
            };
        }
    }
}
