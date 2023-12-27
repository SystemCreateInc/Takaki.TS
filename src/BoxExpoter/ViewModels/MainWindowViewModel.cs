using Prism.Mvvm;

namespace BoxExpoter.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "箱数実績データ送信";
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
