using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Carbon
{
    public interface ICarbonInputCalculator
    {
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
        ///     (kg C)
        /// </summary>
        double GetSupplementalLosses(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            CropViewItem nextYearViewItems,
            Farm farm);

        void AssignManureCarbonInputs(CropViewItem viewItem, Farm farm, List<AnimalComponentEmissionsResults> animalComponentEmissionsResults);
    }
}