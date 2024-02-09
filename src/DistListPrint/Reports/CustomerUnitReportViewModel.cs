using PrintLib;
using System.Windows.Controls;

namespace DistListPrint.Reports
{
    public class CustomerUnitReportViewModel : IPrintViewModel
    {
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
        public string CdBlock { get; set; } = string.Empty;
        public string CdCourse { get; set; } = string.Empty;
        public IList<CustomerUnit> Reports { get; set; } = new List<CustomerUnit>();

        public UserControl GetDocumentView()
        {
            return new CustomerUnitReport();
        }
    }
}
