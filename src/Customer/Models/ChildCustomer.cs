using Customer.Loader;
using Prism.Mvvm;
using TakakiLib.Models;

namespace Customer.Models
{
    public class ChildCustomer : BindableBase
    {
        private string _cdTokuisakiChild = string.Empty;
        public string CdTokuisakiChild
        {
            get => _cdTokuisakiChild;
            set
            {
                SetProperty(ref _cdTokuisakiChild, value);
                NmTokuisaki = NameLoader.GetTokuisaki(CdTokuisakiChild);
            }
        }

        private string _nmTokuisaki = string.Empty;
        public string NmTokuisaki
        {
            get => _nmTokuisaki;
            set => SetProperty(ref _nmTokuisaki, value);
        }
    }
}
