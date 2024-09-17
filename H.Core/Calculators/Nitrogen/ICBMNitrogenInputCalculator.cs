using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen
{
    public class ICBMNitrogenInputCalculator : NitrogenInputCalculatorBase, IICBMNitrogenInputCalculator
    {
        #region Public Methods

        /// <summary>
        /// Equation 2.5.6-1
        /// </summary>
        public double CalculateGrainNitrogenTotal(
            double carbonInputFromAgriculturalProduct,
            double nitrogenConcentrationInProduct)
        {
            var result = (carbonInputFromAgriculturalProduct / 0.45) * nitrogenConcentrationInProduct;

            return result;
        }

        /// <summary>
        /// Equation 2.5.6-2
        /// </summary>
        /// <param name="carbonInputFromProduct">Carbon input from product (kg ha^-1) </param>
        /// <param name="nitrogenConcentrationInProduct">N concentration in the product (kg kg-1) </param>
        public double CalculateNitrogenContentGrainReturnedToSoil(
            double carbonInputFromProduct,
            double nitrogenConcentrationInProduct)
        {
            var result = (carbonInputFromProduct / 0.45) * nitrogenConcentrationInProduct;

            return result;
        }

        /// <summary>
        /// Equation 2.5.6-3
        /// </summary>
        /// <param name="carbonInputFromStraw">Carbon input from straw (kg ha^-1)</param>
        /// <param name="nitrogenConcentrationInStraw"></param>
        public double CalculateNitrogenContentStrawReturnedToSoil(
            double carbonInputFromStraw,
            double nitrogenConcentrationInStraw)
        {
            var result = (carbonInputFromStraw / 0.45) * nitrogenConcentrationInStraw;

            return result;
        }

        /// <summary>
        /// Equation 2.5.6-4
        /// </summary>
        /// <param name="carbonInputFromRoots">Carbon input from roots (kg ha^-1)</param>
        /// <param name="nitrogenConcentrationInRoots">N concentration in the roots (kg kg-1) </param>
        public double CalculateNitrogenContentRootReturnedToSoil(
            double carbonInputFromRoots,
            double nitrogenConcentrationInRoots)
        {
            var result = (carbonInputFromRoots / 0.45) * nitrogenConcentrationInRoots;

            return result;
        }

        /// <summary>
        /// Equation 2.5.6-5
        /// </summary>
        /// <param name="carbonInputFromExtraroots">Carbon input from extra-root material (kg ha^-1)</param>
        /// <param name="nitrogenConcentrationInExtraroots">N concentration in the extra root (kg kg-1) (until known from literature, the same N concentration used for roots will be utilized)</param>
        public double CalculateNitrogenContentExaduatesReturnedToSoil(
            double carbonInputFromExtraroots,
            double nitrogenConcentrationInExtraroots)
        {
            var result = (carbonInputFromExtraroots / 0.45) * nitrogenConcentrationInExtraroots;

            return result;
        }

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
        public double CalculateSyntheticFertilizerApplied(double nitrogenContentOfGrainReturnedToSoil,
            double fertilizerEfficiencyFraction,
            double soilTestN,
            bool isNitrogenFixingCrop,
            double nitrogenFixationAmount,
            double atmosphericNitrogenDeposition)
        {
            var totalNitrogenContent = (nitrogenContentOfGrainReturnedToSoil);

            var result = 0d;
            if (isNitrogenFixingCrop)
            {
                // Equation 2.5.5-6
                result = (totalNitrogenContent * (1 - nitrogenFixationAmount) - soilTestN - atmosphericNitrogenDeposition) / fertilizerEfficiencyFraction;
            }
            else
            {
                // Equation 2.5.5-5
                result = (totalNitrogenContent - soilTestN - atmosphericNitrogenDeposition) / fertilizerEfficiencyFraction;
            }

            // Suggested amount can never be less than zero
            if (result < 0)
            {
                result = 0;
            }

            return result;
        }

        /// <summary>
        /// Equation 2.5.6-6
        /// Equation 2.6.2-2
        /// Equation 2.7.2-3
        /// Equation 2.7.2-5
        /// Equation 2.7.2-7
        /// Equation 2.7.2-9
        /// </summary>
        /// <returns>Above ground residue N (kg N ha^-1)</returns>
        public double CalculateTotalAboveGroundResidueNitrogenUsingIcbm(CropViewItem cropViewItem)
        {
            var nitrogenContentOfGrainReturned = this.CalculateNitrogenContentGrainReturnedToSoil(
                carbonInputFromProduct: cropViewItem.CarbonInputFromProduct,
                nitrogenConcentrationInProduct: cropViewItem.NitrogenContentInProduct);

            var nitrogenContentOfStrawReturned = this.CalculateNitrogenContentStrawReturnedToSoil(
                carbonInputFromStraw: cropViewItem.CarbonInputFromStraw,
                nitrogenConcentrationInStraw: cropViewItem.NitrogenContentInStraw);

            if (cropViewItem.CropType.IsAnnual() || cropViewItem.CropType.IsPerennial())
            {
                return nitrogenContentOfGrainReturned + nitrogenContentOfStrawReturned;
            }

            if (cropViewItem.CropType.IsRootCrop())
            {
                return nitrogenContentOfStrawReturned;
            }

            if (cropViewItem.CropType.IsCoverCrop() || cropViewItem.CropType.IsSilageCrop())
            {
                return nitrogenContentOfGrainReturned;
            }

            // Fallow
            return 0;
        }

        /// <summary>
        /// Equation 2.5.6-7
        /// Equation 2.5.6-8
        /// Equation 2.6.2-5
        /// Equation 2.7.2-4
        /// Equation 2.7.2-6
        /// Equation 2.7.2-8
        /// Equation 2.7.2-10
        /// </summary>
        /// <param name="cropViewItem"></param>
        /// <returns>Below ground residue N (kg N ha^-1)</returns>
        public double CalculateTotalBelowGroundResidueNitrogenUsingIcbm(CropViewItem cropViewItem)
        {
            var grainNitrogen = this.CalculateNitrogenContentGrainReturnedToSoil(
                carbonInputFromProduct: cropViewItem.CarbonInputFromProduct,
                nitrogenConcentrationInProduct: cropViewItem.NitrogenContentInProduct);

            var rootNitrogen = this.CalculateNitrogenContentRootReturnedToSoil(
                carbonInputFromRoots: cropViewItem.CarbonInputFromRoots,
                nitrogenConcentrationInRoots: cropViewItem.NitrogenContentInRoots);

            var exudateNitrogen = this.CalculateNitrogenContentExaduatesReturnedToSoil(
                carbonInputFromExtraroots: cropViewItem.CarbonInputFromExtraroots,
                nitrogenConcentrationInExtraroots: cropViewItem.NitrogenContentInExtraroot);

            // Equation 2.5.6-7
            if (cropViewItem.CropType.IsAnnual())
            {
                return rootNitrogen + exudateNitrogen;
            }

            // Equation 2.5.6-8
            if (cropViewItem.CropType.IsPerennial())
            {
                return rootNitrogen + exudateNitrogen;
            }

            if (cropViewItem.CropType.IsRootCrop())
            {
                return grainNitrogen + exudateNitrogen;
            }

            if (cropViewItem.CropType.IsSilageCrop() || cropViewItem.CropType.IsCoverCrop())
            {
                return rootNitrogen + exudateNitrogen;
            }

            return 0;
        }

        #endregion
    }
}