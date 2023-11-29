﻿using DbLib.Defs.DbLib.Defs;
using DbLib.Extensions;
using ImportLib.Engines;
using ImportLib.Models;
using ImportLib.Repositories;
using ImportLib.Views;
using LogLib;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using WindowLib.Utils;

namespace ImportLib.ViewModels
{
    public class MainImporterViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand ImportCommand { get; set; }
        public DelegateCommand SelectFolderCommand { get; set; }
        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand ExitCommand { get; set; }
        public ObservableCollection<ImportFileViewModel> ImportFiles { get; set; } = new();

        public ObservableCollection<Log> Logs { get; set; } = new();

        private bool _isShifted;
        public bool IsShifted
        {
            get => _isShifted;
            set => SetProperty(ref _isShifted, value);
        }

        private readonly IDialogService _dialogService;
        private List<IImportEngine> _engines = new List<IImportEngine>();
        private List<DataType> _dataTypes = new List<DataType>();
        //private readonly ScopeLogger _logger = new ScopeLogger<MainImporterViewModel>();

        public MainImporterViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            ImportCommand = new DelegateCommand(Import);
            SelectFolderCommand = new DelegateCommand(SelectFolder);
            RefreshCommand = new DelegateCommand(Refresh);
            ExitCommand = new DelegateCommand(Exit);
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

        private void Import()
        {
            var selectEngines = GetSelectEngines();

            if (!selectEngines.Any())
            {
                MessageDialog.Show(_dialogService, "取り込むファイルを選択して下さい", "エラー");
                return;
            }

            _dialogService.ShowDialog(
                nameof(ImportDlg),
                new DialogParameters
                {
                    { "Engines", selectEngines },
                },
                rc =>
                {
                    LoadDatas();
                });
        }

        private List<IImportEngine> GetSelectEngines()
        {
            var selectNames = ImportFiles.Where(x => x.Selected).Select(x => x.Name);
            return _engines.Where(x => selectNames.Contains(x.DataName)).ToList();
        }

        private void SelectFolder()
        {
            if (GetSelectedFolder() is string path)
            {
                try
                {
                    foreach (var engine in _engines)
                    {
                        using (var repo = new ImportRepository())
                        {
                            engine!.SetFolder(path);
                            repo.Save(engine.GetInterfaceFile());
                        }
                    }
                }
                catch (Exception ex)
                {

                    MessageDialog.Show(_dialogService, ex.Message, "エラー");
                }

                LoadDatas();
            }
        }

        private string? GetSelectedFolder()
        {
            using (var cofd = new CommonOpenFileDialog()
            {
                Title = "取り込みフォルダー選択",
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
                ImportFiles.Clear();
                foreach (var engine in _engines)
                {
                    if (engine is not null)
                    {
                        engine.UpdateImportFileInfo();
                        ImportFiles.Add(new ImportFileViewModel
                        {
                            Selected = engine.IsExistFile,
                            Name = engine.DataName,
                            FilePath = engine.ImportFilePath,
                            FileSize = engine.ImportFileSize,
                            LastWriteTime = engine.ImportFileLastWriteDateTime,
                        });
                    }
                }

                CollectionViewHelper.SetCollection(Logs, ImportRepository.GetAllLogs(_dataTypes));
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                WindowLib.Utils.MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (!navigationContext.Parameters.TryGetValue("ImportType", out ImportType importType))
            {
                Debug.Assert(false);
                return;
            }

            SetDataTypes(importType);

            try
            {
                using (var repo = new ImportRepository())
                {
                    foreach (var dataType in _dataTypes)
                    {
                        var interfaceFile = repo.GetInterfaceFileByDataType(dataType);
                        if (interfaceFile is null)
                        {
                            MessageDialog.Show(_dialogService, "取り込みデータの情報を取得できませんでした" +
                                $"{dataType.GetDescription()}の設定をinterfaceFilesテーブルに追加してください。", "エラー");
                            return;
                        }

                        switch (dataType)
                        {
                            // マスタ受信
                            case DataType.Kyoten:
                                _engines.Add(new MKyotenImportEngine(interfaceFile));
                                break;

                            case DataType.Shain:
                                _engines.Add(new MShainImportEngine(interfaceFile));
                                break;

                            case DataType.Tokuisaki:
                                _engines.Add(new MTokuisakiImportEngine(interfaceFile));
                                break;

                            case DataType.Himmoku:
                                _engines.Add(new MHimmokuImportEngine(interfaceFile));
                                break;

                            case DataType.ShukkaBatch:
                                _engines.Add(new MShukkaBatchImportEngine(interfaceFile));
                                break;

                            case DataType.Kotei:
                                _engines.Add(new MKoteiMeishoImportEngine(interfaceFile));
                                break;

                            //// 出荷ﾃﾞｰﾀ受信
                            //case DataType.Pick:
                            //    _engines.Add(new MKyotenImportEngine(interfaceFile));
                            //    break;

                            //case DataType.Hako:
                            //    _engines.Add(new MKyotenImportEngine(interfaceFile));
                            //    break;

                            //case DataType.PickResult:
                            //    _engines.Add(new MKyotenImportEngine(interfaceFile));
                            //    break;

                            //case DataType.HakoResult:
                            //    _engines.Add(new MKyotenImportEngine(interfaceFile));
                            //    break;

                            default:
                                Debug.Assert(false);
                                break;
                        }
                    }
                }

                LoadDatas();
            }
            catch (Exception ex)
            {
                MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
        }

        private void SetDataTypes(ImportType importType)
        {
            if (importType == ImportType.Master)
            {
                _dataTypes.Add(DataType.Kyoten);
                _dataTypes.Add(DataType.Shain);
                _dataTypes.Add(DataType.Tokuisaki);
                _dataTypes.Add(DataType.Himmoku);
                _dataTypes.Add(DataType.ShukkaBatch);
                _dataTypes.Add(DataType.Kotei);
            }
            else
            {
                _dataTypes.Add(DataType.Pick);
                _dataTypes.Add(DataType.Hako);
                _dataTypes.Add(DataType.PickResult);
                _dataTypes.Add(DataType.HakoResult);
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}
