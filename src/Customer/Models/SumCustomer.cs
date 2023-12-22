using Prism.Mvvm;

namespace Customer.Models
{
    public class SumCustomer : BindableBase
    {
        private string _cdKyoten = string.Empty;
        public string CdKyoten
        {
            get => _cdKyoten;
            set => SetProperty(ref _cdKyoten, value);
        }

        private string _cdSumTokuisaki = string.Empty;
        public string CdSumTokuisaki
        {
            get => _cdSumTokuisaki;
            set => SetProperty(ref _cdSumTokuisaki, value);
        }

        private string _nmSumTokuisaki = string.Empty;
        public string NmSumTokuisaki
        {
            get => _nmSumTokuisaki;
            set => SetProperty(ref _nmSumTokuisaki, value);
        }

        public long SumTokuisakiId { get; set; }
        public string Tekiyokaishi { get; set; } = string.Empty;
        public string TekiyoMuko { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public IEnumerable<ChildCustomer> ChildCustomers { get; set; } = Enumerable.Empty<ChildCustomer>();

    }
}
