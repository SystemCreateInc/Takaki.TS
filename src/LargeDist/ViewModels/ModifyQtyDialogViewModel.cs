using LargeDist.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LargeDist.ViewModels
{
    public class ModifyQtyDialogViewModel : BindableBase, IDialogAware
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

        private string? _cdBlock = string.Empty;
        public string? CdBlock
        {
            get => _cdBlock;
            set => SetProperty(ref _cdBlock, value);
        }

        private string? _cdJuchuBin = string.Empty;
        public string? CdJuchuBin
        {
            get => _cdJuchuBin;
            set => SetProperty(ref _cdJuchuBin, value);
        }

        private string? _cdDistGroup = string.Empty;
        public string? CdDistGroup
        {
            get => _cdDistGroup;
            set => SetProperty(ref _cdDistGroup, value);
        }

        private string? _nmDistGroup = string.Empty;
        public string? NmDistGroup
        {
            get => _nmDistGroup;
            set => SetProperty(ref _nmDistGroup, value);
        }

        private string? _cdShukkaBatch = string.Empty;
        public string? CdShukkaBatch
        {
            get => _cdShukkaBatch;
            set => SetProperty(ref _cdShukkaBatch, value);
        }

        private string? _nmShukkaBatch = string.Empty;
        public string? NmShukkaBatch
        {
            get => _nmShukkaBatch;
            set => SetProperty(ref _nmShukkaBatch, value);
        }

        private BoxedQuantity _order = new();
        public BoxedQuantity Order
        {
            get => _order;
            set => SetProperty(ref _order, value);
        }

        private BoxedQuantity _result = new();
        public BoxedQuantity Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        private BoxedQuantity _remain = new();
        public BoxedQuantity Remain
        {
            get => _remain;
            set => SetProperty(ref _remain, value);
        }

        private string? _box;
        public string? Box
        {
            get => _box;
            set
            {
                SetProperty(ref _box, value);
                UpdateTotal();
            }
        }

        private string? _piece;
        public string? Piece
        {
            get => _piece;
            set
            {
                SetProperty(ref _piece, value);
                UpdateTotal();
            }
        }

        private int? _total;
        public int? Total
        {
            get => _total;
            set => SetProperty(ref _total, value);
        }

        private IDialogService _dialogService;
        private LargeDistItem? _item;

        public ModifyQtyDialogViewModel(IDialogService dialogService)
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
            if (Total > Remain.Total)
            {
                WindowLib.Utils.MessageDialog.Show(_dialogService, "予定数を超えている数は入力できません", "エラー");
                return;
            }

            _item!.SetInputTotalPiece(Total ?? 0);

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        public string Title => "数量変更";

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
            var param = parameters.GetValue<ModifyItemDialogParam>("Param");
            _item = param.Item;
            var first = _item.Items.First();
            CdHimban = first.CdHimban;
            CdGtin13 = first.CdGtin13;
            NmHinSeishikimei = first.NmHinSeishikimei;
            StBoxType = first.StBoxType;
            NuBoxUnit = first.NuBoxUnit;
            CdBlock = _item.CdBlock;
            CdJuchuBin = _item.CdJuchuBin;
            CdDistGroup = _item.CdDistGroup;
            NmDistGroup = _item.NmDistGroup;
            CdShukkaBatch = _item.CdShukkaBatch;
            NmShukkaBatch = _item.NmShukkaBatch;
            Order = _item.Order;
            Result = _item.Result;
            Remain = _item.Remain;
            var input = _item.Input;
            Box = input.Box.ToString();
            Piece = input.Piece.ToString();
        }

        private void UpdateTotal()
        {
            int.TryParse(Box, out var box);
            int.TryParse(Piece, out var piece);
            var qty = new BoxedQuantity(NuBoxUnit, box, piece);
            Total = qty.Total;
        }
    }
}
