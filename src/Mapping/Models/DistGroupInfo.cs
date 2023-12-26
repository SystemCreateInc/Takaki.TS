using DbLib.Extensions;
using Mapping.Defs;
using Prism.Mvvm;

namespace Mapping.Models
{
    public class DistGroupInfo : BindableBase
    {
        private bool _select = false;
        public bool Select
        {
            get => _select;
            set => SetProperty(ref _select, value);
        }

        private bool _isSelectEnabled = true;
        public bool IsSelectEnabled
        {
            get => _isSelectEnabled;
            set => SetProperty(ref _isSelectEnabled, value);
        }

        private Defs.MStatus _mstatus = Defs.MStatus.Ready;
        public Defs.MStatus MStatus
        {
            get => _mstatus;
            set
            {
                SetProperty(ref _mstatus, value);
                MStatus_name = EnumExtensions.GetDescription(value);
            }
        }

        private string _mstatus_name = string.Empty;
        public string MStatus_name
        {
            get => _mstatus_name;
            set => SetProperty(ref _mstatus_name, value);
        }

        private string _cdDelivdt = string.Empty;
        public string DtDelivdt
        {
            get => _cdDelivdt;
            set => SetProperty(ref _cdDelivdt, value);
        }

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

        private int _shopcnt = 0;
        public int ShopCnt
        {
            get => _shopcnt;
            set => SetProperty(ref _shopcnt, value);
        }

        private int _overshopcnt = 0;
        public int OverShopCnt
        {
            get => _overshopcnt;
            set => SetProperty(ref _overshopcnt, value);
        }

        private int _loccnt = 0;
        public int LocCnt
        {
            get => _loccnt;
            set => SetProperty(ref _loccnt, value);
        }

        private int _lstatus = 0;
        public int LStatus
        {
            get => _lstatus;
            set
            {
                SetProperty(ref _lstatus, value);
                switch(value)
                {
                    case (int)DbLib.Defs.Status.Ready:
                        NmLStatus = "";
                        break;
                    case (int)DbLib.Defs.Status.Inprog:
                        NmLStatus = "仕分中";
                        break;
                    case (int)DbLib.Defs.Status.Completed:
                        NmLStatus = "完了";
                        break;
                }
            }
        }

        private string _nmlstatus = "";
        public string NmLStatus
        {
            get => _nmlstatus;
            set => SetProperty(ref _nmlstatus, value);
        }

        private int _dstatus = 0;
        public int DStatus
        {
            get => _dstatus;
            set
            {
                SetProperty(ref _dstatus, value);
                switch (value)
                {
                    case (int)DbLib.Defs.Status.Ready:
                        NmDStatus = "";
                        break;
                    case (int)DbLib.Defs.Status.Inprog:
                        NmDStatus = "仕分中";
                        break;
                    case (int)DbLib.Defs.Status.Completed:
                        NmDStatus = "完了";
                        break;
                }
            }
        }

        private string _nmdstatus = "";
        public string NmDStatus
        {
            get => _nmdstatus;
            set => SetProperty(ref _nmdstatus, value);
        }

        private int _mappingstatus = 0;
        public int MappingStatus
        {
            get => _mappingstatus;
            set => SetProperty(ref _mappingstatus, value);
        }
    }
}
