using PrintLib;
using System.Windows.Controls;

namespace WorkReport.Reports
{
    public class ReportViewModel : IPrintViewModel
    {
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public int Page { get; set; }
        public int PageMax { get; set; }
        public string PageInfo => $"{Page}/{PageMax}";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IList<Group> Groups { get; set; } = new List<Group>();

        public UserControl GetDocumentView()
        {
            return new Report();
        }
    }
}
