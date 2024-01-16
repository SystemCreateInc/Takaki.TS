using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using WindowLib.Utils;

namespace CsvLib.Models
{
    public class CsvManager
    {
        public static void Create(DataGrid dataGrid, IEnumerable<string> rows, string fileName)
        {
            using (var busy = new WaitCursor())
            {
                using (var fs = new StreamWriter(fileName, false, Encoding.GetEncoding("SJIS")))
                {
                    // ヘッダーをDataGridから取得して書き込み
                    var header = string.Join(",", dataGrid.Columns.Select(x => x.Header.ToString()));
                    fs.WriteLine(header);

                    // 一覧の書き込み
                    foreach (var row in rows)
                    {
                        fs.WriteLine(row);
                    }
                }
            }
        }

        public static List<T> Load<T>(string filePath)
        {
            var config = new CsvHelper.Configuration.CsvConfiguration(new CultureInfo("ja-JP", false));

            using (var reader = new StreamReader(filePath, Encoding.GetEncoding("Shift_JIS")))
            using (var csv = new CsvReader(reader, config))
            {
                try
                {
                    return csv.GetRecords<T>().ToList();
                }
                catch (System.Exception)
                {
                    // 型相違CSV選択でエラー
                    throw new System.Exception("選択したCSVを確認して下さい");
                }
            }
        }
    }
}
