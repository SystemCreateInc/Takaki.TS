using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picking.Models
{
    public class ProgressCnt : BindableBase
    {
        private string _dt_delivery = string.Empty;
        public string DT_DELIVERY
        {
            get => _dt_delivery;
            set => SetProperty(ref _dt_delivery, value);
        }

        private string _cd_dist_group = string.Empty;
        public string CD_DIST_GROUP
        {
            get => _cd_dist_group;
            set => SetProperty(ref _cd_dist_group, value);
        }

        // 進捗数
        private int _cntmax = 0;
        public int CntMax
        {
            get => _cntmax;
            set => SetProperty(ref _cntmax, value);
        }

        private int _cntvalue = 0;
        public int CntValue
        {
            get => _cntvalue;
            set => SetProperty(ref _cntvalue, value);
        }
        private string _cnttext = "";
        public string CntText
        {
            get => _cnttext;
            set => SetProperty(ref _cnttext, value);
        }

        public void UpdateText()
        {
            if (CntMax == 0)
            {
                CntText = string.Format(@"{0,4} / {1,4}  {2,3}%  ", 0, 0, 0);
                ItemCntText = string.Format(@"{0,4} / {1,4}  {2,3}%  ", 0, 0, 0);
            }
            else
            {
                decimal rate = ((decimal)CntValue / (decimal)CntMax) * 100;
                CntText = string.Format(@"{0,4} / {1,4}  {2,3}%  ", CntValue, CntMax, (int)rate);
                rate = ((decimal)ItemCntValue / (decimal)ItemCntMax) * 100;
                ItemCntText = string.Format(@"{0,4} / {1,4}  {2,3}%  ", ItemCntValue, ItemCntMax, (int)rate);
            }
        }

        private int _itemcntmax = 0;
        public int ItemCntMax
        {
            get => _itemcntmax;
            set => SetProperty(ref _itemcntmax, value);
        }

        private int _itemcntvalue = 0;
        public int ItemCntValue
        {
            get => _itemcntvalue;
            set => SetProperty(ref _itemcntvalue, value);
        }
        private string _itemcnttext = "";
        public string ItemCntText
        {
            get => _itemcnttext;
            set => SetProperty(ref _itemcnttext, value);
        }
    }
}
