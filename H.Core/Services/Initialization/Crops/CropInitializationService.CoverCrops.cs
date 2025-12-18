using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Properties;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        public void InitializeCoverCrops(IEnumerable<CropViewItem> viewItems)
        {
            AssignCoverCropViewItemsDescription(viewItems);
        }

        public void AssignCoverCropViewItemsDescription(IEnumerable<CropViewItem> viewItems)
        {
            // Group items by year
            var viewItemsGroupedByYear = viewItems.GroupBy(x => x.Year);
            foreach (var yearGroup in viewItemsGroupedByYear)
            {
                var year = yearGroup.Key;
                foreach (var cropViewItem in yearGroup)
                    // Check for undersown status since that will be a  different description string being used
                    if (cropViewItem.IsSecondaryCrop && cropViewItem.UnderSownCropsUsed == false &&
                        cropViewItem.CropType.IsFallow() == false)
                        cropViewItem.Description = $"{Resources.CoverCropGrowingType}";
            }
        }
    }
}