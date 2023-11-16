using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace WindowLib.Behaviors
{
    public class DataGridDisableManipulationBoundaryFeedbackBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.ManipulationBoundaryFeedback += ManipulationBoundaryFeedback;
        }

        private void ManipulationBoundaryFeedback(object? sender, System.Windows.Input.ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.ManipulationBoundaryFeedback -= ManipulationBoundaryFeedback;
        }
    }
}
