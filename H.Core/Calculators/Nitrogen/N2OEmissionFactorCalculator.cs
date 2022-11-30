using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;
using H.Core.Providers.Soil;
using H.Core.Services.Animals;
using H.Infrastructure;

namespace H.Core.Calculators.Nitrogen
{
    public class N2OEmissionFactorCalculator
    {
        #region Fields

        protected readonly Table_43_Beef_Dairy_Default_Emission_Factors_Provider _beefDairyDefaultEmissionFactorsProvider = new Table_43_Beef_Dairy_Default_Emission_Factors_Provider();
        protected readonly Table_36_Livestock_Emission_Conversion_Factors_Provider _livestockEmissionConversionFactorsProvider = new Table_36_Livestock_Emission_Conversion_Factors_Provider();
        private readonly Table_13_Soil_N2O_Emission_Factors_Provider _soilN2OEmissionFactorsProvider = new Table_13_Soil_N2O_Emission_Factors_Provider();
        private readonly EcodistrictDefaultsProvider _ecodistrictDefaultsProvider = new EcodistrictDefaultsProvider();

        #endregion

        #region Public Methods

        public List<LandApplicationEmissionResult> CalculateAmmoniaEmissionsFromLandAppliedManure(
            Farm farm,
            AnimalComponentEmissionsResults animalComponentEmissionsResults)
        {
            var componentCategory = animalComponentEmissionsResults.Component.ComponentCategory;
            var animalType = componentCategory.GetAnimalTypeFromComponentCategory();
            var totalManureProducedByAnimals = animalComponentEmissionsResults.TotalVolumeOfManureAvailableForLandApplication * 1000;
            var totalTanForLandApplicationOnDate = animalComponentEmissionsResults.TotalTANAvailableForLandApplication;
            var applicationsAndCropByAnimalType = farm.GetManureApplicationsAndAssociatedCropByAnimalType(animalType);
            var results = new List<LandApplicationEmissionResult>();
            var annualPrecipitation = farm.ClimateData.PrecipitationData.GetTotalAnnualPrecipitation();
            var annualTemperature = farm.ClimateData.TemperatureData.GetMeanAnnualTemperature();
            var evapotranspiration = farm.ClimateData.EvapotranspirationData.GetTotalAnnualEvapotranspiration();

            var emissionFactorData = _livestockEmissionConversionFactorsProvider.GetFactors(ManureStateType.Pasture, componentCategory, annualPrecipitation, annualTemperature, evapotranspiration, 0.0, animalType, farm);
            foreach (var tuple in applicationsAndCropByAnimalType)
            {
                var applicationEmissionResult = new LandApplicationEmissionResult();

                var crop = tuple.Item1;
                applicationEmissionResult.CropViewItem = crop;

                var manureApplication = tuple.Item2;

                var date = manureApplication.DateOfApplication;
                var temperature = farm.ClimateData.GetAverageTemperatureForMonthAndYear(date.Year, (Months)date.Month);

                var fractionOfManureUsed = (manureApplication.AmountOfManureAppliedPerHectare * crop.Area) / totalManureProducedByAnimals;
                if (fractionOfManureUsed > 1.0)
                    fractionOfManureUsed = 1.0;

                applicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication = manureApplication.AmountOfNitrogenAppliedPerHectare * crop.Area;

                applicationEmissionResult.TotalVolumeOfManureUsedDuringApplication = manureApplication.AmountOfManureAppliedPerHectare * crop.Area;

                var adjustedEmissionFactor = CalculateAmbientTemperatureAdjustmentForLandApplication(temperature);

                var emissionFactorForLandApplication = GetEmissionFactorForLandApplication(crop, manureApplication);
                var adjustedAmmoniaEmissionFactor = CalculateAdjustedAmmoniaEmissionFactor(emissionFactorForLandApplication, adjustedEmissionFactor);

                var fractionVolatilized = 0d;
                if (animalType.IsBeefCattleType() || animalType.IsDairyCattleType())
                {
                    // Equation 4.6.2-3
                    applicationEmissionResult.AmmoniacalLoss = fractionOfManureUsed * totalTanForLandApplicationOnDate * adjustedAmmoniaEmissionFactor;

                    // Equation 4.6.3-1
                    fractionVolatilized = applicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication > 0 ? applicationEmissionResult.AmmoniacalLoss / applicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication : 0;
                }
                else if (animalType.IsSheepType() || animalType.IsSwineType() || animalType.IsOtherAnimalType())
                {
                    // Equation 4.6.2-7
                    applicationEmissionResult.AmmoniacalLoss = fractionOfManureUsed * applicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * emissionFactorData.VolatilizationFraction;

                    fractionVolatilized = emissionFactorData.VolatilizationFraction;
                }
                else
                {
                    var emissionFraction = 0d;
                    if (temperature >= 15)
                    {
                        emissionFraction = 0.85;
                    }
                    else if (temperature >= 10 && temperature < 15)
                    {
                        emissionFraction = 0.73;
                    }
                    else if (temperature >= 5 && temperature < 10)
                    {
                        emissionFraction = 0.35;
                    }
                    else
                    {
                        emissionFraction = 0.25;
                    }

                    // Equation 4.6.2-5
                    applicationEmissionResult.AmmoniacalLoss = fractionOfManureUsed * totalManureProducedByAnimals * emissionFraction;

                    // Equation 4.6.3-1
                    fractionVolatilized = applicationEmissionResult.AmmoniacalLoss / applicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication;
                }

                // Equation 4.6.2-4
                // Equation 4.6.2-6
                var ammoniaLoss = applicationEmissionResult.AmmoniacalLoss * CoreConstants.ConvertNH3NToNH3;

                // Equation 4.6.3-2
                applicationEmissionResult.TotalN2ONFromManureVolatilized = applicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * fractionVolatilized * emissionFactorData.EmissionFactorVolatilization;

                // Equation 4.6.3-3
                var n2OVolatilized = applicationEmissionResult.TotalN2ONFromManureVolatilized * CoreConstants.ConvertN2ONToN2O;

                // Equation 4.6.3-4
                applicationEmissionResult.AdjustedAmmoniacalLoss = applicationEmissionResult.AmmoniacalLoss - applicationEmissionResult.TotalN2ONFromManureVolatilized;

                // Equation 4.6.3-5
                var adjustedAmmoniaEmissions = applicationEmissionResult.AdjustedAmmoniacalLoss * CoreConstants.ConvertNH3NToNH3;

                var leachingFraction = CalculateLeachingFraction(annualPrecipitation, evapotranspiration);

                // Equation 4.6.4-1
                applicationEmissionResult.TotalN2ONFromManureLeaching = applicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * leachingFraction * emissionFactorData.EmissionFactorLeach;

                // Equation 4.6.4-4
                applicationEmissionResult.TotalNitrateLeached = applicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * leachingFraction * (1.0 - emissionFactorData.EmissionFactorLeach);

                // Equation 4.6.5-1
                applicationEmissionResult.TotalIndirectN2ONEmissions = applicationEmissionResult.TotalN2ONFromManureVolatilized + applicationEmissionResult.TotalN2ONFromManureLeaching;

                // Equation 4.6.5-2
                applicationEmissionResult.TotalIndirectN2OEmissions = applicationEmissionResult.TotalIndirectN2ONEmissions * CoreConstants.ConvertN2ONToN2O;

                results.Add(applicationEmissionResult);
            }

            return results;
        }

        protected double GetEmissionFactorForLandApplication(
            CropViewItem cropViewItem,
            ManureApplicationViewItem manureApplicationViewItem)
        {
            return !manureApplicationViewItem.ManureStateType.IsLiquidManure()
                ? _beefDairyDefaultEmissionFactorsProvider.GetAmmoniaEmissionFactorForSolidAppliedManure(
                    cropViewItem.TillageType)
                : _beefDairyDefaultEmissionFactorsProvider.GetAmmoniaEmissionFactorForLiquidAppliedManure(
                    manureApplicationViewItem.ManureApplicationMethod);
        }

        private double CalculateLeachingFraction(
            double precipitation,
            double potentialEvapotranspiration)
        {
            return 0.3247 * (precipitation / potentialEvapotranspiration) - 0.0247;
        }

        /// <summary>
        /// Equation 4.6.2-1
        /// </summary>
        /// <param name="temperature">The temperature (degrees C) when manure is applied.</param>
        /// <returns>Ambient temperature-based adjustment</returns>
        public double CalculateAmbientTemperatureAdjustmentForLandApplication(double temperature)
        {
            var result = 1 - (0.058 * (15 - temperature));

            if (result > 1)
            {
                return 1;
            }
            else if (result < 0)
            {
                return 0;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// Equation 4.6.2-2
        /// </summary>
        /// <param name="emissionFactorForLandApplication">Default NH3 emission factor for land application</param>
        /// <param name="ambientTemperatureAdjustment">Ambient temperature based adjustment</param>
        /// <returns></returns>
        public double CalculateAdjustedAmmoniaEmissionFactor(
            double emissionFactorForLandApplication,
            double ambientTemperatureAdjustment)
        {
            return emissionFactorForLandApplication * ambientTemperatureAdjustment;
        }

        /// <summary>
        /// Calculate total indirect emissions from all land applied manure to the crop.
        /// </summary>
        public LandApplicationEmissionResult CalculateTotalIndirectEmissionsFromFieldSpecificManureSpreading(
            CropViewItem viewItem,
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResults,
            Farm farm)
        {
            var result = new LandApplicationEmissionResult();
            var indirectEmissionsForAllFields = new List<LandApplicationEmissionResult>();
            foreach (var animalComponentEmissionsResult in animalComponentEmissionsResults)
            {
                indirectEmissionsForAllFields.AddRange(this.CalculateAmmoniaEmissionsFromLandAppliedManure(farm, animalComponentEmissionsResult));
            }

            // This will be a list of all indirect emissions for land applied manure for each year of history for this field
            var indirectEmissionsForField = indirectEmissionsForAllFields.Where(x => x.CropViewItem.FieldSystemComponentGuid.Equals(viewItem.FieldSystemComponentGuid));

            // Filter by year
            var byYear = indirectEmissionsForField.Where(x => x.CropViewItem.Year.Equals(viewItem.Year));

            foreach (var landApplicationEmissionResult in byYear)
            {
                /*
                 * Emissions are for the entire field. Convert to per hectare when adding to other N2O emissions.
                 */

                result.TotalN2ONFromManureLeaching += landApplicationEmissionResult.TotalN2ONFromManureLeaching > 0
                    ? landApplicationEmissionResult.TotalN2ONFromManureLeaching / viewItem.Area
                    : 0;

                result.TotalIndirectN2ONEmissions += landApplicationEmissionResult.TotalIndirectN2ONEmissions > 0
                    ? landApplicationEmissionResult.TotalIndirectN2ONEmissions / viewItem.Area
                    : 0;

                result.TotalNitrateLeached += landApplicationEmissionResult.TotalNitrateLeached > 0
                    ? landApplicationEmissionResult.TotalNitrateLeached / viewItem.Area
                    : 0;

                result.TotalN2ONFromManureVolatilized += landApplicationEmissionResult.TotalN2ONFromManureVolatilized > 0
                    ? landApplicationEmissionResult.TotalN2ONFromManureVolatilized / viewItem.Area
                    : 0;

                result.TotalVolumeOfManureUsedDuringApplication  += landApplicationEmissionResult.TotalVolumeOfManureUsedDuringApplication > 0
                    ? landApplicationEmissionResult.TotalVolumeOfManureUsedDuringApplication / viewItem.Area
                    : 0;

                result.AmmoniacalLoss += landApplicationEmissionResult.AmmoniacalLoss > 0
                    ? landApplicationEmissionResult.AmmoniacalLoss / viewItem.Area
                    : 0;
                result.ActualAmountOfNitrogenAppliedFromLandApplication += landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication > 0
                    ? landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication / viewItem.Area
                    : 0;
            }

            return result;
        }

        public double CalculateBaseEcodistrictFactor(Farm farm)
        {
            var fractionOfLandOccupiedByLowerPortionsOfLandscape = _ecodistrictDefaultsProvider.GetFractionOfLandOccupiedByPortionsOfLandscape(
                ecodistrictId: farm.DefaultSoilData.EcodistrictId,
                province: farm.DefaultSoilData.Province);

            var emissionsDueToLandscapeAndTopography = this.CalculateTopographyEmissions(
                fractionOfLandOccupiedByLowerPortionsOfLandscape: fractionOfLandOccupiedByLowerPortionsOfLandscape,
                growingSeasonPrecipitation: farm.ClimateData.PrecipitationData.GrowingSeasonPrecipitation,
                growingSeasonEvapotranspiration: farm.ClimateData.EvapotranspirationData.GrowingSeasonEvapotranspiration);

            var baseEcodistrictFactor = this.CalculateBaseEcodistrictValue(
                topographyEmission: emissionsDueToLandscapeAndTopography,
                soilTexture: farm.DefaultSoilData.SoilTexture,
                region: farm.DefaultSoilData.Province.GetRegion());

            return baseEcodistrictFactor;
        }

        public double CalculateSyntheticNitrogenEmissionFactor(
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
                nitrogenSourceType: Table_13_Soil_N2O_Emission_Factors_Provider.NitrogenSourceTypes.SyntheticNitrogen, cropViewItem: viewItem);

            var soilReductionFactor = _soilN2OEmissionFactorsProvider.GetReductionFactorBasedOnApplicationMethod(viewItem.SoilReductionFactor);

            var syntheticNitrogenEmissionFactor = this.CalculateEmissionFactor(
                baseEcodistictEmissionFactor: baseEcodistrictFactor,
                croppingSystemModifier: croppingSystemModifier,
                tillageModifier: tillageModifier,
                nitrogenSourceModifier: nitrogenSourceModifier, 
                applicationMethodReductionFactor: soilReductionFactor);

            return syntheticNitrogenEmissionFactor;
        }

        public double CalculateOrganicNitrogenEmissionFactor(
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
                nitrogenSourceType: Table_13_Soil_N2O_Emission_Factors_Provider.NitrogenSourceTypes.OrganicNitrogen, cropViewItem: viewItem);

            var ecodistrictManureEmissionFactor = this.CalculateEmissionFactor(
                baseEcodistictEmissionFactor: baseEcodistrictFactor,
                croppingSystemModifier: croppingSystemModifier,
                tillageModifier: tillageModifier,
                nitrogenSourceModifier: nitrogenSourceModifier);

            return ecodistrictManureEmissionFactor;
        }

        public double GetEmissionFactorForCropResidues(CropViewItem viewItem, Farm farm)
        {
            var baseEcodistrictFactor = this.CalculateBaseEcodistrictFactor(farm);

            var croppingSystemModifier = _soilN2OEmissionFactorsProvider.GetFactorForCroppingSystem(
                cropType: viewItem.CropType);

            var tillageModifier = _soilN2OEmissionFactorsProvider.GetFactorForTillagePractice(
                region: farm.DefaultSoilData.Province.GetRegion(),
                cropViewItem: viewItem);

            var cropResidueModifier = _soilN2OEmissionFactorsProvider.GetFactorForNitrogenSource(
                nitrogenSourceType: Table_13_Soil_N2O_Emission_Factors_Provider.NitrogenSourceTypes.CropResidueNitrogen, cropViewItem: viewItem);

            var emissionFactorForCropResidues = this.CalculateEmissionFactor(
                baseEcodistictEmissionFactor: baseEcodistrictFactor,
                croppingSystemModifier: croppingSystemModifier,
                tillageModifier: tillageModifier,
                nitrogenSourceModifier: cropResidueModifier);

            return emissionFactorForCropResidues;
        }

        /// <summary>
        /// Equation 4.6.1-1
        /// 
        /// Calculates direct emissions from the manure specifically applied to the field (kg N2O-N (kg N)^-1).
        /// </summary>
        public double CalculateDirectN2ONEmissionsFromFieldSpecificManureSpreading(
            CropViewItem viewItem,
            Farm farm)
        {
            var fieldSpecificOrganicNitrogenEmissionFactor = this.CalculateOrganicNitrogenEmissionFactor(
                viewItem: viewItem,
                farm: farm);

            var totalNitrogenApplied = viewItem.GetTotalManureNitrogenAppliedFromLivestockInYear();

            var result = totalNitrogenApplied * fieldSpecificOrganicNitrogenEmissionFactor;

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-3
        ///
        /// There can be multiple fields on a farm and the emission factor calculations are field-dependent (accounts for crop type, fertilizer, etc.). So
        /// we take the weighted average of these fields when calculating the EF for organic nitrogen (ON). This is to be used when calculating direct emissions
        /// from land applied manure.
        /// </summary>
        public double CalculateWeightedOrganicNitrogenEmissionFactor(List<CropViewItem> itemsByYear, Farm farm)
        {
            var fieldAreasAndEmissionFactors = new List<WeightedAverageInput>();

            foreach (var cropViewItem in itemsByYear)
            {
                var emissionFactor = this.CalculateOrganicNitrogenEmissionFactor(
                    viewItem: cropViewItem,
                    farm: farm);

                fieldAreasAndEmissionFactors.Add(new WeightedAverageInput()
                {
                    Value = emissionFactor,
                    Weight = cropViewItem.Area,
                });
            }

            var weightedEmissionFactor = this.CalculateWeightedEmissionFactor(fieldAreasAndEmissionFactors);

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
            var result = totalManureAvailableForLandApplication - totalManureAlreadyAppliedToFields - totalManureExported;
            if (result < 0)
            {
                // Can't have a negative value of manure remaining
                result = 0;
            }

            return result;
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
        public double CalculateLeftOverEmissionsForField(
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResults,
            Farm farm,
            CropViewItem viewItem,
            double weightedEmissionFactor)
        {
            // Get all fields that exist in the same year
            var itemsInYear = farm.GetCropDetailViewItems().Where(x => x.Year == viewItem.Year).ToList();

            // This is the total amount of N from all animals that is available for land application
            var totalNitrogenAvailableForLandApplication = animalComponentEmissionsResults.TotalNitrogenAvailableForLandApplication();

            // This is the total amount of N that the user has specified is applied to all fields
            var totalManureNitrogenAppliedToAllFields = itemsInYear.Sum(x => x.GetTotalManureNitrogenAppliedFromLivestockInYear());

            // The total N after all applications and exports have been subtracted
            var totalNitrogenRemaining = this.CalculateTotalNitrogenFromLandManureRemaining(
                totalNitrogenAvailableForLandApplication, 
                totalManureNitrogenAppliedToAllFields, 
                0);

            // The total N2O-N from the remaining N
            var emissionsFromNitrogenRemaining = this.CalculateTotalEmissionsFromRemainingManureThatIsAppliedToAllFields(
                weightedEmissionFactor: weightedEmissionFactor,
                totalNitrogenFromLandManureRemaining: totalNitrogenRemaining);

            var totalAreaOfAllFields = itemsInYear.Sum(x => x.Area);
            var areaOfField = viewItem.Area;

            // The total N2O-N that is left over and must be applied to this field so that all manure is applied to the fields in the same year (nothing is remaining to be applied)
            var result = emissionsFromNitrogenRemaining * (areaOfField / (double) totalAreaOfAllFields);

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-7
        /// </summary>
        public double CalculateTotalEmissionsFromExportedManure(
            Farm farm,
            double totalExportedManure,
            List<CropViewItem> itemsByYear)
        {
            var weightedEmissionFactor = this.CalculateWeightedOrganicNitrogenEmissionFactor(itemsByYear, farm);

            var result = totalExportedManure * weightedEmissionFactor;

            return result;
        }

        /// <summary>
        /// Equation 2.6.6-11
        /// 
        /// Frac_volatilizationSoil
        ///
        /// <para>This value used to be a constant (0.1) but is now calculated according to crop type, fertilizer type, etc.</para>
        ///
        /// <para>Implements: Table 14. Coefficients used for the Bouwman et al. (2002) equation, which was of the form: emission factor (%) = 100 x exp (sum of relevant coefficients)</para>
        /// </summary>
        public double CalculateFractionOfNitrogenLostByVolatilization(
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

            // Take average of coefficients if there is more than one application
            var fertilizerTypeFactor = 0.0;
            var fertlizerTypeFactors = new List<double>();
            foreach (var fertilizerApplicationViewItem in cropViewItem.FertilizerApplicationViewItems)
            {
                if (fertilizerApplicationViewItem.FertilizerBlendData.FertilizerBlend == FertilizerBlends.Urea)
                {
                    fertilizerTypeFactor = 0.666;
                }
                if (fertilizerApplicationViewItem.FertilizerBlendData.FertilizerBlend == FertilizerBlends.UreaAmmoniumNitrate)
                {
                    fertilizerTypeFactor = 0.282;
                }
                if (fertilizerApplicationViewItem.FertilizerBlendData.FertilizerBlend == FertilizerBlends.AmmoniumSulphate)
                {
                    fertilizerTypeFactor = -1.151;
                }
                else
                {
                    // Other
                    fertilizerTypeFactor = -0.238;
                }

                fertlizerTypeFactors.Add(fertilizerTypeFactor);
            }

            fertilizerTypeFactor = fertlizerTypeFactors.Any() ? fertlizerTypeFactors.Average() : 0;

            var methodOfApplicationFactor = 0.0;
            var methodOfApplicationFactors = new List<double>();
            foreach (var fertilizerApplicationViewItem in cropViewItem.FertilizerApplicationViewItems)
            {
                // Footnote 1: Broadcast application of fertilizer is assumed for perennials
                if (fertilizerApplicationViewItem.FertilizerApplicationMethodology == FertilizerApplicationMethodologies.Broadcast)
                {
                    methodOfApplicationFactor = -1.305;
                }
                else
                {
                    methodOfApplicationFactor = -1.895;
                }

                methodOfApplicationFactors.Add(methodOfApplicationFactor);
            }

            methodOfApplicationFactor = methodOfApplicationFactors.Any() ? methodOfApplicationFactors.Average() : 0;

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

        /// <summary>
        /// Equation 2.5.1-1
        /// Equation 2.5.1-2
        /// </summary>
        /// <param name="precipitation">Growing season precipitation, by ecodistrict (May – October)</param>
        /// <param name="potentialEvapotranspiration">Growing season potential evapotranspiration, by ecodistrict (May – October)</param>
        /// <returns>Ecodistrict emission factor [kg N2O-N (kg N)-1]</returns>
        public double CalculateEcodistrictEmissionFactor(
            double precipitation, 
            double potentialEvapotranspiration)
        {
            if (precipitation > potentialEvapotranspiration)
            {
                var result = this.CalculateEmissionFactorUsingPrecipitation(precipitation);

                return result;
            }
            else
            {
                var result = this.CalculateEmissionFactorUsingPotentialEvapotranspiration(potentialEvapotranspiration);

                return result;
            }
        }

        /// <summary>
        /// Equation 2.5.1-3
        /// Equation 2.5.1-4
        /// Equation 2.5.1-5
        /// </summary>
        /// <param name="fractionOfLandOccupiedByLowerPortionsOfLandscape">Fraction of land occupied by lower portions of landscape (from Rochette et al. 2008)</param>
        /// <param name="growingSeasonPrecipitation">Annual growing season precipitation (May – October)</param>
        /// <param name="growingSeasonEvapotranspiration">Growing season potential evapotranspiration, by ecodistrict (May – October)</param>
        /// <returns>N2O emission factor adjusted due to position in landscape and moisture regime (kg N2O-N)</returns>
        public double CalculateTopographyEmissions(
            double fractionOfLandOccupiedByLowerPortionsOfLandscape,
            double growingSeasonPrecipitation,
            double growingSeasonEvapotranspiration)
        {
            if (Math.Abs(growingSeasonEvapotranspiration) < double.Epsilon)
            {
                return 0;
            }

            var emissionFactorUsingPotentialEvapotranspiration = this.CalculateEmissionFactorUsingPotentialEvapotranspiration(growingSeasonEvapotranspiration);
            var emissionFactorUsingPrecipitation = this.CalculateEmissionFactorUsingPrecipitation(growingSeasonPrecipitation);

            var emissionFactorForIrrigatedSites = emissionFactorUsingPotentialEvapotranspiration;
            var emissionFactorForHumidEnvironments = this.CalculateEcodistrictEmissionFactor(growingSeasonPrecipitation, growingSeasonEvapotranspiration);
            var emissionFactorForDryEnvironments = emissionFactorUsingPotentialEvapotranspiration * fractionOfLandOccupiedByLowerPortionsOfLandscape + (emissionFactorUsingPrecipitation * (1 - fractionOfLandOccupiedByLowerPortionsOfLandscape));

            var result = 0.0;

            // For irrigated sites
            if (Math.Abs(growingSeasonPrecipitation - growingSeasonEvapotranspiration) < double.Epsilon)
            {
                result = emissionFactorForIrrigatedSites;
            }
            
            // For humid environments
            if ((growingSeasonPrecipitation / growingSeasonEvapotranspiration) > 1)
            {
                result = emissionFactorForHumidEnvironments;
            }

            // For dry environments
            if ((growingSeasonPrecipitation / growingSeasonEvapotranspiration) <= 1)
            {
                result = emissionFactorForDryEnvironments;
            }

            return result;
        }

        /// <summary>
        /// Equation 2.5.1-6
        /// </summary>
        /// <param name="soilTexture">The soil texture of the ecodistrict</param>
        /// <param name="region">The region of the ecodistrict</param>
        /// <param name="fractionOfThisTexture">The fraction of the ecodistrict that is comprised of this soil texture type (100% for now)</param>
        /// <returns>A weighted modifier which provides a correction of the EF_Topo in ecodistrict ‘‘i’’ based on the soil texture </returns>
        public double CalculateModifierBasedOnTexture(
            SoilTexture soilTexture, 
            Region region, 
            double fractionOfThisTexture)
        {
            var textureFactor = _soilN2OEmissionFactorsProvider.GetFactorForSoilTexture(
                soilTexture: soilTexture,
                region: region);

            var result = textureFactor * fractionOfThisTexture;

            return result;
        }

        /// <summary>
        /// Equation 2.5.1-7
        /// </summary>
        /// <param name="topographyEmission">N2O emission factor adjusted due to position in landscape and moisture regime (kg N2O-N)</param>
        /// <param name="soilTexture">The soil texture of the ecodistrict</param>
        /// <param name="region">The region of the ecodistrict</param>
        /// <returns>A function of the three factors that create a base ecodistrict specific value that accounts for the climatic, topographic and edaphic characteristics of the spatial unit for lands</returns>
        public double CalculateBaseEcodistrictValue(
            double topographyEmission, 
            SoilTexture soilTexture, 
            Region region)
        {
            var textureModifier = this.CalculateModifierBasedOnTexture(
                    soilTexture: soilTexture,
                    region: region,

                    /*
                     * This is 1 for now since we allow the user to use a single texture in calculations only (i.e. this texture comprises 100% of the area). This
                     * might change in the future
                     */

                    fractionOfThisTexture: 1
                );

            const double winterCorrection = (1.0 / 0.645);

            var result = topographyEmission * textureModifier * winterCorrection;

            return result;
        }

        /// <summary>
        /// Equation 2.5.1-8
        /// </summary>
        /// <param name="baseEcodistictEmissionFactor">A function of the three factors that create a base ecodistrict specific value that accounts for the climatic, topographic and edaphic characteristics of the spatial unit for lands</param>
        /// <param name="croppingSystemModifier">Cropping system modifier (Ann = Annual, Per = Perennial)</param>
        /// <param name="tillageModifier">tillage modifier RF_Till (Conservation or Conventional Tillage)</param>
        /// <param name="nitrogenSourceModifier">N source modifier RF_NSk (SN = Synthetic Nitrogen; ON = Organic Nitrogen; CRN = Crop Residue Nitrogen)</param>
        /// <param name="applicationMethodReductionFactor">Reduction factor based on application method, only applicable to calculations of EF specific for SN</param>
        /// <returns>The EF considering the impact of the N source on the cropping system and site dependent factors associated with rainfall, topography, soil texture, N source type, tillage, cropping system and moisture managment (kg N2O-N kg-1 N) for ecodistrict ‘‘i’’.</returns>
        public double CalculateEmissionFactor(double baseEcodistictEmissionFactor,
            double croppingSystemModifier,
            double tillageModifier,
            double nitrogenSourceModifier, 
            double applicationMethodReductionFactor = 1.0)
        {
            var result = baseEcodistictEmissionFactor * croppingSystemModifier * tillageModifier * nitrogenSourceModifier * applicationMethodReductionFactor;

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-5
        /// </summary>
        /// <param name="nitrogenContentOfGrainReturnedToSoil">Nitrogen content of the grain returned to the soil (kg N ha^-1)</param>
        /// <param name="nitrogenContentOfStrawReturnedToSoil">Nitrogen content of the straw returned to the soil (kg N ha^-1)</param>
        /// <param name="nitrogenContentOfRootReturnedToSoil">Nitrogen content of the root returned to the soil (kg N ha^-1)</param>
        /// <param name="nitrogenContentOfExtrarootReturnedToSoil">Nitrogen content of the extraroot returned to the soil (kg N ha^-1)</param>
        /// <param name="fertilizerEfficiencyFraction">Fertilizer use efficiency (fraction)</param>
        /// <param name="soilTestN">User defined value for existing Soil N supply for which fertilization rate was adapted</param>
        /// <param name="isNitrogenFixingCrop">Indicates if the type of crop is nitrogen fixing.</param>
        /// <param name="nitrogenFixationAmount">The amount of nitrogen fixation by the crop (fraction)</param>
        /// <param name="atmosphericNitrogenDeposition">N deposition on a specific field n (kg ha^-1) </param>
        /// <returns>N fertilizer applied (kg ha^-1)</returns>
        public double CalculateSyntheticFertilizerApplied(double nitrogenContentOfGrainReturnedToSoil,
            double nitrogenContentOfStrawReturnedToSoil,
            double nitrogenContentOfRootReturnedToSoil,
            double nitrogenContentOfExtrarootReturnedToSoil,
            double fertilizerEfficiencyFraction,
            double soilTestN,
            bool isNitrogenFixingCrop,
            double nitrogenFixationAmount, 
            double atmosphericNitrogenDeposition)
        {
            var totalNitrogenContent = (nitrogenContentOfGrainReturnedToSoil + nitrogenContentOfStrawReturnedToSoil + nitrogenContentOfRootReturnedToSoil + nitrogenContentOfExtrarootReturnedToSoil);

            var result = 0d;
            if (isNitrogenFixingCrop)
            {
                result = (totalNitrogenContent * (1 - nitrogenFixationAmount) - soilTestN - atmosphericNitrogenDeposition) / fertilizerEfficiencyFraction;
            }
            else
            {
                result = (totalNitrogenContent - soilTestN - atmosphericNitrogenDeposition) / fertilizerEfficiencyFraction;
            }

            // Suggested amount can never be less than zero
            if (result < 0)
            {
                result = 0;
            }

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-13
        /// </summary>
        public double CalculateGrainNitrogenTotal(
            double carbonInputFromAgriculturalProduct,
            double nitrogenConcentrationInProduct)
        {
            var result = (carbonInputFromAgriculturalProduct / 0.45) * nitrogenConcentrationInProduct;

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-10
        /// </summary>
        /// <param name="carbonInputFromProduct">Carbon input from product (kg ha^-1) </param>
        /// <param name="nitrogenConcentrationInProduct">N concentration in the product (kg kg-1) </param>
        public double CalculateGrainNitrogen(
            double carbonInputFromProduct, 
            double nitrogenConcentrationInProduct)
        {
            var result = (carbonInputFromProduct / 0.45) * nitrogenConcentrationInProduct;

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-11
        /// </summary>
        /// <param name="carbonInputFromStraw">Carbon input from straw (kg ha^-1)</param>
        /// <param name="nitrogenConcentrationInStraw"></param>
        public double CalculateStrawNitrogen(
            double carbonInputFromStraw,
            double nitrogenConcentrationInStraw)
        {
            var result = (carbonInputFromStraw / 0.45) * nitrogenConcentrationInStraw;

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-12
        /// </summary>
        /// <param name="carbonInputFromRoots">Carbon input from roots (kg ha^-1)</param>
        /// <param name="nitrogenConcentrationInRoots">N concentration in the roots (kg kg-1) </param>
        public double CalculateRootNitrogen(
            double carbonInputFromRoots,
            double nitrogenConcentrationInRoots)
        {
            var result = (carbonInputFromRoots / 0.45) * nitrogenConcentrationInRoots;

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-13
        /// </summary>
        /// <param name="carbonInputFromExtraroots">Carbon input from extra-root material (kg ha^-1)</param>
        /// <param name="nitrogenConcentrationInExtraroots">N concentration in the extra root (kg kg-1) (until known from literature, the same N concentration used for roots will be utilized)</param>
        public double CalculateExtrarootNitrogen(
            double carbonInputFromExtraroots,
            double nitrogenConcentrationInExtraroots)
        {
            var result = (carbonInputFromExtraroots / 0.45) * nitrogenConcentrationInExtraroots;

            return result;
        }

        /// <summary>
        /// Equation 2.6.2-2
        /// Equation 2.7.2-3
        /// Equation 2.7.2-5
        /// Equation 2.7.2-7
        /// Equation 2.7.2-9
        /// </summary>
        /// <returns>Above ground residue N (kg N ha^-1)</returns>
        public double CalculateTotalAboveGroundResidueNitrogenUsingIcbm(CropViewItem cropViewItem)
        {
            var nitrogenContentOfGrainReturned = this.CalculateGrainNitrogen(
                carbonInputFromProduct: cropViewItem.CarbonInputFromProduct,
                nitrogenConcentrationInProduct: cropViewItem.NitrogenContentInProduct);

            var nitrogenContentOfStrawReturned = this.CalculateStrawNitrogen(
                carbonInputFromStraw: cropViewItem.CarbonInputFromStraw,
                nitrogenConcentrationInStraw: cropViewItem.NitrogenContentInStraw);

            if (cropViewItem.CropType.IsAnnual() || cropViewItem.CropType.IsPerennial())
            {
                return nitrogenContentOfGrainReturned + nitrogenContentOfStrawReturned;
            }

            if (cropViewItem.CropType.IsRootCrop())
            {
                return nitrogenContentOfStrawReturned;
            }

            if (cropViewItem.CropType.IsCoverCrop() || cropViewItem.CropType.IsSilageCrop())
            {
                return nitrogenContentOfGrainReturned;
            }

            // Fallow
            return 0;
        }

        /// <summary>
        /// Equation 2.6.2-5
        /// Equation 2.7.2-4
        /// Equation 2.7.2-6
        /// Equation 2.7.2-8
        /// Equation 2.7.2-10
        /// </summary>
        /// <param name="cropViewItem"></param>
        /// <returns>Below ground residue N (kg N ha^-1)</returns>
        public double CalculateTotalBelowGroundResidueNitrogenUsingIcbm(CropViewItem cropViewItem)
        {
            var graiNitrogen = this.CalculateGrainNitrogen(
                carbonInputFromProduct: cropViewItem.CarbonInputFromProduct,
                nitrogenConcentrationInProduct: cropViewItem.NitrogenContentInProduct);

            var rootNitrogen = this.CalculateRootNitrogen(
                carbonInputFromRoots: cropViewItem.CarbonInputFromRoots,
                nitrogenConcentrationInRoots: cropViewItem.NitrogenContentInRoots);

            var extrarootNitrogen = this.CalculateExtrarootNitrogen(
                carbonInputFromExtraroots: cropViewItem.CarbonInputFromExtraroots,
                nitrogenConcentrationInExtraroots: cropViewItem.NitrogenContentInExtraroot);

            if (cropViewItem.CropType.IsAnnual() || cropViewItem.CropType.IsPerennial())
            {
                return rootNitrogen + extrarootNitrogen;
            }

            if (cropViewItem.CropType.IsRootCrop())
            {
                return graiNitrogen + extrarootNitrogen;
            }

            if (cropViewItem.CropType.IsSilageCrop() || cropViewItem.CropType.IsCoverCrop())
            {
                return rootNitrogen + extrarootNitrogen;
            }

            return 0;
        }

        /// <summary>
        /// Equation 2.7.2-1
        /// </summary>
        /// <returns></returns>
        public double CalculateTotalAboveGroundResidueNitrogenUsingIpccTier2(
            double aboveGroundResidueDryMatter,
            double carbonConcentration,
            double nitrogenContentInStraw)
        {
            return aboveGroundResidueDryMatter * carbonConcentration * nitrogenContentInStraw;
        }

        /// <summary>
        /// Equation 2.7.2-2
        /// </summary>
        public double CalculateTotalBelowGroundResidueNitrogenUsingIpccTier2(
            double belowGroundResidueDryMatter,
            double carbonConcentration,
            double nitrogenContentInRoots,
            double area)
        {
            // When using below ground residue dry matter as calculated by IPCC, the reside will be for the entire field

            return (belowGroundResidueDryMatter / area)* carbonConcentration * nitrogenContentInRoots;
        }

        /// <summary>
        /// Equation 2.5.2-20
        /// </summary>
        public double CalculateWeightedEmissionFactor(IEnumerable<WeightedAverageInput> areasAndEmissionFactors)
        {
            return areasAndEmissionFactors.WeightedAverage(record => record.Value, record => record.Weight);
        }

        /// <summary>
        /// Equation 2.5.3-1
        /// Equation 2.7.5-1
        /// Equation 2.7.5-2
        /// </summary>
        /// <param name="growingSeasonPrecipitation">Growing season precipitation, by ecodistrict (May – October)</param>
        /// <param name="growingSeasonEvapotranspiration">Growing season potential evapotranspiration, by ecodistrict (May – October)</param>
        /// <returns>Fraction of N lost by leaching and runoff  (kg N (kg N)^-1)</returns>
        public double CalculateFractionOfNitrogenLostByLeachingAndRunoff(
            double growingSeasonPrecipitation, 
            double growingSeasonEvapotranspiration)
        {
            var fractionOfNitrogenLostByLeachingAndRunoff = 0.3247 * (growingSeasonPrecipitation / growingSeasonEvapotranspiration) - 0.0247;
            if (fractionOfNitrogenLostByLeachingAndRunoff < 0.05)
            {
                return 0.05;
            }

            if (fractionOfNitrogenLostByLeachingAndRunoff > 0.3)
            {
                return 0.3;
            }

            return fractionOfNitrogenLostByLeachingAndRunoff;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Equation 2.5.1-1
        /// </summary>
        private double CalculateEmissionFactorUsingPrecipitation(double precipitation)
        {
            return Math.Exp((0.00558 * precipitation) - 7.7);
        }

        /// <summary>
        /// Equation 2.5.1-2
        /// </summary>
        private double CalculateEmissionFactorUsingPotentialEvapotranspiration(double potentialEvapotranspiration)
        {
            return Math.Exp((0.00558 * potentialEvapotranspiration) - 7.7);
        }

        #endregion
    }
}