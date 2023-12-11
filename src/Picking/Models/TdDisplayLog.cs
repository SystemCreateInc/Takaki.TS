using Prism.Mvvm;

namespace Picking.Models
{
    public class TdDisplayLog : BindableBase
    {
        private string _logText = "";
        public string LogText
        {
            get => _logText;
            set => SetProperty(ref _logText, value);
        }

        // 表示色
        private string _logColor = "";
        public string LogColor
        {
            get => _logColor;
            set => SetProperty(ref _logColor, value);
        }
    }
}
