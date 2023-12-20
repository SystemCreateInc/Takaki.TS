using System.Windows.Controls;

namespace LargeDist.Views
{
    /// <summary>
    /// Interaction logic for ModifyQtyDialog
    /// </summary>
    public partial class ModifyQtyDialog : UserControl
    {
        public ModifyQtyDialog()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Piece.Focus();
        }
    }
}
