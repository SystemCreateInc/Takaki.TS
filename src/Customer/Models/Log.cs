using Prism.Mvvm;

namespace Customer.Models
{
    public class Log : BindableBase
    {
        private bool _selected = false;
        public bool Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public string DispSelected => Selected ? "●" : string.Empty;

        private string _dtTekiyoKaishi = string.Empty;
        public string DtTekiyoKaishi
        {
            get => _dtTekiyoKaishi;
            set => SetProperty(ref _dtTekiyoKaishi, value);
        }

        private string _dtTekiyoMuko = string.Empty;
        public string DtTekiyoMuko
        {
            get => _dtTekiyoMuko;
            set => SetProperty(ref _dtTekiyoMuko, value);
        }

        private string _cdShain = string.Empty;
        public string CdShain
        {
            get => _cdShain;
            set => SetProperty(ref _cdShain, value);
        }
    }
}
