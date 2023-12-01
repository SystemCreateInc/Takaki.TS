using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportLib.CSVModels
{
    public class KyotenFileLine
    {
        [Index(0)]
        public string CdKyoten { get; set; } = string.Empty;
        [Index(1)]
        public string DtTekiyokaishi { get; set; } = string.Empty;
        [Index(2)]
        public string DtTekiyomuko { get; set; } = string.Empty;
        [Index(3)]
        public string DtTorokuNichiji { get; set; } = string.Empty;
        [Index(4)]
        public string DtKoshinNichiji { get; set; } = string.Empty;
        [Index(5)]
        public string CdHenkosha { get; set; } = string.Empty;
        [Index(6)]
        public string NmKyoten { get; set; } = string.Empty;
        [Index(7)]
        public string CdKyotenShubetsu { get; set; } = string.Empty;
        [Index(8)]
        public string CdTorihikisaki { get; set; } = string.Empty;
        [Index(9)]
        public string CdZaikoHikiateBumon { get; set; } = string.Empty;
        [Index(10)]
        public string NmKyotenRyakusho { get; set; } = string.Empty;
        [Index(11)]
        public string CdKyotenBumon { get; set; } = string.Empty;
        [Index(12)]
        public string StSesankanriNippaihin { get; set; } = string.Empty;
        [Index(13)]
        public string StSeisankanriZaikohin { get; set; } = string.Empty;
        [Index(14)]
        public string StShikibetsu { get; set; } = string.Empty;
        [Index(15)]
        public string CdTenpoBrand { get; set; } = string.Empty;
        [Index(16)]
        public string CdBasho { get; set; } = string.Empty;
        [Index(17)]
        public string CdKyotenZouusei { get; set; } = string.Empty;
        [Index(18)]
        public decimal? NuHaita { get; set; } = 0;
        [Index(19)]
        public string DtRenkei { get; set; } = string.Empty;
    }
}
