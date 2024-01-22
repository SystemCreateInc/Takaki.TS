using CsvLib.Models;
using Prism.Mvvm;

namespace StowageListPrint.Models
{
    public class StowageListPrint : BindableBase, ICsvData
    {
        // 積付数更新用
        private List<long> _idStowages = new List<long>();
        public List<long> IdStowages
        {
            get => _idStowages;
            set => SetProperty(ref _idStowages, value);
        }

        private string _cdBlock = string.Empty;
        public string CdBlock
        {
            get => _cdBlock;
            set => SetProperty(ref _cdBlock, value);
        }


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

        private int _largeBoxOps;
        public int LargeBoxOps
        {
            get => _largeBoxOps;
            set => SetProperty(ref _largeBoxOps, value);
        }

        private int _largeBoxRps;
        public int LargeBoxRps
        {
            get => _largeBoxRps;
            set => SetProperty(ref _largeBoxRps, value);
        }

        private int _dispLargeBoxPs;
        public int DispLargeBoxPs
        {
            get => _dispLargeBoxPs;
            set => SetProperty(ref _dispLargeBoxPs, value);
        }

        private int _smallBoxOps;
        public int SmallBoxOps
        {
            get => _smallBoxOps;
            set => SetProperty(ref _smallBoxOps, value);
        }

        private int _smallBoxRps;
        public int SmallBoxRps
        {
            get => _smallBoxRps;
            set => SetProperty(ref _smallBoxRps, value);
        }

        private int _dispSmallBoxPs;
        public int DispSmallBoxPs
        {
            get => _dispSmallBoxPs;
            set => SetProperty(ref _dispSmallBoxPs, value);
        }

        private int _blueBoxOps;
        public int BlueBoxOps
        {
            get => _blueBoxOps;
            set => SetProperty(ref _blueBoxOps, value);
        }

        private int _blueBoxRps;
        public int BlueBoxRps
        {
            get => _blueBoxRps;
            set => SetProperty(ref _blueBoxRps, value);
        }

        private int _dispBlueBoxPs;
        public int DispBlueBoxPs
        {
            get => _dispBlueBoxPs;
            set => SetProperty(ref _dispBlueBoxPs, value);
        }

        private int _etcBoxOps;
        public int EtcBoxOps
        {
            get => _etcBoxOps;
            set => SetProperty(ref _etcBoxOps, value);
        }

        private int _etcBoxRps;
        public int EtcBoxRps
        {
            get => _etcBoxRps;
            set => SetProperty(ref _etcBoxRps, value);
        }

        private int _dispEtcBoxPs;
        public int DispEtcBoxPs
        {
            get => _dispEtcBoxPs;
            set => SetProperty(ref _dispEtcBoxPs, value);
        }

        private DateTime? _dtWorkdtStowage;
        public DateTime? DtWorkdtStowage
        {
            get => _dtWorkdtStowage;
            set => SetProperty(ref _dtWorkdtStowage, value);
        }

        private string _henkoshaCode = string.Empty;
        public string HenkoshaCode
        {
            get => _henkoshaCode;
            set => SetProperty(ref _henkoshaCode, value);
        }

        // CSV出力用メソッド
        public string GetRow()
        {
            return $"{CdBlock},{Tdunitcode},{CdShukkaBatch},{CdCourse},{CdRoute},{CdTokuisaki},{NmTokuisaki},"
                + $"{DispLargeBoxPs},{DispSmallBoxPs},{DispBlueBoxPs},{DispEtcBoxPs},{FormatDtWorkdtStowage},{HenkoshaCode}";
        }

        public string FormatDtWorkdtStowage => DtWorkdtStowage.HasValue ? DtWorkdtStowage.Value.ToString("yyyy/MM/dd HH:mm:ss") : string.Empty;
    }
}
