using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportLib.Models
{
    public class ShainFileLine
    {
        [Index(0)]
        public string CdShain { get; set; } = string.Empty;
        [Index(1)]
        public string DtTekiyokaishi { get; set; } = string.Empty;
        [Index(2)]
        public string DtTekiyomuko { get; set; } = string.Empty;
        [Index(3)]
        public string DtTorokuNichiji { get; set; } = string.Empty;
        [Index(4)]
        public string DtKoshinNichiji { get; set; } = string.Empty;
        [Index(5)]
        public string CdHenkosha{ get; set; } = string.Empty;
        [Index(6)]
        public string NmShain { get; set; } = string.Empty;
        [Index(7)]
        public string NmShainYomi { get; set; } = string.Empty;
        [Index(8)]
        public string NmShainYomiKana { get; set; } = string.Empty;
        [Index(9)]
        public string CdBumon { get; set; } = string.Empty;
        [Index(10)]
        public string FgTaishokusha { get; set; } = string.Empty;
        [Index(11)]
        public string StKoyo { get; set; } = string.Empty;
        [Index(12)]
        public string IfUserMainAddress { get; set; } = string.Empty;
        [Index(13)]
        public string CdYakushoku { get; set; } = string.Empty;
        [Index(14)]
        public string CdSotoYakushoku { get; set; } = string.Empty;
        [Index(15)]
        public string CdShokumu { get; set; } = string.Empty;
        [Index(16)]
        public string CdShozokuKaisha { get; set; } = string.Empty;
        [Index(17)]
        public string CdShozokuKanpani { get; set; } = string.Empty;
        [Index(18)]
        public decimal? NuHaita { get; set; } = 0;
        [Index(19)]
        public string DtRenkei { get; set; } = string.Empty;

    }
}
