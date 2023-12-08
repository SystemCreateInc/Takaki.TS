using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowLib.Utils;

namespace Picking.Views
{
    public partial class ShainDlg : UserControl
    {
        public ShainDlg()
        {
            InitializeComponent();

            // Enter キーでフォーカス移動する
            this.KeyDown += (sender, e) =>
            {
                EnterKeySupport.Next(sender, e);
            };
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            CdShainText.Focus();
        }

        /// <summary>
        /// テキストボックスフォーカス時全選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.SelectAll();
        }
    }
}
