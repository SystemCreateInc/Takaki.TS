using DbLib.Extensions;
using Mapping.Models;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;
using WindowLib.Utils;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using Mapping.Views;
using ControlzEx.Standard;
using Prism.Regions;
using ReferenceLogLib.Models;
using Mapping.Defs;
using ImTools;
using ExportLib.Infranstructures;
using DbLib.Defs;
using ExportLib.Models;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Diagnostics.Eventing.Reader;
using Dapper.FastCrud;
using ZXing;
using MS.WindowsAPICodePack.Internal;

namespace Mapping.ViewModels
{
    public class MainMappingViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand Run { get; }
        public DelegateCommand MapInfo { get; }
        public DelegateCommand Decision { get; }
        public DelegateCommand RackMax { get; }
        public DelegateCommand PathCommand { get; }
        public DelegateCommand Exit { get; }
        public DelegateCommand DistClear { get; }

        private string _dtDelivery = string.Empty;
        public string DtDelivery
        {
            get => _dtDelivery;
            set => SetProperty(ref _dtDelivery, value);
        }

        public string DispDtDelivery => DtDelivery.GetDate();

        private int _dispRackAllocMax = 10;
        public int DispRackAllocMax
        {
            get => _dispRackAllocMax;
            set => SetProperty(ref _dispRackAllocMax, value);
        }

        private ObservableCollection<DistGroupInfo> _distgroupinfos = new ObservableCollection<DistGroupInfo>();
        public ObservableCollection<DistGroupInfo> DistGroupInfos
        {
            get => _distgroupinfos;
            set => SetProperty(ref _distgroupinfos, value);
        }

        private DistGroupInfo? _currentDistGroupInfo;

        public DistGroupInfo? CurrentDistGroupInfo
        {
            get => _currentDistGroupInfo;
            set
            {
                SetProperty(ref _currentDistGroupInfo, value);
            }
        }
        private DistGroupInfo? _selectedDistGroupInfo;

        public DistGroupInfo? SelectedDistGroupInfo
        {
            get => _selectedDistGroupInfo;
            set
            {
                SetProperty(ref _selectedDistGroupInfo, value);
                RefreshButtonOverlay();
            }
        }
        private bool _canDistInfo = false;
        public bool CanDistInfo
        {
            get => _canDistInfo;
            set => SetProperty(ref _canDistInfo, value);
        }

        private bool _canShifted = true;
        public bool CanShifted
        {
            get => _canShifted;
            set => SetProperty(ref _canShifted, value);
        }

        private bool _canMapping = true;
        public bool CanMapping
        {
            get => _canMapping;
            set => SetProperty(ref _canMapping, value);
        }
        private bool _canDecision = false;
        public bool CanDecision
        {
            get => _canDecision;
            set => SetProperty(ref _canDecision, value);
        }

        MappingManager Mapping = new MappingManager();

        private readonly DispatcherTimer _idleTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle, System.Windows.Application.Current.Dispatcher);

        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;


        public MainMappingViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;


            Run = new DelegateCommand(() =>
            {
                Syslog.Debug("MainMappingViewModel:Run");

                // 座席マッピング作成開始
                try
                {
                    // チェック
                    if (Check() == false)
                        return;

                    if (MessageDialog.Show(_dialogService, $"仕分グループ:{SelectedDistGroupInfo?.CdDistGroup} {SelectedDistGroupInfo?.NmDistGroup}\n\n座席マッピングのシミュレーションを開始します。よろしいですか？", "確認", ButtonMask.OK | ButtonMask.Cancel) != ButtonResult.OK)
                        return;

                    using (var busy = new WaitCursor())
                    {
                        // 画面表示内容を更新
                        foreach (var p in DistGroupInfos)
                        {
                            if (p.Select == true)
                            {
                                // Mapping開始
                                Mapping.Run(p.CdDistGroup);

                                p.ShopCnt = Mapping.GetShopCnt(p.CdDistGroup);
                                p.LocCnt = Mapping.GetLocCnt(p.CdDistGroup);
                                p.OverShopCnt = Mapping.GetOverCnt(p.CdDistGroup);

                                if (p.ShopCnt != 0)
                                {
                                    p.MStatus = MStatus.Run;
                                    p.MappingStatus = p.OverShopCnt == 0 ? (int)DbLib.Defs.Status.Completed : (int)DbLib.Defs.Status.Inprog;
                                }
                                break;
                            }
                        }

                        RefreshButtonOverlay();
                    }
                    MessageDialog.Show(_dialogService, $"仕分グループ:{SelectedDistGroupInfo?.CdDistGroup} {SelectedDistGroupInfo?.NmDistGroup}\n\n座席マッピングのシミュレーションか完了しました。", "確認");
                }
                catch (Exception e)
                {
                    Syslog.Error($"MainMappingViewModel:Run:{e.Message}");
                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                }
            }).ObservesCanExecute(() => CanMapping);

            MapInfo = new DelegateCommand(() =>
            {
                Syslog.Debug("MainMappingViewModel:MapInfo");

                if (SelectedDistGroupInfo == null)
                    return;

                _regionManager.RequestNavigate("ContentRegion", nameof(Views.LocInfo), new NavigationParameters
                {
                    { "currentdistinfo", SelectedDistGroupInfo },
                    { "Mapping", Mapping              },
                });

            }).ObservesCanExecute(() => CanDistInfo);

            Decision = new DelegateCommand(() =>
            {
                Syslog.Debug("MainMappingViewModel:Decision");

                try
                {
                    if (SelectedDistGroupInfo != null)
                    {
                        if (SelectedDistGroupInfo.OverShopCnt != 0)
                        {
                            _regionManager.RequestNavigate("ContentRegion", nameof(Views.OverTokuisaki), new NavigationParameters
                            {
                                { "currentdistinfo", SelectedDistGroupInfo },
                                { "Mapping", Mapping              },
                            });
                        }
                        else
                        {
                            // 保存
                            Save(false);
                        }
                    }
                }
                catch (Exception e)
                {
                    Syslog.Error($"MainMappingViewModel:Run:{e.Message}");
                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                }
            }).ObservesCanExecute(() => CanDecision);

            RackMax = new DelegateCommand(() =>
            {
                Syslog.Debug("MainMappingViewModel:RackMax");

                // 棚割ＭＡＸ入力
                _dialogService.ShowDialog(nameof(RackAllocMaxDialog),
                    new DialogParameters
                    {
                        { "RackAllocMax", DispRackAllocMax },
                    },
                    rc =>
                    {
                        if (rc.Result != ButtonResult.OK)
                        {
                            return;
                        }

                        DispRackAllocMax = rc.Parameters.GetValue<int>("RackAllocMax");

                        try
                        {
                            MappingLoader.SetRackAllocMax(DispRackAllocMax);
                        }
                        catch (Exception e)
                        {
                            Syslog.Error($"MainMappingViewModel:Clear:{e.Message}");
                            MessageDialog.Show(_dialogService, e.Message, "エラー");
                        }
                    });

            });

            PathCommand = new DelegateCommand(() =>
            {
                using (var repo = new ExportRepository())
                {
                    if (repo.GetInterfaceFile(DataType.PickResult) is InterfaceFile interfaceFile)
                    {
                        if (GetSelectedFolder(interfaceFile.FileName) is string path)
                        {
                            try
                            {
                                var fileName = Path.GetFileName(interfaceFile.FileName);

                                var newInterfaceFile = interfaceFile with { FileName = Path.Combine(path, fileName) };
                                repo.Save(newInterfaceFile);
                                repo.Commit();
                            }
                            catch (Exception ex)
                            {

                                MessageDialog.Show(_dialogService, ex.Message, "エラー");
                            }
                        }
                    }
                }
            });


            Exit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainMappingViewModel:Exit");
                Application.Current.MainWindow.Close();
            });

            DistClear = new DelegateCommand(() =>
            {
                Syslog.Debug("MainMappingViewModel:Clear");
                try
                {
                    if (SelectedDistGroupInfo == null)
                    {
                        MessageDialog.Show(_dialogService, "初期化する仕分けバッチを選択して下さい", "エラー");
                        return;
                    }

                    if(SelectedDistGroupInfo.MStatus!=MStatus.Ready)
                    {
                        if (MessageDialog.Show(_dialogService, "決定済みの仕分けバッチのみ初期化できます。", "確認", ButtonMask.OK | ButtonMask.Cancel) != ButtonResult.OK)
                            return;
                    }

                    List<string> seldistgroups = DistGroupInfos.Where(x => x.Select == true)
                        .Select(x => x.CdDistGroup)
                        .OrderBy(x => x)
                        .ToList();

                    if (MessageDialog.Show(_dialogService, "選択した仕分グループを初期化します。よろしいですか？", "確認", ButtonMask.OK | ButtonMask.Cancel) != ButtonResult.OK)
                        return;

                    Mapping.ClearDatas(DtDelivery, seldistgroups);

                    LoadDatas(SelectedDistGroupInfo.CdDistGroup);

                    MessageDialog.Show(_dialogService, "初期化しました", "確認");

                }
                catch (Exception e)
                {
                    Syslog.Error($"MainMappingViewModel:Clear:{e.Message}");
                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                }

            }).ObservesCanExecute(() => CanShifted);

            _idleTimer.Tick += (s, e) => CheckShifted();
            _idleTimer.Start();

            // 閉じるボタン応答
            System.Windows.Application.Current.MainWindow.Closing += Closing;

            // 納品日選択
            _dialogService.ShowDialog(nameof(DeliveryDateDialog),
                rc =>
                {
                    if (rc.Result != ButtonResult.OK)
                    {
                        Application.Current.MainWindow.Close();
                        return;
                    }

                    DtDelivery = rc.Parameters.GetValue<DateTime>("Date").ToString("yyyyMMdd");
                    DispRackAllocMax = MappingLoader.GetRackAllocMax();
                    LoadDatas();
                });

        }
        private void Closing(object? sender, CancelEventArgs e)
        {
            if (DistGroupInfos.Where(x => x.MStatus == MStatus.Run).Count() != 0)
            {
                if (MessageDialog.Show(_dialogService, "決定していない仕分バッチがありますが、終了してよろしいですか？", "確認", ButtonMask.OK | ButtonMask.Cancel) != ButtonResult.OK)
                { 
                    e.Cancel = true;
                    return;
                }
            }


            _idleTimer.Tick -= (s, e) => CheckShifted();
            _idleTimer.Stop();
        }

        private void CheckShifted()
        {
            bool shifted = (Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) != 0
                || (Keyboard.GetKeyStates(Key.RightShift) & KeyStates.Down) != 0;

            CanShifted = shifted;
        }

        private void LoadDatas(string cdDistGroup = "")
        {
            try
            {
                if (cdDistGroup=="")
                {
                    var seldistgroup = DistGroupInfos.Where(x=>x.Select==true).Select(x=>x.CdDistGroup).FirstOrDefault();

                    foreach (var p in DistGroupInfos)
                    {
                        p.PropertyChanged -= DisplayPropatyChanged;
                    }

                    CollectionViewHelper.SetCollection(DistGroupInfos, DistGroupInfoLoader.Get(DtDelivery));

                    foreach (var p in DistGroupInfos)
                    {
                        p.PropertyChanged += DisplayPropatyChanged;
                    }

                    List<string> seldistgroups = DistGroupInfos
                        .Select(x => x.CdDistGroup)
                        .OrderBy(x => x)
                        .ToList();

                    Mapping.RackAllocMax = DispRackAllocMax;
                    Mapping.LoadDatas(DtDelivery, seldistgroups);
                }
                else
                {
                    ObservableCollection<DistGroupInfo> tmps = new ObservableCollection<DistGroupInfo>();
                    CollectionViewHelper.SetCollection(tmps, DistGroupInfoLoader.Get(DtDelivery));

                    if (SelectedDistGroupInfo != null)
                    {
                        var p = tmps.FindFirst(x => x.CdDistGroup == SelectedDistGroupInfo.CdDistGroup);

                        // 該当配分グループのみ置き換え
                        if (SelectedDistGroupInfo != null && p != null)
                        {
                            SelectedDistGroupInfo.OverShopCnt = p.OverShopCnt;
                            SelectedDistGroupInfo.ShopCnt = p.ShopCnt;
                            SelectedDistGroupInfo.MStatus = p.ShopCnt == 0 ? Defs.MStatus.Ready : Defs.MStatus.Decision;
                            SelectedDistGroupInfo.LocCnt = p.LocCnt;
                            SelectedDistGroupInfo.LStatus = p.LStatus;
                            SelectedDistGroupInfo.DStatus = p.DStatus;
                        }
                    }
                }

                SelectedDistGroupInfo = DistGroupInfos.FindFirst(x => x.Select == true);
            }
            catch (Exception e)
            {
                Syslog.Error($"MainMappingViewModel:LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }
        private bool Check()
        {
            try
            {
                if (SelectedDistGroupInfo == null)
                {
                    MessageDialog.Show(_dialogService, "座席マッピングする仕分グループを選択して下さい", "エラー");
                    return false;
                }

                var r = DistGroupInfos.Where(x => x.Select == true && x.MStatus == MStatus.Decision);
                if (r.Count()!=0)
                {
                    MessageDialog.Show(_dialogService, "既に決定済みの仕分グループが選択されています", "エラー");
                    return false;
                }
            }
            catch (Exception e)
            {
                Syslog.Error($"MainMappingViewModel:Check:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }

            return true;
        }

        private void Save(bool bCancel)
        {
            try
            {
                if (SelectedDistGroupInfo == null)
                    return;

                bool bEndMessage = false;
                if (Mapping.GetOverCnt(SelectedDistGroupInfo.CdDistGroup) == 0)
                {
                    bEndMessage = true;
                }

                if (bCancel != true)
                {
                    using (var busy = new WaitCursor())
                    {
                        Mapping.Saves(SelectedDistGroupInfo.CdDistGroup);

                        Mapping.Export(SelectedDistGroupInfo.CdDistGroup);

                        Mapping.ExecHulft(SelectedDistGroupInfo.CdDistGroup);
                    }
                }

                LoadDatas(SelectedDistGroupInfo.CdDistGroup);

                if (bCancel != true)
                {
                    if (bEndMessage)
                        MessageDialog.Show(_dialogService, "座席マッピングが正常に終了しました。", "確認");
                }
                else
                {
                    MessageDialog.Show(_dialogService, "座席実績をキャンセルしました。", "確認");
                }
            }
            catch (Exception e)
            {
                Syslog.Error($"MainMappingViewModel:Check:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Syslog.Info($"MainMappingViewModel:OnNavigatedFrom");
            return;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Syslog.Info($"MainMappingViewModel:OnNavigatedTo");

            foreach (var distgroup in Mapping.distgroups)
            {
                if (distgroup.IsSave == true || distgroup.IsCancel == true)
                {
                    Save(distgroup.IsCancel);
                    distgroup.IsSave = false;
                    distgroup.IsCancel = false;
                }
            }
        }
        private string? GetSelectedFolder(string fileName)
        {
            using (var cofd = new CommonOpenFileDialog()
            {
                Title = "実績データ送信先フォルダー選択",
                RestoreDirectory = true,
                IsFolderPicker = true,
                DefaultDirectory = fileName,
            })
            {
                return cofd.ShowDialog() == CommonFileDialogResult.Ok ? cofd.FileName : null;
            }
        }
        private void RefreshButtonOverlay()
        {
            if (SelectedDistGroupInfo == null)
            {
                CanMapping=false;
                CanDecision = false;
                CanDistInfo = false;
            }
            else
            {
                CanMapping = SelectedDistGroupInfo.MStatus == MStatus.Ready ? true : false;
                CanDecision = SelectedDistGroupInfo.MStatus == MStatus.Run ? true : false;
                CanDistInfo = SelectedDistGroupInfo.MStatus != MStatus.Ready ? true : false;
            }
        }

        // 1行のみチェック可能にする
        private void DisplayPropatyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender == null)
                return;

            DistGroupInfo currnt = (DistGroupInfo)sender;
            switch (e.PropertyName)
            {
                case "Select":
                    if (currnt.Select == true)
                    {
                        // ほかのチェックを外す
                        foreach (var p in DistGroupInfos)
                        {
                            if (p != currnt)
                            {
                                if (currnt.Select == true)
                                    p.Select = false;
                            }
                        }
                    }

                    SelectedDistGroupInfo = DistGroupInfos.FindFirst(x => x.Select == true);
                    break;
            }
        }
    }
}
