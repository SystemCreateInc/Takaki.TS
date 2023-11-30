using Prism.Mvvm;

namespace Customer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "集約得意先登録";
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
