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

            this.AssignNitrogenFixation(viewItem);
            this.AssignCarbonConcentration(viewItem, defaults);
            this.AssignIrrigation(farm, viewItem);
            this.AssignDefaultBiomassCoefficients(viewItem, farm);
            this.AssignDefaultNitrogenContentValues(viewItem, farm);
            this.AssignSoilProperties(viewItem, farm);
            this.AssignDefaultPercentageReturns(viewItem, farm.Defaults);
            this.AssignDefaultMoistureContent(viewItem, farm);
            this.AssignDefaultTillageTypeForSelectedProvince(viewItem, farm);
            this.AssignYield(viewItem, farm);
            this.AssignDefaultEnergyRequirements(viewItem, farm);
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

        public void AssignYield(CropViewItem viewItem, Farm farm)
        {
            if (viewItem.CropType.IsSilageCropWithoutDefaults())
            {
                AssignDefaultSilageCropYield(viewItem, farm);
            }
            else
            {
                this.AssignDefaultYield(viewItem, farm);
            }
        }

        public void AssignIrrigation(Farm farm, CropViewItem viewItem)
        {
            viewItem.AmountOfIrrigation = _irrigationService.GetDefaultIrrigationForYear(farm, viewItem.Year);
            viewItem.GrowingSeasonIrrigation = _irrigationService.GetGrowingSeasonIrrigation(farm, viewItem);
        }

        public void AssignCarbonConcentration(CropViewItem viewItem, Defaults defaults)
        {
            viewItem.CarbonConcentration = defaults.CarbonConcentration;
        }

        public void AssignNitrogenFixation(CropViewItem viewItem)
        {
            viewItem.NitrogenFixationPercentage = _nitrogenFixationProvider.GetNitrogenFixationResult(viewItem.CropType).Fixation * 100;
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

        public void AssignDefaultEnergyRequirements(CropViewItem viewItem, Farm farm)
        {
            var soilData = farm.GetPreferredSoilData(viewItem);

            var fuelEnergyEstimates = _fuelEnergyEstimatesProvider.GetFuelEnergyEstimatesDataInstance(
                province: soilData.Province,
                soilCategory: soilData.SoilFunctionalCategory,
                tillageType: viewItem.TillageType,
                cropType: viewItem.CropType);

            var herbicideEnergyEstimates = _herbicideEnergyEstimatesProvider.GetHerbicideEnergyDataInstance(
                provinceName: soilData.Province,
                soilCategory: soilData.SoilFunctionalCategory,
                tillageType: viewItem.TillageType,
                cropType: viewItem.CropType);

            if (fuelEnergyEstimates != null)
            {
                viewItem.FuelEnergy = fuelEnergyEstimates.FuelEstimate;
            }

            if (herbicideEnergyEstimates != null)
            {
                viewItem.HerbicideEnergy = herbicideEnergyEstimates.HerbicideEstimate;
            }
        }

        /// <summary>
        /// Assigns default percentage return to soil values for a <see cref="H.Core.Models.LandManagement.Fields.CropViewItem"/>.
        /// </summary>
        public void AssignDefaultPercentageReturns(CropViewItem viewItem, Defaults defaults)
        {
            if (viewItem.CropType.IsPerennial())
            {
                viewItem.PercentageOfProductYieldReturnedToSoil = defaults.PercentageOfProductReturnedToSoilForPerennials;
                viewItem.PercentageOfRootsReturnedToSoil = defaults.PercentageOfRootsReturnedToSoilForPerennials;
            }
            else if (viewItem.CropType.IsSilageCrop())
            {
                viewItem.PercentageOfProductYieldReturnedToSoil = defaults.PercentageOfProductReturnedToSoilForFodderCorn;
                viewItem.PercentageOfStrawReturnedToSoil = defaults.PercentageOfRootsReturnedToSoilForFodderCorn;
            }
            else if (viewItem.CropType.IsAnnual())
            {
                viewItem.PercentageOfProductYieldReturnedToSoil = defaults.PercentageOfProductReturnedToSoilForAnnuals;
                viewItem.PercentageOfRootsReturnedToSoil = defaults.PercentageOfRootsReturnedToSoilForAnnuals;
                viewItem.PercentageOfStrawReturnedToSoil = defaults.PercentageOfStrawReturnedToSoilForAnnuals;
            }

            if (viewItem.CropType.IsRootCrop())
            {
                viewItem.PercentageOfProductYieldReturnedToSoil = defaults.PercentageOfProductReturnedToSoilForRootCrops;
                viewItem.PercentageOfStrawReturnedToSoil = defaults.PercentageOfStrawReturnedToSoilForRootCrops;
            }

            if (viewItem.CropType.IsCoverCrop())
            {
                viewItem.PercentageOfProductYieldReturnedToSoil = 100;
                viewItem.PercentageOfStrawReturnedToSoil = 100;
                viewItem.PercentageOfRootsReturnedToSoil = 100;
            }

            if (viewItem.HarvestMethod == HarvestMethods.Silage)
            {
                viewItem.PercentageOfProductYieldReturnedToSoil = 2;
                viewItem.PercentageOfStrawReturnedToSoil = 0;
                viewItem.PercentageOfRootsReturnedToSoil = 100;
            }
            else if (viewItem.HarvestMethod == HarvestMethods.Swathing)
            {
                viewItem.PercentageOfProductYieldReturnedToSoil = 30;
                viewItem.PercentageOfStrawReturnedToSoil = 0;
                viewItem.PercentageOfRootsReturnedToSoil = 100;
            }
            else if (viewItem.HarvestMethod == HarvestMethods.GreenManure)
            {
                viewItem.PercentageOfProductYieldReturnedToSoil = 100;
                viewItem.PercentageOfStrawReturnedToSoil = 0;
                viewItem.PercentageOfRootsReturnedToSoil = 100;
            }
        }

        public void AssignDefaultMoistureContent(CropViewItem cropViewItem, Farm farm)
        {
            var residueData = this.GetResidueData(cropViewItem, farm);

            if (cropViewItem.HarvestMethod == HarvestMethods.GreenManure ||
                cropViewItem.HarvestMethod == HarvestMethods.Silage ||
                cropViewItem.HarvestMethod == HarvestMethods.Swathing ||
                cropViewItem.CropType.IsSilageCrop())
            {
                cropViewItem.MoistureContentOfCropPercentage = 65;
            }
            else
            {
                if (residueData != null)
                {
                    cropViewItem.MoistureContentOfCropPercentage = residueData.MoistureContentOfProduct;
                }
                else
                {
                    cropViewItem.MoistureContentOfCropPercentage = 12;
                }
            }
        }

        /// <summary>
        /// Assigns a default yield to the view item using default yield provider.
        /// </summary>
        public void AssignDefaultYield(
            CropViewItem viewItem,
            Farm farm)
        {
            var province = farm.Province;

            // No small area data exists for years > 2018, take average of last 10 years as placeholder values when considering these years
            const int NoDataYear = 2018;
            const int NumberOfYearsInAverage = 10;
            if (viewItem.Year > NoDataYear)
            {
                var startYear = NoDataYear - NumberOfYearsInAverage;
                var yields = new List<double>();
                for (int year = startYear; year <= NoDataYear; year++)
                {
                    var smallAreaYieldData = _smallAreaYieldProvider.GetData(
                        year: year,
                        polygon: farm.PolygonId,
                        cropType: viewItem.CropType,
                        province: province);

                    if (smallAreaYieldData != null)
                    {
                        yields.Add(smallAreaYieldData.Yield);
                    }
                }

                if (yields.Any())
                {
                    viewItem.Yield = Math.Round(yields.Average(), 1);
                    viewItem.DefaultYield = viewItem.Yield;
                }
                else
                {
                    Trace.TraceWarning($"No default yield data found for {viewItem.CropType.GetDescription()}");
                }

                viewItem.CalculateDryYield();

                return;
            }

            var smallAreaYield = _smallAreaYieldProvider.GetData(
                year: viewItem.Year,
                polygon: farm.PolygonId,
                cropType: viewItem.CropType,
                province: province);

            if (smallAreaYield != null)
            {
                viewItem.Yield = smallAreaYield.Yield;
                viewItem.CalculateDryYield();
            }
            else
            {
                Trace.TraceWarning($"No default yield data found for {viewItem.CropType.GetDescription()} in {viewItem.Year}");
            }
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

        public void AssignYieldToAllYears(IEnumerable<CropViewItem> cropViewItems, Farm farm,
            FieldSystemComponent fieldSystemComponent)
        {
            foreach (var viewItem in cropViewItems)
            {
                this.AssignYieldToYear(farm, viewItem, fieldSystemComponent);
            }
        }

        /// <summary>
        /// Assigns a yield to one view item for a field
        /// </summary>
        public void AssignYieldToYear(
            Farm farm,
            CropViewItem viewItem, 
            FieldSystemComponent fieldSystemComponent)
        {
            var yieldAssignmentMethod = farm.UseFieldLevelYieldAssignement ? fieldSystemComponent.YieldAssignmentMethod : farm.YieldAssignmentMethod;
            if (viewItem.CropType == CropType.NotSelected || viewItem.Year == 0)
            {
                Trace.TraceError($"{nameof(FieldResultsService)}.{nameof(AssignYieldToYear)}: bad crop type or bad year for view item '{viewItem}'");

                viewItem.Yield = 0;
            }

            /*
             * The user will enter (or has entered) yields for each year manually, do not overwrite the value
             */

            if (yieldAssignmentMethod == YieldAssignmentMethod.Custom)
            {
                return;
            }

            /*
             * Use an average of the crops
             */

            if (yieldAssignmentMethod == YieldAssignmentMethod.Average)
            {
                // Create an average from the crops that define the rotation
                var average = fieldSystemComponent.CropViewItems.Average(cropViewItem => cropViewItem.Yield);

                viewItem.Yield = average;

                return;
            }

            if (yieldAssignmentMethod == YieldAssignmentMethod.SmallAreaData || 
                yieldAssignmentMethod == YieldAssignmentMethod.CARValue)
            {
                // If the cropviewitem is of a silage crop, we assign defaults using a different method.
                if (viewItem.CropType.IsSilageCropWithoutDefaults())
                {
                    this.AssignDefaultSilageCropYield(viewItem, farm);
                }
                else
                {
                    this.AssignDefaultYield(viewItem, farm);
                }

                return;
            }

            /*
             * Use values from a custom yield file whose path has been specified by the user
             */

            if (yieldAssignmentMethod == YieldAssignmentMethod.InputFile)
            {
                var results = new List<CustomUserYieldData>();

                foreach (var customYieldItem in farm.GeographicData.CustomYieldData)
                {
                    var yearMatch = customYieldItem.Year == viewItem.Year;
                    var fieldNameMatch = fieldSystemComponent.Name.IndexOf(customYieldItem.FieldName.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0;
                    var farmNameMatch = farm.Name.IndexOf(customYieldItem.RotationName.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0;


                    // Don't assign main year yields to a cover crop yield (for now)
                    if (yearMatch && fieldNameMatch && farmNameMatch && viewItem.IsSecondaryCrop == false)
                    {
                        results.Add(customYieldItem);
                    }
                }

                CustomUserYieldData yieldDataRow = null;
                if (results.Count > 1)
                {
                    yieldDataRow = results.FirstOrDefault(x => x.FieldName.Trim().Equals(fieldSystemComponent.Name.Trim(), StringComparison.InvariantCultureIgnoreCase));
                }
                else if (results.Count == 1)
                {
                    yieldDataRow = results.Single();
                }

                if (yieldDataRow != null)
                {
                    viewItem.Yield = yieldDataRow.Yield;
                }
                else
                {
                    Trace.TraceError($"{nameof(FieldResultsService)}.{nameof(AssignYieldToYear)}: no custom yield data for {viewItem.Year} and {fieldSystemComponent.Name} was found in custom yield file. Attempting to assign a default yield for this year from the default yield provider.");

                    // With the Tier 2 model, we need to have yields for the run-in years. If the user loads a custom yield file, they might not have yields for this period. In this case,
                    // we check if we can get yields for these years by checking the small area data table.
                    this.AssignDefaultYield(
                        viewItem: viewItem,
                        farm: farm); ;

                    if (viewItem.Yield == 0)
                    {
                        Trace.TraceError($"{nameof(FieldResultsService)}.{nameof(AssignYieldToYear)}: no yield data for {viewItem.Year} and {fieldSystemComponent.Name} was found.");
                    }                    
                }

                return;
            }

            throw new Exception("Yield assignment method not accounted for");
        }

        public void AssignDefaultBiomassCoefficients(CropViewItem viewItem, Farm farm)
        {
            var residueData = this.GetResidueData(viewItem, farm);
            if (residueData != null)
            {
                viewItem.BiomassCoefficientProduct = residueData.RelativeBiomassProduct;
                viewItem.BiomassCoefficientStraw = residueData.RelativeBiomassStraw;
                viewItem.BiomassCoefficientRoots = residueData.RelativeBiomassRoot;
                viewItem.BiomassCoefficientExtraroot = residueData.RelativeBiomassExtraroot;

                if (viewItem.HarvestMethod == HarvestMethods.Swathing || viewItem.HarvestMethod == HarvestMethods.GreenManure || viewItem.HarvestMethod == HarvestMethods.Silage)
                {
                    viewItem.BiomassCoefficientProduct = residueData.RelativeBiomassProduct + residueData.RelativeBiomassStraw;
                    viewItem.BiomassCoefficientStraw = 0;
                    viewItem.BiomassCoefficientRoots = residueData.RelativeBiomassRoot;
                    viewItem.BiomassCoefficientExtraroot = residueData.RelativeBiomassExtraroot;
                }
            }
        }

        public void AssignDefaultNitrogenContentValues(CropViewItem viewItem, Farm farm)
        {
            // Assign N content values used for the ICBM methodology
            var residueData = this.GetResidueData(viewItem, farm);
            if (residueData != null)
            {
                // Table has values in grams but unit of display is kg
                viewItem.NitrogenContentInProduct = residueData.NitrogenContentProduct / 1000;
                viewItem.NitrogenContentInStraw = residueData.NitrogenContentStraw / 1000;
                viewItem.NitrogenContentInRoots = residueData.NitrogenContentRoot / 1000;
                viewItem.NitrogenContentInExtraroot = residueData.NitrogenContentExtraroot / 1000;

                if (viewItem.CropType.IsPerennial())
                {
                    viewItem.NitrogenContentInStraw = 0;
                }
            }

            // Assign N content values used for IPCC Tier 2
            var cropData = _slopeProviderTable.GetDataByCropType(viewItem.CropType);
            viewItem.NitrogenContent = cropData.NitrogenContentResidues;
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

        /// <summary>
        /// Sets the tillage type (current & past) for a view item based on the province.
        /// </summary>
        public void AssignDefaultTillageTypeForSelectedProvince(
            CropViewItem viewItem, 
            Farm farm)
        {
            var soilData = farm.GetPreferredSoilData(viewItem);

            var province = soilData.Province;
            var residueData = this.GetResidueData(viewItem, farm);
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

        /// <summary>
        /// Calculates the yield of silage crop using information from the grain crop equivalent to that silage crop e.g. if silage crop is Barley Silage, its grain equivalent will be
        /// Barley. We check in the <see cref="AssignSystemDefaults"/> method if the crop is a silage crop without
        /// default data, if yes, this method is called to calculate the yield of that crop.
        /// </summary>
        /// <param name="silageCropViewItem">The <see cref="H.Core.Models.LandManagement.Fields.CropViewItem"/> representing the silage crop. </param>
        /// <param name="farm">The current farm of the user.</param>
        public void AssignDefaultSilageCropYield(CropViewItem silageCropViewItem, Farm farm)
        {
            // Find the grain crop equivalent of the silage crop.
            var grainCrop = silageCropViewItem.CropType.GetGrainCropEquivalentOfSilageCrop();

            // Create a new CropViewItem that will represent this grain crop. It gets assigned the same year as the silage crop.
            var grainCropViewItem = new CropViewItem
            {
                Year = silageCropViewItem.Year,
                CropType = grainCrop,
            };
            // We call AssignSystemDefaults with the CropViewItem representing the grain crop to get its default values.
            var globalSettings = new GlobalSettings();
            AssignSystemDefaults(grainCropViewItem, farm, globalSettings);

            // We specifically find the PlantCarbonInAgriculturalProduct of the grain crop as that is needed in the yield calculation.
            grainCropViewItem.PlantCarbonInAgriculturalProduct = _icbmSoilCarbonCalculator.CalculatePlantCarbonInAgriculturalProduct(previousYearViewItem:null, currentYearViewItem:grainCropViewItem, farm:farm);

            // We then calculate the wet and dry yield of the crop.
            silageCropViewItem.DryYield = CalculateSilageCropYield(grainCropViewItem: grainCropViewItem);
            silageCropViewItem.CalculateWetWeightYield();

            // No defaults for any grass silages so we use SAD data
            if (silageCropViewItem.CropType == CropType.GrassSilage)
            {
                this.AssignDefaultYield(silageCropViewItem, farm);
                silageCropViewItem.CalculateWetWeightYield();
            }
        }

        /// <summary>
        /// Equation 2.1.2-13
        /// 
        /// Calculates the default yield for a silage crop using information from its grain crop equivalent.
        /// </summary>
        /// <param name="grainCropViewItem">The <see cref="H.Core.Models.LandManagement.Fields.CropViewItem"/> for the grain crop.</param>
        /// <returns>The estimated yield (dry matter) for a silage crop</returns>
        public double CalculateSilageCropYield(CropViewItem grainCropViewItem)
        {
            if (grainCropViewItem.BiomassCoefficientProduct == 0)
            {
                return 0;
            }

            var term1 = grainCropViewItem.Yield + grainCropViewItem.Yield * (grainCropViewItem.BiomassCoefficientStraw / grainCropViewItem.BiomassCoefficientProduct);
            var term2 = term1 * (1 + (grainCropViewItem.PercentageOfProductYieldReturnedToSoil/100.0));
            var result = term2 * (1 - grainCropViewItem.MoistureContentOfCrop);

            return result;
        }
    }
}