using Prism.Mvvm;

namespace DistBlock.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "仕分ブロック順登録";
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
