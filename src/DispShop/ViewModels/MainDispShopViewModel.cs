using DispShop.Models;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using WindowLib.Utils;
using TdDpsLib.Defs;
using TdDpsLib.Models;
using System.Collections.Generic;
using Prism.Events;
using Prism.Regions;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Threading;
using WindowLib.ViewModels;
using System.Windows.Markup.Localizer;
using System.Printing;
using PrintPreviewLib;
using System.Windows.Documents;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using SelDistGroupLib.Services.Auth;
using System.Windows.Shapes;
using SelDistGroupLib.Models;
using System.Windows.Media.Media3D;
using System.Runtime.Intrinsics.X86;
using DbLib.Defs;

namespace DispShop.ViewModels
{
    public class MainDispShopViewModel : BindableBase
    {
        public DelegateCommand? CourseLightOn { get; }
        public DelegateCommand? TokuisakiLightOn { get; }
        public DelegateCommand? HakoLightOn { get; }
        public DelegateCommand? PsLightOn { get; }
        public DelegateCommand? LightOff { get; }
        public DelegateCommand? Exit { get; }

        private ObservableCollection<Dist> _distItems = new ObservableCollection<Dist>();
        public ObservableCollection<Dist> DistItems
        {
            get => _distItems;
            set
            {
                SetProperty(ref _distItems, value);
            }
        }

        private DistGroup _distgroup = new DistGroup();
        public DistGroup DistGroup
        {
            get => _distgroup;
            set => SetProperty(ref _distgroup, value);
        }


        private string _dt_delivery = string.Empty;
        public string DtDelivery
        {
            get => _dt_delivery;
            set => SetProperty(ref _dt_delivery, value);
        }

        private string _cd_dist_group = string.Empty;
        public string CdDistGroup
        {
            get => _cd_dist_group;
            set => SetProperty(ref _cd_dist_group, value);
        }

        private string _nm_dist_group = string.Empty;
        public string NmDistGroup
        {
            get => _nm_dist_group;
            set => SetProperty(ref _nm_dist_group, value);
        }

        private bool _canLight = true;
        public bool CanLight
        {
            get => _canLight;
            set => SetProperty(ref _canLight, value);
        }

        private bool _canList = false;
        public bool CanList
        {
            get => _canList;
            set => SetProperty(ref _canList, value);
        }
        private int _lightType = 0;
        public int LightType
        {
            get => _lightType;
            set => SetProperty(ref _lightType, value);
        }

        private List<TdShowPortData> _tdunitports = new List<TdShowPortData>();

        public List<TdShowPortData> TdPorts
        {
            get => _tdunitports;
            set => SetProperty(ref _tdunitports, value);
        }

        private List<TdShowAddrData> _diaplayaddrs = new List<TdShowAddrData>();

        public List<TdShowAddrData> DisplayAddrs
        {
            get => _diaplayaddrs;
            set => SetProperty(ref _diaplayaddrs, value);
        }

        private ObservableCollection<TdShowAddrData> _diaplayerrs = new ObservableCollection<TdShowAddrData>();
        public bool bTdConnectionError { get; set; } = false;

        public TdDpsManager TdDps;
        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private System.Threading.Mutex _mutex;

        public MainDispShopViewModel(IRegionManager regionManager, IDialogService dialogService, IEventAggregator eventAggregator, TdDpsManager tddps)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;
            TdDps = tddps;

            _mutex = new System.Threading.Mutex(false, "MainDispShopViewModel");

            // 二重起動防止
            if (_mutex.WaitOne(0, false) == false)
            {
                Application.Current.MainWindow.Close();
                return;
            }

            if(!SelectDistGroup())
            {
                Application.Current.MainWindow.Close();
                return;
            }

            LoadDatas();

            CourseLightOn = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDispShopViewModel:CourseLightOn");
                LightOn(0);
            }).ObservesCanExecute(() => CanLight);

            TokuisakiLightOn = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDispShopViewModel:TokuisakiLightOn");
                LightOn(1);
            }).ObservesCanExecute(() => CanLight);

            HakoLightOn = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDispShopViewModel:HakoLightOn");
                LightOn(2);
            }).ObservesCanExecute(() => CanLight);

            PsLightOn = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDispShopViewModel:PsLightOn");
                LightOn(3);
            }).ObservesCanExecute(() => CanLight);

            LightOff = new DelegateCommand(() =>
            {
                AllLightOff();

            }, () => !CanLight).ObservesProperty(() => CanLight);

            Exit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDispShopViewModel:Exit");
                if (MessageDialog.Show(_dialogService, "店舗表示処理を終了します。よろしいですか？", "確認", ButtonMask.OK | ButtonMask.Cancel) == ButtonResult.OK)
                {
                    AllLightOff();

                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => Application.Current.MainWindow.Close()));
                }
            });


            try
            {
                TdDps.Init();
                TdDps.Open();

                // 表示器ログイベント登録
                TdDps.SetTdLogEvent(
                    (stno, log) =>
                    {
                        // コネクションエラーは点灯を止める
                        if (log.IndexOf("Connection Error") > 0)
                        {
                            bTdConnectionError = true;
                            var tdunit = TdDps.TdUnits.Find(unit => unit?.TdPort?.Stno == stno);
                            tdunit?.QueCancel();

                        }
                        if (bTdConnectionError)
                        {
                            var buttons = new ButtonResult[] { ButtonResult.OK };
                            MessageDialog.ShowAsync(_dialogService, "表示器コントローラーとの接続が切断されました", "エラー", buttons);
                        }
                    });

                // 表示器ボタン押下イベント登録
                TdDps.SetTdButtonPushEvent(
                    (stno, group, addr, color) =>
                    {
                        try
                        {
                            //反応なしとする
//                            TdUnitManager.TdUnitRcv(TdDps, stno, group, addr, color);
                        }
                        catch (Exception ex)
                        {
                            Syslog.Error(ex.ToString());
                            var message = $"表示器押下持にエラーが発生しました。\n{ex.Message}";
                            var buttons = new ButtonResult[] { ButtonResult.OK };
                            MessageDialog.ShowAsync(_dialogService, message, "エラー", buttons);
                            return;
                        }
                        return;
                    });

                // 表示器レスポンスイベント登録
                TdDps.SetTdResponseEvent(
                    (stno, group, addr, text) =>
                    {
                        try
                        {
                            TdUnitManager.TdUnitResponse(TdDps, stno, group, addr, text);
                        }
                        catch (Exception e)
                        {
                            var buttons = new ButtonResult[] { ButtonResult.OK };
                            MessageDialog.ShowAsync(_dialogService, e.Message, "エラー", buttons);
                        }
                        return;
                    });

                // ポートレスポンスイベント登録（ポートの接続状態変更時）
                TdDps.SetTdPortEvent(
                    (stno, status, text) =>
                    {
                        var port = TdPorts.Where(x => x.Stno == stno).FirstOrDefault();
                        if (port != null)
                        {
                            port.PortStatus = status;
                            port.SetPortStatusName(port.PortStatus);
                        }
                        return;
                    });
            }
            catch (Exception e)
            {
                Syslog.Info($"MainPickingViewModel::{e.ToString()}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
            try
            {
                if (TdDps.TdPorts != null)
                {
                    TdPorts = TdDps.TdPorts
                        .ConvertAll(new Converter<TdPortData, TdShowPortData>(TdPort2Showport));
                }

                if (TdDps.TdAddrs != null)
                {
                    DisplayAddrs = TdDps.TdAddrs
                        .ConvertAll(new Converter<TdAddrData, TdShowAddrData>(Tdaddr2Showaddr));
                }

                // イベント設定
                foreach (var unit in TdDps.TdUnits)
                {
                    unit.TdCmdQue.ItemDequeued += QueDequeued;
                }

                for (int i = 0; i < DisplayAddrs.Count; i++)
                {
                    DisplayAddrs[i].PropertyChanged += DisplayPropatyChanged;
                }

                for (int i = 0; i < TdPorts.Count; i++)
                {
                    TdPorts[i].PropertyChanged += DisplayPropatyChanged;
                }
            }
            catch (Exception e)
            {
                Syslog.Info($"MainPickingViewModel::{e.ToString()}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
            // 閉じるボタン応答
            Application.Current.MainWindow.Closing += Closing;
        }

        // コンバート
        public static TdShowAddrData Tdaddr2Showaddr(TdAddrData p)
        {
            TdShowAddrData data = new TdShowAddrData(p);
            return data;
        }

        public static TdShowPortData TdPort2Showport(TdPortData p)
        {
            TdShowPortData data = new TdShowPortData(p);
            return data;
        }


        // 表示器送信完了
        public void QueDequeued(object? sender, ItemEventArgs<QueCommand> e)
        {
            // 表示用表示器アドレスの状態変更
            QueCommand? que = e.Item;

            if (que != null)
            {
                TdShowAddrData? addr = DisplayAddrs.Find(x => x.TddGroup == que.Addrdata.TddGroup && x.TddAddr == que.Addrdata.TddAddr && x.Stno == que.Addrdata.Stno);

                if (addr != null)
                {

                    addr.GetLedButton(que.Color)?.Set(que.On, que.Blink, que.Text);

                    string text = que.On == true ? que.Blink ? "BLK" : "LIT" : "OFF";

                    addr.SetButton(que.Color, text);
                }
            }
        }

        private void Closing(object? sender, CancelEventArgs e)
        {
            TdDps.Tarm();
        }

        /// <summary>
        /// 表示器状態表示更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayPropatyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender == null)
                return;

            switch (e.PropertyName)
            {
                case "PortStatus":
                    {
                        TdShowPortData p = (TdShowPortData)sender;
                        p.SetPortStatusName(p.PortStatus);
                    }

                    break;
                case "BtnStatus":
                    break;

                case "DStatus":
                    {
                        TdShowAddrData p = (TdShowAddrData)sender;
                        p.SetDStatusName(p.DStatus);
                    }

                    break;

                case "BatteryInfo":
                    {
                        TdShowAddrData p = (TdShowAddrData)sender;
                        p.SetBatteryInfoName(p.BatteryInfo);
                    }

                    break;
            }
        }
        private void DpsLightOn(CancellationTokenSource ct)
        {
            bool bOn = true;
            foreach (var p in DistItems)
            {
                // 中断ボタン押下
                if (ct.IsCancellationRequested)
                    break;

                string tddsplay = "";
                int ledColor = (int)TdLedColor.Red;
                bool ledBlink = false;
                switch (LightType)
                {
                    case 0:
                        if (p.CdSumCourceMaguchi == "") continue;
                        tddsplay = string.Format("{0,3}{1,3}", p.CdSumCourceMaguchi, p.CdSumRouteMaguchi);
                        ledColor = (int)TdLedColor.Red;
                        ledBlink = p.Blink_Course;
                        break;
                    case 1:
                        if (p.CdKyoten == "") continue;
                        tddsplay = string.Format("{0,6}", p.CdSumTokuisaki);
                        ledColor = (int)TdLedColor.Yellow;
                        break;
                    case 2:
                        if (p.CdKyoten == "") continue;
                        tddsplay = string.Format("{0,3}{1,3}", p.Box2 % 1000, p.Box1 % 1000);
                        ledColor = (int)TdLedColor.Green;
                        break;
                    case 3:
                        if (p.CdKyoten == "") continue;
                        tddsplay = string.Format("{0,6}", p.Ops % 1000000);
                        ledColor = (int)TdLedColor.White;
                        break;
                }


                Syslog.Info($"DpsLightOn: addr:{p.TdUnitAddrCode} ledColor:{ledColor} ledBlink:{ledBlink} display:{tddsplay}");

                TdUnitManager.TdUnitLight(p.TdUnitAddrCode, TdDps, bOn, ledBlink, ledColor, tddsplay, false);
            }
        }

        private void DpsLightOff(CancellationTokenSource ct)
        {
            foreach (var p in DistItems)
            {
                // 中断ボタン押下
                if (ct.IsCancellationRequested)
                    break;

                TdUnitManager.TdUnitOff(p.TdUnitAddrCode, TdDps);
            }
        }

        private void DspWait(CancellationTokenSource ct)
        {
            int max = TdDps.WaitTransCount();
            int cnt = -1;

            // キャンセル以外はループ
            while (ct != null && ct.IsCancellationRequested == false)
            {
                cnt = TdDps.WaitTransCount();
                if (max < cnt)
                {
                    max = cnt;
                }

                _eventAggregator.GetEvent<ProgressDialogEvent>()
                    .Publish(new ProgressMessage()
                    {
                        ProgressMax = max,
                        ProgressValue = max - cnt,
                    });

                if (cnt == 0)
                {
                    break;
                }

                // 1秒間隔で更新
                Thread.Sleep(1000);
            }
        }

        private void LoadDatas()
        {
            try
            {
                CollectionViewHelper.SetCollection(DistItems, DistLoaders.Get(DistGroup.DtDelivery.ToString("yyyyMMdd"), DistGroup.CdDistGroup, DistGroup.CdBlock, TdDps.Tdunittype));
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }


        private void AllLightOff()
        {
            Syslog.Debug("MainDispShopViewModel:LightOff");

            if (!CanLight)
            {
                WaitProgressDialog.ShowProgress(
                    "表示器消灯",
                    "表示器消灯中です。しばらくお待ちください。",
                    DpsLightOff,
                    DspWait,
                    null,
                    _dialogService,
                    _eventAggregator);
            }

            CanLight = true;
        }

        private void LightOn(int lighttype)
        {
            bTdConnectionError = false;
            CanLight = false;
            CanList = false;
            LightType = lighttype;

            WaitProgressDialog.ShowProgress(
                "表示器点灯",
                "表示器点灯中です。しばらくお待ちください。",
                DpsLightOn,
                DspWait,
                null,
                _dialogService,
                _eventAggregator);

        }

        private bool SelectDistGroup()
        {
            if (AuthenticateService.AuthDistGroupDialog(_dialogService) is DistGroup distgroup)
            {
                DistGroup = distgroup;
                return true;
            }
            return false;
        }

    }
}
