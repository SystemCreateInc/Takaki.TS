using PrintLib;
using System.Windows.Controls;

namespace TakahataDistListPrint.Reports
{
    public class ItemReportViewModel : IPrintViewModel
    {
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public int Page { get; set; }
        public int PageMax { get; set; }
        public string PageInfo => $"{Page}/{PageMax}";
        public string DtDelivery { get; set; } = string.Empty;
        public string CdHimban { get; set; } = string.Empty;
        public string CdGtin13 { get; set; } = string.Empty;
        public string NmHinSeishikimei { get; set; } = string.Empty;
        public int NuBoxunit { get; set; }
        public IList<Models.TakahataDistListPrint> Reports { get; set; } = new List<Models.TakahataDistListPrint>();

        public UserControl GetDocumentView()
        {
            return new ItemReport();
        }
    }
}
