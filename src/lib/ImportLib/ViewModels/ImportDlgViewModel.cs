using ImportLib.Engines;
using ImportLib.Models;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Diagnostics;
using System.Windows.Threading;
using WindowLib.Utils;

namespace ImportLib.ViewModels
{
    public class ImportDlgViewModel : BindableBase, IDialogAware
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

        public ImportDlgViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            CancelCommand = new DelegateCommand(Cancel);
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        private void Cancel()
        {
            Syslog.Info("Request Cancel");
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
            if (!parameters.TryGetValue("Engines", out IEnumerable<IImportEngine> engines))
            {
                Debug.Assert(false);
                return;
            }

            StartImport(engines);
        }

        private async void StartImport(IEnumerable<IImportEngine> engines)
        {
            Syslog.Debug("Start import");

            try
            {
                var importController = new DataImportController();
                importController.RequestComfirm += ImportController_RequestComfirm;
                importController.UpdateProgress += ImportController_UpdateProgress;

                foreach (var engine in engines)
                {
                    Message = $"{engine.DataName} 取り込み中";
                    await importController.Import(engine, _cancellationTokenSource.Token);
                }

                Close(ButtonResult.OK);
            }
            catch (OperationCanceledException)
            {
                WindowLib.Utils.MessageDialog.Show(_dialogService, "中断されました", "取込中断");
                Close(ButtonResult.Cancel);
            }
            catch (Exception ex)
            {
                Syslog.Error(ex.ToString());
                var message = $"取り込み中にエラーが発生しました。\n{ex.Message}";
                WindowLib.Utils.MessageDialog.Show(_dialogService, message, "エラー");

                var rc = (ex is ImportException ? ButtonResult.No : ButtonResult.Abort);
                Close(rc);
            }

            Syslog.Debug("End import");
        }

        private void ImportController_UpdateProgress(ProgressInfo info)
        {
            _dispatcher.Invoke(() => 
                {
                    Message2 = info.Message;
                    Maximum = info.Maximum;
                    Minimum = info.Minimum;
                    Value = info.Value;
                });
        }

        private ButtonResult ImportController_RequestComfirm(string message, string caption)
        {
            return _dispatcher.Invoke(() => 
                MessageDialog.Show(_dialogService, message, caption, ButtonMask.OK | ButtonMask.Cancel));
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
