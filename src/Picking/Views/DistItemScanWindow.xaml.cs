using System.Windows.Controls;
using WindowLib.Utils;

namespace Picking.Views
{
    /// <summary>
    /// ItemInfo.xaml の相互作用ロジック
    /// </summary>
    public partial class DistItemScanWindow : UserControl
    {
        public DistItemScanWindow()
        {
            InitializeComponent();

            this.PreviewKeyDown += (sender, e) =>
            {
                SCANCODE.Focus();
            };
        }
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SCANCODE.Focus();
        }
    }
}
