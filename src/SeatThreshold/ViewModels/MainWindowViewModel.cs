using Prism.Mvvm;

namespace SeatThreshold.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "座席しきい値情報登録";
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
