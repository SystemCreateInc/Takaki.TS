using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WindowLib.Behaviors
{
    public class DataGridFocusBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty ColumnProperty = DependencyProperty.Register(
            "Column", typeof(int), typeof(DataGridFocusBehavior), new UIPropertyMetadata(-1, OnFocusedColumnPropertyChanged));

        public static readonly DependencyProperty RowProperty = DependencyProperty.Register(
            "Row", typeof(int), typeof(DataGridFocusBehavior), new UIPropertyMetadata(-1, OnFocusedRowPropertyChanged));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreparingCellForEdit += PreparingCellForEdit;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreparingCellForEdit -= PreparingCellForEdit;
        }

        //  編集時に内部のコンボボックスにフォーカスを合わせる
        private void PreparingCellForEdit(object? sender, DataGridPreparingCellForEditEventArgs e)
        {
            AssociatedObject.Dispatcher.InvokeAsync(() =>
            {
                e.EditingElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                Column = -1;
                Row = -1;
            });
        }

        public int Column
        {
            get { return (int)GetValue(ColumnProperty); }
            set { SetValue(ColumnProperty, value); }
        }

        public int Row
        {
            get { return (int)GetValue(RowProperty); }
            set { SetValue(RowProperty, value); }
        }


        public void MoveCell(int rowIndex, int columnIndex)
        {
            if (rowIndex == -1 || columnIndex == -1 || AssociatedObject.Items.Count == 0)
                return;

            AssociatedObject.UpdateLayout();
            AssociatedObject.Focus();
            AssociatedObject.SelectedIndex = rowIndex;
            AssociatedObject.CurrentCell =
                new DataGridCellInfo(AssociatedObject.Items[rowIndex], AssociatedObject.Columns[columnIndex]);
            AssociatedObject.BeginEdit();
        }

        private static void OnFocusedColumnPropertyChanged(
               DependencyObject d,
               DependencyPropertyChangedEventArgs e)
        {
            var bh = (DataGridFocusBehavior)d;
            if (bh.AssociatedObject == null)
            {
                return;
            }

            if (e.NewValue!= e.OldValue)
            {
                bh.MoveCell(bh.Row, bh.Column);
            }
        }

        private static void OnFocusedRowPropertyChanged(
               DependencyObject d,
               DependencyPropertyChangedEventArgs e)
        {
            var bh = (DataGridFocusBehavior)d;
            if (bh.AssociatedObject == null)
            {
                return;
            }

            if (e.NewValue != e.OldValue)
            {
                bh.MoveCell(bh.Row, bh.Column);
            }
        }
    }
}