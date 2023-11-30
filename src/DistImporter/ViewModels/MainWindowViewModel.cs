using Prism.Mvvm;

namespace DistImporter.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "出荷データ受信";
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
