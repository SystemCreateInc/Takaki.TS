using Prism.Mvvm;

namespace DistListPrint.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "仕分・欠品リスト発行";
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
