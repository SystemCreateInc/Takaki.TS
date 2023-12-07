using Prism.Mvvm;

namespace TakahataDistListPrint.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "タカハタDAS対象外仕分リスト発行";
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
