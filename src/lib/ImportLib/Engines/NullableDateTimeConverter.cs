using CsvHelper.Configuration;
using CsvHelper;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportLib.Engines
{
    internal class NullableDateTimeConverter : DefaultTypeConverter
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            return DateTime.TryParse(text, out var result) ? result : null;
        }
    }
}
