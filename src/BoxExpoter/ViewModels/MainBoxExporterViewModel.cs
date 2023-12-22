using Microsoft.Identity.Client;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using WindowLib.Utils;
using Prism.Services.Dialogs;
using BoxExpoter.Infranstructures;
using BoxExpoter.Views;
using ExportLib.Infranstructures;
using Microsoft.WindowsAPICodePack.Dialogs;
using DbLib.Defs;
using ExportLib.Models;
using ExportBase.Views;
using ExportLib;
using ExportLib.Processors;
using System.IO;

namespace BoxExpoter.ViewModels
{
    public class MainBoxExporterViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand SendCommand { get; }
        public DelegateCommand RefreshCommand { get; }
        public DelegateCommand PathCommand { get; }
        public DelegateCommand ExitCommand { get; }

        private ObservableCollection<GroupStowage>  _items = new();
        public ObservableCollection<GroupStowage> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private DateTime _dtDelivery;
        public DateTime DtDelivery
        {
            get => _dtDelivery;
            set => SetProperty(ref _dtDelivery, value);
        }

        private bool _canSend;
        public bool CanSend
        {
            get => _canSend;
            set => SetProperty(ref _canSend, value);
        }

        private IDialogService _dialogService;

        public MainBoxExporterViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            SendCommand = new DelegateCommand(Send).ObservesCanExecute(() => CanSend);
            RefreshCommand = new(Refresh);
            PathCommand = new(SelectFolder);
            ExitCommand = new(Exit);

            SelectDeliveryDate();
        }

        private void SelectDeliveryDate()
        {
            _dialogService.ShowDialog(nameof(DeliveryDateDialog),
                rc =>
                {
                    if (rc.Result != ButtonResult.OK)
                    {
                        Exit();
                        return;
                    }

                    DtDelivery = rc.Parameters.GetValue<DateTime>("Date");
                });
        }

        private void Exit()
        {
            Application.Current.MainWindow.Close();
        }

        private void SelectFolder()
        {
            using (var repo = new ExportRepository())
            {
                if (repo.GetInterfaceFile(DataType.HakoResult) is InterfaceFile interfaceFile)
                {
                    if (GetSelectedFolder(interfaceFile.FileName) is string path)
                    {
                        try
                        {
                            var fileName = Path.GetFileName(interfaceFile.FileName);

                            var newInterfaceFile = interfaceFile with { FileName = Path.Combine(path, fileName) };
                            repo.Save(newInterfaceFile);
                            repo.Commit();
                        }
                        catch (Exception ex)
                        {

                            MessageDialog.Show(_dialogService, ex.Message, "エラー");
                        }
                    }
                }
            }
        }

        private string? GetSelectedFolder(string fileName)
        {
            using (var cofd = new CommonOpenFileDialog()
            {
                Title = "データ送信先フォルダー選択",
                RestoreDirectory = true,
                IsFolderPicker = true,
                DefaultDirectory = fileName,
            })
            {
                return cofd.ShowDialog() == CommonFileDialogResult.Ok ? cofd.FileName : null;
            }
        }

        private void Refresh()
        {
            try
            {
                foreach (var item in Items)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }

                CollectionViewHelper.SetCollection(Items, BoxExpoterQueryService.GetGroupList(DtDelivery));
                UpdateCanSend();

                foreach (var item in Items)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }
            catch (Exception ex)
            {
                MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
        }

        private void Item_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateCanSend();
        }

        private void UpdateCanSend()
        {
            CanSend = Items.Any(x => x.IsSelected);
        }

        private void Send()
        {
            var keys = Items
                .Where(x => x.IsSelected)
                .Select(x => new BoxResultKey(DtDelivery, x.CdDistGroup))
                .ToArray();

            var service = new ExportService(new ExportRepositoryFactory());
            service.AddProcessor(new BoxResultProcessor(new BoxResultRepository(keys)));

            _dialogService.ShowDialog(
                nameof(ExportDlg),
                new DialogParameters
                {
                    { "Service", service },
                },
                rc =>
                {
                    Refresh();
                });
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Refresh();
        }
    }
}
