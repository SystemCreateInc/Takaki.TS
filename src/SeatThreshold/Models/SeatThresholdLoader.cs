using DbLib.Defs;

namespace SeatThreshold.Models
{
    public class SeatThresholdLoader
    {
        public static IEnumerable<SeatThreshold> Get()
        {
            // fixme:読み込み機能
            return new SeatThreshold[]
            {
                new SeatThreshold { CdKyoten = "4201", CdBlock = "1", TdUnitType = TdUnitType.TdCeiling, NuTdunitCnt = 128, NuThreshold = 13 },
                new SeatThreshold { CdKyoten = "4201", CdBlock = "2", TdUnitType = TdUnitType.TdCeiling, NuTdunitCnt = 128, NuThreshold = 13 },
                new SeatThreshold { CdKyoten = "4201", CdBlock = "3", TdUnitType = TdUnitType.TdCeiling, NuTdunitCnt = 128, NuThreshold = 13 },
                new SeatThreshold { CdKyoten = "4201", CdBlock = "4", TdUnitType = TdUnitType.TdCeiling, NuTdunitCnt = 128, NuThreshold = 13 },
                new SeatThreshold { CdKyoten = "4201", CdBlock = "5", TdUnitType = TdUnitType.TdRack, NuTdunitCnt = 256, NuThreshold = 4 },
                new SeatThreshold { CdKyoten = "4201", CdBlock = "6", TdUnitType = TdUnitType.TdRack, NuTdunitCnt = 256, NuThreshold = 4 },
                new SeatThreshold { CdKyoten = "4201", CdBlock = "11", TdUnitType = TdUnitType.TdCeiling, NuTdunitCnt = 128, NuThreshold = 13 },
                new SeatThreshold { CdKyoten = "4201", CdBlock = "15", TdUnitType = TdUnitType.TdRack, NuTdunitCnt = 256, NuThreshold = 4 },
            };
        }
    }
}
