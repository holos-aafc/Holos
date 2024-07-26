using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

        /// <summary>
        /// Sets the tillage type for a view item based on the province.
        /// </summary>
        public void InitializeTillageType(
            CropViewItem viewItem,
            Farm farm)
        {
            var province = farm.DefaultSoilData.Province;
            var soilFunctionCategory = farm.GetPreferredSoilData(viewItem).SoilFunctionalCategory;
            var residueData = _relativeBiomassInformationProvider.GetResidueData(viewItem.IrrigationType, viewItem.AmountOfIrrigation, viewItem.CropType, soilFunctionCategory, province);

            if (residueData != null)
            {
                if (residueData.TillageTypeTable.ContainsKey(province))
                {
                    var tillageTypeForProvince = residueData.TillageTypeTable[province];

                    viewItem.TillageType = tillageTypeForProvince;
                    viewItem.PastTillageType = tillageTypeForProvince;
                }
            }

            if (viewItem.CropType.IsPerennial())
            {
                viewItem.TillageType = TillageType.NoTill;
                viewItem.PastTillageType = TillageType.NoTill;
            }
        }

        #endregion
    }
}