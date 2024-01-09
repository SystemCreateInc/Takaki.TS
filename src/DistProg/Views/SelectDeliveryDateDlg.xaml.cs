using LogLib;
using System.Windows.Controls;

namespace DistProg.Views
{
    /// <summary>
    /// SelectDeliveryDateDlg.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectDeliveryDateDlg : UserControl
    {
        public SelectDeliveryDateDlg()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Date.Focus();
        }

        private void Date_DateValidationError(object sender, DatePickerDateValidationErrorEventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            var datePicker = (DatePicker)sender;
            var text = datePicker.Text;
            var date = DateTime.Today.AddDays(1);

            try
            {
                // /を含まないかつ、8文字だった場合にDateTimeに変換
                if (!text.Contains('/') && text.Length == 8)
                {
                    date = DateTime.ParseExact(text, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None);
                }

                datePicker.SelectedDate = date;
            }
            catch (Exception er)
            {
                Syslog.Error($"Date_DateValidationError:{er.Message}");
            }
        }
    }
}
