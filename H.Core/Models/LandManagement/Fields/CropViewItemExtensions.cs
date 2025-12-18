using System.Collections.Generic;
using System.Linq;

namespace H.Core.Models.LandManagement.Fields
{
    public static class CropViewItemExtensions
    {
        public static List<CropViewItem> GetByYear(this IEnumerable<CropViewItem> viewItems, int year)
        {
            if (viewItems.Any() == false)
            {
                return new List<CropViewItem>();
            }
            else
            {
                return viewItems.Where(x => x.Year == year).ToList();
            }
        }

        /// <summary>
        /// Gets all unique years from a collection of items
        /// </summary>
        public static List<int> GetDistinctYears(this IEnumerable<CropViewItem> viewItems)
        {
            return viewItems.Select(x => x.Year).Distinct().OrderBy(x => x).ToList();
        }

        /// <summary>
        /// Get all items from a collection that have the same year
        /// </summary>
        public static List<CropViewItem> GetItemsByYear(this IEnumerable<CropViewItem> viewItems, int year)
        {
            return viewItems.Where(x => x.Year == year).ToList();
        }

        public static List<CropViewItem> GetSecondaryCrops(this IEnumerable<CropViewItem> viewItems, CropViewItem mainCropViewItem)
        {
            return viewItems.GetSecondaryCrops().Except(new List<CropViewItem>() { mainCropViewItem }).ToList();
        }

        public static List<CropViewItem> GetSecondaryCrops(this IEnumerable<CropViewItem> viewItems)
        {
            return viewItems.OrderBy(x => x.Year).Where(x => x.IsSecondaryCrop).ToList();
        }

        public static List<CropViewItem> GetMainCrops(this IEnumerable<CropViewItem> viewItems)
        {
            return viewItems.OrderBy(x => x.Year).Where(x => x.IsSecondaryCrop == false).ToList();
        }
    }
}