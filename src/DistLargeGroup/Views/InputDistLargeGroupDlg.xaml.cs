using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DistLargeGroup.Views
{
    /// <summary>
    /// InputDistLargeGroupDlg.xaml の相互作用ロジック
    /// </summary>
    public partial class InputDistLargeGroupDlg : UserControl
    {
        public InputDistLargeGroupDlg()
        {
            InitializeComponent();
            PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    var direction = Keyboard.Modifiers == ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next;
                    (FocusManager.GetFocusedElement(Window.GetWindow(this)) as FrameworkElement)?.MoveFocus(new TraversalRequest(direction));
                }
            };
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var a = FocusManager.GetFocusedElement(Window.GetWindow(this));
            if (CdKyoten.IsEnabled)
            {
                CdKyoten.Focus();
            }
            else
            {
                ReferenceDate.Focus();
            }
        }

        private void Regist_Click(object sender, RoutedEventArgs e)
        {
            dataGrid2.Focus();
        }
    }
}
