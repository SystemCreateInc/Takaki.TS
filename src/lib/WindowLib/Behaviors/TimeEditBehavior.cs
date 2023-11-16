using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;

namespace WindowLib.Behaviors
{
    public class TimeEditBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register("SelectedTime", typeof(DateTime?),
            typeof(TimeEditBehavior),
            new FrameworkPropertyMetadata(default(DateTime?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedTimePropertyChangedCallback));

        private static void SelectedTimePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var teb = (TimeEditBehavior)d;
            teb.SelectedTime = (DateTime?)e.NewValue;
            teb.SetSelectedTimeToText();
        }

        public DateTime? SelectedTime
        {
            get => (DateTime?)GetValue(SelectedTimeProperty);
            set => SetValue(SelectedTimeProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.LostFocus += OnLostFocus;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (this.AssociatedObject != null)
                this.AssociatedObject.LostFocus -= OnLostFocus;
        }

        private void OnLostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            SelectedTime = ParseTime();
            SetSelectedTimeToText();
        }

        private void SetSelectedTimeToText()
        {
            if (SelectedTime == null)
            {
                AssociatedObject.Text = "";
            }
            else
            {
                AssociatedObject.Text = ((DateTime)SelectedTime).ToString("H:mm");
            }
        }

        private DateTime? ParseTime()
        {
            string text = AssociatedObject.Text;

            if (text == null || text == "" || text.Length < 3)
                return null;

            int hour, min;
            char[] delim = { ':', ',', ' ' };
            int pos = text.IndexOfAny(delim);
            if (pos != -1)
            {
                if (!int.TryParse(text.Substring(0, pos).Trim(delim), out hour)
                    || !int.TryParse(text.Substring(pos).Trim(delim), out min))
                    return null;
            }
            else
            {
                int num;
                if (!int.TryParse(text, out num))
                    return null;

                hour = num / 100;
                min = num % 100;
            }

            if (hour < 0 || hour > 23 || min < 0 || min > 59)
                return null;

            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, min, 0);
        }
    }
}
