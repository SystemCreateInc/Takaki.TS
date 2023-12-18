using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelDistGroupLib.Models
{
    public class DistGroup : BindableBase
    {
        private string _cd_dist_group = string.Empty;
        public string CdDistGroup
        {
            get => _cd_dist_group;
            set => SetProperty(ref _cd_dist_group, value);
        }

        private string _nm_dist_group = string.Empty;
        public string NmDistGroup
        {
            get => _nm_dist_group;
            set => SetProperty(ref _nm_dist_group, value);
        }

        private DateTime _dt_delivery = DateTime.Now;
        public DateTime DtDelivery
        {
            get => _dt_delivery;
            set => SetProperty(ref _dt_delivery, value);
        }

        private string _cdblock = string.Empty;
        public string CdBlock
        {
            get => _cdblock;
            set => SetProperty(ref _cdblock, value);
        }

        private string _cdkyoten = string.Empty;
        public string CdKyoten
        {
            get => _cdkyoten;
            set => SetProperty(ref _cdkyoten, value);
        }

        private int _idpc = 0;
        public int IdPc
        {
            get => _idpc;
            set => SetProperty(ref _idpc, value);
        }


        private int _tdunittype = 0;
        public int TdUnitType
        {
            get => _tdunittype;
            set => SetProperty(ref _tdunittype, value);
        }
    }
}
