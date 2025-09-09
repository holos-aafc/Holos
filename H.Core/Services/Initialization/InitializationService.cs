﻿using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;
using H.Core.Providers.Carbon;
using H.Core.Providers.Soil;
using H.Core.Services.Initialization.Animals;
using H.Core.Services.Initialization.Crops;

namespace H.Core.Services.Initialization
{
    public class InitializationService : IInitializationService
    {
        #region Constructors

        public InitializationService()
        {
            _cropInitializationService = new CropInitializationService();
            _animalInitializationService = new AnimalInitializationService();
        }

        #endregion

        #region Fields

        private readonly ICropInitializationService _cropInitializationService;
        private readonly IAnimalInitializationService _animalInitializationService;
        private IInitializationService _initializationServiceImplementation;

        #endregion

        #region Public Methods

        public void ReInitializeFarms(IEnumerable<Farm> farms)
        {
            foreach (var farm in farms)
            {
                // Defaults (carbon concentration)
                InitializeCarbonConcentration(farm);

                // Nitrogen Fixation
                _cropInitializationService.InitializeNitrogenFixation(farm);

                // Table 4
                InitializeIrrigationWaterApplication(farm);

                // Table 6
                ReinitializeManureCompositionData(farm);

                // Table 17
                InitializeFeedingActivityCoefficient(farm);

                // Table 21
                InitializeMilkProduction(farm);

                // Table 22
                InitializeLivestockCoefficientSheep(farm);

                // Table 27
                InitializeAnnualEntericMethaneEmissionRate(farm);

                // Table 29
                InitializeManureExcretionRate(farm);

                // Table 31
                InitializeVolatileSolidsExcretion(farm);

                // Table 35
                InitializeMethaneProducingCapacityOfManure(farm);

                // Table 36
                InitializeDefaultEmissionFactors(farm);

                //Table 38
                InitializeDailyManureMethaneEmissionRate(farm);

                // Table 44
                InitializeManureMineralizationFractions(farm);

                // Table 50
                InitializeFuelEnergy(farm);

                // Table 51
                InitializeHerbicideEnergy(farm);

                // Table 63
                InitializeBarnTemperature(farm);
            }
        }

        public void InitializeCarbonConcentration(Farm farm)
        {
            _cropInitializationService.InitializeCarbonConcentration(farm);
        }

        public void InitializeCarbonConcentration(CropViewItem viewItem, Defaults defaults)
        {
            _cropInitializationService.InitializeCarbonConcentration(viewItem, defaults);
        }

        public void InitializeBiomassCoefficients(Farm farm)
        {
            _cropInitializationService.InitializeBiomassCoefficients(farm);
        }

        public void InitializeBiomassCoefficients(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeBiomassCoefficients(viewItem, farm);
        }

        public void InitializeLumCMaxValues(CropViewItem cropViewItem, Farm farm)
        {
            _cropInitializationService.InitializeLumCMaxValues(cropViewItem, farm);
        }

        public void InitializePercentageReturns(Farm farm)
        {
            _cropInitializationService.InitializePercentageReturns(farm);
        }

        public void InitializePercentageReturns(Farm farm, CropViewItem viewItem)
        {
            _cropInitializationService.InitializePercentageReturns(farm, viewItem);
        }

        public void InitializeYield(Farm farm)
        {
            _cropInitializationService.InitializeYield(farm);
        }

        public void InitializeYield(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeYield(viewItem, farm);
        }

        public void InitializeSilageCropYield(CropViewItem silageCropViewItem, Farm farm)
        {
            _cropInitializationService.InitializeSilageCropYield(silageCropViewItem, farm);
        }

        public void InitializeYieldForAllYears(IEnumerable<CropViewItem> cropViewItems, Farm farm,
            FieldSystemComponent fieldSystemComponent)
        {
            _cropInitializationService.InitializeYieldForAllYears(cropViewItems, farm, fieldSystemComponent);
        }

        public void InitializeYieldForYear(Farm farm, CropViewItem viewItem, FieldSystemComponent fieldSystemComponent)
        {
            _cropInitializationService.InitializeYieldForYear(farm, viewItem, fieldSystemComponent);
        }

        public void InitializeEconomicDefaults(Farm farm)
        {
            _cropInitializationService.InitializeEconomicDefaults(farm);
        }

        public void InitializeEconomicDefaults(CropViewItem cropViewItem, Farm farm)
        {
            _cropInitializationService.InitializeEconomicDefaults(cropViewItem, farm);
        }

        public double CalculateAmountOfProductRequired(Farm farm, CropViewItem viewItem,
            FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            return _cropInitializationService.CalculateAmountOfProductRequired(farm, viewItem,
                fertilizerApplicationViewItem);
        }

        public void InitializePhosphorusFertilizerRate(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializePhosphorusFertilizerRate(viewItem, farm);
        }

        public void InitializeFertilizerBlendData(FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            _cropInitializationService.InitializeFertilizerBlendData(fertilizerApplicationViewItem);
        }

        public double CalculateRequiredNitrogenFertilizer(Farm farm, CropViewItem viewItem,
            FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            return _cropInitializationService.CalculateRequiredNitrogenFertilizer(farm, viewItem,
                fertilizerApplicationViewItem);
        }

        public void InitializeNitrogenContent(Farm farm)
        {
            _cropInitializationService.InitializeNitrogenContent(farm);
        }

        public void InitializeNitrogenContent(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeNitrogenContent(viewItem, farm);
        }

        public void InitializeNitrogenFixation(CropViewItem viewItem)
        {
            _cropInitializationService.InitializeNitrogenFixation(viewItem);
        }

        public void InitializeNitrogenFixation(Farm farm)
        {
            _cropInitializationService.InitializeNitrogenFixation(farm);
        }

        public void InitializeHerbicideEnergy(Farm farm)
        {
            _cropInitializationService.InitializeHerbicideEnergy(farm);
        }

        public void InitializeHerbicideEnergy(Farm farm, CropViewItem viewItem)
        {
            _cropInitializationService.InitializeHerbicideEnergy(farm, viewItem);
        }

        public void InitializeIrrigationWaterApplication(Farm farm)
        {
            _cropInitializationService.InitializeIrrigationWaterApplication(farm);
        }

        public void InitializeIrrigationWaterApplication(Farm farm, CropViewItem viewItem)
        {
            _cropInitializationService.InitializeIrrigationWaterApplication(farm, viewItem);
        }

        public void InitializeMoistureContent(Farm farm)
        {
            _cropInitializationService.InitializeMoistureContent(farm);
        }

        public void InitializeMoistureContent(Table_7_Relative_Biomass_Information_Data residueData,
            CropViewItem cropViewItem)
        {
            _cropInitializationService.InitializeMoistureContent(residueData, cropViewItem);
        }

        public void InitializeMoistureContent(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeMoistureContent(viewItem, farm);
        }

        public void InitializeSoilProperties(Farm farm)
        {
            _cropInitializationService.InitializeSoilProperties(farm);
        }

        public void InitializeSoilProperties(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeSoilProperties(viewItem, farm);
        }

        public void InitializeFuelEnergy(Farm farm)
        {
            _cropInitializationService.InitializeFuelEnergy(farm);
        }

        public void InitializeFuelEnergy(Farm farm, CropViewItem viewItem)
        {
            _cropInitializationService.InitializeFuelEnergy(farm, viewItem);
        }

        public void InitializeHarvestMethod(Farm farm)
        {
            _cropInitializationService.InitializeHarvestMethod(farm);
        }

        public void InitializeHarvestMethod(CropViewItem viewItem)
        {
            _cropInitializationService.InitializeHarvestMethod(viewItem);
        }

        public void InitializeFallow(Farm farm)
        {
            _cropInitializationService.InitializeFallow(farm);
        }

        public void InitializeFallow(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeFallow(viewItem, farm);
        }

        public void InitializeTillageType(Farm farm)
        {
            _cropInitializationService.InitializeTillageType(farm);
        }

        public void InitializeTillageType(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeTillageType(viewItem, farm);
        }

        public void InitializeLigninContent(CropViewItem cropViewItem, Farm farm)
        {
            _cropInitializationService.InitializeLigninContent(cropViewItem, farm);
        }

        public void InitializeUserDefaults(CropViewItem viewItem, GlobalSettings globalSettings)
        {
            _cropInitializationService.InitializeUserDefaults(viewItem, globalSettings);
        }

        public void InitializePerennialDefaults(Farm farm)
        {
            _cropInitializationService.InitializePerennialDefaults(farm);
        }

        public void InitializePerennialDefaults(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializePerennialDefaults(viewItem, farm);
        }

        public void InitializeCrop(CropViewItem viewItem, Farm farm, GlobalSettings globalSettings)
        {
            _cropInitializationService.InitializeCrop(viewItem, farm, globalSettings);
        }

        public void InitializeCrops(Farm farm, GlobalSettings globalSettings)
        {
            _cropInitializationService.InitializeCrops(farm, globalSettings);
        }

        public void InitializeAvailableSoilTypes(Farm farm, FieldSystemComponent fieldSystemComponent)
        {
            _cropInitializationService.InitializeAvailableSoilTypes(farm, fieldSystemComponent);
        }

        public void InitializeDefaultSoilForField(Farm farm, FieldSystemComponent fieldSystemComponent)
        {
            _cropInitializationService.InitializeDefaultSoilForField(farm, fieldSystemComponent);
        }

        public void InitializeManureApplicationMethod(CropViewItem viewItem,
            ManureApplicationViewItem manureApplicationViewItem,
            List<ManureApplicationTypes> validManureApplicationTypes)
        {
            _cropInitializationService.InitializeManureApplicationMethod(viewItem, manureApplicationViewItem,
                validManureApplicationTypes);
        }

        public void InitializeFertilizerApplicationMethod(
            CropViewItem viewItem,
            FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            _cropInitializationService.InitializeFertilizerApplicationMethod(viewItem, fertilizerApplicationViewItem);
        }

        public void InitializeAmountOfBlendedProduct(Farm farm, CropViewItem viewItem,
            FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            _cropInitializationService.InitializeAmountOfBlendedProduct(farm, viewItem, fertilizerApplicationViewItem);
        }

        public void InitializeHarvestLoss(Farm farm, HarvestViewItem harvestViewItem)
        {
            _cropInitializationService.InitializeHarvestLoss(farm, harvestViewItem);
        }

        public void InitializeLigninContent(Farm farm)
        {
            _cropInitializationService.InitializeLigninContent(farm);
        }

        public void InitializeCoverCrops(IEnumerable<CropViewItem> viewItems)
        {
            _cropInitializationService.InitializeCoverCrops(viewItems);
        }

        public void AssignCoverCropViewItemsDescription(IEnumerable<CropViewItem> viewItems)
        {
            _cropInitializationService.AssignCoverCropViewItemsDescription(viewItems);
        }

        public void InitializeManureApplicationMethod(Farm farm)
        {
            _cropInitializationService.InitializeManureApplicationMethod(farm);
        }

        public void InitializeFertilizerBlendData(Farm farm)
        {
            _cropInitializationService.InitializeFertilizerBlendData(farm);
        }

        public void InitializePhosphorusFertilizerRate(Farm farm)
        {
            _cropInitializationService.InitializePhosphorusFertilizerRate(farm);
        }

        public void InitializeHarvestLoss(Farm farm)
        {
            _cropInitializationService.InitializeHarvestLoss(farm);
        }

        public void InitializeFertilizerApplicationMethod(Farm farm)
        {
            _cropInitializationService.InitializeFertilizerApplicationMethod(farm);
        }

        public void InitializeDefaultSoilForField(Farm farm)
        {
            _cropInitializationService.InitializeDefaultSoilForField(farm);
        }

        public void InitializeGrazingViewItem(GrazingViewItem grazingViewItem, ManagementPeriod managementPeriod,
            AnimalComponentBase animalComponent, AnimalGroup animalGroup, CropViewItem cropViewItem)
        {
            _cropInitializationService.InitializeGrazingViewItem(grazingViewItem, managementPeriod, animalComponent,
                animalGroup, cropViewItem);
        }

        public void InitializeNitrogenContent(List<CropViewItem> viewItem, Farm farm)
        {
            _cropInitializationService.InitializeNitrogenContent(viewItem, farm);
        }

        public void InitializeGrazingViewItems(Farm farm, CropViewItem viewItem,
            FieldSystemComponent fieldSystemComponent)
        {
            _cropInitializationService.InitializeGrazingViewItems(farm, viewItem, fieldSystemComponent);
        }

        public string InitializeDescription(ManagementPeriod managementPeriod, AnimalGroup animalGroup)
        {
            return _cropInitializationService.InitializeDescription(managementPeriod, animalGroup);
        }

        public void InitializeBiomassCoefficientProduct(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeBiomassCoefficientProduct(viewItem, farm);
        }

        public void InitializeBiomassCoefficientStraw(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeBiomassCoefficientStraw(viewItem, farm);
        }

        public void InitializeBiomassCoefficientRoots(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeBiomassCoefficientRoots(viewItem, farm);
        }

        public void InitializeBiomassCoefficientExtraroots(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeBiomassCoefficientExtraroots(viewItem, farm);
        }

        public void InitializeNitrogenContentInExtraroots(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeNitrogenContentInExtraroots(viewItem, farm);
        }

        public void InitializeNitrogenContentInRoots(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeNitrogenContentInRoots(viewItem, farm);
        }

        public void InitializeNitrogenContentInStraw(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeNitrogenContentInStraw(viewItem, farm);
        }

        public void InitializeNitrogenContentInProduct(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeNitrogenContentInProduct(viewItem, farm);
        }

        public void InitializeIPCCNitrogenContent(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeIPCCNitrogenContent(viewItem, farm);
        }


        public void InitializeDailyManureMethaneEmissionRate(Farm farm)
        {
            _animalInitializationService.InitializeDailyManureMethaneEmissionRate(farm);
        }

        public void InitializeVolatileSolidsExcretion(Farm farm)
        {
            _animalInitializationService.InitializeVolatileSolidsExcretion(farm);
        }

        public void InitializeVolatileSolidsExcretion(ManagementPeriod managementPeriod, Province province)
        {
            _animalInitializationService.InitializeVolatileSolidsExcretion(managementPeriod, province);
        }

        public void InitializeAnnualEntericMethaneEmissionRate(Farm farm)
        {
            _animalInitializationService.InitializeAnnualEntericMethaneEmissionRate(farm);
        }

        public void InitializeAnnualEntericMethaneEmissionRate(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeAnnualEntericMethaneEmissionRate(managementPeriod);
        }

        public void InitializeMethaneProducingCapacityOfManure(Farm farm)
        {
            _animalInitializationService.InitializeMethaneProducingCapacityOfManure(farm);
        }

        public void InitializeMethaneProducingCapacityOfManure(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeMethaneProducingCapacityOfManure(managementPeriod);
        }

        public void ReinitializeBeddingMaterial(Farm farm)
        {
            _animalInitializationService.ReinitializeBeddingMaterial(farm);
        }

        public void InitializeBeddingMaterial(ManagementPeriod managementPeriod,
            Table_30_Default_Bedding_Material_Composition_Data data)
        {
            _animalInitializationService.InitializeBeddingMaterial(managementPeriod, data);
        }

        public void InitializeManureMineralizationFractions(Farm farm)
        {
            _animalInitializationService.InitializeManureMineralizationFractions(farm);
        }

        public void InitializeManureMineralizationFractions(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeManureMineralizationFractions(managementPeriod);
        }

        public void InitializeManureExcretionRate(Farm farm)
        {
            _animalInitializationService.InitializeManureExcretionRate(farm);
        }

        public void InitializeManureExcretionRate(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeManureExcretionRate(managementPeriod);
        }

        public void InitializeBarnTemperature(Farm farm)
        {
            _animalInitializationService.InitializeBarnTemperature(farm);
        }

        public void InitializeMilkProduction(Farm farm)
        {
            _animalInitializationService.InitializeMilkProduction(farm);
        }

        public void ReinitializeManureCompositionData(Farm farm)
        {
            _animalInitializationService.ReinitializeManureCompositionData(farm);
        }

        public void InitializeManureCompositionData(ManagementPeriod managementPeriod,
            DefaultManureCompositionData manureCompositionData)
        {
            _animalInitializationService.InitializeManureCompositionData(managementPeriod, manureCompositionData);
        }

        public void InitializeLivestockCoefficientSheep(Farm farm)
        {
            _animalInitializationService.InitializeLivestockCoefficientSheep(farm);
        }

        public void InitializeDefaultEmissionFactors(Farm farm)
        {
            _animalInitializationService.InitializeDefaultEmissionFactors(farm);
        }

        public void InitializeDefaultEmissionFactors(Farm farm, ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeDefaultEmissionFactors(farm, managementPeriod);
        }

        public void InitializeFeedingActivityCoefficient(Farm farm)
        {
            _animalInitializationService.InitializeFeedingActivityCoefficient(farm);
        }

        public void InitializeFeedingActivityCoefficient(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeFeedingActivityCoefficient(managementPeriod);
        }

        public void InitializeMilkProduction(ManagementPeriod managementPeriod, SoilData soilData)
        {
            _animalInitializationService.InitializeMilkProduction(managementPeriod, soilData);
        }

        public void InitializeLeachingFraction(Farm farm, ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeLeachingFraction(farm, managementPeriod);
        }

        public void InitializeLeachingFraction(Farm farm)
        {
            _animalInitializationService.InitializeLeachingFraction(farm);
        }

        public void InitializeDailyTanExcretion(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeDailyTanExcretion(managementPeriod);
        }

        public void InitializeAmmoniaEmissionFactorForManureStorage(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeAmmoniaEmissionFactorForManureStorage(managementPeriod);
        }

        public void InitializeAmmoniaEmissionFactorForHousing(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeAmmoniaEmissionFactorForHousing(managementPeriod);
        }

        public void InitializeAmmoniaEmissionFactorForLandApplication(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeAmmoniaEmissionFactorForLandApplication(managementPeriod);
        }

        public void InitializeLivestockCoefficientSheep(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeLivestockCoefficientSheep(managementPeriod);
        }

        public void InitializeBaselineCoefficient(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeBaselineCoefficient(managementPeriod);
        }

        public void InitializeGainCoefficient(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeGainCoefficient(managementPeriod);
        }

        public void InitializeBeddingMaterial(ManagementPeriod managementPeriod, Farm farm)
        {
            _animalInitializationService.InitializeBeddingMaterial(managementPeriod, farm);
        }

        public void InitializeBeddingMaterialRate(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeBeddingMaterialRate(managementPeriod);
        }

        public void InitializeNitrogenExcretionRate(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeNitrogenExcretionRate(managementPeriod);
        }

        public void InitializeVolatileSolids(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeVolatileSolids(managementPeriod);
        }

        public void InitializeDailyTanExcretion(Farm farm)
        {
            _animalInitializationService.InitializeDailyTanExcretion(farm);
        }

        public void InitializeAmmoniaEmissionFactorForManureStorage(Farm farm)
        {
            _animalInitializationService.InitializeAmmoniaEmissionFactorForManureStorage(farm);
        }

        public void InitializeAmmoniaEmissionFactorForHousing(Farm farm)
        {
            _animalInitializationService.InitializeAmmoniaEmissionFactorForHousing(farm);
        }

        public void InitializeAmmoniaEmissionFactorForLandApplication(Farm farm)
        {
            _animalInitializationService.InitializeAmmoniaEmissionFactorForLandApplication(farm);
        }

        public void InitializeBeddingMaterial(Farm farm)
        {
            _animalInitializationService.InitializeBeddingMaterial(farm);
        }

        public void InitializeBeddingMaterialRate(Farm farm)
        {
            _animalInitializationService.InitializeBeddingMaterialRate(farm);
        }

        public void InitializeBaselineCoefficient(Farm farm)
        {
            _animalInitializationService.InitializeBaselineCoefficient(farm);
        }

        public void InitializeGainCoefficient(Farm farm)
        {
            _animalInitializationService.InitializeGainCoefficient(farm);
        }

        public void InitializeNitrogenExcretionRate(Farm farm)
        {
            _animalInitializationService.InitializeNitrogenExcretionRate(farm);
        }

        public void InitializeManureCompositionData(Farm farm)
        {
            _animalInitializationService.InitializeManureCompositionData(farm);
        }

        public void InitializeDailyManureMethaneEmissionRate(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeDailyManureMethaneEmissionRate(managementPeriod);
        }

        public void InitializeVolatileSolids(Farm farm)
        {
            _animalInitializationService.InitializeVolatileSolids(farm);
        }

        public List<DietAdditiveType> GetValidDietAdditiveTypes()
        {
            return _animalInitializationService.GetValidDietAdditiveTypes();
        }

        public void InitializeTotals(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeTotals(managementPeriod);
        }

        public void InitializeStartAndEndWeightsForCattle(Farm farm)
        {
            _animalInitializationService.InitializeStartAndEndWeightsForCattle(farm);
        }

        public void InitializeStartAndEndWeightsForCattle(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeStartAndEndWeightsForCattle(managementPeriod);
        }

        public void InitializeFarm(Farm farm)
        {
            _animalInitializationService.InitializeFarm(farm);
        }

        #endregion
    }
}