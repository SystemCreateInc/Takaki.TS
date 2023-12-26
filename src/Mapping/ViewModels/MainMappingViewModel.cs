﻿using DbLib.Extensions;
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
                CanDistInfo = value == null ? false : true;
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
                    using (var busy = new WaitCursor())
                    {
                        // チェック
                        if (Check() == false)
                            return;

                        if (MessageDialog.Show(_dialogService, "座席マッピングのシミュレーションを開始します。よろしいですか？", "確認", ButtonMask.OK | ButtonMask.Cancel) != ButtonResult.OK)
                            return;

                        List<string> seldistgroups = DistGroupInfos.Where(x => x.Select == true)
                            .Select(x => x.CdDistGroup)
                            .OrderBy(x => x)
                            .ToList();

                        Mapping.RackAllocMax = DispRackAllocMax;
                        Mapping.LoadDatas(DtDelivery, seldistgroups);

                        Mapping.Run();

                        // 画面表示内容を更新
                        foreach (var p in DistGroupInfos)
                        {
                            if (p.Select == true)
                            {
                                p.ShopCnt = Mapping.GetShopCnt(p.CdDistGroup);
                                p.LocCnt = Mapping.GetLocCnt(p.CdDistGroup);
                                p.OverShopCnt = Mapping.GetOverCnt(p.CdDistGroup);

                                if (p.ShopCnt != 0)
                                {
                                    p.MStatus = MStatus.Run;
                                    p.MappingStatus = p.OverShopCnt == 0 ? (int)DbLib.Defs.Status.Completed : (int)DbLib.Defs.Status.Inprog;
                                }
                            }
                            p.IsSelectEnabled = false;
                        }

                        CanMapping = false;
                        CanDecision = true;
                    }
                    MessageDialog.Show(_dialogService, "座席マッピングのシミュレーションか完了しました。", "確認");
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

                if (CurrentDistGroupInfo == null)
                    return;

                if (CurrentDistGroupInfo.MappingStatus == (int)DbLib.Defs.Status.Ready)
                {
                    if (CurrentDistGroupInfo.ShopCnt!=0)
                        MessageDialog.Show(_dialogService, "座席マッピング済みの仕分けグループです。", "確認");
                    else
                        MessageDialog.Show(_dialogService, "座席マッピングを実行していない仕分けグループです。", "確認");
                    return;
                }

                _regionManager.RequestNavigate("ContentRegion", nameof(Views.LocInfo), new NavigationParameters
                {
                    { "currentdistinfo", CurrentDistGroupInfo },
                    { "Mapping", Mapping              },
                });

            }).ObservesCanExecute(() => CanDistInfo);

            Decision = new DelegateCommand(() =>
            {
                Syslog.Debug("MainMappingViewModel:Decision");

                try
                {
                    var seldistgroups = DistGroupInfos.FindFirst(x => x.Select == true);

                    if (seldistgroups!=null)
                    {
                        if (seldistgroups.OverShopCnt != 0)
                        {
                            _regionManager.RequestNavigate("ContentRegion", nameof(Views.OverTokuisaki), new NavigationParameters
                            {
                                { "currentdistinfo", seldistgroups },
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
                            Application.Current.MainWindow.Close();
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
                    List<string> seldistgroups = DistGroupInfos.Where(x => x.Select == true)
                        .Select(x => x.CdDistGroup)
                        .OrderBy(x => x)
                        .ToList();

                    if (seldistgroups.Count == 0)
                    {
                        MessageDialog.Show(_dialogService, "初期化する仕分けバッチを選択して下さい", "確認");
                        return;
                    }

                    if (MessageDialog.Show(_dialogService, "選択した仕分グループを初期化します。よろしいですか？", "確認", ButtonMask.OK | ButtonMask.Cancel) != ButtonResult.OK)
                        return;

                    Mapping.ClearDatas(DtDelivery, seldistgroups);

                    LoadDatas();

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
            _idleTimer.Tick -= (s, e) => CheckShifted();
            _idleTimer.Stop();
        }

        private void CheckShifted()
        {
            bool shifted = (Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) != 0
                || (Keyboard.GetKeyStates(Key.RightShift) & KeyStates.Down) != 0;

            CanShifted = shifted;
        }

        private void LoadDatas()
        {
            try
            {
                foreach (var p in DistGroupInfos)
                {
                    p.PropertyChanged -= DisplayPropatyChanged;
                }

                CollectionViewHelper.SetCollection(DistGroupInfos, DistGroupInfoLoader.Get(DtDelivery));

                foreach (var p in DistGroupInfos)
                {
                    p.PropertyChanged += DisplayPropatyChanged;
                }
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
                if (DistGroupInfos.Where(x => x.Select == true) == null)
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
                bool bEndMessage = false;
                foreach (var p in DistGroupInfos)
                {
                    if (p.Select == true)
                    {
                        if (Mapping.GetOverCnt(p.CdDistGroup) == 0)
                        {
                            bEndMessage = true;
                        }
                    }
                }

                if (bCancel != true)
                {
                    using (var busy = new WaitCursor())
                    {
                        Mapping.Saves();

                        Mapping.Export();
                    }
                }

                // クリア
                Mapping = new MappingManager();

                LoadDatas();

                CanMapping = true;
                CanDecision = false;

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

            if (Mapping.IsSave==true || Mapping.IsCancel == true)
            {
                Save(Mapping.IsCancel);
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
                    break;
            }
        }
    }
}
