using Prism.Mvvm;

namespace SearchBoxLib.Models
{
    public class Content : BindableBase
    {
        private bool _isChecked = false;
        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }

        private string _contentName = string.Empty;
        public string ContentName
        {
            get => _contentName;
            set => SetProperty(ref _contentName, value);
        }

        private string _tableName = string.Empty;
        public string TableName
        {
            get => _tableName;
            set => SetProperty(ref _tableName, value);
        }
    }
}
