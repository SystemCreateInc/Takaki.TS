using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportLib.Models
{
    public class StoreExportData
    {
        public List<long> ResultIds = new List<long>();

        public string OrderNo = string.Empty;
        public string Item = string.Empty;
        public int ResultQuantity;
        public DateTime StoreDate;
    }
}
