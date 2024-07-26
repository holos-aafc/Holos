using H.Core.Models.LandManagement.Fields;
using H.Core.Models;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

        /// <summary>
        /// Reinitialize each <see cref="CropViewItem"/> within <see cref="Farm"/> with new default values
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> that will be reinitialized to new default values</param>
        public void InitializeHerbicideEnergy(Farm farm)
        {
            var viewItems = farm.GetCropDetailViewItems();
            foreach (var viewItem in viewItems)
            {
                InitializeHerbicideEnergy(farm, viewItem);
            }
        }

        /// <summary>
        /// Reinitialize the <see cref="CropViewItem"/> from the selected <see cref="Farm"/> with new default values
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant data to pass into <see cref="Table_51_Herbicide_Energy_Estimates_Provider"/></param>
        /// <param name="viewItem">The <see cref="CropViewItem"/> that will have its values reset with new default values</param>
        public void InitializeHerbicideEnergy(Farm farm, CropViewItem viewItem)
        {
            var soilData = farm.GetPreferredSoilData(viewItem);
            var herbicideEnergyEstimates = _herbicideEnergyEstimatesProvider.GetHerbicideEnergyDataInstance(
                provinceName: soilData.Province,
                soilCategory: soilData.SoilFunctionalCategory,
                tillageType: viewItem.TillageType,
                cropType: viewItem.CropType);

            if (herbicideEnergyEstimates != null)
            {
                viewItem.HerbicideEnergy = herbicideEnergyEstimates.HerbicideEstimate;
            }
        }

        /// <summary>
        /// Reinitialize each <see cref="CropViewItem"/> within <see cref="Farm"/> with new default values
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> that will be reinitialized to new default values</param>
        public void InitializeFuelEnergy(Farm farm)
        {
            var viewItems = farm.GetCropDetailViewItems();
            foreach (var viewItem in viewItems)
            {
                InitializeFuelEnergy(farm, viewItem);
            }
        }

        /// <summary>
        /// Reinitialize the <see cref="CropViewItem"/> from the selected <see cref="Farm"/> with new default values
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant data to pass into <see cref="Table_50_Fuel_Energy_Estimates_Provider"/></param>
        /// <param name="viewItem">The <see cref="CropViewItem"/> that will have its values reset with new default values</param>
        public void InitializeFuelEnergy(Farm farm, CropViewItem viewItem)
        {
            var soilData = farm.GetPreferredSoilData(viewItem);
            var fuelEnergyEstimates = _fuelEnergyEstimatesProvider.GetFuelEnergyEstimatesDataInstance(
                province: soilData.Province,
                soilCategory: soilData.SoilFunctionalCategory,
                tillageType: viewItem.TillageType,
                cropType: viewItem.CropType);

            if (fuelEnergyEstimates != null)
            {
                viewItem.FuelEnergy = fuelEnergyEstimates.FuelEstimate;
            }
        }

        #endregion
    }
}