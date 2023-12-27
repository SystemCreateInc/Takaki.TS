using System.Windows.Controls;

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
    }
}
