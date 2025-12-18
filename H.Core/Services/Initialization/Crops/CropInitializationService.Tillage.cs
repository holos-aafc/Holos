using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

        public void InitializeTillageType(Farm farm)
        {
            foreach (var viewItem in farm.GetAllCropViewItems()) InitializeTillageType(viewItem, farm);
        }

        /// <summary>
        ///     Sets the tillage type for a view item based on the province.
        /// </summary>
        public void InitializeTillageType(
            CropViewItem viewItem,
            Farm farm)
        {
            if (viewItem.CropType.IsAnnual()) viewItem.TillageType = TillageType.Reduced;

            if (viewItem.CropType.IsPerennial())
            {
                viewItem.TillageType = TillageType.NoTill;
                viewItem.PastTillageType = TillageType.NoTill;
            }
        }

        #endregion
    }
}