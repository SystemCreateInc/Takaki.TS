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

        private string _cd_sum_course = string.Empty;
        public string CdSumCource
        {
            get => _cd_sum_course;
            set => SetProperty(ref _cd_sum_course, value);
        }
        private string _cd_sum_route =  string.Empty;
        public string CdSumRoute
        {
            get => _cd_sum_route;
            set => SetProperty(ref _cd_sum_route, value);
        }

        private string _cd_sum_tokuisaki = string.Empty;
        public string CdSumTokuisaki
        {
            get => _cd_sum_tokuisaki;
            set => SetProperty(ref _cd_sum_tokuisaki, value);
        }

        private string _nm_sum_tokuisaki = string.Empty;
        public string NmSumTokuisaki
        {
            get => _nm_sum_tokuisaki;
            set => SetProperty(ref _nm_sum_tokuisaki, value);
        }


        private int _ops = 0;
        public int Ops
        {
            get => _ops;
            set
            {
                SetProperty(ref _ops, value);
                if (CdKyoten != "")
                    DispOps = string.Format("{0}", value);
                else
                    DispOps = "";
            }
        }

        private string _dispops = string.Empty;
        public string DispOps
        {
            get => _dispops;
            set => SetProperty(ref _dispops, value);
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
        private int _maguchi = 0;
        public int Maguchi
        {
            get => _maguchi;
            set => SetProperty(ref _maguchi, value);
        }
        private string _cd_sum_course_maguchi = string.Empty;
        public string CdSumCourceMaguchi
        {
            get => _cd_sum_course_maguchi;
            set => SetProperty(ref _cd_sum_course_maguchi, value);
        }
        private string _cd_sum_route_maguchi = string.Empty;
        public string CdSumRouteMaguchi
        {
            get => _cd_sum_route_maguchi;
            set => SetProperty(ref _cd_sum_route_maguchi, value);
        }

        private bool _blink_course = false;
        public bool Blink_Course
        {
            get => _blink_course;
            set => SetProperty(ref _blink_course, value);
        }

        public string BoxCnt
        {
            get { 
                return CdKyoten=="" ? "" : string.Format("{0:#}",Box0 + Box1 + Box2 + Box3);
            }
        }
    }
}
