using DbLib.Extensions;
using DistListPrint.Models;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;
using WindowLib.Utils;

namespace DistListPrint.ViewModels
{
    public class MainDistListPrintViewModel : BindableBase
    {
        public DelegateCommand Search { get; }
        public DelegateCommand Reload { get; }
        public DelegateCommand CustomerPrint { get; }
        public DelegateCommand ItemPrint { get; }
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

        private SearchConditionType _searchConditionType = SearchConditionType.All;
        public SearchConditionType SearchConditionType
        {
            get => _searchConditionType;
            set => SetProperty(ref _searchConditionType, value);
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

            Search = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistListPrintViewModel:Search");
                // fixme: 検索ボタン押下
            });

            Reload = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistListPrintViewModel:Reload");
                //fixme:更新ボタン押下
            });

            CustomerPrint = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistListPrintViewModel:CustomerPrint");
                // fixme:得意先別ボタン押下
            });

            ItemPrint = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistListPrintViewModel:ItemPrint");
                // 商品別ボタン押下
            });

            Exit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistListPrintViewModel:Exit");
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
                CollectionViewHelper.SetCollection(DistListPrints, DistListPrintLoader.Get());
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }
    }
}
