using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace H.Infrastructure
{
    public static class ObservableCollectionExtensions
    {
        /// <summary>
        /// A bug exists in the Telerik GridViewComboBox when calling ObservableCollection.Update() - it doesn't work. This methods is a workaround.
        /// </summary>
        public static void UpdateItems<T>(this ObservableCollection<T> collection, IEnumerable<T> validItems)
        {
            var itemsToRemove = collection.Where(x => validItems.Contains(x) == false).ToList();

            foreach (var item in itemsToRemove)
            {
                collection.Remove(item);
            }

            foreach (var validItem in validItems)
            {
                if (collection.Contains(validItem) == false)
                {
                    collection.Add(validItem);
                }
            }
        }
    }
}