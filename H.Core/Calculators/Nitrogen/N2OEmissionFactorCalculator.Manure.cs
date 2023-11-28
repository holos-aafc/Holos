using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Services.Animals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;
using H.Infrastructure;

namespace H.Core.Calculators.Nitrogen
{
    public partial class N2OEmissionFactorCalculator
    {
        #region Public Methods

        /// <summary>
        /// Includes emissions from both farm-sourced manure and imports. Emissions are for the entire field (not per hectare).
        /// </summary>
        public List<LandApplicationEmissionResult> CalculateIndirectEmissionsFromFieldAppliedManure(
            CropViewItem viewItem,
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResults,
            Farm farm)
        {
            // Initialize the manure service with the calculated emissions for the animal components
            this.ManureService.Initialize(farm, animalComponentEmissionsResults);

            var results = new List<LandApplicationEmissionResult>();

            foreach (var manureApplicationViewItem in viewItem.ManureApplicationViewItems)
            {
                var landApplicationEmissionResult = new LandApplicationEmissionResult();

                landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication = manureApplicationViewItem.AmountOfManureAppliedPerHectare * viewItem.Area;
                landApplicationEmissionResult.AmmoniacalLoss = this.CalculateNH3NLossFromLandAppliedManure(farm, viewItem, manureApplicationViewItem, true);
                landApplicationEmissionResult.AmmoniaLoss = CoreConstants.ConvertToNH3(landApplicationEmissionResult.AmmoniacalLoss);
                landApplicationEmissionResult.TotalN2ONFromManureVolatilized = this.CalculateTotalN2ONFromManureVolatilized(farm, manureApplicationViewItem, landApplicationEmissionResult.AmmoniacalLoss);
                landApplicationEmissionResult.TotalN2OFromManureVolatilized = CoreConstants.ConvertToN2O(landApplicationEmissionResult.TotalN2ONFromManureVolatilized);
                landApplicationEmissionResult.AdjustedAmmoniacalLoss = landApplicationEmissionResult.AmmoniacalLoss - landApplicationEmissionResult.TotalN2ONFromManureVolatilized;
                landApplicationEmissionResult.AdjustedAmmoniaLoss = CoreConstants.ConvertToNH3(landApplicationEmissionResult.AdjustedAmmoniacalLoss);
                landApplicationEmissionResult.TotalN2ONFromManureLeaching = this.CalculateTotalN2ONFromManureLeaching(farm, viewItem, manureApplicationViewItem);
                landApplicationEmissionResult.TotalNitrateLeached = this.CalculateTotalNitrateLeached(farm, viewItem, manureApplicationViewItem);
                landApplicationEmissionResult.TotalIndirectN2ONEmissions = landApplicationEmissionResult.TotalN2ONFromManureVolatilized + landApplicationEmissionResult.TotalN2ONFromManureLeaching;
                landApplicationEmissionResult.TotalIndirectN2OEmissions = CoreConstants.ConvertToN2O(landApplicationEmissionResult.TotalIndirectN2ONEmissions);

                results.Add(landApplicationEmissionResult);
            }

            return results;
        }

        public LandApplicationEmissionResult CalculateTotalIndirectEmissionsFromFieldSpecificManureApplications(
            CropViewItem viewItem,
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResults,
            Farm farm)
        {
            var indirectEmissionsForAllApplications = new List<LandApplicationEmissionResult>();

            // Calculate results for farm produced manure spreading
            var results = this.CalculateIndirectEmissionsFromFieldAppliedManure(viewItem, animalComponentEmissionsResults, farm);

            indirectEmissionsForAllApplications.AddRange(results);
            
            var resultsPerHectare = this.ConvertPerFieldEmissionsToPerHectare(indirectEmissionsForAllApplications, viewItem);

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
        /// Equation 4.6.2-20
        /// </summary>
        public double CalculateNH3NLossFromLandAppliedManure(
            Farm farm, 
            CropViewItem viewItem,
            ManureApplicationViewItem manureApplicationViewItem, 
            bool includeImports)
        {
            var result = 0d;

            if (manureApplicationViewItem.IsImportedManure()) 
            {
                if (includeImports)
                {
                    var landApplicationFactors = this.GetLandApplicationFactors(farm, manureApplicationViewItem);
                    var nitrogenUsed = manureApplicationViewItem.AmountOfManureAppliedPerHectare * viewItem.Area;

                    result = nitrogenUsed * landApplicationFactors.VolatilizationFraction;

                    return result;
                }
                else
                {
                    return 0;
                }
            }

            var animalType = manureApplicationViewItem.AnimalType;
            if (animalType.IsBeefCattleType() || animalType.IsDairyCattleType())
            {
                var adjustedAmmoniaEmissionFactor = this.GetAdjustedAmmoniaEmissionFactor(farm, viewItem, manureApplicationViewItem);
                var tanUsed = this.ManureService.GetAmountOfTanUsedDuringLandApplication(viewItem, manureApplicationViewItem);
                result = tanUsed * adjustedAmmoniaEmissionFactor;
            }
            else if (animalType.IsSheepType() || animalType.IsSwineType() || animalType.IsOtherAnimalType())
            {
                var landApplicationFactors = this.GetLandApplicationFactors(farm, manureApplicationViewItem);
                var nitrogenUsed = manureApplicationViewItem.AmountOfNitrogenAppliedPerHectare * viewItem.Area;

                // 4.6.2-12
                result = nitrogenUsed * landApplicationFactors.VolatilizationFraction;
            }
            else
            {
                var temperature = this.ClimateProvider.GetMeanTemperatureForDay(farm, manureApplicationViewItem.DateOfApplication);
                var volumeOfManureUsed = manureApplicationViewItem.AmountOfManureAppliedPerHectare * viewItem.Area;
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

                result = volumeOfManureUsed * emissionFraction;
            }

            return result;
        }

        public double CalculateNH3NLossFromExportedManure(Farm farm, 
            CropViewItem viewItem, 
            ManureApplicationViewItem manureApplicationViewItem, 
            bool includeImports)
        {
            var result = 0d;

            

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-1
        /// </summary>
        public double CalculateTotalN2ONFromManureVolatilized(
            Farm farm,
            ManureApplicationViewItem manureApplicationViewItem, 
            double ammoniacalLoss)
        {
            var landApplicationFactors = this.GetLandApplicationFactors(farm, manureApplicationViewItem);
            
            var result = ammoniacalLoss * landApplicationFactors.EmissionFactorVolatilization;

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-6
        /// </summary>
        public double CalculateTotalN2ONFromManureLeaching(Farm farm, CropViewItem viewItem, ManureApplicationViewItem manureApplicationViewItem)
        {
            var landApplicationFactors = this.GetLandApplicationFactors(farm, manureApplicationViewItem);
            var nitrogenUsed = manureApplicationViewItem.AmountOfManureAppliedPerHectare * viewItem.Area;
            var growingSeasonPrecipitation = this.ClimateProvider.GetGrowingSeasonPrecipitation(farm, manureApplicationViewItem.DateOfApplication.Year);
            var growingSeasonEvapotranspiration = this.ClimateProvider.GetGrowingSeasonEvapotranspiration(farm, manureApplicationViewItem.DateOfApplication.Year);
            var leachingFraction = this.CalculateLeachingFraction(growingSeasonPrecipitation, growingSeasonEvapotranspiration);

            var result = nitrogenUsed * leachingFraction * landApplicationFactors.EmissionFactorLeach;

            return result;
        }

        public double CalculateTotalNitrateLeached(Farm farm, CropViewItem viewItem, ManureApplicationViewItem manureApplicationViewItem)
        {
            var landApplicationFactors = this.GetLandApplicationFactors(farm, manureApplicationViewItem);
            var nitrogenUsed = manureApplicationViewItem.AmountOfManureAppliedPerHectare * viewItem.Area;
            var growingSeasonPrecipitation = this.ClimateProvider.GetGrowingSeasonPrecipitation(farm, manureApplicationViewItem.DateOfApplication.Year);
            var growingSeasonEvapotranspiration = this.ClimateProvider.GetGrowingSeasonEvapotranspiration(farm, manureApplicationViewItem.DateOfApplication.Year);
            var leachingFraction = this.CalculateLeachingFraction(growingSeasonPrecipitation, growingSeasonEvapotranspiration);

            var result = nitrogenUsed * leachingFraction * (1 - landApplicationFactors.EmissionFactorLeach);

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
        /// Equation 4.6.2-6
        /// </summary>
        public double CalculateTANRemaining(
            CropViewItem viewItem,
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResults,
            Farm farm)
        {
            var year = viewItem.Year;

            // Get total TAN created by all animals
            var totalTanCreated = this.ManureService.GetTotalTANCreated(
                year: year);

            // Get total TAN used 
            var viewItemsByYear = farm.GetCropDetailViewItemsByYear(year);
            var totalTANUsed = this.ManureService.GetTotalTanAppliedToAllFields(year, viewItemsByYear);

            var totalTANExported = this.ManureService.GetAmountOfTanExported(farm, year);

            var result = totalTanCreated - totalTANUsed - totalTANExported;

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-7
        /// </summary>
        public double CalculateAmmoniaFromLeftOverManureForFarm(
            List<CropViewItem> itemsByYear,
            Farm farm,
            double tanRemaining)
        {
            var weightedEmissionFactor = this.CalculateWeightedLandApplicationEmissionFactor(
                itemsByYear: itemsByYear,
                farm: farm);

            var result = tanRemaining * weightedEmissionFactor;

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-8
        /// </summary>
        public double CalculateAmmoniaFromLeftOverManureForField(
            List<CropViewItem> itemsByYear,
            Farm farm,
            CropViewItem viewItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Equation 4.6.2-9
        /// Equation 4.6.2-10
        /// </summary>
        public double CalculateAmountOfTANFromExportedManure(
            Farm farm,
            int year)
        {
            var totalTANExported = this.ManureService.GetAmountOfTanExported(farm, year);

            return totalTANExported;
        }

        /// <summary>
        /// Equation 4.6.2-11
        /// </summary>
        public double CalculateAmmoniaEmissionsFromExportedManureForYear(
            List<CropViewItem> itemsByYear,
            CropViewItem viewItem,
            Farm farm)
        {
            var year = viewItem.Year;

            var weightedEmissionFactor = this.CalculateWeightedLandApplicationEmissionFactor(
                itemsByYear: itemsByYear,
                farm: farm);

            var totalTANExported = this.ManureService.GetAmountOfTanExported(farm, year);

            var result = totalTANExported * weightedEmissionFactor;

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-12
        /// </summary>
        public double CalculateNH3NLossFromLandAppliedManure(
            Farm farm, 
            CropViewItem cropViewItem, 
            bool includeImports)
        {
            var result = 0d;

            foreach (var manureApplicationViewItem in cropViewItem.ManureApplicationViewItems)
            {
                result += this.CalculateNH3NLossFromLandAppliedManure(farm, cropViewItem, manureApplicationViewItem, includeImports);
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-12
        /// </summary>
        public double CalculateNH3NLossFromLandAppliedManureForFarm(
            Farm farm, 
            List<CropViewItem> itemsByYear,
            bool includeImports)
        {
            var result = 0d;

            foreach (var cropViewItem in itemsByYear)
            {
                result += CalculateNH3NLossFromLandAppliedManure(farm, cropViewItem, includeImports);
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-15
        /// </summary>
        public double CalculateAmmoniaEmissionsFromLeftOverManureForFarm(
            Farm farm,
            int year)
        {
            var total = 0d;

            var precipitation = this.ClimateProvider.GetAnnualPrecipitation(farm, year);
            var evapotranspiration = this.ClimateProvider.GetAnnualEvapotranspiration(farm, year);

            var animalManureTypes = this.ManureService.GetManureTypesProducedOnFarm(farm);
            foreach (var animalManureType in animalManureTypes)
            {
                var totalNitrogenRemaining = this.ManureService.GetTotalNitrogenRemaining(year, farm, animalManureType);
                var factors = this.LivestockEmissionConversionFactorsProvider.GetLandApplicationFactors(
                    farm: farm,
                    meanAnnualPrecipitation: precipitation,
                    meanAnnualEvapotranspiration: evapotranspiration,
                    animalType: animalManureType,
                    year: year);

                total += totalNitrogenRemaining * factors.VolatilizationFraction;
            }

            return total;
        }

        /// <summary>
        /// Equation 4.6.2-16
        /// </summary>
        public double CalculateAmmoniaEmissionsFromLeftOverManureForField(
            CropViewItem cropViewItem,
            Farm farm)
        {
            var result = 0d;

            var ammoniaEmissionsFromLeftOverManure = this.CalculateAmmoniaEmissionsFromLeftOverManureForFarm(farm, cropViewItem.Year);

            var areaOfFarm = farm.GetTotalAreaOfFarm(includeNativeGrasslands: false, cropViewItem.Year);
            var areaOfField = cropViewItem.Area;

            result = ammoniaEmissionsFromLeftOverManure * (areaOfField / areaOfFarm);

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-19
        /// </summary>
        public double CalculateAmmoniaEmissionsFromExportedManureForYear(
            Farm farm,
            int year)
        {
            var result = 0d;

            var exportTypes = this.ManureService.GetManureTypesExported(
                farm: farm,
                year: year);

            var precipitation = this.ClimateProvider.GetAnnualPrecipitation(farm, year);
            var evapotranspiration = this.ClimateProvider.GetAnnualEvapotranspiration(farm, year);

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
        /// Equation 4.6.2-20
        /// </summary>
        public double CalculateAmmoniaEmissionsFromImportedManureForField(
            Farm farm,
            CropViewItem cropViewItem,
            int year)
        {
            var result = 0d;

            var manureApplicationViewItems = cropViewItem.GetManureImportsByYear(year);
            var precipitation = this.ClimateProvider.GetAnnualPrecipitation(farm, year);
            var evapotranspiration = this.ClimateProvider.GetAnnualEvapotranspiration(farm, year);

            foreach (var manureImport in manureApplicationViewItems)
            {
                var totalNitrogen = manureImport.AmountOfNitrogenAppliedPerHectare * cropViewItem.Area;

                var landApplicationFactors = LivestockEmissionConversionFactorsProvider.GetLandApplicationFactors(
                    farm: farm,
                    meanAnnualPrecipitation: precipitation,
                    meanAnnualEvapotranspiration: evapotranspiration,
                    animalType: manureImport.AnimalType,
                    year: year);

                result += (totalNitrogen * landApplicationFactors.VolatilizationFraction);
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-20
        /// </summary>
        public double CalculateAmmoniaEmissionsFromImportedManureForFields(
            Farm farm,
            List<CropViewItem> itemsByYear,
            int year)
        {
            var result = 0d;

            foreach (var cropViewItem in itemsByYear)
            {
                result += CalculateAmmoniaEmissionsFromImportedManureForField(farm, cropViewItem, year);
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-21
        /// </summary>
        public double CalculateTotalAmmoniaEmissionsFromLandAppliedManure(
            Farm farm,
            CropViewItem cropViewItem,
            int year)
        {
            var ammoniaFromManureImports = this.CalculateAmmoniaEmissionsFromImportedManureForField(farm, cropViewItem, year);
            var ammoniaFromFarmSourceApplications = CalculateNH3NLossFromLandAppliedManure(farm, cropViewItem, includeImports: true);
            var ammoniaFromLeftOverManure = this.CalculateAmmoniaEmissionsFromLeftOverManureForField(cropViewItem, farm);

            var result = ammoniaFromManureImports * ammoniaFromFarmSourceApplications * ammoniaFromLeftOverManure;

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-21
        /// </summary>
        public double CalculateTotalAmmoniaEmissionsFromLandAppliedManureForYear(
            Farm farm,
            List<CropViewItem> itemsByYear,
            int year)
        {
            var result = 0d;

            foreach (var viewItem in itemsByYear)
            {
                result += CalculateTotalAmmoniaEmissionsFromLandAppliedManure(farm, viewItem, year);
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-22
        /// </summary>
        public double CalculateTotalAmmoniaEmissionsFromFarm(
            Farm farm,
            List<CropViewItem> itemsByYear,
            int year)
        {
            var result = 0d;

            var ammoniaEmissionsFromLandAppliedManure  = CalculateTotalAmmoniaEmissionsFromLandAppliedManureForYear(farm, itemsByYear, year);;
            var ammoniaEmissionsFromExports = this.CalculateAmmoniaEmissionsFromExportedManureForYear(farm, year);

            result = ammoniaEmissionsFromExports * ammoniaEmissionsFromLandAppliedManure;

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-3
        /// </summary>
        /// <returns></returns>
        public double CalculateAmmoniaEmissionsFromExportedManure(
            List<CropViewItem> itemsByYear,
            Farm farm)
        {
            var result = 0d;

            foreach (var farmManureExportViewItem in farm.ManureExportViewItems)
            {
                
            }

            return result;
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



        private double GetEmissionFactorForLandApplication(
            CropViewItem cropViewItem,
            ManureApplicationViewItem manureApplicationViewItem)
        {
            var tillageType = cropViewItem.TillageType;
            var manureStateType = manureApplicationViewItem.ManureStateType;
            var manureApplicationMethod = manureApplicationViewItem.ManureApplicationMethod;

            return !manureStateType.IsLiquidManure() ? this.AnimalAmmoniaEmissionFactorProvider.GetAmmoniaEmissionFactorForSolidAppliedManure(tillageType) : this.AnimalAmmoniaEmissionFactorProvider.GetAmmoniaEmissionFactorForLiquidAppliedManure(manureApplicationMethod);
        }

        private IEmissionData GetLandApplicationFactors(
            Farm farm,
            ManureApplicationViewItem manureApplicationViewItem)
        {
            var annualPrecipitation = this.ClimateProvider.GetAnnualPrecipitation(farm, manureApplicationViewItem.DateOfApplication);
            var evapotranspiration = this.ClimateProvider.GetAnnualEvapotranspiration(farm, manureApplicationViewItem.DateOfApplication);
            var animalType = manureApplicationViewItem.AnimalType;
            var year = manureApplicationViewItem.DateOfApplication.Year;

            var landApplicationFactors = this.LivestockEmissionConversionFactorsProvider.GetLandApplicationFactors(farm, annualPrecipitation, evapotranspiration, animalType, year);

            return landApplicationFactors;
        }

        private double GetAdjustedAmmoniaEmissionFactor(
            Farm farm, 
            CropViewItem cropViewItem,
            ManureApplicationViewItem manureApplicationViewItem)
        {
            var averageDailyTemperature = this.ClimateProvider.GetMeanTemperatureForDay(farm, manureApplicationViewItem.DateOfApplication);

            var adjustedAmmoniaEmissionFactor = CalculateAdjustedAmmoniaEmissionFactor(cropViewItem, manureApplicationViewItem, averageDailyTemperature);

            return adjustedAmmoniaEmissionFactor;
        }

        #endregion
    }
}
