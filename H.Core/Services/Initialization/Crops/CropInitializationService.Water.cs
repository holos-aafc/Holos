using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using H.Core.Services.LandManagement;
using H.Core.Enumerations;
using H.Core.Providers.Carbon;
using System.Linq;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

        /// <summary>
        /// Initialize each <see cref="CropViewItem"/>'s irrigation properties with a <see cref="Farm"/>
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing <see cref="CropViewItem"/>'s to be reinitialized</param>
        public void InitializeIrrigationWaterApplication(Farm farm)
        {
            var viewItems = farm.GetAllCropViewItems();
            foreach (var viewItem in viewItems)
            {
                InitializeIrrigationWaterApplication(farm, viewItem);
            }
        }

        /// <summary>
        /// Initialize the <see cref="CropViewItem"/> irrigation properties
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> that contains the climate data and province data required for the lookup table</param>
        /// <param name="viewItem">The <see cref="CropViewItem"/> to have its irrigation properties reinitialized</param>
        public void InitializeIrrigationWaterApplication(Farm farm, CropViewItem viewItem)
        {
            viewItem.GrowingSeasonIrrigation = _irrigationService.GetGrowingSeasonIrrigation(farm, viewItem);
            viewItem.AmountOfIrrigation = _irrigationService.GetDefaultIrrigationForYear(farm, viewItem.Year);
        }

        /// <summary>
        /// Sets default moisture percentage to the cropViewItem component.
        /// </summary>
        /// <param name="farm"></param>
        public void InitializeMoistureContent(Farm farm)
        {
            if (farm != null)
            {
                foreach (var viewItem in farm.GetAllCropViewItems())
                {
                    var residueData = this.GetResidueData(farm, viewItem);

                    this.InitializeMoistureContent(residueData, viewItem);
                }
            }
        }

        /// <summary>
        /// Sets default moisture percentage to the cropViewItem component.
        /// </summary>
        /// <param name="residueData"> Contains the <see cref="Table_7_Relative_Biomass_Information_Data"/> data to help initialize the <see cref="CropViewItem"/></param>
        /// <param name="cropViewItem"> Contains the <see cref="CropViewItem"/> value to be changed</param>
        public void InitializeMoistureContent(
            Table_7_Relative_Biomass_Information_Data residueData, CropViewItem cropViewItem)
        {
            if (cropViewItem.HarvestMethod == HarvestMethods.GreenManure ||
                cropViewItem.HarvestMethod == HarvestMethods.Silage ||
                cropViewItem.HarvestMethod == HarvestMethods.Swathing ||
                cropViewItem.CropType.IsSilageCrop())
            {
                cropViewItem.MoistureContentOfCropPercentage = 65;
            }
            else
            {
                if (residueData != null && residueData.MoistureContentOfProduct != 0)
                {
                    cropViewItem.MoistureContentOfCropPercentage = residueData.MoistureContentOfProduct;
                }
                else
                {
                    cropViewItem.MoistureContentOfCropPercentage = 12;
                }
            }
        }

        public void InitializeMoistureContent(CropViewItem viewItem, Farm farm)
        {
            var residueData = this.GetResidueData(farm, viewItem);

            this.InitializeMoistureContent(residueData, viewItem);
        }

        #endregion
    }
}