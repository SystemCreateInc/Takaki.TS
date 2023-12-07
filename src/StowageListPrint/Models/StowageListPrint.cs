using Prism.Mvvm;

namespace StowageListPrint.Models
{
    public class StowageListPrint : BindableBase
    {
        private string _tdunitcode = string.Empty;
        public string Tdunitcode
        {
            get => _tdunitcode;
            set => SetProperty(ref _tdunitcode, value);
        }

        private string _cdShukkaBatch = string.Empty;
        public string CdShukkaBatch
        {
            get => _cdShukkaBatch;
            set => SetProperty(ref _cdShukkaBatch, value);
        }

        private string _cdCourse = string.Empty;
        public string CdCourse
        {
            get => _cdCourse;
            set => SetProperty(ref _cdCourse, value);
        }

        private int _cdRoute;
        public int CdRoute
        {
            get => _cdRoute;
            set => SetProperty(ref _cdRoute, value);
        }

        private string _cdTokuisaki = string.Empty;
        public string CdTokuisaki
        {
            get => _cdTokuisaki;
            set => SetProperty(ref _cdTokuisaki, value);
        }

        private string _nmTokuisaki = string.Empty;
        public string NmTokuisaki
        {
            get => _nmTokuisaki;
            set => SetProperty(ref _nmTokuisaki, value);
        }

        private int _largeBox;
        public int LargeBox
        {
            get => _largeBox;
            set => SetProperty(ref _largeBox, value);
        }

        private int _smallBox;
        public int SmallBox
        {
            get => _smallBox;
            set => SetProperty(ref _smallBox, value);
        }

        private int _blueBox;
        public int BlueBox
        {
            get => _blueBox;
            set => SetProperty(ref _blueBox, value);
        }

        private int _etcBox;
        public int EtcBox
        {
            get => _etcBox;
            set => SetProperty(ref _etcBox, value);
        }
    }
}
