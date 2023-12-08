using CsvLib.Services;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;
using WindowLib.Utils;
using WorkReport.Models;
using WorkReport.Views;

namespace WorkReport.ViewModels
{
    public class MainWorkReportViewModel : BindableBase
    {
        public DelegateCommand Print { get; }
        public DelegateCommand<object> CSV { get; }
        public DelegateCommand Exit { get; }

        private readonly IDialogService _dialogService;

        private DateTime _startDate = DateTime.Today;
        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        private DateTime _endDate = DateTime.Today;
        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
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
                // fixme:印刷ボタン押下
            });

            CSV = new DelegateCommand<object>(obj =>
            {
                Syslog.Debug("MainWorkReportViewModel:CSV");

                try
                {
                    CsvFileService.Save(WorkReports.ToList(), $"{nameof(MainWorkReport)}CSVPath", "作業報告書発行");
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
                CollectionViewHelper.SetCollection(WorkReports, WorkReportLoader.Get());
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
