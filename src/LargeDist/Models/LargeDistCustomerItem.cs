using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargeDist.Models
{
    public class LargeDistCustomerItem
    {
        public string? Address => _key.Address;
        public string CdCourse => _key.CdCourse;
        public int CdRoute => _key.CdRoute;
        public string CdTokuisaki => _key.CdTokuisaki;
        public string? NmTokuisaki { get; }
        public BoxedQuantity Input { get; }

        private LargeDistCustomerKey _key;

        public LargeDistCustomerItem(LargeDistCustomerKey key, IEnumerable<DistItem> items)
        {
            _key = key;
            var item = items.First();
            NmTokuisaki = item.NmTokuisaki;
            Input = new BoxedQuantity(item.NuBoxUnit, items.Sum(x => x.InputPiece));
        }
    }
}
