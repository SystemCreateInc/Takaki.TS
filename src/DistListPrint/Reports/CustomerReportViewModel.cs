using PrintLib;
using System.Windows.Controls;

namespace DistListPrint.Reports
{
    public class CustomerReportViewModel : IPrintViewModel
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
        public string CdShukkaBatch { get; set; } = string.Empty;
        public string NmShukkaBatch { get; set; } = string.Empty;
        public string ShukkaBatchInfo => $"{CdShukkaBatch} {NmShukkaBatch}";
        public string CdCourse { get; set; } = string.Empty;
        public int CdRoute { get; set; }
        public string CdTokuisaki { get; set; } = string.Empty;
        public string NmTokuisaki { get; set; } = string.Empty;
        public string TokuisakiInfo => $"{CdTokuisaki} {NmTokuisaki}";
        public IList<Models.DistListPrint> Reports { get; set; } = new List<Models.DistListPrint>();

        public UserControl GetDocumentView()
        {
            return new CustomerReport();
        }

    }
}
