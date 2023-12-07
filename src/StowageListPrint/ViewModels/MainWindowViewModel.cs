using Prism.Mvvm;

namespace StowageListPrint.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "積付表発行";
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
