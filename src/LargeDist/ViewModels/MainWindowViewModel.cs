using Prism.Mvvm;

namespace LargeDist.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "大仕分";
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
