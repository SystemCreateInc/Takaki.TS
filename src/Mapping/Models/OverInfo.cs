using Prism.Mvvm;

namespace Mapping.Models
{
    public class OverInfo : BindableBase
    {
        private string _cdDistGroup = string.Empty;
        public string CdDistGroup
        {
            get => _cdDistGroup;
            set => SetProperty(ref _cdDistGroup, value);
        }

        private string _nmDistGroup = string.Empty;
        public string NmDistGroup
        {
            get => _nmDistGroup;
            set => SetProperty(ref _nmDistGroup, value);
        }
        private string _cdShukkaBatch = string.Empty;
        public string CdShukkaBatch
        {
            get => _cdShukkaBatch;
            set => SetProperty(ref _cdShukkaBatch, value);
        }

        private string _nmShukkaBatch = string.Empty;
        public string NmShukkaBatch
        {
            get => _nmShukkaBatch;
            set => SetProperty(ref _nmShukkaBatch, value);
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

        private string _cdCourse = string.Empty;
        public string CdCourse
        {
            get => _cdCourse;
            set => SetProperty(ref _cdCourse, value);
        }

        private int _cdRoute = 0;
        public int CdRoute
        {
            get => _cdRoute;
            set => SetProperty(ref _cdRoute, value);
        }
    }
}
