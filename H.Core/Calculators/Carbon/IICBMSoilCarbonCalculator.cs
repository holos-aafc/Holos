using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Carbon
{
    public interface IICBMSoilCarbonCalculator
    {
        double CalculateSoilCarbonAtInterval(
            double youngPoolSoilCarbonAboveGroundAtInterval, 
            double youngPoolSoilCarbonBelowGroundAtInterval, 
            double oldPoolSoilCarbonAtInterval, double youngPoolManureAtInterval);

        double CalculateChangeInSoilCarbonAtInterval(
            double soilOrganicCarbonAtInterval, 
            double soilOrganicCarbonAtPreviousInterval);

        double CalculateOldPoolSoilCarbonAtInterval(
            double oldPoolSoilCarbonAtPreviousInterval, 
            double aboveGroundHumificationCoefficient, 
            double belowGroundHumificationCoefficient, 
            double youngPoolDecompositionRate, 
            double oldPoolDecompositionRate, 
            double youngPoolAboveGroundOrganicCarbonAtPreviousInterval, 
            double youngPoolBelowGroundOrganicCarbonAtPreviousInterval, 
            double aboveGroundCarbonResidueAtPreviousInterval, 
            double belowGroundCarbonResidueAtPreviousInterval, 
            double climateParameter, 
            double youngPoolManureAtPreviousInterval, 
            double manureHumificationCoefficient,
            double manureCarbonInputAtPreviousInterval);

        double CalculateYoungPoolBelowGroundCarbonAtInterval(
            double youngPoolBelowGroundCarbonAtPreviousInterval, 
            double belowGroundCarbonAtPreviousInterval, 
            double youngPoolDecompositionRate, 
            double climateParameter);

        double CalculateYoungPoolAboveGroundCarbonAtInterval(
            double youngPoolAboveGroundCarbonAtPreviousInterval, 
            double aboveGroundCarbonAtPreviousInterval, 
            double youngPoolDecompositionRate, 
            double climateParameter);

        double CalculateOldPoolSteadyState(
            double youngPoolDecompositionRate, 
            double oldPoolDecompositionRate, 
            double climateParameter, 
            double aboveGroundHumificationCoefficient, 
            double belowGroundHumificationCoefficient, 
            double averageAboveGroundCarbonInputOfRotation, 
            double averageBelowGroundCarbonInputOfRotation, 
            double aboveGroundYoungPoolSteadyState, 
            double belowGroundYoungPoolSteadyState, 
            double manureYoungPoolSteadyState, 
            double averageManureCarbonInputOfRotation, 
            double manureHumificationCoefficient);

        double CalculateYoungPoolSteadyStateBelowGround(
            double averageBelowGroundCarbonInput, 
            double youngPoolDecompositionRate, 
            double climateParameter);

        double CalculateYoungPoolSteadyStateAboveGround(
            double averageAboveGroundCarbonInput, 
            double youngPoolDecompositionRate, 
            double climateParameter);

        double CalculateAverageAboveGroundResidueCarbonInput(
            double carbonInputFromProductOfEachRotationPhase, 
            double carbonInputFromStrawOfEachRotationPhase);

        double CalculateAverageBelowGroundResidueCarbonInput(
            double carbonInputFromRootsOfEachRotationPhase, 
            double carbonInputFromExtrarootOfEachRotationPhase);

        double CalculateAverageManureCarbonInput(double carbonInputsFromManureInputsOfEachRotationPhase);

        double CalculateYoungPoolSteadyStateManure(
            double averageManureCarbonInput, 
            double youngPoolDecompositionRate, 
            double climateParameter);

        double CalculateYoungPoolManureCarbonAtInterval(
            double youngPoolManureCarbonAtPreviousInterval, 
            double manureCarbonInputAtPreviousInterval, 
            double youngPoolDecompositionRate, 
            double climateParameter);

        double CalculateAmountOfNitrogenAppliedFromManure(
            double manureAmount, 
            double fractionOfNitrogenInAppliedManure);

        double CalculateAmountOfPhosphorusAppliedFromManure(
            double manureAmount, 
            double fractionOfPhosphorusInAppliedManure);

        double CalculateMoistureOfManure(
            double manureAmount, 
            double waterFraction);

        double CalculateChangeInSoilOrganicCarbonForFieldAtInterval(
            double changeInSoilOrganicCarbonAtInterval, 
            double fieldArea);

        CropViewItem SetCarbonInputs(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            CropViewItem nextYearViewItem,
            Farm farm);

        /// <summary>
        /// Calculates the plant carbon in the agricultural product for the given species grown in the given year.
        /// 
        /// C_p
        ///
        /// Equation 2.2.2-1
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the current year</param>
        /// <param name="farm">The <see cref="Farm"/> being considered</param>
        /// <returns>The total above ground carbon input</returns>
        double CalculatePlantCarbonInAgriculturalProduct(
            CropViewItem previousYearViewItem, 
            CropViewItem currentYearViewItem, 
            Farm farm);
    }
}