using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Emissions.Results;
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

        public LandApplicationEmissionResult CalculateTotalIndirectEmissionsFromFieldSpecificDigestateApplications(
            CropViewItem viewItem,
            Farm farm)
        {
            var indirectEmissionsForAllApplications = new List<LandApplicationEmissionResult>();

            // Calculate results for farm produced digestate spreading
            var results = this.CalculateAmmoniaEmissionsFromLandAppliedDigestate(viewItem, farm);

            indirectEmissionsForAllApplications.AddRange(results);

            var resultsPerHectare = this.ConvertPerFieldEmissionsToPerHectare(indirectEmissionsForAllApplications, viewItem);

            return resultsPerHectare;
        }

        public List<LandApplicationEmissionResult> CalculateAmmoniaEmissionsFromLandAppliedDigestate(
            CropViewItem viewItem,
            Farm farm)
        {
            var results = new List<LandApplicationEmissionResult>();

            var emissionFactorForVolatilization = this.GetEmissionFactorForVolatilization(farm, viewItem.Year);

            foreach (var digestateApplicationViewItem in viewItem.DigestateApplicationViewItems)
            {
                var landApplicationEmissionResult = new LandApplicationEmissionResult();

                landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication = this.GetAmountOfNitrogenUsed(viewItem, digestateApplicationViewItem);
                landApplicationEmissionResult.AmmoniacalLoss = landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * 0.1705;
                landApplicationEmissionResult.AmmoniaLoss = CoreConstants.ConvertToNH3(landApplicationEmissionResult.AmmoniacalLoss);
                landApplicationEmissionResult.TotalN2ONFromDigestateVolatilized = landApplicationEmissionResult.AmmoniacalLoss * emissionFactorForVolatilization;
                landApplicationEmissionResult.TotalN2OFromDigestateVolatilized = CoreConstants.ConvertToN2O(landApplicationEmissionResult.TotalN2ONFromDigestateVolatilized);
                landApplicationEmissionResult.AdjustedAmmoniacalLoss = landApplicationEmissionResult.AmmoniacalLoss - landApplicationEmissionResult.TotalN2ONFromDigestateVolatilized;
                landApplicationEmissionResult.AdjustedAmmoniaLoss = CoreConstants.ConvertToNH3(landApplicationEmissionResult.AdjustedAmmoniacalLoss);
                landApplicationEmissionResult.TotalN2ONFromDigestateLeaching = this.CalculateTotalN2ONFromLeachingForField(farm, viewItem, digestateApplicationViewItem);
                landApplicationEmissionResult.TotalNitrateLeached = this.CalculateTotalNitrateLeached(farm, viewItem, digestateApplicationViewItem);
                landApplicationEmissionResult.TotalIndirectN2ONEmissions = landApplicationEmissionResult.TotalN2ONFromDigestateVolatilized + landApplicationEmissionResult.TotalN2ONFromDigestateLeaching;
                landApplicationEmissionResult.TotalIndirectN2OEmissions = CoreConstants.ConvertToN2O(landApplicationEmissionResult.TotalIndirectN2ONEmissions);

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

        /// <summary>
        /// (kg N2O-N)
        /// </summary>
        public double CalculateLeftOverLandAppliedDigestateEmissionsForField(
            CropViewItem viewItem,
            Farm farm)
        {
            var itemsByYear = farm.GetCropDetailViewItemsByYear(viewItem.Year, false);
            var weightedEmissionFactorForOrganicNitrogen = this.CalculateWeightedOrganicNitrogenEmissionFactor(itemsByYear, farm);

            var component = farm.Components.OfType<AnaerobicDigestionComponent>().SingleOrDefault();
            if (component == null)
            {
                return 0;
            }

            // Get all fields that exist in the same year
            var itemsInYear = farm.GetCropDetailViewItemsByYear(viewItem.Year, false);

            // This is the total N remaining after all field applications have been considered
            var nitrogenRemainingAtEndOfYear = viewItem.GetRemainingNitrogenFromDigestateAtEndOfYear();

            // The total N2O-N from the remaining N
            var emissionsFromNitrogenRemaining = this.CalculateTotalEmissionsFromRemainingDigestateThatIsAppliedToAllFields(
                    weightedEmissionFactor: weightedEmissionFactorForOrganicNitrogen,
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