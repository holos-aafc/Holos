using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

        /// <summary>
        /// Initialize default percentage return to soil values for a <see cref="H.Core.Models.LandManagement.Fields.CropViewItem"/>.
        /// </summary>
        /// <param name="farm">The farm containing the <see cref="Defaults"/> object used to initialize each <see cref="CropViewItem"/></param>
        /// <param name="viewItem">The <see cref="CropViewItem"/> that will be initialized</param>
        public void InitializePercentageReturns(Farm farm, CropViewItem viewItem)
        {
            if (farm != null && viewItem != null)
            {
                var defaults = farm.Defaults;

                /*
                 * Initialize the view item by checking the crop type
                 */

                if (viewItem.CropType.IsPerennial())
                {
                    viewItem.PercentageOfProductYieldReturnedToSoil = defaults.PercentageOfProductReturnedToSoilForPerennials;
                    viewItem.PercentageOfStrawReturnedToSoil = 0;
                    viewItem.PercentageOfRootsReturnedToSoil = defaults.PercentageOfRootsReturnedToSoilForPerennials;
                }
                else if (viewItem.CropType.IsAnnual())
                {
                    viewItem.PercentageOfProductYieldReturnedToSoil = defaults.PercentageOfProductReturnedToSoilForAnnuals;
                    viewItem.PercentageOfRootsReturnedToSoil = defaults.PercentageOfRootsReturnedToSoilForAnnuals;
                    viewItem.PercentageOfStrawReturnedToSoil = defaults.PercentageOfStrawReturnedToSoilForAnnuals;
                }

                if (viewItem.CropType.IsRootCrop())
                {
                    viewItem.PercentageOfProductYieldReturnedToSoil = defaults.PercentageOfProductReturnedToSoilForRootCrops;
                    viewItem.PercentageOfStrawReturnedToSoil = defaults.PercentageOfStrawReturnedToSoilForRootCrops;
                }

                if (viewItem.CropType.IsCoverCrop())
                {
                    viewItem.PercentageOfProductYieldReturnedToSoil = 100;
                    viewItem.PercentageOfStrawReturnedToSoil = 100;
                    viewItem.PercentageOfRootsReturnedToSoil = 100;
                }

                /*
                 * Initialize the view item by checking the harvest method (override any setting based on crop type)
                 */

                if (viewItem.CropType.IsSilageCrop() || viewItem.HarvestMethod == HarvestMethods.Silage)
                {
                    viewItem.PercentageOfProductYieldReturnedToSoil = 2;
                    viewItem.PercentageOfStrawReturnedToSoil = 0;
                    viewItem.PercentageOfRootsReturnedToSoil = 100;
                }
                else if (viewItem.HarvestMethod == HarvestMethods.Swathing)
                {
                    viewItem.PercentageOfProductYieldReturnedToSoil = 30;
                    viewItem.PercentageOfStrawReturnedToSoil = 0;
                    viewItem.PercentageOfRootsReturnedToSoil = 100;
                }
                else if (viewItem.HarvestMethod == HarvestMethods.GreenManure)
                {
                    viewItem.PercentageOfProductYieldReturnedToSoil = 100;
                    viewItem.PercentageOfStrawReturnedToSoil = 0;
                    viewItem.PercentageOfRootsReturnedToSoil = 100;
                }
            }
        }

        #endregion
    }
}