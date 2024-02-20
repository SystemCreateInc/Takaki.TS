using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DistExpoter.ViewModels
{
    public class DeliveryDateDialogViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand OKCommand { get; }
        public DelegateCommand CancelCommand { get; }

        private DateTime _date = DateTime.Today;
        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }


        public DeliveryDateDialogViewModel()
        {
            OKCommand = new(OK);
            CancelCommand = new(Cancel);
        }

        private void Cancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        private void OK()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters
            {
                { "Date", Date }
            }));
        }

        public string Title => "納品日指定";

        public event Action<IDialogResult>? RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
