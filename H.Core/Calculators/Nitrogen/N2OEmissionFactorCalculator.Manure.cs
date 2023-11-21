using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Services.Animals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Models.LandManagement.Fields;
using H.Infrastructure;

namespace H.Core.Calculators.Nitrogen
{
    public partial class N2OEmissionFactorCalculator
    {
        #region Public Methods

        public LandApplicationEmissionResult CalculateTotalIndirectEmissionsFromFieldSpecificManureApplications(
            CropViewItem viewItem,
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResults,
            Farm farm)
        {
            var indirectEmissionsForAllFields = new List<LandApplicationEmissionResult>();

            // Calculate results for farm produced manure spreading
            var farmProducedResults = this.CalculateIndirectEmissionsFromFieldAppliedManure(viewItem, animalComponentEmissionsResults, farm);
            var importedResults = this.CalculateAmmoniaFromLandApplicationForImportedManure(viewItem, farm);

            indirectEmissionsForAllFields.AddRange(farmProducedResults);
            indirectEmissionsForAllFields.AddRange(importedResults);

            var resultsPerHectare = this.ConvertPerFieldEmissionsToPerHectare(indirectEmissionsForAllFields, viewItem);

            return resultsPerHectare;
        }

        /// <summary>
        /// Combines total emissions for entire area of a field from each manure application into per hectare emissions
        /// </summary>
        /// <param name="results">The emissions for each field</param>
        /// <param name="viewItem">The <see cref="CropViewItem"/> that will be used to calculate per hectare emissions</param>
        /// <returns></returns>
        public LandApplicationEmissionResult ConvertPerFieldEmissionsToPerHectare(
            List<LandApplicationEmissionResult> results, 
            CropViewItem viewItem)
        {
            var totalAmountsPerHectareFromManureApplications = new LandApplicationEmissionResult();

            foreach (var landApplicationEmissionResult in results)
            {
                /*
                 * Totals are for the entire field. Convert to per hectare below.
                 */

                totalAmountsPerHectareFromManureApplications.TotalN2ONFromManureLeaching += landApplicationEmissionResult.TotalN2ONFromManureLeaching > 0
                    ? landApplicationEmissionResult.TotalN2ONFromManureLeaching / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.TotalIndirectN2ONEmissions += landApplicationEmissionResult.TotalIndirectN2ONEmissions > 0
                    ? landApplicationEmissionResult.TotalIndirectN2ONEmissions / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.TotalNitrateLeached += landApplicationEmissionResult.TotalNitrateLeached > 0
                    ? landApplicationEmissionResult.TotalNitrateLeached / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.TotalIndirectN2OEmissions += landApplicationEmissionResult.TotalIndirectN2OEmissions > 0
                    ? landApplicationEmissionResult.TotalIndirectN2OEmissions / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.TotalN2OFromManureVolatilized += landApplicationEmissionResult.TotalN2OFromManureVolatilized > 0
                    ? landApplicationEmissionResult.TotalN2OFromManureVolatilized / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.TotalN2ONFromManureVolatilized += landApplicationEmissionResult.TotalN2ONFromManureVolatilized > 0
                    ? landApplicationEmissionResult.TotalN2ONFromManureVolatilized / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.TotalVolumeOfManureUsedDuringApplication += landApplicationEmissionResult.TotalVolumeOfManureUsedDuringApplication > 0
                    ? landApplicationEmissionResult.TotalVolumeOfManureUsedDuringApplication / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.AmmoniacalLoss += landApplicationEmissionResult.AmmoniacalLoss > 0
                    ? landApplicationEmissionResult.AmmoniacalLoss / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.AdjustedAmmoniaLoss += landApplicationEmissionResult.AdjustedAmmoniaLoss > 0
                    ? landApplicationEmissionResult.AdjustedAmmoniaLoss / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.AdjustedAmmoniacalLoss += landApplicationEmissionResult.AdjustedAmmoniacalLoss > 0
                    ? landApplicationEmissionResult.AdjustedAmmoniacalLoss / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.AmmoniaLoss += landApplicationEmissionResult.AmmoniaLoss > 0
                    ? landApplicationEmissionResult.AmmoniaLoss / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.ActualAmountOfNitrogenAppliedFromLandApplication += landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication > 0
                    ? landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication / viewItem.Area
                    : 0;

                // Equation 4.6.2-4
                totalAmountsPerHectareFromManureApplications.TotalTANApplied += landApplicationEmissionResult.TotalTANApplied > 0
                    ? landApplicationEmissionResult.TotalTANApplied / viewItem.Area
                    : 0;
            }

            return totalAmountsPerHectareFromManureApplications;
        }

        /// <summary>
        /// Equation 4.6.1-1
        /// 
        /// Calculates direct emissions from the manure specifically applied to the field
        ///
        /// (kg N2O-N (kg N)^-1)
        /// </summary>
        public double CalculateDirectN2ONEmissionsFromFieldSpecificManureSpreading(
            CropViewItem viewItem,
            Farm farm)
        {
            var fieldSpecificOrganicNitrogenEmissionFactor = this.CalculateOrganicNitrogenEmissionFactor(
                viewItem: viewItem,
                farm: farm);

            var totalLocalAndImportedNitrogenApplied = this.GetTotalManureNitrogenAppliedFromLivestockAndImportsInYear(viewItem);

            var result = totalLocalAndImportedNitrogenApplied * fieldSpecificOrganicNitrogenEmissionFactor;

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-2
        /// 
        /// (kg N)
        /// </summary>
        public double GetTotalManureNitrogenAppliedFromLivestockAndImportsInYear(CropViewItem viewItem)
        {
            var totalNitrogen = 0d;

            foreach (var manureApplication in viewItem.ManureApplicationViewItems.Where(manureViewItem => manureViewItem.DateOfApplication.Year == viewItem.Year))
            {
                totalNitrogen += manureApplication.AmountOfNitrogenAppliedPerHectare * viewItem.Area;
            }

            return totalNitrogen;
        }

        /// <summary>
        /// Equation 4.6.1-3
        ///
        /// There can be multiple fields on a farm and the emission factor calculations are field-dependent (accounts for crop type, fertilizer, etc.). So
        /// we take the weighted average of these fields when calculating the EF for organic nitrogen (ON). This is to be used when calculating direct emissions
        /// from land applied manure. Native rangeland is not included.
        /// </summary>
        public double CalculateWeightedOrganicNitrogenEmissionFactor(
            List<CropViewItem> itemsByYear,
            Farm farm)
        {
            var fieldAreasAndEmissionFactors = new List<WeightedAverageInput>();
            var filteredItems = itemsByYear.Where(x => x.IsNativeGrassland == false);

            foreach (var cropViewItem in filteredItems)
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
        ///
        /// (kg N)
        /// </summary>
        public double CalculateTotalManureNitrogenRemaining(
            double totalManureNitrogenAvailableForLandApplication,
            double totalManureNitrogenAlreadyAppliedToFields,
            double totalManureNitrogenExported,
            double totalImportedNitrogen)
        {
            var result = totalManureNitrogenAvailableForLandApplication - (totalManureNitrogenAlreadyAppliedToFields - totalImportedNitrogen) - totalManureNitrogenExported;
            if (result < 0)
            {
                // Can't have a negative value of manure remaining
                result = 0;
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-5
        ///
        /// Calculates the total N2O-N from the remaining manure N not applied to any field
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalDirectN2ONFromRemainingManureNitrogen(
            double weightedEmissionFactor,
            double totalManureNitrogenRemaining)
        {
            var result = totalManureNitrogenRemaining * weightedEmissionFactor;

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-6
        /// </summary>
        public double CalculateDirectN2ONFromLeftOverManure(
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResults,
            Farm farm,
            CropViewItem viewItem,
            double weightedEmissionFactor)
        {
            // The total N after all applications and exports have been subtracted
            var totalNitrogenRemaining = this.CalculateTotalManureNitrogenRemaining(
                animalComponentEmissionsResults,
                farm,
                viewItem);

            var emissionsFromNitrogenRemaining = this.CalculateTotalDirectN2ONFromRemainingManureNitrogen(
                weightedEmissionFactor: weightedEmissionFactor,
                totalManureNitrogenRemaining: totalNitrogenRemaining);

            var totalAreaOfAllFields = farm.GetTotalAreaOfFarm(false, viewItem.Year);
            if (totalAreaOfAllFields == 0)
            {
                totalAreaOfAllFields = 1;
            }

            var areaOfThisField = viewItem.Area;

            // The total N2O-N that is left over and must be associated with this field so that all manure is applied to the fields in the same year (nothing is remaining to be applied)
            var result = emissionsFromNitrogenRemaining * (areaOfThisField / totalAreaOfAllFields);

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-9
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalDirectN2ONFromExportedManure(Farm farm, int year)
        {
            var viewItemsByYear = farm.GetCropDetailViewItemsByYear(year);

            var weightedEmissionFactor = this.CalculateWeightedOrganicNitrogenEmissionFactor(viewItemsByYear, farm);
            var totalExportedManureNitrogen = this.ManureService.GetTotalNitrogenFromExportedManure(year, farm);

            var emissions = this.CalculateTotalDirectN2ONFromExportedManure(totalExportedManureNitrogen, weightedEmissionFactor);

            return emissions;
        }

        /// <summary>
        /// Equation 4.6.1-9
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalDirectN2ONFromExportedManure(
            double totalExportedManureNitrogen,
            double weightedEmissionFactor)
        {
            var result = totalExportedManureNitrogen * weightedEmissionFactor;

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-2
        /// </summary>
        /// <param name="emissionFactorForLandApplication">Default NH3 emission factor for land application</param>
        /// <param name="ambientTemperatureAdjustment">Ambient temperature based adjustment</param>
        public double CalculateAdjustedAmmoniaEmissionFactor(
            double emissionFactorForLandApplication,
            double ambientTemperatureAdjustment)
        {
            return emissionFactorForLandApplication * ambientTemperatureAdjustment;
        }

        /// <summary>
        /// Equation 4.6.2-2
        /// </summary>
        public double CalculateAdjustedAmmoniaEmissionFactor(
            CropViewItem viewItem, 
            ManureApplicationViewItem manureApplication, 
            double averageDailyTemperature)
        {
            var emissionFactorForLandApplication = GetEmissionFactorForLandApplication(viewItem, manureApplication);
            var adjustedEmissionFactor = CalculateAmbientTemperatureAdjustmentForLandApplication(averageDailyTemperature);
            var adjustedAmmoniaEmissionFactor = CalculateAdjustedAmmoniaEmissionFactor(emissionFactorForLandApplication, adjustedEmissionFactor);

            return adjustedAmmoniaEmissionFactor;
        }

        /// <summary>
        /// Equation 4.6.2-3
        /// Equation 4.6.2-12
        /// </summary>
        public double CalculateNH3NLossFromLandAppliedManure(
            double fractionOfManure,
            double totalTANProducedByAllAnimalsInCategory,
            double adjustedAmmoniaEmissionFactor,
            double totalNitrogenProducedByAllAnimalsInCategory,
            double volatilizationFractionForLandApplication,
            double temperature,
            double totalManureProducedByAnimalsInCategory,
            AnimalType animalType)
        {
            var result = 0d;
            if (animalType.IsBeefCattleType() || animalType.IsDairyCattleType())
            {
                // Equation 4.6.2-3
                result = fractionOfManure * totalTANProducedByAllAnimalsInCategory * adjustedAmmoniaEmissionFactor;
            }
            else if (animalType.IsSheepType() || animalType.IsSwineType() || animalType.IsOtherAnimalType())
            {
                // Equation 4.6.2-12
                result = fractionOfManure * totalNitrogenProducedByAllAnimalsInCategory * volatilizationFractionForLandApplication;
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

                result = fractionOfManure * totalManureProducedByAnimalsInCategory * emissionFraction;
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-5
        ///
        /// (kg NH3-N (kg N)^-1)
        /// </summary>
        public double CalculateWeightedLandApplicationEmissionFactor(
            List<CropViewItem> itemsByYear,
            Farm farm)
        {
            var fieldAreasAndEmissionFactors = new List<WeightedAverageInput>();
            var filteredItems = itemsByYear.Where(x => x.IsNativeGrassland == false);

            foreach (var cropViewItem in filteredItems)
            {
                // Each field can have multiple manure applications, to simplify emission factor calculation take first application if there is more than
                // one. Alternative is to take average of emission factors calculated for each manure application
                var manureApplication = cropViewItem.ManureApplicationViewItems.FirstOrDefault();
                if (manureApplication == null)
                {
                    continue;
                }

                var averageDailyTemperature = farm.ClimateData.GetMeanTemperatureForDay(manureApplication.DateOfApplication);
                var adjustedAmmoniaEmissionFactor = this.CalculateAdjustedAmmoniaEmissionFactor(cropViewItem, manureApplication, averageDailyTemperature);
                fieldAreasAndEmissionFactors.Add(new WeightedAverageInput()
                {
                    Value = adjustedAmmoniaEmissionFactor,
                    Weight = cropViewItem.Area,
                });
            }

            var weightedEmissionFactor = this.CalculateWeightedEmissionFactor(fieldAreasAndEmissionFactors);

            return weightedEmissionFactor;
        }

        /// <summary>
        /// Equation 4.6.2-9
        /// Equation 4.6.2-10
        /// </summary>
        public double CalculateAmountOfTANFromExportedManure(
            Farm farm,
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResults,
            int year)
        {
            var result = 0d;

            this.ManureService.Initialize(farm, animalComponentEmissionsResults);

            var totalTANCreated = this.ManureService.GetTotalTANCreated(year);
            var totalVolumeCreated = this.ManureService.GetTotalVolumeCreated(year);
            var totalVolumeExported = this.ManureService.GetTotalVolumeOfManureExported(year, farm);

            // Note volume is already converted so division by 1000 is not performed here
            result = totalTANCreated + (totalVolumeExported / totalVolumeCreated);

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-11
        /// </summary>
        public double AmmoniaEmissionsFromExportedManure(
            double totalTANExported, 
            double weightEmissionFactor)
        {
            return totalTANExported * weightEmissionFactor;
        }

        /// <summary>
        /// Equation 4.6.2-15
        /// </summary>
        public double CalculateAmmoniaEmissionsFromLeftOverManure(
            double nitrogenRemaining,
            double volatilizationFractionForLandApplication)
        {
            var result = nitrogenRemaining * volatilizationFractionForLandApplication;

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-16
        /// </summary>
        public double CalculateAmmoniaEmissionsFromLeftOverManureForField(
            double ammoniaEmissionsFromLeftOverManure,
            CropViewItem cropViewItem,
            Farm farm)
        {
            var result = 0d;

            var areaOfFarm = farm.GetTotalAreaOfFarm(includeNativeGrasslands: false, cropViewItem.Year);
            var areaOfField = cropViewItem.Area;

            result = ammoniaEmissionsFromLeftOverManure * (areaOfField / areaOfFarm);

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-19
        /// </summary>
        public double CalculateAmmoniaEmissionsFromExportedManure(
            Farm farm,
            int year)
        {
            var result = 0d;

            var exportTypes = this.ManureService.GetManureTypesExported(
                farm: farm,
                year: year);

            var precipitation = farm.GetAnnualPrecipitation(year);
            var evapotranspiration = farm.GetAnnualEvapotranspiration(year);

            foreach (var exportType in exportTypes)
            {
                var totalNitrogenExported = this.ManureService.GetTotalNitrogenFromExportedManure(
                    year: year,
                    farm: farm,
                    animalType: exportType);

                var landApplicationFactors = LivestockEmissionConversionFactorsProvider.GetLandApplicationFactors(
                    farm: farm,
                    meanAnnualPrecipitation: precipitation,
                    meanAnnualEvapotranspiration: evapotranspiration,
                    animalType: exportType,
                    year: year);

                result += (totalNitrogenExported * landApplicationFactors.VolatilizationFraction);
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-19
        /// </summary>
        public double CalculateAmmoniaEmissionsFromImportedManure(
            Farm farm,
            int year)
        {
            var result = 0d;

            var typesImported = this.ManureService.GetManureTypesImported(
                farm: farm,
                year: year);

            var precipitation = farm.GetAnnualPrecipitation(year);
            var evapotranspiration = farm.GetAnnualEvapotranspiration(year);

            foreach (var animalType in typesImported)
            {
                var nitrogenFromManureImports = this.ManureService.GetTotalNitrogenFromManureImports(
                    year: year,
                    farm: farm,
                    animalType: animalType);

                var landApplicationFactors = LivestockEmissionConversionFactorsProvider.GetLandApplicationFactors(
                    farm: farm,
                    meanAnnualPrecipitation: precipitation,
                    meanAnnualEvapotranspiration: evapotranspiration,
                    animalType: animalType,
                    year: year);

                result += (nitrogenFromManureImports * landApplicationFactors.VolatilizationFraction);
            }

            return result;
        }

        /// <summary>
        /// Calculates indirect emissions from land applied manure for the different categories of manure applied to the fields (beef cattle manure, dairy cattle manure, etc.).
        /// </summary>
        /// <param name="viewItem">The <see cref="CropViewItem"/> containing the manure applications</param>
        /// <param name="animalComponentEmissionsResults">The animal component emissions from all animal components on the farm</param>
        /// <param name="farm">The farm containing the <see cref="FieldSystemComponent"/>s and associated <see cref="ManureApplicationViewItem"/>s</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="LandApplicationEmissionResult"/> where each result is the associated emissions from one field application of manure</returns>
        public List<LandApplicationEmissionResult> CalculateIndirectEmissionsFromFieldAppliedManure(
            CropViewItem viewItem,
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResults,
            Farm farm)
        {
            // Initialize the manure service with the calculated emissions for the animal components
            this.ManureService.Initialize(farm, animalComponentEmissionsResults);

            var results = new List<LandApplicationEmissionResult>();
            foreach (var animalComponentEmissionsResult in animalComponentEmissionsResults)
            {
                var componentCategory = animalComponentEmissionsResult.Component.ComponentCategory;
                var animalType = componentCategory.GetAnimalTypeFromComponentCategory();
                var animalCategory = animalType.GetComponentCategoryFromAnimalType();

                var totalManureProducedByAnimalCategory = this.ManureService.GetTotalVolumeCreated(
                    year: viewItem.Year,
                    animalType: animalType);

                var totalTANCreatedByAnimalCategory = this.ManureService.GetTotalTANCreated(
                    year: viewItem.Year,
                    animalType: animalType);

                var totalNitrogenCreatedByAnimalCategory = this.ManureService.GetTotalNitrogenCreated(
                    year: viewItem.Year,
                    animalType: animalType);

                // Get all manure applications that have the same manure type as the animal emission results being passed in
                var manureApplicationsOfSameCategory = viewItem.ManureApplicationViewItems.Where(x => x.IsImportedManure() == false && x.AnimalType.GetComponentCategoryFromAnimalType() == animalCategory);
                foreach (var manureApplicationViewItem in manureApplicationsOfSameCategory)
                {
                    var landApplicationEmissionResult = new LandApplicationEmissionResult();

                    var averageDailyTemperature = this.ClimateProvider.GetMeanTemperatureForDay(farm, manureApplicationViewItem.DateOfApplication);
                    var annualPrecipitation = this.ClimateProvider.GetAnnualPrecipitation(farm, manureApplicationViewItem.DateOfApplication);
                    var evapotranspiration = this.ClimateProvider.GetAnnualEvapotranspiration(farm, manureApplicationViewItem.DateOfApplication);
                    var growingSeasonPrecipitation = this.ClimateProvider.GetGrowingSeasonPrecipitation(farm, manureApplicationViewItem.DateOfApplication);
                    var growingSeasonEvapotranspiration = this.ClimateProvider.GetGrowingSeasonEvapotranspiration(farm, manureApplicationViewItem.DateOfApplication);
                    var landApplicationFactors = this.LivestockEmissionConversionFactorsProvider.GetLandApplicationFactors(farm, annualPrecipitation, evapotranspiration, animalType, viewItem.Year);
                    var leachingFraction = CalculateLeachingFraction(growingSeasonPrecipitation, growingSeasonEvapotranspiration);
                    var volatilizationFraction = landApplicationFactors.VolatilizationFraction;
                    var adjustedAmmoniaEmissionFactor = CalculateAdjustedAmmoniaEmissionFactor(viewItem, manureApplicationViewItem, averageDailyTemperature);

                    var fractionOfManureUsed = (manureApplicationViewItem.AmountOfManureAppliedPerHectare * viewItem.Area) / totalManureProducedByAnimalCategory;
                    if (fractionOfManureUsed > 1.0)
                        fractionOfManureUsed = 1.0;

                    landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication = totalNitrogenCreatedByAnimalCategory * fractionOfManureUsed;

                    landApplicationEmissionResult.AmmoniacalLoss = this.CalculateNH3NLossFromLandAppliedManure(
                        fractionOfManure: fractionOfManureUsed,
                        totalTANProducedByAllAnimalsInCategory: totalTANCreatedByAnimalCategory,
                        adjustedAmmoniaEmissionFactor: adjustedAmmoniaEmissionFactor,
                        totalNitrogenProducedByAllAnimalsInCategory: totalNitrogenCreatedByAnimalCategory,
                        volatilizationFractionForLandApplication: volatilizationFraction,
                        temperature: averageDailyTemperature,
                        totalManureProducedByAnimalsInCategory: totalManureProducedByAnimalCategory,
                        animalType: animalType);

                    landApplicationEmissionResult.AmmoniaLoss = CoreConstants.ConvertToNH3(landApplicationEmissionResult.AmmoniacalLoss);

                    landApplicationEmissionResult.TotalN2ONFromManureVolatilized = landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * volatilizationFraction * landApplicationFactors.EmissionFactorVolatilization;
                    landApplicationEmissionResult.TotalN2OFromManureVolatilized = CoreConstants.ConvertToN2O(landApplicationEmissionResult.TotalN2ONFromManureVolatilized);

                    landApplicationEmissionResult.AdjustedAmmoniacalLoss = landApplicationEmissionResult.AmmoniacalLoss - landApplicationEmissionResult.TotalN2ONFromManureVolatilized;
                    landApplicationEmissionResult.AdjustedAmmoniaLoss = CoreConstants.ConvertToNH3(landApplicationEmissionResult.AdjustedAmmoniacalLoss);

                    landApplicationEmissionResult.TotalN2ONFromManureLeaching = landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * leachingFraction * landApplicationFactors.EmissionFactorLeach;
                    landApplicationEmissionResult.TotalNitrateLeached = landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * leachingFraction * (1.0 - landApplicationFactors.EmissionFactorLeach);

                    landApplicationEmissionResult.TotalIndirectN2ONEmissions = landApplicationEmissionResult.TotalN2ONFromManureVolatilized + landApplicationEmissionResult.TotalN2ONFromManureLeaching;
                    landApplicationEmissionResult.TotalIndirectN2OEmissions = CoreConstants.ConvertToN2O(landApplicationEmissionResult.TotalIndirectN2ONEmissions);

                    results.Add(landApplicationEmissionResult);
                }
            }

            return results;
        }

        /// <summary>
        /// Equation 4.6.4-2
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateLeachingEmissionsFromLeftOverManure(
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResults,
            Farm farm,
            CropViewItem viewItem,
            double leachingFraction,
            double emissionFactorForLeaching)
        {
            var remainingManureNitrogen = this.CalculateTotalManureNitrogenRemaining(animalComponentEmissionsResults, farm, viewItem);

            return remainingManureNitrogen * leachingFraction * emissionFactorForLeaching;
        }

        /// <summary>
        /// Equation 4.6.4-2
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalManureNitrogenRemaining(
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResults,
            Farm farm,
            CropViewItem viewItem)
        {
            // Get all fields that exist in the same year but (excluding native rangeland since manure is not spread on those fields)
            var itemsInYear = farm.GetCropDetailViewItemsByYear(viewItem.Year).Where(x => x.IsNativeGrassland == false);

            // This is the total amount of N from all animals that is available for land application
            var totalNitrogenAvailableForLandApplication = animalComponentEmissionsResults.TotalNitrogenAvailableForLandApplication();

            // This is the total amount of N that the user has specified is applied to all fields
            var totalNitrogenFromApplications = 0d;
            foreach (var cropViewItem in itemsInYear)
            {
                totalNitrogenFromApplications += this.GetTotalManureNitrogenAppliedFromLivestockAndImportsInYear(cropViewItem);
            }

            var totalNitrogenFromImportedManure = itemsInYear.Sum(x => x.GetTotalNitrogenFromImportedManure());

            var exportedNitrogen = this.ManureService.GetTotalNitrogenFromExportedManure(viewItem.Year, farm);

            // The total N after all applications and exports have been subtracted
            var totalNitrogenRemaining = this.CalculateTotalManureNitrogenRemaining(
                totalManureNitrogenAvailableForLandApplication: totalNitrogenAvailableForLandApplication,
                totalManureNitrogenAlreadyAppliedToFields: totalNitrogenFromApplications,
                totalManureNitrogenExported: exportedNitrogen,
                totalImportedNitrogen: totalNitrogenFromImportedManure);

            return totalNitrogenRemaining;
        }

        /// <summary>
        /// Equation 4.6.4-3
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateLeachingEmissionsFromExportedManure(
            Farm farm,
            int year,
            double leachingFraction,
            double emissionFactorForLeaching)
        {
            var exportedNitrogen = this.ManureService.GetTotalNitrogenFromExportedManure(year, farm);

            return exportedNitrogen * leachingFraction * emissionFactorForLeaching;
        }

        #endregion

        #region Private Methods

        public List<LandApplicationEmissionResult> CalculateAmmoniaFromLandApplicationForImportedManure(
            CropViewItem viewItem, 
            Farm farm)
        {
            var results = new List<LandApplicationEmissionResult>();

            var annualPrecipitation = this.ClimateProvider.GetAnnualPrecipitation(farm, viewItem.Year);
            var annualEvapotranspiration = this.ClimateProvider.GetAnnualEvapotranspiration(farm, viewItem.Year);
            var growingSeasonPrecipitation = this.ClimateProvider.GetGrowingSeasonPrecipitation(farm, viewItem.Year);
            var growingSeasonEvapotranspiration = this.ClimateProvider.GetGrowingSeasonEvapotranspiration(farm, viewItem.Year);

            var leachingFraction = this.CalculateLeachingFraction(growingSeasonPrecipitation, growingSeasonEvapotranspiration);

            var importedManureApplications = viewItem.ManureApplicationViewItems.Where(x => x.IsImportedManure());
            foreach (var importedManureApplication in importedManureApplications)
            {
                var result = new LandApplicationEmissionResult();

                var landApplicationFactors = this.LivestockEmissionConversionFactorsProvider.GetLandApplicationFactors(
                    farm: farm, 
                    meanAnnualPrecipitation: annualPrecipitation, 
                    meanAnnualEvapotranspiration: annualEvapotranspiration, 
                    animalType: importedManureApplication.AnimalType, 
                    year: viewItem.Year);

                result.ActualAmountOfNitrogenAppliedFromLandApplication = importedManureApplication.AmountOfManureAppliedPerHectare * viewItem.Area;
                
                // 4.6.2-20
                result.AmmoniacalLoss = result.ActualAmountOfNitrogenAppliedFromLandApplication * landApplicationFactors.VolatilizationFraction;
                result.AmmoniaLoss = CoreConstants.ConvertToNH3(result.AmmoniacalLoss);

                result.TotalN2ONFromManureVolatilized = result.ActualAmountOfNitrogenAppliedFromLandApplication * landApplicationFactors.VolatilizationFraction * landApplicationFactors.EmissionFactorVolatilization;
                result.TotalN2OFromManureVolatilized = CoreConstants.ConvertToN2O(result.TotalN2ONFromManureVolatilized);

                result.AdjustedAmmoniacalLoss = result.AmmoniacalLoss - result.TotalN2ONFromManureVolatilized;
                result.AdjustedAmmoniaLoss = CoreConstants.ConvertToNH3(result.AdjustedAmmoniacalLoss);

                result.TotalN2ONFromManureLeaching = result.ActualAmountOfNitrogenAppliedFromLandApplication * leachingFraction * landApplicationFactors.EmissionFactorLeach;
                result.TotalNitrateLeached = result.ActualAmountOfNitrogenAppliedFromLandApplication * leachingFraction * (1.0 - landApplicationFactors.EmissionFactorLeach);

                result.TotalIndirectN2ONEmissions = result.TotalN2ONFromManureVolatilized + result.TotalN2ONFromManureLeaching;
                result.TotalIndirectN2OEmissions = CoreConstants.ConvertToN2O(result.TotalIndirectN2ONEmissions);

                results.Add(result);
            }

            return results;
        }

        private double GetEmissionFactorForLandApplication(
            CropViewItem cropViewItem,
            ManureApplicationViewItem manureApplicationViewItem)
        {
            var tillageType = cropViewItem.TillageType;
            var manureStateType = manureApplicationViewItem.ManureStateType;
            var manureApplicationMethod = manureApplicationViewItem.ManureApplicationMethod;

            return !manureStateType.IsLiquidManure() ? this.AnimalAmmoniaEmissionFactorProvider.GetAmmoniaEmissionFactorForSolidAppliedManure(tillageType) : this.AnimalAmmoniaEmissionFactorProvider.GetAmmoniaEmissionFactorForLiquidAppliedManure(manureApplicationMethod);
        }

        #endregion
    }
}
