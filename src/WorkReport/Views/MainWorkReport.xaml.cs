using LogLib;
using System.Windows.Controls;

namespace WorkReport.Views
{
    /// <summary>
    /// MainWorkReport.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWorkReport : UserControl
    {
        public MainWorkReport()
        {
            InitializeComponent();
        }

        private void DatePicker_DateValidationError(object sender, DatePickerDateValidationErrorEventArgs e)
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
                    var date = DateTime.ParseExact(text, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None);
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
