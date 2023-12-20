using PrintLib;
using System.Windows.Controls;

namespace DistLargePrint.Reports
{
    public class DistLargeListViewModel : IPrintViewModel
    {
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public int Page { get; set; }
        public int PageMax { get; set; }
        public string PageInfo => $"{Page}/{PageMax}";
        public string CdLargeGroup { get; set; } = string.Empty;
        public string DtDelivery { get; set; } = string.Empty;

        public IList<Group> Groups { get; set; } = new List<Group>();

        public UserControl GetDocumentView()
        {
            return new DistLargeList();
        }
    }
}
