using Prism.Mvvm;

namespace DistProg.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "進捗照会";
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
