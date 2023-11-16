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

        public static DataGrid? Grid;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreparingCellForEdit += PreparingCellForEdit;
            Grid = AssociatedObject;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreparingCellForEdit -= PreparingCellForEdit;
        }

        //  編集時に内部のコンボボックスにフォーカスを合わせる
        private void PreparingCellForEdit(object? sender, DataGridPreparingCellForEditEventArgs e)
        {
            Grid?.Dispatcher.InvokeAsync(() =>
            {
                e.EditingElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                // 移動先のコントロール取得
                UIElement NewFocus = (UIElement)Keyboard.FocusedElement;

                if (NewFocus.GetType().Equals(typeof(TextBox)))
                {
                    TextBox textBox = (TextBox)NewFocus;
                    // 入力項目全選択
                    textBox.SelectAll();
                }

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

        public static int _column=-1;
        public static int _row = -1;


        public static void MoveCell(int rowIndex, int columnIndex)
        {
            if (rowIndex == -1 || columnIndex == -1 || Grid?.Items.Count == 0)
                return;

            if (Grid != null)
            {
                Grid.UpdateLayout();
                Grid.Focus();
                Grid.SelectedIndex = rowIndex;
                Grid.CurrentCell =
                    new DataGridCellInfo(Grid.Items[rowIndex], Grid.Columns[columnIndex]);
                Grid.BeginEdit();
            }

            //UIElement NewFocus = Keyboard.FocusedElement as UIElement;
            //Keyboard.Focus(NewFocus);
        }

        private static void OnFocusedColumnPropertyChanged(
               DependencyObject d,
               DependencyPropertyChangedEventArgs e)
        {
            _column = (int)e.NewValue;
            if (e.NewValue!= e.OldValue)
            {
                // 遅延実行(フォーカスが移動しないため)
                if(Grid != null)
                Grid.Dispatcher.InvokeAsync(() => MoveCell(_row, _column));
            }
        }
        private static void OnFocusedRowPropertyChanged(
               DependencyObject d,
               DependencyPropertyChangedEventArgs e)
        {
            _row = (int)e.NewValue;
            if (e.NewValue != e.OldValue)
            {
                // 遅延実行(フォーカスが移動しないため)
                if (Grid != null)
                    Grid.Dispatcher.InvokeAsync(() => MoveCell(_row, _column));
            }
        }
    }
}