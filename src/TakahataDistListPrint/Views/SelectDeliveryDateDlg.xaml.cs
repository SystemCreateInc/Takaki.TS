using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using WindowLib.Utils;

namespace TakahataDistListPrint.Views
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

        private void DatePicker_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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
    }
}
