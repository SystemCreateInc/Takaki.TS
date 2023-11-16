using System.Windows;
using System.Windows.Controls;

namespace SearchBoxLib.Views
{
    /// <summary>
    /// SearchBoxDlg.xaml の相互作用ロジック
    /// </summary>
    public partial class SearchBoxDlg : UserControl
    {
        public SearchBoxDlg()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox.Focus();
        }
    }
}
