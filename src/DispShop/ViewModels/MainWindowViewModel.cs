using Prism.Mvvm;

namespace DispShop.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "店舗表示処理";
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
