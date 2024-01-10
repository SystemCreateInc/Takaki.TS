using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowLib.Utils;

namespace DistBlock.Views
{
    /// <summary>
    /// InputDistBlockDlg.xaml の相互作用ロジック
    /// </summary>
    public partial class InputDistBlockDlg : UserControl
    {
        public InputDistBlockDlg()
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
                dataGrid.Focus();
            }
        }

        private void Regist_Click(object sender, RoutedEventArgs e)
        {
            // 入力中の値を確定させる為にフォーカス移動
            dataGrid2.Focus();
        }
    }
}
