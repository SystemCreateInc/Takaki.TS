using Customer.Models;
using Microsoft.IdentityModel.Tokens;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowLib.Utils;

namespace Customer.Views
{
    /// <summary>
    /// InputCustomer.xaml の相互作用ロジック
    /// </summary>
    public partial class InputCustomer : UserControl
    {
        public InputCustomer()
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
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

        //private void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        //{
        //    var child = e.Row.Item as ChildCustomer;

        //    if(child is null)
        //    {
        //        return;
        //    }

        //    // 得意先名が取得出来ていない場合、編集を完了しない
        //    if (child.NmTokuisaki.IsNullOrEmpty())
        //    {
        //        e.Cancel = true;
        //    }
        //}
    }
}
