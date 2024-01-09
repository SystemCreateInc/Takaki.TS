using Dapper.FastCrud;
using DbLib.DbEntities;
using DbLib.Extensions;
using DistListPrint.Models;
using DistListPrint.Reports;
using LogLib;
using MaterialDesignThemes.Wpf;
using PrintPreviewLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SearchBoxLib;
using SearchBoxLib.Models;
using SelDistGroupLib.Models;
using SelDistGroupLib.Services.Auth;
using System.Collections.ObjectModel;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using WindowLib.Utils;

namespace DistListPrint.ViewModels
{
    public class MainDistListPrintViewModel : BindableBase
    {
        public DelegateCommand<object> Search { get; }
        public DelegateCommand Reload { get; }
        public DelegateCommand CustomerReportPrint { get; }
        public DelegateCommand ItemReportPrint { get; }
        public DelegateCommand Exit { get; }

        private readonly IDialogService _dialogService;
        private SearchBoxService _searchBoxService = new SearchBoxService();

        private const int _chunkSize = 100;

        private PackIconKind _searchIcon = PackIconKind.Search;
        public PackIconKind SearchIcon
        {
            get => _searchIcon;
            set => SetProperty(ref _searchIcon, value);
        }

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

        public string DistGroupInfo => CdDistGroup + " " + NmDistGroup;

        private string _dtDelivery = string.Empty;
        public string DtDelivery
        {
            get => _dtDelivery;
            set => SetProperty(ref _dtDelivery, value);
        }

        public string DispDtDelivery => DtDelivery.GetDate();

        private SearchConditionType _searchConditionType = SearchConditionType.All;
        public SearchConditionType SearchConditionType
        {
            get => _searchConditionType;
            set
            {
                SetProperty(ref _searchConditionType, value);
                LoadDatas();
            }
        }

        private ObservableCollection<Models.DistListPrint> _distListPrints = new ObservableCollection<Models.DistListPrint>();
        public ObservableCollection<Models.DistListPrint> DistListPrints
        {
            get => _distListPrints;
            set => SetProperty(ref _distListPrints, value);
        }

        public MainDistListPrintViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Search = new DelegateCommand<object>(obj =>
            {
                Syslog.Debug("MainDistListPrintViewModel:Search");

                var dataGrid = obj as DataGrid;
                if (dataGrid == null)
                {
                    return;
                }

                if (_searchBoxService.ShowSearchBox(dialogService, GetSearchContents()))
                {
                    LoadDatas();
                    SearchIcon = PackIconKind.SearchAdd;
                }
            });

            Reload = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistListPrintViewModel:Reload");
                _searchBoxService.Clear();
                LoadDatas();
                SearchIcon = PackIconKind.Search;
            });

            CustomerReportPrint = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistListPrintViewModel:CustomerReportPrint");

                try
                {
                    var vms = ReportCreator.CreateCustomerReport(DistListPrints.Select(x => x.IdDist), SearchConditionType, CdDistGroup,
                        NmDistGroup, DispDtDelivery, _chunkSize);
                    var ppm = new PrintPreviewManager(PageMediaSizeName.ISOA4, PageOrientation.Landscape);

                    // 印刷単位毎にプレビューを表示し、処理を軽くする
                    var printUnitVms = vms.Select((v, i) => new { v, i })
                        .GroupBy(x => x.i / _chunkSize).Select(s => s.Select(x => x.v));

                    foreach (var printVms in printUnitVms)
                    {
                        ppm.PrintPreview("得意先別仕分リスト", printVms);
                    }
                }
                catch (Exception e)
                {
                    Syslog.Error($"CustomerReportPrint:{e.Message}");
                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                }
            });

            ItemReportPrint = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistListPrintViewModel:ItemReportPrint");

                try
                {
                    var vms = ReportCreator.CreateItemReport(DistListPrints.Select(x => x.IdDist), SearchConditionType, CdDistGroup,
                        NmDistGroup, DispDtDelivery, _chunkSize);
                    var ppm = new PrintPreviewManager(PageMediaSizeName.ISOA4, PageOrientation.Landscape);

                    // 印刷単位毎にプレビューを表示し、処理を軽くする
                    var printUnitVms = vms.Select((v, i) => new { v, i })
                        .GroupBy(x => x.i / _chunkSize).Select(s => s.Select(x => x.v));

                    foreach (var printVms in printUnitVms)
                    {
                        ppm.PrintPreview("商品別仕分リスト", printVms);
                    }
                }
                catch (Exception e)
                {
                    Syslog.Error($"ItemReportPrint:{e.Message}");
                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                }
            });

            Exit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistListPrintViewModel:Exit");
                Application.Current.MainWindow.Close();
            });

            if (!SelectDistGroup())
            {
                Application.Current.MainWindow.Close();
                return;
            }

            LoadDatas();
        }

        private bool SelectDistGroup()
        {
            if (AuthenticateService.AuthDistGroupDialog(_dialogService) is DistGroup distGroup)
            {
                CdDistGroup = distGroup.CdDistGroup;
                NmDistGroup = distGroup.NmDistGroup;
                DtDelivery = distGroup.DtDelivery.ToString("yyyyMMdd");
                return true;
            }
            return false;
        }

        private void LoadDatas()
        {
            try
            {
                CollectionViewHelper.SetCollection(DistListPrints, DistListPrintLoader.Get(SearchConditionType, CdDistGroup, DtDelivery, _searchBoxService.GetQuery()));
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }

        private IEnumerable<Content> GetSearchContents()
        {
            return new List<Content>()
            {
                new () { ContentName = "出荷バッチ", TableName = Sql.Format<TBDISTEntity>($"{nameof(TBDISTEntity):T}.{nameof(TBDISTEntity.CDSHUKKABATCH):C}") },
                new () { ContentName = "コース", TableName = Sql.Format<TBDISTEntity>($"{nameof(TBDISTEntity):T}.{nameof(TBDISTEntity.CDCOURSE):C}") },
                new () { ContentName = "得意先コード", TableName = Sql.Format<TBDISTEntity>($"{nameof(TBDISTEntity):T}.{nameof(TBDISTEntity.CDTOKUISAKI):C}") },
                new () { ContentName = "得意先名", TableName = Sql.Format<TBDISTMAPPINGEntity>($"{nameof(TBDISTMAPPINGEntity):T}.{nameof(TBDISTMAPPINGEntity.NMTOKUISAKI):C}") },
                new () { ContentName = "品番", TableName = Sql.Format<TBDISTEntity>($"{nameof(TBDISTEntity):T}.{nameof(TBDISTEntity.CDHIMBAN):C}") },
                new () { ContentName = "JANコード", TableName = Sql.Format<TBDISTEntity>($"{nameof(TBDISTEntity):T}.{nameof(TBDISTEntity.CDGTIN13):C}") },
                new () { ContentName = "品名", TableName = Sql.Format<TBDISTMAPPINGEntity>($"{nameof(TBDISTMAPPINGEntity):T}.{nameof(TBDISTMAPPINGEntity.NMHINSEISHIKIMEI):C}") },
                new () { ContentName = "作業担当者", TableName = Sql.Format<TBDISTEntity>($"{nameof(TBDISTEntity):T}.{nameof(TBDISTEntity.NMSHAINDIST):C}") },
            };
        }
    }
}
