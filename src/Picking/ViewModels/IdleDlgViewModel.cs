using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Picking.ViewModels
{

    public class IdleDlgViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand OK { get; }

        public event Action<IDialogResult>? RequestClose;

        private string _title = "休憩";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        readonly Stopwatch _stopwatch = new Stopwatch();
        readonly DispatcherTimer _timer = new DispatcherTimer();

        private string _timertext = "00:00:00";
        public string TimerText
        {
            get => _timertext;
            set => SetProperty(ref _timertext, value);
        }

        public IdleDlgViewModel()
        {
            OK = new DelegateCommand(() =>
            {
                Syslog.Debug("IdleDlgViewModel:OK");
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            });
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            _timer.Tick -= new EventHandler(TimerMethod);
            _stopwatch.Stop();
            _timer.Stop();

            Syslog.Debug("IdleDlgViewModel:OnDialogClosed" + TimerText);

            return;
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _timer.Tick += new EventHandler(TimerMethod);
            _stopwatch.Start();
            _timer.Start();
        }
        private void TimerMethod(object? sender, EventArgs e)
        {
            var result = _stopwatch.Elapsed;
            TimerText = string.Format("{0}:{1}:{2}"
                , result.Hours.ToString("00")
                , result.Minutes.ToString("00")
                , result.Seconds.ToString("00")
            );
        }
    }
}
