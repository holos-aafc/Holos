using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Soil;
using H.Infrastructure;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService
    {
        /// <summary>
        /// Applies the default properties on a crop view item based on Holos defaults and user defaults (if available). Any property that cannot be set in the constructor
        /// of the <see cref="H.Core.Models.LandManagement.Fields.CropViewItem"/> should be set here.
        /// </summary>
        public void AssignSystemDefaults(CropViewItem viewItem, Farm farm, GlobalSettings globalSettings)
        {
            viewItem.IsInitialized = false;

            Trace.TraceInformation($"{nameof(FieldResultsService)}.{nameof(AssignSystemDefaults)}: applying defaults to {viewItem.CropTypeString}");

            var defaults = farm.Defaults;

            _initializationService.InitializeNitrogenFixation(viewItem);
            _initializationService.InitializeCarbonConcentration(viewItem, defaults);
            _initializationService.InitializeIrrigationWaterApplication(farm, viewItem);
            _initializationService.InitializeBiomassCoefficients(viewItem, farm);
            _initializationService.InitializeNitrogenContent(viewItem, farm);

            this.AssignSoilProperties(viewItem, farm);

            _initializationService.InitializePercentageReturns(farm, viewItem);
            _initializationService.InitializeMoistureContent(viewItem, farm);
            _initializationService.InitializeTillageType(viewItem, farm);
            _initializationService.InitializeYield(viewItem, farm);
            _initializationService.InitializeHerbicideEnergy(farm, viewItem);
            _initializationService.InitializeFuelEnergy(farm, viewItem);

            this.AssignFallowDefaultsIfApplicable(viewItem, farm);
            this.AssignPerennialDefaultsIfApplicable(viewItem, farm);
            this.AssignHarvestMethod(viewItem, farm);
            this.AssignDefaultLigninContent(viewItem, farm);

            if (viewItem.CropType == CropType.RangelandNative)
            {
                viewItem.IsNativeGrassland = true;
            }
            else
            {
                viewItem.IsNativeGrassland = false;
            }

            this.AssignEconomicDefaults(viewItem, farm);

            // Lastly, apply user defaults if user has specified custom values for this crop type
            this.AssignUserDefaults(viewItem, globalSettings);

            viewItem.IsInitialized = true;
            viewItem.CropEconomicData.IsInitialized = true;
        }

        public void AssignHarvestMethod(CropViewItem viewItem, Farm farm)
        {
            if (viewItem.CropType.IsSilageCrop())
            {
                viewItem.HarvestMethod = HarvestMethods.Silage;
            }
            else
            {
                viewItem.HarvestMethod = HarvestMethods.CashCrop;
            }
        }

        public void AssignPerennialDefaultsIfApplicable(CropViewItem viewItem, Farm farm)
        {
            if (viewItem.CropType.IsPerennial())
            {
                viewItem.TillageType = TillageType.NoTill;
                viewItem.PastTillageType = TillageType.NoTill;
                viewItem.FertilizerApplicationMethodology = FertilizerApplicationMethodologies.Broadcast;
                viewItem.ForageUtilizationRate = _utilizationRatesForLivestockGrazingProvider.GetUtilizationRate(viewItem.CropType);
                viewItem.TotalBiomassHarvest = viewItem.DefaultYield;
            }
        }

        public void AssignFallowDefaultsIfApplicable(CropViewItem viewItem, Farm farm)
        {
            if (viewItem.CropType.IsFallow())
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

        public void AssignUserDefaults(CropViewItem viewItem, GlobalSettings globalSettings)
        {
            // Check if user has defaults defined for the type of crop
            var cropDefaults = globalSettings.CropDefaults.SingleOrDefault(x => x.CropType == viewItem.CropType);
            if (cropDefaults == null)
            {
                return;
            }

            if (cropDefaults.EnableCustomUserDefaultsForThisCrop == false)
            {
                // User did not specify defaults for this crop (or just wants to use system defaults) so return from here without modifying the view item further

                return;
            }

            var customCropDefaultsMapperConfiguration = new MapperConfiguration(configuration =>
            {
                // Don't copy the GUID, and do not overwrite the year, name, or area, on the crop
                configuration.CreateMap<CropViewItem, CropViewItem>()
                    .ForMember(x => x.Guid, options => options.Ignore())
                    .ForMember(x => x.Year, options => options.Ignore())
                    .ForMember(x => x.Name, options => options.Ignore())
                    .ForMember(x => x.Area, options => options.Ignore());
            });

            var mapper = customCropDefaultsMapperConfiguration.CreateMapper();

            mapper.Map(cropDefaults, viewItem);
        }

        public void AssignDefaultBlendData(FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            var data = _carbonFootprintForFertilizerBlendsProvider.GetData(fertilizerApplicationViewItem.FertilizerBlendData.FertilizerBlend);
            if (data != null)
            {
                /*
                 * Don't reassign the FertilizerBlendData property to the object returned from the provider since the view model will have attached event handlers
                 * that will get lost of the object is assigned, instead copy individual properties
                 */

                fertilizerApplicationViewItem.FertilizerBlendData.PercentageNitrogen = data.PercentageNitrogen;
                fertilizerApplicationViewItem.FertilizerBlendData.PercentagePhosphorus = data.PercentagePhosphorus;
                fertilizerApplicationViewItem.FertilizerBlendData.PercentagePotassium = data.PercentagePotassium;
                fertilizerApplicationViewItem.FertilizerBlendData.PercentageSulphur = data.PercentageSulphur;
                fertilizerApplicationViewItem.FertilizerBlendData.ApplicationEmissions = data.ApplicationEmissions;
                fertilizerApplicationViewItem.FertilizerBlendData.CarbonDioxideEmissionsAtTheGate = data.CarbonDioxideEmissionsAtTheGate;
            }
        }

        public void AssignDefaultLumCMaxValues(CropViewItem cropViewItem, Farm farm)
        {
            if (!cropViewItem.CropType.IsPerennial() && !cropViewItem.CropType.IsGrassland() && !cropViewItem.CropType.IsFallow() && !cropViewItem.IsBrokenGrassland)
            {
                return;
            }

            var lumCMax = 0d;
            var kValue = 0d;

            var ecozone = _ecodistrictDefaultsProvider.GetEcozone(farm.GeographicData.DefaultSoilData.EcodistrictId);

            if (cropViewItem.CropType.IsPerennial() || cropViewItem.IsBrokenGrassland)
            {
                var changeType = _landManagementChangeHelper.GetPerennialCroppingChangeType(cropViewItem.PastPerennialArea, cropViewItem.Area);
                if (cropViewItem.IsBrokenGrassland)
                {
                    // From v3, if is broken grassland then use values for decrease in area when looking up lumc and k
                    changeType = PerennialCroppingChangeType.DecreaseInPerennialCroppingArea;
                }

                lumCMax = _lumCMaxKValuesPerennialCroppingChangeProvider.GetLumCMax(ecozone, farm.GeographicData.DefaultSoilData.SoilTexture, changeType);
                kValue = _lumCMaxKValuesPerennialCroppingChangeProvider.GetKValue(ecozone, farm.GeographicData.DefaultSoilData.SoilTexture, changeType);
            }
            else if (cropViewItem.CropType.IsFallow())
            {
                var changeType = _landManagementChangeHelper.GetFallowPracticeChangeType(cropViewItem.PastFallowArea, cropViewItem.Area);

                lumCMax = _lumCMaxKValuesFallowPracticeChangeProvider.GetLumCMax(ecozone, farm.GeographicData.DefaultSoilData.SoilTexture, changeType);
                kValue = _lumCMaxKValuesFallowPracticeChangeProvider.GetKValue(ecozone, farm.GeographicData.DefaultSoilData.SoilTexture, changeType);
            }

            cropViewItem.LumCMax = lumCMax;
            cropViewItem.KValue = kValue;
        }

        public void AssignSoilProperties(CropViewItem viewItem, Farm farm)
        {
            var soilData = farm.GetPreferredSoilData(viewItem);

            viewItem.Sand = soilData.ProportionOfSandInSoil;
        }

        /// <summary>
        /// When not using a blended P fertilizer approach, use this to assign a P rate directly to the crop
        /// </summary>
        public void AssignDefaultPhosphorusFertilizerRate(CropViewItem viewItem, Farm farm)
        {
            var soilData = farm.GetPreferredSoilData(viewItem);
            var province = soilData.Province;

            // Start with a default then get lookup value if one is available
            viewItem.PhosphorusFertilizerRate = 25;

            var residueData = this.GetResidueData(viewItem, farm);
            if (residueData != null)
            {
                if (residueData.PhosphorusFertilizerRateTable.ContainsKey(province))
                {
                    var phosphorusFertilizerTable = residueData.PhosphorusFertilizerRateTable[province];
                    if (phosphorusFertilizerTable.ContainsKey(soilData.SoilFunctionalCategory))
                    {
                        var rate = phosphorusFertilizerTable[soilData.SoilFunctionalCategory];
                        viewItem.PhosphorusFertilizerRate = rate;
                    }
                }
            }
        }

        public void AssignDefaultNitrogenFertilizerRate(
            CropViewItem viewItem, 
            Farm farm, 
            FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            // Use the more accurate method to calculate N fertilizer rate.
            viewItem.NitrogenFertilizerRate = this.CalculateRequiredNitrogenFertilizer(
                farm: farm, 
                viewItem: viewItem, 
                fertilizerApplicationViewItem: fertilizerApplicationViewItem);
        }

        private void AssignEconomicDefaults(
            CropViewItem cropViewItem, 
            Farm farm)
        {
            var soilData = farm.GetPreferredSoilData(cropViewItem);

            cropViewItem.CropEconomicData.IsInitialized = false;

            cropViewItem.CropEconomicData = _economicsProvider.Get(
                cropType: cropViewItem.CropType,
                soilFunctionalCategory: soilData.SoilFunctionalCategory,
                province: soilData.Province);

            _economicsHelper.ConvertValuesToMetricIfNecessary(cropViewItem.CropEconomicData, farm);

            cropViewItem.CropEconomicData.IsInitialized = true;
        }

        public void AssignDefaultLigninContent(CropViewItem cropViewItem, Farm farm)
        {
            Providers.Carbon.Table_7_Relative_Biomass_Information_Data table10RelativeBiomassData = this.GetResidueData(cropViewItem, farm);
            
            if (table10RelativeBiomassData != null)
            {
                cropViewItem.LigninContent = table10RelativeBiomassData.LigninContent;
            }
            else
            {
                cropViewItem.LigninContent = 0.0;
            }
        }
    }
}