using ExportLib;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Diagnostics;
using System.Windows.Threading;

namespace ExportBase.ViewModels
{
    public class ExportDlgViewModel : BindableBase, IDialogAware
    {
        public string Title => "データ受信";

        public event Action<IDialogResult>? RequestClose;

        public DelegateCommand CancelCommand { get; set; }

        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private string _message2 = string.Empty;
        public string Message2
        {
            get => _message2;
            set => SetProperty(ref _message2, value);
        }

        private int _minmum;
        public int Minimum
        {
            get => _minmum;
            set => SetProperty(ref _minmum, value);
        }

        private int _maximum;
        public int Maximum
        {
            get => _maximum;
            set => SetProperty(ref _maximum, value);
        }

        private int _value;
        public int Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        private IDialogService _dialogService;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Dispatcher _dispatcher;

        public ExportDlgViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            CancelCommand = new DelegateCommand(Cancel);
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        private void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (!parameters.TryGetValue("Service", out ExportService service))
            {
                Debug.Assert(false);
                return;
            }

            StartExport(service);
        }

        private async void StartExport(ExportService service)
        {
            Message = "データ送信中";

            service.StatusChangedEvent += Service_StatusChangedEvent;
            service.ProgressEvent += Service_ProgressEvent;

            try
            {
                await Task.Run(() =>
                {
                    service.ForceExport(_cancellationTokenSource.Token, service.Processors);
                });

                Close(ButtonResult.OK);
            }
            catch (Exception ex)
            {
                Syslog.Error(ex.ToString());
                var message = $"送信中にエラーが発生しました。\n{ex.Message}";
                WindowLib.Utils.MessageDialog.Show(_dialogService, message, "エラー");
                //var rc = (ex is ExportException ? ButtonResult.No : ButtonResult.Abort);
                Close(ButtonResult.Abort);
            }
        }

        private void Service_StatusChangedEvent(ExportStatus status)
        {
            _dispatcher.Invoke(() => Message = $"{status.DataName} {status.Description}");
        }

        private void Service_ProgressEvent(ProgressInfo pi)
        {
            _dispatcher.Invoke(() =>
            {
                Message2 = pi.Message;
                Maximum = pi.Maximum;
                Minimum = pi.Minimum;
                Value = pi.Value;
            });
        }

        private async void Close(ButtonResult result)
        {
            while (RequestClose is null)
            {
                await Task.Delay(100);
            }

            RequestClose?.Invoke(new DialogResult(result));
        }
    }
}
