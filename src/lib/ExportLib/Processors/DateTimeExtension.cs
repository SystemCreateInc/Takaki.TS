using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportLib.Processors
{
    public static class DateTimeExtension
    {
        public static string ToDateTimeText(this DateTime dt) => dt.ToString("yyyyMMddHHmmss");
        public static string ToDateText(this DateTime dt) => dt.ToString("yyyyMMdd");
        public static string ToTimeText(this DateTime dt) => dt.ToString("HHmmss");
        public static string ToTimeText00(this DateTime dt) => dt.ToString("HHmmss00");
    }
}
