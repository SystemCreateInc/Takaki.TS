using DistGroup.Models;
using DistGroup.Views;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;
using WindowLib.Utils;

namespace DistGroup.ViewModels
{
    public class MainDistGroupViewModel : BindableBase
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

        private ObservableCollection<Models.DistGroup> _distGroups = new ObservableCollection<Models.DistGroup>();
        public ObservableCollection<Models.DistGroup> DistGroups
        {
            get => _distGroups;
            set => SetProperty(ref _distGroups, value);
        }

        private Models.DistGroup? _currentDistGroup;
        public Models.DistGroup? CurrentDistGroup
        {
            get => _currentDistGroup;
            set
            {
                SetProperty(ref _currentDistGroup, value);
                CanEdit = CurrentDistGroup != null;
            }
        }

        private bool _canEdit = false;
        public bool CanEdit
        {
            get => _canEdit;
            set => SetProperty(ref _canEdit, value);
        }

        public MainDistGroupViewModel(IDialogService dialogService, IRegionManager regionManager)
        {
            _dialogService = dialogService;

            Add = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistGroupViewModel:Add");
                regionManager.RequestNavigate("ContentRegion", nameof(InputDistGroup), new NavigationParameters
                {
                    { "CurrentDistGroup", null },
                });
            });

            Edit = new DelegateCommand(() =>
            {
                if (CurrentDistGroup == null)
                {
                    return;
                }

                Syslog.Debug("MainDistGroupViewModel:Edit");
                regionManager.RequestNavigate("ContentRegion", nameof(InputDistGroup), new NavigationParameters
                {
                    { "CurrentDistGroup", CurrentDistGroup },
                });
            }).ObservesCanExecute(() => CanEdit);

            Exit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistGroupViewModel:Exit");
                Application.Current.MainWindow.Close();
            });

            LeftDoubleClick = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistGroupViewModel:LeftDoubleClick");
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
                CollectionViewHelper.SetCollection(DistGroups, DistGroupLoader.Get());
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }
    }
}
