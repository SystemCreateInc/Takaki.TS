using Microsoft.Xaml.Behaviors;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WindowLib.Behaviors
{
    public class MaxBytesTextBoxBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty MaxBytesProperty = DependencyProperty.RegisterAttached(
            "MaxBytes", typeof(int?), typeof(MaxBytesTextBoxBehavior), null);

        public int? MaxBytes
        {
            get => (int?)GetValue(MaxBytesProperty);
            set => SetValue(MaxBytesProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += OnPreviewTextInput;
            AssociatedObject.TextChanged += OnTextChanged;
            AssociatedObject.Tag = this;
            TextCompositionManager.AddPreviewTextInputUpdateHandler(AssociatedObject, OnPreviewTextInputUpdate);
            TextCompositionManager.AddTextInputStartHandler(AssociatedObject, OnTextInputStart);
            DataObject.AddPastingHandler(AssociatedObject, OnPasting);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
            AssociatedObject.TextChanged -= OnTextChanged;
            TextCompositionManager.RemovePreviewTextInputUpdateHandler(AssociatedObject, OnPreviewTextInputUpdate);
            TextCompositionManager.RemoveTextInputStartHandler(AssociatedObject, OnTextInputStart);
            DataObject.RemovePastingHandler(AssociatedObject, OnPasting);
        }

        private bool _isIme = false;

        private bool IsValidLength(string text)
        {
            var newText = GetNewText(AssociatedObject, text);
            return IsBytesShort(newText, MaxBytes);
        }

        private void OnTextInputStart(object sender, TextCompositionEventArgs e)
        {
            Debug.Print("start");
            _isIme = false;
        }

        private void OnPreviewTextInputUpdate(object sender, TextCompositionEventArgs e)
        {
            _isIme = true;
            Debug.Print($"pti: {AssociatedObject.Text}");
        }

        private void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (_isIme)
            {
                if (!IsValidLength(""))
                {
                    DeleteOverflowText();
                    e.Handled = true;
                }
            }
            else
            {
                if (!IsValidLength(e.Text))
                {
                    e.Handled = true;
                }
            }
        }

        private void DeleteOverflowText()
        {
            if (MaxBytes == null)
            {
                return;
            }

            int max = (int)MaxBytes;
            var selectionStart = AssociatedObject.SelectionStart;
            var selectinoLength = AssociatedObject.SelectionLength;
            var text = AssociatedObject.Text;
            var enc = Encoding.GetEncoding("shift-jis");
            while (enc.GetByteCount(text) > max)
            {
                text = text.Remove(text.Length - 1);
            }

            AssociatedObject.Text = text;
            AssociatedObject.SelectionStart = selectionStart;
            AssociatedObject.SelectionLength = selectinoLength;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private static void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            var edit = (TextBox)sender;
            var be = (MaxBytesTextBoxBehavior)edit.Tag;

            var pastedText = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
            if (pastedText == null)
            {
                return;
            }

            var newText = GetNewText(edit, pastedText);
            if (!IsBytesShort(newText, be.MaxBytes))
            {
                Debug.Print("Limit max bytes");
                e.CancelCommand();
                e.Handled = true;
            }
        }

        private static string GetNewText(TextBox edit, string text)
        {
            return edit.Text
                .Remove(edit.SelectionStart, edit.SelectionLength)
                .Insert(edit.SelectionStart, text);
        }

        private static bool IsBytesShort(string text, int? limit)
        {
            if (limit == null)
            {
                return true;
            }

            var bytes = Encoding.GetEncoding("shift-jis").GetByteCount(text);
            Debug.Print($"length is {bytes}");
            return bytes <= limit;
        }
    }
}


