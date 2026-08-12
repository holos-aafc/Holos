using System.Collections.Generic;
using H.Core.Emissions.Results;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Carbon
{
    public interface ICarbonInputCalculator
    {
        /// <summary>
        /// When set (per farm run), this calculator's manure tanks are built into the shared per-run store instead of a
        /// private one, so the soil-carbon manure inputs read the same tanks the rest of the pipeline uses (issue #451
        /// follow-up). Byte-identical to the private store; it just avoids a redundant rebuild.
        /// </summary>
        ManureTankStore SharedManureTankStore { get; set; }

        /// <summary>
        /// Equation 2.1.2-34
        /// Equation 2.1.2-2
        ///
        /// (kg C ha^-1)
        /// </summary>
        double CalculateInputsFromSupplementalHayFedToGrazingAnimals(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            CropViewItem nextYearViewItems,
            Farm farm);

        /// <summary>
        /// (kg C)
        /// </summary>
        double GetSupplementalLosses(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            CropViewItem nextYearViewItems,
            Farm farm);

        void AssignManureCarbonInputs(CropViewItem viewItem, Farm farm, List<AnimalComponentEmissionsResults> animalComponentEmissionsResults);
    }
}