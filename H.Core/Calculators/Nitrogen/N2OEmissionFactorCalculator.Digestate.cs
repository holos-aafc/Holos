using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Infrastructure;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.Animals;

namespace H.Core.Calculators.Nitrogen
{
    public partial class N2OEmissionFactorCalculator
    {
        #region Public Methods

        /// <summary>
        /// Calculate total indirect emissions from all land applied digestate to the crop.
        /// </summary>
        /// <returns>Total indirect emissions (ha^-1)</returns>
        public LandApplicationEmissionResult CalculateTotalIndirectEmissionsFromFieldSpecificDigestateApplications(
            CropViewItem viewItem,
            Farm farm)
        {
            var totalAmountsPerHectareFromDigestateApplications = new LandApplicationEmissionResult();

            // Each item in the list will be total amounts per field
            var totalAmountsForField = this.CalculateAmmoniaEmissionsFromLandAppliedDigestate(viewItem, farm);

            // Sum up all the indirect emissions from digestate applications to this field
            foreach (var landApplicationEmissionResult in totalAmountsForField)
            {
                /*
                 * Totals are for the entire field. Convert to per hectare below.
                 */

                // Equation 4.9.2-2
                totalAmountsPerHectareFromDigestateApplications.AmmoniacalLoss += landApplicationEmissionResult.AmmoniacalLoss > 0 
                    ? landApplicationEmissionResult.AmmoniacalLoss / viewItem.Area 
                    : 0;

                // Equation 4.9.4-2
                totalAmountsPerHectareFromDigestateApplications.TotalN2ONFromDigestateLeaching += landApplicationEmissionResult.TotalN2ONFromDigestateLeaching > 0
                    ? landApplicationEmissionResult.TotalN2ONFromDigestateLeaching / viewItem.Area
                    : 0;

                // No equation number
                totalAmountsPerHectareFromDigestateApplications.TotalNitrateLeached += landApplicationEmissionResult.TotalNitrateLeached > 0
                    ? landApplicationEmissionResult.TotalNitrateLeached / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromDigestateApplications.TotalN2ONFromDigestateVolatilized += landApplicationEmissionResult.TotalN2ONFromDigestateVolatilized > 0
                    ? landApplicationEmissionResult.TotalN2ONFromDigestateVolatilized / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromDigestateApplications.ActualAmountOfNitrogenAppliedFromLandApplication += landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication > 0
                    ? landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication / viewItem.Area
                    : 0;
            }

            return totalAmountsPerHectareFromDigestateApplications;
        }

        public List<LandApplicationEmissionResult> CalculateAmmoniaEmissionsFromLandAppliedDigestate(
            CropViewItem viewItem,
            Farm farm)
        {
            var results = new List<LandApplicationEmissionResult>();

            var annualPrecipitation = farm.ClimateData.PrecipitationData.GetTotalAnnualPrecipitation();
            var evapotranspiration = farm.ClimateData.EvapotranspirationData.GetTotalAnnualEvapotranspiration();
            var landApplicationFactors = _livestockEmissionConversionFactorsProvider.GetLandApplicationFactors(farm, annualPrecipitation, evapotranspiration);

            foreach (var digestateApplicationViewItem in viewItem.DigestateApplicationViewItems)
            {
                var landApplicationEmissionResult = new LandApplicationEmissionResult();

                landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication = digestateApplicationViewItem.AmountOfNitrogenAppliedPerHectare * viewItem.Area;

                // Equation 4.9.2-1
                landApplicationEmissionResult.AmmoniacalLoss = landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * landApplicationFactors.VolatilizationFraction;

                // Equation 4.9.2-3
                var ammoniaLoss = landApplicationEmissionResult.AmmoniacalLoss * CoreConstants.ConvertNH3NToNH3;

                // Equation 4.9.3-1
                landApplicationEmissionResult.TotalN2ONFromDigestateVolatilized = landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * landApplicationFactors.VolatilizationFraction * landApplicationFactors.EmissionFactorVolatilization;

                // Equation 4.9.3-2
                var n2OVolatilized = landApplicationEmissionResult.TotalN2ONFromDigestateVolatilized * CoreConstants.ConvertN2ONToN2O;

                // Equation 4.9.3-3
                landApplicationEmissionResult.AdjustedAmmoniacalLoss = landApplicationEmissionResult.AmmoniacalLoss - landApplicationEmissionResult.TotalN2ONFromDigestateVolatilized;

                // Equation 4.9.3-4
                var adjustedAmmoniaEmissions = landApplicationEmissionResult.AdjustedAmmoniacalLoss * CoreConstants.ConvertNH3NToNH3;

                var leachingFraction = CalculateLeachingFraction(annualPrecipitation, evapotranspiration);

                // Equation 4.9.4-1
                landApplicationEmissionResult.TotalN2ONFromDigestateLeaching = landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * leachingFraction * landApplicationFactors.EmissionFactorLeach;

                // Equation 4.9.4-3
                var n2OLeached = landApplicationEmissionResult.TotalN2ONFromDigestateLeaching * CoreConstants.ConvertN2ONToN2O;

                // No equation number
                landApplicationEmissionResult.TotalNitrateLeached = landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * leachingFraction * (1.0 - landApplicationFactors.EmissionFactorLeach);

                results.Add(landApplicationEmissionResult);
            }

            return results;
        }

        public double CalculateTotalEmissionsFromRemainingDigestateThatIsAppliedToAllFields(
            double weightedEmissionFactor,
            double totalNitrogenFromRemainingDigestate)
        {
            var result = totalNitrogenFromRemainingDigestate * weightedEmissionFactor;

            return result;
        }

        /// <summary>
        /// Calculates direct emissions from the digestate specifically applied to the field (kg N2O-N (kg N)^-1)
        /// </summary>
        public double CalculateDirectN2ONEmissionsFromFieldSpecificDigestateSpreading(
            CropViewItem viewItem,
            Farm farm)
        {
            var totalNitrogenApplied = 0d;

            var fieldSpecificOrganicNitrogenEmissionFactor = this.CalculateOrganicNitrogenEmissionFactor(
                viewItem: viewItem,
                farm: farm);

            foreach (var digestateApplicationViewItem in viewItem.DigestateApplicationViewItems)
            {
                totalNitrogenApplied += digestateApplicationViewItem.AmountOfNitrogenAppliedPerHectare * viewItem.Area;
            }

            var result = totalNitrogenApplied * fieldSpecificOrganicNitrogenEmissionFactor;

            return result;
        }

        public double CalculateLeftOverLandAppliedDigestateEmissionsForField(
            CropViewItem viewItem,
            Farm farm,
            double weightedEmissionFactor)
        {
            var component = farm.Components.OfType<AnaerobicDigestionComponent>().SingleOrDefault();
            if (component == null)
            {
                return 0;
            }

            // Get all fields that exist in the same year
            var itemsInYear = farm.GetCropDetailViewItemsByYear(viewItem.Year);

            // This is the total N remaining after all field applications have been considered
            var nitrogenRemainingAtEndOfYear = viewItem.GetRemainingNitrogenFromDigestateAtEndOfYear();

            // The total N2O-N from the remaining N
            var emissionsFromNitrogenRemaining = this.CalculateTotalEmissionsFromRemainingDigestateThatIsAppliedToAllFields(
                    weightedEmissionFactor: weightedEmissionFactor,
                    totalNitrogenFromRemainingDigestate: nitrogenRemainingAtEndOfYear);

            var totalAreaOfAllFields = itemsInYear.Sum(x => x.Area);
            var areaOfThisField = viewItem.Area;

            // The total N2O-N that is left over and must be associated with this field so that all digestate is applied to the fields in the same year (nothing is remaining to be applied)
            var result = emissionsFromNitrogenRemaining * (areaOfThisField / totalAreaOfAllFields);

            return result;
        }

        #endregion   
    }
}