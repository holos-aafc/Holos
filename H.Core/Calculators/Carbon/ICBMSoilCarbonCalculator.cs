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

        public CropViewItem CalculateEquilibriumYear(
            List<CropViewItem> detailViewItems, Farm farm, 
            Guid componentId)
        {
            var fieldSystemComponent = farm.GetFieldSystemComponent(componentId);

            // Get the field system component that will be used to calculate the equilibrium year            
            var sizeOfFirstRotationForField =
                detailViewItems.OrderBy(viewItem => viewItem.Year).First().SizeOfFirstRotationForField;
            if (sizeOfFirstRotationForField == 0)
            {
                // Was not set during creation of old farm                
                sizeOfFirstRotationForField = fieldSystemComponent.SizeOfFirstRotationInField();
            }

            // Get the detail view items that represent the crops that define the first rotation
            var viewItemsInRotation = detailViewItems.Take(sizeOfFirstRotationForField).ToList();

            /*
             * Calculate averages for the crops that define the first rotation - this is used to calculate the equilibrium state
             *
             * Use a specified strategy to calculate these starting values
             */

            var equilibriumAboveGroundInput = 0d;
            var equilibriumBelowGroundInput = 0d;
            var equilibriumManureInput = 0d;
            var equilibriumClimateParameter = 0d;
            var equilibriumManagementFactor = 0d;
            var equilibriumDigestateInput = 0d;

            var equilibriumAboveGroundNitrogen = 0d;
            var equilibriumBelowGroundNitrogen = 0d;

            var strategy = farm.Defaults.EquilibriumCalculationStrategy;
            if (strategy == EquilibriumCalculationStrategies.CarMultipleYearAverage)
            {
                // Not implemented yet
            }
            else
            {
                // At this point, the detail view items have had their C inputs calculated
                // Equation 2.1.3-1
                equilibriumAboveGroundInput = viewItemsInRotation.Average(x => x.CombinedAboveGroundInput);

                // Equation 2.1.3-2
                equilibriumBelowGroundInput = viewItemsInRotation.Average(x => x.CombinedBelowGroundInput);

                // Equation 2.1.3-3
                equilibriumManureInput = viewItemsInRotation.Average(x => x.CombinedManureInput);
                equilibriumDigestateInput = viewItemsInRotation.Average(x => x.CombinedDigestateInput);

                equilibriumClimateParameter = viewItemsInRotation.Average(x => x.ClimateParameter);
                equilibriumManagementFactor = viewItemsInRotation.Average(x => x.ManagementFactor);

                equilibriumAboveGroundNitrogen = viewItemsInRotation.Average(x => x.CombinedAboveGroundResidueNitrogen);
                equilibriumBelowGroundNitrogen = viewItemsInRotation.Average(x => x.CombinedBelowGroundResidueNitrogen);
            }

            // This is the equilibrium year result
            var result = new CropViewItem();

            result.CombinedAboveGroundInput = equilibriumAboveGroundInput;
            result.CombinedBelowGroundInput = equilibriumBelowGroundInput;

            result.CombinedAboveGroundResidueNitrogen = equilibriumAboveGroundNitrogen;
            result.CombinedBelowGroundResidueNitrogen = equilibriumBelowGroundNitrogen;

            result.CombinedManureInput = equilibriumManureInput;
            result.CombinedDigestateInput = equilibriumDigestateInput;
            result.ClimateParameter = equilibriumClimateParameter;
            result.ManagementFactor = equilibriumManagementFactor;

            var averageNitrogenConcentrationInProduct = viewItemsInRotation.Select(x => x.NitrogenContentInProduct).Average();
            var averageNitrogenConcentrationInStraw = viewItemsInRotation.Select(x => x.NitrogenContentInStraw).Average();
            var averageNitrogenConcentrationInRoots = viewItemsInRotation.Select(x => x.NitrogenContentInRoots).Average();
            var averageNitrogenConcentrationInExtraroots = viewItemsInRotation.Select(x => x.NitrogenContentInExtraroot).Average();

            result.NitrogenContentInProduct = averageNitrogenConcentrationInProduct;
            result.NitrogenContentInStraw = averageNitrogenConcentrationInStraw;
            result.NitrogenContentInRoots = averageNitrogenConcentrationInRoots;
            result.NitrogenContentInExtraroot = averageNitrogenConcentrationInExtraroots;

            /*
             * Carbon
             */

            // The user can choose to use either the climate parameter or the management factor in the calculations
            var climateOrManagementFactor = farm.Defaults.UseClimateParameterInsteadOfManagementFactor
                ? result.ClimateParameter
                : result.ManagementFactor;

            if (farm.UseCustomStartingSoilOrganicCarbon == false)
            {
                result.YoungPoolSoilCarbonAboveGround =
                    this.CalculateYoungPoolSteadyStateAboveGround(
                        averageAboveGroundCarbonInput: result.CombinedAboveGroundInput,
                        youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool,
                        climateParameter: climateOrManagementFactor);
            }
            else
            {

                // Equation 2.1.3-8
                var youngPoolAboveGround = result.CombinedAboveGroundInput /
                                           (climateOrManagementFactor *
                                            farm.Defaults.DecompositionRateConstantYoungPool);

                result.YoungPoolSoilCarbonAboveGround = youngPoolAboveGround;
            }

            if (farm.UseCustomStartingSoilOrganicCarbon == false)
            {
                result.YoungPoolSoilCarbonBelowGround =
                    this.CalculateYoungPoolSteadyStateBelowGround(
                        averageBelowGroundCarbonInput: result.CombinedBelowGroundInput,
                        youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool,
                        climateParameter: climateOrManagementFactor);
            }
            else
            {
                // Equation 2.1.3-9
                var youngPoolBelowGround = result.CombinedBelowGroundInput /
                                           (climateOrManagementFactor *
                                            farm.Defaults.DecompositionRateConstantYoungPool);

                result.YoungPoolSoilCarbonBelowGround = youngPoolBelowGround;
            }

            result.YoungPoolManureCarbon = this.CalculateYoungPoolSteadyStateManure(
                averageManureCarbonInput: (result.CombinedManureInput + result.CombinedDigestateInput),
                youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool,
                climateParameter: climateOrManagementFactor);

            if (farm.UseCustomStartingSoilOrganicCarbon == false)
            {
                result.OldPoolSoilCarbon = this.CalculateOldPoolSteadyState(
                    youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool,
                    oldPoolDecompositionRate: farm.Defaults.DecompositionRateConstantOldPool,
                    climateParameter: climateOrManagementFactor,
                    aboveGroundHumificationCoefficient: farm.Defaults.HumificationCoefficientAboveGround,
                    belowGroundHumificationCoefficient: farm.Defaults.HumificationCoefficientBelowGround,
                    averageAboveGroundCarbonInputOfRotation: result.CombinedAboveGroundInput,
                    averageBelowGroundCarbonInputOfRotation: result.CombinedBelowGroundInput,
                    aboveGroundYoungPoolSteadyState: result.YoungPoolSoilCarbonAboveGround,
                    belowGroundYoungPoolSteadyState: result.YoungPoolSoilCarbonBelowGround,
                    manureYoungPoolSteadyState: result.YoungPoolSteadyStateManure,
                    averageManureCarbonInputOfRotation: (result.CombinedManureInput + result.CombinedDigestateInput),
                    manureHumificationCoefficient: farm.Defaults.HumificationCoefficientManure);
            }
            else
            {
                // Equation 2.1.3-10
                result.OldPoolSoilCarbon = farm.StartingSoilOrganicCarbon -
                                           (result.YoungPoolSoilCarbonAboveGround +
                                            result.YoungPoolSoilCarbonBelowGround);
            }

            result.SoilCarbon = this.CalculateSoilCarbonAtInterval(
                youngPoolSoilCarbonAboveGroundEndOfYearAtInterval: result.YoungPoolSoilCarbonAboveGround,
                youngPoolSoilCarbonBelowGroundEndOfYearAtInterval: result.YoungPoolSoilCarbonBelowGround,
                oldPoolSoilCarbonAtInterval: result.OldPoolSoilCarbon,
                youngPoolManureEndOfYearAtInterval: result.YoungPoolManureCarbon);

            return result;
        }

        /// <summary>
        /// Equation 2.1.3-15
        /// </summary>
        /// <param name="youngPoolSoilCarbonAbovegroundAtStartOfYear">Young pool soil organic C - aboveground at the beginning of the year (kg C ha^-1)</param>
        /// <param name="aboveGroundResidueCarbonInput">Aboveground residue C input (kg C ha^-1)</param>
        /// <returns>Young pool soil organic C - aboveground at the end of the year with fresh residues included</returns>
        public double CalculateYoungPoolCarbonAbovegroundEndOfYear(
            double youngPoolSoilCarbonAbovegroundAtStartOfYear, 
            double aboveGroundResidueCarbonInput)
        {
            return youngPoolSoilCarbonAbovegroundAtStartOfYear + aboveGroundResidueCarbonInput;
        }

        /// <summary>
        /// Equation 2.1.3-16
        /// </summary>
        /// <param name="youngPoolSoilCarbonBelowgroundAtStartOfYear">Young pool soil organic C - belowground at the beginning of the year (kg C ha^-1)</param>
        /// <param name="belowGroundResidueCarbonInput">Belowground residue C input (kg C ha^-1)</param>
        /// <returns>Young pool soil organic C - belowground at the end of the year with fresh residues included</returns>
        public double CalculateYoungPoolCarbonBelowgroundEndOfYear(
            double youngPoolSoilCarbonBelowgroundAtStartOfYear,
            double belowGroundResidueCarbonInput)
        {
            return youngPoolSoilCarbonBelowgroundAtStartOfYear + belowGroundResidueCarbonInput;
        }

        /// <summary>
        /// Equation 2.1.3-17
        /// </summary>
        /// <param name="youngPoolManureAtStartOfYear">Young pool soil organic C - manure at the beginning of the year (kg C ha^-1)</param>
        /// <param name="combinedManureInputs"></param>
        /// <returns>/// <returns>Young pool soil organic C - manure at the end of the year with fresh residues included</returns></returns>
        public double CalculateYoungPoolCarbonManureEndOfYear(double youngPoolManureAtStartOfYear,
            double combinedManureInputs)
        {
            return youngPoolManureAtStartOfYear +  combinedManureInputs;
        }

        /// <summary>
        /// Equation 2.1.3-18
        /// </summary>
        public double CalculateSoilCarbonAtInterval(
            double youngPoolSoilCarbonAboveGroundEndOfYearAtInterval, 
            double youngPoolSoilCarbonBelowGroundEndOfYearAtInterval, 
            double oldPoolSoilCarbonAtInterval, 
            double youngPoolManureEndOfYearAtInterval)
        {
            return youngPoolSoilCarbonAboveGroundEndOfYearAtInterval + youngPoolSoilCarbonBelowGroundEndOfYearAtInterval + oldPoolSoilCarbonAtInterval + youngPoolManureEndOfYearAtInterval;
        }

        /// <summary>
        /// Equation 2.1.3-19
        /// </summary>
        public double CalculateChangeInSoilCarbonAtInterval(
            double soilOrganicCarbonAtInterval, 
            double soilOrganicCarbonAtPreviousInterval)
        {
            return soilOrganicCarbonAtInterval - soilOrganicCarbonAtPreviousInterval;
        }

        /// <summary>
        /// Equation 2.1.3-20
        /// </summary>
        public double CalculateChangeInSoilOrganicCarbonForFieldAtInterval(
            double changeInSoilOrganicCarbonAtInterval, 
            double fieldArea)
        {
            return changeInSoilOrganicCarbonAtInterval * fieldArea;
        }

        /// <summary>
        /// Equation 2.1.3-21
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

        public void CalculateCarbonAtInterval(
            CropViewItem previousYearResults, 
            CropViewItem currentYearResults, 
            Farm farm)
        {
            // The user can choose to use either the climate parameter or the management factor in the calculations
            var climateParameterOrManagementFactor = farm.Defaults.UseClimateParameterInsteadOfManagementFactor
                ? currentYearResults.ClimateParameter
                : currentYearResults.ManagementFactor;

            currentYearResults.YoungPoolSoilCarbonAboveGround = this.CalculateYoungPoolAboveGroundCarbonAtInterval(
                    youngPoolAboveGroundCarbonAtPreviousInterval: previousYearResults.YoungPoolSoilCarbonAboveGround,
                    aboveGroundCarbonAtPreviousInterval: previousYearResults.CombinedAboveGroundInput,
                    youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool,
                    climateParameter: climateParameterOrManagementFactor);

            currentYearResults.YoungPoolSoilCarbonAboveGroundEndOfYear = this.CalculateYoungPoolCarbonAbovegroundEndOfYear(
                youngPoolSoilCarbonAbovegroundAtStartOfYear: currentYearResults.YoungPoolSoilCarbonAboveGround,
                aboveGroundResidueCarbonInput: currentYearResults.CombinedAboveGroundInput);

            currentYearResults.YoungPoolSoilCarbonBelowGround = this.CalculateYoungPoolBelowGroundCarbonAtInterval(
                    youngPoolBelowGroundCarbonAtPreviousInterval: previousYearResults.YoungPoolSoilCarbonBelowGround,
                    belowGroundCarbonAtPreviousInterval: previousYearResults.CombinedBelowGroundInput,
                    youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool,
                    climateParameter: climateParameterOrManagementFactor);

            currentYearResults.YoungPoolSoilCarbonBelowGroundEndOfYear = this.CalculateYoungPoolCarbonBelowgroundEndOfYear(
                youngPoolSoilCarbonBelowgroundAtStartOfYear: currentYearResults.YoungPoolSoilCarbonBelowGround,
                belowGroundResidueCarbonInput: currentYearResults.CombinedBelowGroundInput);

            currentYearResults.YoungPoolManureCarbon = this.CalculateYoungPoolManureCarbonAtInterval(
                    youngPoolManureCarbonAtPreviousInterval: previousYearResults.YoungPoolManureCarbon,
                    manureCarbonInputAtPreviousInterval: previousYearResults.CombinedManureAndDigestateInput,
                    youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool,
                    climateParameter: climateParameterOrManagementFactor);

            currentYearResults.YoungPoolManureCarbonEndOfYear = this.CalculateYoungPoolCarbonManureEndOfYear(
                youngPoolManureAtStartOfYear: currentYearResults.YoungPoolManureCarbon,
                combinedManureInputs: currentYearResults.CombinedManureAndDigestateInput);

            currentYearResults.OldPoolSoilCarbon = this.CalculateOldPoolSoilCarbonAtInterval(
                oldPoolSoilCarbonAtPreviousInterval: previousYearResults.OldPoolSoilCarbon,
                aboveGroundHumificationCoefficient: farm.Defaults.HumificationCoefficientAboveGround,
                belowGroundHumificationCoefficient: farm.Defaults.HumificationCoefficientBelowGround,
                youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool,
                oldPoolDecompositionRate: farm.Defaults.DecompositionRateConstantOldPool,
                youngPoolAboveGroundOrganicCarbonAtPreviousInterval: previousYearResults.YoungPoolSoilCarbonAboveGround,
                youngPoolBelowGroundOrganicCarbonAtPreviousInterval: previousYearResults.YoungPoolSoilCarbonBelowGround,
                aboveGroundCarbonResidueAtPreviousInterval: previousYearResults.CombinedAboveGroundInput,
                belowGroundCarbonResidueAtPreviousInterval: previousYearResults.CombinedBelowGroundInput,
                climateParameter: climateParameterOrManagementFactor,
                youngPoolManureAtPreviousInterval: previousYearResults.YoungPoolManureCarbon,
                manureHumificationCoefficient: farm.Defaults.HumificationCoefficientManure,
                manureCarbonInputAtPreviousInterval: previousYearResults.CombinedManureAndDigestateInput);

            //currentYearResults.SoilCarbon = this.CalculateSoilCarbonAtInterval(
            //    youngPoolSoilCarbonAboveGroundEndOfYearAtInterval: currentYearResults.YoungPoolSoilCarbonAboveGround,
            //    youngPoolSoilCarbonBelowGroundEndOfYearAtInterval: currentYearResults.YoungPoolSoilCarbonBelowGround,
            //    oldPoolSoilCarbonAtInterval: currentYearResults.OldPoolSoilCarbon,
            //    youngPoolManureEndOfYearAtInterval: currentYearResults.YoungPoolManureCarbon);

            currentYearResults.SoilCarbon = this.CalculateSoilCarbonAtInterval(
                youngPoolSoilCarbonAboveGroundEndOfYearAtInterval: currentYearResults.YoungPoolSoilCarbonAboveGroundEndOfYear,
                youngPoolSoilCarbonBelowGroundEndOfYearAtInterval: currentYearResults.YoungPoolSoilCarbonBelowGroundEndOfYear,
                oldPoolSoilCarbonAtInterval: currentYearResults.OldPoolSoilCarbon,
                youngPoolManureEndOfYearAtInterval: currentYearResults.YoungPoolManureCarbonEndOfYear);

            currentYearResults.ChangeInCarbon = this.CalculateChangeInSoilCarbonAtInterval(
                soilOrganicCarbonAtInterval: currentYearResults.SoilCarbon,
                soilOrganicCarbonAtPreviousInterval: previousYearResults.SoilCarbon);

            // If there is a measured value, then use the calculated Y_ag, Y_bg, and O pool values for the current year (using 2.1.3-8, 2.1.3-9, and 2.1.3-10)
            if (previousYearResults.Year == 0 && farm.UseCustomStartingSoilOrganicCarbon)
            {
                currentYearResults.YoungPoolSoilCarbonAboveGround = previousYearResults.YoungPoolSoilCarbonAboveGround;
                currentYearResults.YoungPoolSoilCarbonBelowGround = previousYearResults.YoungPoolSoilCarbonBelowGround;
                currentYearResults.YoungPoolManureCarbon = previousYearResults.YoungPoolManureCarbon;
                currentYearResults.OldPoolSoilCarbon = previousYearResults.OldPoolSoilCarbon;
                currentYearResults.SoilCarbon = farm.StartingSoilOrganicCarbon;
                currentYearResults.ChangeInCarbon = 0;
            }
        }

        #endregion
    }
}