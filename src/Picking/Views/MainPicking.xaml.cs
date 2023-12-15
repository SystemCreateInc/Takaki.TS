using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Picking.Views
{
    /// <summary>
    /// Interaction logic for MainPicking
    /// </summary>
    public partial class MainPicking : UserControl
    {
        public MainPicking()
        {
            InitializeComponent();

            this.PreviewKeyDown += (sender, e) =>
            {
                distcolorgrid.Focus();
            };
        }
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            distcolorgrid.Focus();
        }
    }
}
