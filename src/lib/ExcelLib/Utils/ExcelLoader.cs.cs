using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using ExcelLib.Models;
using Microsoft.Office.Interop.Excel;

namespace ExcelLib.Utils
{
    public class ExcelLoader
    {
        public IEnumerable<Sheet> GetSheetData(string sourcePath)
        {
            using (var excelApp = ComWrapperFactory.Create(new Application()))
            using (var wbs = ComWrapperFactory.Create(excelApp.Obj.Workbooks.Open(sourcePath, 0, true)))
            using (var shs = ComWrapperFactory.Create(wbs.Obj.Sheets))
            {
                // 00で始まるシートが出てきた回数
                var list = new List<Sheet>();
                for (var i = 1; i <= shs.Obj.Count; ++i)
                {
                    using (var ws = ComWrapperFactory.Create(shs.Obj[i]))
                    {
                        // 00で始まるシート名は取込を行わない
                        //var regex = @"^00";
                        //if (Regex.IsMatch(ws.Obj.Name, regex))
                        //{
                        //    manuscriptCount++;
                        //    continue;
                        //}

                        //// 出力対象Rangeの取得
                        using (var firstRange = ComWrapperFactory.Create(ws.Obj.Cells[1, 1]))
                        using (var lastRange = ComWrapperFactory.Create(ws.Obj.Cells[ws.Obj.UsedRange.Rows.count, ws.Obj.UsedRange.Columns.Count]))
                        using (var tagRng = ComWrapperFactory.Create(ws.Obj.Range[firstRange.Obj, lastRange.Obj]))
                        {
                            var sheetAr = (object[,])tagRng.Obj.Value2;
                            var columnCount = sheetAr.GetLength(1);
                            var ar = sheetAr
                                .Cast<object>()
                                .Select((v, i) => (v, i))
                                .GroupBy(x => x.i / columnCount)
                                .Select(x => x.Select(y => y.v).ToArray())
                                .ToArray();

                            list.Add(new Sheet(i, ws.Obj.Name, ar));
                        }
                    }
                }

                //// タスクマネージャーに残る為、終了させる
                excelApp.Obj.Quit();
                return list;
            }
        }
    }
}
