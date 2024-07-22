using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;
using H.Core.Providers.Carbon;

namespace H.Core.Services.Initialization
{
    public interface IInitializationService
    {
        void CheckInitialization(Farm farm);
        void ReInitializeFarms(IEnumerable<Farm> farms);

        void InitializeManureCompositionData(Farm farm);
        void InitializeManureCompositionData(ManagementPeriod managementPeriod, DefaultManureCompositionData manureCompositionData);
        void InitializeMilkProduction(Farm farm);
        void InitializeFuelEnergy(Farm farm);
        void InitializeFuelEnergy(Farm farm, CropViewItem viewItem);
        void InitializeHerbicideEnergy(Farm farm);
        void InitializeHerbicideEnergy(Farm farm, CropViewItem viewItem);
        void InitializeManureMineralizationFractions(Farm farm);
        void InitializeManureMineralizationFractions(ManagementPeriod managementPeriod, FractionOfOrganicNitrogenMineralizedData  mineralizationFractions);
        void InitializeDefaultEmissionFactors(Farm farm, ManagementPeriod managementPeriod);
        void ReinitializeBeddingMaterial(Farm farm);
        void InitializeBeddingMaterial(ManagementPeriod managementPeriod, Table_30_Default_Bedding_Material_Composition_Data data);
        void InitializeDefaultEmissionFactors(Farm farm);
        void InitializeMethaneProducingCapacity(Farm farm);
        void InitializeMethaneProducingCapacity(ManagementPeriod managementPeriod);
        void InitializeCattleFeedingActivity(Farm farm);
        void InitializeParameterAdjustmenstForManure(Farm farm);
        void InitializeManureExcretionRate(ManagementPeriod managementPeriod);
        void InitializeManureExcretionRate(Farm farm);
        void InitializeAnnualEntericMethaneRate(Farm farm);
        void InitializeAnnualEntericMethaneRate(ManagementPeriod managementPeriod);
        void InitializeLivestockCoefficientSheep(Farm farm);
        void InitializeVolatileSolidsExcretion(Farm farm);
        void InitializeVolatileSolidsExcretion(ManagementPeriod managementPeriod, Province province);
        void InitializeIrrigationWaterApplication(Farm farm);
        void InitializeIrrigationWaterApplication(Farm farm, CropViewItem viewItem);
        void InitializeBarnTemperature(Farm farm);
        void InitializeOtherLivestockDefaultCH4EmissionFactor(Farm farm);
        void InitializePercentageReturns(Farm farm, CropViewItem viewItem);
        void InitializeCarbonConcentration(Farm farm);
        void InitializeCarbonConcentration(CropViewItem viewItem, Defaults defaults);
        void InitializeNitrogenFixation(Farm farm);
        void InitializeNitrogenFixation(CropViewItem viewItem);

        void InitializeMoistureContent(Farm farm);
        void InitializeMoistureContent(
            Table_7_Relative_Biomass_Information_Data residueData, CropViewItem cropViewItem);


        void InitializeSilageCropYield(CropViewItem silageCropViewItem, Farm farm);

        void InitializeYield(CropViewItem viewItem, Farm farm);

        void InitializeYieldForAllYears(IEnumerable<CropViewItem> cropViewItems, Farm farm,
            FieldSystemComponent fieldSystemComponent);

        void InitializeYieldForYear(
            Farm farm,
            CropViewItem viewItem,
            FieldSystemComponent fieldSystemComponent);

        void InitializeMoistureContent(CropViewItem viewItem, Farm farm);
        void InitializeBiomassCoefficients(CropViewItem viewItem, Farm farm);
        void InitializeNitrogenContent(CropViewItem viewItem, Farm farm);

        /// <summary>
        /// Sets the tillage type for a view item based on the province.
        /// </summary>
        void InitializeTillageType(
            CropViewItem viewItem,
            Farm farm);

        void InitializeFallow(CropViewItem viewItem, Farm farm);
    }
}
    
