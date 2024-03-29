﻿using DbLib;
using ImTools;
using LargeDist.Infranstructures;
using LargeDist.Models;
using LargeDist.Views;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace LargeDist.ViewModels
{
    public class ItemScanViewModel : BindableBase, INavigationAware, IRegionMemberLifetime
    {
        public DelegateCommand RefreshCommand { get; }
        public DelegateCommand ItemListCommand { get; }
        public DelegateCommand CancelModeCommand { get; }
        public DelegateCommand DeleteItemCommand { get; }
        public DelegateCommand ScanOrderCommand { get; }
        public DelegateCommand BackCommand { get; }
        public DelegateCommand BlockLargeDistCommand { get; }
        public DelegateCommand ItemLargeDistCommand { get; }
        public DelegateCommand EnterScancodeCommand { get; }
        public DelegateCommand<object> SlotPushCommand { get; }

        private DateTime _deliveryDate;
        public DateTime DeliveryDate
        {
            get => _deliveryDate;
            set => SetProperty(ref _deliveryDate, value);
        }

        private Person? _person;
        public Person? Person
        {
            get => _person;
            set => SetProperty(ref _person, value);
        }

        private string _modeText = string.Empty;
        public string ModeText
        {
            get => _modeText;
            set => SetProperty(ref _modeText, value);
        }

        private LargeDistGroup? _largeDistGroup;
        public LargeDistGroup? LargeDistGroup
        {
            get => _largeDistGroup;
            set => SetProperty(ref _largeDistGroup, value);
        }

        private ItemProgress _itemProgress = new ItemProgress();
        public ItemProgress ItemProgress
        {
            get => _itemProgress;
            set => SetProperty(ref _itemProgress, value);
        }

        private string _scancode = string.Empty;
        public string Scancode
        {
            get => _scancode;
            set => SetProperty(ref _scancode, value);
        }

        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private bool _isCancelMode;
        public bool IsCancelMode
        {
            get => _isCancelMode;
            set => SetProperty(ref _isCancelMode, value);
        }

        private bool _isScannedItem;
        public bool IsScannedItem
        {
            get => _isScannedItem;
            set
            {
                SetProperty(ref _isScannedItem, value);
                IsEmptyItem = !_isScannedItem;
            }
        }

        private bool _canScanOrder;
        public bool IsEmptyItem
        {
            get => _canScanOrder;
            set => SetProperty(ref _canScanOrder, value);
        }

        public ScanGridController ScanGrid { get; } = new ScanGridController();

        public bool KeepAlive { get; set; } = true;

        private readonly IDialogService _dialogService;
        private readonly ScopeLogger _logger = new ScopeLogger(nameof(ItemScanViewModel));
        private IRegionNavigationService? _regionNavigationService;
        private bool _initialized;
        private bool _requestClearItem;
        private bool _requestGridSetup;

        public ItemScanViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            RefreshCommand = new DelegateCommand(Refresh);
            ItemListCommand = new DelegateCommand(ItemList);
            CancelModeCommand = new DelegateCommand(ToggleCancelMode).ObservesCanExecute(() => IsEmptyItem);
            DeleteItemCommand = new DelegateCommand(DeleteItem);
            ScanOrderCommand = new DelegateCommand(ScanOrder).ObservesCanExecute(() => IsEmptyItem);
            BackCommand = new DelegateCommand(Back);
            BlockLargeDistCommand = new DelegateCommand(BlockLargeDist).ObservesCanExecute(() => IsScannedItem);
            ItemLargeDistCommand = new DelegateCommand(ItemLargeDist).ObservesCanExecute(() => IsScannedItem);
            EnterScancodeCommand = new DelegateCommand(EnterScancode);
            SlotPushCommand = new DelegateCommand<object>(SlotPush);

            try
            {
                LargeLockRepository.UnlockAll();
            }
            catch (Exception ex)
            {
                WindowLib.Utils.MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
        }

        private void SlotPush(object obj)
        {
            if (obj is ScanSlotItem item)
            {
                ScanGrid.UnselectSlotAll();
                item.IsSelected = true;
            }
        }

        private void EnterScancode()
        {
            if (string.IsNullOrEmpty(Scancode))
            {
                return;
            }

            try
            {
                if (IsCancelMode)
                {
                    ScanCancelMode();
                }
                else
                {
                    ScanDistMode();
                }

                Scancode = string.Empty;
                Message = string.Empty;
            }
            catch (Exception ex)
            {
                _logger.Debug($"EnterScancode: {ex}");
                Message = ex.Message;
                Scancode = string.Empty;
            }
        }

        private void ScanCancelMode()
        {
            var items = LargeDistQueryService.FindItem(DeliveryDate, LargeDistGroup!, Scancode, IsCancelMode);
            var item = SelectItem(items);
            if (item is null)
            {
                return;
            }

            _dialogService.ShowDialog(nameof(CancelDistDialog), new DialogParameters
            {
                {
                    "Param", new CancelDistDialogParam(item, _person!)
                }
            },
            rc =>
            {
                ToggleCancelMode();
                Refresh();
            });
        }

        private void ScanDistMode()
        {
            var items = LargeDistQueryService.FindItem(DeliveryDate, LargeDistGroup!, Scancode, IsCancelMode);
            // スキャン済みの商品を除く
            items = items.Where(x => !ScanGrid.IsPushedItem(x)).ToArray();
            if (!items.Any())
            {
                throw new Exception("スキャン済みの商品です");
            }

            var item = SelectItem(items);
            if (item is null)
            {
                return;
            }

            LargeLockRepository.Lock(_largeDistGroup!.CdLargeGroup, item);
            ScanGrid.PushItem(_largeDistGroup!.CdLargeGroup,item);
            Refresh();
        }

        private LargeDistItem? SelectItem(IEnumerable<LargeDistItem> items)
        {
            if (items.Count() == 1)
            {
                return items.First();
            }

            LargeDistItem? selectedItem = null;
            _dialogService.ShowDialog(nameof(SelectItemDialog), new DialogParameters
            {
                {
                    "Param", new SelectItemDialogParam(items)
                }
            },
            rc =>
            {
                rc.Parameters.TryGetValue("SelectedItem", out selectedItem);
            });

            return selectedItem;
        }

        private void ItemLargeDist()
        {
            _regionNavigationService.RequestNavigate(nameof(ItemLargeDist), new NavigationParameters
            {
                {
                    "Param", new LargeDistParam(DeliveryDate, Person!, LargeDistGroup!, ScanGrid)
                }
            });

            _requestClearItem = true;
        }

        private void BlockLargeDist()
        {
            _regionNavigationService.RequestNavigate(nameof(BlockLargeDist), new NavigationParameters
            {
                {
                    "Param", new LargeDistParam(DeliveryDate, Person!, LargeDistGroup!, ScanGrid)
                }
            });

            _requestClearItem = true;
        }

        private void Back()
        {
            KeepAlive = false;
            _regionNavigationService?.Journal.GoBack();
        }

        private void ScanOrder()
        {
            _regionNavigationService.RequestNavigate(nameof(ScanOrderConfig), new NavigationParameters
            {
                {
                    "Param", new ScanOrderConfigParam(ScanGrid)
                }
            });

            _requestGridSetup = true;
        }

        private void DeleteItem()
        {
            _logger.Debug("Delete Item");

            if (ScanGrid.SelectedItem?.Item == null)
            {
                return;
            }

            try
            {
                LargeLockRepository.Unlock(_largeDistGroup!.CdLargeGroup, ScanGrid.SelectedItem.Item);
                ScanGrid.DeleteSelectedItem();
                Refresh();
            }
            catch (Exception ex)
            {
                WindowLib.Utils.MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
        }

        private void ToggleCancelMode()
        {
            IsCancelMode = !IsCancelMode;
            _logger.Debug($"Cancel Mode {IsCancelMode}");
        }

        private void ItemList()
        {
            _regionNavigationService.RequestNavigate(nameof(ItemList), new NavigationParameters
            {
                {
                    "Param", new ItemListParam(DeliveryDate, Person!, LargeDistGroup!, ScanGrid, ItemProgress)
                }
            });
        }

        private void Refresh()
        {
            ItemProgress = LargeDistQueryService.GetProgress(DeliveryDate, LargeDistGroup!);
            IsScannedItem = ScanGrid.HasScanItem;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (_initialized)
            {
                if (_requestClearItem)
                {
                    ClearItem();
                }

                if (_requestGridSetup)
                {
                    _logger.Debug("Request Grid Setup");
                    SetupGrid();
                }

                Refresh();
                return;
            }

            _regionNavigationService = navigationContext.NavigationService;
            var param = navigationContext.Parameters.GetValue<ScanItemParam>("Param");
            DeliveryDate = param.DeliveryDate;
            LargeDistGroup = param.LargeDistGroup;
            Person = param.Person;
            SetupGrid();
            Refresh();
            _initialized = true;
            _logger.Debug("Initialized");
        }

        private void ClearItem()
        {
            _logger.Debug("Request Clear Item");
            try
            {
                LargeLockRepository.UnlockAll();
                ScanGrid.Clear();
                _requestClearItem = false;
            }
            catch (Exception ex)
            {
                WindowLib.Utils.MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private void SetupGrid()
        {
            _logger.Debug("Setup Grid");
            ScanGrid.Clear();

            try
            {
                var order = LargeDistGridOrderRepository.Get();
                ScanGrid.SetOrder(order);
            }
            catch (Exception ex)
            {
                WindowLib.Utils.MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }

            ScanGrid.SetNextHeadSlot();
            _requestGridSetup = false;
        }
    }
}
