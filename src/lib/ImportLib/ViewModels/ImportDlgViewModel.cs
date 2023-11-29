using ImportLib.Engines;
using ImportLib.Models;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Diagnostics;

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

        private IDialogService _dialogService;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public ImportDlgViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            CancelCommand = new DelegateCommand(Cancel);
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
            if (!parameters.TryGetValue("Engines", out IEnumerable<IImportEngine> engines))
            {
                Debug.Assert(false);
                return;
            }

            StartImport(engines);
        }

        private async void StartImport(IEnumerable<IImportEngine> engines)
        {
            try
            {
                foreach(var engine in engines)
                {
                    Message = $"{engine.DataName} 取り込み中";
                    await new DataImportController().Import(engine, _cancellationTokenSource.Token);
                }

                Close(ButtonResult.OK);
            }
            catch (Exception ex)
            {
                Syslog.Error(ex.ToString());
                var message = $"取り込み中にエラーが発生しました。\n{ex.Message}";
                WindowLib.Utils.MessageDialog.Show(_dialogService, message, "エラー");

                var rc = (ex is ImportException ? ButtonResult.No : ButtonResult.Abort);
                Close(rc);
            }
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
