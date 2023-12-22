using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportLib.Models
{
    public class IssueExportData
    {
        public List<long> ResultIds = new List<long>();

        public string ControlNo = string.Empty;
        public string ArrangeNo = string.Empty;
        public string Item = string.Empty;
        public int ResultQuantity;
        public DateTime IssueDate;
        public string LocationId = string.Empty;
    }
}
