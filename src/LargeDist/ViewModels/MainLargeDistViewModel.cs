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
using System.Windows;

namespace LargeDist.ViewModels
{
    public class MainLargeDistViewModel : BindableBase
    {
        public DelegateCommand RefreshCommand { get; }
        public DelegateCommand SelectCommand { get; }
        public DelegateCommand ExitCommand { get; }

        private DateTime _deliveryDate = DateTime.Now + new TimeSpan(1, 0, 0, 0);
        public DateTime DeliveryDate
        {
            get => _deliveryDate;
            set => SetProperty(ref _deliveryDate, value);
        }

        private IEnumerable<Person> _personList = Enumerable.Empty<Person>();
        public IEnumerable<Person>  PersonList
        {
            get => _personList;
            set => SetProperty(ref _personList, value);
        }

        private Person? _currentPerson;
        public Person? CurrentPerson
        {
            get => _currentPerson;
            set => SetProperty(ref _currentPerson, value);
        }

        private ObservableCollection<LargeDistGroup> _groupList;
        public ObservableCollection<LargeDistGroup> GroupList
        {
            get => _groupList;
            set => SetProperty(ref _groupList, value);
        }


        private bool _canSelect;
        public bool CanSelect
        {
            get => _canSelect;
            set => SetProperty(ref _canSelect, value);
        }

        private IDialogService _dialogService;
        private IRegionManager _regionManager;

        public MainLargeDistViewModel(IDialogService dialogService, IRegionManager regionManager)
        {
            _dialogService = dialogService;
            _regionManager = regionManager;
            RefreshCommand = new DelegateCommand(Refresh);
            SelectCommand = new DelegateCommand(Select).ObservesCanExecute(() => CanSelect);
            ExitCommand = new DelegateCommand(Exit);

            try
            {
                SetupPersonList();
            }
            catch (Exception ex)
            {
                WindowLib.Utils.MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
        }

        private void SetupPersonList()
        {
            PersonList = PersonQueryService.GetAll();
        }

        private void Exit()
        {
            Application.Current.MainWindow.Close();
        }

        private void Select()
        {
            throw new NotImplementedException();
        }

        private void Refresh()
        {
            try
            {
                GroupList = LargeGroupQueryService.GetAll(DeliveryDate);
            }
            catch (Exception ex)
            {
                WindowLib.Utils.MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
        }
    }
}
