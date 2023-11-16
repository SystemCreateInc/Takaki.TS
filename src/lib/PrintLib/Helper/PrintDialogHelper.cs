using System.Printing;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PrintLib.Helper
{
    public static class PrintDialogHelper
    {
        public static bool Show(string title, FixedDocument doc, PrintQueue printQuue, PrintTicket ticket)
        {
            var dialog = new PrintDialog()
            {
                UserPageRangeEnabled = true,
                PrintQueue = printQuue,
                PrintTicket = ticket,
            };

            if (dialog.ShowDialog() != true)
            {
                return false;
            }

            DocumentPaginator paginator = dialog.PageRangeSelection == PageRangeSelection.UserPages
                ? new PageRangeDocumentPaginator(doc.DocumentPaginator, dialog.PageRange)
                : doc.DocumentPaginator;

            dialog.PrintDocument(paginator, title);
            return true;
        }
    }
}
