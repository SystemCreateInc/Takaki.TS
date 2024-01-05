using Dapper.FastCrud;
using DbLib.DbEntities;
using DbLib.Extensions;
using LogLib;
using MaterialDesignThemes.Wpf;
using PrintPreviewLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SearchBoxLib;
using SearchBoxLib.Models;
using System.Collections.ObjectModel;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using TakahataDistListPrint.Models;
using TakahataDistListPrint.Reports;
using TakahataDistListPrint.Views;
using WindowLib.Utils;

namespace TakahataDistListPrint.ViewModels
{
    public class MainTakahataDistListPrintViewModel : BindableBase
    {
        public DelegateCommand<object> Search { get; }
        public DelegateCommand Reload { get; }
        public DelegateCommand CustomerPrint { get; }
        public DelegateCommand ItemPrint { get; }
        public DelegateCommand Exit { get; }

        private readonly IDialogService _dialogService;
        private SearchBoxService _searchBoxService = new SearchBoxService();

        private PackIconKind _searchIcon = PackIconKind.Search;
        public PackIconKind SearchIcon
        {
            get => _searchIcon;
            set => SetProperty(ref _searchIcon, value);
        }

        private string _dtDelivery = string.Empty;
        public string DtDelivery
        {
            get => _dtDelivery;
            set => SetProperty(ref _dtDelivery, value);
        }

        public string DispDtDelivery => DtDelivery.GetDate();

        private ObservableCollection<Models.TakahataDistListPrint> _takahataDistListPrints = new ObservableCollection<Models.TakahataDistListPrint>();
        public ObservableCollection<Models.TakahataDistListPrint> TakahataDistListPrints
        {
            get => _takahataDistListPrints;
            set => SetProperty(ref _takahataDistListPrints, value);
        }

        public MainTakahataDistListPrintViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Search = new DelegateCommand<object>(obj =>
            {
                Syslog.Debug("MainTakahataDistListPrintViewModel:Search");

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
                Syslog.Debug("MainTakahataDistListPrintViewModel:Reload");
                _searchBoxService.Clear();
                LoadDatas();
                SearchIcon = PackIconKind.Search;
            });

            CustomerPrint = new DelegateCommand(() =>
            {
                Syslog.Debug("MainTakahataDistListPrintViewModel:CustomerPrint");

                try
                {
                    var vms = ReportCreator.CreateCustomerReport(TakahataDistListPrints.Select(x => x.IdDist), DtDelivery);
                    var ppm = new PrintPreviewManager(PageMediaSizeName.ISOA4, PageOrientation.Portrait);
                    ppm.PrintPreview("得意先別仕分リスト(対象外)", vms);
                }
                catch (Exception e)
                {
                    Syslog.Error($"CustomerPrint:{e.Message}");
                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                }
            });

            ItemPrint = new DelegateCommand(() =>
            {
                Syslog.Debug("MainTakahataDistListPrintViewModel:ItemPrint");

                try
                {
                    var vms = ReportCreator.CreateItemReport(TakahataDistListPrints.Select(x => x.IdDist), DtDelivery);
                    var ppm = new PrintPreviewManager(PageMediaSizeName.ISOA4, PageOrientation.Portrait);
                    ppm.PrintPreview("商品別仕分リスト(対象外)", vms);
                }
                catch (Exception e)
                {
                    Syslog.Error($"ItemPrint:{e.Message}");
                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                }
            });

            Exit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainTakahataDistListPrintViewModel:Exit");
                Application.Current.MainWindow.Close();
            });

            SelectDeliveryDate();
            LoadDatas();
        }

        private void LoadDatas()
        {
            try
            {
                CollectionViewHelper.SetCollection(TakahataDistListPrints, TakahataDistListPrintLoader.Get(DtDelivery, _searchBoxService.GetQuery()));
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }

        private void SelectDeliveryDate()
        {
            _dialogService.ShowDialog(nameof(SelectDeliveryDateDlg),
                rc =>
                {
                    if (rc.Result != ButtonResult.OK)
                    {
                        Application.Current.MainWindow.Close();
                        return;
                    }

                    DtDelivery = rc.Parameters.GetValue<DateTime>("Date").ToString("yyyyMMdd");
                });
        }

        private IEnumerable<Content> GetSearchContents()
        {
            return new List<Content>()
            {
                new () { ContentName = "出荷バッチ", TableName = Sql.Format<TBDISTEntity>($"{nameof(TBDISTEntity):T}.{nameof(TBDISTEntity.CDSHUKKABATCH):C}") },
                new () { ContentName = "コース", TableName = Sql.Format<TBDISTEntity>($"{nameof(TBDISTEntity):T}.{nameof(TBDISTEntity.CDCOURSE):C}") },
                new () { ContentName = "得意先コード", TableName = Sql.Format<TBDISTEntity>($"{nameof(TBDISTEntity):T}.{nameof(TBDISTEntity.CDTOKUISAKI):C}") },
                new () { ContentName = "得意先名", TableName = "v1.NM_TOKUISAKI" },
                new () { ContentName = "品番", TableName = Sql.Format<TBDISTEntity>($"{nameof(TBDISTEntity):T}.{nameof(TBDISTEntity.CDHIMBAN):C}") },
                new () { ContentName = "JANコード", TableName = Sql.Format<TBDISTEntity>($"{nameof(TBDISTEntity):T}.{nameof(TBDISTEntity.CDGTIN13):C}") },
                new () { ContentName = "品名", TableName = "v2.NM_HIN_SEISHIKIMEI"},
            };
        }
    }
}
