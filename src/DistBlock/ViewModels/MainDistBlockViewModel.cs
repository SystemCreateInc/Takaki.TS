using DistBlock.Loader;
using DistBlock.Models;
using DistBlock.Views;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;
using TakakiLib.Models;
using WindowLib.Utils;

namespace DistBlock.ViewModels
{
    public class MainDistBlockViewModel : BindableBase
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

        private ObservableCollection<DistBlockInfo> _distBlocks = new ObservableCollection<DistBlockInfo>();
        public ObservableCollection<DistBlockInfo> DistBlocks
        {
            get => _distBlocks;
            set => SetProperty(ref _distBlocks, value);
        }

        private DistBlockInfo? _currentDistBlock;
        public DistBlockInfo? CurrentDistBlock
        {
            get => _currentDistBlock;
            set
            {
                SetProperty(ref _currentDistBlock, value);
                CanEdit = CurrentDistBlock is not null && SelectedShain;
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

        public MainDistBlockViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            LoadDatas();
            SetShain();

            Add = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistBlockViewModel:Add");
                if (ShowInputDialog(new DistBlockInfo()))
                {
                    LoadDatas();
                }
            }).ObservesCanExecute(() => SelectedShain);

            Edit = new DelegateCommand(() =>
            {
                if (CurrentDistBlock == null)
                {
                    return;
                }

                Syslog.Debug("MainDistBlockViewModel:Edit");
                if (ShowInputDialog(CurrentDistBlock))
                {
                    LoadDatas();
                }
            }).ObservesCanExecute(() => CanEdit);

            Exit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistBlockViewModel:Exit");
                Application.Current.MainWindow.Close();
            });

            LeftDoubleClick = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistBlockViewModel:LeftDoubleClick");
                Edit.Execute();
            }).ObservesCanExecute(() => CanEdit);
        }

        private void LoadDatas()
        {
            try
            {
                CollectionViewHelper.SetCollection(DistBlocks, DistBlockLoader.Get());
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }

        private bool ShowInputDialog(DistBlockInfo distBlock)
        {
            IDialogResult? result = null;

            _dialogService.ShowDialog(
                nameof(InputDistBlockDlg),
                new DialogParameters
                {
                    { "DistBlock", distBlock },
                    { "ShainInfo", _shainInfo },
                },
                r => result = r);

            return result?.Result == ButtonResult.OK;
        }

        private void SetShain()
        {
            _shainInfo = ShainLoader.Get();
            SelectedShain = _shainInfo is not null;
            Shain = $"{_shainInfo?.HenkoshaCode}  {_shainInfo?.HenkoshaName}";
        }
    }
}
