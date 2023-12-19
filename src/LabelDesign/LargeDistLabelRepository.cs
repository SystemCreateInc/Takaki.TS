using LargeDistLabelLib;

namespace LabelDesign
{
    internal class LargeDistLabelRepository : ILabelRepository
    {
        public LargeDistLabel Get()
        {
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
                NmHinSeishikimei = "ミルクフランスあいうえおかきくけこさしすせそ",
                NuBoxUnit = 999,
                BoxPs = 99,
                BaraPs = 99,
                TotalPs = 99,
            };
        }
    }
}
