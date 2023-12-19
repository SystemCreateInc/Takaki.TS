using LargeDist.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LargeDist.Views
{
    /// <summary>
    /// BlockListLine.xaml の相互作用ロジック
    /// </summary>
    public partial class BlockListLine : UserControl
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(BlockListLine), new PropertyMetadata(null));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(
           "Item", typeof(ChartItem), typeof(BlockListLine), new PropertyMetadata(null));

        public ChartItem Item
        {
            get => (ChartItem)GetValue(ItemProperty);
            set => SetValue(ItemProperty, value);
        }

        public BlockListLine()
        {
            InitializeComponent();
        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Command?.Execute(Item);
        }
    }
}
