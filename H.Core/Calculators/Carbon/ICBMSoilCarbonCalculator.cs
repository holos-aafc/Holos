#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using H.Core.Calculators.Nitrogen;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;
using H.Core.Providers.Carbon;
using H.Core.Providers.Climate;
using H.Core.Services.LandManagement;

#endregion

namespace H.Core.Calculators.Carbon
{
    /// <summary>
    /// </summary>
    public partial class ICBMSoilCarbonCalculator : CarbonCalculatorBase, IICBMSoilCarbonCalculator
    {
        #region Fields

        private ICBMCarbonInputCalculator _inputCalculator;

        #endregion

        #region Constructors

        public ICBMSoilCarbonCalculator(IClimateProvider climateProvider, N2OEmissionFactorCalculator n2OEmissionFactorCalculator)
        {
            if (climateProvider != null)
            {
                _climateProvider = climateProvider;
            }
            else
            {
                throw new ArgumentNullException(nameof(climateProvider));
            }

            if (n2OEmissionFactorCalculator != null)
            {
                base.N2OEmissionFactorCalculator = n2OEmissionFactorCalculator;
            }
            else
            {
                throw new ArgumentNullException(nameof(n2OEmissionFactorCalculator));
            }

            _inputCalculator = new ICBMCarbonInputCalculator();
        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        /// <summary>
        /// Equation 2.2.2-27
        /// </summary>
        public double CalculateAmountOfNitrogenAppliedFromManure(
            double manureAmount, 
            double fractionOfNitrogenInAppliedManure)
        {
            return manureAmount * fractionOfNitrogenInAppliedManure;
        }

        /// <summary>
        /// Equation 2.2.2-28
        /// </summary>
        public double CalculateAmountOfPhosphorusAppliedFromManure(
            double manureAmount, 
            double fractionOfPhosphorusInAppliedManure)
        {
            return manureAmount * fractionOfPhosphorusInAppliedManure;
        }

        /// <summary>
        /// Equation 2.2.2-29
        /// </summary>
        public double CalculateMoistureOfManure(
            double manureAmount, 
            double waterFraction)
        {
            return manureAmount * waterFraction / 10000;
        }

        /// <summary>
        /// Equation 2.1.3-1
        /// </summary>
        public double CalculateAverageAboveGroundResidueCarbonInput(
            double carbonInputFromProductOfEachRotationPhase, 
            double carbonInputFromStrawOfEachRotationPhase)
        {
            return carbonInputFromProductOfEachRotationPhase + carbonInputFromStrawOfEachRotationPhase;
        }

        /// <summary>
        /// Equation 2.1.3-2
        /// </summary>
        public double CalculateAverageBelowGroundResidueCarbonInput(
            double carbonInputFromRootsOfEachRotationPhase, 
            double carbonInputFromExtrarootOfEachRotationPhase)
        {
            return carbonInputFromExtrarootOfEachRotationPhase + carbonInputFromRootsOfEachRotationPhase;
        }

        /// <summary>
        /// Equation 2.1.3-3
        /// </summary>
        public double CalculateAverageManureCarbonInput(double carbonInputsFromManureInputsOfEachRotationPhase)
        {
            return carbonInputsFromManureInputsOfEachRotationPhase;
        }

        /// <summary>
        /// Equation 2.1.3-4
        /// </summary>
        public double CalculateYoungPoolSteadyStateAboveGround(
            double averageAboveGroundCarbonInput, 
            double youngPoolDecompositionRate, 
            double climateParameter)
        {
            var numerator = averageAboveGroundCarbonInput * Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);
            var denominator = 1 - Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);

            var result = numerator / denominator;

            return result;
        }

        /// <summary>
        /// Equation 2.1.3-5
        /// </summary>
        public double CalculateYoungPoolSteadyStateBelowGround(
            double averageBelowGroundCarbonInput, 
            double youngPoolDecompositionRate, 
            double climateParameter)
        {
            var numerator = averageBelowGroundCarbonInput * Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);
            var denominator = 1 - Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);

            var result = numerator / denominator;

            return result;
        }

        /// <summary>
        /// Equation 2.1.3-6
        /// </summary>
        public double CalculateYoungPoolSteadyStateManure(
            double averageManureCarbonInput, 
            double youngPoolDecompositionRate, 
            double climateParameter)
        {
            var numerator = averageManureCarbonInput * Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);
            var denominator = 1 - Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);

            var result = numerator / denominator;

            return result;
        }

        /// <summary>
        /// Equation 2.1.3-7
        /// </summary>
        public double CalculateOldPoolSteadyState(
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
            double manureHumificationCoefficient)
        {
            var firstFactorNumerator = Math.Exp(-1 * youngPoolDecompositionRate * climateParameter) - Math.Exp(-1 * oldPoolDecompositionRate * climateParameter);
            var firstFactorDenominator = 1 - Math.Exp(-1 * oldPoolDecompositionRate * climateParameter);

            var secondFactorNumeratorFactorOne = aboveGroundHumificationCoefficient * youngPoolDecompositionRate;
            var secondFactorNumeratorFactorTwo = aboveGroundYoungPoolSteadyState + averageAboveGroundCarbonInputOfRotation;

            var secondFactorNumeratorFactorThree = belowGroundHumificationCoefficient * youngPoolDecompositionRate;
            var secondFactorNumeratorFactorFour = belowGroundYoungPoolSteadyState + averageBelowGroundCarbonInputOfRotation;

            var secondFactorNumeratorFactorFive = manureHumificationCoefficient * youngPoolDecompositionRate;
            var secondFactorNumeratorFactorSix = manureYoungPoolSteadyState + averageManureCarbonInputOfRotation;

            var secondFactorNumerator = secondFactorNumeratorFactorOne * secondFactorNumeratorFactorTwo +
                                        secondFactorNumeratorFactorThree * secondFactorNumeratorFactorFour +
                                        secondFactorNumeratorFactorFive * secondFactorNumeratorFactorSix;

            var secondFactorDenominator = oldPoolDecompositionRate - youngPoolDecompositionRate;

            var result = (firstFactorNumerator / firstFactorDenominator) * (secondFactorNumerator / secondFactorDenominator);

            return result;
        }


        /// <summary>
        /// Equation 2.1.3-11
        /// </summary>
        public double CalculateYoungPoolAboveGroundCarbonAtInterval(
            double youngPoolAboveGroundCarbonAtPreviousInterval, 
            double aboveGroundCarbonAtPreviousInterval, 
            double youngPoolDecompositionRate, 
            double climateParameter)
        {
            var firstFactor = youngPoolAboveGroundCarbonAtPreviousInterval + aboveGroundCarbonAtPreviousInterval;
            var secondFactor = Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);

            var result = firstFactor * secondFactor;

            return result;
        }

        /// <summary>
        /// Equation 2.1.3-12
        /// </summary>
        public double CalculateYoungPoolBelowGroundCarbonAtInterval(
            double youngPoolBelowGroundCarbonAtPreviousInterval, 
            double belowGroundCarbonAtPreviousInterval, 
            double youngPoolDecompositionRate, 
            double climateParameter)
        {
            var firstFactor = youngPoolBelowGroundCarbonAtPreviousInterval + belowGroundCarbonAtPreviousInterval;
            var secondFactor = Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);

            var result = firstFactor * secondFactor;

            return result;
        }

        /// <summary>
        /// Equation 2.1.3-13
        /// </summary>
        public double CalculateYoungPoolManureCarbonAtInterval(
            double youngPoolManureCarbonAtPreviousInterval, 
            double manureCarbonInputAtPreviousInterval,
            double youngPoolDecompositionRate, 
            double climateParameter)
        {
            var firstFactor = youngPoolManureCarbonAtPreviousInterval + manureCarbonInputAtPreviousInterval;
            var secondFactor = Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);

            var result = firstFactor * secondFactor;

            return result;
        }

        /// <summary>
        /// Equation 2.1.3-14
        /// </summary>
        /// <returns></returns>
        public double CalculateOldPoolSoilCarbonAtInterval(
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
            double manureCarbonInputAtPreviousInterval)
        {
            var decompositionRateDifference = oldPoolDecompositionRate - youngPoolDecompositionRate;

            var aboveGroundDivisionTermNumerator = youngPoolDecompositionRate *
                                                   (youngPoolAboveGroundOrganicCarbonAtPreviousInterval +
                                                    aboveGroundCarbonResidueAtPreviousInterval); 
            var aboveGroundDivisionTerm = aboveGroundHumificationCoefficient *
                                          (aboveGroundDivisionTermNumerator / decompositionRateDifference);

            var belowGroundDivisionTermNumerator = youngPoolDecompositionRate *
                                                   (youngPoolBelowGroundOrganicCarbonAtPreviousInterval +
                                                    belowGroundCarbonResidueAtPreviousInterval);
            var belowGroundDivisionTerm = belowGroundHumificationCoefficient *
                                          (belowGroundDivisionTermNumerator / decompositionRateDifference);

            var manureDivisionTermNumerator = youngPoolDecompositionRate *
                                              (youngPoolManureAtPreviousInterval + manureCarbonInputAtPreviousInterval);
            var manureDivisionTerm = manureHumificationCoefficient *
                                     (manureDivisionTermNumerator / decompositionRateDifference);

            var firstTerm =
                (oldPoolSoilCarbonAtPreviousInterval - aboveGroundDivisionTerm - belowGroundDivisionTerm -
                 manureDivisionTerm) * Math.Exp(-1 * oldPoolDecompositionRate * climateParameter);
            var secondTerm = aboveGroundDivisionTerm * Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);
            var thirdTerm = belowGroundDivisionTerm * Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);
            var fourthTerm = manureDivisionTerm * Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);

            var result = firstTerm + secondTerm + thirdTerm + fourthTerm;

            return result;
        }

        /// <summary>
        /// Equation 2.1.3-15
        /// </summary>
        public double CalculateSoilCarbonAtInterval(
            double youngPoolSoilCarbonAboveGroundAtInterval, 
            double youngPoolSoilCarbonBelowGroundAtInterval, 
            double oldPoolSoilCarbonAtInterval, 
            double youngPoolManureAtInterval)
        {
            return youngPoolSoilCarbonAboveGroundAtInterval + youngPoolSoilCarbonBelowGroundAtInterval + oldPoolSoilCarbonAtInterval + youngPoolManureAtInterval;
        }

        /// <summary>
        /// Equation 2.1.3-16
        /// </summary>
        public double CalculateChangeInSoilCarbonAtInterval(
            double soilOrganicCarbonAtInterval, 
            double soilOrganicCarbonAtPreviousInterval)
        {
            return soilOrganicCarbonAtInterval - soilOrganicCarbonAtPreviousInterval;
        }

        /// <summary>
        /// Equation 2.1.3-17
        /// </summary>
        public double CalculateChangeInSoilOrganicCarbonForFieldAtInterval(
            double changeInSoilOrganicCarbonAtInterval, 
            double fieldArea)
        {
            return changeInSoilOrganicCarbonAtInterval * fieldArea;
        }

        /// <summary>
        /// Equation 2.1.3-18
        /// </summary>
        public double CalculateChangeInSoilOrganicCarbonForFarmAtInterval(
            IEnumerable<double> changeInSoilOrganicCarbonForFields)
        {
            return changeInSoilOrganicCarbonForFields.Sum();
        }

        /// <summary>
        /// Equation 2.1.4-1
        /// </summary>
        public double CalculateCarbonDioxideEquivalentsForSoil(double soilOrganicCarbonAtInterval)
        {
            var carbonDioxideEquivalentForSoil = soilOrganicCarbonAtInterval * (44.0 / 12.0);
            return carbonDioxideEquivalentForSoil;
        }

        /// <summary>
        /// Equation 2.1.4-2
        /// </summary>
        public double CalculateChangeInCarbonDioxideEquivalentsForSoil(double changeInSoilOrganicCarbonAtInterval)
        {
            var changeInCarbonDioxideEquivalentsForSoil = changeInSoilOrganicCarbonAtInterval * (44.0 / 12.0);
            return changeInCarbonDioxideEquivalentsForSoil;
        }

        /// <summary>
        /// Equation 2.1.4-3
        /// </summary>
        public double CalculateCarbonDioxideChangeForSoilsByMonth(double changeInCarbonDioxideEquivalentsForSoil)
        {
            var carbonDioxideChangeForSoilByMonth = changeInCarbonDioxideEquivalentsForSoil / 12.0;
            return carbonDioxideChangeForSoilByMonth;
        }

        /// <summary>
        /// Equation 11.3.2-4
        /// </summary>
        /// <returns>Total carbon losses from grazing animals (kg C)</returns>
        public double RecalculatePlantCarbonForGrazingScenario(
            CropViewItem viewItem)
        {
            var lossesFromGrazing = viewItem.TotalCarbonLossesByGrazingAnimals;

            var averageUtilizationRate = viewItem.GrazingViewItems.Any() ? viewItem.GrazingViewItems.Average(x => x.Utilization) : 0;
            var denominator = 1 - (averageUtilizationRate / 100.0);
            if (denominator < 0)
            {
                denominator = 1;
            }

            var uptake = lossesFromGrazing / denominator;

            return uptake;
        }

        public double RecalculateCarbonInputForGrazingScenario(
            CropViewItem viewItem)
        {
            // Check for negative values here

            var result = viewItem.PlantCarbonInAgriculturalProduct - (viewItem.TotalCarbonLossesByGrazingAnimals / viewItem.Area);
            if (result < 0)
            {
                return viewItem.PlantCarbonInAgriculturalProduct;
            }

            return result;
        }

        #endregion
    }
}