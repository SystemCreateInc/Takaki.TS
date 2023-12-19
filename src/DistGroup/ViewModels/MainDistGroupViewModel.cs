using DistGroup.Loader;
using DistGroup.Views;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;
using TakakiLib.Models;
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

        private ObservableCollection<Models.DistGroupInfo> _distGroups = new ObservableCollection<Models.DistGroupInfo>();
        public ObservableCollection<Models.DistGroupInfo> DistGroups
        {
            get => _distGroups;
            set => SetProperty(ref _distGroups, value);
        }

        private Models.DistGroupInfo? _currentDistGroup;
        public Models.DistGroupInfo? CurrentDistGroup
        {
            get => _currentDistGroup;
            set
            {
                SetProperty(ref _currentDistGroup, value);
                CanEdit = CurrentDistGroup != null && IsSelectedShain;
            }
        }

        private bool _canEdit = false;
        public bool CanEdit
        {
            get => _canEdit;
            set => SetProperty(ref _canEdit, value);
        }


        private bool _isSelectedShain = false;
        public bool IsSelectedShain
        {
            get => _isSelectedShain;
            set => SetProperty(ref _isSelectedShain, value);
        }

        private ShainInfo? _shainInfo = new ShainInfo();

        public MainDistGroupViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            SetShain();
            LoadDatas();

            Add = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistGroupViewModel:Add");
                if (ShowInputDialog(new Models.DistGroupInfo()))
                {
                    LoadDatas();
                }
            }).ObservesCanExecute(() => IsSelectedShain);

            Edit = new DelegateCommand(() =>
            {
                if (CurrentDistGroup == null)
                {
                    return;
                }

                Syslog.Debug("MainDistGroupViewModel:Edit");
                if (ShowInputDialog(CurrentDistGroup))
                {
                    LoadDatas();
                }
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

        private bool ShowInputDialog(Models.DistGroupInfo distGroup)
        {
            IDialogResult? result = null;

            _dialogService.ShowDialog(
                nameof(InputDistGroupDlg),
                new DialogParameters
                {
                    { "DistGroup", distGroup },
                    { "ShainInfo", _shainInfo },
                },
                r => result = r);

            return result?.Result == ButtonResult.OK;
        }

        private void SetShain()
        {
            _shainInfo = ShainLoader.Get();

            IsSelectedShain = _shainInfo is not null;
            Shain = $"{_shainInfo?.HenkoshaCode}  {_shainInfo?.HenkoshaName}";
        }
    }
}
