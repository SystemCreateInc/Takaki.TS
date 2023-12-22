using PrintLib;
using System.Windows.Controls;

namespace StowageListPrint.Reports
{
    public class StowageReportViewModel : IPrintViewModel
    {
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public int Page { get; set; }
        public int PageMax { get; set; }
        public string PageInfo => $"{Page}/{PageMax}";
        public string CdDistGroup { get; set; } = string.Empty;
        public string NmDistGroup { get; set; } = string.Empty;
        public string DistGroupInfo => $"{CdDistGroup} {NmDistGroup}";
        public string DtDelivery { get; set; } = string.Empty;
        public IList<Models.StowageListPrint> Reports { get; set; } = new List<Models.StowageListPrint>();

        public UserControl GetDocumentView()
        {
            return new StowageReport();
        }
    }
}
