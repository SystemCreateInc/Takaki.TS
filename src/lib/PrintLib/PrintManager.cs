using System.Collections.Generic;
using System.Printing;
using System.Windows.Documents;
using System.Windows;
using PrintLib.Helper;

namespace PrintLib
{
    public class PrintManager
    {
        private PageOrientation _paperOrientation = PageOrientation.Landscape;
        private PageMediaSize _pageMediaSize;

        public PrintManager()
        {
            _pageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);
        }

        public PrintManager(PageMediaSizeName name, PageOrientation orientation)
        {
            _pageMediaSize = new PageMediaSize(name);
            _paperOrientation = orientation;
        }

        public PrintManager(PageMediaSize pageMediaSize, PageOrientation orientation)
        {
            _pageMediaSize = pageMediaSize;
            _paperOrientation = orientation;
        }

        public bool PrintShowDialog(string docTitle, IEnumerable<IPrintViewModel> viewModels)
        {
            using (var printQueue = GetPrintQueue())
            {
                var doc = CreaatePrintDocument(viewModels);
                return PrintDialogHelper.Show(docTitle, doc, printQueue, CreatePrintTicket(printQueue));
            }
        }

        public bool Print(string docTitle, IEnumerable<IPrintViewModel> viewModels)
        {
            using (var printQueue = GetPrintQueue())
            {
                var doc = CreaatePrintDocument(viewModels);
                var ticket = CreatePrintTicket(printQueue);
                var xpsDocWriter = PrintQueue.CreateXpsDocumentWriter(printQueue);
                xpsDocWriter.Write(doc, ticket);
            }

            return true;
        }

        public FixedDocument CreaatePrintDocument(IEnumerable<IPrintViewModel> viewModels)
        {
            var doc = new FixedDocument();

            foreach (var vm in viewModels)
            {
                var printView = vm.GetDocumentView();
                printView.DataContext = vm;
                var pageSize = new Size(printView.Width, printView.Height);
                printView.Measure(pageSize);
                printView.Arrange(new Rect(pageSize));
                printView.UpdateLayout();

                // 印刷ページを用紙サイズに調整
                var fixedPage = new FixedPage();
                fixedPage.Height = pageSize.Height;
                fixedPage.Width = pageSize.Width;
                fixedPage.Measure(pageSize);
                fixedPage.Arrange(new Rect(pageSize));
                fixedPage.UpdateLayout();
                fixedPage.Children.Add(printView);

                var pageContent = new PageContent() { Child = fixedPage };
                doc.Pages.Add(pageContent);
            }

            return doc;
        }

        public PrintTicket CreatePrintTicket(PrintQueue printQueue)
        {
            var modifiedTicket = new PrintTicket();
            modifiedTicket.PageOrientation = _paperOrientation;
            modifiedTicket.PageMediaSize = _pageMediaSize;
            return printQueue.MergeAndValidatePrintTicket(printQueue.UserPrintTicket, modifiedTicket).ValidatedPrintTicket;
        }

        public PrintQueue GetPrintQueue()
        {
            return ReservedPrinter.GetPrintQueue();
        }
    }
}
