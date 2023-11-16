using System.Text.Json.Serialization;
using System.Windows;

namespace ColumnVisibilityLib.Models
{
    public class ColumnVisibilityInfo
    {
        public string SortPathName { get; set; } = string.Empty;
        public bool IsVisible { get; set; } = true;

        [JsonIgnore]
        public Visibility ColumnVisiblity => IsVisible ? Visibility.Visible : Visibility.Collapsed;
        [JsonIgnore]
        public string ColumnName { get; set; } = string.Empty;
    }
}
