using Prism.Mvvm;

namespace EnvironSetting.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "環境設定";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {

        }
    }
}
