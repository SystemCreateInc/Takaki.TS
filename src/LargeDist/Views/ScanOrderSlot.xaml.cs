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
using WindowLib.Behaviors;

namespace LargeDist.Views
{
    /// <summary>
    /// ScanOrderSlot.xaml の相互作用ロジック
    /// </summary>
    public partial class ScanOrderSlot : UserControl
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand),
            typeof(ScanOrderSlot), new PropertyMetadata(null));

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.RegisterAttached("Index", typeof(int?),
            typeof(ScanOrderSlot), new PropertyMetadata(null));

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.RegisterAttached("Id", typeof(int),
            typeof(ScanOrderSlot), new PropertyMetadata(null));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public int? Index
        {
            get => (int?)GetValue(IndexProperty);
            set => SetValue(IndexProperty, value);
        }

        public int Id
        {
            get => (int)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        public ScanOrderSlot()
        {
            InitializeComponent();
        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Command?.Execute(Id);
        }
    }
}
