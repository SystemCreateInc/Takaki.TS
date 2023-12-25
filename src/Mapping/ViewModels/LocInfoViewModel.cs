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
            });


            OnBack = new DelegateCommand(() =>
            {
                Syslog.Info("【戻る】LocInfoViewModel:OnBack");

                regionManager.Regions["ContentRegion"].NavigationService.Journal.GoBack();

                if (_distgroupinfo != null)
                {
                    if (_distgroupinfo.OverShopCnt != 0)
                    {
                        _regionManager.RequestNavigate("ContentRegion", nameof(Views.OverTokuisaki), new NavigationParameters
                        {
                            { "currentdistinfo", _distgroupinfo },
                            { "Mapping", _mapping              },
                        });
                    }
                }

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
            var distgroup = _mapping!.distgroups.Where(x => x.CdDistGroup == _distgroupinfo!.CdDistGroup).FirstOrDefault();
            if (distgroup != null)
            {
                CdDistGroup = distgroup.CdDistGroup;
                NmDistGroup = distgroup.NmDistGroup;

                if (_mapping!.GetShopCnt(_distgroupinfo!.CdDistGroup) != 0)
                {
                    // マッピングした情報をもとに表示する
                    foreach (var p in distgroup.mappings)
                    {
                        // ロケーションのみ設定
                        if (p.CdBlock != "")
                        {
                            locs.Add(new Models.LocInfo(p));
                            TokuisakiCnt++;

                            // 足りないアドレスを追加
                            if(1<p.Maguchi)
                            {
                                var block = _mapping!.blocks.Find(x => x.CdBlock == p.CdBlock);
                                if (block != null)
                                {
                                    for (int i = 0; i < block.addrs.Count(); i++)
                                    {
                                        if (block.addrs[i].TdUnitAddrCode == p.tdunitaddrcode)
                                        {
                                            for(int j=1;j<p.Maguchi;j++ )
                                            {
                                                int idx = i + j;
                                                if (idx <  block.addrs.Count())
                                                {
                                                    var addr = block.addrs[idx];
                                                    var loc = new Models.LocInfo(block.CdBlock, addr.TdUnitAddrCode);
                                                    locs.Add(loc);
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // マッピング済みなのでデータを読み込み
                }
            }

            return locs;
        }
    }
}
