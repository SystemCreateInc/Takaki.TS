using DbLib;
using LargeDist.Infranstructures;
using MahApps.Metro.Controls.Dialogs;
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
    public class ScanOrderConfigViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand BackCommand { get; }
        public DelegateCommand<object> SlotPushCommand { get; }

        private ObservableCollection<int?> _indexes = new (new int?[18]);
        public ObservableCollection<int?> Indexes
        {
            get => _indexes;
            set => SetProperty(ref _indexes, value);
        }

        private bool _canSave;
        public bool CanSave
        {
            get => _canSave;
            set => SetProperty(ref _canSave, value);
        }


        private IRegionNavigationService? _regionNavigationService;
        private IDialogService _dialogService;
        private int _nextIndex = 1;

        public ScanOrderConfigViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            SaveCommand = new DelegateCommand(Save).ObservesCanExecute(() => CanSave);
            BackCommand = new DelegateCommand(Back);
            SlotPushCommand = new(SlotPush);
        }

        private void SlotPush(object obj)
        {
            if (obj is int id)
            {
                if (Indexes[id] is null)
                {
                    Indexes[id] = _nextIndex++;
                    CanSave = true;
                }
            }
        }

        private void Back()
        {
            _regionNavigationService?.Journal.GoBack();
        }

        private void Save()
        {
            try
            {
                LargeDistGridOrderRepository.Save(Indexes);
                Back();
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

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _regionNavigationService = navigationContext.NavigationService;
        }
    }
}
