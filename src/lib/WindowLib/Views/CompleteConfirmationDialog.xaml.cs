using System;
using System.Windows;
using System.Windows.Threading;

namespace WindowLib.Views
{
    /// <summary>
    /// Interaction logic for CompleteConfirmationDialog.xaml
    /// </summary>
    public partial class CompleteConfirmationDialog : Window
    {
        public CompleteConfirmationDialog(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }

        /// <summary>
        /// 画面表示0.5秒後自動削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
            timer.Start();
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                Close();
            };
        }
    }
}
