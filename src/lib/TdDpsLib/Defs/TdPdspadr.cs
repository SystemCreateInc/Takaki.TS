using System.ComponentModel;

namespace TdDpsLib.Defs
{
    public enum TdPdspadr
    {
        [Description("論理ｱﾄﾞﾚｽ")]
        Addr = 0,
        [Description("物理ｱﾄﾞﾚｽ")]
        Physical,
        [Description("表示桁数")]
        TextLen,
        [Description("エリア灯1")]
        Area1,
        [Description("エリア灯1Stno")]
        Area1Stno,
        [Description("エリア灯2")]
        Area2,
        [Description("エリア灯2Stno")]
        Area2Stno,
        [Description("使用有無")]
        IsUse,
        [Description("Stno")]
        Stno,
    }

}
