using H.Core.Models.LandManagement.Fields;
using H.Core.Models;

namespace H.Core.Calculators.Carbon
{
    public class CarbonInputCalculatorBase : ICarbonInputCalculator
    {
        #region Public Methods

        public double CalculateInputsFromSupplementalHayFedToGrazingAnimals(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            CropViewItem nextYearViewItems,
            Farm farm)
        {
            var result = 0.0;

            // Get total amount of supplemental hay added
            var hayImportViewItems = currentYearViewItem.HayImportViewItems;
            foreach (var hayImportViewItem in hayImportViewItems)
            {
                // Total dry matter weight
                var totalDryMatterWeight = hayImportViewItem.GetTotalDryMatterWeightOfAllBales();

                // Amount lost during feeding
                var loss = farm.Defaults.DefaultSupplementalFeedingLossPercentage / 100;

                // Total additional carbon that must be added to above ground inputs for the field - NOTE: moisture content is already considered in the above method call and so it
                // is not included here as it is in the equation from the algorithm document
                var totalCarbon = (totalDryMatterWeight * (loss)) * currentYearViewItem.CarbonConcentration;

                result += totalCarbon;
            }

            return (result / currentYearViewItem.Area);
        }

        #endregion
    }
}