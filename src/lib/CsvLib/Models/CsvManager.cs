using CsvHelper;
using LogLib;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace CsvLib.Models
{
    public class CsvManager
    {
        public static void Create<T>(List<T> datas, string filePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var writer = new StreamWriter(filePath, false, Encoding.GetEncoding("Shift_JIS")))
            using (var csv = new CsvWriter(writer, new CultureInfo("ja-JP", false)))
            {
                csv.WriteRecords(datas);
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
