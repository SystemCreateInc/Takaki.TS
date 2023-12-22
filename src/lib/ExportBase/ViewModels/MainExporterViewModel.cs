using BackendLib;
using DbLib;
using DbLib.Defs;
using DbLib.Extensions;
using ExportBase.Models;
using ExportBase.Repositories;
using ExportBase.Views;
using ExportLib;
using ExportLib.Infranstructures;
using ExportLib.Processors;
using FastExpressionCompiler.LightExpression;
using LogLib;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using WindowLib.Utils;

namespace ExportBase.ViewModels
{
    public class MainExporterViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand ExportCommand { get; set; }
        public DelegateCommand SelectFolderCommand { get; set; }
        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand AutoExportSettingsCommand { get; set; }
        public DelegateCommand ExitCommand { get; set; }
        public ObservableCollection<ExportFileViewModel> ExportFiles { get; set; } = new();

        private ObservableCollection<ProcessorInfo> _processors = new();
        public ObservableCollection<ProcessorInfo> Processors
        {
            get => _processors;
            set => SetProperty(ref _processors, value);
        }

        public ObservableCollection<Log> Logs { get; set; } = new();

        private bool _isShifted;
        public bool IsShifted
        {
            get => _isShifted;
            set => SetProperty(ref _isShifted, value);
        }

        private readonly IDialogService _dialogService;
        private IEnumerable<DataType> _dataTypes = Enumerable.Empty<DataType>();
        private readonly ExportService _service = new ExportService(new ExportRepositoryFactory());
        private readonly ScopeLogger _logger = new ScopeLogger<MainExporterViewModel>();

        public MainExporterViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            ExportCommand = new DelegateCommand(Export);
            SelectFolderCommand = new DelegateCommand(SelectFolder);
            RefreshCommand = new DelegateCommand(Refresh);
            AutoExportSettingsCommand = new DelegateCommand(AutoExportSettings);
            ExitCommand = new DelegateCommand(Exit);
        }

        private void AutoExportSettings()
        {
            _dialogService.ShowDialog(
                nameof(ExportSettingsDlg),
                new DialogParameters
                {
                    { "Processor", _service.Processors.First() },
                },
                rc =>
                {
                    if (rc.Result == ButtonResult.OK)
                    {
                        RequestReloadToExportSvr();
                        LoadDatas();
                    }
                });
        }

        private void CheckShifted()
        {
            IsShifted = (Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) != 0
                || (Keyboard.GetKeyStates(Key.RightShift) & KeyStates.Down) != 0;
        }

        private void Exit()
        {
            System.Windows.Application.Current.MainWindow.Close();
        }

        private void Refresh()
        {
            LoadDatas();
        }

        private void Export()
        {
            _dialogService.ShowDialog(
                nameof(ExportDlg),
                new DialogParameters
                {
                    { "Service", _service },
                },
                rc =>
                {
                    LoadDatas();
                });
        }

        private void SelectFolder()
        {
            if (GetSelectedFolder() is string path)
            {
                using (var wait = new WaitCursor())
                {
                    try
                    {
                        using (var repo = new ExportRepository())
                        {
                            foreach (var proc in _service.Processors)
                            {
                                proc.SetFolder(path);
                                repo.Save(proc.GetInterfaceFile());
                            }

                            repo.Commit();
                        }

                        RequestReloadToExportSvr();
                    }
                    catch (Exception ex)
                    {
                        MessageDialog.Show(_dialogService, ex.Message, "エラー");
                    }

                    LoadDatas();
                }
            }
        }

        private string? GetSelectedFolder()
        {
            using (var cofd = new CommonOpenFileDialog()
            {
                Title = "送信フォルダー選択",
                RestoreDirectory = true,
                IsFolderPicker = true
            })
            {
                return cofd.ShowDialog() == CommonFileDialogResult.Ok ? cofd.FileName : null;
            }
        }
        
        private void LoadDatas()
        {
            try
            {
                _service.UpdateAllProcessorInfo();
                _service.UpdateAvailavleExportCount();
                _service.UpdateNextExportTime();
                CollectionViewHelper.SetCollection(Processors, _service.Processors.Select(x => new ProcessorInfo(x)).ToList());

                CollectionViewHelper.SetCollection(Logs, ExportBaseRepository.GetAllLogs(_dataTypes));
            }
            catch (Exception ex)
            {
                Syslog.Error($"LoadDatas: {ex}");
                WindowLib.Utils.MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.TryGetValue("DataTypes", out IEnumerable<DataType> dataTypes))
            {
                _dataTypes = dataTypes;
            }

            if (navigationContext.Parameters.TryGetValue("DataType", out DataType dataType))
            {
                _dataTypes = new[] { dataType };
            }

            if (!_dataTypes.Any())
            {
                Debug.Assert(false);
                return;
            }

            foreach (var type in _dataTypes)
            {
                SetupProcessor(type);
            }

            LoadDatas();
        }

        private void SetupProcessor(DataType dataType)
        {
            try
            {
                switch (dataType)
                {
                    case DataType.HakoResult:
                        break;

                    default:
                        Debug.Assert(false);
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private void RequestReloadToExportSvr()
        {
            var request = 
                $"{Environment.MachineName}\n"
                + "0\n"
                + "0\n"
                + "UpdateSchedule\n";

            try
            {
                using (var be = new BackendService(ClientType.BE_CLIENTTYPE_FRONTEND, "ExportBase", "", "", ""))
                {
                    be.PostMessageByName("PROC_EXPORT", BackendService.BEM_CORE, Encoding.ASCII.GetBytes(request), BackendService.BEMS_NONEEDRESULT);
                }
            }
            catch (Exception ex)
            {
                MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
        }
    }
}
