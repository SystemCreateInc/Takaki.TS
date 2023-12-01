using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetHenkosha.Models
{
    public class Shain : BindableBase
    {
        private string _cd_shain = string.Empty;
        public string CdShain
        {
            get => _cd_shain;
            set => SetProperty(ref _cd_shain, value);
        }
        private string _nm_shain = string.Empty;
        public string NmShain
        {
            get => _nm_shain;
            set => SetProperty(ref _nm_shain, value);
        }
    }
}
