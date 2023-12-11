namespace ReferenceLogLib.Models
{
    public class CreateTekiyoSql
    {
        // 適用範囲内抽出SQL
        public static string GetFromRange(string tableName = "")
        {
            // （[開始日]が開始日以上、無効日未満 or [終了日]が開始日より上、無効日以下）
            return $@"(@startDate >= {tableName}DT_TEKIYOKAISHI and @startDate < {tableName}DT_TEKIYOMUKO or
                        @endDate > {tableName}DT_TEKIYOKAISHI and @endDate <= {tableName}DT_TEKIYOMUKO)";
        }

        // 適用範囲内抽出SQL(参照日)
        public static string GetFromDate(string tableName = "")
        {
            // 開始日以上、無効日未満
            return $"@selectDate >= {tableName}DT_TEKIYOKAISHI and @selectDate < {tableName}DT_TEKIYOMUKO";
        }

    }
}
