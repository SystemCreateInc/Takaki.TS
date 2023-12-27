using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowLib.Utils;

namespace Mapping.Views
{
    /// <summary>
    /// Interaction logic for DeliveryDateDialog
    /// </summary>
    public partial class DeliveryDateDialog : UserControl
    {
        public DeliveryDateDialog()
        {
            InitializeComponent();
            // Enter キーでフォーカス移動する
            KeyDown += (sender, e) =>
            {
                if (Keyboard.FocusedElement is UIElement newFocus && newFocus.GetType().Equals(typeof(TextBox)))
                {
                    EnterKeySupport.Next(sender, e);
                }
            };
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Date.Focus();
        }
    }
}
