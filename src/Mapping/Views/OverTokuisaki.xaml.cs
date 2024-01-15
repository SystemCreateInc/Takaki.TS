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

namespace Mapping.Views
{
    /// <summary>
    /// MainMapping.xaml の相互作用ロジック
    /// </summary>
    public partial class OverTokuisaki : UserControl
    {
        public OverTokuisaki()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // 毎回スクロールバーを先頭へ移動
            var border = VisualTreeHelper.GetChild(gridovertokuisaki, 0) as Decorator;
            if (border != null)
            {
                var scrollViewer = border.Child as ScrollViewer;
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollToTop();
                }
            }
        }
    }
}
