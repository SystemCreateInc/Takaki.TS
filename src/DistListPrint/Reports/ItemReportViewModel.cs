using PrintLib;
using System.Windows.Controls;

namespace DistListPrint.Reports
{
    public class ItemReportViewModel : IPrintViewModel
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
        public string CdHimban { get; set; } = string.Empty;
        public string CdGtin13 { get; set; } = string.Empty;
        public string NmHinSeishikimei { get; set; } = string.Empty;
        public int NuBoxunit { get; set; }
        public IList<Models.DistListPrint> Reports { get; set; } = new List<Models.DistListPrint>();

        public UserControl GetDocumentView()
        {
            return new ItemReport();
        }
    }
}
