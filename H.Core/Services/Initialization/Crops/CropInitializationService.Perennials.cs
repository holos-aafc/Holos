using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

        public void InitializePerennialDefaults(Farm farm)
        {
            foreach (var viewItem in farm.GetAllCropViewItems()) InitializePerennialDefaults(viewItem, farm);
        }


        public void InitializePerennialDefaults(CropViewItem viewItem, Farm farm)
        {
            if (farm != null && viewItem != null && viewItem.CropType.IsPerennial())
            {
                viewItem.TillageType = TillageType.NoTill;
                viewItem.PastTillageType = TillageType.NoTill;
                viewItem.FertilizerApplicationMethodology = FertilizerApplicationMethodologies.Broadcast;
                viewItem.ForageUtilizationRate =
                    _utilizationRatesForLivestockGrazingProvider.GetUtilizationRate(viewItem.CropType);
                viewItem.TotalBiomassHarvest = viewItem.DefaultYield;
                viewItem.IsNativeGrassland = viewItem.CropType == CropType.RangelandNative;
            }
        }

        #endregion
    }
}