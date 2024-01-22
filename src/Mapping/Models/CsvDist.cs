using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapping.Models
{
    public class CsvDist
    {
        public CsvDist(Dist dist)
        {
            DtDelivery = dist.DtDelivery;
            CdShukkaBatch = dist.CdShukkaBatch;
            CdKyoten = dist.CdKyoten;
            CdBlock = dist.CdBlock;
            TdUnitAddrCode = dist.tdunitaddrcode;
            CdHaishoBin = dist.CdHaishoBin;
            CdCourse = dist.CdCourse;
            CdRoute = dist.CdRoute.ToString();
            CdJuchuBin = dist.CdJuchuBin;
            CdTokuisaki = dist.CdTokuisaki;
            CdHimban = dist.CdHimban;
            CdGtin13 = dist.CdGtin13;
            CdGtin14 = dist.CdGtin14;
            NuOps = dist.Ops;
            NuRps = dist.Rps;
            DtTorokuNichiji = dist.DtTorokuNichiji;
            DtKoshinNichiji = dist.DtKoshinNichiji;
            CdHenkosha = dist.CdHenkosha;
        }

        [Index(0)]
        public string DtDelivery { get; set; } = string.Empty;
        [Index(1)]
        public string CdShukkaBatch { get; set; } = string.Empty;
        [Index(2)]
        public string CdKyoten { get; set; } = string.Empty;
        [Index(3)]
        public string CdBlock { get; set; } = string.Empty;
        [Index(4)]
        public string TdUnitAddrCode { get; set; } = string.Empty;
        [Index(5)]
        public string CdHaishoBin { get; set; } = string.Empty;
        [Index(6)]
        public string CdCourse { get; set; } = string.Empty;
        [Index(7)]
        public string CdRoute { get; set; } = string.Empty;
        [Index(8)]
        public string CdJuchuBin { get; set; } = string.Empty;
        [Index(9)]
        public string CdTokuisaki { get; set; } = string.Empty;
        [Index(10)]
        public string CdHimban { get; set; } = string.Empty;
        [Index(11)]
        public string CdGtin13 { get; set; } = string.Empty;
        [Index(12)]
        public string CdGtin14 { get; set; } = string.Empty;
        [Index(13)]
        public int NuOps { get; set; } = 0;
        [Index(14)]
        public int NuRps { get; set; } = 0;
        [Index(15)]
        public string DtTorokuNichiji { get; set; } = string.Empty;
        [Index(16)]
        public string DtKoshinNichiji { get; set; } = string.Empty;
        [Index(17)]
        public string CdHenkosha { get; set; } = string.Empty;
    }
}
