using System.ComponentModel;

namespace DbLib.Defs
{
    // 配送便集計
    public enum BinSumType
    {
        [Description("する")]
        Yes = 1,

        [Description("しない")]
        No = 2,
    }
}
