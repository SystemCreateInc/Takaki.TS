using System.Windows.Controls;
using WindowLib.Utils;

namespace Picking.Views
{
    /// <summary>
    /// Interaction logic for ModifyBoxUnitDialog
    /// </summary>
    public partial class ModifyBoxUnitDialog : UserControl
    {
        public ModifyBoxUnitDialog()
        {
            InitializeComponent();

            // Enter キーでフォーカス移動する
            this.KeyDown += (sender, e) =>
            {
                EnterKeySupport.Next(sender, e);
            };
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Value.Focus();
        }
    }
}
