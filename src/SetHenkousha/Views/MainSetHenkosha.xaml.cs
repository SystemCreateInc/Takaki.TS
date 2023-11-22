using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using WindowLib.Utils;

namespace SetHenkosha.Views
{
    /// <summary>
    /// Interaction logic for MainSetHenkosha
    /// </summary>
    public partial class MainSetHenkosha : UserControl
    {
        public MainSetHenkosha()
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Shain.Focus();
        }
    }
}
