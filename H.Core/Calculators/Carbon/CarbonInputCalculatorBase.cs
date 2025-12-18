using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.Animals;

namespace H.Core.Calculators.Carbon
{
    public class CarbonInputCalculatorBase : ICarbonInputCalculator
    {
        #region Constructors

        public CarbonInputCalculatorBase()
        {
            manureService = new ManureService();
            digestateService = new DigestateService();
            animalService = new AnimalResultsService();
        }

        #endregion

        #region Fields

        protected readonly IManureService manureService;
        protected readonly IDigestateService digestateService;
        protected readonly IAnimalService animalService;

        #endregion

        #region Public Methods

        /// <summary>
        ///     Equation 2.1.2-34
        ///     Equation 2.1.2-2
        ///     (kg C ha^-1)
        /// </summary>
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
                var totalCarbon = totalDryMatterWeight * loss * currentYearViewItem.CarbonConcentration;

                result += totalCarbon;
            }

            return result / currentYearViewItem.Area;
        }

        /// <summary>
        ///     Equation 11.3.2-2 (b)
        ///     (kg C)
        /// </summary>
        public double GetSupplementalLosses(
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

                var totalCarbon = totalDryMatterWeight * currentYearViewItem.CarbonConcentration;

                // Amount lost during feeding
                var loss = farm.Defaults.DefaultSupplementalFeedingLossPercentage / 100;

                var localResult = totalCarbon / loss * (1 - loss);

                result += localResult;
            }

            return result;
        }

        public void AssignManureCarbonInputs(CropViewItem viewItem, Farm farm, List<AnimalComponentEmissionsResults> animalComponentEmissionsResults)
        {
            manureService.Initialize(farm, animalComponentEmissionsResults);
            digestateService.Initialize(farm, animalComponentEmissionsResults);

            if (farm.IsCommandLineMode == false)
            {
                viewItem.ManureCarbonInputsPerHectare = manureService.GetTotalManureCarbonInputsForField(farm, viewItem.Year, viewItem);
                viewItem.ManureCarbonInputsFromManureOnly = viewItem.GetTotalCarbonFromAppliedManure() / viewItem.Area;
            }
            else
            {
                if (viewItem.IsRunInPeriodItem)
                {
                    // Don't include manure C inputs for run in period in any CLI adjustments so that run in period total C inputs will be consistent with GUI total C inputs
                }
                else
                {
                    // Check if the user specified an amount of manure to be applied to the field.
                    if (viewItem.ManureCarbonInputsPerHectare <= 0)
                    {
                        /*
                         * If amount is zero, recalculate on behalf of the user since they may have a manure application made but not able to determine the total C added.
                         */

                        viewItem.ManureCarbonInputsPerHectare = manureService.GetTotalManureCarbonInputsForField(farm, viewItem.Year, viewItem);
                        viewItem.ManureCarbonInputsFromManureOnly = viewItem.GetTotalCarbonFromAppliedManure() / viewItem.Area;
                    }
                    else
                    {
                        // User has specified a non-zero amount of manure in the field input file so we leave those amounts alone (don't overwrite)
                    }
                }
            }

            viewItem.ManureCarbonInputsPerHectare += viewItem.TotalCarbonInputFromManureFromAnimalsGrazingOnPasture;
        }

        #endregion
    }
}