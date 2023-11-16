using System.Windows.Controls;
using WindowLib.Utils;

namespace LightTest.Views
{
    /// <summary>
    /// Interaction logic for EditDlg
    /// </summary>
    public partial class SettingsDlg : UserControl
    {
        public SettingsDlg()
        {
            InitializeComponent();

            // Enter キーでフォーカス移動する
            this.KeyDown += (sender, e) =>
            {
                EnterKeySupport.Next(sender, e);
            };
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }
    }
}
