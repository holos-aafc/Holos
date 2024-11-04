using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using H.Core.Emissions.Results;
using System.Collections.Generic;
using System.Windows.Controls;

namespace H.Core.Calculators.Carbon
{
    public interface ICarbonService
    {
        bool CanCalculateInputsUsingIpccTier2(CropViewItem cropViewItem);
        void CalculateInputsAndLosses(CropViewItem previousYear, CropViewItem viewItem, CropViewItem nextYear, Farm farm);
        void CalculateLosses(CropViewItem cropViewItem, Farm farm);

        double CalculateManureCarbonInputFromGrazingAnimals(
            FieldSystemComponent fieldSystemComponent,
            CropViewItem cropViewItem,
            List<AnimalComponentEmissionsResults> results);

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
        /// Equation 5.6.1-1
        ///
        /// (kg C ha^-1)
        /// </summary>
        void CalculateManureCarbonInputByGrazingAnimals(
            FieldSystemComponent fieldSystemComponent,
            IEnumerable<AnimalComponentEmissionsResults> results,
            List<CropViewItem> cropViewItems);

        /// <summary>
        /// Calculates how much carbon was lost due to bales being exported off field.
        /// </summary>
        void CalculateCarbonLostFromHayExports(Farm farm, CropViewItem cropViewItem);

        /// <summary>
        /// Equation 11.3.2-4
        /// </summary>
        void CalculateCarbonLostByGrazingAnimals(Farm farm,
            FieldSystemComponent fieldSystemComponent,
            IEnumerable<AnimalComponentEmissionsResults> animalComponentEmissionsResults,
            List<CropViewItem> viewItems);

        double CalculateTotalDryMatterLossFromResidueExports(CropViewItem cropViewItem, Farm farm);
    }
}