using DbLib.DbEntities;
using DbLib.Defs;
using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Mapping.Models
{
    public class Dist
    {
        public Dist()
        {
        }

        public Dist(Dist d, BoxType boxtype)
        {
            Id = 0;
            DtDelivery = d.DtDelivery;
            CdKyoten = d.CdKyoten;
            NmKyoten = d.NmKyoten;
            CdShukkaBatch = d.CdShukkaBatch;
            NmShukkaBatch = d.NmShukkaBatch;
            CdLargeGroup = d.CdLargeGroup;
            NmLargeGroup = d.NmLargeGroup;
            CdBlock = d.CdBlock;
            CdCourse = d.CdCourse;
            CdRoute = d.CdRoute;
            CdTokuisaki = d.CdTokuisaki;
            NmTokuisaki = d.NmTokuisaki;
            CdHimban = d.CdHimban;
            NmHinSeishikimei = d.NmHinSeishikimei;
            CdSumTokuisaki = d.CdSumTokuisaki;
            NmSumTokuisaki = d.NmSumTokuisaki;
            CdSumCourse = d.CdSumCourse;
            CdSumRoute = d.CdSumRoute;
            tdunitaddrcode = d.tdunitaddrcode;
            MappingSeq = d.MappingSeq;

            // 箱
            StBoxType = (int)boxtype;
            NuBoxCnt = 0;
            CdHaiShobin = d.CdHaiShobin;
        }

        public long Id = 0;
        public string CdKyoten = string.Empty;
        public string NmKyoten = string.Empty;
        public string CdShukkaBatch = string.Empty;
        public string NmShukkaBatch = string.Empty;
        public string CdLargeGroup = string.Empty;
        public string NmLargeGroup = string.Empty;
        public string CdBlock = string.Empty;
        public string CdCourse = string.Empty;
        public int CdRoute = 0;
        public string CdTokuisaki = string.Empty;
        public string NmTokuisaki = string.Empty;
        public string CdHimban = string.Empty;
        public string NmHinSeishikimei = string.Empty;
        public string CdSumTokuisaki = string.Empty;
        public string NmSumTokuisaki = string.Empty;
        public string CdSumCourse = string.Empty;
        public int CdSumRoute = 0;
        public string tdunitaddrcode = string.Empty;
        public int MappingSeq = 0;
        public int Maguchi = 0;

        public int StBoxType = 0;
        public int NuBoxCnt = 0;
        public string CdHaiShobin = string.Empty;
        public string DtDelivery = string.Empty;

        public int LargeBox = 0;    // 厚箱
        public int SmallBox = 0;    // 薄箱

        public decimal GetBoxSize()
        {
            return (decimal)LargeBox + (decimal)((decimal)SmallBox / 2);
        }
        public int GetMaguchi(decimal NuThreshold)
        {
            decimal a = GetBoxSize();
            decimal box = Math.Floor(GetBoxSize() / NuThreshold);

            int resut =  box * NuThreshold == GetBoxSize() ? (int)box : (int)box + 1;
            Syslog.Info($"しきい値計算 Seq[{MappingSeq}] CdShukkaBatch[{CdShukkaBatch}] CdSumCourse[{CdSumCourse}] CdSumRoute[{CdSumRoute}] CdCourse[{CdCourse}] CdRoute[{CdRoute}] CdSumTokuisaki[{CdSumTokuisaki}] CdTokuisaki[{CdTokuisaki}] しきい値[{NuThreshold}] 箱[{GetBoxSize()}] 間口[{resut}]");
            return resut;
        }

    }
}
