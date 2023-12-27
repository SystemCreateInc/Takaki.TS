using System.Linq;

namespace TakakiLib.Models
{
    // 適用日関連Sql
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

        // 最終更新日の適用データ抽出(join)
        public static string GetFromLastUpdateJoin(string tableName, string keyColumnNames)
        {
            var onColumnsSql = keyColumnNames.Split(",").Select(x => $"t1_2.{x} = t1.{x}");

            // 適用開始日以外のキーで更新日降順にし、最初のデータとjoin
            return $" join(select {keyColumnNames}, DT_TEKIYOKAISHI"
                  +$",ROW_NUMBER() OVER(PARTITION BY {keyColumnNames} ORDER BY updatedAt desc) row"
                  +$" from {tableName}) t1_2"
                  +$" on {string.Join(" and ", onColumnsSql)} and t1_2.DT_TEKIYOKAISHI = t1.DT_TEKIYOKAISHI and t1_2.row = 1";
        }

    }
}
