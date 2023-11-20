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
        public string CD_DIST_GROUP
        {
            get => _cd_dist_group;
            set => SetProperty(ref _cd_dist_group, value);
        }

        private string _nm_dist_group = string.Empty;
        public string NM_DIST_GROUP
        {
            get => _nm_dist_group;
            set => SetProperty(ref _nm_dist_group, value);
        }

        private DateTime _dt_delivery = DateTime.Now;
        public DateTime DT_DELIVERY
        {
            get => _dt_delivery;
            set => SetProperty(ref _dt_delivery, value);
        }
    }
}
