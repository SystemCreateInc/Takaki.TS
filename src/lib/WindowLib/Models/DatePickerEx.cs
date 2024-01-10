using LogLib;
using System;
using System.Globalization;
using System.Windows.Controls;

namespace WindowLib.Models
{
    public partial class DatePickerEx : DatePicker
    {
        public DatePickerEx() 
        {
            DateValidationError += DatePickerSt_DateValidationError;
        }

        private void DatePickerSt_DateValidationError(object? sender, DatePickerDateValidationErrorEventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            var datePicker = (DatePicker)sender;
            var text = datePicker.Text;

            try
            {
                // /を含まないかつ、8文字だった場合にDateTimeに変換
                if (!text.Contains('/') && text.Length == 8)
                {
                    var date = DateTime.ParseExact(text, "yyyyMMdd", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
                    datePicker.SelectedDate = date;
                }
            }
            catch (Exception er)
            {
                Syslog.Error($"Date_DateValidationError:{er.Message}");
            }
        }
    }
}
