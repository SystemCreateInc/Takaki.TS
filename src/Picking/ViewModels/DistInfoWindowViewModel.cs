using LogLib;
using Picking.Models;
using Picking.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using SelDistGroupLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TdDpsLib.Models;

namespace Picking.ViewModels
{
    public class DistInfoWindowViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand OnRefresh { get; }
        public DelegateCommand OnShowDetailInfo { get; }
        public DelegateCommand OnBack { get; }

        private List<DistInfo>? _displaydistinfoDatas = new List<DistInfo>();

        public List<DistInfo>? DisplayDistInfoDatas
        {
            get => _displaydistinfoDatas;
            set => SetProperty(ref _displaydistinfoDatas, value);
        }

        private DistInfo? _currentDistInfo;

        public DistInfo? CurrentDistInfo
        {
            get => _currentDistInfo;
            set
            {
                SetProperty(ref _currentDistInfo, value);
                CanDistInfo = value == null ? false : true;
            }
        }

        private bool _candistinfo = false;

        public bool CanDistInfo
        {
            get => _candistinfo;
            set => SetProperty(ref _candistinfo, value);
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

        public DistInfoWindowViewModel(IRegionManager regionManager, IDialogService dialogService, IEventAggregator eventAggregator, DistColorInfo distcolorinfo, DistGroupEx distgroup)
        {
            _dialogService = dialogService;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _distcolorinfo = distcolorinfo;
            _distgroup = distgroup;

            OnBack = new DelegateCommand(() =>
            {
                Syslog.Info("【戻る】DistnfoViewModal:OnBack");

                _regionManager.Regions["ContentRegion"].NavigationService.Journal.GoBack();
            });

            OnRefresh = new DelegateCommand(() =>
            {
                Syslog.Info("【更新】DistnfoViewModal:OnRefresh");

                RefreshInfo();
            });

            OnShowDetailInfo = new DelegateCommand(() =>
            {
                Syslog.Info("【明細】DistnfoViewModal:OnShowDetailInfo");

                _regionManager.RequestNavigate("ContentRegion", nameof(Views.DistDetailWindow), new NavigationParameters
                {
                    {
                        "currentdistinfo", CurrentDistInfo
                    },
                });

            }, () => CanDistInfo).ObservesProperty(() => CanDistInfo);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Syslog.Info($"DistnfoViewModal:OnNavigatedFrom");
            return;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            DisplayDistInfoDatas = new List<DistInfo>();
            CurrentDistInfo = null;

            // 画面表示
            RefreshInfo();

            Syslog.Info($"DistnfoViewModal:OnNavigatedTo");
        }

        public void RefreshInfo()
        {
            try
            {
                DisplayDistInfoDatas = DistColorManager.LoadInfs(_distgroup!);
                PackCnt = DistProgressManager.GetProgressCnts(DistGroup!);
                if (DisplayDistInfoDatas == null)
                {
                    DisplayDistInfoDatas = new List<DistInfo>();
                }
            }
            catch (Exception e)
            {
                Syslog.Info($"DistInfoWindowViewModel::RefreshInfo{e.ToString()}");
                MessageBox.Show(e.Message, "エラー");
            }
        }
    }
}
