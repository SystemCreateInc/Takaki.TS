using LargeDist.Infranstructures;
using LargeDist.Models;
using LargeDist.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using WindowLib.Utils;

namespace LargeDist.ViewModels
{
    public class MainLargeDistViewModel : BindableBase
    {
        public DelegateCommand RefreshCommand { get; }
        public DelegateCommand SelectCommand { get; }
        public DelegateCommand ExitCommand { get; }
        public DelegateCommand EnterComboCommand { get; }

        private DateTime _deliveryDate = DateTime.Now + new TimeSpan(1, 0, 0, 0);
        public DateTime DeliveryDate
        {
            get => _deliveryDate;
            set
            {
                SetProperty(ref _deliveryDate, value);
                Refresh();
            } 
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
            set
            {
                SetProperty(ref _currentPerson, value);
                UpdateCanSelect();
            }
        }

        private string _personCode = string.Empty;
        public string PersonCode
        {
            get => _personCode;
            set => SetProperty(ref _personCode, value);
        }


        private ObservableCollection<LargeDistGroup> _groupList = new ObservableCollection<LargeDistGroup>();
        public ObservableCollection<LargeDistGroup> GroupList
        {
            get => _groupList;
            set => SetProperty(ref _groupList, value);
        }

        private LargeDistGroup? _selectedItem;
        public LargeDistGroup? SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                UpdateCanSelect();
            }
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
            RefreshCommand = new(Refresh);
            SelectCommand = new DelegateCommand(Select).ObservesCanExecute(() => CanSelect);
            ExitCommand = new(Exit);
            EnterComboCommand = new(EnterCombo);

            try
            {
                LargeLockRepository.UnlockAll();
                SetupPersonList();
            }
            catch (Exception ex)
            {
                WindowLib.Utils.MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }

            Refresh();
        }

        private void EnterCombo()
        {
            // 入力テキストが7桁未満なら先頭０を追加して７桁にする
            if (PersonCode.Length < 7)
            {
                PersonCode = PersonCode.PadLeft(7, '0');
            }

            CurrentPerson = PersonList.FirstOrDefault(x => x.Code == PersonCode);
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
            _regionManager.RequestNavigate("ContentRegion", nameof(ItemScan), new NavigationParameters
            {
                {
                    "Param", new ScanItemParam(DeliveryDate, CurrentPerson!, SelectedItem!)
                },
            }); ;
        }

        private void Refresh()
        {
            try
            {
                CollectionViewHelper.SetCollection(GroupList, LargeGroupQueryService.GetAll(DeliveryDate));
            }
            catch (Exception ex)
            {
                WindowLib.Utils.MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
        }

        private void UpdateCanSelect()
        {
            CanSelect = CurrentPerson is not null && SelectedItem is not null;
        }
    }
}
