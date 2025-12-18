using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

        public void InitializeFallow(Farm farm)
        {
            foreach (var viewItem in farm.GetAllCropViewItems()) InitializeFallow(viewItem, farm);
        }

        public void InitializeFallow(CropViewItem viewItem, Farm farm)
        {
            if (farm != null && viewItem != null && viewItem.CropType.IsFallow())
            {
                viewItem.Yield = 0;
                viewItem.TillageType = farm.Defaults.DefaultTillageTypeForFallow;
                viewItem.PastTillageType = TillageType.NoTill;
                viewItem.HarvestMethod = HarvestMethods.None;
                viewItem.PercentageOfProductYieldReturnedToSoil = 0;
                viewItem.PercentageOfStrawReturnedToSoil = 0;
                viewItem.PercentageOfRootsReturnedToSoil = 0;
            }
        }

        #endregion
    }
}