using System.Windows.Controls;

namespace LargeDist.Views
{
    /// <summary>
    /// Interaction logic for ModifyBoxUnitDialog
    /// </summary>
    public partial class ModifyBoxUnitDialog : UserControl
    {
        public ModifyBoxUnitDialog()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Value.Focus();
        }
    }
}
