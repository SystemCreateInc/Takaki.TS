using DbLib.Extensions;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;
using TakahataDistListPrint.Models;
using WindowLib.Utils;

namespace TakahataDistListPrint.ViewModels
{
    public class MainTakahataDistListPrintViewModel : BindableBase
    {
        public DelegateCommand Search { get; }
        public DelegateCommand Reload { get; }
        public DelegateCommand CustomerPrint { get; }
        public DelegateCommand ItemPrint { get; }
        public DelegateCommand Exit { get; }

        private readonly IDialogService _dialogService;

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

            Search = new DelegateCommand(() =>
            {
                Syslog.Debug("MainTakahataDistListPrintViewModel:Search");
                // fixme:検索ボタン押下
            });

            Reload = new DelegateCommand(() =>
            {
                Syslog.Debug("MainTakahataDistListPrintViewModel:Reload");
                //fixme:更新ボタン押下
            });

            CustomerPrint = new DelegateCommand(() =>
            {
                Syslog.Debug("MainTakahataDistListPrintViewModel:CustomerPrint");
                // fixme:得意先別ボタン押下
            });

            ItemPrint = new DelegateCommand(() =>
            {
                Syslog.Debug("MainTakahataDistListPrintViewModel:ItemPrint");
                // 商品別ボタン押下
            });

            Exit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainTakahataDistListPrintViewModel:Exit");
                Application.Current.MainWindow.Close();
            });

            // fixme:納品日
            DtDelivery = "20231015";

            LoadDatas();
        }

        private void LoadDatas()
        {
            try
            {
                CollectionViewHelper.SetCollection(TakahataDistListPrints, TakahataDistListPrintLoader.Get());
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }
    }
}
