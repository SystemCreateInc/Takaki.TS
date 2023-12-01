using Prism.Mvvm;

namespace DistLargeGroup.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "大仕分グループ情報登録";
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
