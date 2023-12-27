using Mapping.Models;
using PrintLib;
using System.Windows.Controls;

namespace Mapping.Reports
{
    public class LocReportViewModel : IPrintViewModel
    {
        public string Title { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public int Page { get; set; }
        public int PageMax { get; set; }
        public string PageInfo => $"{Page}/{PageMax}";
        public string CdDistGroup { get; set; } = string.Empty;
        public string NmDistGroup { get; set; } = string.Empty;
        public string DistGroupInfo => $"{CdDistGroup} {NmDistGroup}";
        public string DtDelivery { get; set; } = string.Empty;
        public string CdBlock { get; set; } = string.Empty;

        public IList<LocInfo> Reports { get; set; } = new List<LocInfo>();

        public UserControl GetDocumentView()
        {
            return new LocReport();
        }
    }
}
