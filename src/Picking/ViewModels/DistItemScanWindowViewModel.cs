﻿using ImTools;
using LogLib;
using Picking.Defs;
using Picking.Models;
using Picking.Services;
using Picking.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using TdDpsLib.Models;
using WindowLib.Utils;
using static ImTools.ImMap;
using static System.Net.Mime.MediaTypeNames;

namespace Picking.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderingLockInstance
    {
        /// <summary>
        /// 
        /// </summary>
        protected string Identifier { get; set; } = string.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    internal class OrderingLockManager
    {
        /// <summary>
        /// 
        /// </summary>
        static protected Dictionary<OrderingLockInstance, Queue<OrderingLock>> lockQueue = new Dictionary<OrderingLockInstance, Queue<OrderingLock>>();

        /// <summary>
        /// 
        /// </summary>
        static protected OrderingLockManager? instance = null;

        /// <summary>
        /// 
        /// </summary>
        public static OrderingLockManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockQueue)
                    {
                        if (instance == null)
                        {
                            instance = new OrderingLockManager();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locker"></param>
        /// <param name="lockInstance"></param>
        public void EnQueue(OrderingLock locker, OrderingLockInstance lockInstance)
        {
            lock (lockQueue)
            {
                if (lockQueue.ContainsKey(lockInstance) == true)
                {
                    // 誰か忙しい人がいる
                    lockQueue[lockInstance].Enqueue(locker);
                }
                else
                {
                    lockQueue.Add(lockInstance, new Queue<OrderingLock>());

                    // 走りだせ!
                    locker.Ready.Set();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locker"></param>
        /// <param name="lockInstance"></param>
        public void Finished(OrderingLock locker, OrderingLockInstance lockInstance)
        {
            lock (lockQueue)
            {
                if (lockQueue.ContainsKey(lockInstance) == true)
                {
                    if (lockQueue[lockInstance].Count == 0)
                    {
                        // 終了
                        lockQueue.Remove(lockInstance);
                    }
                    else
                    {
                        // 次へ
                        lockQueue[lockInstance].Dequeue().Ready.Set();
                    }
                }
                else
                {
                    // ERROR!(あってはならないルート)
                    throw new InvalidOperationException("Method calling sequence error.");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected OrderingLockManager()
        {
            // NOP
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// このクラスの一般的な利用方法を次の例に示します。
    /// <code><![CDATA[
    /// OrderingLockInstance lockInstance = new OrderingLockInstance();
    ///
    /// ...
    /// 
    /// using (new OrderingLock(lockInstance))
    /// {
    ///     // Do something
    /// }
    /// ]]></code>
    /// </example>
    public class OrderingLock : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        protected AutoResetEvent ready = new AutoResetEvent(false);

        /// <summary>
        /// 
        /// </summary>
        protected OrderingLockInstance? lockInstance = null;

        /// <summary>
        /// 
        /// </summary>
        internal AutoResetEvent Ready
        {
            get
            {
                return ready;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lockInstance"></param>
        public OrderingLock(OrderingLockInstance lockInstance)
        {
            this.lockInstance = lockInstance;
            OrderingLockManager.Instance.EnQueue(this, lockInstance);

            ready.WaitOne();
        }

        /// <summary>
        /// 
        /// </summary>
        ~OrderingLock()
        {
            // using パターンを使わない人のために

            // 次の人へ
            if (lockInstance != null)
            {
                OrderingLockManager.Instance.Finished(this, lockInstance);
                lockInstance = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            // 次へ
            if (lockInstance != null)
            {
                OrderingLockManager.Instance.Finished(this, lockInstance);
                lockInstance = null;
            }
        }
    }
    public class DistItemScanWindowViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand OnItemCancel { get; }
        public DelegateCommand OnChangeQty { get; }
        public DelegateCommand OnChangeBoxUnit { get; }
        public DelegateCommand OnShowDetailInfo { get; }
        public DelegateCommand OnBack { get; }
        public DelegateCommand OnCancel { get; }
        public DelegateCommand OnStop { get; }
        public DelegateCommand OnEnter { get; }
        public DelegateCommand OnStart { get; }

        private ReaderWriterLockSlim _lockLight = new ReaderWriterLockSlim();

        private int _color = 0;
        public int Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        private string _scancode = string.Empty;
        public string Scancode
        {
            get => _scancode;
            set => SetProperty(ref _scancode, value);
        }

        private int _inseq = 0;
        public int InSeq
        {
            get => _inseq;
            set => SetProperty(ref _inseq, value);
        }

        private List<DistItemSeq>? _displaydistitemDatas = null;

        public List<DistItemSeq>? DisplayDistItemDatas
        {
            get => _displaydistitemDatas;
            set => SetProperty(ref _displaydistitemDatas, value);
        }

        private DistItemSeq? _currentdistitemseq;

        public DistItemSeq? CurrentDistItemSeq
        {
            get => _currentdistitemseq;
            set
            {
                SetProperty(ref _currentdistitemseq, value);
                CanDistItem = value == null || value.GetUniqueItemKey == "" ? false : true;
                CanDistItemDetail = value == null || value.GetUniqueItemKey == "" ? false : true;
            }
        }

        private bool _candistitem = false;
        public bool CanDistItem
        {
            get => _candistitem;
            set
            {
                if (CanInfo == true)
                    value = false;

                SetProperty(ref _candistitem, value);
            }
        }

        private bool _candistitemdetail = false;
        public bool CanDistItemDetail
        {
            get => _candistitemdetail;
            set => SetProperty(ref _candistitemdetail, value);
        }

        private bool _isCheck = false;
        public bool IsCheck
        {
            get => _isCheck;
            set => SetProperty(ref _isCheck, value);
        }

        private bool _isExtraction = false;
        public bool IsExtraction
        {
            get => _isExtraction;
            set => SetProperty(ref _isExtraction, value);
        }

        private bool _isWorking = false;
        public bool IsWorking
        {
            get => _isWorking;
            set => SetProperty(ref _isWorking, value);
        }

        private bool _pagelock = false;

        public bool PageLock
        {
            get => _pagelock;
            set => SetProperty(ref _pagelock, value);
        }
        private int _selectitemidx = 0;

        public int SelectItemIdx
        {
            get => _selectitemidx;
            set => SetProperty(ref _selectitemidx, value);
        }

        private SolidColorBrush _colorBack = new SolidColorBrush(Colors.Gray);
        public SolidColorBrush ColorBack
        {
            get => _colorBack;
            set => SetProperty(ref _colorBack, value);
        }
        private SolidColorBrush _colorText = new SolidColorBrush(Colors.Gray);
        public SolidColorBrush ColorText
        {
            get => _colorText;
            set => SetProperty(ref _colorText, value);
        }

        private Shain _selectedshain = new Shain();
        public Shain SelectedShain
        {
            get => _selectedshain;
            set => SetProperty(ref _selectedshain, value);
        }

        private bool _canInfo = false;
        public bool CanInfo
        {
            get => _canInfo;
            set => SetProperty(ref _canInfo, value);
        }

        private bool _isenablescan = true;
        public bool IsEnableScan
        {
            get => _isenablescan;
            set => SetProperty(ref _isenablescan, value);
        }

        private DistColorInfo? _distcolorinfo = null;
        public DistColorInfo? DistColorInfo
        {
            get => _distcolorinfo;
            set => SetProperty(ref _distcolorinfo, value);
        }

        private string _errMsg = string.Empty;
        public string ErrMsg
        {
            get => _errMsg;
            set => SetProperty(ref _errMsg, value);
        }

        private readonly IDialogService _dialogService;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        private DistGroupEx _distgroup;
        public DistGroupEx DistGroup
        {
            get => _distgroup;
            set => SetProperty(ref _distgroup, value);
        }
        public TdDpsManager? TdDps { get; set; } = null;

        private OrderingLockInstance lockInstance = new OrderingLockInstance();

        public DistItemScanWindowViewModel(IRegionManager regionManager, IDialogService dialogService, IEventAggregator eventAggregator, DistColorInfo distcolorinfo, DistGroupEx distgroup, TdDpsManager tddps)
        {
            _dialogService = dialogService;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            DistColorInfo = distcolorinfo;
            _distgroup = distgroup;
            TdDps = tddps;

            OnBack = new DelegateCommand(() =>
            {
                Syslog.Info("【戻る】DistItemScanWindowModel:OnBack");

                DisplayDistItemDatas = null;
                _regionManager.Regions["ContentRegion"].NavigationService.Journal.GoBack();
            });

            OnStop = new DelegateCommand(() =>
            {
                Syslog.Info($"【中断】DistItemScanWindowModel:OnStop");

                DisplayDistItemDatas = null;

                DistColor? disstcolor = _distcolorinfo?.GetDistColor(Color);
                if (disstcolor != null)
                {
                    Task.Run(() =>
                    {
                        _lockLight.EnterWriteLock();
                        Syslog.Info($"【中断】DistItemScanWindowModel:OnStop:{disstcolor.DistColor_name}");
                        try
                        {
                            bool bRet = TdUnitManager.TdLightOff(ref disstcolor, TdDps, distcolorinfo);
                            // 作業報告書終了
                            disstcolor.ReportEnd();
                            distcolorinfo!.ReportUpdate(disstcolor.ReportShain, disstcolor.DistWorkMode);

                            DistColorManager.DistUpdate(disstcolor);
                        }
                        finally
                        {
                            _lockLight.ExitWriteLock();
                        }
                    });
                }

                _regionManager.Regions["ContentRegion"].NavigationService.Journal.GoBack();
            }, () => CanInfo).ObservesProperty(() => CanInfo);

            OnCancel = new DelegateCommand(() =>
            {
                Syslog.Info("【ｷｬﾝｾﾙ】DistItemScanWindowModel:OnCancel");

                DisplayDistItemDatas = null;

                Task.Run(() =>
                {
                    DistColor? disstcolor = _distcolorinfo?.GetDistColor(Color);
                    if (disstcolor != null)
                    {
                        _lockLight.EnterWriteLock();
                        Syslog.Info($"【ｷｬﾝｾﾙ】DistItemScanWindowModel:OnStop:{disstcolor.DistColor_name}");
                        try
                        {
                            bool bRet = TdUnitManager.TdLightOff(ref disstcolor, TdDps, distcolorinfo);
                            // 作業報告書終了
                            disstcolor.ReportEnd();
                            distcolorinfo!.ReportUpdate(disstcolor.ReportShain, disstcolor.DistWorkMode);

                            disstcolor.DistColorClear();
                        }
                        finally
                        {
                            _lockLight.ExitWriteLock();
                        }
                    }
                });
                _regionManager.Regions["ContentRegion"].NavigationService.Journal.GoBack();
            }, () => CanInfo).ObservesProperty(() => CanInfo);


            OnItemCancel = new DelegateCommand(() =>
            {
                Syslog.Info("【商品取り消し】DistItemScanWindowModel:OnItemCancel");

                if (CurrentDistItemSeq != null)
                {
                    if (CurrentDistItemSeq.CdHimban != "")
                    {
                        for (int i = SelectItemIdx; i < DistBase.ITEMMAX - 1; i++)
                        {
                            if (DisplayDistItemDatas!.Count() >= i)
                            {
                                SetItemRow(i, DisplayDistItemDatas![i + 1]);
                            }
                        }
                        InSeq--;
                        // 空白設定
                        SetItemRow(DistBase.ITEMMAX - 1, new DistItemSeq());
                    }
                }

            }, () => CanDistItem).ObservesProperty(() => CanDistItem);

            OnShowDetailInfo = new DelegateCommand(() =>
            {
                Syslog.Info("【明細】DistItemScanWindowModel:OnShowDetailInfo");

                _regionManager.RequestNavigate("ContentRegion"
                    , nameof(Views.DistDetailWindow)
                    , new NavigationParameters
                    {
                        { "currentdistinfo", ItemToInfo(CurrentDistItemSeq!) },
                        { "Color", Color},
                        { "ItemSeq", CurrentDistItemSeq},
                        { "CanInfo", CanInfo },
                    }
                );

            }, () => CanDistItemDetail).ObservesProperty(() => CanDistItemDetail);

            OnChangeQty = new DelegateCommand(() =>
            {
                Syslog.Info("【数量変更】DistItemScanWindowModel:OnChangeQty");

                // 検品、抜き取りは数量訂正出来ない
                if (IsCheck == true || IsExtraction == true)
                {
                    string msg = string.Format("{0}処理のため数量訂正出来ません", IsCheck ? "検品" : "抜き取り");
                    MessageDialog.Show(_dialogService, msg, "エラー");
                    return;
                }

                dialogService.ShowDialog(
                    nameof(ChangeQtyDlg),
                    new DialogParameters
                    {
                        { "CurrentDistDetail", ItemToDetail(CurrentDistItemSeq!) },
                        { "TokuisakiTotal", 1 },
                    },
                    (rc) =>
                    {
                        if (rc.Result == ButtonResult.OK)
                        {
                            var distDetail = rc.Parameters.GetValue<DistDetail>("DistDetail");
                            if (distDetail != null)
                            {
                                try
                                {
                                    QtyChange(distDetail);
                                }
                                catch (Exception e)
                                {
                                    Syslog.Info($"DistDetailWindowViewModel::OnChangeQty{e.ToString()}");
                                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                                }
                            }
                            RefreshInfo();
                        }
                    });
            }, () => CanDistItem).ObservesProperty(() => CanDistItem);

            OnChangeBoxUnit = new DelegateCommand(() =>
            {
                Syslog.Info("【箱数変更】DistItemScanWindowModel:OnChangeBoxUnit");

                dialogService.ShowDialog(
                    nameof(ModifyBoxUnitDialog),
                    new DialogParameters
                    {
                        { "CurrentDistDetail", ItemToDetail(CurrentDistItemSeq!) },
                    },
                    (rc) =>
                    {
                        if (rc.Result == ButtonResult.OK)
                        {
                            var distDetail = rc.Parameters.GetValue<DistDetail>("DistDetail");
                            if (distDetail != null)
                            {
                                try
                                {
                                    BoxUnitChange(distDetail);
                                }
                                catch (Exception e)
                                {
                                    Syslog.Info($"DistDetailWindowViewModel::OnChangeBoxUnit{e.ToString()}");
                                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                                }
                            }
                            RefreshInfo();
                        }
                    });
            }, () => CanDistItem).ObservesProperty(() => CanDistItem);

            OnEnter = new DelegateCommand(() =>
            {
                ErrMsg = string.Empty;

                try
                {
                    Syslog.Info($"【商品決定】DistItemScanWindowModel:OnEnter scancode=[{Scancode}]");

                    if (Scancode.Count()==0)
                        return;

                    // ９桁未満は０埋めし９桁へ変更
                    if (Scancode.Count() < 9)
                    {
                        Scancode = Scancode.PadLeft(9, '0');
                    }
 

                    if (InSeq == DistBase.ITEMMAX)
                    {
                        ErrMsg = "登録出来るのは９商品までです。";
//                        MessageDialog.Show(_dialogService, "登録出来るのは９商品までです。", "エラー");
                        return;
                    }
                    var selectitems = DistColorManager.GetItems(DistGroup!, Scancode, IsCheck, IsExtraction);

                    if (selectitems != null)
                    {
                        if (selectitems.Count == 1)
                        {
                            // 商品チェック
                            CheckItem(selectitems[0]);

                            // 商品設定
                            SetItemRow(InSeq, selectitems[0]);
                            InSeq++;
                        }
                        else
                        {
                            // 商品重複
                            dialogService.ShowDialog(
                                nameof(SelectItemDlg),
                                new DialogParameters
                                {
                                { "scancode", Scancode },
                                { "selectitems", selectitems },
                                { "distcolor", _distcolorinfo?.GetDistColor(Color) },
                            },
                                (rc) =>
                                {
                                    if (rc.Result == ButtonResult.OK)
                                    {
                                        try
                                        {

                                            var selectitem = rc.Parameters.GetValue<DistItemSeq>("selectitem");
                                            var selectitemidx = rc.Parameters.GetValue<int>("selectitemidx");

                                            if (selectitem != null)
                                            {
                                                // 商品チェック
                                                CheckItem(selectitem);

                                                // 商品設定
                                                SetItemRow(InSeq, selectitem);
                                                InSeq++;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Syslog.Error($"DistItemScanWindowViewModel:OnEnter:{e.Message}");
                                            //MessageDialog.Show(_dialogService, e.Message, "エラー");
                                            ErrMsg = e.Message;

                                        }
                                    }
                                });
                        }
                    }
                }
                catch (Exception e)
                {
                    Syslog.Error($"DistItemScanWindowViewModel:OnEnter:{e.Message}");
                    ErrMsg = e.Message;
//                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                }
                Scancode = "";
            });

            OnStart = new DelegateCommand(() =>
            {
                Syslog.Info($"【配分開始】DistItemScanWindowModel:OnStart:color:{Color}");

                if (InSeq!=0)
                {
                    DistColor? distcolor = _distcolorinfo?.GetDistColor(Color);

                    if (distcolor != null)
                    {
                        distcolor.ItemSeqs = DisplayDistItemDatas!;
                        distcolor.CdShain = SelectedShain.CdShain;
                        distcolor.NmShain = SelectedShain.NmShain;
                        distcolor.DistWorkMode = IsCheck ? (int)DistWorkMode.Check : IsExtraction ? (int)DistWorkMode.Extraction : (int)DistWorkMode.Dist;

                        // 一斉配分の場合は順序を設定
                        if (distcolorinfo.IsDistWorkNormal==true)
                        {
                            int distseq = ++_distcolorinfo!.DistSeq;
                            for (int i=0; i<distcolor.DistSeqs.Count();i++)
                                distcolor.DistSeqs[i] = distseq;
                        }

                        // 明細データ読み込み
                        for (int i = 0; i < DistBase.ITEMMAX; i++)
                        {
                            if (distcolor.ItemSeqs[i] != null)
                            {
                                Syslog.Info($"【配分開始商品】 :distseq={distcolor.DistSeqs[0]} itemseq:{i} item:{distcolor.ItemSeqs[i].CdHimban} {distcolor.ItemSeqs[i].NmHinSeishikimei}");
                                distcolor.ItemSeqs[i].Details = DistColorManager.LoadInfoDetails(DistGroup!, distcolor.ItemSeqs[i]);
                            }
                        }
                        DisplayDistItemDatas = null;

                        Task.Run(() =>
                        {
                            using (new OrderingLock(lockInstance))
                            {
                                Syslog.Info($"【点灯開始】DistItemScanWindowModel:TdLight:{distcolor.DistColor_name}");

                                bool bRet = TdUnitManager.TdLight(ref distcolor, distcolorinfo.IsDistWorkNormal, TdDps);

                                int dops = distcolor.ItemSeqs.Sum(x => x.Dops);
                                if (dops == 0)
                                {
                                    // 即座に完了処理へ
                                    DistColorManager.DistUpdate(distcolor);
                                }
                                else
                                {
                                    // 作業報告書開始
                                    distcolor.ReportStart(SelectedShain, distcolor.Distitem_cnt, distcolor.DistWorkMode);

                                    UpdateProgress();
                                }
                            }
                        });

                        _regionManager.Regions["ContentRegion"].NavigationService.Journal.GoBack();
                    }
                }
                else
                {
                    MessageDialog.Show(_dialogService, "作業する商品が設定されていません。", "エラー");
                }
            });

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Syslog.Info($"DistItemScanWindowViewModel:OnNavigatedFrom");
            return;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Scancode = "";
            ErrMsg = string.Empty;

            if (DisplayDistItemDatas == null)
            {
                Color = navigationContext.Parameters.GetValue<int>("color");
                IsCheck = navigationContext.Parameters.GetValue<bool>("ischeck");
                IsExtraction = navigationContext.Parameters.GetValue<bool>("isextraction");
                IsWorking = navigationContext.Parameters.GetValue<bool>("isworking");
                SelectedShain = navigationContext.Parameters.GetValue<Shain>("SelectedShain");

                ColorText = TdColor.GetColorText(Color);
                ColorBack = TdColor.GetColorBack(Color);

                DisplayDistItemDatas = new List<DistItemSeq>();
                for (int i = 0; i < DistBase.ITEMMAX; i++)
                {
                    DisplayDistItemDatas.Add(new DistItemSeq { InSeq = i + 1 });
                }

                // 新規 or 参照
                DistColor? disstcolor = _distcolorinfo?.GetDistColor(Color);
                if (disstcolor != null)
                {
                    CanInfo = disstcolor.IsWorking();
                    IsEnableScan = disstcolor.IsWorking() == true ? false : true;
                }

                // 画面表示
                RefreshInfo();

                CanDistItem = false;
                InSeq = 0;
            }
            else
            {
                for (int i = 0; i < DistBase.ITEMMAX; i++)
                {
                    var item = DisplayDistItemDatas[i];
                    if (item.CdHimban != "")
                    {
                        RefreshItem(item);
                    }
                }
            }

            Syslog.Info($"DistItemScanWindowViewModel:OnNavigatedTo");
        }

        public void RefreshInfo()
        {
            try
            {
                DistColor? disstcolor = _distcolorinfo?.GetDistColor(Color);
                if (disstcolor != null)
                {
                    if (IsEnableScan)
                    {
                        // 内容コピー
                        if (disstcolor.ItemSeqs.Count() != 0)
                        {
                            for (int i = 0; i < DistBase.ITEMMAX; i++)
                            {
                                SetItemRow(i, disstcolor.ItemSeqs[i]);
                            }
                        }
                    }
                    else
                    {
                        // 参照
                        DisplayDistItemDatas = disstcolor.ItemSeqs;
                    }
                }
            }
            catch (Exception e)
            {
                Syslog.Error($"DistItemScanWindowViewModel:RefreshInfo:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }

        void SetItemRow(int inseq, DistItemSeq distitemseq)
        {
            Syslog.Info($"DistItemScanWindowViewModel:SetItemRow:itemseq:{inseq} item:{distitemseq.CdHimban} {distitemseq.NmHinSeishikimei} Ops:{distitemseq.Ops} Rps:{distitemseq.Drps} OShop:{distitemseq.Order_shop_cnt} RShop:{distitemseq.Result_shop_cnt}");

            DisplayDistItemDatas![inseq].DtDelivery = distitemseq.DtDelivery;
            DisplayDistItemDatas![inseq].CdShukkaBatch = distitemseq.CdShukkaBatch;
            DisplayDistItemDatas![inseq].CdJuchuBin = distitemseq.CdJuchuBin;
            DisplayDistItemDatas![inseq].CdHimban = distitemseq.CdHimban;
            DisplayDistItemDatas![inseq].NmHinSeishikimei = distitemseq.NmHinSeishikimei;
            DisplayDistItemDatas![inseq].CdGtin13 = distitemseq.CdGtin13;
            DisplayDistItemDatas![inseq].CdGtin14 = distitemseq.CdGtin14;
            DisplayDistItemDatas![inseq].StBoxType = distitemseq.StBoxType;
            DisplayDistItemDatas![inseq].NuBoxUnit = distitemseq.NuBoxUnit;
            DisplayDistItemDatas![inseq].Csunit = distitemseq.Csunit;
            DisplayDistItemDatas![inseq].DStatus = distitemseq.DStatus;
            DisplayDistItemDatas![inseq].Ops = distitemseq.Ops;
            DisplayDistItemDatas![inseq].Dops = distitemseq.Dops;
            DisplayDistItemDatas![inseq].Drps = distitemseq.Drps;
            DisplayDistItemDatas![inseq].Ddps = distitemseq.Ddps;
            DisplayDistItemDatas![inseq].Lrps = distitemseq.Lrps;
            DisplayDistItemDatas![inseq].Order_shop_cnt = distitemseq.Order_shop_cnt;
            DisplayDistItemDatas![inseq].Result_shop_cnt = distitemseq.Result_shop_cnt;
            DisplayDistItemDatas![inseq].Remain_shop_cnt = distitemseq.Remain_shop_cnt;
            DisplayDistItemDatas![inseq].Left_shop_cnt = distitemseq.Left_shop_cnt;
            DisplayDistItemDatas![inseq].Right_shop_cnt = distitemseq.Right_shop_cnt;
            DisplayDistItemDatas![inseq].Left_ps_cnt = distitemseq.Left_ps_cnt;
            DisplayDistItemDatas![inseq].Right_ps_cnt = distitemseq.Right_ps_cnt;

            CanDistItem = CurrentDistItemSeq == null || CurrentDistItemSeq.GetUniqueItemKey == "" ? false : true;
        }
        DistInfo ItemToInfo(DistItemSeq distitemseq)
        {
            return new DistInfo
            {
                DtDelivery = distitemseq.DtDelivery,
                CdShukkaBatch = distitemseq.CdShukkaBatch,
                CdJuchuBin = distitemseq.CdJuchuBin,
                CdHimban = distitemseq.CdHimban,
                NmHinSeishikimei = distitemseq.NmHinSeishikimei,
                CdGtin13 = distitemseq.CdGtin13,
                CdGtin14 = distitemseq.CdGtin14,
                StBoxType = distitemseq.StBoxType,
                NuBoxUnit = distitemseq.NuBoxUnit,
                Csunit = distitemseq.Csunit,
                DStatus = distitemseq.DStatus,
                Ops = distitemseq.Ops,
                Dops = distitemseq.Dops,
                Drps = distitemseq.Drps,
                Ddps = distitemseq.Ddps,
                Lrps = distitemseq.Lrps,
                Order_shop_cnt = distitemseq.Order_shop_cnt,
                Result_shop_cnt = distitemseq.Result_shop_cnt,
                Remain_shop_cnt = distitemseq.Remain_shop_cnt,
                Left_shop_cnt = distitemseq.Left_shop_cnt,
                Right_shop_cnt = distitemseq.Right_shop_cnt,
                Left_ps_cnt = distitemseq.Left_ps_cnt,
                Right_ps_cnt = distitemseq.Right_ps_cnt,
            };
        }

        DistDetail ItemToDetail(DistItemSeq distitemseq)
        {
            return new DistDetail
            {
                DtDelivery = distitemseq.DtDelivery,
                CdShukkaBatch = distitemseq.CdShukkaBatch,
                CdJuchuBin = distitemseq.CdJuchuBin,
                CdHimban = distitemseq.CdHimban,
                NmHinSeishikimei = distitemseq.NmHinSeishikimei,
                CdGtin13 = distitemseq.CdGtin13,
                CdGtin14 = distitemseq.CdGtin14,
                StBoxType = distitemseq.StBoxType,
                NuBoxUnit = distitemseq.NuBoxUnit,
                Csunit = distitemseq.Csunit,
                DStatus = distitemseq.DStatus,
                Ops = distitemseq.Ops,
                Dops = distitemseq.Dops,
                Drps = distitemseq.Drps,
                Ddps = distitemseq.Ddps,
                Lrps = distitemseq.Lrps,
                Order_shop_cnt = distitemseq.Order_shop_cnt,
                Result_shop_cnt = distitemseq.Result_shop_cnt,
                Remain_shop_cnt = distitemseq.Remain_shop_cnt,
                Left_shop_cnt = distitemseq.Left_shop_cnt,
                Right_shop_cnt = distitemseq.Right_shop_cnt,
                Left_ps_cnt = distitemseq.Left_ps_cnt,
                Right_ps_cnt = distitemseq.Right_ps_cnt,
            };
        }


        void CheckItem(DistItemSeq distitemseq)
        {
            // 選択商品が作業中かチェックする
            for (int i = 0; i < DistBase.ITEMMAX; i++)
            {
                if (DisplayDistItemDatas![i].GetUniqueItemKey == distitemseq.GetUniqueItemKey)
                {
                    throw new Exception($"選択済みの商品です。");
                }
            }

            foreach(var distcolor in _distcolorinfo!.DistColors!)
            {
                for (int i = 0; i < DistBase.ITEMMAX; i++)
                {
                    if (distcolor!.ItemSeqs.Count() == DistBase.ITEMMAX && distcolor!.ItemSeqs[i].GetUniqueItemKey == distitemseq.GetUniqueItemKey)
                    {
                        throw new Exception($"{distcolor.DistColor_name}で作業中の商品です。");
                    }
                }
            }
        }
        void QtyChange(DistDetail distdetail)
        {
            List<DistDetail>? details = DistColorManager.LoadInfoDetails(DistGroup!, distdetail);


            if(details != null)
            {
                // 残数を昇順に設定
                int ops = distdetail.Dops - distdetail.Drps;
                details.ForEach(d =>
                {
                    int zan = d.Lrps - d.Drps;

                    if (ops < zan)
                        zan = ops;

                    d.Dops = zan + d.Drps;
                    ops -= zan;
                });
                DistColorManager.UpdateQtyDetail(details);

                RefreshItem(distdetail);
            }
        }

        void BoxUnitChange(DistDetail distdetail)
        {
            List<DistDetail>? details = DistColorManager.LoadInfoDetails(DistGroup!, distdetail);

            if (details != null)
            {
                details.ForEach(d =>
                {
                    d.NuBoxUnit = distdetail.NuBoxUnit;
                    d.Csunit = distdetail.NuBoxUnit;
                });


                DistColorManager.UpdateBoxUnit(details);

                RefreshItem(distdetail);
            }
        }


        private void RefreshItem(DistBase distdetail)
        {
            var refreshitem = DistColorManager.RefreshItems(DistGroup!, distdetail, IsCheck, IsExtraction);

            if (CurrentDistItemSeq != null)
            {
                if (refreshitem != null)
                {
                    CurrentDistItemSeq.NuBoxUnit = refreshitem.NuBoxUnit;
                    CurrentDistItemSeq.Csunit = refreshitem.NuBoxUnit;
                    CurrentDistItemSeq.Left_ps_cnt = refreshitem.Left_ps_cnt;
                    CurrentDistItemSeq.Right_ps_cnt = refreshitem.Right_ps_cnt;
                    CurrentDistItemSeq.Ops = refreshitem.Ops;
                    CurrentDistItemSeq.Dops = refreshitem.Dops;
                    CurrentDistItemSeq.Drps = refreshitem.Drps;
                    CurrentDistItemSeq.Ddps = refreshitem.Ddps;
                    CurrentDistItemSeq.Remain_shop_cnt = refreshitem.Remain_shop_cnt;
                }
                else
                {
                    CurrentDistItemSeq.NuBoxUnit = 0;
                    CurrentDistItemSeq.Dops = 0;
                    CurrentDistItemSeq.Ddps = 0;
                    CurrentDistItemSeq.Remain_shop_cnt = 0;
                }
            }
        }

        private void UpdateProgress()
        {
            try
            {
                if (DistGroup != null)
                {
                    ProgressCnt? progcnt = DistProgressManager.GetProgressCnts(DistGroup);
                    if (progcnt != null)
                    {
                        DistProgressManager.UpdateDistProgress(DistGroup, DistColorInfo!, progcnt);
                    }
                }
            }
            catch (Exception e)
            {
                Syslog.Error($"UpdateProgress:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }
    }
}
