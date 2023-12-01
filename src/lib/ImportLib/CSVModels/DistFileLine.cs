using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportLib.CSVModels
{
    public class DistFileLine
    {
        [Index(0)]
        public string DtDelivery { get; set; } = string.Empty;
        [Index(1)]
        public string CdJuchuBin { get; set; } = string.Empty;
        [Index(2)]
        public string CdShukkaBatch { get; set; } = string.Empty;
        [Index(3)]
        public string CdKyoten { get; set; } = string.Empty;
        [Index(4)]
        public string CdHaishoBin { get; set; } = string.Empty;
        [Index(5)]
        public string CdCourse { get; set; } = string.Empty;
        [Index(6)]
        public int CdRoute { get; set; } = 0;
        [Index(7)]
        public string CdTokuisaki { get; set; } = string.Empty;
        [Index(8)]
        public string CdHimban { get; set; } = string.Empty;
        [Index(9)]
        public string CdGtin13 { get; set; } = string.Empty;
        [Index(10)]
        public string CdGtin14 { get; set; } = string.Empty;
        [Index(11)]
        public short StBoxtype { get; set; } = 0;
        [Index(12)]
        public int NuBoxunit { get; set; } = 0;
        [Index(13)]
        public int NuOps { get; set; } = 0;
        [Index(14)]
        public string DtTorokuNichiji { get; set; } = string.Empty;
        [Index(15)]
        public string DtKoshinNichiji { get; set; } = string.Empty;
        [Index(16)]
        public string CdHenkosha { get; set; } = string.Empty;
    }
}