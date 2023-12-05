using DbLib.Extensions;
using DistLargePrint.Models;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;
using WindowLib.Utils;

namespace DistLargePrint.ViewModels
{
    public class MainDistLargePrintViewModel : BindableBase
    {
        public DelegateCommand Search { get; }
        public DelegateCommand Reload { get; }
        public DelegateCommand Print { get; }
        public DelegateCommand Exit { get; }

        private readonly IDialogService _dialogService;

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
            set => SetProperty(ref _searchConditionType, value);
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

            Search = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistLargePrintViewModel:Search");
                // fixme:検索ボタン押下
            });

            Reload = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistLargePrintViewModel:Reload");
                // fixme:更新ボタン押下
            });

            Print = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistLargePrintViewModel:Print");
                // fixme:印刷ボタン押下
            });

            Exit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistLargePrintViewModel:Exit");
                Application.Current.MainWindow.Close();
            });

            // fixme:大仕分G, 納品日
            CdLargeGroup = "001";
            DtDelivery = "20231015";

            LoadDatas();
        }

        private void LoadDatas()
        {
            try
            {
                CollectionViewHelper.SetCollection(DistLargePrints, DistLargePrintLoader.Get());
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }
    }
}
