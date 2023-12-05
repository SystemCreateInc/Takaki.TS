using DbLib.Defs;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using SeatMapping.Models;
using System.Collections.ObjectModel;
using System.Windows;
using WindowLib.Utils;

namespace SeatMapping.ViewModels
{
    public class MainSeatMappingViewModel : BindableBase
    {
        public DelegateCommand Remove { get; }
        public DelegateCommand Release { get; }
        public DelegateCommand Exit { get; }

        private readonly IDialogService _dialogService;

        private int _blockIndex = -1;
        public int BlockIndex
        {
            get => _blockIndex;
            set
            {
                SetProperty(ref _blockIndex, value);
                if (BlockIndex == -1)
                {
                    return;
                }

                LoadDatas();
            }
        }

        private List<Combo> _blockCombo = new List<Combo>();
        public List<Combo> BlockCombo
        {
            get => _blockCombo;
            set => SetProperty(ref _blockCombo, value);
        }

        private ObservableCollection<Models.SeatMapping> _seatMappings = new ObservableCollection<Models.SeatMapping>();
        public ObservableCollection<Models.SeatMapping> SeatMappings
        {
            get => _seatMappings;
            set => SetProperty(ref _seatMappings, value);
        }

        private Models.SeatMapping? _currentSeatMapping;
        public Models.SeatMapping? CurrentSeatMapping
        {
            get => _currentSeatMapping;
            set
            {
                SetProperty(ref _currentSeatMapping, value);
                if (CurrentSeatMapping == null)
                {
                    return;
                }

                CanRemove = CurrentSeatMapping.RemoveType == RemoveType.Include;
                CanRelease = CurrentSeatMapping.RemoveType == RemoveType.Remove;
            }
        }

        private bool _canRemove = false;
        public bool CanRemove
        {
            get => _canRemove;
            set => SetProperty(ref _canRemove, value);
        }

        private bool _canRelease = false;
        public bool CanRelease
        {
            get => _canRelease;
            set => SetProperty(ref _canRelease, value);
        }

        public MainSeatMappingViewModel(IDialogService dialogService, IRegionManager regionManager)
        {
            _dialogService = dialogService;

            Remove = new DelegateCommand(() =>
            {
                Syslog.Debug("MainSeatMappingViewModel:Remove");
                // fixme:対象外ボタン押下
            }).ObservesCanExecute(() => CanRemove);

            Release = new DelegateCommand(() =>
            {
                Syslog.Debug("MainSeatMappingViewModel:Release");
                // fixme:解除ボタン押下
            }).ObservesCanExecute(() => CanRelease);

            Exit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainSeatMappingViewModel:Exit");
                Application.Current.MainWindow.Close();
            });

            BlockIndex = -1;
            BlockCombo = ComboCreator.Create();
            BlockIndex = 0;
        }

        private void LoadDatas()
        {
            try
            {
                CollectionViewHelper.SetCollection(SeatMappings, SeatMappingLoader.Get());
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }
    }
}
