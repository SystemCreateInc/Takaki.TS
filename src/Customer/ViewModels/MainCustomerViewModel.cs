using Customer.Loader;
using Customer.Models;
using Customer.Views;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;
using WindowLib.Utils;

namespace Customer.ViewModels
{
    public class MainCustomerViewModel : BindableBase
    {
        public DelegateCommand Add { get; }
        public DelegateCommand Edit { get; }
        public DelegateCommand Exit { get; }
        public DelegateCommand LeftDoubleClick { get; }

        private readonly IDialogService _dialogService;

        private string _shain = string.Empty;
        public string Shain
        {
            get => _shain;
            set => SetProperty(ref _shain, value);
        }

        private ObservableCollection<SumCustomer> _customers = new ObservableCollection<SumCustomer>();
        public ObservableCollection<SumCustomer> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        private SumCustomer? _currentCustomer;
        public SumCustomer? CurrentCustomer
        {
            get => _currentCustomer;
            set
            {
                SetProperty(ref _currentCustomer, value);
                CanEdit = CurrentCustomer is not null && SelectedShain;
            }
        }

        private bool _canEdit = false;
        public bool CanEdit
        {
            get => _canEdit;
            set => SetProperty(ref _canEdit, value);
        }

        private bool _selectedShain = false;
        public bool SelectedShain
        {
            get => _selectedShain;
            set => SetProperty(ref _selectedShain, value);
        }

        private ShainInfo? _shainInfo = new ShainInfo();

        IRegionManager _regionManager;

        public MainCustomerViewModel(IDialogService dialogService, IRegionManager regionManager)
        {
            _dialogService = dialogService;
            _regionManager = regionManager;

            SetShain();
            LoadDatas();

            Add = new DelegateCommand(() =>
            {
                Syslog.Debug("MainCustomerViewModel:Add");
                ShowDialog(false);
            }).ObservesCanExecute(() => SelectedShain);

            Edit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainCustomerViewModel:Edit");
                ShowDialog(true);
            }).ObservesCanExecute(() => CanEdit);

            Exit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainCustomerViewModel:Exit");
                Application.Current.MainWindow.Close();
            });

            LeftDoubleClick = new DelegateCommand(() =>
            {
                Syslog.Debug("MainCustomerViewModel:LeftDoubleClick");
                Edit.Execute();
            }).ObservesCanExecute(() => CanEdit);
        }

        private void SetShain()
        {
            _shainInfo = ShainLoader.Get();
            SelectedShain = _shainInfo is not null;
            Shain = $"{_shainInfo?.HenkoshaCode}  {_shainInfo?.HenkoshaName}";
        }

        private void LoadDatas()
        {
            try
            {
                CollectionViewHelper.SetCollection(Customers, CustomerLoader.Get());
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }

        private void ShowDialog(bool IsEdit)
        {
            _regionManager.RequestNavigate("ContentRegion", nameof(InputCustomer), new NavigationParameters
                {
                    { "CurrentCustomer", IsEdit ? CurrentCustomer : null },
                    { "ShainInfo", _shainInfo },
                });
        }
    }
}
