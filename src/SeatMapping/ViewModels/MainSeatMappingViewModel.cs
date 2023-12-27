using DbLib.Defs;
using LogLib;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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

        private ObservableCollection<SeatMappingInfo> _seatMappings = new ObservableCollection<SeatMappingInfo>();
        public ObservableCollection<SeatMappingInfo> SeatMappings
        {
            get => _seatMappings;
            set => SetProperty(ref _seatMappings, value);
        }

        private SeatMappingInfo? _currentSeatMapping;
        public SeatMappingInfo? CurrentSeatMapping
        {
            get => _currentSeatMapping;
            set
            {
                SetProperty(ref _currentSeatMapping, value);

                CanRemove = CurrentSeatMapping?.RemoveType == RemoveType.Include;
                CanRelease = CurrentSeatMapping?.RemoveType == RemoveType.Remove;
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
            initialize();

            Remove = new DelegateCommand(() =>
            {
                Syslog.Debug("MainSeatMappingViewModel:Remove");
                if (Regist((short)RemoveType.Remove))
                {
                    LoadDatas();
                }
            }).ObservesCanExecute(() => CanRemove);

            Release = new DelegateCommand(() =>
            {
                Syslog.Debug("MainSeatMappingViewModel:Release");
                if (Regist((short)RemoveType.Include))
                {
                    LoadDatas();
                }
            }).ObservesCanExecute(() => CanRelease);

            Exit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainSeatMappingViewModel:Exit");
                Application.Current.MainWindow.Close();
            });
        }

        private bool Regist(short stRemove)
        {
            try
            {
                LocposEntityManager.Regist(BlockCombo[BlockIndex].Id, CurrentSeatMapping!.Tdunitaddrcode, stRemove);
                return true;
            }
            catch (Exception ex)
            {
                MessageDialog.Show(_dialogService, ex.Message, "エラー");
                return false;
            }
        }

        private void initialize()
        {
            // ブロック設定無し　終了
            if (!SetBlockCombo())
            {
                Application.Current.MainWindow.Close();
                return;
            }

            BlockCombo = ComboCreator.Create();
            LoadDatas();
        }

        private void LoadDatas()
        {
            try
            {
                var selectBlock = BlockCombo[BlockIndex];

                CollectionViewHelper.SetCollection(SeatMappings, SeatMappingLoader.Get(selectBlock.UnitType, selectBlock.Id));
                // 表示器側に存在しないアドレスをLocPosから削除
                LocposEntityManager.DeleteNotExistAddr(SeatMappings.Select(x => x.Tdunitaddrcode), selectBlock.Id);
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }

        private bool SetBlockCombo()
        {
            BlockCombo = ComboCreator.Create();
            if (!BlockCombo.Any())
            {
                MessageDialog.Show(_dialogService, $"ブロックが設定されていません\n終了します。", "エラー");
                return false;
            }

            BlockIndex = BlockCombo.First().Index;
            return true;
        }
    }
}
