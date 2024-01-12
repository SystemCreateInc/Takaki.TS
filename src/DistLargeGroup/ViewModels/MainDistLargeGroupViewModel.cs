using DistLargeGroup.Infranstructures;
using DistLargeGroup.Views;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;
using TakakiLib.Models;
using WindowLib.Utils;

namespace DistLargeGroup.ViewModels
{
    public class MainDistLargeGroupViewModel : BindableBase
    {
        public DelegateCommand AddCommand { get; }
        public DelegateCommand EditCommand { get; }
        public DelegateCommand ExitCommand { get; }
        public DelegateCommand LeftDoubleClick { get; }

        private readonly IDialogService _dialogService;

        private ShainInfo? _shain;
        public ShainInfo? Shain
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
                CanEdit = CurrentDistLargeGroup != null && IsSelectedShain;
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

        public MainDistLargeGroupViewModel(IDialogService dialogService, IRegionManager regionManager)
        {
            _dialogService = dialogService;

            AddCommand = new DelegateCommand(Add).ObservesCanExecute(() => IsSelectedShain);
            EditCommand = new DelegateCommand(Edit).ObservesCanExecute(() => CanEdit);
            ExitCommand = new DelegateCommand(Exit);
            LeftDoubleClick = new DelegateCommand(Edit).ObservesCanExecute(() => CanEdit);

            try
            {
                Shain = ShainLoader.Get();
                IsSelectedShain = Shain is not null;

                // 社員情報取得失敗で終了
                if (!IsSelectedShain)
                {
                    MessageDialog.Show(_dialogService, $"変更社員情報が選択されていません", "エラー");
                    Application.Current.MainWindow.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }

            LoadDatas();
        }

        private void Exit()
        {
            Syslog.Debug("MainDistLargeGroupViewModel:Exit");
            Application.Current.MainWindow.Close();
        }

        private void Edit()
        {
            if (CurrentDistLargeGroup == null)
            {
                return;
            }

            Syslog.Debug("MainDistLargeGroupViewModel:Edit");
            if (ShowInputDialog(CurrentDistLargeGroup))
            {
                LoadDatas();
            }
        }

        private void Add()
        {
            Syslog.Debug("MainDistLargeGroupViewModel:Add");
            if (ShowInputDialog(null))
            {
                LoadDatas();
            }
        }

        private void LoadDatas()
        {
            try
            {
                CollectionViewHelper.SetCollection(DistLargeGroups, LargeGroupQueryService.FindAll());
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }

        private bool ShowInputDialog(Models.DistLargeGroup? distLargeGroup)
        {
            if (Shain is null)
            {
                return false;
            }

            IDialogResult? result = null;

            _dialogService.ShowDialog(
                nameof(InputDistLargeGroupDlg),
                new DialogParameters
                {
                    { 
                        "DistLargeGroup", distLargeGroup 
                    },
                    {
                        "Shain", Shain
                    }
                },
                r => result = r);

            return result?.Result == ButtonResult.OK;
        }
    }
}
