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
using ReferenceLogLib.Models;
using PrintPreviewLib;
using System.Printing;
using Mapping.Reports;
using PrintLib;

namespace Mapping.ViewModels
{
    public class OverTokuisakiViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand OnPrint { get; }
        public DelegateCommand OnBack { get; }

        private int _tokuisakiCnt = 0;
        public int TokuisakiCnt
        {
            get => _tokuisakiCnt;
            set => SetProperty(ref _tokuisakiCnt, value);
        }
        private ObservableCollection<OverInfo> _overinfos = new ObservableCollection<OverInfo>();
        public ObservableCollection<OverInfo> OverInfos
        {
            get => _overinfos;
            set => SetProperty(ref _overinfos, value);
        }

        private DistGroupInfo? _distgroupinfo = null;
        private MappingManager? _mapping = null;

        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;

        public OverTokuisakiViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;

            OnPrint = new DelegateCommand(() =>
            {
                Syslog.Debug("OverTokuisakiViewModel:OverPrint");
                try
                {
                    using (var busy = new WaitCursor())
                    {
                        var overs = OverInfos.ToList();

                        if (overs == null || overs.Count == 0)
                        {
                            return;
                        }

                        var viewModel = ReportCreator.GetOverList(overs);

                        var ppm = new PrintPreviewManager(PageMediaSizeName.ISOA4, PageOrientation.Landscape);
                        ppm.PrintPreview("あふれ一覧", viewModel);

                        // 実際に印刷していないのでｷｬﾝｾﾙ
                        if (ppm.IsPrinted == false)
                            return;

#if false
                    var ppm = new PrintManager(PageMediaSizeName.ISOA4, PageOrientation.Landscape);
                    ppm.Print("あふれ一覧", viewModel);
#endif

                        if (_mapping != null)
                        {
                            var distgroup = _mapping!.distgroups.Where(x => x.CdDistGroup == _distgroupinfo!.CdDistGroup).FirstOrDefault();
                            if (distgroup != null)
                            {
                                distgroup.IsSave = true;
                            }
                        }
                    }

                    regionManager.Regions["ContentRegion"].NavigationService.Journal.GoBack();
                }
                catch (Exception e)
                {
                    Syslog.Error($"MainMappingViewModel:Clear:{e.Message}");
                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                }

            });


            OnBack = new DelegateCommand(() =>
            {
                Syslog.Info("【戻る】OverTokuisakiViewModel:OnBack");

                if (MessageDialog.Show(_dialogService, "座席マッピングの実行結果をキャンセルします。よろしいですか？", "確認", ButtonMask.OK | ButtonMask.Cancel) != ButtonResult.OK)
                    return;

                if (_mapping != null)
                {
                    var distgroup = _mapping!.distgroups.Where(x => x.CdDistGroup == _distgroupinfo!.CdDistGroup).FirstOrDefault();
                    if (distgroup != null)
                    {
                        distgroup.IsCancel = true;
                    }
                }

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
            Syslog.Info($"OverTokuisakiViewModel:OnNavigatedFrom");
            return;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _mapping = navigationContext.Parameters.GetValue<MappingManager>("Mapping");
            _distgroupinfo = navigationContext.Parameters.GetValue<DistGroupInfo>("currentdistinfo");

            try
            {
                CollectionViewHelper.SetCollection(OverInfos, LoadDatas());
            }
            catch (Exception e)
            {
                Syslog.Error($"OverTokuisakiViewModel:LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }

            Syslog.Info($"OverTokuisakiViewModel:OnNavigatedTo");
        }

        public IEnumerable<Models.OverInfo> LoadDatas()
        {
            List<Models.OverInfo> overs = new List<Models.OverInfo>();

            var distgroup = _mapping!.distgroups.Where(x => x.CdDistGroup == _distgroupinfo!.CdDistGroup).FirstOrDefault();
            if (distgroup != null)
            {
                // アドレスなしのみ表示
                foreach (var dist in distgroup.dists)
                {
                    if (dist.tdunitaddrcode=="")
                    {
                        overs.Add(new Models.OverInfo(dist, distgroup));
                    }
                }
            }

            TokuisakiCnt = overs.Select(x=>x.CdTokuisaki).Distinct().Count();

            return overs;
        }
    }
}
