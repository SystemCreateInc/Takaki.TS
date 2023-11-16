using ColumnVisibilityLib.Models;
using ColumnVisibilityLib.Views;
using LogLib;
using Prism.Services.Dialogs;
using ProcessorLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ColumnVisibilityLib.Services
{
    public class ColumnVisibilityService
    {
        public static void ShowDialog(IDialogService dialogService, object obj, IColumnVisibility columnVisibility)
        {
            var dataGrid = obj as System.Windows.Controls.DataGrid;
            if (dataGrid == null)
            {
                return;
            }

            // 現在のGridから設定画面を表示
            var columnVisibilities = dataGrid!.Columns.Select(x => new ColumnVisibilityInfo
            {
                SortPathName = x.SortMemberPath,
                ColumnName = x.Header?.ToString() ?? "",
                IsVisible = x.Visibility == Visibility.Visible,
            }).ToList();

            dialogService.ShowDialog(
                nameof(SelectColumnVisibilityDlg),
                new DialogParameters
                {
                    { "ColumnVisibilities", columnVisibilities },
                },
                (rc) =>
                {
                    if (rc.Result != ButtonResult.OK)
                    {
                        return;
                    }

                    // visibilityの適用
                    columnVisibility.SetVisibility(rc.Parameters.GetValue<List<ColumnVisibilityInfo>>("ColumnVisibilities"));
                });
        }

        // 行表示状態読込
        public static void LoadVisibilities(string viewName, IColumnVisibility columnVisibility)
        {
            try
            {
                var configValue = ConfigManager.GetConfig($"{viewName}Column");
                if (configValue is null)
                {
                    throw new Exception($"config[{nameof(viewName)}Column] is not Found");
                }

                var type = columnVisibility.GetType();
                var configVisibilities = Json.Deserialize(configValue, type);
                if (configVisibilities is null)
                {
                    throw new Exception($"config[{nameof(viewName)}Column, {nameof(type)}] Deserialize Error");
                }

                columnVisibility.LoadVisibility(configVisibilities);
            }
            catch (Exception e)
            {
                Syslog.Error($"ColumnVisibilityService.LoadVisibilities:{e.Message}");
            }
        }
    }
}
