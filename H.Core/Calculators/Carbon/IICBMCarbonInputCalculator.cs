using System.Collections.Generic;
using H.Core.Emissions.Results;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.LandManagement;

namespace H.Core.Calculators.Carbon
{
    public interface IICBMCarbonInputCalculator : ICarbonInputCalculator
    {
        /// <summary>
        ///     Calculates the plant carbon in the agricultural product for the given species grown in the given year.
        ///     C_p
        ///     Equation 2.1.2-1
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the current year</param>
        /// <param name="farm">The <see cref="Farm" /> being considered</param>
        /// <returns>The total above ground carbon input</returns>
        double CalculatePlantCarbonInAgriculturalProduct(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            Farm farm);

        /// <summary>
        ///     Equation 2.1.2-22
        ///     Equation 2.1.2-26
        /// </summary>
        double CalculateProductivity(double annualPrecipitation,
            double annualPotentialEvapotranspiration,
            double proportionOfPrecipitationMayThroughSeptember,
            double carbonConcentration);

        /// <summary>
        ///     Estimates the plant carbon in agricultural product for the subsequent year (t + 1) when considering perennial
        ///     stands with missing yield data.
        ///     C_p
        /// </summary>
        /// <param name="nextYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the subsequent (t + 1) year</param>
        /// <param name="farm">The <see cref="Farm" /> being considered</param>
        /// <returns>The estimated plant carbon in agricultural product</returns>
        double EstimatePlantCarbonInAgriculturalProductForNextYear(
            CropViewItem nextYearViewItem,
            Farm farm);

        /// <summary>
        ///     Calculates the total above ground carbon input for a year in which a perennial crop is grown. Note that for
        ///     perennials, all above ground inputs are
        ///     contributed by the C_ptoSoil as there is no C_s calculation for perennials.
        ///     Side effect: if this year doesn't have any C_p, a value will be assigned
        ///     C_ag (which is equivalent to the C_ptoSoil when considering a perennial)
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the current year</param>
        /// <param name="nextYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the subsequent year</param>
        /// <param name="farm">The <see cref="Farm" /> being considered</param>
        /// <returns>The total above ground carbon input</returns>
        double CalculateAboveGroundCarbonInputFromPerennials(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            CropViewItem nextYearViewItem,
            Farm farm);

        /// <summary>
        ///     Calculates the carbon input from the product for the given species grown in the given year.
        ///     C_ptoSoil
        ///     Equation 2.1.2-6
        ///     Equation 2.1.2-10
        ///     Equation 2.1.2-14
        ///     Equation 2.1.2-17
        ///     Equation 2.1.2-20
        ///     Equation 2.1.2-23
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the current year</param>
        /// <param name="nextYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the subsequent year</param>
        /// <param name="farm">The <see cref="Farm" /> being considered</param>
        /// <returns>The carbon input from the product</returns>
        double CalculateCarbonInputFromProduct(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            CropViewItem nextYearViewItem,
            Farm farm);

        /// <summary>
        ///     Calculates the carbon input from straw for the given species grown in the given year.
        ///     C_s
        ///     Equation 2.1.2-7
        ///     Equation 2.1.2-18
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the current year</param>
        /// <param name="farm">The <see cref="Farm" /> being considered</param>
        /// <returns>The carbon input from straw</returns>
        double CalculateCarbonInputFromStraw(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            Farm farm);

        /// <summary>
        ///     Calculates the carbon input from roots for a year in which a perennial crop is grown.
        ///     C_r
        ///     Note: the <see cref="FieldResultsService.AssignDefaultBiomassCoefficients" /> sets the
        ///     <see cref="H.Core.Models.LandManagement.Fields.CropViewItem.BiomassCoefficientProduct" /> to the value of the
        ///     biomass coefficient of straw returned by the provider for perennials.
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the current year</param>
        /// <param name="farm">The <see cref="Farm" /> being considered</param>
        /// <returns>The carbon input from roots</returns>
        double CalculateCarbonInputFromRootsForPerennials(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            Farm farm);

        /// <summary>
        ///     Calculates the carbon input from extraroots for the given species grown in the given year when the harvest method
        ///     is green manure.
        ///     C_e
        ///     When green manure is chosen as a harvest option for a main crop (meaning the user is entering an above ground
        ///     biomass value instead of a grain yield), the residue fractions for product
        ///     and straw are combined to form a single coefficient, Rp, and Cs is omitted.
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the current year</param>
        /// <param name="farm">The <see cref="Farm" /> being considered</param>
        /// <returns>The carbon input from extraroots</returns>
        double CalculateCarbonInputFromExtrarootsForGreenManureOrSwathing(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            Farm farm);

        /// <summary>
        ///     Calculates the carbon input from extraroots for a year in which a perennial crop is grown.
        ///     C_e
        ///     Note: the <see cref="FieldResultsService.AssignDefaultBiomassCoefficients" /> sets the
        ///     <see cref="H.Core.Models.LandManagement.Fields.CropViewItem.BiomassCoefficientProduct" /> to the value of the
        ///     biomass coefficient of straw returned by the provider for perennials.
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the current year</param>
        /// <param name="farm">The <see cref="Farm" /> being considered</param>
        /// <returns>The carbon input from extraroots</returns>
        double CalculateCarbonInputFromExtrarootsForPerennials(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            Farm farm);

        /// <summary>
        ///     Calculates the carbon input from extraroots for the given species grown in the given year.
        ///     C_e
        ///     Equation 2.1.2-9
        ///     Equation 2.1.2-12
        ///     Equation 2.1.2-16
        ///     Equation 2.1.2-19
        ///     Equation 2.1.2-25
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the current year</param>
        /// <param name="farm">The <see cref="Farm" /> being considered</param>
        /// <returns>The carbon input from extraroots</returns>
        double CalculateCarbonInputFromExtraroot(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            Farm farm);

        /// <summary>
        ///     Calculates the carbon input from roots for the given species grown in the given year when the harvest method is
        ///     green manure.
        ///     C_r
        ///     When green manure is chosen as a harvest option for a main crop (meaning the user is entering an above ground
        ///     biomass value instead of a grain yield), the residue fractions for product
        ///     and straw are combined to form a single coefficient, Rp, and Cs is omitted.
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the current year</param>
        /// <param name="farm">The <see cref="Farm" /> being considered</param>
        /// <returns>The carbon input from roots</returns>
        double CalculateCarbonInputFromRootsForGreenManureOrSwathing(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            Farm farm);

        /// <summary>
        ///     Calculates the carbon input from roots for the given species grown in the given year.
        ///     C_r
        ///     Equation 2.1.2-8
        ///     Equation 2.1.2-11
        ///     Equation 2.1.2-15
        ///     Equation 2.1.2-21
        ///     Equation 2.1.2-24
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent" /> in the current year</param>
        /// <param name="farm">The <see cref="Farm" /> being considered</param>
        /// <returns>The carbon input from roots</returns>
        double CalculateCarbonInputFromRoots(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            Farm farm);

        /// <summary>
        ///     Calculates the total above ground carbon input for the given species grown in the given year.
        ///     C_ag
        ///     Equation 2.1.2-2
        ///     Equation 2.1.2-4
        /// </summary>
        /// <param name="cropViewItem">The details of the <see cref="FieldSystemComponent" /> in the current year</param>
        /// <param name="farm">The <see cref="Farm" /> being considered</param>
        /// <returns>The total above ground carbon input</returns>
        double CalculateTotalAboveGroundCarbonInput(
            CropViewItem cropViewItem,
            Farm farm);

        /// <summary>
        ///     Calculates the total below ground carbon input for the given species grown in the given year.
        ///     C_bg
        ///     Equation 2.1.2-3
        ///     Equation 2.1.2-5
        /// </summary>
        /// <param name="cropViewItem">The details of the <see cref="FieldSystemComponent" /> in the current year</param>
        /// <param name="farm">The <see cref="Farm" /> being considered</param>
        /// <returns>The total below ground carbon input</returns>
        double CalculateTotalBelowGroundCarbonInput(
            CropViewItem cropViewItem,
            Farm farm);

        CropViewItem AssignInputs(CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            CropViewItem nextYearViewItem,
            Farm farm, List<AnimalComponentEmissionsResults> animalResults);
    }
}