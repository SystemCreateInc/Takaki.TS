using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowLib.Utils;

namespace SeatThreshold.Views
{
    /// <summary>
    /// InputSeatThresholdDlg.xaml の相互作用ロジック
    /// </summary>
    public partial class InputSeatThresholdDlg : UserControl
    {
        public InputSeatThresholdDlg()
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
            if (CdKyoten.IsEnabled)
            {
                CdKyoten.Focus();
            }
            else
            {
                ReferenceDate.Focus();
            }
        }
    }
}
