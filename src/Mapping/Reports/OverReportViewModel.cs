using Mapping.Models;
using PrintLib;
using System.Windows.Controls;

namespace Mapping.Reports
{
    public class OverReportViewModel : IPrintViewModel
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
        public string CdCourse { get; set; } = string.Empty;
        public int CdRoute { get; set; } = 0;
        public string CdTokuisaki { get; set; } = string.Empty;
        public string NmTokuisaki { get; set; } = string.Empty;
        public int LargeBox { get; set; } = 0;
        public int SmallBox { get; set; } = 0;
        public int BlueBox { get; set; } = 0;


        public IList<OverInfo> Reports { get; set; } = new List<OverInfo>();

        public UserControl GetDocumentView()
        {
            return new OverReport();
        }
    }
}
