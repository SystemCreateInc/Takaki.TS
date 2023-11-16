using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WindowLib.ViewModels
{
    public class ProgressRingViewModel : BindableBase, IDialogAware
    {
        private string _title = "";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public ProgressRingViewModel()
        {

        }

        public event Action<IDialogResult>? RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            Title = parameters.GetValue<string>("Title");
            var task = parameters.GetValue<Task>("Task");

            try
            {
                await task;
            }
            catch (Exception)
            {
                //  taskの例外は呼出もとで処理するのでここでは無視する
            }
            finally
            {
                //  例外発生時も確実にクローズする
                RequestClose?.Invoke(new DialogResult());

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
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            (sender as DispatcherTimer)?.Stop();
            RequestClose?.Invoke(new DialogResult());
        }
    }
}
