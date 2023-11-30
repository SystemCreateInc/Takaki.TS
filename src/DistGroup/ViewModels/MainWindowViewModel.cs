using Prism.Mvvm;

namespace DistGroup.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "仕分グループ登録";
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
