using Prism.Mvvm;

namespace Mapping.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "座席マッピング";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public MainWindowViewModel()
        {
        }
    }
}
