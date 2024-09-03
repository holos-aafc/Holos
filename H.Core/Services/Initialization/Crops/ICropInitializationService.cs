using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Carbon;

namespace H.Core.Services.Initialization.Crops
{
    public interface ICropInitializationService
    {

        /**
         * Need to add secondary functions with only farm argument for
         * -InitializeEconomicDefaults
         * -InitializeUserDefaults
         * -InitializeLumCMaxValues
         * -InitializePhosphorusFertilizerRate
         * -InitializeYieldForAllYears
         * -InitializeBlendData
         * -InitializeAvailableSoilTypes
         * -InitializeDefaultSoilDataForField
         * -InitializeManureApplicationMethod
         * -InitializeFertilizerApplicationMethod
         * -InitializeUtilization
         */


        /// <summary>
        /// Initialize the carbon concentration of each <see cref="CropViewItem"/> within a farm
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="CropViewItem"/>s that will have reset their carbon concentrations reset</param>
        void InitializeCarbonConcentration(Farm farm);

        /// <summary>
        /// Initialize the carbon concentration of the <see cref="CropViewItem"/> with the values in the <see cref="Defaults"/> parameter
        /// </summary>
        /// <param name="viewItem">The <see cref="CropViewItem"/> to have it's carbon concentration reset with default value</param>
        /// <param name="defaults">The <see cref="Defaults"/> containing the default carbon concentration</param>
        void InitializeCarbonConcentration(CropViewItem viewItem, Defaults defaults);

        void InitializeBiomassCoefficients(Farm farm);
        void InitializeBiomassCoefficients(CropViewItem viewItem, Farm farm);
        void InitializeLumCMaxValues(CropViewItem cropViewItem, Farm farm);

        /// <summary>
        /// Initialize default percentage return to soil values for a <see cref="H.Core.Models.LandManagement.Fields.CropViewItem"/>.
        /// </summary>
        /// <param name="farm">The farm containing the <see cref="Defaults"/> object used to initialize each <see cref="CropViewItem"/></param>
        /// <param name="viewItem">The <see cref="CropViewItem"/> that will be initialized</param>
        void InitializePercentageReturns(Farm farm, CropViewItem viewItem);
        void InitializePercentageReturns(Farm farm);
        void InitializeYield(Farm farm);
        void InitializeYield(CropViewItem viewItem, Farm farm);

        /// <summary>
        /// Calculates the yield of silage crop using information from the grain crop equivalent to that silage crop e.g. if silage crop is Barley Silage, its grain equivalent will be
        /// Barley.
        /// </summary>
        /// <param name="silageCropViewItem">The <see cref="H.Core.Models.LandManagement.Fields.CropViewItem"/> representing the silage crop. </param>
        /// <param name="farm">The current farm of the user.</param>
        void InitializeSilageCropYield(CropViewItem silageCropViewItem, Farm farm);

        void InitializeYieldForAllYears(IEnumerable<CropViewItem> cropViewItems, Farm farm,
            FieldSystemComponent fieldSystemComponent);

        /// <summary>
        /// Assigns a yield to one view item for a field
        /// </summary>
        void InitializeYieldForYear(
            Farm farm,
            CropViewItem viewItem,
            FieldSystemComponent fieldSystemComponent);

        void InitializeEconomicDefaults(Farm farm);

        void InitializeEconomicDefaults(
            CropViewItem cropViewItem,
            Farm farm);

        /// <summary>
        /// Equation 2.5.5-7
        ///
        /// Calculates the amount of the fertilizer blend needed to support the yield that was input.This considers the amount of nitrogen uptake by the plant and then
        /// converts that value into an amount of fertilizer blend/product
        /// </summary>
        /// <returns>The amount of product required (kg product ha^-1)</returns>
        double CalculateAmountOfProductRequired(
            Farm farm,
            CropViewItem viewItem,
            FertilizerApplicationViewItem fertilizerApplicationViewItem);

        /// <summary>
        /// When not using a blended P fertilizer approach, use this to assign a P rate directly to the crop
        /// </summary>
        void InitializePhosphorusFertilizerRate(CropViewItem viewItem, Farm farm);

        void InitializeBlendData(FertilizerApplicationViewItem fertilizerApplicationViewItem);

        /// <summary>
        /// Determines the amount of N fertilizer required for the specified crop type and yield
        /// </summary>
        double CalculateRequiredNitrogenFertilizer(
            Farm farm,
            CropViewItem viewItem,
            FertilizerApplicationViewItem fertilizerApplicationViewItem);
        void InitializeNitrogenContent(Farm farm);
        void InitializeNitrogenContent(CropViewItem viewItem, Farm farm);

        /// <summary>
        /// Initialize the nitrogen fixation within a <see cref="CropViewItem"/>
        /// </summary>
        /// <param name="viewItem">The <see cref="CropViewItem"/> having its nitrogen fixation reinitialized</param>
        void InitializeNitrogenFixation(CropViewItem viewItem);

        /// <summary>
        /// Initialize the nitrogen fixation for each <see cref="CropViewItem"/> within a <see cref="Farm"/>
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="CropViewItem"/>'s that will have their nitrogen fixation reinitialized</param>
        void InitializeNitrogenFixation(Farm farm);

        /// <summary>
        /// Reinitialize each <see cref="CropViewItem"/> within <see cref="Farm"/> with new default values
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> that will be reinitialized to new default values</param>
        void InitializeHerbicideEnergy(Farm farm);

        /// <summary>
        /// Reinitialize the <see cref="CropViewItem"/> from the selected <see cref="Farm"/> with new default values
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant data to pass into <see cref="Table_51_Herbicide_Energy_Estimates_Provider"/></param>
        /// <param name="viewItem">The <see cref="CropViewItem"/> that will have its values reset with new default values</param>
        void InitializeHerbicideEnergy(Farm farm, CropViewItem viewItem);

        /// <summary>
        /// Reinitialize each <see cref="CropViewItem"/>'s irrigation properties with a <see cref="Farm"/>
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing <see cref="CropViewItem"/>'s to be reinitialized</param>
        void InitializeIrrigationWaterApplication(Farm farm);

        /// <summary>
        /// Reinitialize the <see cref="CropViewItem"/> irrigation properties
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> that contains the climate data and province data required for the lookup table</param>
        /// <param name="viewItem">The <see cref="CropViewItem"/> to have its irrigation properties reinitialized</param>
        void InitializeIrrigationWaterApplication(Farm farm, CropViewItem viewItem);

        /// <summary>
        /// Sets default moisture percentage to the cropViewItem component.
        /// </summary>
        /// <param name="farm"></param>
        void InitializeMoistureContent(Farm farm);

        /// <summary>
        /// Sets default moisture percentage to the cropViewItem component.
        /// </summary>
        /// <param name="residueData"> Contains the <see cref="Table_7_Relative_Biomass_Information_Data"/> data to help initialize the <see cref="CropViewItem"/></param>
        /// <param name="cropViewItem"> Contains the <see cref="CropViewItem"/> value to be changed</param>
        void InitializeMoistureContent(
            Table_7_Relative_Biomass_Information_Data residueData, CropViewItem cropViewItem);

        void InitializeMoistureContent(CropViewItem viewItem, Farm farm);
        void InitializeSoilProperties(Farm farm);
        void InitializeSoilProperties(CropViewItem viewItem, Farm farm);

        /// <summary>
        /// Reinitialize each <see cref="CropViewItem"/> within <see cref="Farm"/> with new default values
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> that will be reinitialized to new default values</param>
        void InitializeFuelEnergy(Farm farm);

        /// <summary>
        /// Reinitialize the <see cref="CropViewItem"/> from the selected <see cref="Farm"/> with new default values
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant data to pass into <see cref="Table_50_Fuel_Energy_Estimates_Provider"/></param>
        /// <param name="viewItem">The <see cref="CropViewItem"/> that will have its values reset with new default values</param>
        void InitializeFuelEnergy(Farm farm, CropViewItem viewItem);
        void InitializeHarvestMethod(Farm farm);
        void InitializeHarvestMethod(CropViewItem viewItem);
        void InitializeFallow(Farm farm);
        void InitializeFallow(CropViewItem viewItem, Farm farm);

        /// <summary>
        /// Sets the tillage type for a view item based on the province.
        /// </summary>
        void InitializeTillageType(
            CropViewItem viewItem,
            Farm farm);

        void InitializeTillageType(Farm farm);

        void InitializeLigninContent(CropViewItem cropViewItem, Farm farm);
        void InitializeUserDefaults(CropViewItem viewItem, GlobalSettings globalSettings);
        void InitializePerennialDefaults(CropViewItem viewItem, Farm farm);
        void InitializePerennialDefaults(Farm farm);

        /// <summary>
        /// Applies the default properties on a crop view item based on Holos defaults and user defaults (if available). Any property that cannot be set in the constructor
        /// of the <see cref="H.Core.Models.LandManagement.Fields.CropViewItem"/> should be set here.
        /// </summary>
        void InitializeCrop(CropViewItem viewItem, Farm farm, GlobalSettings globalSettings);

        void InitializeCrops(Farm farm, GlobalSettings globalSettings);
        void InitializeAvailableSoilTypes(Farm farm, FieldSystemComponent fieldSystemComponent);
        void InitializeDefaultSoilForField(Farm farm, FieldSystemComponent fieldSystemComponent);
        void InitializeManureApplicationMethod(CropViewItem viewItem, ManureApplicationViewItem manureApplicationViewItem, List<ManureApplicationTypes> validManureApplicationTypes);
        void InitializeFertilizerApplicationMethod(CropViewItem viewItem, FertilizerApplicationViewItem fertilizerApplicationViewItem);
        void InitializeAmountOfBlendedProduct(Farm farm, CropViewItem viewItem, FertilizerApplicationViewItem fertilizerApplicationViewItem);
        void InitializeUtilization(Farm farm, HarvestViewItem harvestViewItem);
        void InitializeLigninContent(Farm farm);

        void InitializeCoverCrops(IEnumerable<CropViewItem> viewItems);

        void AssignCoverCropViewItemsDescription(IEnumerable<CropViewItem> viewItems);
    }
}