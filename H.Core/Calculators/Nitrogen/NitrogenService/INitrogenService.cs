using System.Collections.Generic;
using H.Core.Emissions.Results;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen.NitrogenService
{
    public interface INitrogenService
    {
        /// <summary>
        ///     Calculates the total aboveground residue N
        ///     (kg N ha^-1)
        /// </summary>
        double CalculateAboveGroundResidueNitrogen(Farm farm, CropViewItem currentYearViewItem,
            CropViewItem previousYearViewItem);

        /// <summary>
        ///     Calculates the total belowground residue N
        ///     (kg N ha^-1)
        /// </summary>
        double CalculateBelowGroundResidueNitrogen(Farm farm, CropViewItem currentYearViewItem,
            CropViewItem previousYearViewItem);

        double CalculateCropResidueExportNitrogen(CropViewItem cropViewItem);
        void AssignNitrogenInputs(CropViewItem currentYearViewItem, Farm farm, CropViewItem previousYearViewItem);
        void AssignNitrogenInputs(List<CropViewItem> viewItems, Farm farm);

        /// <summary>
        ///     Combines N inputs for all items by year. This would combine the inputs from the main crop grown that year plus the
        ///     cover crop (if specified by user).
        ///     Inputs from the secondary crop are added to the main crop since the main crop view item will be used in the final
        ///     ICBM/IPCC Tier 2 calculations
        /// </summary>
        void CombineNitrogenInputs(Farm farm, List<CropViewItem> viewItems);

        void ProcessCommandLineItems(List<CropViewItem> viewItems, Farm farm);
        void AssignNitrogenInputs(AdjoiningYears adjoiningYears, Farm farm);

        /// <summary>
        ///     Equation 5.6.2-1
        ///     (kg N ha^-1)
        /// </summary>
        double CalculateManureNitrogenInputsFromGrazingAnimals(FieldSystemComponent fieldSystemComponent,
            CropViewItem cropViewItem,
            List<AnimalComponentEmissionsResults> results);

        void CalculateManureNitrogenInputByGrazingAnimals(FieldSystemComponent fieldSystemComponent,
            IEnumerable<AnimalComponentEmissionsResults> results, List<CropViewItem> cropViewItems);
    }
}