using System.ComponentModel;

namespace DbLib.Defs
{
    // ロケーション対象外設定
    public enum RemoveType
    {
        // 対象
        [Description("")]
        Include = 0,

        [Description("対象外")]
        Remove = 1,
    }
}
