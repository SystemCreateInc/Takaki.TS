namespace DbLib.Extensions
{
    public static class StringExtensions
    {
        public static string GetDate(this string value)
        {
            if (value == null || value.Length != 8)
                return value;

            return value.Insert(4, "/").Insert(7, "/");
        }
    }
}
