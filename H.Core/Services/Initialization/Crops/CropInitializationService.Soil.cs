using System.Linq;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using H.Core.Providers.Soil;

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

        public void InitializeSoil(Farm farm)
        {
            if (farm != null)
            {
                foreach (var fieldSystemComponent in farm.FieldSystemComponents)
                {
                    this.InitializeDefaultSoilForField(fieldSystemComponent);
                }
            }
        }

        public void InitializeDefaultSoilForField(FieldSystemComponent fieldSystemComponent)
        {
            if (fieldSystemComponent != null)
            {
                // Old farms will have an empty collection of available soil types for the soil data collection held by the field
                if (fieldSystemComponent.SoilDataAvailableForField == null || fieldSystemComponent.SoilDataAvailableForField.Any() == false)
                {
                    
                }
            }
        }

        public void InitializeAvailableSoilTypes(Farm farm, FieldSystemComponent fieldSystemComponent)
        {
            if (farm != null)
            {
                foreach (var soilData in farm.GeographicData.SoilDataForAllComponentsWithinPolygon)
                {
                    // Add this soil type if it does not already exist as an option for this field
                    var shouldAddToField = fieldSystemComponent.SoilDataAvailableForField.FirstOrDefault(x => x.SoilGreatGroup == soilData.SoilGreatGroup) == null;
                    if (shouldAddToField)
                    {
                        // We don't model organic soil at this time
                        if (soilData.IsOrganic() == false)
                        {
                            var copiedSoil = new SoilData();
                            _soilDataMapper.Map(soilData, copiedSoil);

                            fieldSystemComponent.SoilDataAvailableForField.Add(copiedSoil);
                        }
                    }
                }
            }
        }

        #endregion
    }
}