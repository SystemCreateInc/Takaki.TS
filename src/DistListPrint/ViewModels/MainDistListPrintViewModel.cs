using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Extensions;
using DistListPrint.Models;
using DistListPrint.Reports;
using LogLib;
using MaterialDesignThemes.Wpf;
using PrintLib;
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
        public DelegateCommand CustomerUnitReportPrint { get; }
        public DelegateCommand Exit { get; }

        private readonly IDialogService _dialogService;
        private SearchBoxService _searchBoxService = new SearchBoxService();

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
                    var chunkSize = GetChunkSize();
                    var vms = ReportCreator.CreateCustomerReport(DistListPrints.Select(x => x.IdDist), SearchConditionType, CdDistGroup,
                            NmDistGroup, DispDtDelivery, chunkSize);

                    var isChunk = vms.Count() > chunkSize;
                    var message = isChunk ? "プレビューは表示しません。" : "プレビューを表示します。";
                    if (MessageDialog.Show(_dialogService, $"{vms.Count()}頁印刷します。よろしいですか？{message}",
                        "印刷確認", ButtonMask.Yes | ButtonMask.No, MessageBoxImage.Question) != ButtonResult.Yes)
                    {
                        return;
                    }

                    if (isChunk)
                    {
                        SplitPrint("得意先別仕分リスト", vms, chunkSize, PageOrientation.Landscape);
                    }
                    else
                    {
                        Print("得意先別仕分リスト", vms, PageOrientation.Landscape);
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
                    var chunkSize = GetChunkSize();
                    var vms = ReportCreator.CreateItemReport(DistListPrints.Select(x => x.IdDist), SearchConditionType, CdDistGroup,
                        NmDistGroup, DispDtDelivery, chunkSize);

                    var isChunk = vms.Count() > chunkSize;
                    var message = isChunk ? "プレビューは表示しません。" : "プレビューを表示します。";
                    if (MessageDialog.Show(_dialogService, $"{vms.Count()}頁印刷します。よろしいですか？{message}",
                        "印刷確認", ButtonMask.Yes | ButtonMask.No, MessageBoxImage.Question) != ButtonResult.Yes)
                    {
                        return;
                    }

                    if (isChunk)
                    {
                        SplitPrint("商品別仕分リスト", vms, chunkSize, PageOrientation.Landscape);
                    }
                    else
                    {
                        Print("商品別仕分リスト", vms, PageOrientation.Landscape);
                    }
                }
                catch (Exception e)
                {
                    Syslog.Error($"ItemReportPrint:{e.Message}");
                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                }
            });

            CustomerUnitReportPrint = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistListPrintViewModel:CustomerUnitReportPrint");

                try
                {
                    var chunkSize = GetChunkSize();
                    var vms = ReportCreator.CreateCustomerUnitReport(DistListPrints.Select(x => x.IdDist), CdDistGroup, NmDistGroup,
                        DispDtDelivery, chunkSize);

                    var isChunk = vms.Count() > chunkSize;
                    var message = isChunk ? "プレビューは表示しません。" : "プレビューを表示します。";
                    if (MessageDialog.Show(_dialogService, $"{vms.Count()}頁印刷します。よろしいですか？{message}",
                        "印刷確認", ButtonMask.Yes | ButtonMask.No, MessageBoxImage.Question) != ButtonResult.Yes)
                    {
                        return;
                    }

                    if (isChunk)
                    {
                        SplitPrint("得意先別総個数集計リスト", vms, chunkSize, PageOrientation.Portrait);
                    }
                    else
                    {
                        Print("得意先別総個数集計リスト", vms, PageOrientation.Portrait);
                    }
                }
                catch (Exception e)
                {
                    Syslog.Error($"CustomerUnitReportPrint:{e.Message}");
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
                new () { ContentName = "ブロック", TableName = Sql.Format<TBDISTMAPPINGEntity>($"{nameof(TBDISTMAPPINGEntity):T}.{nameof(TBDISTMAPPINGEntity.CDBLOCK):C}") },
                new () { ContentName = "コース", TableName = Sql.Format<TBDISTEntity>($"{nameof(TBDISTEntity):T}.{nameof(TBDISTEntity.CDCOURSE):C}") },
                new () { ContentName = "得意先コード", TableName = Sql.Format<TBDISTEntity>($"{nameof(TBDISTEntity):T}.{nameof(TBDISTEntity.CDTOKUISAKI):C}") },
                new () { ContentName = "得意先名", TableName = Sql.Format<TBDISTMAPPINGEntity>($"{nameof(TBDISTMAPPINGEntity):T}.{nameof(TBDISTMAPPINGEntity.NMTOKUISAKI):C}") },
                new () { ContentName = "品番", TableName = Sql.Format<TBDISTEntity>($"{nameof(TBDISTEntity):T}.{nameof(TBDISTEntity.CDHIMBAN):C}") },
                new () { ContentName = "JANコード", TableName = Sql.Format<TBDISTEntity>($"{nameof(TBDISTEntity):T}.{nameof(TBDISTEntity.CDGTIN13):C}") },
                new () { ContentName = "品名", TableName = Sql.Format<TBDISTMAPPINGEntity>($"{nameof(TBDISTMAPPINGEntity):T}.{nameof(TBDISTMAPPINGEntity.NMHINSEISHIKIMEI):C}") },
                new () { ContentName = "作業担当者", TableName = Sql.Format<TBDISTEntity>($"{nameof(TBDISTEntity):T}.{nameof(TBDISTEntity.NMSHAINDIST):C}") },
            };
        }

        private int GetChunkSize()
        {
            var chunkSize = 0;
            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                chunkSize = new Settings(tr).GetInt("PrintChunkSize");
            }

            return chunkSize == 0 ? 100 : chunkSize;
        }

        private void SplitPrint(string title, IEnumerable<IPrintViewModel> vms, int chunkSize, PageOrientation orientation)
        {
            // プレビュー画面を表示しないで印刷
            var printUnitVms = vms.Select((v, i) => new { v, i })
                .GroupBy(x => x.i / chunkSize).Select(s => s.Select(x => x.v));

            var ppm = new PrintManager(PageMediaSizeName.ISOA4, orientation);
            foreach (var printVms in printUnitVms)
            {
                ppm.Print(title, printVms);
            }
        }

        private void Print(string title, IEnumerable<IPrintViewModel> vms, PageOrientation orientation)
        {
            // プレビュー画面を表示
            var ppm = new PrintPreviewManager(PageMediaSizeName.ISOA4, orientation);
            ppm.PrintPreview(title, vms);
        }
    }
}
