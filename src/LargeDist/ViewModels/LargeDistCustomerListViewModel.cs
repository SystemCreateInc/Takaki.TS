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
    public class LargeDistCustomerListViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand BackCommand { get; }

        private DateTime _deliveryDate;
        public DateTime DeliveryDate
        {
            get => _deliveryDate;
            set => SetProperty(ref _deliveryDate, value);
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

        private int _nuBoxUnit;
        public int NuBoxUnit
        {
            get => _nuBoxUnit;
            set => SetProperty(ref _nuBoxUnit, value);
        }

        private BoxedQuantity _remain = new();
        public BoxedQuantity Remain
        {
            get => _remain;
            set => SetProperty(ref _remain, value);
        }

        private ObservableCollection<LargeDistCustomerItem> _items = new();
        public ObservableCollection<LargeDistCustomerItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private IRegionManager _regionManager;
        private IDialogService _dialogService;

        public LargeDistCustomerListViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;
            BackCommand = new DelegateCommand(Back);
        }

        private void Back()
        {
            _regionManager.Regions["ContentRegion"].NavigationService.Journal.GoBack();
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
            var param = navigationContext.Parameters.GetValue<CustomerListParam>("Param");
            var item = param.Item;
            CdBlock = item.CdBlock;
            CdBlock = item.CdJuchuBin;
            CdJuchuBin = item.CdJuchuBin;
            CdDistGroup = item.CdDistGroup;
            NmDistGroup = item.NmDistGroup;
            CdShukkaBatch = item.CdShukkaBatch;
            NmShukkaBatch = item.NmShukkaBatch;
            CdHimban = item.CdHimban;
            CdGtin13 = item.CdGtin13;
            NmHinSeishikimei = item.NmHinSeishikimei;
            NuBoxUnit = item.NuBoxUnit;
            Remain = item.Remain;
            Items = new(param.Item.CustomerItems);
        }
    }
}
