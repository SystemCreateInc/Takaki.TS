using DbLib;
using DbLib.DbEntities;
using LogLib;
using Picking.Models;
using Picking.Services;
using Picking.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using ProcessorLib;
using SelDistGroupLib.Models;
using SelDistGroupLib.Services.Auth;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using TdDpsLib.Models;
using WindowLib.Utils;
using WindowLib.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace Picking.ViewModels
{
    public class MainPickingViewModel : BindableBase, INavigationAware
    {
        private static readonly object _locklog = new object();

        private DateTime _lastLogTime = DateTime.Now;


        public DelegateCommand? OnItemInfo { get; }
        public DelegateCommand? OnCheck { get; }
        public DelegateCommand? OnExtraction { get; }
        public DelegateCommand? OnSetShain { get; }
        public DelegateCommand? OnChangeDistType { get; }

        public DelegateCommand? OnExit { get; }
        public DelegateCommand? OnColorBtn1 { get; }
        public DelegateCommand? OnColorBtn2 { get; }
        public DelegateCommand? OnColorBtn3 { get; }
        public DelegateCommand? OnColorBtn4 { get; }
        public DelegateCommand? OnColorBtn5 { get; }



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


        private DistGroupEx _distgroup = new DistGroupEx();
        public DistGroupEx DistGroup
        {
            get => _distgroup;
            set => SetProperty(ref _distgroup, value);
        }

        private List<DistColor>? _displaydistColorDatas = new List<DistColor>();

        public List<DistColor>? DisplayDistColorDatas
        {
            get => _displaydistColorDatas;
            set => SetProperty(ref _displaydistColorDatas, value);
        }

        private DistColor? _currentdistcolor = null;

        public DistColor? CurrentDistColor
        {
            get => _currentdistcolor;
            set
            {
                SetProperty(ref _currentdistcolor, value);
                CanDistColor = _currentdistcolor == null ? false : _currentdistcolor.DStatus == 0 ? false : true;
            }
        }
        private bool _canDistColor = false;

        public bool CanDistColor
        {
            get => _canDistColor;
            set => SetProperty(ref _canDistColor, value);
        }

        // 色0
        private Shain _colorBtnName1 = new Shain();
        public Shain ColorBtnName1
        {
            get => _colorBtnName1;
            set => SetProperty(ref _colorBtnName1, value);
        }

        // 色1
        private Shain _colorBtnName2 = new Shain();
        public Shain ColorBtnName2
        {
            get => _colorBtnName2;
            set => SetProperty(ref _colorBtnName2, value);
        }

        // 色1
        private Shain _colorBtnName3 = new Shain();
        public Shain ColorBtnName3
        {
            get => _colorBtnName3;
            set => SetProperty(ref _colorBtnName3, value);
        }

        // 色1
        private Shain _colorBtnName4 = new Shain();
        public Shain ColorBtnName4
        {
            get => _colorBtnName4;
            set => SetProperty(ref _colorBtnName4, value);
        }

        // 色1
        private Shain _colorBtnName5 = new Shain();
        public Shain ColorBtnName5
        {
            get => _colorBtnName5;
            set => SetProperty(ref _colorBtnName5, value);
        }

        private SolidColorBrush _backgroundColor1 = new SolidColorBrush(Colors.Red);
        public SolidColorBrush BackgroundColor1
        {
            get => _backgroundColor1;
            set => SetProperty(ref _backgroundColor1, value);
        }

        private SolidColorBrush _backgroundColor2 = new SolidColorBrush(Colors.Yellow);
        public SolidColorBrush BackgroundColor2
        {
            get => _backgroundColor2;
            set => SetProperty(ref _backgroundColor2, value);
        }

        private SolidColorBrush _backgroundColor3 = new SolidColorBrush(Colors.LightGreen);
        public SolidColorBrush BackgroundColor3
        {
            get => _backgroundColor3;
            set => SetProperty(ref _backgroundColor3, value);
        }

        private SolidColorBrush _backgroundColor4 = new SolidColorBrush(Colors.White);
        public SolidColorBrush BackgroundColor4
        {
            get => _backgroundColor4;
            set => SetProperty(ref _backgroundColor4, value);
        }

        private SolidColorBrush _backgroundColor5 = new SolidColorBrush(Colors.Blue);
        public SolidColorBrush BackgroundColor5
        {
            get => _backgroundColor5;
            set => SetProperty(ref _backgroundColor5, value);
        }


        private SolidColorBrush _foregroundColor1 = new SolidColorBrush(Colors.Black);
        public SolidColorBrush ForegroundColor1
        {
            get => _foregroundColor1;
            set => SetProperty(ref _foregroundColor1, value);
        }

        private SolidColorBrush _foregroundColor2 = new SolidColorBrush(Colors.Black);
        public SolidColorBrush ForegroundColor2
        {
            get => _foregroundColor2;
            set => SetProperty(ref _foregroundColor2, value);
        }

        private SolidColorBrush _foregroundColor3 = new SolidColorBrush(Colors.Black);
        public SolidColorBrush ForegroundColor3
        {
            get => _foregroundColor3;
            set => SetProperty(ref _foregroundColor3, value);
        }

        private SolidColorBrush _foregroundColor4 = new SolidColorBrush(Colors.Black);
        public SolidColorBrush ForegroundColor4
        {
            get => _foregroundColor4;
            set => SetProperty(ref _foregroundColor4, value);
        }

        private SolidColorBrush _foregroundColor5 = new SolidColorBrush(Colors.White);
        public SolidColorBrush ForegroundColor5
        {
            get => _foregroundColor5;
            set => SetProperty(ref _foregroundColor5, value);
        }

        private SolidColorBrush _checkColor = new SolidColorBrush(Colors.DarkTurquoise);
        public SolidColorBrush CheckColor
        {
            get => _checkColor;
            set => SetProperty(ref _checkColor, value);
        }

        private bool _ischeck = false;
        public bool IsCheck
        {
            get => _ischeck;
            set => SetProperty(ref _ischeck, value);
        }

        private SolidColorBrush _extractionColor = new SolidColorBrush(Colors.DarkTurquoise);
        public SolidColorBrush ExtractionColor
        {
            get => _extractionColor;
            set => SetProperty(ref _extractionColor, value);
        }

        private bool _isExtraction = false;
        public bool IsExtraction
        {
            get => _isExtraction;
            set => SetProperty(ref _isExtraction, value);
        }


        // 進捗数
        private ProgressCnt? _packcnt = null;

        public ProgressCnt? PackCnt
        {
            get => _packcnt;
            set
            {
                value?.UpdateText();
                SetProperty(ref _packcnt, value);
            }
        }

        private bool _canConfirmation = true;
        public bool CanConfirmation
        {
            get => _canConfirmation;
            set => SetProperty(ref _canConfirmation, value);
        }

        private bool _canConfirmationShifted;
        public bool CanConfirmationShifted
        {
            get => _canConfirmationShifted;
            set => SetProperty(ref _canConfirmationShifted, value);
        }

        private bool _canUndoShifted;
        public bool CanUndoShifted
        {
            get => _canUndoShifted;
            set => SetProperty(ref _canUndoShifted, value);
        }

        private bool _candisttype = true;
        public bool CanDistType
        {
            get => _candisttype;
            set => SetProperty(ref _candisttype, value);
        }

        private bool _canClose = false;

        private Shain? _selectedshain = null;
        public Shain? SelectedShain
        {
            get => _selectedshain;
            set => SetProperty(ref _selectedshain, value);
        }

        private DistColorInfo _distcolorinfo;
        public DistColorInfo DistColorInfo
        {
            get => _distcolorinfo;
            set => SetProperty(ref _distcolorinfo, value);
        }

        public bool bTdConnectionError { get; set; } = false;

        public TdDpsManager TdDps;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private System.Threading.Mutex _mutex;
        private readonly DispatcherTimer _idleTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle, System.Windows.Application.Current.Dispatcher);

        public MainPickingViewModel(IRegionManager regionManager, IDialogService dialogService, IEventAggregator eventAggregator, TdDpsManager tddps, DistColorInfo distcolorinfo, DistGroupEx distgroup)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;
            TdDps = tddps;
            _distcolorinfo = distcolorinfo;
            _distgroup = distgroup;


            // 配分種類デフォルト設定 1:一斉　0:追駆け
            DistColorInfo.DistWorkType = 1;

            _mutex = new System.Threading.Mutex(false, "MainPickingViewModel");

            // 二重起動防止
            if (_mutex.WaitOne(0, false) == false)
            {
                System.Windows.Application.Current.MainWindow.Close();
                return;
            }


            // 仕分グループ選択
            if (!SelectDistGroup())
            {
                System.Windows.Application.Current.MainWindow.Close();
                return;
            }

            OnItemInfo = new DelegateCommand(() =>
            {
                Syslog.Info("【商品一覧】:OnItemInfo");

                _regionManager.RequestNavigate("ContentRegion", nameof(Views.DistInfoWindow));
            });

            OnCheck = new DelegateCommand(() =>
            {
                Syslog.Info("【検品】:OnCheck");

                if (IsCheck == true)
                {
                    IsCheck = false;
                    CheckColor = new SolidColorBrush(Colors.DarkTurquoise);
                }
                else
                {
                    IsCheck = true;
                    CheckColor = new SolidColorBrush(Colors.Orange);
                }
                IsExtraction = false;
                ExtractionColor = new SolidColorBrush(Colors.DarkTurquoise);
            });

            OnExtraction = new DelegateCommand(() =>
            {
                Syslog.Info("【抜き取り】:OnExtraction");

                if (IsExtraction == true)
                {
                    IsExtraction = false;
                    ExtractionColor = new SolidColorBrush(Colors.DarkTurquoise);
                }
                else
                {
                    IsExtraction = true;
                    ExtractionColor = new SolidColorBrush(Colors.Orange);
                }
                IsCheck = false;
                CheckColor = new SolidColorBrush(Colors.DarkTurquoise);
            });

            OnSetShain = new DelegateCommand(() =>
            {
                Syslog.Info("【社員設定】:OnSetShain");

                dialogService.ShowDialog(
                    nameof(ShainDlg),
                    new DialogParameters
                    {
                        { "_datas", DisplayDistColorDatas },
                        { "colorbtn1", _colorBtnName1 },
                        { "colorbtn2", _colorBtnName2 },
                        { "colorbtn3", _colorBtnName3 },
                        { "colorbtn4", _colorBtnName4 },
                        { "colorbtn5", _colorBtnName5 },
                    },
                    (rc) =>
                    {
                        if (rc.Result == ButtonResult.OK)
                        {
                            ColorBtnName1 = rc.Parameters.GetValue<Shain>("Shain1");
                            ColorBtnName2 = rc.Parameters.GetValue<Shain>("Shain2");
                            ColorBtnName3 = rc.Parameters.GetValue<Shain>("Shain3");
                            ColorBtnName4 = rc.Parameters.GetValue<Shain>("Shain4");
                            ColorBtnName5 = rc.Parameters.GetValue<Shain>("Shain5");

                            for (int idx = 0; idx < _distcolorinfo.DistColors?.Count(); idx++)
                            {
                                Shain ?syain = null;
                                switch (idx)
                                {
                                    case 0: syain = ColorBtnName1; break;
                                    case 1: syain = ColorBtnName2; break;
                                    case 2: syain = ColorBtnName3; break;
                                    case 3: syain = ColorBtnName4; break;
                                    case 4: syain = ColorBtnName5; break;
                                }
                                if (syain != null)
                                {
                                    if (_distcolorinfo.DistColors[idx].IsWorking() == false)
                                    {
                                        _distcolorinfo.DistColors[idx].CdShain = syain.CdShain;
                                        _distcolorinfo.DistColors[idx].NmShain = syain.NmShain;
                                    }
                                }
                            }
                        }
                    });
            });

            OnChangeDistType = new DelegateCommand(() =>
            {
                Syslog.Info("【配分タイプ変更】:OnChangeDistType");

                for (int idx = 0; idx < _distcolorinfo.DistColors?.Count(); idx++)
                {
                    if (_distcolorinfo.IsWorking() == true)
                    {
                        MessageDialog.Show(_dialogService, "作業中のため変更出来ません。", "エラー");
                        return;
                    }
                }

                distcolorinfo.DistWorkType = distcolorinfo.DistWorkType == 0 ? 1 : 0;

            }).ObservesCanExecute(() => CanDistType);



            OnExit = new DelegateCommand(() =>
            {
                Syslog.Info("【終了】:Exit");
                if (MessageDialog.Show(_dialogService, "仕分処理を終了します。よろしいですか？", "確認", ButtonMask.OK | ButtonMask.Cancel) == ButtonResult.OK)
                {
                    try
                    {
                        for (int idx = 0; idx < _distcolorinfo.DistColors?.Count(); idx++)
                        {
                            DistColor distcolor = _distcolorinfo.DistColors[idx];
                            bool bRet = TdUnitManager.TdLightOff(ref distcolor, TdDps, true);

                            distcolor.ReportEnd();
                        }

                        // 作業実績書き込み
                        DistColorManager.WorkReportAppend(DistGroup, _distcolorinfo.RepotShains);

                        UpdateProgress(true);

                        WaitProgressDialog.ShowProgress(
                            "表示器消灯",
                            "表示器消灯中です。しばらくお待ちください。",
                            null,
                            DspWait,
                            null,
                            _dialogService,
                            _eventAggregator);

                        _canClose = true;
                        System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => System.Windows.Application.Current.MainWindow.Close()));
                    }
                    catch (Exception e)
                    {
                        var buttons = new ButtonResult[] { ButtonResult.OK };
                        MessageDialog.ShowAsync(_dialogService, e.Message, "エラー", buttons);
                    }
                }
            }).ObservesCanExecute(() => CanUndoShifted);


            OnColorBtn1 = new DelegateCommand(() =>
            {
                Syslog.Info("【社員設定】:OnColorBtn1");
                if (PushColorBtn(1))
                {
                    return;
                }
            });
            OnColorBtn2 = new DelegateCommand(() =>
            {
                Syslog.Info("【社員設定】:OnColorBtn2");
                if (PushColorBtn(2))
                {
                    return;
                }
            });
            OnColorBtn3 = new DelegateCommand(() =>
            {
                Syslog.Info("【社員設定】:OnColorBtn3");
                if (PushColorBtn(3))
                {
                    return;
                }
            });
            OnColorBtn4 = new DelegateCommand(() =>
            {
                Syslog.Info("【社員設定】:OnColorBtn4");
                if (PushColorBtn(4))
                {
                    return;
                }
            });
            OnColorBtn5 = new DelegateCommand(() =>
            {
                Syslog.Info("【社員設定】:OnColorBtn5");
                if (PushColorBtn(5))
                {
                    return;
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
                            bool bDistEnd = false;
                            TdUnitManager.TdUnitRcv(TdDps, _distcolorinfo, stno, group, addr, color, ref bDistEnd);
                            // 完了したので進捗更新
                            if (bDistEnd)
                            {
                                UpdateProgress();
                            }
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

                // 表示器ﾀｲﾌﾟ設定
                DistGroup.TdUnitType = TdDps.Tdunittype;

                _idleTimer.Tick += (s, e) => CheckShifted();
                _idleTimer.Start();

                // 色設定
                _distcolorinfo.DistColors = DistColorManager.SetColors();

                UpdateColorDisplay();
            }
            catch (Exception e)
            {
                Syslog.Info($"MainPickingViewModel::{e.ToString()}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
            // 閉じるボタン応答
            System.Windows.Application.Current.MainWindow.Closing += Closing;
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

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            UpdateProgress();
            IsCheck = false;
            CheckColor = new SolidColorBrush(Colors.DarkTurquoise);
            IsExtraction = false;
            ExtractionColor = new SolidColorBrush(Colors.DarkTurquoise);
        }

        private void Closing(object? sender, CancelEventArgs e)
        {
            // 
            if (!_canClose)
            {
                e.Cancel = true;
                return;
            }

            try
            {
                _cancellationTokenSource?.Cancel();
            }
            catch (Exception ex)
            {
                Syslog.Emerg($"EXCEPTION: {ex.Message}");
            }

            TdDps.Tarm();

            _idleTimer.Tick -= (s, e) => CheckShifted();
            _idleTimer.Stop();
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

        private void UpdateColorDisplay()
        {
            DisplayDistColorDatas = _distcolorinfo.DistColors?.ToList();
        }

        private void UpdateProgress(bool bEnd=false)
        {
            Task.Run(() =>
            {
                try
                {
                    RefreshToolBarBtn();
                    PackCnt = DistProgressManager.GetProgressCnts(DistGroup);
                    if (PackCnt != null)
                    {
                        DistProgressManager.UpdateDistProgress(DistGroup, _distcolorinfo, PackCnt, bEnd);
                    }
                }
                catch (Exception e)
                {
                    Syslog.Error($"UpdateProgress:{e.Message}");
                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                }

                if (PackCnt == null)
                {
                    PackCnt = new ProgressCnt();
                }
            });
        }
        private void CheckShifted()
        {
            bool shifted = (Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) != 0
                || (Keyboard.GetKeyStates(Key.RightShift) & KeyStates.Down) != 0;

            if (PackCnt is not null && PackCnt.CntMax == PackCnt.CntValue)
            {
                CanConfirmationShifted = true;
                CanUndoShifted = true;
            }
            else
            {
                CanConfirmationShifted = CanConfirmation && shifted;
                CanUndoShifted = shifted;
            }
        }
        private void RefreshInfoEvent(object? sender, EventArgs e)
        {
            RefreshToolBarBtn();
        }
        private void RefreshToolBarBtn()
        {
            CanDistColor = _currentdistcolor == null ? false : _currentdistcolor.DStatus == 0 ? false : true;
        }
        private bool SelectDistGroup()
        {
            if (AuthenticateService.AuthDistGroupDialog(_dialogService) is DistGroup distgroup)
            {
                DistGroup.CdDistGroup = distgroup.CdDistGroup;
                DistGroup.NmDistGroup = distgroup.NmDistGroup;
                DistGroup.DtDelivery = distgroup.DtDelivery;
                DistGroup.IdPc = distgroup.IdPc;
                DistGroup.CdBlock = distgroup.CdBlock;
                DistGroup.CdKyoten = distgroup.CdKyoten;
                return true;
            }
            return false;
        }
        private bool PushColorBtn(int color)
        {
            switch(color)
            {
                case 1: SelectedShain = ColorBtnName1; break;
                case 2: SelectedShain = ColorBtnName2; break;
                case 3: SelectedShain = ColorBtnName3; break;
                case 4: SelectedShain = ColorBtnName4; break;
                case 5: SelectedShain = ColorBtnName5; break;
            }

            if (SelectedShain == null || SelectedShain.CdShain == "")
            {
                MessageDialog.Show(_dialogService, "作業担当者を設定して下さい。", "エラー");
                return false;
            }

            // ウインドウ表示
            _regionManager.RequestNavigate("ContentRegion", nameof(Views.DistItemScanWindow), new NavigationParameters
                {
                    { "color", color },
                    { "ischeck", IsCheck},
                    { "isextraction", IsExtraction},
                    { "SelectedShain", SelectedShain},
                });
            return true;
        }
    }
}
