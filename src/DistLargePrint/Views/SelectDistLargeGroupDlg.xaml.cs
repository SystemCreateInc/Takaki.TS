using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using WindowLib.Utils;

namespace DistLargePrint.Views
{
    /// <summary>
    /// SelectDistLargeGroupDlg.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectDistLargeGroupDlg : UserControl
    {
        public SelectDistLargeGroupDlg()
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
            DeliveryDate.Focus();
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
    }
}
