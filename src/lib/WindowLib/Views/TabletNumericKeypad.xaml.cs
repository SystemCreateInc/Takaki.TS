using System.Windows.Controls;
using System.Windows.Input;

namespace WindowLib.Views
{
    /// <summary>
    /// Interaction logic for TabletNumericKeypad
    /// </summary>
    public partial class TabletNumericKeypad : UserControl
    {
        public TabletNumericKeypad()
        {
            InitializeComponent();
        }

        private void Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender == button_0)
            {
                SendText("0");
            }
            else if (sender == button_1)
            {
                SendText("1");
            }
            else if (sender == button_2)
            {
                SendText("2");
            }
            else if (sender == button_3)
            {
                SendText("3");
            }
            else if (sender == button_4)
            {
                SendText("4");
            }
            else if (sender == button_5)
            {
                SendText("5");
            }
            else if (sender == button_6)
            {
                SendText("6");
            }
            else if (sender == button_7)
            {
                SendText("7");
            }
            else if (sender == button_8)
            {
                SendText("8");
            }
            else if (sender == button_9)
            {
                SendText("9");
            }
            else if (sender == button_bs)
            {
                SendKey(Key.Back);
            }
            else if (sender == button_enter)
            {
                SendKey(Key.Enter);
                return;
            }
            else if (sender == button_del)
            {
                SendKey(Key.Delete);
            }
        }

        private void SendText(string text)
        {
            var target = Keyboard.FocusedElement;
            if (target == null)
                return;

            target.RaiseEvent(new TextCompositionEventArgs(
                InputManager.Current.PrimaryKeyboardDevice,
                new TextComposition(InputManager.Current, target, text))
            {
                RoutedEvent = TextCompositionManager.TextInputEvent
            });
        }

        private void SendKey(Key key)
        {
            var target = Keyboard.FocusedElement;
            if (target == null)
                return;

            target.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice,
                Keyboard.PrimaryDevice.ActiveSource, 0, key)
            {
                RoutedEvent = Keyboard.PreviewKeyDownEvent
            });

            target.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice,
                Keyboard.PrimaryDevice.ActiveSource, 0, key)
            {
                RoutedEvent = Keyboard.KeyDownEvent
            });
        }
    }
}
