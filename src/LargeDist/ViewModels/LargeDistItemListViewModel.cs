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
    public class LargeDistItemListViewModel : BindableBase, INavigationAware
	{
        public DelegateCommand BackCommand { get; }

        private DateTime _deliveryDate;
        public DateTime DeliveryDate
        {
            get => _deliveryDate;
            set => SetProperty(ref _deliveryDate, value);
        }

        private LargeDistGroup? _largeDistGroup;
        public LargeDistGroup? LargeDistGroup
        {
            get => _largeDistGroup;
            set => SetProperty(ref _largeDistGroup, value);
        }

        private ObservableCollection<LargeDistItem> _items = new();
        public ObservableCollection<LargeDistItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private IRegionManager _regionManager;
        private IDialogService _dialogService;

        public LargeDistItemListViewModel(IRegionManager regionManager, IDialogService dialogService)
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
            var param = navigationContext.Parameters.GetValue<LargeDistItemListParam>("Param");
            DeliveryDate = param.DeliveryDate;
            LargeDistGroup = param.LargeDistGroup;
            Items = new(param.LargeDistItems);
        }
    }
}
