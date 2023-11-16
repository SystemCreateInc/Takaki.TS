using Prism.Mvvm;

namespace LightTest.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "表示器テスト";
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
