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

namespace H.Core.Calculators.Nitrogen
{
    public partial class N2OEmissionFactorCalculator
    {
        #region Public Methods

        /// <summary>
        /// Equation 4.6.1-1
        /// 
        /// Calculates direct emissions from the manure specifically applied to the field (kg N2O-N (kg N)^-1)
        /// </summary>
        public double CalculateDirectN2ONEmissionsFromFieldSpecificManureSpreading(
            CropViewItem viewItem,
            Farm farm)
        {
            var fieldSpecificOrganicNitrogenEmissionFactor = this.CalculateOrganicNitrogenEmissionFactor(
                viewItem: viewItem,
                farm: farm);

            var totalLocalAndImportedNitrogenApplied = viewItem.GetTotalManureNitrogenAppliedFromLivestockAndImportsInYear();

            var result = totalLocalAndImportedNitrogenApplied * fieldSpecificOrganicNitrogenEmissionFactor;

            return result;
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
        /// Equation 4.6.2-5
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
        public double CalculateLeftOverLandAppliedManureEmissionsForField(
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResults,
            Farm farm,
            CropViewItem viewItem,
            double weightedEmissionFactor)
        {
            // Get all fields that exist in the same year but (excluding native rangeland since manure is not spread on those fields)
            var itemsInYear = farm.GetCropDetailViewItemsByYear(viewItem.Year).Where(x => x.IsNativeGrassland == false);

            // This is the total amount of N from all animals that is available for land application
            var totalNitrogenAvailableForLandApplication = animalComponentEmissionsResults.TotalNitrogenAvailableForLandApplication();

            // This is the total amount of N that the user has specified is applied to all fields
            var totalManureNitrogenAppliedToAllFields = itemsInYear.Sum(x => x.GetTotalManureNitrogenAppliedFromLivestockAndImportsInYear());

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
            }

            return totalAmountsPerHectareFromManureApplications;
        }

        #endregion

        #region Private Methods

        private double CalculateEmissionsFromExportedManure()
        {
            throw new NotImplementedException();
        }

        private List<LandApplicationEmissionResult>  CalculateAmmoniaFromLandApplicationForImportedManure(CropViewItem viewItem, Farm farm)
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
            var totalTanForLandApplicationOnDate = animalComponentEmissionsResults.TotalTANAvailableForLandApplication;
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

                landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication = manureApplication.AmountOfNitrogenAppliedPerHectare * viewItem.Area;

                landApplicationEmissionResult.TotalVolumeOfManureUsedDuringApplication = manureApplication.AmountOfManureAppliedPerHectare * viewItem.Area;

                var adjustedEmissionFactor = CalculateAmbientTemperatureAdjustmentForLandApplication(averageDailyTemperature);

                var emissionFactorForLandApplication = GetEmissionFactorForLandApplication(viewItem, manureApplication);
                var adjustedAmmoniaEmissionFactor = CalculateAdjustedAmmoniaEmissionFactor(emissionFactorForLandApplication, adjustedEmissionFactor);

                var fractionVolatilized = 0d;
                if (animalType.IsBeefCattleType() || animalType.IsDairyCattleType())
                {
                    // Equation 4.6.2-3
                    landApplicationEmissionResult.AmmoniacalLoss = fractionOfManureUsed * totalTanForLandApplicationOnDate * adjustedAmmoniaEmissionFactor;

                    // Equation 4.6.3-1
                    fractionVolatilized = landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication > 0 ? landApplicationEmissionResult.AmmoniacalLoss / landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication : 0;
                }
                else if (animalType.IsSheepType() || animalType.IsSwineType() || animalType.IsOtherAnimalType())
                {
                    // Equation 4.6.2-7
                    landApplicationEmissionResult.AmmoniacalLoss = fractionOfManureUsed * landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * landApplicationFactors.VolatilizationFraction;

                    fractionVolatilized = landApplicationFactors.VolatilizationFraction;
                }
                else
                {
                    var emissionFraction = 0d;
                    if (averageDailyTemperature >= 15)
                    {
                        emissionFraction = 0.85;
                    }
                    else if (averageDailyTemperature >= 10 && averageDailyTemperature < 15)
                    {
                        emissionFraction = 0.73;
                    }
                    else if (averageDailyTemperature >= 5 && averageDailyTemperature < 10)
                    {
                        emissionFraction = 0.35;
                    }
                    else
                    {
                        emissionFraction = 0.25;
                    }

                    // Equation 4.6.2-5
                    landApplicationEmissionResult.AmmoniacalLoss = fractionOfManureUsed * totalManureProducedByAnimals * emissionFraction;

                    // Equation 4.6.3-1
                    fractionVolatilized = landApplicationEmissionResult.AmmoniacalLoss / landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication;
                }

                // Equation 4.6.2-4
                // Equation 4.6.2-6
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
