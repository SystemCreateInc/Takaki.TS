using System.ComponentModel;

namespace DistLargePrint.Models
{
    // ラジオボタン検索条件
    public enum SearchConditionType
    {
        [Description("全件")]
        All = 0,

        [Description("未処理・欠品")]
        Uncompleted = 1,
    }
}
