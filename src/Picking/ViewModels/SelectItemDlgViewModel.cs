using Picking.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using WindowLib.Utils;

namespace Picking.ViewModels
{
    public class SelectItemDlgViewModel : BindableBase, IDialogAware
    {
        private string _title = "商品選択";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public event Action<IDialogResult>? RequestClose = null;


        private DistColor? _distcolor = null;
        public DistColor? DistColor
        {
            get => _distcolor;
            set => SetProperty(ref _distcolor, value);
        }

        private List<DistItemSeq> _selectitems = new List<DistItemSeq>();
        public List<DistItemSeq> SelectItems
        {
            get => _selectitems;
            set => SetProperty(ref _selectitems, value);
        }

        private DistItemSeq _currentselectitems = new DistItemSeq();
        public DistItemSeq CurrentSelectItem
        {
            get => _currentselectitems;
            set => SetProperty(ref _currentselectitems, value);
        }

        private string _scancode = string.Empty;
        public string Scancode
        {
            get => _scancode;
            set => SetProperty(ref _scancode, value);
        }

        private string _cdgtin13 = string.Empty;
        public string CdGtin13
        {
            get => _cdgtin13;
            set => SetProperty(ref _cdgtin13, value);
        }

        private int _selectitemidx = 0;

        public int SelectItemIdx
        {
            get => _selectitemidx;
            set
            {
                SetProperty(ref _selectitemidx, value);
                CanUp = value == 0 ? false : true;
                CanDown = value == SelectItems.Count-1 ? false : true;
            }
        }

        private bool _canup = false;
        public bool CanUp
        {
            get => _canup;
            set => SetProperty(ref _canup, value);
        }

        private bool _candown = false;
        public bool CanDown
        {
            get => _candown;
            set => SetProperty(ref _candown, value);
        }


        public DelegateCommand Save { get; }
        public DelegateCommand Cancel { get; }

        public DelegateCommand OnUp { get; }
        public DelegateCommand OnDown { get; }


        private readonly IDialogService _dialogService;

        public SelectItemDlgViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Save = new DelegateCommand(() =>
            {
                try
                {
                    var result = new DialogResult(ButtonResult.OK);

                    result.Parameters.Add("selectitem", CurrentSelectItem);
                    result.Parameters.Add("selectitemidx", SelectItemIdx);

                    RequestClose?.Invoke(result);
                }
                catch (Exception e)
                {
                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                    return;
                }
            });

            Cancel = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });

            OnUp = new DelegateCommand(() =>
            {
                if (0 < SelectItemIdx)
                    SelectItemIdx--;
            }, () => CanUp).ObservesProperty(() => CanUp);

            OnDown = new DelegateCommand(() =>
            {
                if (SelectItemIdx <= SelectItems.Count)
                    SelectItemIdx++;
            }, () => CanDown).ObservesProperty(() => CanDown);
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
            Scancode = parameters.GetValue<string>("scancode");
            DistColor = parameters.GetValue<DistColor>("distcolor");
            SelectItems = parameters.GetValue<List<DistItemSeq>>("selectitems");
            SelectItemIdx = 0;

            // ＪＡＮ設定
            if (SelectItems.Count!=0)
            {
                CdGtin13 = SelectItems[0].CdGtin13;
                if (CdGtin13 == "")
                    CdGtin13 = Scancode;
            }
}
    }
}
