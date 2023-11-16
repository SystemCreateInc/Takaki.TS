using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Threading;

namespace LightTest.Views
{
    /// <summary>
    /// Interaction logic for MainLightTest
    /// </summary>
    public partial class MainLightTest : UserControl
    {
        public MainLightTest()
        {
            InitializeComponent();
        }

        private void DisplayLog_TargetUpdated(object? sender, System.Windows.Data.DataTransferEventArgs e)
        {
            ((INotifyCollectionChanged)DisplayListBox.ItemsSource).CollectionChanged += new NotifyCollectionChangedEventHandler(DisplayLog_CollectionChanged);
#if false
            if (DisplayListBox.Items.Count < 1)
            {
                return;
            }

            DisplayListBox.ScrollIntoView(DisplayListBox.Items[this.DisplayListBox.Items.Count - 1]);
#endif
        }
        private void DisplayLog_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.InvokeAsync(new Action(() => 
            {
                if (DisplayListBox.Items.Count >= 1)
                {
                    DisplayListBox.ScrollIntoView(DisplayListBox.Items[DisplayListBox.Items.Count - 1]);
                }
            }));
        }
        private void TextBox_PreviewTextInput(object? sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // 0-9のみ
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        }
    }
}
