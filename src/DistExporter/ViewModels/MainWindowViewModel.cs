using Prism.Mvvm;

namespace DistExpoter.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "出荷実績データ送信";
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
