using System.Diagnostics;
using System.Windows.Controls;

namespace LargeDist.Views
{
    /// <summary>
    /// Interaction logic for ItemScan
    /// </summary>
    public partial class ItemScan : UserControl
    {
        public ItemScan()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Scancode.Focus();
        }

        private void UserControl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Scancode.Focus();
        }
    }
}
