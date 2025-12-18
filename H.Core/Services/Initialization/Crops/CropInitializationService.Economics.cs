using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

        public void InitializeEconomicDefaults(Farm farm)
        {
            foreach (var viewItem in farm.GetAllCropViewItems()) InitializeEconomicDefaults(viewItem, farm);
        }

        public void InitializeEconomicDefaults(
            CropViewItem cropViewItem,
            Farm farm)
        {
            var soilData = farm.GetPreferredSoilData(cropViewItem);

            cropViewItem.CropEconomicData.IsInitialized = false;

            cropViewItem.CropEconomicData = _economicsProvider.Get(
                cropViewItem.CropType,
                soilData.SoilFunctionalCategory,
                soilData.Province);

            _economicsHelper.ConvertValuesToMetricIfNecessary(cropViewItem.CropEconomicData, farm);

            cropViewItem.CropEconomicData.IsInitialized = true;
        }

        #endregion
    }
}