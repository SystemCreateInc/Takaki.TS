namespace DistLargeGroup.Models
{
    public class DistLargeGroupLoader
    {
        public static IEnumerable<DistLargeGroup> Get()
        {
            // fixme:読み込み機能追加
            return new DistLargeGroup[]
            {
                new DistLargeGroup { CdKyoten = "4201", CdLargeGroup = "001", CdLargeGroupName = "大仕分グループ名称" },
                new DistLargeGroup { CdKyoten = "4201", CdLargeGroup = "002", CdLargeGroupName = "大仕分グループ名称" },
                new DistLargeGroup { CdKyoten = "4201", CdLargeGroup = "003", CdLargeGroupName = "大仕分グループ名称" },
                new DistLargeGroup { CdKyoten = "4201", CdLargeGroup = "004", CdLargeGroupName = "大仕分グループ名称" },
                new DistLargeGroup { CdKyoten = "4201", CdLargeGroup = "005", CdLargeGroupName = "大仕分グループ名称" },
                new DistLargeGroup { CdKyoten = "4201", CdLargeGroup = "006", CdLargeGroupName = "大仕分グループ名称" },
                new DistLargeGroup { CdKyoten = "4201", CdLargeGroup = "007", CdLargeGroupName = "大仕分グループ名称" },
                new DistLargeGroup { CdKyoten = "4201", CdLargeGroup = "008", CdLargeGroupName = "大仕分グループ名称" },
            };
        }
    }
}
