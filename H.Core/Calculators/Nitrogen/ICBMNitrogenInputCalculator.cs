using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen
{
    public class ICBMNitrogenInputCalculator : NitrogenInputCalculatorBase, IICBMNitrogenInputCalculator
    {
        #region Public Methods

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
        public double CalculateTotalAboveGroundResidueNitrogenUsingIcbm(
            CropViewItem currentYearViewItem,
            CropViewItem previousYearViewItem)
        {
            if (previousYearViewItem == null)
            {
                return 0;
            }

            var nitrogenContentOfGrainReturned = this.CalculateNitrogenContentGrainReturnedToSoil(
                carbonInputFromProduct: previousYearViewItem.CarbonInputFromProduct,
                nitrogenConcentrationInProduct: previousYearViewItem.NitrogenContentInProduct);

            var nitrogenContentOfStrawReturned = this.CalculateNitrogenContentStrawReturnedToSoil(
                carbonInputFromStraw: previousYearViewItem.CarbonInputFromStraw,
                nitrogenConcentrationInStraw: previousYearViewItem.NitrogenContentInStraw);

            var cropType = previousYearViewItem.CropType;

            if (cropType.IsAnnual() || cropType.IsPerennial())
            {
                return nitrogenContentOfGrainReturned + nitrogenContentOfStrawReturned;
            }

            if (cropType.IsRootCrop())
            {
                return nitrogenContentOfStrawReturned;
            }

            if (cropType.IsCoverCrop() || cropType.IsSilageCrop())
            {
                return nitrogenContentOfGrainReturned;
            }

            // Fallow
            return 0;
        }

        /// <summary>
        /// Equation 2.6.5-8
        /// Equation 2.6.2-6
        /// Equation 2.7.2-4
        /// Equation 2.7.2-6
        /// Equation 2.7.2-8
        /// Equation 2.7.2-10
        /// </summary>
        /// <returns>Below ground residue N (kg N ha^-1)</returns>
        public double CalculateTotalBelowGroundResidueNitrogenUsingIcbm(
            CropViewItem currentYearViewItem,
            CropViewItem previousYearViewItem)
        {
            if (previousYearViewItem == null)
            {
                return 0;
            }

            var grainNitrogen = this.CalculateNitrogenContentGrainReturnedToSoil(
                carbonInputFromProduct: previousYearViewItem.CarbonInputFromProduct,
                nitrogenConcentrationInProduct: previousYearViewItem.NitrogenContentInProduct);

            var rootNitrogen = this.CalculateNitrogenContentRootReturnedToSoil(
                carbonInputFromRoots: previousYearViewItem.CarbonInputFromRoots,
                nitrogenConcentrationInRoots: previousYearViewItem.NitrogenContentInRoots);

            var exudateNitrogen = this.CalculateNitrogenContentExaduatesReturnedToSoil(
                carbonInputFromExtraroots: previousYearViewItem.CarbonInputFromExtraroots,
                nitrogenConcentrationInExtraroots: previousYearViewItem.NitrogenContentInExtraroot);

            var cropType = previousYearViewItem.CropType;

            // Equation 2.5.6-7
            if (cropType.IsAnnual())
            {
                return rootNitrogen + exudateNitrogen;
            }

            // Equation 2.5.6-8
            // Equation 2.6.2-6
            if (cropType.IsPerennial())
            {
                return rootNitrogen + exudateNitrogen;
            }

            if (cropType.IsRootCrop())
            {
                return grainNitrogen + exudateNitrogen;
            }

            if (cropType.IsSilageCrop() || cropType.IsCoverCrop())
            {
                return rootNitrogen + exudateNitrogen;
            }

            return 0;
        }

        #endregion
    }
}