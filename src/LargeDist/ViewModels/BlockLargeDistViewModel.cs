using DbLib;
using LargeDist.Infranstructures;
using LargeDist.Models;
using LargeDist.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LargeDist.ViewModels
{
    public class BlockLargeDistViewModel : BindableBase, INavigationAware, IRegionMemberLifetime
    {
        public DelegateCommand ItemListCommand { get; }
        public DelegateCommand ItemDescCommand { get; }
        public DelegateCommand ModifyQtyCommand { get; }
        public DelegateCommand ModifyBoxUnitCommand { get; }
        public DelegateCommand StopItemCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand MovePrevCommand { get; }
        public DelegateCommand MoveNextCommand { get; }
        public DelegateCommand CompleteCommand { get; }
        public DelegateCommand<object> SlotPushCommand { get; }

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

        private bool _isPrintLabel = true;
        public bool IsPrintLabel
        {
            get => _isPrintLabel;
            set => SetProperty(ref _isPrintLabel, value);
        }

        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private ScanGridController _scanGrid = new ScanGridController
        {
            DisableHeading = true,
        };
        public ScanGridController ScanGrid
        {
            get => _scanGrid;
            set => SetProperty(ref _scanGrid, value);
        }

        private bool _isSelectedItem;
        public bool IsSelectedItem
        {
            get => _isSelectedItem;
            set => SetProperty(ref _isSelectedItem, value);
        }

        public bool KeepAlive { get; set; } = true;

        private IRegionNavigationService? _regionNavigationService;
        private IDialogService _dialogService;
        private DateTime _deliveryDate;
        private ScanGridController? _parentGridContoller;
        private LargeDistGroup? _largeDistGroup;
        private ScopeLogger _logger = new ScopeLogger(nameof(ItemScanViewModel));
        private BlockLargeDistController? _blockLargeDistController;
        private Person _person = new("", "");
        private bool _initialized = false;

        public BlockLargeDistViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            ItemListCommand = new DelegateCommand(ItemList);
            ItemDescCommand = new DelegateCommand(ItemDesc).ObservesCanExecute(() => IsSelectedItem);
            ModifyQtyCommand = new DelegateCommand(ModifyQty).ObservesCanExecute(() => IsSelectedItem);
            ModifyBoxUnitCommand = new DelegateCommand(ModifyBoxUnit).ObservesCanExecute(() => IsSelectedItem);
            StopItemCommand = new DelegateCommand(StopItem).ObservesCanExecute(() => IsSelectedItem);
            CancelCommand = new DelegateCommand(Cancel);
            MovePrevCommand = new DelegateCommand(MovePrev);
            MoveNextCommand = new DelegateCommand(MoveNext);
            CompleteCommand = new DelegateCommand(Complete);
            SlotPushCommand = new DelegateCommand<object>(SlotPush);
        }

        private void SlotPush(object obj)
        {
            if (obj is ScanSlotItem item)
            {
                ScanGrid.UnselectSlotAll();
                item.IsSelected = true;
                IsSelectedItem = true;
            }
        }

        private void Complete()
        {
            try
            {
                _blockLargeDistController!.SaveCurrentItem(_person);

                if (_blockLargeDistController.IsCompleted())
                {
                    NotifyCompleted();
                    Cancel();
                    return;
                }

                MoveNext();
            }
            catch (Exception ex)
            {
                WindowLib.Utils.MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
        }

        private void NotifyCompleted()
        {
            _dialogService.ShowDialog(nameof(CompletedDialog), new DialogParameters
            {
                {  "Text", $"大仕分グループ：{_largeDistGroup!.CdLargeGroup} {_largeDistGroup!.NmLargeGroup}" },
            },
            rc => { });
        }

        private void MoveNext()
        {
            _blockLargeDistController!.MoveNext();
            UpdateDisplayInfo();
        }

        private void MovePrev()
        {
            _blockLargeDistController!.MovePrev();
            UpdateDisplayInfo();
        }

        private void Cancel()
        {
            KeepAlive = false;
            _regionNavigationService?.Journal.GoBack();
        }

        private void StopItem()
        {
            if (ScanGrid.SelectedItem is ScanSlotItem slotItem && slotItem.Item is not null)
            {
                slotItem.IsStopped = !slotItem.IsStopped;
            }
        }

        private void ModifyBoxUnit()
        {
            if (ScanGrid.SelectedItem is ScanSlotItem slotItem && slotItem.Item is not null)
            {
                _dialogService.ShowDialog(nameof(ModifyBoxUnitDialog), new DialogParameters
                {
                    { "Param", new ModifyItemDialogParam(slotItem.Item) },
                },
                rc =>
                {
                    if (rc.Result != ButtonResult.OK)
                    {
                        return;
                    }

                    var unit = rc.Parameters.GetValue<int>("Unit");
                    slotItem.Item.SetBoxUnit(unit);
                    slotItem.UpdateItem();
                });
            }
        }

        private void ModifyQty()
        {
            if (ScanGrid.SelectedItem is ScanSlotItem slotItem && slotItem.Item is not null)
            {
                _dialogService.ShowDialog(nameof(ModifyQtyDialog), new DialogParameters
                {
                    { "Param", new ModifyItemDialogParam(slotItem.Item) },
                },
                rc =>
                {
                    slotItem.UpdateItem();
                });
            }
        }

        private void ItemDesc()
        {
            if (ScanGrid.SelectedItem is ScanSlotItem slotItem && slotItem.Item is not null)
            {
                _regionNavigationService.RequestNavigate(nameof(LargeDistCustomerList), new NavigationParameters
                {
                    {
                        "Param", new CustomerListParam(slotItem.Item)
                    }
                });
            }
        }

        private void ItemList()
        {
            _regionNavigationService.RequestNavigate(nameof(LargeDistItemList), new NavigationParameters
            {
                {
                    "Param", new LargeDistItemListParam(_deliveryDate, _largeDistGroup!, ScanGrid.LargeDistItems)
                }
            });
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (_initialized)
            {
                return;
            }

            _regionNavigationService = navigationContext.NavigationService;
            if (navigationContext.Parameters.TryGetValue<LargeDistParam>("Param", out var param))
            {
                _deliveryDate = param.DeliveryDate;
                _parentGridContoller = param.ScanGrid;
                _largeDistGroup = param.LargeDistGroup;
                _blockLargeDistController = new BlockLargeDistController(_largeDistGroup, _parentGridContoller);
                _person = param.Person;
                UpdateDisplayInfo();
                _initialized = true;
            }
        }

        private void UpdateDisplayInfo()
        {
            ScanGrid.Clear();
            _blockLargeDistController!.SetupGridController(ScanGrid);
            var item = _blockLargeDistController.CurrentItem;
            CdBlock = item.CdBlock;
            CdJuchuBin = item.CdJuchuBin;
            CdDistGroup = item.CdDistGroup;
            NmDistGroup = item.NmDistGroup;
            CdShukkaBatch = item.CdShukkaBatch;
            NmShukkaBatch = item.NmShukkaBatch;
        }
    }
}
