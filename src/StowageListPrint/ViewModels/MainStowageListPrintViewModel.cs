using CsvLib.Services;
using DbLib.Extensions;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using StowageListPrint.Models;
using StowageListPrint.Views;
using System.Collections.ObjectModel;
using System.Windows;
using WindowLib.Utils;

namespace StowageListPrint.ViewModels
{
    public class MainStowageListPrintViewModel : BindableBase
    {
        public DelegateCommand Search { get; }
        public DelegateCommand Reload { get; }
        public DelegateCommand Print { get; }
        public DelegateCommand Edit { get; }
        public DelegateCommand<object> CSV { get; }
        public DelegateCommand Exit { get; }

        private readonly IDialogService _dialogService;

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

        private bool _canCSV;
        public bool CanCSV
        {
            get => _canCSV;
            set => SetProperty(ref _canCSV, value);
        }

        public MainStowageListPrintViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Search = new DelegateCommand(() =>
            {
                Syslog.Debug("MainStowageListPrintViewModel:Search");
                // fixme:検索ボタン押下
            });

            Reload = new DelegateCommand(() =>
            {
                Syslog.Debug("MainStowageListPrintViewModel:Reload");
                // fixme:更新ボタン押下
            });

            Print = new DelegateCommand(() =>
            {
                Syslog.Debug("MainStowageListPrintViewModel:Print");
                // fixme:積付表発行ボタン押下
            });

            Edit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainStowageListPrintViewModel:Edit");
                // fixme:積付数変更ボタン押下
            });

            CSV = new DelegateCommand<object>(obj =>
            {
                Syslog.Debug("MainStowageListPrintViewModel:CSV");

                try
                {
                    CsvFileService.Save(StowageListPrints.ToList(), $"{nameof(MainStowageListPrint)}CSVPath", "積付表発行");
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

            // fixme:仕分グループ + 仕分名称
            CdDistGroup = "02001";
            NmDistGroup = "広島常温1便";

            // fixme:納品日
            DtDelivery = "20231015";

            LoadDatas();
        }

        private void LoadDatas()
        {
            try
            {
                CollectionViewHelper.SetCollection(StowageListPrints, StowageListPrintLoader.Get());
                CanCSV = StowageListPrints.Any();
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }
    }
}
