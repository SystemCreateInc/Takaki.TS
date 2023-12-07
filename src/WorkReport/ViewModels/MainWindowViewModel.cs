using Prism.Mvvm;

namespace WorkReport.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "作業報告書発行";
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
