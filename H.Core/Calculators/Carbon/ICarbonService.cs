using System.Collections.Generic;
using H.Core.Emissions.Results;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Carbon
{
    public interface ICarbonService
    {
        bool CanCalculateInputsUsingIpccTier2(CropViewItem cropViewItem);

        void AssignInputsAndLosses(CropViewItem previousYear, CropViewItem viewItem, CropViewItem nextYear, Farm farm,
            List<AnimalComponentEmissionsResults> animalResults);

        void CalculateLosses(CropViewItem cropViewItem, Farm farm);

        double CalculateManureCarbonInputFromGrazingAnimals(FieldSystemComponent fieldSystemComponent,
            CropViewItem cropViewItem,
            List<AnimalComponentEmissionsResults> results, Farm farm);

        /// <summary>
        ///     Equation 2.1.2-34
        ///     Equation 2.1.2-2
        ///     (kg C ha^-1)
        /// </summary>
        double CalculateInputsFromSupplementalHayFedToGrazingAnimals(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            CropViewItem nextYearViewItems,
            Farm farm);

        /// <summary>
        ///     Equation 5.6.1-1
        ///     (kg C ha^-1)
        /// </summary>
        void CalculateManureCarbonInputByGrazingAnimals(FieldSystemComponent fieldSystemComponent,
            IEnumerable<AnimalComponentEmissionsResults> results,
            List<CropViewItem> cropViewItems, Farm farm);

        /// <summary>
        ///     Calculates how much carbon was lost due to bales being exported off field.
        /// </summary>
        void CalculateCarbonLostFromHayExports(Farm farm, CropViewItem cropViewItem);

        /// <summary>
        ///     Equation 11.3.2-4
        /// </summary>
        void CalculateCarbonLostByGrazingAnimals(Farm farm,
            FieldSystemComponent fieldSystemComponent,
            IEnumerable<AnimalComponentEmissionsResults> animalComponentEmissionsResults,
            List<CropViewItem> viewItems);

        double CalculateTotalDryMatterLossFromResidueExports(CropViewItem cropViewItem, Farm farm);

        void AssignInputs(List<CropViewItem> cropViewItems, Farm farm,
            List<AnimalComponentEmissionsResults> animalResults);

        void CalculateLosses(List<CropViewItem> viewItems, Farm farm);

        void AssignInputsAndLosses(List<CropViewItem> viewItems, Farm farm,
            List<AnimalComponentEmissionsResults> animalResults);

        void AssignInputsAndLosses(AdjoiningYears tuple, Farm farm,
            List<AnimalComponentEmissionsResults> animalResults);

        /// <summary>
        ///     Totals the aboveground carbon input
        ///     (kg C ha^-1)
        /// </summary>
        double SumTotalAbovegroundCarbonInput(List<CropViewItem> viewItems);

        /// <summary>
        ///     Totals the belowground carbon input
        ///     (kg C ha^-1)
        /// </summary>
        double SumTotalBelowgroundCarbonInput(List<CropViewItem> viewItems);

        /// <summary>
        ///     Totals the manure carbon input
        ///     (kg C ha^-1)
        /// </summary>
        double SumTotalManureCarbonInput(List<CropViewItem> viewItems);

        /// <summary>
        ///     Totals the digestate carbon input
        ///     (kg C ha^-1)
        /// </summary>
        double SumTotalDigestateCarbonInput(List<CropViewItem> viewItems);

        /// <summary>
        ///     Combines C inputs for all items by year. This would combine the inputs from the main crop grown that year plus the
        ///     cover crop (if specified by user). This must be called
        ///     after all inputs from crops, manure, etc. have been calculated for each detail view item since we simply add up the
        ///     total above and below ground C inputs for each
        ///     year. Inputs from the secondary crop are added to the main crop since the main crop view item will be used in the
        ///     final ICBM/IPCC Tier 2 calculations
        /// </summary>
        void CombineCarbonInputs(Farm farm,
            List<CropViewItem> viewItems);

        void ProcessCommandLineItems(List<CropViewItem> viewItems, Farm farm,
            List<AnimalComponentEmissionsResults> animalResults);

        double GetSupplementalLosses(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            CropViewItem nextYearViewItems,
            Farm farm);
    }
}