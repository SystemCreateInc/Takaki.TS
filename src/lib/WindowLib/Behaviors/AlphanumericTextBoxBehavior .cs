using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Input;

namespace WindowLib.Behaviors
{
    // 半角英数字入力テキストボックス
    public class AlphanumericTextBoxBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyDown += OnKeyDown;
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyDown -= OnKeyDown;
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            if ((Key.D0 <= e.Key && e.Key <= Key.D9 && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
                || (Key.NumPad0 <= e.Key && e.Key <= Key.NumPad9)
                || (Key.A <= e.Key && e.Key <= Key.Z)
                || Key.Delete == e.Key || Key.Back == e.Key || Key.Tab == e.Key
                || e.Key == Key.Enter || e.Key == Key.Back || e.Key == Key.Clear
                || (e.Key == Key.OemPeriod && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
                || e.Key == Key.Decimal)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
