using ControlzEx.Standard;
using LogLib;
using Picking.Models;
using Picking.Services;
using Picking.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using SelDistGroupLib.Models;
using SelDistGroupLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WindowLib.Utils;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Picking.ViewModels
{
    public class DistDetailWindowViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand OnRefresh { get; }
        public DelegateCommand OnChangeQty { get; }
        public DelegateCommand OnBack { get; }

        private string _headermsgtext = "";
        public string HeaderMsgText
        {
            get => _headermsgtext;
            set => SetProperty(ref _headermsgtext, value);
        }

        private List<DistDetail>? _displaydistdetailDatas = new List<DistDetail>();

        public List<DistDetail>? DisplayDistDetailDatas
        {
            get => _displaydistdetailDatas;
            set => SetProperty(ref _displaydistdetailDatas, value);
        }

        private DistDetail? _currentDistDetail;

        public DistDetail? CurrentDistDetail
        {
            get => _currentDistDetail;
            set
            {
                SetProperty(ref _currentDistDetail, value);
                CanDistDetail = value == null || CanInfo==true ? false : true;
            }
        }

        private bool _candistdetail = false;

        public bool CanDistDetail
        {
            get => _candistdetail;
            set => SetProperty(ref _candistdetail, value);
        }


        private DistInfo _base_distinfo = new();

        private int _color = 0;
        public int Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }
        private DistItemSeq? _itemseq = null;
        public DistItemSeq? ItemSeq
        {
            get => _itemseq;
            set => SetProperty(ref _itemseq, value);
        }
        private bool _canInfo = false;
        public bool CanInfo
        {
            get => _canInfo;
            set => SetProperty(ref _canInfo, value);
        }


        private readonly IDialogService _dialogService;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        private DistColorInfo? _distcolorinfo = null;
        private DistGroupEx? _distgroup = null;
        public DistGroupEx? DistGroup
        {
            get => _distgroup;
            set => SetProperty(ref _distgroup, value);
        }

        public DistDetailWindowViewModel(IRegionManager regionManager, IDialogService dialogService, IEventAggregator eventAggregator, DistColorInfo distcolorinfo, DistGroupEx distgroup)
        {
            _dialogService = dialogService;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _distcolorinfo = distcolorinfo;
            _distgroup = distgroup;

            OnRefresh = new DelegateCommand(() =>
            {
                Syslog.Info("【更新】DistDetailViewModel:OnRefresh");

                RefreshInfo();
            });

            OnBack = new DelegateCommand(() =>
            {
                Syslog.Info("【戻る】DistDetailViewModel:OnBack");

                regionManager.Regions["ContentRegion"].NavigationService.Journal.GoBack();
            });

            OnChangeQty = new DelegateCommand(() =>
            {
                Syslog.Info("【数量変更】DistDetailViewModel:OnChangeQty");

                if (CurrentDistDetail?.Dops== CurrentDistDetail?.Drps)
                {
                    MessageDialog.Show(_dialogService, "完了しているので数量変更出来ません", "エラー");
                    return;
                }
                if (CurrentDistDetail?.LStatus == (int)DbLib.Defs.Status.Ready)
                {
                    MessageDialog.Show(_dialogService, "大仕分けされていなので数量変更出来ません", "エラー");
                    return;
                }


                dialogService.ShowDialog(
                    nameof(ChangeQtyDlg),
                    new DialogParameters
                    {
                        { "CurrentDistDetail", CurrentDistDetail },
                        { "TokuisakiTotal", 0 },
                    },
                    (rc) =>
                    {
                        if (rc.Result == ButtonResult.OK)
                        {
                            var DistDetail = rc.Parameters.GetValue<DistDetail>("DistDetail");
                            if (DistDetail != null)
                            {
                                try
                                {
                                    List<DistDetail> Details = new List<DistDetail>();
                                    Details.Add(DistDetail);

                                    DistColorManager.UpdateQtyDetail(Details);
                                }
                                catch (Exception e)
                                {
                                    Syslog.Info($"DistDetailWindowViewModel::OnChangeQty{e.ToString()}");
                                    MessageBox.Show(e.Message, "エラー");
                                }
                            }
                            RefreshInfo();
                        }
                    });

            }, () => CanDistDetail).ObservesProperty(() => CanDistDetail);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Syslog.Info($"DistDetailViewModel:OnNavigatedFrom");
            return;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _base_distinfo = navigationContext.Parameters.GetValue<DistInfo>("currentdistinfo");
            Color = navigationContext.Parameters.GetValue<int>("Color");
            ItemSeq = navigationContext.Parameters.GetValue<DistItemSeq>("ItemSeq");
            CanInfo = navigationContext.Parameters.GetValue<bool>("CanInfo");

            DisplayDistDetailDatas = new List<DistDetail>();
            RefreshInfo();

            Syslog.Info($"DistDetailViewModel:OnNavigatedTo");
        }
        public void RefreshInfo()
        {
            try
            {
                if (CanInfo == false)
                {
                    // 状況からの一覧
                    DisplayDistDetailDatas = DistColorManager.LoadInfoDetails(_distgroup!, _base_distinfo);
                    if (DisplayDistDetailDatas == null)
                    {
                        DisplayDistDetailDatas = new List<DistDetail>();
                    }
                    else
                    {
                        HeaderMsgText = string.Format($"品番:{_base_distinfo.CdHimban} {_base_distinfo.CdGtin13} {_base_distinfo.NmHinSeishikimei} 箱入数:{_base_distinfo.NuBoxUnit}");
                    }
                }
                else
                {
                    // 配分明細からの一覧
                    if (ItemSeq != null)
                    {
                        DisplayDistDetailDatas = ItemSeq.Details;

                        if (DisplayDistDetailDatas == null)
                        {
                            DisplayDistDetailDatas = new List<DistDetail>();
                        }
                        else
                        {
                            HeaderMsgText = string.Format($"品番:{_base_distinfo.CdHimban} {_base_distinfo.CdGtin13} {_base_distinfo.NmHinSeishikimei} 箱入数:{_base_distinfo.NuBoxUnit}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Syslog.Info($"DistDetailWindowViewModel::RefreshInfo{e.ToString()}");
                MessageBox.Show(e.Message, "エラー");
            }
        }
    }
}
