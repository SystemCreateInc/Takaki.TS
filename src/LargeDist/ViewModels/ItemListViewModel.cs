using DbLib;
using LargeDist.Infranstructures;
using LargeDist.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LargeDist.ViewModels
{
    public class ItemListViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand RefreshCommand { get; }
        public DelegateCommand BackCommand { get; }

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

        private bool _isFilterAll = true;
        public bool IsFilterAll
        {
            get => _isFilterAll;
            set
            {
                SetProperty(ref _isFilterAll, value);
                Refresh();
            }
        }

        private ObservableCollection<LargeDistItem> _items = new();
        public ObservableCollection<LargeDistItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private IRegionManager _regionManager;
        private IDialogService _dialogService;
        private ScopeLogger _logger = new ScopeLogger(nameof(ItemScanViewModel));
        private ScanGridController? _scanGridController;

        public ItemListViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;
            RefreshCommand = new DelegateCommand(Refresh);
            BackCommand = new DelegateCommand(Back);
        }

        private void Back()
        {
            _regionManager.Regions["ContentRegion"].NavigationService.Journal.GoBack();
        }

        private void Refresh()
        {
            try
            {
                var items = LargeDistQueryService.GetItemsByLargeDist(DeliveryDate, LargeDistGroup!, !IsFilterAll);
                Items = new(SetScanOrder(items));
            }
            catch (Exception ex)
            {
                WindowLib.Utils.MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
        }

        private IEnumerable<LargeDistItem> SetScanOrder(IEnumerable<LargeDistItem> items)
        {
            foreach (var item in items)
            {
                for (var i = 0; i < _scanGridController!.CustomOrderItems.Length; ++i) 
                {
                    if (_scanGridController!.CustomOrderItems[i].Item == item)
                    {
                        item.ScanOrder = i + 1;
                        break;
                    }
                }
            }

            // 並べ替え
            return items.OrderBy(x => x.ScanOrder ?? int.MaxValue).ToArray();
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
            var param = navigationContext.Parameters.GetValue<ItemListParam>("Param");
            DeliveryDate = param.DeliveryDate;
            Person = param.Person;
            LargeDistGroup = param.LargeDistGroup;
            ItemProgress = param.ItemProgress;
            _scanGridController = param.gridController;
            Refresh();
        }
    }
}
