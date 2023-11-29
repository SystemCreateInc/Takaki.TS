using DryIoc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispShop.Models
{
    public class Dist : BindableBase
    {
        private string _tdunitaddrcode = string.Empty;
        public string TdUnitAddrCode
        {
            get => _tdunitaddrcode;
            set => SetProperty(ref _tdunitaddrcode, value);
        }

        private string _dt_delivery = string.Empty;
        public string DtDelivery
        {
            get => _dt_delivery;
            set => SetProperty(ref _dt_delivery, value);
        }

        private string _cd_kyoten = string.Empty;
        public string CdKyoten
        {
            get => _cd_kyoten;
            set => SetProperty(ref _cd_kyoten, value);
        }

        private string _cd_dist_group = string.Empty;
        public string CdDistGroup
        {
            get => _cd_dist_group;
            set => SetProperty(ref _cd_dist_group, value);
        }

        private string _cd_course = string.Empty;
        public string CdCource
        {
            get => _cd_course;
            set => SetProperty(ref _cd_course, value);
        }
        private int _cd_route = 0;
        public int CdRoute
        {
            get => _cd_route;
            set => SetProperty(ref _cd_route, value);
        }

        private string _cd_tokuisaki = string.Empty;
        public string CdTokuisaki
        {
            get => _cd_tokuisaki;
            set => SetProperty(ref _cd_tokuisaki, value);
        }

        private string _nm_tokuisaki = string.Empty;
        public string NmTokuisaki
        {
            get => _nm_tokuisaki;
            set => SetProperty(ref _nm_tokuisaki, value);
        }


        private int _ops = 0;
        public int Ops
        {
            get => _ops;
            set => SetProperty(ref _ops, value);
        }
        private int _rps = 0;
        public int Rps
        {
            get => _rps;
            set => SetProperty(ref _rps, value);
        }
        public int Zps
        {
            get { return _ops - _rps; }
        }

        private int _box0 = 0;
        public int Box0
        {
            get => _box0;
            set => SetProperty(ref _box0, value);
        }
        private int _box1 = 0;
        public int Box1
        {
            get => _box1;
            set => SetProperty(ref _box1, value);
        }
        private int _box2 = 0;
        public int Box2
        {
            get => _box2;
            set => SetProperty(ref _box2, value);
        }
        private int _box3 = 0;
        public int Box3
        {
            get => _box3;
            set => SetProperty(ref _box3, value);
        }
        public int BoxCnt
        {
            get { return Box0 + Box1 + Box2 + Box3; }
        }
    }
}
