using DbLib;
using ImTools;
using LargeDist.Models;
using LargeDist.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using System.Windows.Threading;

namespace LargeDist.ViewModels
{
    public class ItemLargeDistViewModel : BindableBase, INavigationAware, IRegionMemberLifetime
    {
        public DelegateCommand ItemListCommand { get; }
        public DelegateCommand ItemDescCommand { get; }
        public DelegateCommand ModifyQtyCommand { get; }
        public DelegateCommand ModifyBoxUnitCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand MovePrevCommand { get; }
        public DelegateCommand MoveNextCommand { get; }
        public DelegateCommand CompleteCommand { get; }
        public DelegateCommand<object> RowPushCommand { get; }

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

        private string? _cdHimban;
        public string? CdHimban
        {
            get => _cdHimban;
            set => SetProperty(ref _cdHimban, value);
        }

        private string? _cdgtin13;
        public string? CdGtin13
        {
            get => _cdgtin13;
            set => SetProperty(ref _cdgtin13, value);
        }


        private int _nuBoxUnit;
        public int NuBoxUnit
        {
            get => _nuBoxUnit;
            set => SetProperty(ref _nuBoxUnit, value);
        }

        private string? _nmHinSeishikimei;
        public string? NmHinSeishikimei
        {
            get => _nmHinSeishikimei;
            set => SetProperty(ref _nmHinSeishikimei, value);
        }


        private bool _isPrintLabel = true;
        public bool IsPrintLabel
        {
            get => _largeDistState.IsPrintLabel;
            set
            {
                SetProperty(ref _isPrintLabel, value);
                _largeDistState.IsPrintLabel = value;
            }
        }

        private bool _isNoPrintLabel;
        public bool IsNoPrintLabel
        {
            get => !_largeDistState.IsPrintLabel;
            set
            {
                SetProperty(ref _isNoPrintLabel, value);
                IsPrintLabel = !value;
            }
        }

        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private bool _isSelectedItem;
        public bool IsSelectedItem
        {
            get => _isSelectedItem;
            set => SetProperty(ref _isSelectedItem, value);
        }

        private bool _canShifted = true;
        public bool CanShifted
        {
            get => _canShifted;
            set => SetProperty(ref _canShifted, value);
        }

        private ItemChartController _chart = new ItemChartController();
        public ItemChartController Chart
        {
            get => _chart;
            set => SetProperty(ref _chart, value);
        }


        private readonly IDialogService _dialogService;
        private readonly DispatcherTimer _idleTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle, System.Windows.Application.Current.Dispatcher);
        private readonly IEventAggregator _eventAggregator;
        private readonly LargeDistState _largeDistState;
        private IRegionNavigationService? _regionNavigationService;
        private DateTime _deliveryDate;
        private ScanGridController? _parentGridContoller;
        private LargeDistGroup? _largeDistGroup;
        private ScopeLogger _logger = new ScopeLogger(nameof(ItemScanViewModel));
        private ItemLargeDistController? _itemLargeDistController;
        private Person _person = new("", "");
        private bool _initialized = false;

        public ItemLargeDistViewModel(IDialogService dialogService, IEventAggregator eventAggregator, LargeDistState largeDistState)
        {
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;
            _largeDistState = largeDistState;
            ItemListCommand = new DelegateCommand(ItemList);
            ItemDescCommand = new DelegateCommand(ItemDesc).ObservesCanExecute(() => IsSelectedItem);
            ModifyQtyCommand = new DelegateCommand(ModifyQty).ObservesCanExecute(() => IsSelectedItem);
            ModifyBoxUnitCommand = new DelegateCommand(ModifyBoxUnit);
            CancelCommand = new DelegateCommand(Cancel).ObservesCanExecute(() => CanShifted);
            MovePrevCommand = new DelegateCommand(MovePrev);
            MoveNextCommand = new DelegateCommand(MoveNext);
            CompleteCommand = new DelegateCommand(Complete);
            RowPushCommand = new DelegateCommand<object>(RowPush);

            _idleTimer.Tick += (s, e) => CheckShifted();
            _idleTimer.Start();
        }

        private void CheckShifted()
        {
            bool shifted = (Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) != 0
                || (Keyboard.GetKeyStates(Key.RightShift) & KeyStates.Down) != 0;

            CanShifted = shifted;
        }

        private void RowPush(object obj)
        {
            if (obj is ChartItem item && Chart.Items[0] != item)
            {
                Chart.UnselectLineAll();
                item.IsSelected = true;
                IsSelectedItem = true;
            }
        }

        private void Complete()
        {
            try
            {
                _itemLargeDistController!.SaveCurrentItem(_person);

                if (IsPrintLabel)
                {
                    new LargeDistLabelPrinter(_itemLargeDistController.GetLabelForCurrentItem(), _eventAggregator).Print(_dialogService);
                }

                if (_itemLargeDistController.IsCompleted())
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
            _itemLargeDistController!.MoveNext();
            UpdateDisplayInfo();
        }

        private void MovePrev()
        {
            _itemLargeDistController!.MoveNext();
            UpdateDisplayInfo();
        }

        private void Cancel()
        {
            KeepAlive = false;
            _regionNavigationService?.Journal.GoBack();
        }

        private void ModifyBoxUnit()
        {
            _dialogService.ShowDialog(nameof(ModifyBoxUnitDialog), new DialogParameters
            {
                { "Param", new ModifyBoxUnitDialogParam(_itemLargeDistController!.CurrentItem.Items.First()) },
            },
            rc =>
            {
                if (rc.Result != ButtonResult.OK) 
                {
                    return;
                }

                var unit = rc.Parameters.GetValue<int>("Unit");
                Chart.SetBoxUnit(unit);
                NuBoxUnit = unit;
            });
        }

        private void ModifyQty()
        {
            if (Chart.SelectedItem is ChartItem chartItem && chartItem.Item is not null)
            {
                _dialogService.ShowDialog(nameof(ModifyQtyDialog), new DialogParameters
                {
                    { "Param", new ModifyItemDialogParam(chartItem.Item) },
                },
                rc =>
                {
                    Chart.UpdateAllItems();
                });
            }
        }

        private void ItemDesc()
        {
            if (Chart.SelectedItem is ChartItem chartItem && chartItem.Item is not null)
            {
                _regionNavigationService.RequestNavigate(nameof(LargeDistCustomerList), new NavigationParameters
                {
                    {
                        "Param", new CustomerListParam(chartItem.Item)
                    }
                });
            }
        }

        private void ItemList()
        {
            _regionNavigationService.RequestNavigate(nameof(LargeDistItemList), new NavigationParameters
            {
                {
                    "Param", new LargeDistItemListParam(_deliveryDate, _largeDistGroup!, _parentGridContoller!.LargeDistItems)
                }
            });
        }

        public bool KeepAlive { get; set; } = true;

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
                _itemLargeDistController = new ItemLargeDistController(_largeDistGroup, _parentGridContoller);
                _person = param.Person;
                UpdateDisplayInfo();
                _initialized = true;
            }
        }

        private void UpdateDisplayInfo()
        {
            Chart.Clear();
            _itemLargeDistController!.SetupChartController(Chart);
            var item = _itemLargeDistController.CurrentItem;
            CdJuchuBin = item.CdJuchuBin;
            CdDistGroup = item.CdDistGroup;
            NmDistGroup = item.NmDistGroup;
            CdShukkaBatch = item.CdShukkaBatch;
            NmShukkaBatch = item.NmShukkaBatch;
            CdHimban = item.CdHimban;
            CdGtin13 = item.CdGtin13;
            NuBoxUnit = item.NuBoxUnit;
            NmHinSeishikimei = item.NmHinSeishikimei;
        }
    }
}
