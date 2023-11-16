using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LightTest.Converter
{
    /// <summary>
    /// ステータス文字列⇒Visibility変換
    /// </summary>
    public class StatusVisibleConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string ?status = value?.ToString();

            if (status == null || status == "")
            {
                return Visibility.Hidden;
            }

            Visibility visible = status == "OFF" ? Visibility.Hidden : Visibility.Visible;

            return visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// 改行削除
    /// </summary>
    public class NewLineDeleteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string ?tmp = value?.ToString();
            return tmp==null ? "" : tmp.Replace(Environment.NewLine, " ");      // 改行をスペースに
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
