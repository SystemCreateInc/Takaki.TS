using System.Windows.Controls;

namespace LargeDist.Views
{
    /// <summary>
    /// Interaction logic for MainLargeDist
    /// </summary>
    public partial class MainLargeDist : UserControl
    {
        public MainLargeDist()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Person.Focus();
        }
    }
}
