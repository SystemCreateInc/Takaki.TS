using DistGroup.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowLib.Utils;

namespace DistGroup.Views
{
    /// <summary>
    /// InputDistGroupDlg.xaml の相互作用ロジック
    /// </summary>
    public partial class InputDistGroupDlg : UserControl
    {
        public InputDistGroupDlg()
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

        private void Regist_Click(object sender, RoutedEventArgs e)
        {
            dataGrid2.Focus();
        }
    }
}
