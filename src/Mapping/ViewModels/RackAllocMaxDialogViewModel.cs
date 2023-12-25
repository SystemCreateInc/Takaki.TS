using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using WindowLib.Utils;

namespace Mapping.ViewModels
{
    public class RackAllocMaxDialogViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand OKCommand { get; }
        public DelegateCommand CancelCommand { get; }

        private int _rackallocmax = 0;

        public int RackAllocMax
        {
            get => _rackallocmax;
            set => SetProperty(ref _rackallocmax, value);
        }

        private readonly IDialogService _dialogService;

        public RackAllocMaxDialogViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            OKCommand = new(OK);
            CancelCommand = new(Cancel);
        }

        private void Cancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        private void OK()
        {
            if (RackAllocMax==0)
            {
                MessageDialog.Show(_dialogService, "棚割ＭＡＸ数は必ず入力して下さい。", "エラー");
                return;
            }

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters
            {
                { "RackAllocMax", RackAllocMax }
            }));
        }

        public string Title => "棚割ＭＡＸ数入力";

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
            RackAllocMax = parameters.GetValue<int>("RackAllocMax");
        }
    }
}
