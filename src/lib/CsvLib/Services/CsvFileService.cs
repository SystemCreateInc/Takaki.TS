﻿using CsvLib.Models;
using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace CsvLib.Services
{
    public static class CsvFileService
    {
        // 指定PathCSV保存
        public static void Save(object obj, IEnumerable<string>? rows, string configName = "", string defaultFileName = "")
        {
            var dataGrid = obj as DataGrid;
            if (dataGrid == null)
            {
                throw new ArgumentNullException(nameof(dataGrid));
            }

            if (rows == null || rows.Count() == 0)
            {
                return;
            }

            var needConfig = !string.IsNullOrEmpty(configName);
            var defaultPath = string.Empty;

            // エクスプローラ初期Path取得
            if (needConfig)
            {
                defaultPath = ConfigManager.GetConfig(configName);
            }

            // 保存先選択
            var savePath = FileDirectorySelector.SelectSavePath(defaultPath ?? string.Empty, defaultFileName);

            if (string.IsNullOrEmpty(savePath))
            {
                return;
            }

            // CSV保存
            CsvManager.Create(dataGrid, rows, savePath);
            Syslog.Debug($"CsvManager SelectSavePath:{savePath}");

            // 初期Path保存
            if (needConfig)
            {
                ConfigManager.SetConfig(savePath, configName);
            }
        }

        // 指定CSVから指定型のList取得
        public static List<T> Load<T>(string configName = "")
        {
            var needConfig = !string.IsNullOrEmpty(configName);
            var defaultPath = string.Empty;

            if (needConfig)
            {
                defaultPath = ConfigManager.GetConfig(configName);
            }

            // 読込ファイル選択
            var loadPath = FileDirectorySelector.SelectLoadFile(defaultPath ?? string.Empty);

            if (string.IsNullOrEmpty(loadPath))
            {
                return new List<T>();
            }

            // CSV読込(型指定List)
            var loadFiles = CsvManager.Load<T>(loadPath);
            Syslog.Debug($"CsvManager SelectLoadFile:{loadPath}");

            if (needConfig)
            {
                ConfigManager.SetConfig(loadPath, configName);
            }

            return loadFiles;
        }
    }
}
