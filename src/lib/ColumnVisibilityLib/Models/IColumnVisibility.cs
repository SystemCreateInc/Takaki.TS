using System.Collections.Generic;

namespace ColumnVisibilityLib.Models
{
    public interface IColumnVisibility
    {
        void LoadVisibility(object configVisibilities);
        void SetVisibility(List<ColumnVisibilityInfo> columnVisibilities);
    }
}
