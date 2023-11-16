using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using WindowLib.Utils;

namespace WindowLib.ViewModels
{
    public class SelectFromToDateDlgViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand OK { get; }
        public DelegateCommand Cancel { get; }

        public string Title => "日付選択";

        public event Action<IDialogResult>? RequestClose;

        private string _selectMessage = "日付を選択して下さい";
        public string SelectMessage
        {
            get => _selectMessage;
            set => SetProperty(ref _selectMessage, value);
        }

        private DateTime _fromDate = DateTime.Now;
        public DateTime FromDate
        {
            get => _fromDate;
            set => SetProperty(ref _fromDate, value);
        }

        private DateTime _toDate = DateTime.Now;
        public DateTime ToDate
        {
            get => _toDate;
            set => SetProperty(ref _toDate, value);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        private readonly IDialogService _dialogService;

        public SelectFromToDateDlgViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            OK = new DelegateCommand(() =>
            {
                if (FromDate > ToDate)
                {
                    MessageDialog.Show(_dialogService, "開始日が終了日より後日です", "エラー");
                    return;
                }

                var param = new DialogParameters
                {
                    { "FromDate", FromDate },
                    { "ToDate", ToDate },
                };
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, param));
            });

            Cancel = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var message = parameters.GetValue<string>("Message");
            var fromDate = parameters.GetValue<DateTime>("FromDate");
            var toDate = parameters.GetValue<DateTime>("ToDate");

            if (message != null)
            {
                SelectMessage = message;
            }

            if (fromDate.Year != 1)
            {
                FromDate = fromDate;
            }

            if (toDate.Year != 1)
            {
                ToDate = toDate;
            }
        }
    }
}
