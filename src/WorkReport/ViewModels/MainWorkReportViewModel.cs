using CsvLib.Services;
using LogLib;
using PrintPreviewLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.IO;
using System.Printing;
using System.Windows;
using WindowLib.Utils;
using WorkReport.Models;
using WorkReport.Reports;
using WorkReport.Views;

namespace WorkReport.ViewModels
{
    public class MainWorkReportViewModel : BindableBase
    {
        public DelegateCommand Print { get; }
        public DelegateCommand<object> CSV { get; }
        public DelegateCommand Exit { get; }

        private readonly IDialogService _dialogService;

        private DateTime _startDate = DateTime.Today.AddDays(1);
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                SetProperty(ref _startDate, value);
                LoadDatas();
            }
        }

        private DateTime _endDate = DateTime.Today.AddDays(1);
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                SetProperty(ref _endDate, value);
                LoadDatas();
            }
        }

        private ObservableCollection<Models.WorkReport> _workReports = new ObservableCollection<Models.WorkReport>();
        public ObservableCollection<Models.WorkReport> WorkReports
        {
            get => _workReports;
            set => SetProperty(ref _workReports, value);
        }

        private bool _canCSV;
        public bool CanCSV
        {
            get => _canCSV;
            set => SetProperty(ref _canCSV, value);
        }

        public MainWorkReportViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Print = new DelegateCommand(() =>
            {
                Syslog.Debug("MainWorkReportViewModel:Print");

                try
                {
                    var list = WorkReportLoader.Get(StartDate.ToString("yyyyMMdd"), EndDate.AddDays(1).ToString("yyyyMMdd"));
                    var vms = ReportCreator.Create(StartDate, EndDate, list);
                    var ppm = new PrintPreviewManager(PageMediaSizeName.ISOA4, PageOrientation.Landscape);
                    ppm.PrintPreview("作業報告書", vms);
                }
                catch (Exception e)
                {
                    Syslog.Error($"Print:{e.Message}");
                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                }
            });

            CSV = new DelegateCommand<object>(obj =>
            {
                Syslog.Debug("MainWorkReportViewModel:CSV");

                try
                {
                    LoadDatas();
                    var rows = WorkReports.ToList().Select(x => x.GetRow());
                    CsvFileService.Save(obj, rows, $"{nameof(MainWorkReport)}CSVPath", "作業報告書発行");
                }
                catch (IOException e)
                {
                    Syslog.Error($"CSV:{e.Message}");
                    var errorMessage = e.Message.Contains("it is being used by another process")
                        ? "同じファイルを開いているので保存できません。" : e.Message;

                    MessageDialog.Show(_dialogService, errorMessage, "エラー");
                }
                catch (Exception e)
                {
                    Syslog.Error($"CSV:{e.Message}");
                    MessageDialog.Show(_dialogService, e.Message, "エラー");
                }
            }).ObservesCanExecute(() => CanCSV);

            Exit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainWorkReportViewModel:Exit");
                Application.Current.MainWindow.Close();
            });

            LoadDatas();
        }

        private void LoadDatas()
        {
            try
            {
                CollectionViewHelper.SetCollection(WorkReports, WorkReportLoader.Get(StartDate.ToString("yyyyMMdd"), EndDate.AddDays(1).ToString("yyyyMMdd")));
                CanCSV = WorkReports.Any();
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }
    }
}
