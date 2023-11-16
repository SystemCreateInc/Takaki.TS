// <copyright file="MainLightTestViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LightTest.ViewModels
{
    using LightTest.Models;
    using LightTest.Views;
    using LogLib;
    using Prism.Commands;
    using Prism.Events;
    using Prism.Mvvm;
    using Prism.Regions;
    using Prism.Services.Dialogs;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup.Localizer;
    using System.Windows.Media;
    using TdDpsLib.Defs;
    using TdDpsLib.Models;
    using WindowLib.Utils;
    using WindowLib.ViewModels;

    public class MainLightTestViewModel : BindableBase, INavigationAware
    {
        private static readonly object _locklog = new object();

        private DateTime _lastLogTime = DateTime.Now;

        public DelegateCommand LightOn { get; }

        public DelegateCommand LightOff { get; }

        public DelegateCommand Exit { get; }

        public DelegateCommand SelAll { get; }

        public DelegateCommand DeSelAll { get; }

        public DelegateCommand Select { get; }
        public DelegateCommand Settings { get; }

        public TdDpsManager TdDps { get; set; } = new TdDpsManager();

        private ObservableCollection<TdDisplayLog> _displayLogs = new ObservableCollection<TdDisplayLog>();

        public ObservableCollection<TdDisplayLog> DisplayLogs
        {
            get => _displayLogs;
            set => SetProperty(ref _displayLogs, value);
        }

        private bool[] _dispBlinkType = { true, false };

        public bool[] DispBlinkType
        {
            get => _dispBlinkType;
            set => SetProperty(ref _dispBlinkType, value);
        }

        private bool _selSelectCol = true;

        public bool SelSelectCol
        {
            get => _selSelectCol;
            set => SetProperty(ref _selSelectCol, value);
        }

        // 点灯対象ボタン
        private bool[] _buttons = new bool[] { true, false, false, false, false };

        public bool[] Buttons
        {
            get => _buttons;
            set => SetProperty(ref _buttons, value);
        }

        // 表示器表示タイプ　0:論理 1:物理 2:888888
        private bool[] _dispType = { true, false, false };

        public bool[] DispType
        {
            get => _dispType;
            set => SetProperty(ref _dispType, value);
        }

        public DISPTYPE GetDispType
        {
            get
            {
                if (DispType[0])
                {
                    return DISPTYPE.DISPNO;
                }
                else if (DispType[1])
                {
                    return DISPTYPE.ADDR;
                }
                else
                {
                    return DISPTYPE.ALL8;
                }
            }
        }

        /// <summary>データグリッドで選択中のアイテム(複数可)</summary>
        public ObservableCollection<object> SelectedItems { get; set; } = new ObservableCollection<object>();

        private bool _canLight = true;

        public bool CanLight
        {
            get => _canLight;
            set => SetProperty(ref _canLight, value);
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

        public ObservableCollection<TdShowAddrData> DisplayErrs
        {
            get => _diaplayerrs;
            set => SetProperty(ref _diaplayerrs, value);
        }
        public bool bTdConnectionError { get; set; } = false;

        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private System.Threading.Mutex _mutex;

        public MainLightTestViewModel(IDialogService dialogService, IEventAggregator eventAggregator)
        {
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;

            // イベント
            LightOn = new DelegateCommand(() =>
            {
                Syslog.Info("【点灯】:LightOn");

                CanLight = false;
                DisplayErrs = new ObservableCollection<TdShowAddrData>();

                // DisplayErrsをスレッドセーフへ
                BindingOperations.EnableCollectionSynchronization(this.DisplayErrs, new object());

                // wakeupは始業で実行するように変更
#if false
                if (TdDps.IsWakeUp() == true)
                {
                    // WwakeUp開始
                    WaitProgressDialog.ShowProgress(
                        "WakeUp",
                        "WakeUp中です。しばらくお待ちください。",
                        TdDps.StartWakeUp,
                        WakeUpWait,
                        null,
                        _dialogService,
                        _eventAggregator);
                }
#endif
                if (TdDps.IsBatteryCheck() == true)
                {
                    // WwakeUp開始
                    WaitProgressDialog.ShowProgress(
                        "バッテリーチェック中",
                        "バッテリーチェック中です。しばらくお待ちください。",
                        TdDps.StartBatteryCheck,
                        DspWait,
                        null,
                        _dialogService,
                        _eventAggregator);
                }

                WaitProgressDialog.ShowProgress(
                    "表示器点灯",
                    "表示器点灯中です。しばらくお待ちください。",
                    DpsLightOn,
                    DspWait,
                    null,
                    _dialogService,
                    _eventAggregator);
            }, () => CanLight).ObservesProperty(() => CanLight);

            LightOff = new DelegateCommand(() =>
            {
                Syslog.Info("【消灯】:LightOff");

                WaitProgressDialog.ShowProgress(
                    "表示器消灯",
                    "表示器消灯中です。しばらくお待ちください。",
                    DpsLightOff,
                    DspWait,
                    null,
                    _dialogService,
                    _eventAggregator);

                CanLight = true;
            }, () => !CanLight).ObservesProperty(() => CanLight);

            Exit = new DelegateCommand(() =>
            {
                Syslog.Info("【終了】:Exit");

                Application.Current.MainWindow.Close();
            });

            SelAll = new DelegateCommand(() =>
            {
                Syslog.Info("【全選択】:SelAll");

                DisplayAddrs.ForEach(x =>
                {
                    x.Select = true;
                });
            });

            DeSelAll = new DelegateCommand(() =>
            {
                Syslog.Info("【全解除】:DeSelAll");

                DisplayAddrs.ForEach(x =>
                {
                    x.Select = false;
                });
            });

            Select = new DelegateCommand(() =>
            {
                Syslog.Info("【指定選択】:Select");

                foreach (TdShowAddrData disp in SelectedItems)
                {
                    disp.Select = !disp.Select;
                }
            });

            Settings = new DelegateCommand(() =>
            {
                dialogService.ShowDialog(
                    nameof(SettingsDlg),
                    new DialogParameters
                    {
                        { "_buttons", _buttons },
                        { "_dispType", _dispType },
                        { "_dispBlinkType", _dispBlinkType },
                    },
                    (rc) =>
                    {
                        if (rc.Result == ButtonResult.OK)
                        {
                            // デフォルト値読み込み
                            Buttons = LightDefaultManager.LoadDefaultButtons();
                            DispType = LightDefaultManager.LoadDefaultDispTypes();
                            DispBlinkType = LightDefaultManager.LoadDefaultBlinkTypes();
                        }
                    });
            });

            _mutex = new System.Threading.Mutex(false, "MainLightTestViewModel");

            // 二重起動防止
            if (_mutex.WaitOne(0, false) == false)
            {
                Application.Current.MainWindow.Close();
                return;
            }

            // DisplayLogsをスレッドセーフへ
            BindingOperations.EnableCollectionSynchronization(this.DisplayLogs, new object());

            // DisplayErrsをスレッドセーフへ
            BindingOperations.EnableCollectionSynchronization(this.DisplayErrs, new object());

            try
            {
                TdDps.Init();
                TdDps.Open();

                // 表示器ログイベント登録
                TdDps.SetTdLogEvent(
                    (stno, log) =>
                    {
                        lock (_locklog)
                        {
                            DateTime time = DateTime.Now;
                            TimeSpan span = time - _lastLogTime;
                            var logdat = new TdDisplayLog() { LogText = $"{time.ToString("HH:mm:ss.fff ")} {span.Milliseconds.ToString("d3")} > {log}", LogColor = log.Substring(0, 1) };

                            _lastLogTime = time;
                            DisplayLogs.Add(logdat);

                            // コネクションエラーは点灯を止める
                            if (log.IndexOf("Connection Error") > 0)
                            {
                                bTdConnectionError = true;
                                var tdunit = TdDps.TdUnits.Find(unit => unit?.TdPort?.Stno == stno);
                                tdunit?.QueCancel();

                            }
                        }
                        if (bTdConnectionError)
                        {
                            MessageDialog.Show(_dialogService, "表示器コントローラーとの接続が切断されました", "エラー", ButtonMask.OK);
                        }
                    });

                // 表示器ボタン押下イベント登録
                TdDps.SetTdButtonPushEvent(
                    (stno, group, addr, color) =>
                    {
                        TdAddrData? addrdata;
                        TdDps.GetTdAddr(out addrdata, stno, group, addr);

                        if (addrdata != null)
                        {
                            var led = addrdata.GetLedButton(color);
                            if (led == null)
                            {
                                Syslog.Info($"SetTdButtonEvent::color err {color}");
                                return;
                            }
                            // 1ボタンなのでどのボタンを押しても消灯するように変更
                            if (led.IsLight == false)
                            {
                                color = addrdata.GetBlinkButton();
                                led = addrdata.GetLedButton(color);
                            }
                            if (led == null)
                            {
                                return;
                            }

                            if (led.IsLight == true)
                            {
                                // 消灯
                                string ?text = addrdata.IsLedButtonOtherLight(color) ? led.Text : "";
                                TdDps.Light(ref addrdata, false, false, color, text??"", true);
                            }
                        }

                        return;
                    });

                // 表示器レスポンスイベント登録
                TdDps.SetTdResponseEvent(
                    (stno, group, addr, text) =>
                    {
                        TdShowAddrData ?addrdata = DisplayAddrs.Find(p => p.TddGroup == group && p.TddAddr == addr && p.Stno == stno);

                        if (addr == 999 && text != "ACK")
                        {
                            // 接続失敗
                            var port = TdPorts.Where(x => x.Stno == stno).FirstOrDefault();
                            if (port != null)
                                port.PortStatus = TdPortStatus.Error;

                            // 更新イベント
//                            RaisePropertyChanged(nameof(port.GetPortStatusName));
                        }

                        if (addrdata != null)
                        {
                            // 画面更新＆状態設定
                            addrdata.SetDStatusName(addrdata.TdResponseToUpdateStatus(text));

                            // バッテリー容量更新
                            addrdata.SetBatteryInfoName(addrdata.BatteryInfo);

                            // エラーの場合はエラー表示
                            if (addrdata.IsTdErr(text) == true)
                            {
                                // 同じアドレスは書き換え
                                var p = DisplayErrs.Where(x => x.TdUnitCode == addrdata.TdUnitCode).FirstOrDefault();

                                if (p == null)
                                {
                                    TdShowAddrData newAddr = new TdShowAddrData(addrdata);
                                    newAddr.LightTestErrStatusName = addrdata.LightTestErrStatusName;
                                    DisplayErrs.Add(newAddr);
                                }
                                else
                                {
                                    p.LightTestErrStatusName = addrdata.LightTestErrStatusName;
                                }
                            }
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
                Syslog.Info($"MainLightTestViewModel::{e.ToString()}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }




            // 閉じるボタン応答
            Application.Current.MainWindow.Closing += Closing;

            Buttons = LightDefaultManager.LoadDefaultButtons();
            DispType = LightDefaultManager.LoadDefaultDispTypes();
            DispBlinkType = LightDefaultManager.LoadDefaultBlinkTypes();

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
                Syslog.Info($"MainLightTestViewModel::{e.ToString()}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }

        // 閉じる
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

        public void DpsLightOn(CancellationTokenSource ct)
        {
            bTdConnectionError = false;

            var addrs = DisplayAddrs.Where(x => x.Select == true);

            // 点灯
            foreach (var p in addrs)
            {
                // 中断ボタン押下
                if(ct.IsCancellationRequested)
                    break;

                TdAddrData? addrdata;
                TdDps.GetTdAddr(out addrdata, p.Stno, p.TddGroup, p.TddAddr);

                if (addrdata != null)
                {
                    string display = "888888";
                    switch (GetDispType)
                    {
                        case DISPTYPE.DISPNO:
                            string tmp = addrdata.TdUnitAddrCode.Trim().Count() >= 7 ? addrdata.TdUnitAddrCode.Trim().Substring(addrdata.TdUnitAddrCode.Count() - 6) : addrdata.TdUnitAddrCode.Trim();

                            display = $"{tmp,6}";
                            break;
                        case DISPTYPE.ADDR:
                            display = string.Format($"{addrdata.TddGroup:00}{addrdata.TddAddr:000}");
//                            display = $"{addrdata.Physics,6}";
                            break;
                    }

                    bool blink = DispBlinkType[1];

                    int color = 0;
                    foreach (var btn in Buttons)
                    {
                        color++;
                        if (btn == true)
                        {
                            TdDps.Light(ref addrdata, true, blink, color, display);
                        }
                    }
                }
            }
        }

        public void DpsLightOff(CancellationTokenSource ct)
        {
            bTdConnectionError = false;

            foreach (var p in DisplayAddrs)
            {
                // 中断ボタン押下
                if (ct.IsCancellationRequested)
                    break;

                int color = 0;
                foreach (var btn in Buttons)
                {
                    color++;
                    if (p.GetLedButton(color)?.IsLight==true)
                    {
                        TdAddrData? addrdata;
                        TdDps.GetTdAddr(out addrdata, p.Stno, p.TddGroup, p.TddAddr);

                        if (addrdata != null)
                        {
                            TdDps.Light(ref addrdata, false, false, color, "");
                        }
                    }
                }
            }
        }

        public void DspWait(CancellationTokenSource ct)
        {
            int max = TdDps.WaitTransCount();
            int cnt = -1;

            // キャンセル以外はループ
            while (ct != null && ct.IsCancellationRequested == false)
            {
                // 表示器接続エラーは中断する
                if (bTdConnectionError)
                    break;

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

        public void WakeUpWait(CancellationTokenSource ct)
        {
            int cnt = 150;
#if false
            _eventAggregator.GetEvent<ProgressDialogEvent>()
                .Publish(new ProgressMessage()
                {
                    Title = "表示器",
                    Message = "ウエイクアップ中です。しばらくお待ち下さい",
                    ProgressMax = 0,
                    ProgressValue = 0,
                });
#endif

            // ３０秒待つ
            for (int i = 0; i < cnt; i++)
            {
                // キャンセルされた？
                if (ct == null || (ct != null && ct.IsCancellationRequested))
                {
                    break;
                }

                _eventAggregator.GetEvent<ProgressDialogEvent>()
                    .Publish(new ProgressMessage()
                    {
                        ProgressMax = cnt,
                        ProgressValue = i,
                    });

                Thread.Sleep(200);
            }

            return;
        }

        // 表示器送信完了
        public void QueDequeued(object ?sender, ItemEventArgs<QueCommand> e)
        {
            // 表示用表示器アドレスの状態変更
            QueCommand ?que = e.Item;

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

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        private void Closing(object ?sender, CancelEventArgs e)
        {
            WaitProgressDialog.ShowProgress(
                "表示器消灯",
                "表示器消灯中です。しばらくお待ちください。",
                DpsLightOff,
                DspWait,
                null,
                _dialogService,
                _eventAggregator);

            TdDps.Tarm();
        }

        /// <summary>
        /// 表示器状態表示更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayPropatyChanged(object ?sender, PropertyChangedEventArgs e)
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

        /// <summary>
        /// 選択ボタン色データ変換
        /// </summary>
        /// <returns></returns>
        private List<int> SetColorButtons()
        {
            var btnList = new List<int>();

            for (int idx = 0; idx < Buttons.Length; idx++)
            {
                if (Buttons[idx])
                {
                    btnList.Add(idx + 1);
                }
            }

            return btnList;
        }
    }
}
