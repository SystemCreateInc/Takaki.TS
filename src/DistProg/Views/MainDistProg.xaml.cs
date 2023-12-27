using System.Windows;
using System.Windows.Controls;

namespace DistProg.Views
{
    /// <summary>
    /// MainDistProg.xaml の相互作用ロジック
    /// </summary>
    public partial class MainDistProg : UserControl
    {
        public MainDistProg()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid.Focus();
        }
    }
}
