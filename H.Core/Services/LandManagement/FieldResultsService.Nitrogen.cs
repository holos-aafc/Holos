using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Soil;
using H.Infrastructure;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService
    {
        #region Public Methods


        public double CalculateAmmoniaEmissionsFromLandAppliedManure(
            CropViewItem cropViewItem,
            Farm farm,
            AnimalComponentEmissionsResults animalComponentEmissionsResults)
        {
            var result = 0.0;

            foreach (var manureApplicationViewItem in cropViewItem.ManureApplicationViewItems)
            {
                var dateOfManureApplication = manureApplicationViewItem.DateOfApplication;
                var temperature = farm.ClimateData.TemperatureData.GetMeanTemperatureForMonth(dateOfManureApplication.Month);

                //// Equation 4.6.1-1
                //var ambientTemperatureAdjustmentForLandApplication = this.CalculateAmbientTemperatureAdjustmentForLandApplication(
                //    temperature: temperature);

                //// Emission factor will depend on the type of manure being applied (solid/liquid)
                //var emissionFactorForApplicationType = 0.0;
                //if (manureApplicationViewItem.ManureStateType.IsLiquidManure())
                //{
                //    emissionFactorForApplicationType = _beefDairyDefaultEmissionFactorsProvider.GetAmmoniaEmissionFactorForLiquidAppliedManure(
                //        manureApplicationType: manureApplicationViewItem.ManureApplicationMethod);
                //}
                //else
                //{
                //    emissionFactorForApplicationType = _beefDairyDefaultEmissionFactorsProvider.GetAmmoniaEmissionFactorForSolidAppliedManure(
                //        tillageType: cropViewItem.TillageType);
                //}

                //// Equation 4.6.1-2
                //var adjustedEmissionFactor = this.CalculateAdjustedAmmoniaEmissionFactor(
                //    emissionFactorForLandApplication: emissionFactorForApplicationType,
                //    ambientTemperatureAdjustment: ambientTemperatureAdjustmentForLandApplication);

                //// Check how much manure was produced on the date of application

            }

            return result;
        }

        /// <summary>
        /// Calculate amount of nitrogen input from all manure applications in a year
        /// </summary>
        /// <returns>The amount of nitrogen input during the year (kg N)</returns>
        public double CalculateManureNitrogenInput(
            CropViewItem cropViewItem,
            Farm farm)
        {
            var result = 0.0;

            foreach (var manureApplicationViewItem in cropViewItem.ManureApplicationViewItems)
            {
                var manureCompositionData = manureApplicationViewItem.DefaultManureCompositionData;

                // Value will be a percentage, so we divide by 100.
                var fractionOfNitrogen = manureCompositionData.NitrogenFraction / 100;

                var amountOfNitrogen = _icbmSoilCarbonCalculator.CalculateAmountOfNitrogenAppliedFromManure(
                    manureAmount: manureApplicationViewItem.AmountOfManureAppliedPerHectare,
                    fractionOfNitrogen);

                result += amountOfNitrogen;
            }

            return Math.Round(result, DefaultNumberOfDecimalPlaces);
        }

        /// <summary>
        /// Determines the amount of N fertilizer required for the specified crop type and yield
        /// </summary>
        public double CalculateRequiredNitrogenFertilizer(
            Farm farm, 
            CropViewItem viewItem, 
            FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            var defaults = farm.Defaults;

            _icbmSoilCarbonCalculator.SetCarbonInputs(
                previousYearViewItem: null,
                currentYearViewItem: viewItem,
                nextYearViewItem: null,
                farm: farm);

            var nitrogenContentOfGrainReturnedToSoil = _singleYearNitrogenEmissionsCalculator.CalculateGrainNitrogenTotal(
                carbonInputFromAgriculturalProduct: viewItem.PlantCarbonInAgriculturalProduct,
                nitrogenConcentrationInProduct: viewItem.NitrogenContentInProduct);

            var nitrogenContentOfStrawReturnedToSoil = _singleYearNitrogenEmissionsCalculator.CalculateStrawNitrogen(
                carbonInputFromStraw: viewItem.CarbonInputFromStraw,
                nitrogenConcentrationInStraw: viewItem.NitrogenContentInStraw);

            var nitrogenContentOfRootReturnedToSoil = _singleYearNitrogenEmissionsCalculator.CalculateRootNitrogen(
                carbonInputFromRoots: viewItem.CarbonInputFromRoots,
                nitrogenConcentrationInRoots: viewItem.NitrogenContentInRoots);

            var nitrogenContentOfExtrarootReturnedToSoil = _singleYearNitrogenEmissionsCalculator.CalculateExtrarootNitrogen(
                carbonInputFromExtraroots: viewItem.CarbonInputFromExtraroots,
                nitrogenConcentrationInExtraroots: viewItem.NitrogenContentInExtraroot);

            var isLeguminousCrop = viewItem.CropType.IsLeguminousCoverCrop() || viewItem.CropType.IsPulseCrop();

            var syntheticFertilizerApplied = _singleYearNitrogenEmissionsCalculator.CalculateSyntheticFertilizerApplied(
                nitrogenContentOfGrainReturnedToSoil: nitrogenContentOfGrainReturnedToSoil,
                nitrogenContentOfStrawReturnedToSoil: nitrogenContentOfStrawReturnedToSoil,
                nitrogenContentOfRootReturnedToSoil: nitrogenContentOfRootReturnedToSoil,
                nitrogenContentOfExtrarootReturnedToSoil: nitrogenContentOfExtrarootReturnedToSoil,
                fertilizerEfficiencyFraction: fertilizerApplicationViewItem.FertilizerEfficiencyFraction,
                soilTestN: viewItem.SoilTestNitrogen,
                isNitrogenFixingCrop: isLeguminousCrop,
                nitrogenFixationAmount: viewItem.NitrogenFixation, 
                atmosphericNitrogenDeposition: viewItem.NitrogenDepositionAmount);

            return syntheticFertilizerApplied;
        }

        /// <summary>
        /// Equation 2.5.2-6
        ///
        /// Calculates the amount of the fertilizer blend needed to support the yield that was input.This considers the amount of nitrogen uptake by the plant and then
        /// converts that value into an amount of fertilizer blend/product
        /// </summary>
        /// <returns>The amount of product required (kg product ha^-1)</returns>
        public double CalculateAmountOfProductRequired(
            Farm farm, 
            CropViewItem viewItem, 
            FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            var requiredNitrogen = CalculateRequiredNitrogenFertilizer(
                farm: farm, 
                viewItem: viewItem, 
                fertilizerApplicationViewItem: fertilizerApplicationViewItem);

            // If blend is custom, default N value will be zero and so we can't calculate the amount of product required
            if (fertilizerApplicationViewItem.FertilizerBlendData.PercentageNitrogen == 0)
            {
                return 0;
            }

            // Need to convert to amount of fertilizer product from required nitrogen
            var requiredAmountOfProduct = (requiredNitrogen / (fertilizerApplicationViewItem.FertilizerBlendData.PercentageNitrogen/100));

            return requiredAmountOfProduct;
        }

        /// <summary>
        /// Equation 4.6.1-1
        /// 
        /// Calculates emissions from the manure specifically applied to the field (kg N2O-N (kg N)^-1).
        /// </summary>
        public double CalculateDirectEmissionsFromFieldSpecificManureSpreading(
            CropViewItem viewItem,
            Farm farm)
        {
            var fieldSpecificOrganicNitrogenEmissionFactor = this.CalculateManureApplicationEmissionFactor(
                viewItem: viewItem,
                farm: farm);

            var totalNitrogenApplied = viewItem.GetTotalManureNitrogenAppliedFromLivestockInYear();

            var result = totalNitrogenApplied * fieldSpecificOrganicNitrogenEmissionFactor;

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-2
        ///
        /// (kg N)
        /// </summary>
        public double CalculateTotalManureNitrogenAppliedToAllFields(FarmEmissionResults farmEmissionResults)
        {
            var result = 0d;

            foreach (var fieldComponentEmissionResult in farmEmissionResults.FieldComponentEmissionResults)
            {
                var viewItem = fieldComponentEmissionResult.FieldSystemComponent.GetSingleYearViewItem();
                var appliedNitrogenFromManure = viewItem.GetTotalManureNitrogenAppliedFromLivestockInYear();

                result += appliedNitrogenFromManure;
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-3
        ///
        /// There can be multiple fields on a farm and the emission factor calculations are field-dependent (accounts for crop type, fertilizer, etc.). So
        /// we take the weight average of these fields when calculating the EF for organic nitrogen(ON)
        /// </summary>
        public double CalculateWeightedOrganicNitrogenEmissionFactor(FarmEmissionResults farmEmissionResults)
        {
            var fieldComponentEmissionResults = farmEmissionResults.FieldComponentEmissionResults;
            if (fieldComponentEmissionResults.Any() == false)
            {
                return 0;
            }

            var fieldAreasAndEmissionFactors = new List<WeightedAverageInput>();

            foreach (var emissionResults in fieldComponentEmissionResults)
            {
                var emissionFactor = this.CalculateManureApplicationEmissionFactor(
                    viewItem: emissionResults.FieldSystemComponent.GetSingleYearViewItem(),
                    farm: farmEmissionResults.Farm);

                fieldAreasAndEmissionFactors.Add(new WeightedAverageInput()
                {
                    Value = emissionFactor,
                    Weight = emissionResults.FieldSystemComponent.FieldArea,
                });
            }

            var weightedEmissionFactor = _singleYearNitrogenEmissionsCalculator.CalculateWeightedEmissionFactor(fieldAreasAndEmissionFactors);

            return weightedEmissionFactor;
        }

        /// <summary>
        /// Equation 4.6.1-4
        /// </summary>
        public double CalculateTotalNitrogenFromLandManureRemaining(
            double totalManureAvailableForLandApplication, 
            double totalManureAlreadyAppliedToFields, 
            double totalManureExported)
        {
            return totalManureAvailableForLandApplication - totalManureAlreadyAppliedToFields - totalManureExported;
        }

        /// <summary>
        /// Equation 4.6.1-5
        /// </summary>
        public double CalculateTotalEmissionsFromRemainingManureThatIsAppliedToAllFields(
            double weightedEmissionFactor, 
            double totalNitrogenFromLandManureRemaining)
        {

            var result = totalNitrogenFromLandManureRemaining * weightedEmissionFactor;

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-6
        /// </summary>
        public double CalculateTotalEmissionsFromExportedManure(FarmEmissionResults farmEmissionResults, 
            double totalExportedManure)
        {
            var weightedEmissionFactor = this.CalculateWeightedOrganicNitrogenEmissionFactor(farmEmissionResults);

            var result = totalExportedManure * weightedEmissionFactor;

            return result;
        }

        /// <summary>
        /// Calculates emissions from all manure that is created by all animal components on the farm (not for a single field). This uses all manure produced
        /// on the farm and not the sum of the manure applications made by the user.
        /// </summary>
        public SoilN2OEmissionsResults CalculateManureN2OEmissionsForFarm(FarmEmissionResults farmEmissionResults)
        {
            var result = new SoilN2OEmissionsResults()
            {
                LandEmissionSource = LandEmissionSourceType.Manure,
                DirectN2OEmissions = 0,
                IndirectN2OEmissions = 0,
                Name = LandEmissionSourceType.Manure.GetDescription(),
            };

            // Equation 2.5.2-20
            var weightedEmissionFactor = this.CalculateWeightedOrganicNitrogenEmissionFactor(farmEmissionResults);

            /*
             * Direct emissions for organic nitrogen inputs from all fields on the farm (considers all manure produced by all animals)
             */

            var totalManureNitrogenAppliedToFields = this.CalculateTotalManureNitrogenAppliedToAllFields(farmEmissionResults);

            /*
             * All manure nitrogen applied to fields will need to be subtracted from the total amount of manure produced on farm. Any remaining amount is still associated with
             * the farm and must be accounted for in the 'left over' manure.
             */

            var remainingManure = this.CalculateTotalNitrogenFromLandManureRemaining(
                totalManureAvailableForLandApplication: farmEmissionResults.TotalAvailableManureNitrogenInStoredManureAvailableForLandApplication,
                totalManureAlreadyAppliedToFields: totalManureNitrogenAppliedToFields,
                totalManureExported: 0);

            // Equation 
            var directEmissionsFromManureApplication = this.CalculateTotalEmissionsFromRemainingManureThatIsAppliedToAllFields(
                weightedEmissionFactor: weightedEmissionFactor,
                totalNitrogenFromLandManureRemaining: remainingManure);

            // Equation 2.5.3-1
            var fractionLeach = _singleYearNitrogenEmissionsCalculator.CalculateFractionOfNitrogenLostByLeachingAndRunoff(
                growingSeasonPrecipitation: farmEmissionResults.Farm.ClimateData.PrecipitationData.GrowingSeasonPrecipitation,
                growingSeasonEvapotranspiration: farmEmissionResults.Farm.ClimateData.EvapotranspirationData.GrowingSeasonEvapotranspiration);

            // Equation 2.5.3-4
            var emissionsFromLeachingAndRunoff = _singleYearNitrogenEmissionsCalculator.CalculateNitrogenEmissionsDueToLeachingAndRunoffFromAllLandAppliedManure(
                totalNitrogenInputsFromLandAppliedManureNitrogen: farmEmissionResults.TotalAvailableManureNitrogenInStoredManureAvailableForLandApplication,
                fractionOfNitrogenLostByLeachingAndRunoff: fractionLeach,
                emissionsFactorForLeachingAndRunoff: farmEmissionResults.Farm.Defaults.EmissionFactorForLeachingAndRunoff);

            // Equation 2.5.3-7
            var emissionsFromVolatilization = _singleYearNitrogenEmissionsCalculator.CalculateNitrogenEmissionsDueToVolatilizationOfAllLandAppliedManure(
                totalNitrogenInputsFromManure: farmEmissionResults.TotalAvailableManureNitrogenInStoredManureAvailableForLandApplication,
                fractionOfNitrogenLostByVolatilization: farmEmissionResults.Farm.Defaults.FractionOfNLostByVolatilization,
                emissionFactorForVolatilization: farmEmissionResults.Farm.Defaults.EmissionFactorForVolatilization);

            // Equation 2.5.4-4
            var indirectEmissionsFromManureApplication = _singleYearNitrogenEmissionsCalculator.CalculateTotalIndirectNitrogenEmissions(
                emissionsDueToLeachingAndRunoff: emissionsFromLeachingAndRunoff,
                emissionsDueToVolatilization: emissionsFromVolatilization);

            // Equation 2.5.4-5
            var totalNEmissions = _singleYearNitrogenEmissionsCalculator.CalculateTotalNitrogenEmissions(
                totalDirectNitrogenEmissions: directEmissionsFromManureApplication,
                totalIndirectNitrogenEmissions: indirectEmissionsFromManureApplication);

            var convertedDirectEmissions = _singleYearNitrogenEmissionsCalculator.ConvertN2ONToN2O(
                n2ONEmissions: directEmissionsFromManureApplication);

            var convertedIndirectEmissions = _singleYearNitrogenEmissionsCalculator.ConvertN2ONToN2O(
                n2ONEmissions: indirectEmissionsFromManureApplication);

            result.DirectN2OEmissions = convertedDirectEmissions;
            result.IndirectN2OEmissions = convertedIndirectEmissions;

            return result;
        }

        /// <summary>
        /// Calculates emissions from all mineralized nitrogen from all field components on the farm (not for one single field)
        /// </summary>
        public SoilN2OEmissionsResults CalculateMineralN2OEmissionsForFarm(FarmEmissionResults farmEmissionResults)
        {
            var result = new SoilN2OEmissionsResults()
            {
                LandEmissionSource = LandEmissionSourceType.Mineral,
                DirectN2OEmissions = 0,
                IndirectN2OEmissions = 0,
                Name = LandEmissionSourceType.Mineral.GetDescription(),
            };

            var fieldComponentEmissionResults = farmEmissionResults.FieldComponentEmissionResults;
            if (fieldComponentEmissionResults.Any() == false)
            {
                return result;
            }

            // C_soil
            var carbonChangeForAllFieldsOnFarm = farmEmissionResults.CarbonChangeForSoils;

            // N_min
            var inputsFromMineralization = _singleYearNitrogenEmissionsCalculator.CalculateNitrogenInputsFromMineralizationOfNativeSoilOrganicMatter(
                carbonChangeInSoil: carbonChangeForAllFieldsOnFarm);

            /*
             * There can be multiple fields on a farm and the emission factor calculations are field-dependent. So
             * we take the weighted average of these fields when calculating the EF for CRN_min since we report the mineralized N for the entire farm
             */
            var fieldAreasAndEmissionFactors = new List<WeightedAverageInput>();

            foreach (var fieldComponentResults in fieldComponentEmissionResults)
            {
                var emissionFactor = this.CalculateNitrogenMineralizationEmissionFactor(
                    fieldSystemComponent: fieldComponentResults.FieldSystemComponent,
                    farm: farmEmissionResults.Farm);

                fieldAreasAndEmissionFactors.Add(new WeightedAverageInput()
                {
                    Value = emissionFactor,
                    Weight = fieldComponentResults.FieldSystemComponent.FieldArea,
                });
            }

            var weightedEmissionFactor = _singleYearNitrogenEmissionsCalculator.CalculateWeightedEmissionFactor(fieldAreasAndEmissionFactors);

            /*
             * Direct emissions for mineralization nitrogen inputs
             */

            // Equation 2.5.4-2
            var directEmissionsFromMineralizedNitrogen = _singleYearNitrogenEmissionsCalculator.CalculateEmissionsFromMineralizedNitrogen(
                inputsFromMineralization: inputsFromMineralization,
                weightedEcodistrictEmissionFactorForMineralizedNitrogen: weightedEmissionFactor);

            /*
             * Indirect emissions for mineralization nitrogen inputs
             */

            // Equation 2.5.3-1
            var fractionLeach = _singleYearNitrogenEmissionsCalculator.CalculateFractionOfNitrogenLostByLeachingAndRunoff(
                growingSeasonPrecipitation: farmEmissionResults.Farm.ClimateData.PrecipitationData.GrowingSeasonPrecipitation,
                growingSeasonEvapotranspiration: farmEmissionResults.Farm.ClimateData.EvapotranspirationData.GrowingSeasonEvapotranspiration);

            // Equation 2.5.3-3
            var emissionsFromLeachingAndRunoff = _singleYearNitrogenEmissionsCalculator.CalculateNitrogenEmissionsDueToLeachingAndRunoffFromMineralizedNitrogen(
                nitrogenInputsFromMineralizationOfNativeSoilOrganicMatter: inputsFromMineralization,
                fractionOfNitrogenLostByLeachingAndRunoff: fractionLeach,
                emissionsFactorForLeachingAndRunoff: farmEmissionResults.Farm.Defaults.EmissionFactorForLeachingAndRunoff);

            // Equation 2.5.4-4
            var indirectEmissionsFromMineralizedNitrogen = _singleYearNitrogenEmissionsCalculator.CalculateTotalIndirectNitrogenEmissions(
                emissionsDueToLeachingAndRunoff: emissionsFromLeachingAndRunoff,
                emissionsDueToVolatilization: 0); // There is no emissions due to volatilization from mineral nitrogen

            // Equation 2.5.4-5
            var totalNEmissions = _singleYearNitrogenEmissionsCalculator.CalculateTotalNitrogenEmissions(
                totalDirectNitrogenEmissions: directEmissionsFromMineralizedNitrogen,
                totalIndirectNitrogenEmissions: indirectEmissionsFromMineralizedNitrogen);

            var convertedDirectEmissions = _singleYearNitrogenEmissionsCalculator.ConvertN2ONToN2O(
                n2ONEmissions: directEmissionsFromMineralizedNitrogen);

            var convertedIndirectEmissions = _singleYearNitrogenEmissionsCalculator.ConvertN2ONToN2O(
                n2ONEmissions: indirectEmissionsFromMineralizedNitrogen);

            result.DirectN2OEmissions = convertedDirectEmissions;
            result.IndirectN2OEmissions = convertedIndirectEmissions;

            return result;
        }

        public SoilN2OEmissionsResults CalculateCropN2OEmissions(
            FieldSystemComponent fieldSystemComponent, 
            Farm farm)
        {
            var result = new SoilN2OEmissionsResults()
            {
                LandEmissionSource = LandEmissionSourceType.Crop,
                Name = fieldSystemComponent.Name + " - " + fieldSystemComponent.CropString,
                FieldSystemComponent = fieldSystemComponent,
            };

            var viewItem = fieldSystemComponent.GetSingleYearViewItem();
            if (viewItem == null)
            {
                return result;
            }

            // From v3 GrasslandForm.vb: If Grassland (native or non native) has fertilizer or is irrigated then it is improved grassland and should be calculated, else it is unimproved and should not be calculated
            if (viewItem.CropType.IsPerennial() && viewItem.IsImprovedGrassland == false)
            {
                return result;
            }

            // Fallow emissions are not considered for N20
            if (viewItem.CropType.IsFallow())
            {
                return result;
            }

            var baseEcodistrictFactor = this.CalculateBaseEcodistrictFactor(farm);

            var croppingSystemModifier = _soilN2OEmissionFactorsProvider.GetFactorForCroppingSystem(
                cropType: viewItem.CropType);

            var tillageModifier = _soilN2OEmissionFactorsProvider.GetFactorForTillagePractice(
                region: farm.DefaultSoilData.Province.GetRegion(),
                cropViewItem: viewItem);

            var syntheticNitrogenModifier = _soilN2OEmissionFactorsProvider.GetFactorForNitrogenSource(
                nitrogenSourceType: Table_15_16_Soil_N2O_Emission_Factors_Provider.NitrogenSourceTypes.SyntheticNitrogen, cropViewItem: viewItem);

            var organicNitrogenModifier = _soilN2OEmissionFactorsProvider.GetFactorForNitrogenSource(
                nitrogenSourceType: Table_15_16_Soil_N2O_Emission_Factors_Provider.NitrogenSourceTypes.OrganicNitrogen,
                cropViewItem: viewItem);

            var cropResidueModifier = _soilN2OEmissionFactorsProvider.GetFactorForNitrogenSource(
                nitrogenSourceType: Table_15_16_Soil_N2O_Emission_Factors_Provider.NitrogenSourceTypes.CropResidueNitrogen, cropViewItem: viewItem);

            // Equation 2.5.1-8
            var emissionFactorForSyntheticFertilizer = _singleYearNitrogenEmissionsCalculator.CalculateEmissionFactor(
                baseEcodistictEmissionFactor: baseEcodistrictFactor,
                croppingSystemModifier: croppingSystemModifier,
                tillageModifier: tillageModifier,
                nitrogenSourceModifier: syntheticNitrogenModifier);

            var emissionFactorForOrganicFertilizer = _singleYearNitrogenEmissionsCalculator.CalculateEmissionFactor(
                baseEcodistictEmissionFactor: baseEcodistrictFactor,
                croppingSystemModifier: croppingSystemModifier,
                tillageModifier: tillageModifier,
                nitrogenSourceModifier: organicNitrogenModifier);

            // Equation 2.5.1-8
            var emissionFactorForCropResidues = _singleYearNitrogenEmissionsCalculator.CalculateEmissionFactor(
                baseEcodistictEmissionFactor: baseEcodistrictFactor,
                croppingSystemModifier: croppingSystemModifier,
                tillageModifier: tillageModifier,
                nitrogenSourceModifier: cropResidueModifier);

            // Equation 2.5.2-2
            var inputsFromSyntheticFertilizer = _singleYearNitrogenEmissionsCalculator.CalculateNitrogenInputsFromSyntheticFertilizer(
                    fertilizerApplied: viewItem.NitrogenFertilizerRate,
                    area: viewItem.Area);

            var emissionsFromSyntheticFertilizer = _singleYearNitrogenEmissionsCalculator.CalculateEmissionsFromSyntheticFetilizer(
                inputsFromSyntheticFertilizer: inputsFromSyntheticFertilizer,
                factor: emissionFactorForSyntheticFertilizer);

            result.EmissionsFromSyntheticFertilizer = emissionsFromSyntheticFertilizer;

            var totalOrganicNitrogenInputs = 0d;
            foreach (var fertilizerApplicationViewItem in viewItem.FertilizerApplicationViewItems.Where(x => x.FertilizerBlendData.FertilizerBlend == FertilizerBlends.CustomOrganic))
            {
                totalOrganicNitrogenInputs += fertilizerApplicationViewItem.AmountOfNitrogenApplied;
            }

            var emissionsFromOrganicFertilizer = _singleYearNitrogenEmissionsCalculator.CalculateEmissionsFromOrganicFetilizer(
                inputsFromOrganicFertilizer: totalOrganicNitrogenInputs,
                factor: emissionFactorForOrganicFertilizer);

            result.FieldSpecificDirectEmissionsFromManureApplication = CalculateDirectEmissionsFromFieldSpecificManureSpreading(viewItem, farm);

            // Equation 2.6.6-2
            var aboveGroundResidueNitrogen = this.CalculateAboveGroundResidueNitrogen(
                previousYearViewItem: null, 
                currentYearViewItem: viewItem, 
                farm: farm);

            result.AboveGroundResidueNitrogen = aboveGroundResidueNitrogen;

            // Equation 2.6.6-5
            var belowGroundResidueNitrogen = this.CalculateBelowGroundResidueNitrogen(
                previousYearViewItem: null, 
                currentYearViewItem: viewItem, 
                farm: farm);

            result.BelowGroundResidueNitrogen = belowGroundResidueNitrogen;

            // Equation 2.5.2-11
            var inputsFromResidueReturned = _singleYearNitrogenEmissionsCalculator.CalculateInputsFromResidueReturned(
                aboveGroundResidue: aboveGroundResidueNitrogen,
                belowGroundResidue: belowGroundResidueNitrogen,
                area: viewItem.Area);

            result.InputFromResiduesReturned = inputsFromResidueReturned;

            // Equation 2.5.2-18
            var emissionsFromResidues = _singleYearNitrogenEmissionsCalculator.CalculateEmissionsFromResidues(
                inputFromResidueReturnedToSoil: inputsFromResidueReturned,
                emissionFactor: emissionFactorForCropResidues);

            result.EmissionsFromInputsFromResidues = emissionsFromResidues;

            // Equation 2.5.3-1
            var fractionLeach = _singleYearNitrogenEmissionsCalculator.CalculateFractionOfNitrogenLostByLeachingAndRunoff(
                growingSeasonPrecipitation: farm.ClimateData.PrecipitationData.GrowingSeasonPrecipitation, 
                growingSeasonEvapotranspiration: farm.ClimateData.EvapotranspirationData.GrowingSeasonEvapotranspiration);

            // Equation 2.5.3-2
            var emissionsFromLeaching = _singleYearNitrogenEmissionsCalculator.CalculateCropLeach(
                inputsFromSyntheticFertilizer: inputsFromSyntheticFertilizer,
                inputsFromResidueReturned: inputsFromResidueReturned,
                factionLeach: fractionLeach,
                emissionFactorLeachingRunoff: farm.Defaults.EmissionFactorForLeachingAndRunoff);

            var fractionOfNitrogenLostByVolatilization = this.CalculateFractionOfNitrogenLostByVolatilization(
                cropViewItem: viewItem,
                farm);

            // Equation 2.5.3-5
            var emissionsFromVolatilization = _singleYearNitrogenEmissionsCalculator.CalculateNitrogenEmissionsDueToVolatizationFromCropland(
                nitrogenInputsFromSyntheticFertilizer: inputsFromSyntheticFertilizer,
                fractionOfNitrogenLostByVolatilization: fractionOfNitrogenLostByVolatilization,
                emissionFactorForVolatilization: farm.Defaults.EmissionFactorForVolatilization);

            // Equation 2.5.4-1
            var directEmissionsFromCrops = _singleYearNitrogenEmissionsCalculator.CalculateTotalDirectEmissionsForCrop(
                emissionsFromSyntheticFertilizer: emissionsFromSyntheticFertilizer,
                emissionsFromResidues: emissionsFromResidues, 
                emissionsFromOrganicFertilizer: emissionsFromOrganicFertilizer, 
                emissionsFromLandAppliedManure: result.FieldSpecificDirectEmissionsFromManureApplication);

            // Equation 2.5.4-4
            var indirectEmissionsFromCrops = _singleYearNitrogenEmissionsCalculator.CalculateTotalIndirectNitrogenEmissions(
                emissionsDueToLeachingAndRunoff: emissionsFromLeaching,
                emissionsDueToVolatilization: emissionsFromVolatilization);

            // Equation 2.5.4-5
            var totalNEmissions = _singleYearNitrogenEmissionsCalculator.CalculateTotalNitrogenEmissions(
                totalDirectNitrogenEmissions: directEmissionsFromCrops,
                totalIndirectNitrogenEmissions: indirectEmissionsFromCrops);

            var convertedDirectEmissions = _singleYearNitrogenEmissionsCalculator.ConvertN2ONToN2O(
                n2ONEmissions: directEmissionsFromCrops);

            var convertedIndirectEmissions = _singleYearNitrogenEmissionsCalculator.ConvertN2ONToN2O(
                n2ONEmissions: indirectEmissionsFromCrops);

            result.DirectN2OEmissions = convertedDirectEmissions;
            result.IndirectN2OEmissions = convertedIndirectEmissions;

            return result;
        }

        /// <summary>
        /// Equation 2.5.3-5
        /// 
        /// Frac_volatilizationSoil
        ///
        /// <para>This value used to be a constant (0.1) but is now calculated according to crop type, fertilizer type, etc.</para>
        ///
        /// <para>Implements: Table 17. Coefficients used for the Bouwman et al. (2002) equation, which was of the form: emission factor (%) = 100 x exp (sum of relevant coefficients)</para>
        /// </summary>
        public double  CalculateFractionOfNitrogenLostByVolatilization(
            CropViewItem cropViewItem, 
            Farm farm)
        {
            var cropTypeFactor = 0.0;
            if (cropViewItem.CropType.IsPerennial())
            {
                cropTypeFactor = -0.158;
            }
            else
            {
                // Annuals
                cropTypeFactor = -0.045;
            }

            var fertilizerTypeFactor = 0.0;
            if (cropViewItem.NitrogenFertilizerType == NitrogenFertilizerType.Urea)
            {
                fertilizerTypeFactor = 0.666;
            }
            else if (cropViewItem.NitrogenFertilizerType == NitrogenFertilizerType.UreaAmmoniumNitrate)
            {
                fertilizerTypeFactor = 0.282;
            }
            else if (cropViewItem.NitrogenFertilizerType == NitrogenFertilizerType.AnhydrousAmmonia)
            {
                fertilizerTypeFactor = -1.151;
            }
            else
            {
                // Other
                fertilizerTypeFactor = -0.238;
            }

            var methodOfApplicationFactor = 0.0;
            // Footnote 1: Broadcast application of fertilizer is assumed for perennials
            if (cropViewItem.FertilizerApplicationMethodology == FertilizerApplicationMethodologies.Broadcast)
            {
                methodOfApplicationFactor = -1.305;
            }
            else
            {
                methodOfApplicationFactor = -1.895;
            }

            var soilPhFactor = 0.0;
            if (farm.DefaultSoilData.SoilPh < 7.25)
            {
                soilPhFactor = -1;
            }
            else
            {
                soilPhFactor = -0.608;
            }

            var soilCecFactor = 0.0;
            if (farm.DefaultSoilData.SoilCec < 250)
            {
                soilCecFactor = 0.0507;
            }
            else
            {
                soilCecFactor = 0.0848;
            }

            const double temperatureFactor = -0.402;

            var result = (100 * Math.Exp(cropTypeFactor + fertilizerTypeFactor + methodOfApplicationFactor + soilPhFactor + soilCecFactor + temperatureFactor)) / 100;

            return result;
        }

        #endregion

        #region Private Methods

        private double CalculateSyntheticNitrogenEmissionFactor(
           CropViewItem viewItem,
           Farm farm)
        {
            var baseEcodistrictFactor = this.CalculateBaseEcodistrictFactor(farm);

            var croppingSystemModifier = _soilN2OEmissionFactorsProvider.GetFactorForCroppingSystem(
                cropType: viewItem.CropType);

            var tillageModifier = _soilN2OEmissionFactorsProvider.GetFactorForTillagePractice(
                region: farm.DefaultSoilData.Province.GetRegion(),
                cropViewItem: viewItem);

            var nitrogenSourceModifier = _soilN2OEmissionFactorsProvider.GetFactorForNitrogenSource(
                nitrogenSourceType: Table_15_16_Soil_N2O_Emission_Factors_Provider.NitrogenSourceTypes.SyntheticNitrogen, cropViewItem: viewItem);

            var syntheticNitrogenEmissionFactor = _singleYearNitrogenEmissionsCalculator.CalculateEmissionFactor(
                baseEcodistictEmissionFactor: baseEcodistrictFactor,
                croppingSystemModifier: croppingSystemModifier,
                tillageModifier: tillageModifier,
                nitrogenSourceModifier: nitrogenSourceModifier);

            return syntheticNitrogenEmissionFactor;
        }

        private double CalculateNitrogenMineralizationEmissionFactor(
            FieldSystemComponent fieldSystemComponent,
            Farm farm)
        {
            var viewItem = fieldSystemComponent.GetSingleYearViewItem();
            if (viewItem == null)
            {
                return 0;
            }

            return this.CalculateNitrogenMineralizationEmissionFactor(viewItem, farm);
        }

        private double CalculateNitrogenMineralizationEmissionFactor(
            CropViewItem viewItem,
            Farm farm)
        {
            var baseEcodistrictFactor = this.CalculateBaseEcodistrictFactor(farm);

            var croppingSystemModifier = _soilN2OEmissionFactorsProvider.GetFactorForCroppingSystem(
                cropType: viewItem.CropType);

            var tillageModifier = _soilN2OEmissionFactorsProvider.GetFactorForTillagePractice(
                region: farm.DefaultSoilData.Province.GetRegion(),
                cropViewItem: viewItem);

            var nitrogenSourceModifier = _soilN2OEmissionFactorsProvider.GetFactorForNitrogenSource(
                nitrogenSourceType: Table_15_16_Soil_N2O_Emission_Factors_Provider.NitrogenSourceTypes.CropResidueNitrogen, cropViewItem: viewItem);

            var ecodistrictMineralEmissionFactor = _singleYearNitrogenEmissionsCalculator.CalculateEmissionFactor(
                baseEcodistictEmissionFactor: baseEcodistrictFactor,
                croppingSystemModifier: croppingSystemModifier,
                tillageModifier: tillageModifier,
                nitrogenSourceModifier: nitrogenSourceModifier);

            return ecodistrictMineralEmissionFactor;
        }

        private double CalculateManureApplicationEmissionFactor(
            CropViewItem viewItem,
            Farm farm)
        {
            if (viewItem == null)
            {
                return 0;
            }

            var baseEcodistrictFactor = this.CalculateBaseEcodistrictFactor(farm);

            var croppingSystemModifier = _soilN2OEmissionFactorsProvider.GetFactorForCroppingSystem(
                cropType: viewItem.CropType);

            var tillageModifier = _soilN2OEmissionFactorsProvider.GetFactorForTillagePractice(
                region: farm.DefaultSoilData.Province.GetRegion(),
                cropViewItem: viewItem);

            var nitrogenSourceModifier = _soilN2OEmissionFactorsProvider.GetFactorForNitrogenSource(
                nitrogenSourceType: Table_15_16_Soil_N2O_Emission_Factors_Provider.NitrogenSourceTypes.OrganicNitrogen, cropViewItem: viewItem);

            var ecodistrictManureEmissionFactor = _singleYearNitrogenEmissionsCalculator.CalculateEmissionFactor(
                baseEcodistictEmissionFactor: baseEcodistrictFactor,
                croppingSystemModifier: croppingSystemModifier,
                tillageModifier: tillageModifier,
                nitrogenSourceModifier: nitrogenSourceModifier);

            return ecodistrictManureEmissionFactor;
        }

        private void CalculateNitrogenAtInterval(
            CropViewItem previousYearResults,
            CropViewItem currentYearResults,
            CropViewItem nextYearResults,
            Farm farm,
            int yearIndex)
        {
            // For debugging
            var year = farm.CarbonModellingEquilibriumYear + yearIndex;

            var climateParameterOrManagementFactor = farm.Defaults.UseClimateParameterInsteadOfManagementFactor ? currentYearResults.ClimateParameter : currentYearResults.ManagementFactor;

            var aboveGroundResiduePool_AGresidueN = 0d;
            var belowGroundResiduePool_BGresidueN = 0d;
            var manureResiduePool_ManureN = 0d;
            var syntheticNitrogenPool_N_SN = 0d;
            var cropResidueNitrogenPool_N_CropResidues = 0d;
            var mineralizedNitrogenPool_N_min = 0d;
            var organicNitrogenPool_N_ON = 0d;

            // Use the previous years' mineral N pool as the starting point for this year
            var mineralNitrogenPool_N_mineralN = previousYearResults.MineralNitrogenPool_N_mineralN;

            // Use the previous years' microbial pool as the starting point for this year (or a starting value if in equilibrium year)
            var microbeNitrogenPool_N_microbeN = 0d;
            if (yearIndex == 0)
            {
                // Microbe pool always starts with 25 kg N.
                microbeNitrogenPool_N_microbeN = 25;
            }
            else
            {

                microbeNitrogenPool_N_microbeN = previousYearResults.MicrobeNitrogenPool_N_microbeN;
            }

            // These are running sums used for display only
            currentYearResults.TotalNitrogenInputs = 0;
            currentYearResults.TotalNitrogenEmissions = 0;
            currentYearResults.SumOfMineralAndMicrobialPools = 0;

            #region Nitrogen Applications

            // Equation 2.6.1-1
            syntheticNitrogenPool_N_SN = _multiYearNitrousOxideCalculator.CalculateSyntheticNitrogenFromFertilizerApplication(
                syntheticFertilizerApplied: currentYearResults.NitrogenFertilizerRate,
                fieldArea: currentYearResults.Area);

            // Equation 2.6.1-2
            syntheticNitrogenPool_N_SN += _multiYearNitrousOxideCalculator.CalculateSyntheticNitrogenFromDeposition(
                nitrogenDeposition: currentYearResults.NitrogenDepositionAmount,
                fieldArea: currentYearResults.Area);

            // Add the fertilizer and deposition to total inputs (before the SN pool is adjusted)
            currentYearResults.TotalNitrogenInputs += syntheticNitrogenPool_N_SN;

            // Display the SN pool before it is adjusted
            currentYearResults.SyntheticInputsBeforeAdjustment = syntheticNitrogenPool_N_SN;

            #endregion

            #region Crop Residue Nitrogen Inputs

            // Equation 2.5.2-4
            var grainNitrogen = _singleYearNitrogenEmissionsCalculator.CalculateGrainNitrogen(
                carbonInputFromProduct: previousYearResults.CarbonInputFromProduct,
                nitrogenConcentrationInProduct: previousYearResults.NitrogenContentInProduct);

            // Equation 2.5.2-5
            var strawNitrogen = _singleYearNitrogenEmissionsCalculator.CalculateStrawNitrogen(
                carbonInputFromStraw: previousYearResults.CarbonInputFromStraw,
                nitrogenConcentrationInStraw: previousYearResults.NitrogenContentInStraw);

            // Equation 2.5.2-6
            var rootNitrogen = _singleYearNitrogenEmissionsCalculator.CalculateRootNitrogen(
                carbonInputFromRoots: previousYearResults.CarbonInputFromRoots,
                nitrogenConcentrationInRoots: previousYearResults.NitrogenContentInRoots);

            // Equation 2.5.2-7
            var extrarootNitrogen = _singleYearNitrogenEmissionsCalculator.CalculateExtrarootNitrogen(
                carbonInputFromExtraroots: previousYearResults.CarbonInputFromExtraroots,
                nitrogenConcentrationInExtraroots: previousYearResults.NitrogenContentInExtraroot);

            // Equation 2.6.2-2
            var aboveGroundResidueNitrogenForCropAtPreviousInterval = _singleYearNitrogenEmissionsCalculator.CalculateAboveGroundResidueNitrogen(
                nitrogenContentOfGrainReturned: grainNitrogen,
                nitrogenContentOfStrawReturned: strawNitrogen);

            // Equation 2.6.2-5
            var belowGroundResidueNitrogenForCropAtPreviousInterval = _singleYearNitrogenEmissionsCalculator.CalculateBelowGroundResidueNitrogen(
                nitrogenContentOfRootReturned: rootNitrogen,
                nitrogenContentOfExtrarootReturned: extrarootNitrogen);

            // Above ground N inputs from crop are not adjusted later on, so we can display them at this point
            currentYearResults.AboveGroundNitrogenResidueForCrop = aboveGroundResidueNitrogenForCropAtPreviousInterval;
            currentYearResults.BelowGroundResidueNitrogenForCrop = belowGroundResidueNitrogenForCropAtPreviousInterval;

            if (yearIndex == 0)
            {
                // Calculate the above and below ground starting crop residue pools for the field (t = 0)

                // Equation 2.6.2-1
                aboveGroundResiduePool_AGresidueN = _multiYearNitrousOxideCalculator.CalculateAboveGroundResidueNitrogenAtEquilibrium(
                    carbonInputFromProduct: previousYearResults.CarbonInputFromProduct,
                    nitrogenConcentrationInProduct: previousYearResults.NitrogenContentInProduct,
                    carbonInputFromStraw: previousYearResults.CarbonInputFromStraw,
                    nitrogenConcentrationInStraw: previousYearResults.NitrogenContentInStraw,
                    climateFactor: climateParameterOrManagementFactor,
                    youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool);

                // Equation 2.6.2-4
                belowGroundResiduePool_BGresidueN = _multiYearNitrousOxideCalculator.CalculateBelowGroundResidueNitrogenAtEquilibrium(
                    carbonInputFromRoots: previousYearResults.CarbonInputFromRoots,
                    nitrogenConcentrationInRoots: previousYearResults.NitrogenContentInRoots,
                    carbonInputFromExtraroots: previousYearResults.CarbonInputFromExtraroots,
                    nitrogenConcentrationInExtraroots: previousYearResults.NitrogenContentInExtraroot,
                    youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool,
                    climateFactor: climateParameterOrManagementFactor);
            }
            else
            {
                // Calculate the above and below ground crop residue pools for the field on subsequent years (t > 0)

                // Equation 2.6.2-3
                aboveGroundResiduePool_AGresidueN = _multiYearNitrousOxideCalculator.CalculateAboveGroundResidueNitrogenForFieldAtInterval(
                    aboveGroundResidueNitrogenForFieldAtPreviousInterval: previousYearResults.AboveGroundResiduePool_AGresidueN,
                    aboveGroundResidueNitrogenForCropAtPreviousInterval: aboveGroundResidueNitrogenForCropAtPreviousInterval,
                    climateManagementFactor: climateParameterOrManagementFactor,
                    decompositionRateYoungPool: farm.Defaults.DecompositionRateConstantYoungPool);

                // Equation 2.6.2-6
                belowGroundResiduePool_BGresidueN = _multiYearNitrousOxideCalculator.CalculateBelowGroundResidueNitrogenForFieldAtInterval(
                    belowGroundResidueNitrogenForFieldAtPreviousInterval: previousYearResults.BelowGroundResiduePool_BGresidueN,
                    belowGroundResidueNitrogenForCropAtPreviousInterval: belowGroundResidueNitrogenForCropAtPreviousInterval,
                    climateManagementFactor: climateParameterOrManagementFactor,
                    decompositionRateYoungPool: farm.Defaults.DecompositionRateConstantYoungPool);
            }

            // Equation 2.6.2-7
            manureResiduePool_ManureN = _multiYearNitrousOxideCalculator.CalculateManureResidueNitrogenPool(
                manureResidueNitrogenPoolAtPreviousInterval: previousYearResults.ManureResiduePool_ManureN,
                amountOfManureAppliedAtPreviousInterval: previousYearResults.AmountOfManureApplied,
                decompositionRateYoungPool: farm.Defaults.DecompositionRateConstantYoungPool,
                climateParameter: climateParameterOrManagementFactor);

            #endregion

            #region 2.6.3 Mineralization

            if (yearIndex == 0)
            {
                // Equation 2.6.3-1
                cropResidueNitrogenPool_N_CropResidues = _multiYearNitrousOxideCalculator.CalculateCropResiduesAtStartingPoint(
                    aboveGroundResidueNitrogenForCropAtStartingPoint: aboveGroundResidueNitrogenForCropAtPreviousInterval,
                    belowGroundResidueNitrogenForCropAtStartingPoint: belowGroundResidueNitrogenForCropAtPreviousInterval,
                    decompositionRateConstantYoungPool: farm.Defaults.DecompositionRateConstantYoungPool,
                    climateParameter: climateParameterOrManagementFactor);

                // Equation 2.6.3-3
                organicNitrogenPool_N_ON = _multiYearNitrousOxideCalculator.CalculateAvailabilityOfNitrogenFromManureDecompositionAtStartingPoint(
                    manureResiduePoolAtEquilibrium: manureResiduePool_ManureN,
                    decompositionRateConstantYoungPool: farm.Defaults.DecompositionRateConstantYoungPool,
                    climateParameter: climateParameterOrManagementFactor);
            }
            else
            {
                // Equation 2.6.3-2
                cropResidueNitrogenPool_N_CropResidues = _multiYearNitrousOxideCalculator.CalculateCropResiduesAtInterval(
                    aboveGroundResidueNitrogenForFieldAtCurrentInterval: aboveGroundResiduePool_AGresidueN,
                    aboveGroundResidueNitrogenForFieldAtPreviousInterval: previousYearResults.AboveGroundResiduePool_AGresidueN,
                    aboveGroundResidueNitrogenForCropAtPreviousInterval: previousYearResults.AboveGroundNitrogenResidueForCrop,
                    belowGroundResidueNitrogenForFieldAtCurrentInterval: belowGroundResiduePool_BGresidueN,
                    belowGroundResidueNitrogenForFieldAtPreviousInterval: previousYearResults.BelowGroundResiduePool_BGresidueN,
                    belowGroundResidueNitrogenForCropAtPreviousInterval: previousYearResults.BelowGroundResidueNitrogenForCrop);

                // Equation 2.6.3-4
                organicNitrogenPool_N_ON = _multiYearNitrousOxideCalculator.CalculateAvailabilityOfNitrogenFromManureDecompositionAtInterval(
                    manureResiduePoolAtPreviousInterval: previousYearResults.ManureResiduePool_ManureN,
                    manureResiduePoolAtCurrentInterval: manureResiduePool_ManureN,
                    amountOfManureAppliedAtPreviousInterval: previousYearResults.AmountOfManureApplied);
            }

            // Equation 2.6.3-5
            mineralizedNitrogenPool_N_min = _multiYearNitrousOxideCalculator.CalculateMineralizedNitrogenFromDecompositionOfOldCarbon(
                oldPoolSoilCarbonAtPreviousInterval: previousYearResults.OldPoolSoilCarbon,
                oldPoolDecompositionRate: farm.Defaults.DecompositionRateConstantOldPool,
                climateParameter: climateParameterOrManagementFactor,
                oldCarbonNitrogen: farm.Defaults.OldPoolCarbonN);

            // Display the pools on output grid before they are adjusted
            currentYearResults.CropResiduesBeforeAdjustment = cropResidueNitrogenPool_N_CropResidues;
            currentYearResults.OrganicNitrogenResiduesBeforeAdjustment = organicNitrogenPool_N_ON;

            // This is the mineralized Nitrogen (n_min) before adjustment
            currentYearResults.N_min_FromDecompositionOfOldCarbon = mineralizedNitrogenPool_N_min;

            // Add to total inputs before the pools are reduced. Note that the crop residue N pool is added as an input and not the AGresidueN or the BGresidueN pools
            currentYearResults.TotalNitrogenInputs += cropResidueNitrogenPool_N_CropResidues;
            currentYearResults.TotalNitrogenInputs += mineralizedNitrogenPool_N_min;
            currentYearResults.TotalNitrogenInputs += organicNitrogenPool_N_ON;

            #endregion

            #region Nitrous Oxide Emissions

            var syntheticNitrogenEmissionFactor = this.CalculateSyntheticNitrogenEmissionFactor(currentYearResults, farm);
            var cropResidueNitrogenEmissionFactor = this.CalculateNitrogenMineralizationEmissionFactor(currentYearResults, farm);
            var manureApplicationEmissionFactor = this.CalculateManureApplicationEmissionFactor(currentYearResults, farm);

            // Equation 2.6.4-1
            var directNitrousOxideEmissionsFromSyntheticNitrogen = _multiYearNitrousOxideCalculator.CalculateNitrousOxideEmissionsFromFertilizerApplication(
                nitrogenInputsFromSyntheticFertilizer: syntheticNitrogenPool_N_SN,
                emissionFactor: syntheticNitrogenEmissionFactor);

            // Equation 2.6.4-2
            var directNitrousOxideEmissionsFromCropResidues = _multiYearNitrousOxideCalculator.CalculateNitrousOxideEmissionsFromCropResidues(
                nitrogenInputsFromResidues: cropResidueNitrogenPool_N_CropResidues,
                emissionFactor: cropResidueNitrogenEmissionFactor);

            // TODO: need to get exported inputs value
            // Equation 2.6.4-3
            var directNitrousOxideEmissionsFromExportedCropResidues = _multiYearNitrousOxideCalculator.CalculateNitrousOxideEmissionsFromExportedCropResidues(
                nitrogenInputsFromExportedResidues: 0,
                emissionFactor: cropResidueNitrogenEmissionFactor);

            // Equation 2.6.4-4
            var directNitrousOxideEmissionsFromMineralizedNitrogen = _multiYearNitrousOxideCalculator.CalculateNitrousOxideEmissionsFromNitrogenMineralization(
                nitrogenInputsFromMineralizedNitrogen: mineralizedNitrogenPool_N_min,
                emissionFactor: cropResidueNitrogenEmissionFactor);

            // Equation 2.6.4-5
            var directNitrousOxideEmissionsFromOrganicNitrogen = _multiYearNitrousOxideCalculator.CalculateNitrousOxideEmissionsFromManureApplication(
                nitrogenInputsFromManureApplication: organicNitrogenPool_N_ON,
                emissionFactor: manureApplicationEmissionFactor);

            // TODO: need to get exported inputs value
            // Equation 2.6.4-6
            var directNitrousOxideEmissionsFromExportedOrganicNitrogen = _multiYearNitrousOxideCalculator.CalculateNitrousOxideEmissionsFromManureApplicationExportedFromFarm(
                nitrogenInputsFromManureApplicationExportedFromFarm: 0,
                emissionFactor: manureApplicationEmissionFactor);

            #endregion

            #region Nitric Oxide Emissions

            // Equation 2.6.4-7
            var directNitricOxideEmissionsFromSyntheticNitrogen = _multiYearNitrousOxideCalculator.CalculateNitricOxideEmissionsFromFertilizerApplication(
                nitrousOxideInputsFromSyntheticFertilizer: directNitrousOxideEmissionsFromSyntheticNitrogen,
                nitricOxideRatio: farm.Defaults.NORatio);

            // Equation 2.6.4-8
            var directNitricOxideEmissionsFromCropResidues = _multiYearNitrousOxideCalculator.CalculateNitricOxideEmissionsFromCropResidues(
                nitrousOxideInputsFromResidues: directNitrousOxideEmissionsFromCropResidues,
                nitricOxideRatio: farm.Defaults.NORatio);

            // Equation 2.6.4-9
            var directNitricOxideEmissionsFromExportedCropResidues = _multiYearNitrousOxideCalculator.CalculateNitricOxideEmissionsFromExportedCropResidues(
                nitrousOxideInputsFromExportedResidues: directNitrousOxideEmissionsFromExportedCropResidues,
                nitricOxideRatio: farm.Defaults.NORatio);

            // Equation 2.6.4-10
            var directNitricOxideEmissionsFromMineralizedNitrogen = _multiYearNitrousOxideCalculator.CalculateNitricOxideEmissionsFromNitrogenMineralization(
                nitrousOxideInputsFromMineralizedNitrogen: directNitrousOxideEmissionsFromMineralizedNitrogen,
                nitricOxideRatio: farm.Defaults.NORatio);

            // Equation 2.6.4-11
            var directNitricOxideEmissionsFromOrganicNitrogen = _multiYearNitrousOxideCalculator.CalculateNitricOxideEmissionsFromManureApplication(
                nitrousOxideInputsFromManureApplication: directNitrousOxideEmissionsFromOrganicNitrogen,
                nitricOxideRatio: farm.Defaults.NORatio);

            // TODO: need to get exported inputs value
            // Equation 2.6.4-12
            var directNitricOxideEmissionsFromExportedOrganicNitrogen = _multiYearNitrousOxideCalculator.CalculateNitricOxideEmissionsFromManureApplicationExportedFromFarm(
                nitrousOxideInputsFromManureApplicationExportedFromFarm: directNitrousOxideEmissionsFromExportedOrganicNitrogen,
                nitricOxideRatio: farm.Defaults.NORatio);

            #endregion

            #region Indirect Nitrous Oxide Emissions

            // Equation 2.6.5-1
            // Equation 2.6.5-2
            var fractionOfNitrogenLostByLeachingAndRunoff = _multiYearNitrousOxideCalculator.CalculateFractionOfNitrogenLostByLeachingAndRunoff(
                growingSeasonPrecipitation: farm.ClimateData.PrecipitationData.GrowingSeasonPrecipitation,
                growingSeasonEvapotranspiration: farm.ClimateData.EvapotranspirationData.GrowingSeasonEvapotranspiration);

            // Equation 2.6.5-3
            var nitrousOxideLeachedFromSyntheticNitrogen = _multiYearNitrousOxideCalculator.CalculateIndirectNitrousOxideEmissionsFromLeachingAndRunoffFromSyntheticFertilizer(
                nitrogenInputsFromSyntheticFertilizer: syntheticNitrogenPool_N_SN,
                fractionOfNitrogenLostByLeachingAndRunoff: fractionOfNitrogenLostByLeachingAndRunoff,
                emissionFactorForLeachingAndRunoff: farm.Defaults.EmissionFactorForLeachingAndRunoff);

            // Equation 2.6.5-4
            var nitrousOxideLeachedFromCropResidues = _multiYearNitrousOxideCalculator.CalculateIndirectNitrousOxideEmissionsFromLeachingAndRunoffOfCropResidues(
                nitrogenInputsFromCropResidues: cropResidueNitrogenPool_N_CropResidues,
                fractionOfNitrogenLostByLeachingAndRunoff: fractionOfNitrogenLostByLeachingAndRunoff,
                emissionFactorForLeachingAndRunoff: farm.Defaults.EmissionFactorForLeachingAndRunoff);

            // Equation 2.6.5-5
            var nitrousOxideLeachedFromMineralizedNitrogen = _multiYearNitrousOxideCalculator.CalculateNitrousOxideEmissionsDueToLeachingAndRunoffOfMineralizedNitrogen(
                nitrogenInputsFromMineralizedNitrogen: mineralizedNitrogenPool_N_min,
                fractionOfNitrogenLostByLeachingAndRunoff: fractionOfNitrogenLostByLeachingAndRunoff,
                emissionFactorForVolatilization: farm.Defaults.EmissionFactorForLeachingAndRunoff);

            // Equation 2.6.5-6
            var nitrousOxideLeachedFromOrganicNitrogen = _multiYearNitrousOxideCalculator.CalculateNitrousOxideEmissionsDueToOrganicNitrogen(
                nitrogenInutsFromOrganicNitrogen: organicNitrogenPool_N_ON,
                fractionOfNitrogenLostByLeachingAndRunoff: fractionOfNitrogenLostByLeachingAndRunoff,
                emissionFactorForVolatilization: farm.Defaults.EmissionFactorForLeachingAndRunoff);

            #endregion

            #region Calculation of Actual Amount Leached

            // Equation 2.6.5-7
            var nitrateLeachedFromSyntheticNitrogen = _multiYearNitrousOxideCalculator.CalculateActualAmountOfNitrogenLeachedFromSyntheticFertilizer(
                nitrogenInputsFromSyntheticFertilizer: syntheticNitrogenPool_N_SN,
                fractionOfNitrogenLostByLeaching: fractionOfNitrogenLostByLeachingAndRunoff,
                emissionFactorForLeaching: farm.Defaults.EmissionFactorForLeachingAndRunoff);

            // Equation 2.6.5-8
            var nitrateLeachedFromCropResidues = _multiYearNitrousOxideCalculator.CalculateActualAmountOfNitrogenLeachedFromResidues(
                availabilityOfCropResiduesOnField: cropResidueNitrogenPool_N_CropResidues,
                fractionOfNitrogenLostByLeaching: fractionOfNitrogenLostByLeachingAndRunoff,
                emissionFactorForLeaching: farm.Defaults.EmissionFactorForLeachingAndRunoff);

            // Equation 2.6.5-9
            var nitrateLeachedFromMineralizedNitrogen = _multiYearNitrousOxideCalculator.CalculateActualAmountOfNitrogenLeachedFromMineralizedNitrogen(
                availabilityOfMineralizedNitrogenOnField: mineralizedNitrogenPool_N_min,
                fractionOfNitrogenLostByLeaching: fractionOfNitrogenLostByLeachingAndRunoff,
                emissionFactorForLeaching: farm.Defaults.EmissionFactorForLeachingAndRunoff);

            // Equation 2.6.5-10
            var mitrateEmissionsLeachedFromOrganicNitrogen = _multiYearNitrousOxideCalculator.CalculateActualAmountOfNitrogenLeachedFromOrganicNitrogen(
                nitrogenInutsFromOrganicNitrogen: organicNitrogenPool_N_ON,
                fractionOfNitrogenLostByLeaching: fractionOfNitrogenLostByLeachingAndRunoff,
                emissionFactorForLeaching: farm.Defaults.EmissionFactorForLeachingAndRunoff);

            #endregion

            #region Emissions Due to Volatilization

            var fractionOfNitrogenLostByVolatilization = this.CalculateFractionOfNitrogenLostByVolatilization(
                cropViewItem: currentYearResults,
                farm);

            // Equation 2.6.5-11
            var nitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogen = _multiYearNitrousOxideCalculator.CalculateIndirectNitrousOxideEmissionsBasedOnAmountOfNitrogenVolatilized(
                nitrogenInputsFromSyntheticFertilizer: syntheticNitrogenPool_N_SN,
                fractionOfNitrogenLostByVolatilization: fractionOfNitrogenLostByVolatilization,
                emissionFactorVolatilization: farm.Defaults.EmissionFactorForVolatilization);

            // Equation 2.6.5-13
            var ammoniumEmissionsFromVolatilizationOfSyntheticNitrogen = _multiYearNitrousOxideCalculator.CalculateAmoniaEmissionFromSyntheticFertilizer(
                nitrogenInputsFromSyntheticFertilizer: syntheticNitrogenPool_N_SN,
                fractionOfNitrogenLostByVolatilization: fractionOfNitrogenLostByVolatilization,
                emissionFactorVolatilization: farm.Defaults.EmissionFactorForVolatilization);

            // Equation 2.6.5-14
            var ammoniaEmissionsFromVolatilizationOfOrganicNitrogen = _multiYearNitrousOxideCalculator.CalculateAmoniaEmissionFromOrganicNitrogen(
                nitrogenInputsFromOrganicNitrogen: organicNitrogenPool_N_ON,
                fractionOfNitrogenLostByVolatilization: fractionOfNitrogenLostByVolatilization,
                emissionFactorVolatilization: farm.Defaults.EmissionFactorForVolatilization);

            #endregion

            #region Adjust Active Pools

            // Equation 2.6.6-1
            // Equation 2.6.6-2
            // Equation 2.6.6-3
            syntheticNitrogenPool_N_SN = _multiYearNitrousOxideCalculator.AdjustSyntheticNitrogenPool(
                currentSyntheticNitrogenPool: syntheticNitrogenPool_N_SN,
                directNitrousOxideEmissionsFromSyntheticFertilizer: directNitrousOxideEmissionsFromSyntheticNitrogen,
                nitricOxideEmissionsFromSyntheticFertilizer: directNitricOxideEmissionsFromSyntheticNitrogen,
                nitrousOxideEmissionsFromLeachedSyntheticFertilizer: nitrousOxideLeachedFromSyntheticNitrogen,
                actualAmountOfNitrogenLeachedFromSyntheticFertilizer: nitrateLeachedFromSyntheticNitrogen,
                nitrousOxideEmissionsFromVolatilization: nitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogen,
                ammoniaEmissionsFromSyntheticEmissions: ammoniumEmissionsFromVolatilizationOfSyntheticNitrogen);

            // Equation 2.6.6-4
            // Equation 2.6.6-5
            cropResidueNitrogenPool_N_CropResidues = _multiYearNitrousOxideCalculator.AdjustCropResidueNitrogenPool(
                currentCropResidueNitrogenPool: cropResidueNitrogenPool_N_CropResidues,
                directNitrousOxideEmissionsFromCropResidueNitrogen: directNitrousOxideEmissionsFromCropResidues,
                nitricOxideEmissionsFromCropResidueNitrogen: directNitricOxideEmissionsFromCropResidues,
                nitrousOxideEmissionsFromLeachingOfCropResidueNitrogen: nitrousOxideLeachedFromCropResidues,
                actualAmountOfNitrogenLeachedFromCropResidues: nitrateLeachedFromCropResidues);

            // Equation 2.6.6-6
            // Equation 2.6.6-7
            mineralizedNitrogenPool_N_min = _multiYearNitrousOxideCalculator.AdjustMineralizedNitrogenPool(
                currentMineralizeNitrogenPool: mineralizedNitrogenPool_N_min,
                directNitrousOxideEmissionsFromMineralizedNitrogen: directNitrousOxideEmissionsFromMineralizedNitrogen,
                nitricOxideEmissionsFromMineralizedNitrogen: directNitricOxideEmissionsFromMineralizedNitrogen,
                nitrousOxideEmissionsFromLeachingOfMineralizedNitrogen: nitrousOxideLeachedFromMineralizedNitrogen,
                actualAmountOfNitrogenLeachedFromMineralizedNitrogen: nitrateLeachedFromMineralizedNitrogen);

            // Equation 2.6.6-8
            // Equation 2.6.6-9
            // Equation 2.6.6-10
            organicNitrogenPool_N_ON = _multiYearNitrousOxideCalculator.AdjustOrganicNitrogenPool(
                currentOrganicNitrogenPool: organicNitrogenPool_N_ON,
                directNitrousOxideEmissionsFromOrganicNitrogen: directNitrousOxideEmissionsFromOrganicNitrogen,
                nitricOxideEmissionsFromOrganicNitrogen: directNitricOxideEmissionsFromOrganicNitrogen,
                nitrousOxideEmissionsFromLeachingOfOrganicNitrogen: nitrousOxideLeachedFromOrganicNitrogen,
                actualAmountOfNitrogenFromLeachingOfOrganicNitrogen: mitrateEmissionsLeachedFromOrganicNitrogen,
                nitrousOxideEmissionsFromVolalitlizationOfOrganicNitrogen: 0, // TODO: check if this value is included               
                ammoniaEmissionsFromOrganicNitrogen: ammoniaEmissionsFromVolatilizationOfOrganicNitrogen);

            #endregion

            #region Closing the Nitrogen Budget

            // Equation 2.6.7-1
            // Equation 2.6.7-2
            microbeNitrogenPool_N_microbeN = _multiYearNitrousOxideCalculator.CloseMineralNitrogenPool(
                currentMicrobialNitrogenPool: microbeNitrogenPool_N_microbeN,
                syntheticNitrogenPool: syntheticNitrogenPool_N_SN,
                mineralizedNitrogenPool: mineralizedNitrogenPool_N_min);

            // Equation 2.6.7-3
            // Equation 2.6.7-4
            microbeNitrogenPool_N_microbeN = _multiYearNitrousOxideCalculator.CloseMicrobeNitrogenPool(
                currentMicrobialNitrogenPool: microbeNitrogenPool_N_microbeN,
                cropResiduePool: cropResidueNitrogenPool_N_CropResidues,
                organicNitrogenPool: organicNitrogenPool_N_ON);

            currentYearResults.MicrobialPoolAfterCloseOfBudget = microbeNitrogenPool_N_microbeN;

            #endregion

            #region Nitrogen Requirements for Carbon Change and Crop Growth

            // Equation 2.6.7-5
            // Equation 2.6.7-6
            var ratioBetweenMineralAndMicrobialNitrogen = _multiYearNitrousOxideCalculator.CalculateRatioBetweenMineralAndMicrobialNitrogen(
                   availabilityOfNitrogenInMicrobialPoolOfField: microbeNitrogenPool_N_microbeN,
                   availabilityOfMineralNitrogenOnField: mineralNitrogenPool_N_mineralN);

            currentYearResults.Ratio = ratioBetweenMineralAndMicrobialNitrogen;

            // Equation 2.6.7-7
            var oldPoolNitrogenDemand = _multiYearNitrousOxideCalculator.CalculateNitrogenRequirementForCarbonTransitionFromYoungToOldPool(
                youngPoolHumificationConstantAboveGround: farm.Defaults.HumificationCoefficientAboveGround,
                youngPoolHumificationConstantBelowGround: farm.Defaults.HumificationCoefficientBelowGround,
                youngPoolHumificationConstantManure: farm.Defaults.HumificationCoefficientManure,
                decompositionRateConstantYoungPool: farm.Defaults.DecompositionRateConstantYoungPool,
                decompositionRateConstantOldPool: farm.Defaults.DecompositionRateConstantOldPool,
                youngPoolSoilOrganicCarbonAboveGroundAtPreviousInterval: previousYearResults.YoungPoolSoilCarbonAboveGround,
                youngPoolSoilOrganicCarbonBelowGroundAtPreviousInterval: previousYearResults.YoungPoolSoilCarbonBelowGround,
                youngPoolSoilOrganicCarbonManureAtPreviousInterval: previousYearResults.YoungPoolManureCarbon,
                aboveGroundResidueCarbonInputAtPreviousInterval: previousYearResults.AboveGroundCarbonInput,
                belowGroundResidueCarbonInputAtPreviousInterval: previousYearResults.BelowGroundCarbonInput,
                manureCarbonInputAtPreviousInterval: previousYearResults.ManureCarbonInput,
                oldCarbonN: farm.Defaults.OldPoolCarbonN,
                climateParameter: climateParameterOrManagementFactor);

            // If mineralN is 0, use calculations in 2.6.7-10 and 2.6.7-11
            if (Math.Abs(mineralNitrogenPool_N_mineralN) < double.Epsilon)
            {
                // Equation 2.6.7-10
                // Equation 2.6.7-11

                mineralNitrogenPool_N_mineralN -= oldPoolNitrogenDemand * ratioBetweenMineralAndMicrobialNitrogen;
                microbeNitrogenPool_N_microbeN -= oldPoolNitrogenDemand * (1 - ratioBetweenMineralAndMicrobialNitrogen);
            }
            else
            {
                if (mineralNitrogenPool_N_mineralN > microbeNitrogenPool_N_microbeN)
                {
                    // Equation 2.6.7-8
                    // Equation 2.6.7-9                

                    mineralNitrogenPool_N_mineralN -= oldPoolNitrogenDemand * (1 - ratioBetweenMineralAndMicrobialNitrogen);
                    microbeNitrogenPool_N_microbeN -= oldPoolNitrogenDemand * ratioBetweenMineralAndMicrobialNitrogen;
                }
                else
                {
                    // Equation 2.6.7-10
                    // Equation 2.6.7-11

                    mineralNitrogenPool_N_mineralN -= oldPoolNitrogenDemand * ratioBetweenMineralAndMicrobialNitrogen;
                    microbeNitrogenPool_N_microbeN -= oldPoolNitrogenDemand * (1 - ratioBetweenMineralAndMicrobialNitrogen);
                }
            }

            currentYearResults.MicrobialPoolAfterOldPoolDemandAdjustment = microbeNitrogenPool_N_microbeN;

            // Need to ensure we are passing in 100% of returns since the N uptake is before the harvest

            // Equation 2.6.7-12
            var cropNitrogenDemand = _multiYearNitrousOxideCalculator.CalculateCropNitrogenDemand(
                carbonInputFromProduct: currentYearResults.CarbonInputFromProduct,
                carbonInputFromStraw: currentYearResults.CarbonInputFromStraw,
                carbonInputFromRoots: currentYearResults.CarbonInputFromRoots,
                carbonInputFromExtraroots: currentYearResults.CarbonInputFromExtraroots,
                moistureContentOfCropFraction: currentYearResults.MoistureContentOfCrop,
                nitrogenConcentrationInTheProduct: currentYearResults.NitrogenContentInProduct,
                nitrogenConcentrationInTheStraw: currentYearResults.NitrogenContentInStraw,
                nitrogenConcentrationInTheRoots: currentYearResults.NitrogenContentInRoots,
                nitrogenConcentrationInExtraroots: currentYearResults.NitrogenContentInExtraroot,
                nitrogenFixation: farm.Defaults.DefaultNitrogenFixation);

            currentYearResults.TotalUptake += oldPoolNitrogenDemand;
            currentYearResults.TotalUptake += cropNitrogenDemand;

            // If mineralN is 0, use calculations as in 2.6.7-15 and 2.6.7-16
            if (Math.Abs(mineralNitrogenPool_N_mineralN) < double.Epsilon)
            {
                // Equation 2.6.7-15
                // Equation 2.6.7-16

                mineralNitrogenPool_N_mineralN -= cropNitrogenDemand * ratioBetweenMineralAndMicrobialNitrogen;
                microbeNitrogenPool_N_microbeN -= cropNitrogenDemand * (1 - ratioBetweenMineralAndMicrobialNitrogen);
            }
            else
            {
                if (mineralNitrogenPool_N_mineralN > microbeNitrogenPool_N_microbeN)
                {
                    // Equation 2.6.7-13
                    // Equation 2.6.7-14               

                    mineralNitrogenPool_N_mineralN -= cropNitrogenDemand * (1 - ratioBetweenMineralAndMicrobialNitrogen);
                    microbeNitrogenPool_N_microbeN -= cropNitrogenDemand * ratioBetweenMineralAndMicrobialNitrogen;
                }
                else
                {
                    // Equation 2.6.7-15
                    // Equation 2.6.7-16

                    mineralNitrogenPool_N_mineralN -= cropNitrogenDemand * ratioBetweenMineralAndMicrobialNitrogen;
                    microbeNitrogenPool_N_microbeN -= cropNitrogenDemand * (1 - ratioBetweenMineralAndMicrobialNitrogen);
                }
            }

            currentYearResults.MicrobialPoolAfterCropDemandAdjustment = microbeNitrogenPool_N_microbeN;

            #endregion

            #region Balance the pools

            // Equation 2.6.7-17
            // Equation 2.6.7-18
            var amountOfMicrobeDeath = _multiYearNitrousOxideCalculator.CalculateAmountOfMicrobeDeath(
                currentMicrobialNitrogenPool: microbeNitrogenPool_N_microbeN,
                microbeDeathRate: farm.Defaults.MicrobeDeath);

            // Equation 2.6.7-19
            microbeNitrogenPool_N_microbeN -= amountOfMicrobeDeath;

            // Equation 2.6.7-20
            mineralNitrogenPool_N_mineralN += amountOfMicrobeDeath;

            // Equation 2.6.7-21
            // Equation 2.6.7-22
            var amountOfDenitrification = _multiYearNitrousOxideCalculator.CalculateAmountOfDenitrification(
                availabilityOfMineralNitrogenOnField: mineralNitrogenPool_N_mineralN,
                denitrification: farm.Defaults.Denitrification);

            // Equation 2.6.7-23
            mineralNitrogenPool_N_mineralN -= amountOfDenitrification;

            var mineralNitrogenBalance = 0d;
            var microbialNitrogenBalance = 0d;

            if (yearIndex == 0)
            {
                // Equation 2.6.7-24
                // Equation 2.6.7-25
                mineralNitrogenBalance = mineralNitrogenPool_N_mineralN;
                microbialNitrogenBalance = microbeNitrogenPool_N_microbeN - 25;
            }
            else
            {
                // Equation 2.6.7-26
                // Equation 2.6.7-27
                mineralNitrogenBalance = mineralNitrogenPool_N_mineralN - previousYearResults.MineralNitrogenPool_N_mineralN;
                microbialNitrogenBalance = microbeNitrogenPool_N_microbeN - previousYearResults.MicrobeNitrogenPool_N_microbeN;
            }

            if (mineralNitrogenPool_N_mineralN < 0)
            {
                microbeNitrogenPool_N_microbeN += mineralNitrogenPool_N_mineralN;
                mineralNitrogenPool_N_mineralN = 0;
            }

            #endregion

            // Assign final pool values to view item so calculations for the next year can use values

            currentYearResults.MicrobeDeath = amountOfMicrobeDeath;
            currentYearResults.MineralizedNitrogenPool_N_min = mineralizedNitrogenPool_N_min;
            currentYearResults.OrganicNitrogenPool_N_ON = organicNitrogenPool_N_ON;
            currentYearResults.ManureResiduePool_ManureN = manureResiduePool_ManureN;
            currentYearResults.CropResidueNitrogenPool_N_CropResidues = cropResidueNitrogenPool_N_CropResidues;
            currentYearResults.AboveGroundResiduePool_AGresidueN = aboveGroundResiduePool_AGresidueN;
            currentYearResults.BelowGroundResiduePool_BGresidueN = belowGroundResiduePool_BGresidueN;
            currentYearResults.SyntheticNitrogenPool_N_SN = syntheticNitrogenPool_N_SN;
            currentYearResults.OldPoolNitrogenRequirement = oldPoolNitrogenDemand;
            currentYearResults.CropNitrogenDemand = cropNitrogenDemand;
            currentYearResults.MicrobeNitrogenPool_N_microbeN = microbeNitrogenPool_N_microbeN;
            currentYearResults.MineralNitrogenPool_N_mineralN = mineralNitrogenPool_N_mineralN;

            currentYearResults.SumOfMineralAndMicrobialPools = mineralNitrogenPool_N_mineralN + microbeNitrogenPool_N_microbeN;
            currentYearResults.MineralNitrogenBalance = mineralNitrogenBalance;
            currentYearResults.MicrobialNitrogenBalance = microbialNitrogenBalance;

            /*
             * Total emissions
             */

            #region Nitrous Oxide

            var totalDirectNitrousOxide = _multiYearNitrousOxideCalculator.CalculateDirectNitrousOxideEmissions(
                nitrousOxideEmissionsSyntheicNitrogen: directNitrousOxideEmissionsFromSyntheticNitrogen,
                nitrousOxideEmissionsFromCropResidues: directNitrousOxideEmissionsFromCropResidues,
                directNitrousOxideEmissionsFromMineralizedNitrogen: directNitrousOxideEmissionsFromMineralizedNitrogen,
                directNitrousOxideEmissionsFromOrganicNitrogen: directNitrousOxideEmissionsFromOrganicNitrogen);

            var totalIndirectNitrousOxide = _multiYearNitrousOxideCalculator.CalculateIndirectNitrousOxideEmissions(
                indirectNitrousOxideEmissionsFromLeachingOfSyntheticNitrogen: nitrousOxideLeachedFromSyntheticNitrogen,
                indirectNitrousOxideEmissionsFromLeachingOfCropResidues: nitrousOxideLeachedFromCropResidues,
                indirectNitrousOxideEmissionsFromLeachingOfMineralNitrogen: nitrousOxideLeachedFromMineralizedNitrogen,
                indirectNitrousOxideEmissionsFromLeachingOfOrganicNitrogen: nitrousOxideLeachedFromOrganicNitrogen,
                indirectNitrousOxideEmissionsFromVolatlizationOfSyntheticNitrogen: nitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogen,
                indirectNitrousOxideEmissionsFromVolatilizationOfOrganicNitrogen: 0); // TODO: is this value included (currently crossed out in doc.)

            var totalNitrousOxide = _multiYearNitrousOxideCalculator.CalculateSumOfDirectN2OEmissions(
                directNitrousOxideEmissions: totalDirectNitrousOxide,
                indirectNitrousOxideEmissions: totalIndirectNitrousOxide);

            currentYearResults.TotalNitrogenEmissions += totalNitrousOxide;

            var totalNitrousOxideForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: totalNitrousOxide,
                areaOfField: currentYearResults.Area);

            currentYearResults.TotalNitrousOxideForArea = totalNitrousOxideForArea;

            var directNitrousOxideEmissionsFromSyntheticNitrogenForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: directNitrousOxideEmissionsFromSyntheticNitrogen,
                areaOfField: currentYearResults.Area);

            currentYearResults.DirectNitrousOxideEmissionsFromSyntheticNitrogenForArea = directNitrousOxideEmissionsFromSyntheticNitrogenForArea;

            var directNitrousOxideEmissionsFromCropResiduesForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: directNitrousOxideEmissionsFromCropResidues,
                areaOfField: currentYearResults.Area);

            currentYearResults.DirectNitrousOxideEmissionsFromCropResiduesForArea = directNitrousOxideEmissionsFromCropResiduesForArea;

            var directNitrousOxideEmissionsFromMineralizedNitrogenForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: directNitrousOxideEmissionsFromMineralizedNitrogen,
                areaOfField: currentYearResults.Area);

            currentYearResults.DirectNitrousOxideEmissionsFromMineralizedNitrogenForArea = directNitrousOxideEmissionsFromMineralizedNitrogenForArea;

            var directNitrousOxideEmissionsFromOrganicNitrogenForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: directNitrousOxideEmissionsFromOrganicNitrogen,
                areaOfField: currentYearResults.Area);

            currentYearResults.DirectNitrousOxideEmissionsFromOrganicNitrogenForArea = directNitrousOxideEmissionsFromOrganicNitrogenForArea;

            var totalDirectNitrousOxideForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: totalDirectNitrousOxide,
                areaOfField: currentYearResults.Area);

            currentYearResults.TotalDirectNitrousOxideForArea = totalDirectNitrousOxideForArea;

            var indirectNitrousOxideEmissionsFromSyntheticNitrogenForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: nitrousOxideLeachedFromSyntheticNitrogen,
                areaOfField: currentYearResults.Area);

            currentYearResults.IndirectNitrousOxideEmissionsFromSyntheticNitrogenForArea = indirectNitrousOxideEmissionsFromSyntheticNitrogenForArea;

            var indirectNitrousOxideEmissionsFromCropResiduesForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: nitrousOxideLeachedFromCropResidues,
                areaOfField: currentYearResults.Area);

            currentYearResults.IndirectNitrousOxideEmissionsFromCropResiduesForArea = indirectNitrousOxideEmissionsFromCropResiduesForArea;

            var indirectNitrousOxideEmissionsFromMineralizedNitrogenForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: nitrousOxideLeachedFromMineralizedNitrogen,
                areaOfField: currentYearResults.Area);

            currentYearResults.IndirectNitrousOxideEmissionsFromMineralizedNitrogenForArea = indirectNitrousOxideEmissionsFromMineralizedNitrogenForArea;

            var indirectNitrousOxideEmissionsFromOrganicNitrogenForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: nitrousOxideLeachedFromOrganicNitrogen,
                areaOfField: currentYearResults.Area);

            currentYearResults.IndirectNitrousOxideEmissionsFromOrganicNitrogenForArea = indirectNitrousOxideEmissionsFromOrganicNitrogenForArea;

            var indirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: nitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogen,
                areaOfField: currentYearResults.Area);

            currentYearResults.IndirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForArea = indirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForArea;

            var totalIndirectNitrousOxideForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: totalIndirectNitrousOxide,
                areaOfField: currentYearResults.Area);

            currentYearResults.TotalIndirectNitrousOxideForArea = totalIndirectNitrousOxideForArea;

            #endregion

            #region Nitric Oxide

            var totalNitricOxide = _multiYearNitrousOxideCalculator.CalculateTotalNitricOxideEmissions(
                nitricOxideEmissionsFromLeachingOfSyntheicNitrogen: directNitricOxideEmissionsFromSyntheticNitrogen,
                nitricOxideEmissionsFromLeachingOfCropResidues: directNitricOxideEmissionsFromCropResidues,
                nitricOxideOxideEmissionsFromLeachingOfMineralizedNitrogen: directNitricOxideEmissionsFromMineralizedNitrogen,
                nitricOxideEmissionsFromLeachingOfOrganicNitrogen: directNitricOxideEmissionsFromOrganicNitrogen);

            currentYearResults.TotalNitrogenEmissions += totalNitricOxide;

            var totalNitricOxideForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: totalNitricOxide,
                areaOfField: currentYearResults.Area);

            currentYearResults.TotalNitricOxideForArea = totalNitricOxideForArea;

            var directNitricOxideEmissionsFromSyntheticNitrogenForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: directNitricOxideEmissionsFromSyntheticNitrogen,
                areaOfField: currentYearResults.Area);

            currentYearResults.DirectNitricOxideEmissionsFromSyntheticNitrogenForArea = directNitricOxideEmissionsFromSyntheticNitrogenForArea;

            var directNitricOxideEmissionsFromCropResiduesForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: directNitricOxideEmissionsFromCropResidues,
                areaOfField: currentYearResults.Area);

            currentYearResults.DirectNitricOxideEmissionsFromCropResiduesForArea = directNitricOxideEmissionsFromCropResiduesForArea;

            var directNitricOxideEmissionsFromMineralizedNitrogenForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: directNitricOxideEmissionsFromMineralizedNitrogen,
                areaOfField: currentYearResults.Area);

            currentYearResults.DirectNitricOxideEmissionsFromMineralizedNitrogenForArea = directNitricOxideEmissionsFromMineralizedNitrogenForArea;

            var directNitricOxideEmissionsFromOrganicNitrogenForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: directNitricOxideEmissionsFromOrganicNitrogen,
                areaOfField: currentYearResults.Area);

            currentYearResults.DirectNitricOxideEmissionsFromOrganicNitrogenForArea = directNitricOxideEmissionsFromOrganicNitrogenForArea;

            #endregion

            #region Nitrate Leaching

            var totalNitrateLeaching = _multiYearNitrousOxideCalculator.CalculateTotalNitrateLeachingEmissions(
                nitrateEmissionsFromLeachingOfSyntheticNitrogen: nitrateLeachedFromSyntheticNitrogen,
                nitrateEmissionsFromLeachingOfCropResidues: nitrateLeachedFromCropResidues,
                nitrateEmissionsFromLeachingOfMineralizedNitrogen: nitrateLeachedFromMineralizedNitrogen,
                nitrateEmissionsFromLeachingOfOrganicNitrogen: mitrateEmissionsLeachedFromOrganicNitrogen);

            currentYearResults.TotalNitrogenEmissions += totalNitrateLeaching;

            var totalNitrateLeachingForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: totalNitrateLeaching,
                areaOfField: currentYearResults.Area);

            currentYearResults.TotalNitrateLeachingForArea = totalNitrateLeachingForArea;

            var indirectNitrateFromSyntheticNitrogenForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: nitrateLeachedFromSyntheticNitrogen,
                areaOfField: currentYearResults.Area);

            currentYearResults.IndirectNitrateFromSyntheticNitrogenForArea = indirectNitrateFromSyntheticNitrogenForArea;

            var indirectNitrateFromCropResiduesForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: nitrateLeachedFromCropResidues,
                areaOfField: currentYearResults.Area);

            currentYearResults.IndirectNitrateFromCropResiduesForArea = indirectNitrateFromCropResiduesForArea;

            var indirectNitrateFromMineralizedNitrogenForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: nitrateLeachedFromMineralizedNitrogen,
                areaOfField: currentYearResults.Area);

            currentYearResults.IndirectNitrateFromMineralizedNitrogenForArea = indirectNitrateFromMineralizedNitrogenForArea;

            var indirectNitrateFromOrganicNitrogenForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: mitrateEmissionsLeachedFromOrganicNitrogen,
                areaOfField: currentYearResults.Area);

            currentYearResults.IndirectNitrateFromOrganicNitrogenForArea = indirectNitrateFromOrganicNitrogenForArea;

            #endregion

            #region Ammonia Volatilization

            var totalAmmonium = _multiYearNitrousOxideCalculator.CalculateTotalAmmoniaEmissions(
                ammoniaEmissionsOrganicNitrogen: ammoniumEmissionsFromVolatilizationOfSyntheticNitrogen,
                ammoniaEmissionsFromSyntheicNitrogen: ammoniaEmissionsFromVolatilizationOfOrganicNitrogen);

            currentYearResults.TotalNitrogenEmissions += totalAmmonium;

            var totalAmmoniaForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: totalAmmonium,
                areaOfField: currentYearResults.Area);

            currentYearResults.TotalAmmoniaForArea = totalAmmoniaForArea;

            var indirectAmmoniumEmissionsFromVolatilizationOfOrganicNitrogenForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: ammoniaEmissionsFromVolatilizationOfOrganicNitrogen,
                areaOfField: currentYearResults.Area);

            currentYearResults.IndirectAmmoniumEmissionsFromVolatilizationOfOrganicNitrogenForArea = indirectAmmoniumEmissionsFromVolatilizationOfOrganicNitrogenForArea;

            var indirectAmmoniumEmissionsFromVolatilizationOfSyntheticNitrogenForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: ammoniumEmissionsFromVolatilizationOfSyntheticNitrogen,
                areaOfField: currentYearResults.Area);

            currentYearResults.IndirectAmmoniumEmissionsFromVolatilizationOfSyntheticNitrogenForArea = indirectAmmoniumEmissionsFromVolatilizationOfSyntheticNitrogenForArea;

            #endregion

            #region Denitrification (N2 loss)

            var denitrificationForArea = _multiYearNitrousOxideCalculator.CalculateEmissionsForArea(
                emissions: amountOfDenitrification,
                areaOfField: currentYearResults.Area);

            currentYearResults.DenitrificationForArea = denitrificationForArea;

            #endregion

            // Equation 2.6.8-31
            currentYearResults.TotalNitrogenOutputs = currentYearResults.TotalUptake + currentYearResults.TotalNitrogenEmissions;
            currentYearResults.DifferenceBetweenInputsAndOutputs = currentYearResults.TotalNitrogenInputs - currentYearResults.TotalNitrogenOutputs;

            // Equation 2.6.8-32
            currentYearResults.Overflow = mineralNitrogenPool_N_mineralN + microbeNitrogenPool_N_microbeN - amountOfDenitrification;

            // Sum of emissions from export 

            var totalNitrousOxideEmissionsExported = _multiYearNitrousOxideCalculator.CaculateSumOfExportedNitrousOxideEmissions(
                nitrousOxideEmissionsFromExportedResidues: directNitrousOxideEmissionsFromExportedCropResidues,
                nitrousOxideEmissionsFromExportedOrganics: directNitrousOxideEmissionsFromExportedOrganicNitrogen);

            var totalNitricOxideEmissionsExported = _multiYearNitrousOxideCalculator.CaculateSumOfExportedNitricOxideEmissions(
                nitricOxideEmissionsFromExportedResidues: directNitricOxideEmissionsFromExportedCropResidues,
                nitricOxideEmissionsFromExportedOrganics: directNitricOxideEmissionsFromExportedOrganicNitrogen);

            // Conversions
            var convertedNitrousOxide = _multiYearNitrousOxideCalculator.ConvertNitrousOxide(emission: totalNitrousOxideForArea);
            var convertedNitricOxide = _multiYearNitrousOxideCalculator.ConvertNitricOxide(emission: totalNitricOxideForArea);
            var convertedNitrate = _multiYearNitrousOxideCalculator.ConvertNitrate(emission: totalNitrateLeachingForArea);
            var convertedAmmonium = _multiYearNitrousOxideCalculator.ConvertAmmoniumToNitrousOxide(emission: totalAmmoniaForArea);
            var convertedNitrogen = _multiYearNitrousOxideCalculator.ConvertNitrogen(emission: denitrificationForArea);
        }

        /// <summary>
        /// Equation 2.6.2-2
        /// </summary>
        private double CalculateAboveGroundResidueNitrogen(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            Farm farm)
        {
            _icbmSoilCarbonCalculator.SetCarbonInputs(
                previousYearViewItem: previousYearViewItem,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: null,
                farm: farm);

            var nitrogenContentOfGrainReturnedToSoil = _singleYearNitrogenEmissionsCalculator.CalculateGrainNitrogen(
                carbonInputFromProduct: currentYearViewItem.CarbonInputFromProduct,
                nitrogenConcentrationInProduct: currentYearViewItem.NitrogenContentInProduct);

            var nitrogenContentOfStrawReturnedToSoil = _singleYearNitrogenEmissionsCalculator.CalculateStrawNitrogen(
                carbonInputFromStraw: currentYearViewItem.CarbonInputFromStraw,
                nitrogenConcentrationInStraw: currentYearViewItem.NitrogenContentInStraw);

            var aboveGroundNitrogen = _singleYearNitrogenEmissionsCalculator.CalculateAboveGroundResidueNitrogen(
                nitrogenContentOfGrainReturned: nitrogenContentOfGrainReturnedToSoil,
                nitrogenContentOfStrawReturned: nitrogenContentOfStrawReturnedToSoil);

            return aboveGroundNitrogen;
        }

        /// <summary>
        /// Equation 2.6.2-5
        /// </summary>
        private double CalculateBelowGroundResidueNitrogen(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            Farm farm)
        {
            _icbmSoilCarbonCalculator.SetCarbonInputs(
                previousYearViewItem: previousYearViewItem,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: null,
                farm: farm);

            var nitrogenContentOfRootReturnedToSoil = _singleYearNitrogenEmissionsCalculator.CalculateRootNitrogen(
                carbonInputFromRoots: currentYearViewItem.CarbonInputFromRoots,
                nitrogenConcentrationInRoots: currentYearViewItem.NitrogenContentInRoots);

            var nitrogenContentOfExtrarootReturnedToSoil = _singleYearNitrogenEmissionsCalculator.CalculateExtrarootNitrogen(
                carbonInputFromExtraroots: currentYearViewItem.CarbonInputFromExtraroots,
                nitrogenConcentrationInExtraroots: currentYearViewItem.NitrogenContentInExtraroot);

            var belowGroundNitrogen = _singleYearNitrogenEmissionsCalculator.CalculateBelowGroundResidueNitrogen(
                nitrogenContentOfRootReturned: nitrogenContentOfRootReturnedToSoil,
                nitrogenContentOfExtrarootReturned: nitrogenContentOfExtrarootReturnedToSoil);

            if (currentYearViewItem.CropType.IsPerennial())
            {
                if (farm.EnableCarbonModelling)
                {
                    // Use the stand length as determined by the sequence of perennial crops entered by the user (Hay-Hay-Hay-Wheat = 3 year stand)
                    belowGroundNitrogen = _singleYearNitrogenEmissionsCalculator
                        .CalculateBelowGroundResidueNitrogenForPerennialForage(
                            nitrogenContentOfRootReturned: nitrogenContentOfRootReturnedToSoil,
                            nitrogenContentOfExtrarootReturned: nitrogenContentOfExtrarootReturnedToSoil,
                            perennialStandLength: currentYearViewItem.PerennialStandLength);
                }
                else
                {
                    // Use the input that specifies when the perennial was first seeded
                    belowGroundNitrogen = _singleYearNitrogenEmissionsCalculator
                        .CalculateBelowGroundResidueNitrogenForPerennialForage(
                            nitrogenContentOfRootReturned: nitrogenContentOfRootReturnedToSoil,
                            nitrogenContentOfExtrarootReturned: nitrogenContentOfExtrarootReturnedToSoil,
                            perennialStandLength: currentYearViewItem.YearsSinceYearOfConversion);
                };
            }

            return belowGroundNitrogen;
        }

        private double CalculateBaseEcodistrictFactor(Farm farm)
        {
            var fractionOfLandOccupiedByLowerPortionsOfLandscape = _ecodistrictDefaultsProvider.GetFractionOfLandOccupiedByPortionsOfLandscape(
                ecodistrictId: farm.DefaultSoilData.EcodistrictId,
                province: farm.DefaultSoilData.Province);

            var emissionsDueToLandscapeAndTopography = _singleYearNitrogenEmissionsCalculator.CalculateTopographyEmissions(
                fractionOfLandOccupiedByLowerPortionsOfLandscape: fractionOfLandOccupiedByLowerPortionsOfLandscape,
                growingSeasonPrecipitation: farm.ClimateData.PrecipitationData.GrowingSeasonPrecipitation,
                growingSeasonEvapotranspiration: farm.ClimateData.EvapotranspirationData.GrowingSeasonEvapotranspiration);

            var baseEcodistrictFactor = _singleYearNitrogenEmissionsCalculator.CalculateBaseEcodistrictValue(
                topographyEmission: emissionsDueToLandscapeAndTopography,
                soilTexture: farm.DefaultSoilData.SoilTexture,
                region: farm.DefaultSoilData.Province.GetRegion());

            return baseEcodistrictFactor;
        } 

        #endregion
    }
}