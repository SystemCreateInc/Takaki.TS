using DbLib;
using ExportLib;
using ExportLib.Infranstructures;
using LogLib;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportBase.ViewModels
{
    public class ExportSettingsDlgViewModel : BindableBase, IDialogAware
    {
        public string Title => "自動送信設定";

        public event Action<IDialogResult>? RequestClose;

        private IExportProcessor? _processor;
        private readonly IDialogService _dialogService;
        private readonly ScopeLogger _logger = new ScopeLogger<ExportSettingsDlgViewModel>();

        private DateTime? _time;
        public DateTime? Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }

        private bool _disableTiming;
        public bool DisableTiming
        {
            get => _disableTiming;
            set => SetProperty(ref _disableTiming, value);
        }


        private bool _enableTiming;
        public bool EnableTiming
        {
            get => _enableTiming;
            set => SetProperty(ref _enableTiming, value);
        }

        public DelegateCommand OkCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public ExportSettingsDlgViewModel(IDialogService dialogService)
        {
            OkCommand = new DelegateCommand(Ok);
            CancelCommand = new DelegateCommand(Cancel);
            _dialogService = dialogService;
        }

        private void Cancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        private void Ok()
        {
            try
            {
                _processor!.EnableTiming = EnableTiming;
                _processor!.SpecifiedTimings = Time is null ? Enumerable.Empty<TimeSpan>() : new[] { ((DateTime)Time).TimeOfDay };

                using (var repo = new ExportRepository())
                {
                    repo.Save(_processor.GetInterfaceFile());
                    repo.Commit();
                }

                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            }
            catch (Exception ex)
            {
                _logger.Error($"OK: {ex}");
                WindowLib.Utils.MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
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
            if (!parameters.TryGetValue("Processor", out _processor))
            {
                Debug.Assert(false);
                return;
            }

            DisableTiming = !_processor!.EnableTiming;
            EnableTiming = _processor!.EnableTiming;

            if (_processor.SpecifiedTimings.FirstOrDefault() is TimeSpan timeSpan)
            {
                Time = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    timeSpan.Hours,
                    timeSpan.Minutes,
                    timeSpan.Seconds);
            }
            else
            {
                Time = null;
            }
        }
    }
}
