using DistBlock.Models;
using DistBlock.Views;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;
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

        private ObservableCollection<Models.DistBlock> _distBlocks = new ObservableCollection<Models.DistBlock>();
        public ObservableCollection<Models.DistBlock> DistBlocks
        {
            get => _distBlocks;
            set => SetProperty(ref _distBlocks, value);
        }

        private Models.DistBlock? _currentDistBlock;
        public Models.DistBlock? CurrentDistBlock
        {
            get => _currentDistBlock;
            set
            {
                SetProperty(ref _currentDistBlock, value);
                CanEdit = CurrentDistBlock != null;
            }
        }

        private bool _canEdit = false;
        public bool CanEdit
        {
            get => _canEdit;
            set => SetProperty(ref _canEdit, value);
        }

        public MainDistBlockViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Add = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistBlockViewModel:Add");
                if (ShowInputDialog(new Models.DistBlock()))
                {
                    LoadDatas();
                }
            });

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

            // fixme:社員コード + 社員名称
            Shain = "0000033550" + "　" + "小田　賢行";
            LoadDatas();
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

        private bool ShowInputDialog(Models.DistBlock distBlock)
        {
            IDialogResult? result = null;

            _dialogService.ShowDialog(
                nameof(InputDistBlockDlg),
                new DialogParameters
                {
                    { "DistBlock", distBlock },
                },
                r => result = r);

            return result?.Result == ButtonResult.OK;
        }
    }
}
