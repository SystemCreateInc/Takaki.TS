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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
                CanDistDetail = value == null ? false : true;
            }
        }

        private bool _candistdetail = false;

        public bool CanDistDetail
        {
            get => _candistdetail;
            set => SetProperty(ref _candistdetail, value);
        }


        private DistInfo _base_distinfo = new();


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

            DisplayDistDetailDatas = new List<DistDetail>();
            RefreshInfo();

            Syslog.Info($"DistDetailViewModel:OnNavigatedTo");
        }
        public void RefreshInfo()
        {
            try
            {
                if (_base_distinfo != null)
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
#if false
                    // ゾーン明細からの一覧
                    DisplayDistDetailDatas = DistColorManager.LoadDetails(ref _base_distzone);
                    if (DisplayDistDetailDatas == null)
                    {
                        DisplayDistDetailDatas = new List<DistDetail>();
                    }
                    else
                    {
                        HeaderMsgText = string.Format($"ｿﾞｰﾝ:{_base_distzone.Zoneno} 作業No:{_base_distzone.Workno}\n店舗:{_base_distzone.CD_TOKUISAKI} {_base_distzone.NM_TOKUISAKI} 梱包箱No:{_base_distzone.Packno}");
                    }
#endif
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
