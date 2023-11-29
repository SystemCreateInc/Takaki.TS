using DbLib.Defs;

namespace DistGroup.Models
{
    public class DistGroupLoader
    {
        public static IEnumerable<DistGroup> Get()
        {
            // fixme:読み込み機能追加
            return new DistGroup[]
            {
                new DistGroup
                {
                    CdDistGroup = "0200",
                    NmDistGroup = "仕分グループ名",
                    CdKyoten = "4201",
                    NmKyoten = "広島製品出荷",
                    CdBinSum = BinSumType.No,
                    CdLargeGroup = "001",
                    CdLargeGroupName = "大仕分グループ名",
                }
            };
        }
    }
}
