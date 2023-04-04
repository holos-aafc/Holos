using System;
using System.Linq;
using H.Core.Models;
using H.Core.Models.Infrastructure;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen
{
    public partial class N2OEmissionFactorCalculator
    {
        #region Public Methods

        public double CalculateTotalEmissionsFromRemainingDigestateThatIsAppliedToAllFields(
            double weightedEmissionFactor,
            double totalNitrogenFromRemainingDigestate)
        {

            var result = totalNitrogenFromRemainingDigestate * weightedEmissionFactor;

            return result;
        }

        public double CalculateDirectN2ONEmissionsFromFieldSpecificDigestateSpreading(
            CropViewItem viewItem,
            Farm farm)
        {
            var totalNitrogenApplied = 0d;

            var fieldSpecificOrganicNitrogenEmissionFactor = this.CalculateOrganicNitrogenEmissionFactor(
                viewItem: viewItem,
                farm: farm);

            var dailyOutputs = _digestateService.GetDailyResults(farm);
            foreach (var digestateApplicationViewItem in viewItem.DigestateApplicationViewItems)
            {
                var tank = _digestateService.GetTank(farm, digestateApplicationViewItem.DateCreated, dailyOutputs);
                totalNitrogenApplied += _digestateService.CalculateTotalNitrogenFromDigestateApplication(viewItem, digestateApplicationViewItem, tank);
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
            var nitrogenRemainingAtEndOfYear = _digestateService.GetTotalNitrogenRemainingAtEndOfYear(viewItem.Year, farm, component);

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