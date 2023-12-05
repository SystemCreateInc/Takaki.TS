using Prism.Mvvm;

namespace DistLargePrint.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "大仕分リスト発行";
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
