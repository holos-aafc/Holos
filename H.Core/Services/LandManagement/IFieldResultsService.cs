using System.Collections.Generic;
using System.Globalization;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models.Results;
using H.Core.Providers.Carbon;

namespace H.Core.Services.LandManagement
{
    public interface IFieldResultsService
    {
        void AssignDefaultYield(CropViewItem viewItem, Farm farm);        
        double CalculateClimateParameter(CropViewItem viewItem, Farm farm);
        double CalculateTillageFactor(CropViewItem viewItem, Farm farm);
        double CalculateManagementFactor(double climateParameter, double tillageFactor);
        double CalculateRequiredNitrogenFertilizer(Farm farm, CropViewItem viewItem,
                                                   FertilizerApplicationViewItem fertilizerApplicationViewItem);
        void CreateDetailViewItems(Farm farm);

        
        List<CropViewItem> CalculateFinalResults(IEnumerable<Farm> farms);

        bool ExportResultsToFile(IEnumerable<CropViewItem> results,
                                            object path,
                                            CultureInfo cultureInfo,
                                            MeasurementSystemType measurementSystemType,
                                            string languageAddOn,
                                            bool exportedFromGui,
                                            Farm farm);

        SoilN2OEmissionsResults CalculateCropN2OEmissions(FieldSystemComponent fieldSystemComponent,
                                                          Farm farm);

        SoilN2OEmissionsResults CalculateManureN2OEmissionsForFarm(FarmEmissionResults farmEmissionResults);
        SoilN2OEmissionsResults CalculateMineralN2OEmissionsForFarm(FarmEmissionResults farmEmissionResults);
        double CalculateHarvest(CropViewItem viewItem);
        void AssignYieldToYear(Farm farm, CropViewItem viewItem);
        void AssignYieldToAllYears(IEnumerable<CropViewItem> cropViewItems, Farm farm);

        void AssignDefaultPercentageReturns(
            CropViewItem viewItem,
            Defaults defaults);

        void AssignDefaultPercentageReturns(List<CropViewItem> viewItems, Defaults defaults);
        FieldSystemDetailsStageState GetStageState(Farm farm);
        void AssignDefaultLumCMaxValues(CropViewItem cropViewItem, Farm farm);
        void AssignDefaultNitrogenFertilizerRate(CropViewItem viewItem, Farm farm,
                                                 FertilizerApplicationViewItem fertilizerApplicationViewItem);
        void AssignDefaultTillageTypeForSelectedProvince(CropViewItem viewItem, Farm farm);
        void AssignDefaultPhosphorusFertilizerRate(CropViewItem viewItem, Farm farm);
        void AssignDefaultNitrogenContentValues(CropViewItem viewItem, Farm farm);
        void AssignDefaultBiomassCoefficients(CropViewItem viewItem, Farm farm);
        Table_10_Relative_Biomass_Data GetResidueData(CropViewItem cropViewItem, Farm farm);
        void AssignSystemDefaults(CropViewItem viewItem, Farm farm, GlobalSettings globalSettings);
        FieldComponentEmissionResults CalculateResultsForFieldComponent(FieldSystemComponent fieldSystemComponent, Farm farm);
        CropViewItem MapDetailsScreenViewItemFromComponentScreenViewItem(CropViewItem viewItem);
        List<EstimatesOfProductionResultsViewItem> CalculateHarvestForField(FieldSystemComponent fieldSystemComponent, Farm farm);

        /// <summary>
        /// Calculates results for a <see cref="FieldSystemComponent"/> and then sets the <see cref="ComponentBase.ResultsCalculated"/> value to true.
        /// </summary>
        List<FieldComponentEmissionResults> CalculateResultsForFieldComponent(Farm farm);


        /// <summary>
        /// Calculates C/CO2 changes based on land use changes for single year fields.
        /// </summary>
        LandUseChangeResults CalculateLandUseChangeResults(FieldSystemComponent fieldSystemComponent, Farm farm);

        void InitializeStageState(Farm farm);
        void AssignDefaultMoistureContent(CropViewItem cropViewItem, Farm farm);
        void AssignDefaultEnergyRequirements(CropViewItem viewItem, Farm farm);
        void AssignPerennialDefaultsIfApplicable(CropViewItem viewItem, Farm farm);
        void AssignFallowDefaultsIfApplicable(CropViewItem viewItem, Farm farm);
        List<CropViewItem> CalculateFinalResults(Farm farm);
        void AssignDefaultBlendData(FertilizerApplicationViewItem fertilizerApplicationViewItem);

        /// <summary>
        /// Equation 2.5.2-6
        /// </summary>
        /// <returns>The amount of product required (kg product ha^-1)</returns>
        double CalculateAmountOfProductRequired(
            Farm farm, 
            CropViewItem viewItem, 
            FertilizerApplicationViewItem fertilizerApplicationViewItem);

        void AssignHarvestMethod(CropViewItem viewItem, Farm farm);
    }
}