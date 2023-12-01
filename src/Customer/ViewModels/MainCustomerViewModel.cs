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

        private ObservableCollection<Models.Customer> _customers = new ObservableCollection<Models.Customer>();
        public ObservableCollection<Models.Customer> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        private Models.Customer? _currentCustomer;
        public Models.Customer? CurrentCustomer
        {
            get => _currentCustomer;
            set => SetProperty(ref _currentCustomer, value);
        }

        private bool _canEdit = false;
        public bool CanEdit
        {
            get => _canEdit;
            set => SetProperty(ref _canEdit, value);
        }

        public MainCustomerViewModel(IDialogService dialogService, IRegionManager regionManager)
        {
            _dialogService = dialogService;

            Add = new DelegateCommand(() =>
            {
                Syslog.Debug("MainCustomerViewModel:Add");
                regionManager.RequestNavigate("ContentRegion", nameof(InputCustomer), new NavigationParameters
                {
                    { "CurrentCustomer", null },
                });
            });

            Edit = new DelegateCommand(() =>
            {
                if (CurrentCustomer == null)
                {
                    return;
                }

                Syslog.Debug("MainCustomerViewModel:Edit");
                regionManager.RequestNavigate("ContentRegion", nameof(InputCustomer), new NavigationParameters
                {
                    { "CurrentCustomer", CurrentCustomer },
                });
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

            // fixme:社員コード + 社員名称
            Shain = "0000033550" + "　" + "小田　賢行";
            LoadDatas();
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
    }
}
