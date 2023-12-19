using LargeDist.Infranstructures;
using LargeDist.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LargeDist.ViewModels
{
    public class CancelDistDialogViewModel : BindableBase, IDialogAware
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

        public string Title => "大仕分取消し";

        private LargeDistItem? _item;
        private Person? _person;
        private IDialogService _dialogService;

        public event Action<IDialogResult>? RequestClose;

        public CancelDistDialogViewModel(IDialogService dialogService)
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
            try
            {
                _item!.Undo();
                using (var repo = new LargeDistRepository())
                {
                    foreach (var item in _item.Items)
                    {
                        repo.Save(item, _person!);
                    }

                    repo.Commit();
                }

                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            }
            catch (Exception ex)
            {
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
            var param = parameters.GetValue<CancelDistDialogParam>("Param");
            _item = param.Item;
            _person = param.Person;
            CdHimban = _item.CdHimban;
            CdGtin13 = _item.CdGtin13;
            NmHinSeishikimei = _item.NmHinSeishikimei;
            StBoxType = _item.StBoxType;
            NuBoxUnit = _item.NuBoxUnit;
        }
    }
}
