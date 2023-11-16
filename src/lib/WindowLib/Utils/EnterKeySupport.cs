using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WindowLib.Utils
{
    public class EnterKeySupport
    {
        public static void Next(object sender, KeyEventArgs e, bool selectAll = true)
        {
            if (e.Key != Key.Enter)
                return;

            var direction = Keyboard.Modifiers == ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next;
            UIElement? elementWithFocus = Keyboard.FocusedElement as UIElement;
            if (elementWithFocus != null)
            {
                // カーソル移動
                elementWithFocus.MoveFocus(new TraversalRequest(direction));

                // 移動先のコントロール取得
                UIElement? NewFocus = Keyboard.FocusedElement as UIElement;
                if (NewFocus?.GetType()?.Equals(typeof(TextBox)) == true)
                {
                    TextBox? textBox = (TextBox)NewFocus;

                    if (selectAll)
                        textBox.SelectAll();
                    else
                        textBox.SelectionStart = textBox.Text.Length;   // カーソルを最後に移動
                }
            }
        }
    }
}
