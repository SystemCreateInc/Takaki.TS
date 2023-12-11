using Prism.Mvvm;

namespace Picking.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "仕分処理";
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
