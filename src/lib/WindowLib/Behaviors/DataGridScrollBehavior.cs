using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace WindowLib.Behaviors
{
    public class DataGridScrollBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty RowProperty = DependencyProperty.Register(
            "Row", typeof(int), typeof(DataGridScrollBehavior), new UIPropertyMetadata(-1, OnFocusedRowPropertyChanged));

        public static readonly DependencyProperty FocusProperty = DependencyProperty.Register(
            "Focus", typeof(bool), typeof(DataGridScrollBehavior), new UIPropertyMetadata(true));

        protected override void OnAttached()
        {
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        public int Row
        {
            get { return (int)GetValue(RowProperty); }
            set { SetValue(RowProperty, value); }
        }

        public bool Focus
        {
            get => (bool)GetValue(FocusProperty);
            set => SetValue(FocusProperty, value);
        }

        public void Scroll(int rowIndex)
        {
            if (rowIndex == -1 || AssociatedObject.Items.Count == 0)
                return;

            AssociatedObject.UpdateLayout();

            if (Focus)
            {
                AssociatedObject.Focus();
            }

            AssociatedObject.SelectedIndex = rowIndex;
            AssociatedObject.ScrollIntoView(AssociatedObject.Items.GetItemAt(rowIndex));
            AssociatedObject.BeginEdit();
        }

        private static void OnFocusedRowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bh = (DataGridScrollBehavior)d;
            var row = (int)e.NewValue;
            if (e.NewValue != e.OldValue && bh != null)
            {
                bh.Dispatcher.InvokeAsync(() => bh.Scroll(row));
            }
        }
    }
}