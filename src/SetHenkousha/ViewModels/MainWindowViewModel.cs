using Prism.Mvvm;

namespace SetHenkosha.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "社員選択";
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
