using LargeDist.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;

namespace LargeDist.ViewModels
{
    public class ModifyBoxUnitDialogViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand OKCommand { get; }
        public DelegateCommand CancelCommand { get; }

        private string? _cdHimban;
        public string? CdHimban
        {
            get => _cdHimban;
            set => SetProperty(ref _cdHimban, value);
        }

        private string? _cdGtin13;
        public string? CdGtin13
        {
            get => _cdGtin13;
            set => SetProperty(ref _cdGtin13, value);
        }

        private string? _nmHinSeishikimei;
        public string? NmHinSeishikimei
        {
            get => _nmHinSeishikimei;
            set => SetProperty(ref _nmHinSeishikimei, value);
        }

        private int _stBoxType;
        public int StBoxType
        {
            get => _stBoxType;
            set => SetProperty(ref _stBoxType, value);
        }

        private int _nuBoxUnit;
        public int NuBoxUnit
        {
            get => _nuBoxUnit;
            set => SetProperty(ref _nuBoxUnit, value);
        }

        private string? _value;
        public string? Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        private IDialogService _dialogService;
        private DistItem? _item;

        public ModifyBoxUnitDialogViewModel(IDialogService dialogService)
        {
            OKCommand = new(OK);
            CancelCommand = new(Cancel);
            _dialogService = dialogService;
        }

        private void Cancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        private void OK()
        {
            int.TryParse(Value, out var value);

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters
            {
                {
                    "Unit", value
                }
            }));
        }

        public string Title => "箱入数変更";

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
            var param = parameters.GetValue<ModifyBoxUnitDialogParam>("Param");
            _item = param.Item;
            CdHimban = _item.CdHimban;
            CdGtin13 = _item.CdGtin13;
            NmHinSeishikimei = _item.NmHinSeishikimei;
            StBoxType = _item.StBoxType;
            NuBoxUnit = _item.NuBoxUnit;
            Value = _item.NuBoxUnit.ToString();
        }
    }
}
