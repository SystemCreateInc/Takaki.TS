using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace WindowLib.Behaviors
{
    public class ComboBoxBehavior
    {
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.RegisterAttached("MaxLength", typeof(int),
            typeof(ComboBoxBehavior), new UIPropertyMetadata(1, OnMaxLengthChanged));

        public static int GetMaxLength(DependencyObject obj)
        {
            return (int)obj.GetValue(MaxLengthProperty);
        }

        public static void SetMaxLength(DependencyObject obj, int value)
        {
            obj.SetValue(MaxLengthProperty, value);
        }

        public static void OnMaxLengthChanged
                (DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var comboBox = obj as ComboBox;
            if (comboBox == null) return;
            comboBox.Loaded +=
                (s, e) =>
                {
                    var textBox = comboBox.Template.FindName("PART_EditableTextBox", comboBox) as TextBox;
                    if (textBox == null) return;

                    textBox.MaxLength = (int)args.NewValue;
                };
        }
    }
}
