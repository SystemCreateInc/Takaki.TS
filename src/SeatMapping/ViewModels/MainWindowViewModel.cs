using Prism.Mvvm;

namespace SeatMapping.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "座席マッピングポジション";
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
