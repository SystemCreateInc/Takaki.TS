using System.Windows.Controls;

namespace BoxExpoter.Views
{
    /// <summary>
    /// Interaction logic for DeliveryDateDialog
    /// </summary>
    public partial class DeliveryDateDialog : UserControl
    {
        public DeliveryDateDialog()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Date.Focus();
        }
    }
}
