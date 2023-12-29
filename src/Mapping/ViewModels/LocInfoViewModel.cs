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
using Prism.Events;
using Prism.Regions;
using System.Printing;
using Mapping.Reports;
using PrintPreviewLib;
using PrintLib;
using DbLib.Defs;

namespace Mapping.ViewModels
{
    public class LocInfoViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand OnPrint { get; }
        public DelegateCommand OnBack { get; }

        private string _cdDistGroup = string.Empty;
        public string CdDistGroup
        {
            get => _cdDistGroup;
            set => SetProperty(ref _cdDistGroup, value);
        }

        private string _nmDistGroup = string.Empty;
        public string NmDistGroup
        {
            get => _nmDistGroup;
            set => SetProperty(ref _nmDistGroup, value);
        }
        private string _dtDelivery = string.Empty;
        public string DtDelivery
        {
            get => _dtDelivery;
            set => SetProperty(ref _dtDelivery, value);
        }


        private int _tokuisakiCnt = 0;
        public int TokuisakiCnt
        {
            get => _tokuisakiCnt;
            set => SetProperty(ref _tokuisakiCnt, value);
        }
        private ObservableCollection<Models.LocInfo> _locinfos = new ObservableCollection<Models.LocInfo>();
        public ObservableCollection<Models.LocInfo> LocInfos
        {
            get => _locinfos;
            set => SetProperty(ref _locinfos, value);
        }

        private DistGroupInfo? _distgroupinfo = null;
        private MappingManager? _mapping = null;

        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;

        public LocInfoViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;

            OnPrint = new DelegateCommand(() =>
            {
                Syslog.Debug("LocInfoViewModel:OverPrint");

                try
                {
                    var locs = LocInfos.ToList();

                    if (locs == null || locs.Count == 0)
                    {
                        return;
                    }

                    var viewModel = ReportCreator.GetLocList(locs,CdDistGroup,NmDistGroup,DtDelivery);
#if DEBUG
                    var ppm = new PrintPreviewManager(PageMediaSizeName.ISOA4, PageOrientation.Landscape);
                    ppm.PrintPreview("ロケーション一覧一覧", viewModel);
#else
                    var ppm = new PrintManager(PageMediaSizeName.ISOA4, PageOrientation.Landscape);
                    ppm.Print("ロケーション一覧一覧", viewModel);
#endif
                }
                catch (Exception e)
                {
                    Syslog.Error($"LocInfoViewModel:OverPrint:{e.Message}");
                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                }
            });


            OnBack = new DelegateCommand(() =>
            {
                Syslog.Info("【戻る】LocInfoViewModel:OnBack");

                regionManager.Regions["ContentRegion"].NavigationService.Journal.GoBack();
            });

        }
        private void Closing(object? sender, CancelEventArgs e)
        {
        }
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Syslog.Info($"LocInfoViewModel:OnNavigatedFrom");
            return;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _mapping = navigationContext.Parameters.GetValue<MappingManager>("Mapping");
            _distgroupinfo = navigationContext.Parameters.GetValue<DistGroupInfo>("currentdistinfo");

            try
            {
                CollectionViewHelper.SetCollection(LocInfos, LoadDatas());
            }
            catch (Exception e)
            {
                Syslog.Error($"LocInfoViewModel:LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }

            Syslog.Info($"LocInfoViewModel:OnNavigatedTo");
        }
        public IEnumerable<Models.LocInfo> LoadDatas()
        {
            TokuisakiCnt = 0;
            List<Models.LocInfo> locs = new List<Models.LocInfo>();
            List<Models.LocInfo> tmps = new List<Models.LocInfo>();

            var distgroup = _mapping!.distgroups.Where(x => x.CdDistGroup == _distgroupinfo!.CdDistGroup).FirstOrDefault();
            if (distgroup != null)
            {
                tmps = DistGroupInfoLoader.GetLoc(_distgroupinfo);

                foreach (var blockseq in distgroup.DistBlockSeqs)
                {
                    var block = _mapping!.blocks.Find(x => x.CdBlock == blockseq.CdBlock);
                    if (block != null)
                    {
                        foreach (var addr in block.addrs)
                        {
                            var loc = new Models.LocInfo(block.CdBlock, addr.TdUnitAddrCode);
                            locs.Add(loc);
                        }
                    }
                }

                CdDistGroup = distgroup.CdDistGroup;
                NmDistGroup = distgroup.NmDistGroup;
                DtDelivery = _mapping.DtDelivery;

                if (_mapping!.GetShopCnt(_distgroupinfo!.CdDistGroup) != 0)
                {
                    foreach (var loc in locs)
                    {
                        var p = distgroup.mappings.Find(x => x.CdBlock == loc.CdBlock && x.tdunitaddrcode == loc.Tdunitaddrcode);
                        if (p != null)
                        {
                            loc.CdCourse = p.CdSumCourse;
                            loc.CdRoute = p.CdSumRoute.ToString();
                            loc.CdTokuisaki = p.CdSumTokuisaki;
                            loc.NmTokuisaki = p.NmSumTokuisaki;
                            loc.CdBinSum = p.CdBinSum == (int)BinSumType.Yes ? "●" : "";
                            loc.CdSumTokuisaki = p.CdTokuisaki != p.CdSumTokuisaki ? "●" : "";
                            loc.Maguchi = p.Maguchi.ToString();
                            TokuisakiCnt++;
                        }
                    }
                }
                else
                {
                    foreach (var loc in locs)
                    {
                        var p = tmps.Find(x => x.CdBlock == loc.CdBlock && x.Tdunitaddrcode == loc.Tdunitaddrcode);
                        if (p != null)
                        {
                            loc.CdCourse = p.CdCourse;
                            loc.CdRoute = p.CdRoute;
                            loc.CdTokuisaki = p.CdTokuisaki;
                            loc.NmTokuisaki = p.NmTokuisaki;
                            loc.CdBinSum = p.CdBinSum;
                            loc.CdSumTokuisaki = p.CdSumTokuisaki;
                            loc.Maguchi = p.Maguchi;
                            TokuisakiCnt++;
                        }
                    }
                }
            }

            return locs;
        }
    }
}
