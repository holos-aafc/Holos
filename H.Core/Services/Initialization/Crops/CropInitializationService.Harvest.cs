using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

        public void InitializeHarvestMethod(CropViewItem viewItem)
        {
            if (viewItem.CropType.IsSilageCrop())
            {
                viewItem.HarvestMethod = HarvestMethods.Silage;
            }
            else
            {
                viewItem.HarvestMethod = HarvestMethods.CashCrop;
            }
        }

        public void InitializeUtilization(Farm farm, HarvestViewItem harvestViewItem)
        {
            if (harvestViewItem.ForageActivity == ForageActivities.Hayed)
            {
                // If item was hayed (there is a 35% loss by default)
                harvestViewItem.Utilization = 100 - farm.Defaults.PercentageOfProductReturnedToSoilForPerennials;
            }
            else
            {
                harvestViewItem.Utilization = 65; // For silage... not sure what else to put here
            }
        }

        #endregion
    }
}