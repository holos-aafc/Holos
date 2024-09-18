using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen
{
    public interface IICBMNitrogenInputCalculator : INitrogenInputCalculator
    {
        /// <summary>
        /// Equation 2.5.6-1
        /// </summary>
        double CalculateGrainNitrogenTotal(
            double carbonInputFromAgriculturalProduct,
            double nitrogenConcentrationInProduct);

        /// <summary>
        /// Equation 2.5.6-2
        /// </summary>
        /// <param name="carbonInputFromProduct">Carbon input from product (kg ha^-1) </param>
        /// <param name="nitrogenConcentrationInProduct">N concentration in the product (kg kg-1) </param>
        double CalculateNitrogenContentGrainReturnedToSoil(
            double carbonInputFromProduct,
            double nitrogenConcentrationInProduct);

        /// <summary>
        /// Equation 2.5.6-3
        /// </summary>
        /// <param name="carbonInputFromStraw">Carbon input from straw (kg ha^-1)</param>
        /// <param name="nitrogenConcentrationInStraw"></param>
        double CalculateNitrogenContentStrawReturnedToSoil(
            double carbonInputFromStraw,
            double nitrogenConcentrationInStraw);

        /// <summary>
        /// Equation 2.5.6-4
        /// </summary>
        /// <param name="carbonInputFromRoots">Carbon input from roots (kg ha^-1)</param>
        /// <param name="nitrogenConcentrationInRoots">N concentration in the roots (kg kg-1) </param>
        double CalculateNitrogenContentRootReturnedToSoil(
            double carbonInputFromRoots,
            double nitrogenConcentrationInRoots);

        /// <summary>
        /// Equation 2.5.6-5
        /// </summary>
        /// <param name="carbonInputFromExtraroots">Carbon input from extra-root material (kg ha^-1)</param>
        /// <param name="nitrogenConcentrationInExtraroots">N concentration in the extra root (kg kg-1) (until known from literature, the same N concentration used for roots will be utilized)</param>
        double CalculateNitrogenContentExaduatesReturnedToSoil(
            double carbonInputFromExtraroots,
            double nitrogenConcentrationInExtraroots);

        /// <summary>
        /// Equation 2.5.5-5
        /// Equation 2.5.5-6
        /// </summary>
        /// <param name="nitrogenContentOfGrainReturnedToSoil">Nitrogen content of the grain returned to the soil (kg N ha^-1)</param>
        /// <param name="fertilizerEfficiencyFraction">Fertilizer use efficiency (fraction)</param>
        /// <param name="soilTestN">User defined value for existing Soil N supply for which fertilization rate was adapted</param>
        /// <param name="isNitrogenFixingCrop">Indicates if the type of crop is nitrogen fixing.</param>
        /// <param name="nitrogenFixationAmount">The amount of nitrogen fixation by the crop (fraction)</param>
        /// <param name="atmosphericNitrogenDeposition">N deposition on a specific field n (kg ha^-1) </param>
        /// <returns>N fertilizer applied (kg ha^-1)</returns>
        double CalculateSyntheticFertilizerApplied(double nitrogenContentOfGrainReturnedToSoil,
            double fertilizerEfficiencyFraction,
            double soilTestN,
            bool isNitrogenFixingCrop,
            double nitrogenFixationAmount,
            double atmosphericNitrogenDeposition);

        double CalculateTotalAboveGroundResidueNitrogenUsingIcbm(CropViewItem cropViewItem);
        double CalculateTotalBelowGroundResidueNitrogenUsingIcbm(CropViewItem cropViewItem);
    }
}