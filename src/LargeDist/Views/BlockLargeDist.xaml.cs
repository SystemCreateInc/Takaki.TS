using System.Windows.Controls;

namespace LargeDist.Views
{
    /// <summary>
    /// Interaction logic for BlockLargeDist
    /// </summary>
    public partial class BlockLargeDist : UserControl
    {
        public BlockLargeDist()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            CompleteButton.Focus();
        }

        private void UserControl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                CompleteButton.Command.Execute(null);
                e.Handled = true;
            }
        }
    }
}
