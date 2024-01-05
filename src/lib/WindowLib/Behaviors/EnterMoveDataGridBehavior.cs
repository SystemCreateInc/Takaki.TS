using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace WindowLib.Behaviors
{
    public class EnterMoveDataGridBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;
        }

        private int CurrentColumnIndex => AssociatedObject.Columns.IndexOf(AssociatedObject.CurrentColumn);

        private int CurrentRowIndex => AssociatedObject.Items.IndexOf(AssociatedObject.CurrentItem);

        private void AssociatedObject_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (CurrentColumnIndex == AssociatedObject.Columns.Count - 1)
                {
                    var nextColumnIdx = AssociatedObject.Columns.IndexOf(AssociatedObject.Columns.FirstOrDefault(x => !x.IsReadOnly));
                    if (nextColumnIdx == -1)
                    {
                        nextColumnIdx = 0;
                    }

                    MoveCell(CurrentRowIndex + 1, nextColumnIdx);
                }
                else
                {
                    MoveCell(CurrentRowIndex, CurrentColumnIndex + 1);
                }

                e.Handled = true;
            }
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
    }
}
