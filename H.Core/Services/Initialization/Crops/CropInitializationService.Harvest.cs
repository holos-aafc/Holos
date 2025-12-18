using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

        public void InitializeHarvestMethod(Farm farm)
        {
            foreach (var viewItem in farm.GetAllCropViewItems())
            {
                this.InitializeHarvestMethod(viewItem);
            }
        }

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

        public void InitializeHarvestLoss(Farm farm)
        {
            foreach (var cropViewItem in farm.GetAllCropViewItems())
            {
                foreach (var harvestViewItem in cropViewItem.HarvestViewItems)
                {
                    this.InitializeHarvestLoss(farm, harvestViewItem);
                }
            }
        }

        public void InitializeHarvestLoss(Farm farm, HarvestViewItem harvestViewItem)
        {
            if (harvestViewItem.ForageActivity == ForageActivities.Hayed)
            {
                // If item was hayed (there is a 35% loss by default)
                harvestViewItem.HarvestLossPercentage = 35;
            }
            else
            {
                harvestViewItem.HarvestLossPercentage = 65; // For silage... not sure what else to put here
            }
        }

        #endregion
    }
}