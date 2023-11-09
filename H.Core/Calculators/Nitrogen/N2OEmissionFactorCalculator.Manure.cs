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
            double totalTANProduced,
            double adjustedAmmoniaEmissionFactor,
            double totalNitrogenProduced,
            double volatilizationFractionForLandApplication,
            double temperature,
            double totalManureProducedByAnimals,
            AnimalType animalType)
        {
            var result = 0d;
            if (animalType.IsBeefCattleType() || animalType.IsDairyCattleType())
            {
                // Equation 4.6.2-3
                result = fractionOfManure * totalTANProduced * adjustedAmmoniaEmissionFactor;
            }
            else if (animalType.IsSheepType() || animalType.IsSwineType() || animalType.IsOtherAnimalType())
            {
                // Equation 4.6.2-12
                result = fractionOfManure * totalNitrogenProduced * volatilizationFractionForLandApplication;
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

                result = fractionOfManure * totalManureProducedByAnimals * emissionFraction;
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
            var totalExportedManureNitrogen = _manureService.GetTotalNitrogenFromExportedManure(year, farm);

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
        /// Equation 4.6.2-9
        /// Equation 4.6.2-10
        /// </summary>
        public double CalculateAmountOfTANFromExportedManure(
            Farm farm,
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResults,
            int year)
        {
            var result = 0d;

            _manureService.Initialize(farm, animalComponentEmissionsResults);

            var totalTANCreated = _manureService.GetTotalTANCreated(year);
            var totalVolumeCreated = _manureService.GetTotalVolumeCreated(year);
            var totalVolumeExported = _manureService.GetTotalVolumeOfManureExported(year, farm);

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
        /// Calculate total indirect emissions from all land applied manure to the crop. All calculations are per hectare.
        /// </summary>
        /// <returns>Total indirect emissions (ha^-1)</returns>
        public LandApplicationEmissionResult CalculateTotalIndirectEmissionsFromFieldSpecificManureApplications(
            CropViewItem viewItem,
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResults,
            Farm farm)
        {
            var totalAmountsPerHectareFromManureApplications = new LandApplicationEmissionResult();
            var indirectEmissionsForAllFields = new List<LandApplicationEmissionResult>();

            // Calculate results for on farm produced manure spreading
            foreach (var animalComponentEmissionsResult in animalComponentEmissionsResults)
            {
                // Each item in the list will be total amounts per field
                indirectEmissionsForAllFields.AddRange(this.CalculateAmmoniaEmissionsFromLandAppliedManure(farm, animalComponentEmissionsResult));
            }

            // Calculate results for imported manure
            var indirectEmissionsFromImportedManureSpreading = this.CalculateAmmoniaFromLandApplicationForImportedManure(viewItem, farm);

            // This will be a list of all indirect emissions for land applied manure for each year of history for this field
            var indirectEmissionsForField = indirectEmissionsForAllFields.Where(x => x.CropViewItem.FieldSystemComponentGuid.Equals(viewItem.FieldSystemComponentGuid));

            // Filter by year
            var byYear = indirectEmissionsForField.Where(x => x.CropViewItem.Year.Equals(viewItem.Year));

            var combinedResults = byYear.Concat(indirectEmissionsFromImportedManureSpreading);

            foreach (var landApplicationEmissionResult in combinedResults)
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

                totalAmountsPerHectareFromManureApplications.TotalN2ONFromManureVolatilized += landApplicationEmissionResult.TotalN2ONFromManureVolatilized > 0
                    ? landApplicationEmissionResult.TotalN2ONFromManureVolatilized / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.TotalVolumeOfManureUsedDuringApplication += landApplicationEmissionResult.TotalVolumeOfManureUsedDuringApplication > 0
                    ? landApplicationEmissionResult.TotalVolumeOfManureUsedDuringApplication / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.AmmoniacalLoss += landApplicationEmissionResult.AmmoniacalLoss > 0
                    ? landApplicationEmissionResult.AmmoniacalLoss / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.AdjustedAmmoniacalLoss += landApplicationEmissionResult.AdjustedAmmoniacalLoss > 0
                    ? landApplicationEmissionResult.AdjustedAmmoniacalLoss / viewItem.Area
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

            var exportedNitrogen = _manureService.GetTotalNitrogenFromExportedManure(viewItem.Year, farm);

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
            var exportedNitrogen = _manureService.GetTotalNitrogenFromExportedManure(year, farm);

            return exportedNitrogen * leachingFraction * emissionFactorForLeaching;
        }

        #endregion

        #region Private Methods

        private List<LandApplicationEmissionResult> CalculateAmmoniaFromLandApplicationForImportedManure(CropViewItem viewItem, Farm farm)
        {
            var results = new List<LandApplicationEmissionResult>();

            var annualPrecipitation = farm.ClimateData.PrecipitationData.GetTotalAnnualPrecipitation();
            var growingSeasonPrecipitation = farm.ClimateData.PrecipitationData.GrowingSeasonPrecipitation;

            var evapotranspiration = farm.ClimateData.EvapotranspirationData.GetTotalAnnualEvapotranspiration();
            var growingSeasonEvapotranspiration = farm.ClimateData.EvapotranspirationData.GrowingSeasonEvapotranspiration;

            var leachingFraction = CalculateLeachingFraction(growingSeasonPrecipitation, growingSeasonEvapotranspiration);

            var applications = viewItem.ManureApplicationViewItems.Where(x => x.IsImportedManure());
            foreach (var manureApplicationViewItem in applications)
            {
                var landApplicationFactors = _livestockEmissionConversionFactorsProvider.GetLandApplicationFactors(farm, annualPrecipitation, evapotranspiration, manureApplicationViewItem.AnimalType, viewItem.Year);

                var amountOfN = manureApplicationViewItem.AmountOfNitrogenAppliedPerHectare * viewItem.Area;

                // Equation 4.6.2-18
                var ammoniacalLoss = amountOfN * landApplicationFactors.VolatilizationFraction;

                // Equation 4.6.3-2
                var volatilizedManure = amountOfN * landApplicationFactors.VolatilizationFraction * landApplicationFactors.EmissionFactorVolatilization;

                // Equation 4.6.3-4
                var adjustedLoss = ammoniacalLoss - volatilizedManure;

                // Equation 4.6.4-1
                var n2OFromManureLeaching = amountOfN * leachingFraction * landApplicationFactors.EmissionFactorLeach;

                var no3Leached = amountOfN * leachingFraction * (1 - landApplicationFactors.EmissionFactorLeach);

                // Equation 4.6.5-1
                var totalIndirectN2ONEmissions = volatilizedManure + n2OFromManureLeaching;

                results.Add(new LandApplicationEmissionResult()
                {
                    AmmoniacalLoss = ammoniacalLoss,
                    AdjustedAmmoniacalLoss = adjustedLoss,
                    TotalN2ONFromManureLeaching = n2OFromManureLeaching,
                    TotalNitrateLeached = no3Leached,
                    TotalIndirectN2ONEmissions = totalIndirectN2ONEmissions,
                });
            }

            return results;
        }

        private List<LandApplicationEmissionResult> CalculateAmmoniaEmissionsFromLandAppliedManure(
            Farm farm,
            AnimalComponentEmissionsResults animalComponentEmissionsResults)
        {
            var componentCategory = animalComponentEmissionsResults.Component.ComponentCategory;
            var growingSeasonPrecipitation = farm.ClimateData.PrecipitationData.GrowingSeasonPrecipitation;
            var growingSeasonEvapotranspiration = farm.ClimateData.EvapotranspirationData.GrowingSeasonEvapotranspiration;
            var animalType = componentCategory.GetAnimalTypeFromComponentCategory();
            var totalManureProducedByAnimals = animalComponentEmissionsResults.TotalVolumeOfManureAvailableForLandApplication * 1000;
            var totalTANProduced = animalComponentEmissionsResults.TotalTANAvailableForLandApplication;
            var totalNitrogenProduced = animalComponentEmissionsResults.TotalOrganicNitrogenAvailableForLandApplication;
            var applicationsAndCropByAnimalType = farm.GetManureApplicationsAndAssociatedCropByAnimalType(animalType);
            var results = new List<LandApplicationEmissionResult>();
            var annualPrecipitation = farm.ClimateData.PrecipitationData.GetTotalAnnualPrecipitation();
            var evapotranspiration = farm.ClimateData.EvapotranspirationData.GetTotalAnnualEvapotranspiration();

            foreach (var tuple in applicationsAndCropByAnimalType)
            {
                var landApplicationEmissionResult = new LandApplicationEmissionResult();

                var viewItem = tuple.Item1;
                landApplicationEmissionResult.CropViewItem = viewItem;

                var manureApplication = tuple.Item2;

                var landApplicationFactors = _livestockEmissionConversionFactorsProvider.GetLandApplicationFactors(farm, annualPrecipitation, evapotranspiration, manureApplication.AnimalType, viewItem.Year);

                var dateOfApplication = manureApplication.DateOfApplication;
                var averageDailyTemperature = farm.ClimateData.GetMeanTemperatureForDay(dateOfApplication);

                var fractionOfManureUsed = (manureApplication.AmountOfManureAppliedPerHectare * viewItem.Area) / totalManureProducedByAnimals;
                if (fractionOfManureUsed > 1.0)
                    fractionOfManureUsed = 1.0;

                landApplicationEmissionResult.TotalTANApplied = totalTANProduced * fractionOfManureUsed;
                landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication = manureApplication.AmountOfNitrogenAppliedPerHectare * viewItem.Area;
                landApplicationEmissionResult.TotalVolumeOfManureUsedDuringApplication = manureApplication.AmountOfManureAppliedPerHectare * viewItem.Area;

                var adjustedAmmoniaEmissionFactor = CalculateAdjustedAmmoniaEmissionFactor(viewItem, manureApplication, averageDailyTemperature);

                var fractionVolatilized = landApplicationFactors.VolatilizationFraction;
                landApplicationEmissionResult.AmmoniacalLoss = this.CalculateNH3NLossFromLandAppliedManure(
                    fractionOfManure: fractionOfManureUsed,
                    totalTANProduced: totalTANProduced,
                    adjustedAmmoniaEmissionFactor: adjustedAmmoniaEmissionFactor,
                    totalNitrogenProduced: totalNitrogenProduced,
                    volatilizationFractionForLandApplication: fractionVolatilized,
                    temperature: averageDailyTemperature,
                    totalManureProducedByAnimals: totalManureProducedByAnimals,
                    animalType: animalType);

                var ammoniaLoss = CoreConstants.ConvertToNH3(landApplicationEmissionResult.AmmoniacalLoss);

                // Equation 4.6.3-2
                landApplicationEmissionResult.TotalN2ONFromManureVolatilized = landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * fractionVolatilized * landApplicationFactors.EmissionFactorVolatilization;

                var n2OVolatilized = CoreConstants.ConvertToN2O(landApplicationEmissionResult.TotalN2ONFromManureVolatilized);

                // Equation 4.6.3-3
                landApplicationEmissionResult.AdjustedAmmoniacalLoss = landApplicationEmissionResult.AmmoniacalLoss - landApplicationEmissionResult.TotalN2ONFromManureVolatilized;

                // Equation 4.6.3-5
                var adjustedAmmoniaEmissions = CoreConstants.ConvertToNH3(landApplicationEmissionResult.AdjustedAmmoniacalLoss);

                var leachingFraction = CalculateLeachingFraction(growingSeasonPrecipitation, growingSeasonEvapotranspiration);
                var e = landApplicationFactors.LeachingFraction;

                // Equation 4.6.4-1
                landApplicationEmissionResult.TotalN2ONFromManureLeaching = landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * leachingFraction * landApplicationFactors.EmissionFactorLeach;

                // Equation 4.6.4-3
                landApplicationEmissionResult.TotalNitrateLeached = landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * leachingFraction * (1.0 - landApplicationFactors.EmissionFactorLeach);

                // Equation 4.6.5-1
                landApplicationEmissionResult.TotalIndirectN2ONEmissions = landApplicationEmissionResult.TotalN2ONFromManureVolatilized + landApplicationEmissionResult.TotalN2ONFromManureLeaching;

                // Equation 4.6.5-2
                landApplicationEmissionResult.TotalIndirectN2OEmissions = CoreConstants.ConvertToN2O(landApplicationEmissionResult.TotalIndirectN2ONEmissions);

                results.Add(landApplicationEmissionResult);
            }

            return results;
        }

        private double GetEmissionFactorForLandApplication(
            CropViewItem cropViewItem,
            ManureApplicationViewItem manureApplicationViewItem)
        {
            return !manureApplicationViewItem.ManureStateType.IsLiquidManure()
                ? _beefDairyDefaultEmissionFactorsProvider.GetAmmoniaEmissionFactorForSolidAppliedManure(
                    cropViewItem.TillageType)
                : _beefDairyDefaultEmissionFactorsProvider.GetAmmoniaEmissionFactorForLiquidAppliedManure(
                    manureApplicationViewItem.ManureApplicationMethod);
        }

        #endregion
    }
}
