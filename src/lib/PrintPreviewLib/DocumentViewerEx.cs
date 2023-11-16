using Microsoft.Identity.Client;
using PrintLib.Helper;
using Prism.Mvvm;
using System.ComponentModel;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PrintPreviewLib
{
    public class DocumentViewerEx : DocumentViewer, INotifyPropertyChanged
    {
        public PrintTicket? PrintTicket { get; set; }
        public PrintQueue? PrintQueue { get; set; }
        public string? Title { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected override void OnPrintCommand()
        {
            // 印刷実行を通知
            NotifyPropertyChanged();
            PrintDialogHelper.Show(Title!, (FixedDocument)Document, PrintQueue!, PrintTicket!);
        }
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
