using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using LogLib;
using Prism.Commands;
using WindowLib.Utils;
using System.Threading;
using Prism.Events;
using System.Windows.Threading;
using System.Windows;

namespace WindowLib.ViewModels
{
    public class ProgressMessage
    {
        public ProgressMessage()
        {
            Title = "";
            Message = "";
            ProgressMax = -1;
            ProgressValue = -1;
        }

        // タイトル
        public string Title;
        // メッセージ
        public string Message;
        // 最大数
        public int ProgressMax;
        // 現在
        public int ProgressValue;
    }

    public class ProgressDialogEvent : PubSubEvent<ProgressMessage> { }
    public class ProgressDialogCancelEvent : PubSubEvent { }

    public class ProgressDialogViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand Cancel { get; set; }

        private string _title = "処理中";
        public string Title { get => _title; set => SetProperty(ref _title, value); }
        private string _message = "準備中";
        public string Message { get => _message; set => SetProperty(ref _message, value); }

        private int _progressMax = 0;
        public int ProgressMax
        {
            get => _progressMax;
            set => SetProperty(ref _progressMax, value);
        }

        private int _progressValue = 0;
        public int ProgressValue
        {
            get => _progressValue;
            set => SetProperty(ref _progressValue, value);
        }

        private IDialogService _dialogService;

        private bool _bConfStopMessage = true;

        public ProgressDialogViewModel(IDialogService dialogService, IEventAggregator eventAggregator)
        {
            _dialogService = dialogService;

            Cancel = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });

            eventAggregator
                .GetEvent<ProgressDialogEvent>()
                .Subscribe(UpateMessage);
        }

        public event Action<IDialogResult>? RequestClose = null;

        public bool CanCloseDialog()
        {
            if (_bConfStopMessage)
            {
                var buttons = new ButtonResult[] { ButtonResult.OK, ButtonResult.Cancel };
                if (MessageDialog.Show(_dialogService, "中断します。よろしいですか？", "中断確認", buttons, MessageBoxImage.Warning) != ButtonResult.OK)
                    return false;
            }

            return true;
        }

        public void OnDialogClosed()
        {
            canceller?.Cancel();
            return;
        }

        private CancellationTokenSource? canceller = null;
        private Task? task = null;

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            canceller = parameters.GetValue<CancellationTokenSource>("canceller");
            task = parameters.GetValue<Task>("task");
            Title = parameters.GetValue<string>("title");
            Message = parameters.GetValue<string>("message");

            try
            {
                //  取込終了時に閉じる
                await task;
                _bConfStopMessage = false;

            }
            catch
            {
                _bConfStopMessage = false;
            }
            finally
            {
                _bConfStopMessage = false;
                //  例外発生時も確実にクローズする
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));

                if (RequestClose == null)
                {
                    var timer = new DispatcherTimer
                    {
                        IsEnabled = true,
                        Interval = TimeSpan.FromSeconds(1),
                    };
                    timer.Tick += Timer_Tick;
                    timer.Start();
                }
            }

            await task.ContinueWith((compt) =>
            {
                if (compt.Exception is AggregateException age)
                {
                    if (age.InnerException != null)
                    {
                        LogLib.Syslog.Error($"ProgressDialog: EXCEPTION: {age.InnerException.Message}");
                        MessageDialog.Show(_dialogService, age.InnerException.Message, "エラー", ButtonMask.OK, MessageBoxImage.Error);
                    }
                    //                    throw new Exception(age.InnerException.Message);
                }
            });
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (sender == null)
                return;

            ((DispatcherTimer)sender).Stop();
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }


        private void UpateMessage(ProgressMessage pm)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (pm.ProgressMax != -1)
                    ProgressMax = pm.ProgressMax;
                if (pm.ProgressValue != -1)
                    ProgressValue = pm.ProgressValue;
                if (pm.Title != "")
                    Title = pm.Title;
                if (pm.Message != "")
                    Message = pm.Message;
            });
        }
    }
}
