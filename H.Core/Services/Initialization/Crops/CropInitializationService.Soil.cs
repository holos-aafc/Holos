using H.Core.Models.LandManagement.Fields;
using H.Core.Models;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods
        
        public void InitializeSoilProperties(CropViewItem viewItem, Farm farm)
        {
            var soilData = farm.GetPreferredSoilData(viewItem);

            viewItem.Sand = soilData.ProportionOfSandInSoil;
        }

        public void InitializeLigninContent(CropViewItem cropViewItem, Farm farm)
        {
            var province = farm.DefaultSoilData.Province;
            var soilFunctionCategory = farm.GetPreferredSoilData(cropViewItem).SoilFunctionalCategory;
            var residueData = _relativeBiomassInformationProvider.GetResidueData(cropViewItem.IrrigationType, cropViewItem.AmountOfIrrigation, cropViewItem.CropType, soilFunctionCategory, province);

            if (residueData != null)
            {
                cropViewItem.LigninContent = residueData.LigninContent;
            }
            else
            {
                cropViewItem.LigninContent = 0.0;
            }
        }

        #endregion
    }
}