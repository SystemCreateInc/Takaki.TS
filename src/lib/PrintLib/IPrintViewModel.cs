using System.Windows.Controls;

namespace PrintLib
{
    public interface IPrintViewModel
    {
        UserControl GetDocumentView();

        int Page { get; set; }
        int PageMax { get; set; }
        string PageInfo { get; }
    }
}
