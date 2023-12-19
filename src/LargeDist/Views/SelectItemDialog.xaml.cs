using System.Windows.Controls;

namespace LargeDist.Views
{
    /// <summary>
    /// Interaction logic for SelectItemDialog
    /// </summary>
    public partial class SelectItemDialog : UserControl
    {
        public SelectItemDialog()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            dataGrid.Focus();
        }
    }
}
