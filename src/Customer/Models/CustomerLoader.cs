namespace Customer.Models
{
    public class CustomerLoader
    {
        public static IEnumerable<Customer> Get()
        {
            // fixme:読み込み機能追加
            return new Customer[]
            {
                new Customer { CdKyoten = "4201", CdSumTokuisaki = "200061", NmSumTokuisaki = "得意先名1" },
                new Customer { CdKyoten = "4201", CdSumTokuisaki = "200061", NmSumTokuisaki = "得意先名2" },
                new Customer { CdKyoten = "4201", CdSumTokuisaki = "200061", NmSumTokuisaki = "得意先名3" },
                new Customer { CdKyoten = "4201", CdSumTokuisaki = "200092", NmSumTokuisaki = "得意先名4" },
            };
        }
    }
}
