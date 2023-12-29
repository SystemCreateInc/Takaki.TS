using DbLib.Defs;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapping.Models
{
    public class LocInfo : BindableBase
    {
        public LocInfo()
        {
        }
        public LocInfo(string cdblock,string tdunitaddrcode)
        {
            CdBlock = cdblock;
            Tdunitaddrcode = tdunitaddrcode;
        }
        public LocInfo(Dist dist)
        {
            CdBlock = dist.CdBlock;
            Tdunitaddrcode = dist.tdunitaddrcode;
            CdCourse = dist.CdSumCourse;
            CdRoute = dist.CdSumRoute.ToString();
            CdTokuisaki = dist.CdSumTokuisaki;
            NmTokuisaki = dist.NmSumTokuisaki;
            CdBinSum = dist.CdBinSum == (int)BinSumType.Yes ? "●" : "";
            CdSumTokuisaki = dist.CdTokuisaki != dist.CdSumTokuisaki ? "●" : "";
            Maguchi = dist.Maguchi.ToString();
        }

        private string _cdBlock = string.Empty;
        public string CdBlock
        {
            get => _cdBlock;
            set => SetProperty(ref _cdBlock, value);
        }
        private string _tdunitaddrcode = string.Empty;
        public string Tdunitaddrcode
        {
            get => _tdunitaddrcode;
            set => SetProperty(ref _tdunitaddrcode, value);
        }

        private string _cdCourse = string.Empty;
        public string CdCourse
        {
            get => _cdCourse;
            set => SetProperty(ref _cdCourse, value);
        }

        private string _cdRoute = string.Empty;
        public string CdRoute
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
        private string _cdBinSum = string.Empty;
        public string CdBinSum
        {
            get => _cdBinSum;
            set => SetProperty(ref _cdBinSum, value);
        }

        private string _cdSumTokuisaki = string.Empty;
        public string CdSumTokuisaki
        {
            get => _cdSumTokuisaki;
            set => SetProperty(ref _cdSumTokuisaki, value);
        }

        private string _magichi = string.Empty;
        public string Maguchi
        {
            get => _magichi;
            set => SetProperty(ref _magichi, value);
        }
    }
}
