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
            Close(ButtonResult.Cancel);
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
                var importController = new DataImportController();

                foreach (var engine in engines)
                {
                    Message = $"{engine.DataName} 取り込み中";

                    var sameDistResult = await importController.CheckSameDist(engine, _cancellationTokenSource.Token);

                    if (sameDistResult == GetSameDistResult.WORK)
                    {
                        WindowLib.Utils.MessageDialog.Show(_dialogService,
                            "同じ納品日・出荷バッチコードのデータで作業済みがある為中断します。", $"{engine.DataName} エラー");

                        Close(ButtonResult.Cancel);
                        return;
                    }

                    if (sameDistResult == GetSameDistResult.EXIST)
                    {
                        if (WindowLib.Utils.MessageDialog.Show(_dialogService,
                            "同じ納品日・出荷バッチコードのデータが登録されています。\n入れ替えますか？", $"{engine.DataName} 入替確認",
                            WindowLib.Utils.ButtonMask.OK | WindowLib.Utils.ButtonMask.Cancel) != ButtonResult.OK)
                        {
                            Close(ButtonResult.Cancel);
                            return;
                        }
                    }

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
