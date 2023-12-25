using Azure;
using LogLib;
using Microsoft.Extensions.Configuration;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Mapping.ViewModels
{
    public class SelDeleveryDateDlgModel : BindableBase, IDialogAware
    {
        public DelegateCommand Enter { get; }
        public DelegateCommand OK { get; }
        public DelegateCommand Cancel { get; }

        public event Action<IDialogResult>? RequestClose;

        private string _title = "納品日選択";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private DateTime _dt_delivery = DateTime.Now.AddDays(1);
        public DateTime DtDelivery
        {
            get => _dt_delivery;
            set => SetProperty(ref _dt_delivery, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }


        public SelDeleveryDateDlgModel(IDialogService dialogService)
        {
            OK = new DelegateCommand(() =>
            {
                ErrorMessage = string.Empty;

                if (!Check())
                {
                    return;
                }

                // ダイアログを閉じる
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters
                {
                    { "DtDelivery", DtDelivery },
                }));
            });

            Enter = new DelegateCommand(() =>
            {
                OK.Execute();
            });

            Cancel = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            return;
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
        private bool Check()
        {
            return true;
        }
    }
}
