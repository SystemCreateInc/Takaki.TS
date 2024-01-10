using LogLib;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using WindowLib.Utils;

namespace Mapping.Views
{
    /// <summary>
    /// Interaction logic for DeliveryDateDialog
    /// </summary>
    public partial class DeliveryDateDialog : UserControl
    {
        public DeliveryDateDialog()
        {
            InitializeComponent();
            // Enter キーでフォーカス移動する
            KeyDown += (sender, e) =>
            {
                if (Keyboard.FocusedElement is UIElement newFocus && newFocus.GetType().Equals(typeof(TextBox)))
                {
                    EnterKeySupport.Next(sender, e);
                }
            };
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Date.Focus();
        }

        private void DatePicker_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            if (Keyboard.FocusedElement is UIElement newFocus && newFocus.GetType().Equals(typeof(DatePickerTextBox)))
            {
                EnterKeySupport.Next(sender, e);
                e.Handled = true;
            }
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
