namespace DistBlock.Models
{
    public class DistBlockLoader
    {
        public static IEnumerable<DistBlock> Get()
        {
            // fixme:読み込み機能追加
            return new DistBlock[]
            {
                new DistBlock { CdKyoten = "4201", NmKyoten = "広島製品出荷", CdDistGroup = "02001", NmDistGroup = "広島常温1便", },
                new DistBlock { CdKyoten = "4201", NmKyoten = "広島製品出荷", CdDistGroup = "02002", NmDistGroup = "仕分G名称-", },
            };
        }
    }
}
