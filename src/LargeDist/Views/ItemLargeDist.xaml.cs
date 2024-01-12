using System.Windows.Controls;

namespace LargeDist.Views
{
    /// <summary>
    /// Interaction logic for ItemLargeDist
    /// </summary>
    public partial class ItemLargeDist : UserControl
    {
        public ItemLargeDist()
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
