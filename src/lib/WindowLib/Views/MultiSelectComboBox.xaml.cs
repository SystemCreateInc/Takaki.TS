using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Diagnostics;

namespace WindowLib.Views
{
    /// <summary>
    /// Interaction logic for MultiSelectComboBox.xaml
    /// </summary>
    public partial class MultiSelectComboBox : UserControl
    {
        private ObservableCollection<Node> _nodeList;
        public MultiSelectComboBox()
        {
            InitializeComponent();
            _nodeList = new ObservableCollection<Node>();
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(MultiSelectComboBox), 
                 new FrameworkPropertyMetadata(null, new PropertyChangedCallback(MultiSelectComboBox.OnItemsSourceChanged)));

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IEnumerable<object>), typeof(MultiSelectComboBox), 
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(MultiSelectComboBox.OnSelectedItemsChanged)));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MultiSelectComboBox), new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty DefaultTextProperty =
            DependencyProperty.Register("DefaultText", typeof(string), typeof(MultiSelectComboBox), 
                new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(MultiSelectComboBox),
                new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty AllTextProperty =
            DependencyProperty.Register("AllText", typeof(string), typeof(MultiSelectComboBox),
                new UIPropertyMetadata("全て"));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public IEnumerable<object> SelectedItems
        {
            get => (IEnumerable<object>)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string DefaultText
        {
            get => (string)GetValue(DefaultTextProperty);
            set => SetValue(DefaultTextProperty, value);
        }

        public string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        public string AllText
        {
            get => (string)GetValue(AllTextProperty);
            set => SetValue(AllTextProperty, value);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectComboBox control = (MultiSelectComboBox)d;
            control.DisplayInControl();
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectComboBox control = (MultiSelectComboBox)d;
            control.SelectNodes();
            control.SetText();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox clickedBox = (CheckBox)sender;

            if ((string)clickedBox.Content == AllText)
            {
                foreach (Node node in _nodeList)
                {
                    node.IsSelected = clickedBox.IsChecked == true;
                }
            }
            else
            {
                //  全てチェックされていたら”全て”にチェックをする
                int _selectedCount = _nodeList.Count(x => x.IsSelected && x.Title != AllText);
                var allItem = _nodeList.FirstOrDefault(i => i.Title == AllText);
                if (allItem != null)
                {
                    allItem.IsSelected = _selectedCount == _nodeList.Count - 1;
                }
            }

            SetSelectedItems();
            SetText();

        }


        private void SelectNodes()
        {
            foreach (var value in SelectedItems)
            {
                Node? node = _nodeList.FirstOrDefault(i => i.Value == value);
                if (node != null)
                {
                    node.IsSelected = true;
                }
            }

            //  すべて選択されていたらすべてにチェックを入れる
            if (_nodeList.Where(i => (i.Value as string) != AllText && i.IsSelected).Count() == _nodeList.Count() - 1)
            {
                _nodeList.First().IsSelected = true;
            }
        }

        private void SetSelectedItems()
        {
            SelectedItems = _nodeList
                .Where(x => x.IsSelected && x.Title != AllText)
                .Select(x => x.Value)
                .ToArray();
        }

        private void DisplayInControl()
        {
            _nodeList.Clear();

            foreach (var value in this.ItemsSource)
            {
                _nodeList.Add(new Node(value, DisplayMemberPath));
            }

            if (_nodeList.Count > 0)
            {
                _nodeList.Insert(0, new Node(AllText, null));
            }

            MultiSelectCombo.ItemsSource = _nodeList;
        }

        private void SetText()
        {
            if (this.SelectedItems != null)
            {
                StringBuilder displayText = new StringBuilder();
                foreach (Node s in _nodeList)
                {
                    if (s.IsSelected == true && s.Title == AllText)
                    {
                        displayText = new StringBuilder();
                        displayText.Append(AllText);
                        break;
                    }
                    else if (s.IsSelected == true && s.Title != AllText)
                    {
                        displayText.Append(s.Title);
                        displayText.Append(',');
                    }
                }
                this.Text = displayText.ToString().TrimEnd(new char[] { ',' }); 
            }           
            // set DefaultText if nothing else selected
            if (string.IsNullOrEmpty(this.Text))
            {
                this.Text = this.DefaultText;
            }
        }

        private void MultiSelectCombo_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        }

        private void MultiSelectCombo_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        }

        private void MultiSelectCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((ComboBox)sender).SelectedItem = null;
        }
    }

    public class Node : INotifyPropertyChanged
    {
        private bool _isSelected;
        private string? _displayMemberPath;

        public object Value { get; set; }

        public Node(object value, string? displayMemberPath)
        {
            Value = value;
            _displayMemberPath = displayMemberPath;
        }

        public string Title
        {
            get 
            {
                return (string.IsNullOrEmpty(_displayMemberPath) 
                    ? Value.ToString()
                    : ((string?)Value.GetType().GetProperty(_displayMemberPath)?.GetValue(Value))) ?? "";
            }
        }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
