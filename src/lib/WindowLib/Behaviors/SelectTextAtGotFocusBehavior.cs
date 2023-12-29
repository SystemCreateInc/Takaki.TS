using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WindowLib.Behaviors
{
    public class SelectTextAtGotFocusBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotFocus += AssociatedObject_GotFocus;
        }

        private void AssociatedObject_GotFocus(object sender, RoutedEventArgs e)
        {
            AssociatedObject.SelectAll();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.GotFocus -= AssociatedObject_GotFocus;
        }
    }
}
