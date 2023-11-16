using ExcelLib.Models;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using WindowLib.Utils;

namespace ExcelLib.Utils
{
    /// <summary>
    /// 指定データをExcelで表示、保存
    /// </summary>
    public class ExcelExport
    {
        /// <summary>
		/// グリッドに表示しているデータからExcel出力
		/// </summary>
		/// <param name="dataGrid"></param>
		/// <param name="title"></param>
        /// 「DataGridTemplateColumn」を使用した列は、表示する変数名を「SortMemberPath」で指定
		public void ExportFromGrid(DataGrid dataGrid, string title)
        {
            using (var busy = new WaitCursor())
            {
                if (dataGrid == null)
                    throw new ArgumentNullException(nameof(dataGrid));

                var excelDatas = CreateExportData(dataGrid);

                ExportNewFile(excelDatas, null);
                // 呼び出し元でtitle「null」指定で保存処理カット　※修正箇所多数の為ここでnull指定
                //ExportNewFile(excelDatas, title);
            }
        }

        // グリッドからExcelData作成
        private object?[,] CreateExportData(DataGrid dataGrid)
        {
            var gridPropertyInfo = new List<PropertyInfo>();

            object?[,] excelDatas = new object[dataGrid.Items.Count + 1, dataGrid.Columns.Count];

            int row = 0;
            var stringFormats = new string[dataGrid.Columns.Count];

            // 1行目ヘッダー名記載&カラム順プロパティリスト追加
            for (int ct = 0; ct < dataGrid.Columns.Count; ct++)
            {
                excelDatas[row, ct] = dataGrid.Columns[ct].Header.ToString();

                if (dataGrid.Items.Count > 0)
                {
                    gridPropertyInfo.Add(dataGrid.Items[0].GetType().GetProperty(dataGrid.Columns[ct].SortMemberPath)!);
                }

                // StringFormat取得
                stringFormats[ct] = dataGrid.Columns[ct].ClipboardContentBinding.StringFormat;
            }

            row++;

            // 2行目以降からGrid内データを記載
            foreach (var item in dataGrid.ItemsSource)
            {
                int column = 0;
                foreach (var info in gridPropertyInfo)
                {
                    if (info.GetMethod?.ReturnType.FullName?.Contains("DateTime") == true && stringFormats[column] != null)
                        excelDatas[row, column] = SetDateTimeFormatValue(info.GetValue(item)!, stringFormats[column]);
                    else
                        excelDatas[row, column] = info?.GetValue(item) ?? "";

                    column++;
                }
                row++;
            }

            return excelDatas;
        }

        // 日付フォーマットを摘要した値で取得
        private object SetDateTimeFormatValue(object value, string format)
        {
            DateTime dateTime;

            if (DateTime.TryParse(value?.ToString(), out dateTime))
                return dateTime.ToString(format);
            else
                return "";
        }

        /// <summary>
        /// Excel出力(新規)
        /// </summary>
        /// <param name="outList">Excel出力データ</param>
        /// <param name="title"></param>
        public void ExportNewFile(object?[,] outList, string? title)
        {
            try
            {
                //Excelシートのインスタンスを作る
                using (var excelApp = ComWrapperFactory.Create(new Application()))
                using (var wbs = ComWrapperFactory.Create(excelApp.Obj.Workbooks))
                using (var wb = ComWrapperFactory.Create(wbs.Obj.Add()))
                using (var shs = ComWrapperFactory.Create(wb.Obj.Sheets))
                using (var ws = ComWrapperFactory.Create(shs.Obj[1]))
                {
                    try
                    {
                        // todo:ExcelData以外の引数が全てDefaultになる。次回使用時、関数の引数をList<ExcelDataInfo>に修正
                        SetSheetData(new ExcelDataInfo(outList), ws);

                        excelApp.Obj.Visible = true;

                        if (title != null)
                        {
                            // ダイアログのインスタンスを生成
                            var dialog = new SaveFileDialog();

                            dialog.FileName = title;
                            dialog.Filter = "xlsxファイル (*.xlsx)|*.xlsx";

                            // ダイアログを表示する
                            if (dialog.ShowDialog() == true)
                                wb.Obj.SaveAs(dialog.FileName);
                        }
                        // カーソル初期位置
                        using (var range1 = ComWrapperFactory.Create(ws.Obj.Cells[1, 1]))
                        {
                            range1.Obj.Select();
                        }
                    }
                    catch
                    {
                        excelApp.Obj.Visible = true;
                        throw;
                    }
                }
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        /// <summary>
        /// Excel出力(コピー元を編集し、別名で保存)
        /// </summary>
        /// <param name="outList">Excel出力データ</param>
        /// <param name="sourcePath">コピー元ExcelのPath</param>
        public void ExportFromBaseFile(List<ExcelDataInfo> dataInfos, string title, string sourcePath)
        {
            ExportFromBaseFile(new List<List<ExcelDataInfo>> { dataInfos }, title, sourcePath);
        }

        /// <summary>
        /// Excel出力(コピー元を編集し、別名で保存)　シート複製対応
        /// </summary>
        /// <param name="outList">Excel出力データ</param>
        /// <param name="sourcePath">コピー元ExcelのPath</param>
        public void ExportFromBaseFile(List<List<ExcelDataInfo>> multiDataInfos, string title, string sourcePath)
        {
            try
            {
                //Excelシートのインスタンスを作る
                using (var excelApp = ComWrapperFactory.Create(new Application()))
                using (var wbs = ComWrapperFactory.Create(excelApp.Obj.Workbooks.Open(sourcePath, 0)))
                {
                    // データ数分のシートを複製
                    for (int x = 1; x < multiDataInfos.Count; x++)
                    {
                        wbs.Obj.Worksheets[1].Copy(After: wbs.Obj.Worksheets[x]);
                    }

                    var sheetCont = 1;

                    foreach (var dataInfos in multiDataInfos)
                    {
                        using (var shs = ComWrapperFactory.Create(wbs.Obj.Sheets))
                        using (var ws = ComWrapperFactory.Create(shs.Obj[sheetCont]))
                        {
                            // 1件目からシート名取得、設定
                            ws.Obj.name = dataInfos[0].SheetName;

                            try
                            {
                                int insertCount = 0;

                                for (int cnt = 0; cnt < dataInfos.Count; cnt++)
                                {
                                    if (dataInfos[cnt].ExcelData.Length == 0)
                                        continue;
                                    // 行挿入分を開始行に加算
                                    if (cnt != 0)
                                    {
                                        // 1回分前のデータ
                                        var prevData = dataInfos[cnt - 1];
                                        if (prevData.Insert)
                                        {
                                            insertCount += dataInfos[cnt - 1].ExcelData.GetLength(0);
                                        }

                                        // 最終行削除時-1行
                                        if (prevData.DeleteDefaultRow)
                                        {
                                            insertCount--;
                                        }
                                    }

                                    dataInfos[cnt].StartRow += insertCount;

                                    SetSheetData(dataInfos[cnt], ws);
                                }

                                // カーソル初期位置
                                using (var range1 = ComWrapperFactory.Create(ws.Obj.Cells[1, 1]))
                                {
                                    range1.Obj.Select();
                                }
                                // コピーモード終了
                                excelApp.Obj.CutCopyMode = new XlCutCopyMode();
                            }
                            catch
                            {
                                excelApp.Obj.Visible = true;
                                throw;
                            }
                        }
                        sheetCont++;
                    }

                    wbs.Obj.Worksheets[1].Select();

                    excelApp.Obj.Visible = true;

                    if (title != null)
                    {
                        // ダイアログのインスタンスを生成
                        var dialog = new SaveFileDialog();

                        dialog.FileName = title;
                        dialog.Filter = "xlsxファイル (*.xlsx)|*.xlsx";

                        // ダイアログを表示する
                        if (dialog.ShowDialog() == true)
                            wbs.Obj.SaveAs(dialog.FileName);
                    }
                }
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
        /// <summary>
        /// 指定シートにデータを入力
        /// </summary>
        /// <param name="header">ヘッダー入力行数</param>
        private void SetSheetData(ExcelDataInfo dataInfo, dynamic ws)
        {
            int maxRow = dataInfo.ExcelData.GetLength(0) + dataInfo.StartRow - 1;
            int maxCol = dataInfo.ExcelData.GetLength(1) + dataInfo.StartColumn - 1;

            // 行挿入
            if (dataInfo.Insert)
            {
                // 既存行の次へ行挿入(1行目が他セルの集計関数で指定された場合に、セル番号変化を回避)
                // todo:Insertをintにし、行挿入開始位置指定で「defaultRow」を削減可
                var defaultRow = dataInfo.InsertCopy ? 1 : 0;
                if (dataInfo.InsertCopy)
                {
                    using (var copyRange = ComWrapperFactory.Create(ws.Obj.Cells[dataInfo.StartRow, 1]))
                    {
                        copyRange.Obj.EntireRow().Copy();
                    }
                }

                using (var range1 = ComWrapperFactory.Create(ws.Obj.Cells[dataInfo.StartRow + defaultRow, 1]))
                using (var range2 = ComWrapperFactory.Create(ws.Obj.Cells[maxRow + defaultRow, 1]))
                using (var insertRange = ComWrapperFactory.Create(ws.Obj.Range[range1.Obj, range2.Obj]))
                {
                    // 行全体選択、下行の書式使用
                    insertRange.Obj.EntireRow().Insert(XlInsertShiftDirection.xlShiftDown, XlInsertFormatOrigin.xlFormatFromRightOrBelow);
                }
            }

            // 合計追記（SUM関数 2行先へ「最終行＋既存行」）
            if (dataInfo.StartSum > -1)
            {
                for (int column = dataInfo.StartColumn + dataInfo.StartSum; column <= maxCol; column++)
                {
                    using (var range1 = ComWrapperFactory.Create(ws.Obj.Cells[maxRow + 2, column]))
                    {
                        // 書式「文字列」以外
                        if (range1.Obj.NumberFormat != "@")
                            range1.Obj.FormulaR1C1 = $"=SUM(R[-{dataInfo.ExcelData.GetLength(0) + 1}]C:R[-1]C)";
                    }
                }
            }

            // 既存行削除
            if (dataInfo.DeleteDefaultRow)
            {
                using (var deleteRange = ComWrapperFactory.Create(ws.Obj.Cells[maxRow + 1, 1]))
                {
                    deleteRange.Obj.EntireRow().Delete();
                }
            }

            if (dataInfo.Format)
            {
                string[] numberFormats = GetNumberFormats(dataInfo.ExcelData, dataInfo.Header);

                // 指定列へ書式設定
                for (int col = 0; col < numberFormats.Length; col++)
                {
                    // 初期値が標準(General)の為スルー
                    if (numberFormats[col] == "General")
                        continue;

                    using (var range1 = ComWrapperFactory.Create(ws.Obj.Cells[dataInfo.Header + 1, col + 1]))
                    using (var range2 = ComWrapperFactory.Create(ws.Obj.Cells[maxRow, col + 1]))
                    using (var formatRange = ComWrapperFactory.Create(ws.Obj.Range[range1.Obj, range2.Obj]))
                    {
                        formatRange.Obj.NumberFormat = numberFormats[col];
                    }
                }
            }

            // 出力対象Rangeの取得
            using (var firstRange = ComWrapperFactory.Create(ws.Obj.Cells[dataInfo.StartRow, dataInfo.StartColumn]))
            using (var lastRange = ComWrapperFactory.Create(ws.Obj.Cells[maxRow, maxCol]))
            using (var tagRng = ComWrapperFactory.Create(ws.Obj.Range[firstRange.Obj, lastRange.Obj]))
            {
                tagRng.Obj.Value2 = dataInfo.ExcelData;
            }

            ws.Obj.Select(Type.Missing);
        }
        /// <summary>
        /// セルの書式設定セット        
        /// </summary>
        /// <param name="header">ヘッダー入力行数(書式取得データidx)</param>
        /// <returns>書式文字列</returns>
        private string[] GetNumberFormats(object?[,] excelDatas, int header)
        {
            int maxColumn = excelDatas.GetLength(1);

            string[] numberFormats = new string[maxColumn];

            // ヘッダーのみ出力時は書式設定無し
            if (excelDatas.GetLength(0) == header)
                return numberFormats;

            for (int col = 0; col < maxColumn; col++)
            {
                Console.WriteLine($"{excelDatas[header, col]}:{excelDatas[header, col]?.GetType().Name}");

                // 1行目のデータから変数名を取得
                switch (excelDatas[header, col]?.GetType().Name)
                {
                    case "String":
                        numberFormats[col] = "@";
                        break;

                    default:
                        numberFormats[col] = "General";
                        break;
                }
            }

            return numberFormats;
        }

        /// <summary>
        /// 二次元リストをExcel出力データ（二次元配列）に変換
        /// </summary>
        public object[,]? GetExcelDataFromList(List<List<object>> setDatas)
        {
            if (setDatas.Count == 0)
                return null;

            var excelDatas = new object[setDatas.Count, setDatas[0].Count];

            for (int row = 0; row < setDatas.Count; row++)
            {
                var tagetReport = setDatas[row];
                for (int col = 0; col < tagetReport.Count; col++)
                {
                    excelDatas[row, col] = tagetReport[col];
                }
            }

            return excelDatas;
        }
    }
}
