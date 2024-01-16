using CsvLib.Services;
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
using StowageListPrint.Models;
using StowageListPrint.Reports;
using StowageListPrint.Views;
using System.Collections.ObjectModel;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using WindowLib.Utils;

namespace StowageListPrint.ViewModels
{
    public class MainStowageListPrintViewModel : BindableBase
    {
        public DelegateCommand<object> Search { get; }
        public DelegateCommand Reload { get; }
        public DelegateCommand Print { get; }
        public DelegateCommand Edit { get; }
        public DelegateCommand<object> CSV { get; }
        public DelegateCommand Exit { get; }
        public DelegateCommand LeftDoubleClick { get; }

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

        private ObservableCollection<Models.StowageListPrint> _stowageListPrints = new ObservableCollection<Models.StowageListPrint>();
        public ObservableCollection<Models.StowageListPrint> StowageListPrints
        {
            get => _stowageListPrints;
            set => SetProperty(ref _stowageListPrints, value);
        }

        private Models.StowageListPrint? _currentStowageListPrint;
        public Models.StowageListPrint? CurrentStowageListPrint
        {
            get => _currentStowageListPrint;
            set
            {
                SetProperty(ref _currentStowageListPrint, value);
                CanEdit = CurrentStowageListPrint != null;
            }
        }

        private bool _canEdit;
        public bool CanEdit
        {
            get => _canEdit;
            set => SetProperty(ref _canEdit, value);
        }

        private bool _canCSV;
        public bool CanCSV
        {
            get => _canCSV;
            set => SetProperty(ref _canCSV, value);
        }

        public MainStowageListPrintViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Search = new DelegateCommand<object>(obj =>
            {
                Syslog.Debug("MainStowageListPrintViewModel:Search");

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
                Syslog.Debug("MainStowageListPrintViewModel:Reload");
                _searchBoxService.Clear();
                LoadDatas();
                SearchIcon = PackIconKind.Search;
            });

            Print = new DelegateCommand(() =>
            {
                Syslog.Debug("MainStowageListPrintViewModel:Print");

                try
                {
                    var reportList = StowageListPrintLoader.Get(CdDistGroup, DtDelivery, _searchBoxService.GetQuery());
                    var vms = StowageReportCreator.Create(CdDistGroup, NmDistGroup, DispDtDelivery, reportList);
                    var ppm = new PrintPreviewManager(PageMediaSizeName.ISOA4, PageOrientation.Landscape);
                    ppm.PrintPreview("積付表", vms);
                }
                catch (Exception e)
                {
                    Syslog.Error($"Print:{e.Message}");
                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                }
            });

            Edit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainStowageListPrintViewModel:Edit");

                if (ShowInputDialog())
                {
                    LoadDatas();
                }
            }).ObservesCanExecute(() => CanEdit);

            CSV = new DelegateCommand<object>(obj =>
            {
                Syslog.Debug("MainStowageListPrintViewModel:CSV");

                try
                {
                    LoadDatas();
                    var rows = StowageListPrints.ToList().Select(x => x.GetRow());
                    CsvFileService.Save(obj, rows, $"{nameof(MainStowageListPrint)}CSVPath", "積付表発行");
                }
                catch (Exception e)
                {
                    Syslog.Error($"CSV:{e.Message}");
                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                }
            }).ObservesCanExecute(() => CanCSV);

            Exit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainStowageListPrintViewModel:Exit");
                Application.Current.MainWindow.Close();
            });

            LeftDoubleClick = new DelegateCommand(() =>
            {
                Syslog.Debug("MainStowageListPrintViewModel:LeftDoubleClick");
                Edit.Execute();
            }).ObservesCanExecute(() => CanEdit);

            if (!SelectDistGroup())
            {
                Application.Current.MainWindow.Close();
                return;
            }

            LoadDatas();
        }

        private bool SelectDistGroup()
        {
            IDialogResult? result = null;
            _dialogService.ShowDialog(
                nameof(SelectDistGroupDlg),
                null,
                r => result = r);

            if (result?.Result != ButtonResult.OK)
            {
                return false;
            }

            CdDistGroup = result.Parameters.GetValue<string>("CdDistGroup");
            NmDistGroup = result.Parameters.GetValue<string>("NmDistGroup");
            DtDelivery = result.Parameters.GetValue<string>("DtDelivery");
            return true;
        }

        private void LoadDatas()
        {
            try
            {
                CollectionViewHelper.SetCollection(StowageListPrints, StowageListPrintLoader.Get(CdDistGroup, DtDelivery, _searchBoxService.GetQuery()));
                CanCSV = StowageListPrints.Any();
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }

        private bool ShowInputDialog()
        {
            IDialogResult? result = null;

            _dialogService.ShowDialog(
                nameof(InputStowageDlg),
                new DialogParameters
                {
                    { "CurrentStowageListPrint", CurrentStowageListPrint },
                },
                r => result = r);

            return result?.Result == ButtonResult.OK;
        }

        private IEnumerable<Content> GetSearchContents()
        {
            return new List<Content>()
            {
                new () { ContentName = "コース", TableName = Sql.Format<TBSTOWAGEEntity>($"{nameof(TBSTOWAGEEntity):T}.{nameof(TBSTOWAGEEntity.CDCOURSE):C}") },
                new () { ContentName = "得意先コード", TableName = Sql.Format<TBSTOWAGEEntity>($"{nameof(TBSTOWAGEEntity):T}.{nameof(TBSTOWAGEEntity.CDTOKUISAKI):C}") },
                new () { ContentName = "得意先名", TableName = Sql.Format<TBSTOWAGEMAPPINGEntity>($"{nameof(TBSTOWAGEMAPPINGEntity):T}.{nameof(TBSTOWAGEMAPPINGEntity.NMTOKUISAKI):C}") },
            };
        }
    }
}
