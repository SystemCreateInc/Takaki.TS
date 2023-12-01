using DistLargeGroup.Models;
using DistLargeGroup.Views;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WindowLib.Utils;

namespace DistLargeGroup.ViewModels
{
    public class MainDistLargeGroupViewModel : BindableBase
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

        private ObservableCollection<Models.DistLargeGroup> _distLargeGroups = new ObservableCollection<Models.DistLargeGroup>();
        public ObservableCollection<Models.DistLargeGroup> DistLargeGroups
        {
            get => _distLargeGroups;
            set => SetProperty(ref _distLargeGroups, value);
        }

        private Models.DistLargeGroup? _currentDistLargeGroup;
        public Models.DistLargeGroup? CurrentDistLargeGroup
        {
            get => _currentDistLargeGroup;
            set
            {
                SetProperty(ref _currentDistLargeGroup, value);
                CanEdit = CurrentDistLargeGroup != null;
            }
        }

        private bool _canEdit = false;
        public bool CanEdit
        {
            get => _canEdit;
            set => SetProperty(ref _canEdit, value);
        }

        public MainDistLargeGroupViewModel(IDialogService dialogService, IRegionManager regionManager)
        {
            _dialogService = dialogService;

            Add = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistLargeGroupViewModel:Add");
                regionManager.RequestNavigate("ContentRegion", nameof(InputDistLargeGroup), new NavigationParameters
                {
                    { "CurrentDistLargeGroup", null },
                });
            });

            Edit = new DelegateCommand(() =>
            {
                if (CurrentDistLargeGroup == null)
                {
                    return;
                }

                Syslog.Debug("MainDistLargeGroupViewModel:Edit");
                regionManager.RequestNavigate("ContentRegion", nameof(InputDistLargeGroup), new NavigationParameters
                {
                    { "CurrentDistLargeGroup", CurrentDistLargeGroup },
                });
            }).ObservesCanExecute(() => CanEdit);

            Exit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistLargeGroupViewModel:Exit");
                Application.Current.MainWindow.Close();
            });

            LeftDoubleClick = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistLargeGroupViewModel:LeftDoubleClick");
                Edit.Execute();
            }).ObservesCanExecute(() => CanEdit);

            // fixme:社員コード + 社員名称
            Shain = "0000033550" + "　" + "小田賢行";

            LoadDatas();
        }

        private void LoadDatas()
        {
            try
            {
                CollectionViewHelper.SetCollection(DistLargeGroups, DistLargeGroupLoader.Get());
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }
    }
}
