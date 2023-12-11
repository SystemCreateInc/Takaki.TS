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
#if false
        public DistColor? GetDistZone(int zoneno)
        {
            return DistZones?.Find(x=>x.Zoneno == zoneno);
        }
        public DistColor? GetDistZoneIndex(int zoneidx)
        {
            return DistZones?.Find(x => x.Zoneidx == zoneidx);
        }
        public DistColor? GetDistZoneButton(int tabletdid)
        {
            foreach (var zone in DistZones!)
            {
                var zoneaddr = zone.Zoneaddrs?.Find(x => x.Tabletid == tabletdid);
                if (zoneaddr != null)
                {
                    return zone;
                }
            }
            return null;
        }
#endif
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

        public DistColor? GetNetDistSeq(int distseq, string tdunitaddrcode)
        {
            var tmp = DistColors!.Where(x => x.DistSeq != 0 && distseq < x.DistSeq)
                .OrderBy(x => x.DistSeq).ToList();

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
