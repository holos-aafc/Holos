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

            var totalNitrogenApplied = viewItem.GetTotalManureNitrogenAppliedFromLivestockAndImportsInYear();

            var result = totalNitrogenApplied * fieldSpecificOrganicNitrogenEmissionFactor;

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
        public double CalculateLeftOverLandAppliedManureEmissionsForField(
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResults,
            Farm farm,
            CropViewItem viewItem,
            double weightedEmissionFactor)
        {
            // Get all fields that exist in the same year
            var itemsInYear = farm.GetCropDetailViewItemsByYear(viewItem.Year);

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

            foreach (var animalComponentEmissionsResult in animalComponentEmissionsResults)
            {
                // Each item in the list will be total amounts per field
                indirectEmissionsForAllFields.AddRange(this.CalculateAmmoniaEmissionsFromLandAppliedManure(farm, animalComponentEmissionsResult));
            }

            // This will be a list of all indirect emissions for land applied manure for each year of history for this field
            var indirectEmissionsForField = indirectEmissionsForAllFields.Where(x => x.CropViewItem.FieldSystemComponentGuid.Equals(viewItem.FieldSystemComponentGuid));

            // Filter by year
            var byYear = indirectEmissionsForField.Where(x => x.CropViewItem.Year.Equals(viewItem.Year));

            foreach (var landApplicationEmissionResult in byYear)
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

                totalAmountsPerHectareFromManureApplications.ActualAmountOfNitrogenAppliedFromLandApplication += landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication > 0
                    ? landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication / viewItem.Area
                    : 0;
            }

            return totalAmountsPerHectareFromManureApplications;
        }

        #endregion

        #region Private Methods

        private List<LandApplicationEmissionResult> CalculateAmmoniaEmissionsFromLandAppliedManure(
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
            var evapotranspiration = farm.ClimateData.EvapotranspirationData.GetTotalAnnualEvapotranspiration();

            var landApplicationFactors = _livestockEmissionConversionFactorsProvider.GetLandApplicationFactors(farm, annualPrecipitation, evapotranspiration);
            foreach (var tuple in applicationsAndCropByAnimalType)
            {
                var landApplicationEmissionResult = new LandApplicationEmissionResult();

                var crop = tuple.Item1;
                landApplicationEmissionResult.CropViewItem = crop;

                var manureApplication = tuple.Item2;

                var date = manureApplication.DateOfApplication;
                var temperature = farm.ClimateData.GetAverageTemperatureForMonthAndYear(date.Year, (Months)date.Month);

                var fractionOfManureUsed = (manureApplication.AmountOfManureAppliedPerHectare * crop.Area) / totalManureProducedByAnimals;
                if (fractionOfManureUsed > 1.0)
                    fractionOfManureUsed = 1.0;

                landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication = manureApplication.AmountOfNitrogenAppliedPerHectare * crop.Area;

                landApplicationEmissionResult.TotalVolumeOfManureUsedDuringApplication = manureApplication.AmountOfManureAppliedPerHectare * crop.Area;

                var adjustedEmissionFactor = CalculateAmbientTemperatureAdjustmentForLandApplication(temperature);

                var emissionFactorForLandApplication = GetEmissionFactorForLandApplication(crop, manureApplication);
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
                    landApplicationEmissionResult.AmmoniacalLoss = fractionOfManureUsed * totalManureProducedByAnimals * emissionFraction;

                    // Equation 4.6.3-1
                    fractionVolatilized = landApplicationEmissionResult.AmmoniacalLoss / landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication;
                }

                // Equation 4.6.2-4
                // Equation 4.6.2-6
                var ammoniaLoss = landApplicationEmissionResult.AmmoniacalLoss * CoreConstants.ConvertNH3NToNH3;

                // Equation 4.6.3-2
                landApplicationEmissionResult.TotalN2ONFromManureVolatilized = landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * fractionVolatilized * landApplicationFactors.EmissionFactorVolatilization;

                // Equation 4.6.3-3
                var n2OVolatilized = landApplicationEmissionResult.TotalN2ONFromManureVolatilized * CoreConstants.ConvertN2ONToN2O;

                // Equation 4.6.3-4
                landApplicationEmissionResult.AdjustedAmmoniacalLoss = landApplicationEmissionResult.AmmoniacalLoss - landApplicationEmissionResult.TotalN2ONFromManureVolatilized;

                // Equation 4.6.3-5
                var adjustedAmmoniaEmissions = landApplicationEmissionResult.AdjustedAmmoniacalLoss * CoreConstants.ConvertNH3NToNH3;

                var leachingFraction = CalculateLeachingFraction(annualPrecipitation, evapotranspiration);

                // Equation 4.6.4-1
                landApplicationEmissionResult.TotalN2ONFromManureLeaching = landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * leachingFraction * landApplicationFactors.EmissionFactorLeach;

                // Equation 4.6.4-4
                landApplicationEmissionResult.TotalNitrateLeached = landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * leachingFraction * (1.0 - landApplicationFactors.EmissionFactorLeach);

                // Equation 4.6.5-1
                landApplicationEmissionResult.TotalIndirectN2ONEmissions = landApplicationEmissionResult.TotalN2ONFromManureVolatilized + landApplicationEmissionResult.TotalN2ONFromManureLeaching;

                // Equation 4.6.5-2
                landApplicationEmissionResult.TotalIndirectN2OEmissions = landApplicationEmissionResult.TotalIndirectN2ONEmissions * CoreConstants.ConvertN2ONToN2O;

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
