using DistProg.Models;
using DistProg.Views;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;
using WindowLib.Utils;

namespace DistProg.ViewModels
{
    public class MainDistProgViewModel : BindableBase
    {
        public DelegateCommand Reload { get; }
        public DelegateCommand ShowDistUncompleted { get; }
        public DelegateCommand ShowDistCompleted { get; }
        public DelegateCommand Exit { get; }

        private readonly IDialogService _dialogService;

        private DateTime _latestTime;
        public DateTime LatestTime
        {
            get => _latestTime;
            set => SetProperty(ref _latestTime, value);
        }

        private ObservableCollection<Models.DistProg> _distProgs = new ObservableCollection<Models.DistProg>();
        public ObservableCollection<Models.DistProg> DistProgs
        {
            get => _distProgs;
            set => SetProperty(ref _distProgs, value);
        }

        public MainDistProgViewModel(IDialogService dialogService, IRegionManager regionManager)
        {
            _dialogService = dialogService;

            Reload = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistProgViewModel:Reload");
                // fixme:更新ボタン押下
            });

            ShowDistUncompleted = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistProgViewModel:ShowDistUncompleted");

                // fixme:仕分未完了ボタン押下
                regionManager.RequestNavigate("ContentRegion", nameof(DistUncompleted), new NavigationParameters
                {
                    { "", null },
                });
            });

            ShowDistCompleted = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistProgViewModel:ShowDistCompleted");

                // fixme:仕分完了ボタン押下
                regionManager.RequestNavigate("ContentRegion", nameof(DistCompleted), new NavigationParameters
                {
                    { "", null },
                });
            });

            Exit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistProgViewModel:Exit");
                Application.Current.MainWindow.Close();
            });

            LoadDatas();
        }

        private void LoadDatas()
        {
            try
            {
                CollectionViewHelper.SetCollection(DistProgs, DistProgLoader.Get());
                LatestTime = DateTime.Now;
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }
    }
}
