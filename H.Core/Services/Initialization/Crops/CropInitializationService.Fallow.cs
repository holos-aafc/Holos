using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

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