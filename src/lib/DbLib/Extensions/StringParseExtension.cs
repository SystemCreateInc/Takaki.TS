namespace DbLib.Extensions
{
    public static class StringParseExtension
    {
        public static int ParseIntOrDefault(this string? text, int defvalue = 0)
        {
            if (!Int32.TryParse(text, out int value))
            {
                return defvalue;
            }

            return value;
        }

        public static decimal ParseDecimalOrDefault(this string? text, decimal defvalue = 0)
        {
            if (!decimal.TryParse(text, out decimal value))
            {
                return defvalue;
            }

            return value;
        }

        public static double ParseDoubleOrDefault(this string? text, double defvalue = 0)
        {
            if (!double.TryParse(text, out double value))
            {
                return defvalue;
            }

            return value;
        }

        public static DateTime? ParseDateTimeOrDefault(this string? text, DateTime? defvalue = null)
        {
            if (!double.TryParse(text, out double value))
            {
                return defvalue;
            }

            return DateTime.FromOADate(value);
        }
    }
}
