using Picking.Defs;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TdDpsLib.Models;

namespace Picking.Models
{
    public class DistColorInfo : BindableBase
    {
        private int _distworktype = 1;
        public int DistWorkType
        {
            get => _distworktype;
            set
            {
                SetProperty(ref _distworktype, value);
                DistWorkTypeName = _distworktype == 0 ? "＜追駆け仕分＞" : "＜一斉仕分＞";
                DistTypeColor = _distworktype == 0 ? ConsoleColor.Blue : ConsoleColor.Green;
            }
        }
        private string _distworktypename = "";
        public string DistWorkTypeName
        {
            get => _distworktypename;
            set => SetProperty(ref _distworktypename, value);
        }
        public bool IsDistWorkNormal
        {
            get => DistWorkType == 1 ? true : false;
        }
        private ConsoleColor _disttypecolor = ConsoleColor.Black;
        public ConsoleColor DistTypeColor
        {
            get => _disttypecolor;
            set => SetProperty(ref _disttypecolor, value);
        }

        // 配分順番
        private int _distseq = 0;
        public int DistSeq
        {
            get => _distseq;
            set => SetProperty(ref _distseq, value);
        }

        public DistColor? GetDistColor(int color)
        {
            return DistColors?.Find(x => x.DistColor_code == color);
        }
        // ５色
        private List<DistColor>? _distcolors = null;

        public List<DistColor>? DistColors
        {
            get => _distcolors;
            set => SetProperty(ref _distcolors, value);
        }
        public bool IsWorking()
        {
            foreach (var item in DistColors!)
            {
                if (item.IsWorking() == true)
                    return true;
            }

            return false;
        }

        // 一斉配分で次の色に点灯表示器があるか
        public DistColor? GetNextDistSeq(int distseq, string tdunitaddrcode)
        {
            // DistSeqs[1]固定
            var tmp = DistColors!.Where(x => x.DistSeqs[1] != 0 && distseq < x.DistSeqs[1])
                .OrderBy(x => x.DistSeqs[1]).ToList();

            foreach (var distcolor in tmp)
            {
                if(distcolor.Tdunitdisplay.Find(x => x.Tdunitaddrcode == tdunitaddrcode)!=null)
                {
                    return distcolor;
                }
            }

            return null;
        }

        // 担当者実績
        public List<ReportShain> RepotShains = new List<ReportShain>();

        ReportShain? GetReportShain(string cdshain)
        {
            return RepotShains.Find(x => x.CdShain == cdshain);
        }

        public void ReportUpdate(ReportShain reportshain, int distworkmode)
        {
            var report = GetReportShain(reportshain.CdShain);
            if (report == null)
            {
                // 追加
                RepotShains.Add(new ReportShain
                {
                    CdShain = reportshain.CdShain,
                    NmShain = reportshain.NmShain,
                    DtWorkStart = reportshain.DtWorkStart,
                });
            }

            report = GetReportShain(reportshain.CdShain);
            if (report != null)
            {
                report.DtWorkEnd = DateTime.Now;
                if (reportshain.DistWorkMode == (int)DistWorkMode.Dist)
                {
                    report.WorkTime += reportshain.WorkTime;
                    report.ItemCnt += reportshain.ItemCnt;
                    report.ShopCnt += reportshain.addrs.Count();
                    report.DistCnt += reportshain.DistCnt;
                }
                else
                {
                    report.CheckTime += reportshain.WorkTime;
                    report.CheckCnt++;
                }
            }
        }
    }
}
