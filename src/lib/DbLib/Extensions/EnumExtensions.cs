using System;
using System.ComponentModel;

namespace DbLib.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            if (value == null)
                return "";

            var field = value.GetType().GetField(value.ToString());
            if (field == null)
                return "";

            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute != null ? attribute.Description : value.ToString();
        }
    }
}
