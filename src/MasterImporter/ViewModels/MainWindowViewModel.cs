using Prism.Mvvm;

namespace MasterImporter.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "マスタ受信";
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
