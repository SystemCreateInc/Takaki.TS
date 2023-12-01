using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportLib.CSVModels
{
    public class KoteiMeishoFileLine
    {
        [Index(0)]
        public string CdMeishoShikibetsu { get; set; } = string.Empty;
        [Index(1)]
        public string CdMeisho { get; set; } = string.Empty;
        [Index(2)]
        public string DtTorokuNichiji { get; set; } = string.Empty;
        [Index(3)]
        public string DtKoshinNichiji { get; set; } = string.Empty;
        [Index(4)]
        public string CdHenkosha { get; set; } = string.Empty;
        [Index(5)]
        public string Nm { get; set; } = string.Empty;
        [Index(6)]
        public string NmYomi { get; set; } = string.Empty;
        [Index(7)]
        public string NmRyaku { get; set; } = string.Empty;
        [Index(8)]
        public string NmRyakuYomi { get; set; } = string.Empty;
        [Index(9)]
        public string CdKyuMeishoShikibetsu { get; set; } = string.Empty;
        [Index(10)]
        public string CdKyuMeisho { get; set; } = string.Empty;
        [Index(11)]
        public string CdEx1 { get; set; } = string.Empty;
        [Index(12)]
        public string CdEx2 { get; set; } = string.Empty;
        [Index(13)]
        public string CdEx3 { get; set; } = string.Empty;
        [Index(14)]
        public string CdEx4 { get; set; } = string.Empty;
        [Index(15)]
        public string CdEx5 { get; set; } = string.Empty;
        [Index(16)]
        public string FgEx1 { get; set; } = string.Empty;
        [Index(17)]
        public string FgEx2 { get; set; } = string.Empty;
        [Index(18)]
        public string FgEx3 { get; set; } = string.Empty;
        [Index(19)]
        public string FgEx4 { get; set; } = string.Empty;
        [Index(20)]
        public string FgEx5 { get; set; } = string.Empty;
        [Index(21)]
        public decimal? NuHaita { get; set; } = 0;
        [Index(22)]
        public string DtRenkei { get; set; } = string.Empty;
    }
}
