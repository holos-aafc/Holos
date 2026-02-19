#if !WINDOWS
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Collections.ObjectModel
{
    public static class ObservableCollectionAddRangePolyfill
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}
#endif
