using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace WindowLib.Utils
{
    public static class CollectionViewHelper
    {
        public static void SetCollection<T>(ObservableCollection<T> oc, IEnumerable<T> source)
        {
            var lcv = CollectionViewSource.GetDefaultView(oc) as ListCollectionView;
            var pos = lcv?.CurrentPosition;
            SortDescription[]? sortDescription = null;

            // 並び順が変わるので一度クリアして再度追加する
            if (lcv != null)
            {
                sortDescription = lcv.SortDescriptions.Select(x => x).ToArray();
                lcv.SortDescriptions.Clear();
            }

            oc.Clear();
            oc.AddRange(source);

            if (sortDescription != null)
            {
                lcv?.SortDescriptions.AddRange(sortDescription);
            }

            if (lcv != null && pos != null && lcv.Count > pos)
            {
                lcv?.MoveCurrentToPosition((int)pos);
            }
        }
    }
}
