using Microsoft.Xaml.Behaviors;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WindowLib.Behaviors
{
    public class NumericTextBoxBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty DotProperty = DependencyProperty.RegisterAttached(
            "Dot", typeof(bool?), typeof(NumericTextBoxBehavior), null);

        public bool? Dot
        {
            get => (bool?)GetValue(DotProperty);
            set => SetValue(DotProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyDown += OnKeyDown;
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
            AssociatedObject.TextChanged += OnTextChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyDown -= OnKeyDown;
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
            AssociatedObject.TextChanged -= OnTextChanged;
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

            if ((Key.D0 <= e.Key && e.Key <= Key.D9) ||
                (Key.NumPad0 <= e.Key && e.Key <= Key.NumPad9) ||
                (Key.Delete == e.Key) || (Key.Back == e.Key) || (Key.Tab == e.Key) ||
                (e.Key == Key.Enter || e.Key == Key.Back || e.Key == Key.Clear))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;                                           // 数値以外なので一度trueにする

                if (Dot != null && Dot == true && (e.Key == Key.OemPeriod || e.Key == Key.Decimal))
                    e.Handled = false;
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (AssociatedObject.Text == null)
                AssociatedObject.Text = "";

            // 全角チェック（貼り付け対策）
            if (ZenkakuCheck(sender, e))
                e.Handled = true;
        }

        private bool ZenkakuCheck(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && !isOneByteChar(textBox.Text))
            {
                MatchCollection mc = Regex.Matches(textBox.Text, @"[0-9]");

                string tmp = "";
                foreach (Match m in mc)
                {
                    tmp += m.Value;
                }

                textBox.Text = tmp;
                return true;
            }
            return false;
        }

        private bool isOneByteChar(string str)
        {
            byte[] byte_data = System.Text.Encoding.GetEncoding(932).GetBytes(str);
            return byte_data.Length == str.Length;
        }
    }
}
