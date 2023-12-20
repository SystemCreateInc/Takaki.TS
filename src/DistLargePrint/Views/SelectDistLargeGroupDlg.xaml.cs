using System.Windows.Controls;

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
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LargeDistGroupCombo.Focus();
        }
    }
}
