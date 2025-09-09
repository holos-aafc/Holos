using System.Linq;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Soil;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

        public void InitializeSoilProperties(Farm farm)
        {
            foreach (var viewItem in farm.GetAllCropViewItems()) InitializeSoilProperties(viewItem, farm);
        }

        public void InitializeSoilProperties(CropViewItem viewItem, Farm farm)
        {
            var soilData = farm.GetPreferredSoilData(viewItem);

            viewItem.Sand = soilData.ProportionOfSandInSoil;
        }

        public void InitializeLigninContent(Farm farm)
        {
            var viewItems = farm.GetAllCropViewItems();
            foreach (var viewItem in viewItems) InitializeLigninContent(viewItem, farm);
        }

        public void InitializeLigninContent(CropViewItem cropViewItem, Farm farm)
        {
            var residueData = GetResidueData(farm, cropViewItem);
            if (residueData != null)
                cropViewItem.LigninContent = residueData.LigninContent;
            else
                cropViewItem.LigninContent = 0.0;
        }

        public void InitializeDefaultSoilForField(Farm farm)
        {
            foreach (var fieldSystemComponent in farm.FieldSystemComponents)
            {
                fieldSystemComponent.SoilDataAvailableForField.Clear();

                InitializeDefaultSoilForField(farm, fieldSystemComponent);
            }
        }

        public void InitializeDefaultSoilForField(Farm farm, FieldSystemComponent fieldSystemComponent)
        {
            if (fieldSystemComponent != null)
            {
                // Old farms will have an empty collection of available soil types for the soil data collection held by the field
                if (fieldSystemComponent.SoilDataAvailableForField == null ||
                    fieldSystemComponent.SoilDataAvailableForField.Any() == false)
                {
                    InitializeAvailableSoilTypes(farm, fieldSystemComponent);

                    if (fieldSystemComponent.SoilDataAvailableForField != null)
                        fieldSystemComponent.SoilData = fieldSystemComponent.SoilDataAvailableForField.FirstOrDefault();
                }

                if (fieldSystemComponent.SoilData != null && fieldSystemComponent.SoilData.PolygonId == 0)
                {
                    var soilDataAvailableForField = fieldSystemComponent.SoilDataAvailableForField;
                    if (soilDataAvailableForField != null)
                        fieldSystemComponent.SoilData = soilDataAvailableForField.FirstOrDefault();
                }
            }
        }

        public void InitializeAvailableSoilTypes(Farm farm)
        {
        }

        public void InitializeAvailableSoilTypes(Farm farm, FieldSystemComponent fieldSystemComponent)
        {
            if (farm != null && farm.GeographicData != null &&
                farm.GeographicData.SoilDataForAllComponentsWithinPolygon != null)
                foreach (var soilData in farm.GeographicData.SoilDataForAllComponentsWithinPolygon)
                {
                    // Add this soil type if it does not already exist as an option for this field
                    var shouldAddToField =
                        fieldSystemComponent.SoilDataAvailableForField.FirstOrDefault(x =>
                            x.SoilGreatGroup == soilData.SoilGreatGroup) == null;
                    if (shouldAddToField)
                        // We don't model organic soil at this time
                        if (soilData.IsOrganic() == false)
                        {
                            var copiedSoil = new SoilData();
                            _soilDataMapper.Map(soilData, copiedSoil);

                            fieldSystemComponent.SoilDataAvailableForField.Add(copiedSoil);
                        }
                }
        }

        #endregion
    }
}