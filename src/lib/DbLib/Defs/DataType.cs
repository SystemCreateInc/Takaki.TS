using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLib.Defs
{
    namespace DbLib.Defs
    {
        public enum DataType
        {
            [Description("拠点マスタ")]
            Kyoten,

            [Description("社員マスタ")]
            Syain,

            [Description("得意先マスタ")]
            Tokuisaki,

            [Description("品目マスタ")]
            Himmoku,

            [Description("出荷バッチマスタ")]
            ShukkaBatch,

            [Description("固定名称マスタ")]
            Kotei,

            [Description("出荷データ")]
            Pick = 10,

            [Description("箱数予定データ")]
            Hako,

            [Description("出荷実績データ")]
            PickResult = 100,

            [Description("箱数実績データ")]
            HakoResult,
        }
    }
}
