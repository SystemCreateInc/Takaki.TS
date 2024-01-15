using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Controls;
using WindowLib.Utils;

namespace CsvLib.Models
{
    public class CsvManager
    {
        public static void Create(DataGrid dataGrid, string fileName)
        {
            using (var busy = new WaitCursor())
            {
                using (var fs = new StreamWriter(fileName, false, Encoding.GetEncoding("SJIS")))
                {
                    // ヘッダー取得
                    var header = string.Join(",", dataGrid.Columns.Select(x => x.Header.ToString()));

                    // ヘッダー書き込み
                    fs.WriteLine(header);

                    // 
                    for (int row = 0; row < dataGrid.Items.Count; row++)
                    {
                        var line = new List<string>();
                        for (int column = 0; column < dataGrid.Columns.Count; column++)
                        {
                            var property = dataGrid.Items[row].GetType().GetProperty(dataGrid.Columns[column].SortMemberPath);
                            var format = dataGrid.Columns[column].ClipboardContentBinding.StringFormat;
                            var b = dataGrid.Items[row];
                            line.Add(FormatString(property, dataGrid.Items[row], format));
                        }

                        var rowText = string.Join(",", line);
                        fs.WriteLine(rowText);
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

        // 書式設定
        private static string FormatString(PropertyInfo? property, object? obj, string? format)
        {
            if (property == null || obj == null)
            {
                return string.Empty;
            }

            var value = property.GetValue(obj);

            if (value == null)
            {
                return string.Empty;
            }

            if (format == null)
            {
                // StringFormatが指定されていない場合は、文字として返す
                return value.ToString() ?? string.Empty;
            }
            else
            {
                // StringFormatが指定されているので、formatしてから返す
                // DateTime型は、カスタム書式設定で指定されているため別処理でformatする
                if (IsDateTime(property))
                {
                    return SetDateTimeFormatValue(value, format);
                }
                else if(IsTimeSpan(property))
                {
                    return SetTimeSpanFormatValue(value, format);
                }
                else
                {
                    var formatString = string.Format(format, value);
                    return formatString;
                }
            }
        }

        private static bool IsDateTime(PropertyInfo property)
        {
            var methodInfo = property.GetMethod;
            if (methodInfo == null)
            {
                return false;
            }

            var fullName = methodInfo.ReturnType.FullName;
            if (fullName == null)
            {
                return false;
            }

            return fullName.Contains("DateTime");
        }

        private static bool IsTimeSpan(PropertyInfo property)
        {
            var methodInfo = property.GetMethod;
            if(methodInfo == null)
            {
                return false;
            }

            var fullName = methodInfo.ReturnType.FullName;
            if(fullName == null)
            {
                return false;
            }

            return fullName.Contains("TimeSpan");
        }

        // DateTimeの日付フォーマットの設定
        private static string SetDateTimeFormatValue(object value, string format)
        {
            DateTime dateTime;

            if (DateTime.TryParse(value?.ToString(), out dateTime))
                return dateTime.ToString(format);
            else
                return "";
        }

        // TimeSpanの日付フォーマットの設定
        private static string SetTimeSpanFormatValue(object value, string format)
        {
            TimeSpan timeSpan;

            if (TimeSpan.TryParse(value?.ToString(), out timeSpan))
                return timeSpan.ToString(format);
            else
                return "";
        }
    }
}
