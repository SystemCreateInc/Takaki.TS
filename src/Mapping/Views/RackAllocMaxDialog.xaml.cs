using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowLib.Utils;

namespace Mapping.Views
{
    /// <summary>
    /// Interaction logic for RackAllocMaxDialog
    /// </summary>
    public partial class RackAllocMaxDialog : UserControl
    {
        public RackAllocMaxDialog()
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
            RackAllocMax.Focus();
            RackAllocMax.SelectAll();
        }
    }
}
