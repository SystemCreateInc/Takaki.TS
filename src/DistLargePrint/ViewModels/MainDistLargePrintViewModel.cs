using Dapper.FastCrud;
using DbLib.DbEntities;
using DbLib.Extensions;
using DistLargePrint.Models;
using DistLargePrint.Reports;
using DistLargePrint.Views;
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
using WindowLib.Utils;

namespace DistLargePrint.ViewModels
{
    public class MainDistLargePrintViewModel : BindableBase
    {
        public DelegateCommand<object> Search { get; }
        public DelegateCommand Reload { get; }
        public DelegateCommand Print { get; }
        public DelegateCommand Exit { get; }

        private readonly IDialogService _dialogService;
        private SearchBoxService _searchBoxService = new SearchBoxService();

        private PackIconKind _searchIcon = PackIconKind.Search;
        public PackIconKind SearchIcon
        {
            get => _searchIcon;
            set => SetProperty(ref _searchIcon, value);
        }

        private string _cdLargeGroup = string.Empty;
        public string CdLargeGroup
        {
            get => _cdLargeGroup;
            set => SetProperty(ref _cdLargeGroup, value);
        }

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

        private ObservableCollection<Models.DistLargePrint> _distLargePrints = new ObservableCollection<Models.DistLargePrint>();
        public ObservableCollection<Models.DistLargePrint> DistLargePrints
        {
            get => _distLargePrints;
            set => SetProperty(ref _distLargePrints, value);
        }

        public MainDistLargePrintViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Search = new DelegateCommand<object>(obj =>
            {
                Syslog.Debug("MainDistLargePrintViewModel:Search");

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
                Syslog.Debug("MainDistLargePrintViewModel:Reload");
                _searchBoxService.Clear();
                LoadDatas();
                SearchIcon = PackIconKind.Search;
            });

            Print = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistLargePrintViewModel:Print");
                var vms = DistLargeReportCreator.Create(CdLargeGroup, DtDelivery, DistLargePrints);
                var ppm = new PrintPreviewManager(PageMediaSizeName.ISOA4, PageOrientation.Portrait);
                ppm.PrintPreview("部品在庫一覧", vms);
            });

            Exit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistLargePrintViewModel:Exit");
                Application.Current.MainWindow.Close();
            });

            if (!ShowDialog())
            {
                Application.Current.MainWindow.Close();
                return;
            }

            LoadDatas();
        }

        private bool ShowDialog()
        {
            IDialogResult? result = null;
            _dialogService.ShowDialog(
                nameof(SelectDistLargeGroupDlg),
                null,
                r => result = r);

            if (result?.Result != ButtonResult.OK)
            {
                return false;
            }

            CdLargeGroup = result.Parameters.GetValue<string>("CdLargeGroup");
            DtDelivery = result.Parameters.GetValue<string>("DtDelivery");
            return true;
        }

        private void LoadDatas()
        {
            try
            {
                CollectionViewHelper.SetCollection(DistLargePrints, DistLargePrintLoader.Get(SearchConditionType, CdLargeGroup, DtDelivery, _searchBoxService.GetQuery()));
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
                new () { ContentName = "品番", TableName = Sql.Format<TBDISTEntity>($"{nameof(TBDISTEntity):T}.{nameof(TBDISTEntity.CDHIMBAN):C}"), },
                new () { ContentName = "JANコード", TableName = Sql.Format<TBDISTEntity>($"{nameof(TBDISTEntity):T}.{nameof(TBDISTEntity.CDGTIN13):C}"), },
                new () { ContentName = "品名", TableName = Sql.Format<TBDISTMAPPINGEntity>($"{nameof(TBDISTMAPPINGEntity):T}.{nameof(TBDISTMAPPINGEntity.NMHINSEISHIKIMEI):C}"), },
                new () { ContentName = "ブロック", TableName = Sql.Format<TBDISTMAPPINGEntity>($"{nameof(TBDISTMAPPINGEntity):T}.{nameof(TBDISTMAPPINGEntity.CDBLOCK):C}"), },
            };
        }
    }
}
