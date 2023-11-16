using PrintLib;
using System.Collections.Generic;
using System.Printing;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PrintPreviewLib
{
    public class PrintPreviewManager
    {
        public PrintManager PrintManager { get; set; }

        public PrintPreviewManager(PageMediaSizeName name, PageOrientation orientation)
        {
            PrintManager = new PrintManager(name, orientation);
        }

        public bool PrintPreview(string docTitle, IEnumerable<IPrintViewModel> viewModels)
        {
            using (var printQueue = PrintManager.GetPrintQueue())
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var window = new PrintPreview();
                var viewer = window.Viewer;
                viewer.Document = PrintManager.CreaatePrintDocument(viewModels);
                viewer.PrintQueue = printQueue;
                viewer.PrintTicket = PrintManager.CreatePrintTicket(printQueue);
                viewer.Title = docTitle;
                Mouse.OverrideCursor = null;
                window.ShowDialog();
            }

            return true;
        }
    }
}
