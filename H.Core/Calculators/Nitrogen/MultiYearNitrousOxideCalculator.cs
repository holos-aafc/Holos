#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Navigation;
using H.Core.Models.LandManagement.Fields;

#endregion

namespace H.Core.Calculators.Nitrogen
{
    public class MultiYearNitrousOxideCalculator
    {
        #region Fields

        private readonly SingleYearNitrousOxideCalculator _singleYearNitrousOxideCalculator = new SingleYearNitrousOxideCalculator();        

        #endregion

        #region Constructors

        public MultiYearNitrousOxideCalculator()
        {            
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Equation 2.6.1-1
        /// </summary>
        public double CalculateSyntheticNitrogenFromFertilizerApplication(
            double syntheticFertilizerApplied,
            double fieldArea)
        {
            return syntheticFertilizerApplied * fieldArea;
        }

        /// <summary>
        /// Equation 2.6.1-2
        /// </summary>
        public double CalculateSyntheticNitrogenFromDeposition(double nitrogenDeposition, double fieldArea)
        {
            return nitrogenDeposition * fieldArea;
        }

        /// <summary>
        /// Equation 2.6.2-1
        /// </summary>
        public double CalculateAboveGroundResidueNitrogenAtEquilibrium(
            double carbonInputFromProduct,
            double nitrogenConcentrationInProduct,
            double carbonInputFromStraw,
            double nitrogenConcentrationInStraw,
            double climateFactor,
            double youngPoolDecompositionRate)
        {
            return ((carbonInputFromProduct / 0.45 * nitrogenConcentrationInProduct +
                    carbonInputFromStraw / 0.45 * nitrogenConcentrationInStraw) *
                   Math.Exp(-1 * youngPoolDecompositionRate * climateFactor) ) /

                   (1 - Math.Exp(-1 * youngPoolDecompositionRate * climateFactor));
        }

        /// <summary>
        /// Equation 2.6.2-3
        /// </summary>
        public double CalculateAboveGroundResidueNitrogenForFieldAtInterval(
            double aboveGroundResidueNitrogenForFieldAtPreviousInterval,
            double aboveGroundResidueNitrogenForCropAtPreviousInterval,
            double climateManagementFactor,
            double decompositionRateYoungPool)
        {            
            var result = (aboveGroundResidueNitrogenForFieldAtPreviousInterval + aboveGroundResidueNitrogenForCropAtPreviousInterval) * Math.Exp((-1) * decompositionRateYoungPool * climateManagementFactor);

            return result;
        }

        /// <summary>
        /// Equation 2.6.2-4
        /// </summary>
        public double CalculateBelowGroundResidueNitrogenAtEquilibrium(
            double carbonInputFromRoots,
            double nitrogenConcentrationInRoots,
            double carbonInputFromExtraroots,
            double nitrogenConcentrationInExtraroots,
            double youngPoolDecompositionRate,
            double climateFactor)
        {
            return ((carbonInputFromRoots / 0.45 * nitrogenConcentrationInRoots +
                    carbonInputFromExtraroots / 0.45 * nitrogenConcentrationInExtraroots) *
                   Math.Exp(-1 * youngPoolDecompositionRate * climateFactor)) /

                   (1 - Math.Exp(-1 * youngPoolDecompositionRate * climateFactor));
        }


        /// <summary>
        /// Equation 2.6.2-6
        /// </summary>
        public double CalculateBelowGroundResidueNitrogenForFieldAtInterval(
            double belowGroundResidueNitrogenForFieldAtPreviousInterval,
            double belowGroundResidueNitrogenForCropAtPreviousInterval,
            double climateManagementFactor,
            double decompositionRateYoungPool)
        {            
            var result = (belowGroundResidueNitrogenForFieldAtPreviousInterval + belowGroundResidueNitrogenForCropAtPreviousInterval) * Math.Exp((-1) * decompositionRateYoungPool * climateManagementFactor);

            return result;
        }

        /// <summary>
        /// Equation 2.6.2-7
        /// </summary>
        public double CalculateManureResidueNitrogenPool(double manureResidueNitrogenPoolAtPreviousInterval,
                                                         double amountOfManureAppliedAtPreviousInterval,
                                                         double decompositionRateYoungPool,
                                                         double climateParameter)
        {
           var result = (manureResidueNitrogenPoolAtPreviousInterval + amountOfManureAppliedAtPreviousInterval) * Math.Exp((-1) * decompositionRateYoungPool * climateParameter);

            return result;
        }

        /// <summary>
        /// Equation 2.6.3-1
        /// </summary>
        public double CalculateCropResiduesAtStartingPoint(
            double aboveGroundResidueNitrogenForCropAtStartingPoint,
            double belowGroundResidueNitrogenForCropAtStartingPoint,
            double decompositionRateConstantYoungPool,
            double climateParameter)
        {
            var result = (aboveGroundResidueNitrogenForCropAtStartingPoint + belowGroundResidueNitrogenForCropAtStartingPoint) -
                         (aboveGroundResidueNitrogenForCropAtStartingPoint + belowGroundResidueNitrogenForCropAtStartingPoint) * Math.Exp((-1) * decompositionRateConstantYoungPool * climateParameter);

            return result;
        }

        /// <summary>
        /// Equation 2.6.3-2
        /// </summary>
        public double CalculateCropResiduesAtInterval(
            double aboveGroundResidueNitrogenForFieldAtCurrentInterval,
            double aboveGroundResidueNitrogenForFieldAtPreviousInterval,                        
            double aboveGroundResidueNitrogenForCropAtPreviousInterval,
            double belowGroundResidueNitrogenForFieldAtCurrentInterval,            
            double belowGroundResidueNitrogenForFieldAtPreviousInterval,
            double belowGroundResidueNitrogenForCropAtPreviousInterval)
        {
            var result = ((aboveGroundResidueNitrogenForFieldAtPreviousInterval + aboveGroundResidueNitrogenForCropAtPreviousInterval) - aboveGroundResidueNitrogenForFieldAtCurrentInterval) +
                         ((belowGroundResidueNitrogenForFieldAtPreviousInterval + belowGroundResidueNitrogenForCropAtPreviousInterval) - belowGroundResidueNitrogenForFieldAtCurrentInterval);

            return result;
        }

        /// <summary>
        /// Equation 2.6.3-3
        /// </summary>
        public double CalculateAvailabilityOfNitrogenFromManureDecompositionAtStartingPoint(
            double manureResiduePoolAtEquilibrium,
            double decompositionRateConstantYoungPool,
            double climateParameter)
        {
            // TODO: double check that ManureN_crop is the same as ManureN_field at this stage
            var result = manureResiduePoolAtEquilibrium - (manureResiduePoolAtEquilibrium * Math.Exp(-1 * decompositionRateConstantYoungPool * climateParameter));

            return result;
        }

        /// <summary>
        /// Equation 2.6.3-4
        /// </summary>
        public double CalculateAvailabilityOfNitrogenFromManureDecompositionAtInterval(
            double manureResiduePoolAtPreviousInterval,
            double manureResiduePoolAtCurrentInterval,
            double amountOfManureAppliedAtPreviousInterval)
        {
            var result = (manureResiduePoolAtPreviousInterval + amountOfManureAppliedAtPreviousInterval) - manureResiduePoolAtCurrentInterval;

            return result;
        }

        /// <summary>
        /// Equation 2.6.3-5
        /// </summary>
        public double CalculateMineralizedNitrogenFromDecompositionOfOldCarbon(
            double oldPoolSoilCarbonAtPreviousInterval,
            double oldPoolDecompositionRate,
            double climateParameter,
            double oldCarbonNitrogen)
        {
            var result = (oldPoolSoilCarbonAtPreviousInterval - (oldPoolSoilCarbonAtPreviousInterval * Math.Exp((-1) * climateParameter * oldPoolDecompositionRate))) * oldCarbonNitrogen;

            return result;
        }

        /// <summary>
        /// Equation 2.6.4-1
        /// </summary>
        public double CalculateNitrousOxideEmissionsFromFertilizerApplication(
            double nitrogenInputsFromSyntheticFertilizer,
            double emissionFactor)
        {
            return nitrogenInputsFromSyntheticFertilizer * emissionFactor;
        }

        /// <summary>
        /// Equation 2.6.4-2
        /// </summary>
        public double CalculateNitrousOxideEmissionsFromCropResidues(
            double nitrogenInputsFromResidues,
            double emissionFactor)
        {
            return nitrogenInputsFromResidues * emissionFactor;
        }

        /// <summary>
        /// Equation 2.6.4-3
        /// </summary>
        public double CalculateNitrousOxideEmissionsFromExportedCropResidues(
            double nitrogenInputsFromExportedResidues,
            double emissionFactor)
        {
            return nitrogenInputsFromExportedResidues * emissionFactor;
        }

        /// <summary>
        /// Equation 2.6.4-4
        /// </summary>
        public double CalculateNitrousOxideEmissionsFromNitrogenMineralization(
            double nitrogenInputsFromMineralizedNitrogen,
            double emissionFactor)
        {
            return nitrogenInputsFromMineralizedNitrogen * emissionFactor;
        }

        /// <summary>
        /// Equation 2.6.4-5
        /// </summary>
        public double CalculateNitrousOxideEmissionsFromManureApplication(
            double nitrogenInputsFromManureApplication,
            double emissionFactor)
        {
            return nitrogenInputsFromManureApplication * emissionFactor;
        }

        /// <summary>
        /// Equation 2.6.4-6
        /// </summary>
        public double CalculateNitrousOxideEmissionsFromManureApplicationExportedFromFarm(
            double nitrogenInputsFromManureApplicationExportedFromFarm,
            double emissionFactor)
        {
            return nitrogenInputsFromManureApplicationExportedFromFarm * emissionFactor;
        }

        /// <summary>
        /// Equation 2.6.4-7
        /// </summary>
        public double CalculateNitricOxideEmissionsFromFertilizerApplication(
            double nitrousOxideInputsFromSyntheticFertilizer,
            double nitricOxideRatio)
        {
            return nitrousOxideInputsFromSyntheticFertilizer * nitricOxideRatio;
        }

        /// <summary>
        /// Equation 2.6.4-8
        /// </summary>
        public double CalculateNitricOxideEmissionsFromCropResidues(
            double nitrousOxideInputsFromResidues,
            double nitricOxideRatio)
        {
            return nitrousOxideInputsFromResidues * nitricOxideRatio;
        }

        /// <summary>
        /// Equation 2.6.4-9
        /// </summary>
        public double CalculateNitricOxideEmissionsFromExportedCropResidues(
            double nitrousOxideInputsFromExportedResidues,
            double nitricOxideRatio)
        {
            return nitrousOxideInputsFromExportedResidues * nitricOxideRatio;
        }

        /// <summary>
        /// Equation 2.6.4-10
        /// </summary>
        public double CalculateNitricOxideEmissionsFromNitrogenMineralization(
            double nitrousOxideInputsFromMineralizedNitrogen,
            double nitricOxideRatio)
        {
            return nitrousOxideInputsFromMineralizedNitrogen * nitricOxideRatio;
        }

        /// <summary>
        /// Equation 2.6.4-11
        /// </summary>
        public double CalculateNitricOxideEmissionsFromManureApplication(
            double nitrousOxideInputsFromManureApplication,
            double nitricOxideRatio)
        {
            return nitrousOxideInputsFromManureApplication * nitricOxideRatio;
        }

        /// <summary>
        /// Equation 2.6.4-12
        /// </summary>
        public double CalculateNitricOxideEmissionsFromManureApplicationExportedFromFarm(
            double nitrousOxideInputsFromManureApplicationExportedFromFarm,
            double nitricOxideRatio)
        {
            return nitrousOxideInputsFromManureApplicationExportedFromFarm * nitricOxideRatio;
        }             

        /// <summary>
        /// Equation 2.6.5-1
        /// Equation 2.6.5-2
        /// </summary>
        public double CalculateFractionOfNitrogenLostByLeachingAndRunoff(
            double growingSeasonPrecipitation,
            double growingSeasonEvapotranspiration)
        {
            var result = 0.3247 * growingSeasonPrecipitation / growingSeasonEvapotranspiration - 0.0247;

            if (result > 0.3)
            {
                return 0.3;
            }

            if (result < 0.05)
            {
                return 0.05;
            }

            return result;
        }

        /// <summary>
        /// Equation 2.6.5-3
        /// </summary>
        public double CalculateIndirectNitrousOxideEmissionsFromLeachingAndRunoffFromSyntheticFertilizer(
            double nitrogenInputsFromSyntheticFertilizer,
            double fractionOfNitrogenLostByLeachingAndRunoff,
            double emissionFactorForLeachingAndRunoff)
        {
            return nitrogenInputsFromSyntheticFertilizer * 
                   fractionOfNitrogenLostByLeachingAndRunoff *
                   emissionFactorForLeachingAndRunoff;
        }

        /// <summary>
        /// Equation 2.6.5-4
        /// </summary>
        public double CalculateIndirectNitrousOxideEmissionsFromLeachingAndRunoffOfCropResidues(
            double nitrogenInputsFromCropResidues,
            double fractionOfNitrogenLostByLeachingAndRunoff,
            double emissionFactorForLeachingAndRunoff)
        {
            return nitrogenInputsFromCropResidues * 
                   fractionOfNitrogenLostByLeachingAndRunoff *
                   emissionFactorForLeachingAndRunoff;
        }

        /// <summary>
        /// Equation 2.6.5-5
        /// </summary>
        public double CalculateNitrousOxideEmissionsDueToLeachingAndRunoffOfMineralizedNitrogen(
            double nitrogenInputsFromMineralizedNitrogen,
            double fractionOfNitrogenLostByLeachingAndRunoff,
            double emissionFactorForVolatilization)
        {
            return nitrogenInputsFromMineralizedNitrogen * 
                   fractionOfNitrogenLostByLeachingAndRunoff * 
                   emissionFactorForVolatilization;
        }

        /// <summary>
        /// Equation 2.6.5-6
        /// </summary>
        public double CalculateNitrousOxideEmissionsDueToOrganicNitrogen(
            double nitrogenInutsFromOrganicNitrogen,
            double fractionOfNitrogenLostByLeachingAndRunoff,
            double emissionFactorForVolatilization)
        {
            return nitrogenInutsFromOrganicNitrogen * 
                   fractionOfNitrogenLostByLeachingAndRunoff *
                   emissionFactorForVolatilization;
        }

        /// <summary>
        /// Equation 2.6.5-7
        /// </summary>
        public double CalculateActualAmountOfNitrogenLeachedFromSyntheticFertilizer(
            double nitrogenInputsFromSyntheticFertilizer,
            double fractionOfNitrogenLostByLeaching,
            double emissionFactorForLeaching)
        {
            return nitrogenInputsFromSyntheticFertilizer *
                   fractionOfNitrogenLostByLeaching *
                   (1 - emissionFactorForLeaching);
        }

        /// <summary>
        /// Equation 2.6.5-8
        /// </summary>
        public double CalculateActualAmountOfNitrogenLeachedFromResidues(
            double availabilityOfCropResiduesOnField,
            double fractionOfNitrogenLostByLeaching,
            double emissionFactorForLeaching)
        {
            return availabilityOfCropResiduesOnField *
                   fractionOfNitrogenLostByLeaching *
                   (1 - emissionFactorForLeaching);
        }

        /// <summary>
        /// Equation 2.6.5-9
        /// </summary>
        public double CalculateActualAmountOfNitrogenLeachedFromMineralizedNitrogen(
            double availabilityOfMineralizedNitrogenOnField,
            double fractionOfNitrogenLostByLeaching,
            double emissionFactorForLeaching)
        {
            return availabilityOfMineralizedNitrogenOnField *
                   fractionOfNitrogenLostByLeaching *
                   (1 - emissionFactorForLeaching);
        }

        /// <summary>
        /// Equation 2.6.5-10
        /// </summary>
        public double CalculateActualAmountOfNitrogenLeachedFromOrganicNitrogen(
            double nitrogenInutsFromOrganicNitrogen,
            double fractionOfNitrogenLostByLeaching,
            double emissionFactorForLeaching)
        {
            return nitrogenInutsFromOrganicNitrogen *
                   fractionOfNitrogenLostByLeaching *
                   (1 - emissionFactorForLeaching);
        }

        /// <summary>
        /// Equation 2.6.5-12
        /// </summary>
        public double CalculateActualAmountOfNitrogenLeachedFromMineralizedNitrogenAtInterval(
            double mineralNitrogenPoolAtPreviousInterval,
            double fractionOfNitrogenLostLeaching,
            double emissionFactorForLeaching)
        {
            return mineralNitrogenPoolAtPreviousInterval *
                   fractionOfNitrogenLostLeaching *
                   (1 - emissionFactorForLeaching);
        }

        /// <summary>
        /// Equation 2.6.5-11
        /// </summary>
        public double CalculateIndirectNitrousOxideEmissionsBasedOnAmountOfNitrogenVolatilized(
            double nitrogenInputsFromSyntheticFertilizer,
            double fractionOfNitrogenLostByVolatilization,
            double emissionFactorVolatilization)
        {
            return nitrogenInputsFromSyntheticFertilizer * fractionOfNitrogenLostByVolatilization * emissionFactorVolatilization;
        }

        /// <summary>
        /// Equation 2.6.5-13
        /// </summary>
        public double CalculateAmoniaEmissionFromSyntheticFertilizer(
            double nitrogenInputsFromSyntheticFertilizer,
            double fractionOfNitrogenLostByVolatilization,
            double emissionFactorVolatilization)
        {
            return nitrogenInputsFromSyntheticFertilizer * fractionOfNitrogenLostByVolatilization *
                   (1 - emissionFactorVolatilization);
        }

        /// <summary>
        /// Equation 2.6.5-14
        /// </summary>
        public double CalculateAmoniaEmissionFromOrganicNitrogen(
            double nitrogenInputsFromOrganicNitrogen,
            double fractionOfNitrogenLostByVolatilization,
            double emissionFactorVolatilization)
        {
            return nitrogenInputsFromOrganicNitrogen * fractionOfNitrogenLostByVolatilization *
                   (1 - emissionFactorVolatilization);
        }

        /// <summary>
        /// Equation 2.6.6-1
        /// Equation 2.6.6-2
        /// Equation 2.6.6-3
        /// </summary>
        public double AdjustSyntheticNitrogenPool(
            double currentSyntheticNitrogenPool,
            double directNitrousOxideEmissionsFromSyntheticFertilizer,
            double nitricOxideEmissionsFromSyntheticFertilizer,
            double nitrousOxideEmissionsFromLeachedSyntheticFertilizer,
            double actualAmountOfNitrogenLeachedFromSyntheticFertilizer,
            double nitrousOxideEmissionsFromVolatilization,
            double ammoniaEmissionsFromSyntheticEmissions)
        {
            currentSyntheticNitrogenPool -= (directNitrousOxideEmissionsFromSyntheticFertilizer + nitricOxideEmissionsFromSyntheticFertilizer);
            currentSyntheticNitrogenPool -= (nitrousOxideEmissionsFromLeachedSyntheticFertilizer + actualAmountOfNitrogenLeachedFromSyntheticFertilizer);
            currentSyntheticNitrogenPool -= (nitrousOxideEmissionsFromVolatilization + ammoniaEmissionsFromSyntheticEmissions);

            return currentSyntheticNitrogenPool;
        }

        /// <summary>
        /// Equation 2.6.6-4
        /// Equation 2.6.6-5
        /// </summary>
        public double AdjustCropResidueNitrogenPool(
            double currentCropResidueNitrogenPool,
            double directNitrousOxideEmissionsFromCropResidueNitrogen,
            double nitricOxideEmissionsFromCropResidueNitrogen,
            double nitrousOxideEmissionsFromLeachingOfCropResidueNitrogen,
            double actualAmountOfNitrogenLeachedFromCropResidues)
        {
            currentCropResidueNitrogenPool -= (directNitrousOxideEmissionsFromCropResidueNitrogen + nitricOxideEmissionsFromCropResidueNitrogen);
            currentCropResidueNitrogenPool -= (nitrousOxideEmissionsFromLeachingOfCropResidueNitrogen + actualAmountOfNitrogenLeachedFromCropResidues);

            return currentCropResidueNitrogenPool;
        }

        /// <summary>
        /// Equation 2.6.6-6
        /// Equation 2.6.6-7
        /// </summary>
        public double AdjustMineralizedNitrogenPool(
            double currentMineralizeNitrogenPool,
            double directNitrousOxideEmissionsFromMineralizedNitrogen,
            double nitricOxideEmissionsFromMineralizedNitrogen,
            double nitrousOxideEmissionsFromLeachingOfMineralizedNitrogen,
            double actualAmountOfNitrogenLeachedFromMineralizedNitrogen)
        {
            currentMineralizeNitrogenPool -= (directNitrousOxideEmissionsFromMineralizedNitrogen + nitricOxideEmissionsFromMineralizedNitrogen);
            currentMineralizeNitrogenPool -= (nitrousOxideEmissionsFromLeachingOfMineralizedNitrogen + actualAmountOfNitrogenLeachedFromMineralizedNitrogen);

            return currentMineralizeNitrogenPool;
        }

        /// <summary>
        /// Equation 2.6.6-8
        /// Equation 2.6.6-9
        /// Equation 2.6.6-10
        /// </summary>
        public double AdjustOrganicNitrogenPool(
            double currentOrganicNitrogenPool,
            double directNitrousOxideEmissionsFromOrganicNitrogen,
            double nitricOxideEmissionsFromOrganicNitrogen,
            double nitrousOxideEmissionsFromLeachingOfOrganicNitrogen,
            double actualAmountOfNitrogenFromLeachingOfOrganicNitrogen,
            double nitrousOxideEmissionsFromVolalitlizationOfOrganicNitrogen,
            double ammoniaEmissionsFromOrganicNitrogen)
        {
            currentOrganicNitrogenPool -= (directNitrousOxideEmissionsFromOrganicNitrogen +  nitricOxideEmissionsFromOrganicNitrogen);
            currentOrganicNitrogenPool -= (nitrousOxideEmissionsFromLeachingOfOrganicNitrogen + actualAmountOfNitrogenFromLeachingOfOrganicNitrogen);
            currentOrganicNitrogenPool -= (nitrousOxideEmissionsFromVolalitlizationOfOrganicNitrogen + ammoniaEmissionsFromOrganicNitrogen);

            return currentOrganicNitrogenPool;
        }

        /// <summary>
        /// Equation 2.6.6-11
        /// </summary>
        public double AdjustMineralNitrogenPool(
            double mineralNitrogenPoolAtPreviousInterval,
            double actualAmountOfMineralNitrogenLeached,
            double nitrousOxideEmissionsFromLeachingOfMineralNitrogen)
        {
            var result = mineralNitrogenPoolAtPreviousInterval - actualAmountOfMineralNitrogenLeached - nitrousOxideEmissionsFromLeachingOfMineralNitrogen;

            return result;
        }

        /// <summary>
        /// Equation 2.6.7-1
        /// Equation 2.6.7-2
        /// </summary>
        public double CloseMineralNitrogenPool(
            double currentMicrobialNitrogenPool,
            double syntheticNitrogenPool,
            double mineralizedNitrogenPool)
        {
            currentMicrobialNitrogenPool += syntheticNitrogenPool;
            currentMicrobialNitrogenPool += mineralizedNitrogenPool;

            return currentMicrobialNitrogenPool;
        }

        /// <summary>
        /// Equation 2.6.7-3
        /// Equation 2.6.7-4
        /// </summary>
        public double CloseMicrobeNitrogenPool(
            double currentMicrobialNitrogenPool,
            double cropResiduePool,
            double organicNitrogenPool)
        {
            currentMicrobialNitrogenPool += cropResiduePool;
            currentMicrobialNitrogenPool += organicNitrogenPool;

            return currentMicrobialNitrogenPool;
        }

        /// <summary>
        /// Equation 2.6.7-5
        /// Equation 2.6.7-6
        /// </summary>
        public double CalculateRatioBetweenMineralAndMicrobialNitrogen(
            double availabilityOfNitrogenInMicrobialPoolOfField,
            double availabilityOfMineralNitrogenOnField)
        {
            // Ratio cannot be negative - this is a temporary modification to the equations made by Roland on June 26, 2020
            availabilityOfNitrogenInMicrobialPoolOfField = Math.Abs(availabilityOfNitrogenInMicrobialPoolOfField);
            availabilityOfMineralNitrogenOnField = Math.Abs(availabilityOfMineralNitrogenOnField);

            // If mineralN is 0 we return calculation as in 2.6.7-6
            if (Math.Abs(availabilityOfMineralNitrogenOnField) < double.Epsilon)
            {
                return availabilityOfMineralNitrogenOnField / availabilityOfNitrogenInMicrobialPoolOfField;
            }

            if (availabilityOfMineralNitrogenOnField > availabilityOfNitrogenInMicrobialPoolOfField)
            {
                return 1.0 / (availabilityOfMineralNitrogenOnField / availabilityOfNitrogenInMicrobialPoolOfField);
            }

            return availabilityOfMineralNitrogenOnField / availabilityOfNitrogenInMicrobialPoolOfField;
        }

        /// <summary>
        /// Equation 2.6.7-7
        /// </summary>
        public double CalculateNitrogenRequirementForCarbonTransitionFromYoungToOldPool(
            double youngPoolHumificationConstantAboveGround,
            double youngPoolHumificationConstantBelowGround,
            double youngPoolHumificationConstantManure,
            double decompositionRateConstantYoungPool,
            double decompositionRateConstantOldPool,
            double youngPoolSoilOrganicCarbonAboveGroundAtPreviousInterval,
            double youngPoolSoilOrganicCarbonBelowGroundAtPreviousInterval,
            double youngPoolSoilOrganicCarbonManureAtPreviousInterval,
            double aboveGroundResidueCarbonInputAtPreviousInterval,
            double belowGroundResidueCarbonInputAtPreviousInterval,
            double manureCarbonInputAtPreviousInterval,
            double oldCarbonN,
            double climateParameter)
        {
            var decompositionRateConstantDifference =
                decompositionRateConstantOldPool - decompositionRateConstantYoungPool;

            var firstTerm = ((-1) * youngPoolHumificationConstantAboveGround) *
                            
                            ((decompositionRateConstantYoungPool *
                             (youngPoolSoilOrganicCarbonAboveGroundAtPreviousInterval + aboveGroundResidueCarbonInputAtPreviousInterval)) /
                             decompositionRateConstantDifference);

            var secondTerm = ((-1) * youngPoolHumificationConstantBelowGround) *

                             ((decompositionRateConstantYoungPool *
                              (youngPoolSoilOrganicCarbonBelowGroundAtPreviousInterval + belowGroundResidueCarbonInputAtPreviousInterval)) /
                              decompositionRateConstantDifference);

            var thirdTerm = ((-1) * youngPoolHumificationConstantManure) *

                            ((decompositionRateConstantYoungPool *
                             (youngPoolSoilOrganicCarbonManureAtPreviousInterval + manureCarbonInputAtPreviousInterval)) /
                             decompositionRateConstantDifference);

            var firstCombinedTerm = (firstTerm + secondTerm + thirdTerm) * Math.Exp((-1) * decompositionRateConstantOldPool * climateParameter);

            var fourthTerm = (youngPoolHumificationConstantAboveGround) *

                             ((decompositionRateConstantYoungPool *
                              (youngPoolSoilOrganicCarbonAboveGroundAtPreviousInterval + aboveGroundResidueCarbonInputAtPreviousInterval)) /
                              decompositionRateConstantDifference) * 
                             
                             Math.Exp((-1) * decompositionRateConstantOldPool * climateParameter);

            var fifthTerm = (youngPoolHumificationConstantAboveGround) *

                             ((decompositionRateConstantYoungPool *
                               (youngPoolSoilOrganicCarbonBelowGroundAtPreviousInterval + belowGroundResidueCarbonInputAtPreviousInterval)) /
                              decompositionRateConstantDifference) *

                             Math.Exp((-1) * decompositionRateConstantOldPool * climateParameter);

            var sixthTerm = (youngPoolHumificationConstantAboveGround) *

                            ((decompositionRateConstantYoungPool *
                              (youngPoolSoilOrganicCarbonManureAtPreviousInterval + manureCarbonInputAtPreviousInterval)) /
                             decompositionRateConstantDifference) *

                            Math.Exp((-1) * decompositionRateConstantOldPool * climateParameter);

            var result = (firstCombinedTerm + fourthTerm + fifthTerm + sixthTerm) * oldCarbonN;

            return result;
        }

        /// <summary>
        /// Equation 2.6.7-12
        /// </summary>
        public double CalculateCropNitrogenDemand(double carbonInputFromProduct,
                                                  double carbonInputFromStraw,
                                                  double carbonInputFromRoots,
                                                  double carbonInputFromExtraroots,
                                                  double moistureContentOfCropFraction,
                                                  double nitrogenConcentrationInTheProduct,
                                                  double nitrogenConcentrationInTheStraw,
                                                  double nitrogenConcentrationInTheRoots,
                                                  double nitrogenConcentrationInExtraroots,
                                                  double nitrogenFixation)
        {            
            return (carbonInputFromProduct / 0.45 * (1 - moistureContentOfCropFraction)) * nitrogenConcentrationInTheProduct +
                   (carbonInputFromStraw / 0.45 * (1 - moistureContentOfCropFraction)) * nitrogenConcentrationInTheStraw +
                   (carbonInputFromRoots / 0.45 * (1 - moistureContentOfCropFraction)) * nitrogenConcentrationInTheRoots +

                   (carbonInputFromExtraroots / 0.45 * (1 - moistureContentOfCropFraction)) * nitrogenConcentrationInExtraroots - (1 - nitrogenFixation);
        }

        /// <summary>
        /// Equation 2.6.7-17        
        /// Equation 2.6.7-18        
        /// </summary>
        public double CalculateAmountOfMicrobeDeath(
            double currentMicrobialNitrogenPool,
            double microbeDeathRate)
        {
            if (currentMicrobialNitrogenPool > 0)
            {
                return currentMicrobialNitrogenPool * microbeDeathRate;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Equation 2.6.7.18
        /// </summary>
        public double CalculateIncreaseInAvailabilityOfMineralNOnField(
            double currentAvailabilityOfMineralNitrogenOnField,
            double microbeDeath)
        {
            currentAvailabilityOfMineralNitrogenOnField += currentAvailabilityOfMineralNitrogenOnField * microbeDeath;

            return currentAvailabilityOfMineralNitrogenOnField;
        }

        /// <summary>
        /// Equation 2.6.7-21
        /// Equation 2.6.7-22
        /// </summary>
        public double CalculateAmountOfDenitrification(
            double availabilityOfMineralNitrogenOnField,
            double denitrification)
        {
            if (availabilityOfMineralNitrogenOnField > 0)
            {
                return availabilityOfMineralNitrogenOnField * denitrification;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Equation 2.6.7-20
        /// </summary>
        public double CalculateFifthReductionInAvailabilityOfMineralNOnField(
            double currentAvailabilityOfMineralNOnField,
            double denitrification)
        {
            currentAvailabilityOfMineralNOnField -= currentAvailabilityOfMineralNOnField * denitrification;

            return currentAvailabilityOfMineralNOnField;
        }


        /// <summary>
        /// Equation 2.6.8-1
        /// </summary>
        public double CalculateDirectNitrousOxideEmissions(
            double nitrousOxideEmissionsSyntheicNitrogen,
            double nitrousOxideEmissionsFromCropResidues,
            double directNitrousOxideEmissionsFromMineralizedNitrogen,
            double directNitrousOxideEmissionsFromOrganicNitrogen)
        {
            return nitrousOxideEmissionsSyntheicNitrogen +
                   nitrousOxideEmissionsFromCropResidues +
                   directNitrousOxideEmissionsFromMineralizedNitrogen +
                   directNitrousOxideEmissionsFromOrganicNitrogen;
        }

        /// <summary>
        /// Equation 2.6.8-2
        /// </summary>
        public double CalculateIndirectNitrousOxideEmissions(
            double indirectNitrousOxideEmissionsFromLeachingOfSyntheticNitrogen,
            double indirectNitrousOxideEmissionsFromLeachingOfCropResidues,
            double indirectNitrousOxideEmissionsFromLeachingOfMineralNitrogen,
            double indirectNitrousOxideEmissionsFromLeachingOfOrganicNitrogen,
            double indirectNitrousOxideEmissionsFromVolatlizationOfSyntheticNitrogen,
            double indirectNitrousOxideEmissionsFromVolatilizationOfOrganicNitrogen)
        {
            return indirectNitrousOxideEmissionsFromLeachingOfSyntheticNitrogen +
                   indirectNitrousOxideEmissionsFromLeachingOfCropResidues +
                   indirectNitrousOxideEmissionsFromLeachingOfMineralNitrogen +
                   indirectNitrousOxideEmissionsFromLeachingOfOrganicNitrogen +
                   indirectNitrousOxideEmissionsFromVolatlizationOfSyntheticNitrogen +
                   indirectNitrousOxideEmissionsFromVolatilizationOfOrganicNitrogen;
        }

        /// <summary>
        /// Equation 2.6.8-3
        /// </summary>
        public double CalculateSumOfDirectN2OEmissions(
            double directNitrousOxideEmissions,
            double indirectNitrousOxideEmissions)
        {
            return directNitrousOxideEmissions +
                   indirectNitrousOxideEmissions;
        }

        /// <summary>
        /// Equation 2.6.8-4
        /// Equation 2.6.8-6
        /// Equation 2.6.8-8
        /// </summary>
        public double CalculateEmissionsForArea(
            double emissions,
            double areaOfField)
        {
            var result = emissions / areaOfField;

            return result;
        }

        /// <summary>
        /// Equation 2.6.8-5
        /// </summary>
        public double CalculateTotalNitricOxideEmissions(
            double nitricOxideEmissionsFromLeachingOfSyntheicNitrogen,
            double nitricOxideEmissionsFromLeachingOfCropResidues,
            double nitricOxideOxideEmissionsFromLeachingOfMineralizedNitrogen,
            double nitricOxideEmissionsFromLeachingOfOrganicNitrogen)
        {
            return nitricOxideEmissionsFromLeachingOfSyntheicNitrogen +
                   nitricOxideEmissionsFromLeachingOfCropResidues +
                   nitricOxideOxideEmissionsFromLeachingOfMineralizedNitrogen +
                   nitricOxideEmissionsFromLeachingOfOrganicNitrogen;
        }

        /// <summary>
        /// Equation 2.6.8-7
        /// </summary>
        public double CalculateTotalNitrateLeachingEmissions(
            double nitrateEmissionsFromLeachingOfSyntheticNitrogen,
            double nitrateEmissionsFromLeachingOfCropResidues,
            double nitrateEmissionsFromLeachingOfMineralizedNitrogen,
            double nitrateEmissionsFromLeachingOfOrganicNitrogen)
        {
            return nitrateEmissionsFromLeachingOfSyntheticNitrogen +
                   nitrateEmissionsFromLeachingOfCropResidues +
                   nitrateEmissionsFromLeachingOfMineralizedNitrogen +
                   nitrateEmissionsFromLeachingOfOrganicNitrogen;
        }

        /// <summary>
        /// Equation 2.6.8-9
        /// </summary>
        public double CalculateTotalAmmoniaEmissions(
            double ammoniaEmissionsFromSyntheicNitrogen,
            double ammoniaEmissionsOrganicNitrogen)
        {
            return ammoniaEmissionsFromSyntheicNitrogen +
                   ammoniaEmissionsOrganicNitrogen;
        }

        /// <summary>
        /// Equation 2.6.8-10
        /// </summary>
        public double CalculateAmmoniaEmissionForField(
            double totalAmmoniaEmissions,
            double areaOfField)
        {
            var result = totalAmmoniaEmissions / areaOfField;

            return result;
        }

        /// <summary>
        /// Equation 2.6.8-11
        /// </summary>
        public double CalculateDenitrificationForField(
            double denitrificationOfMineralNitrogen,
            double areaOfField)
        {
            return denitrificationOfMineralNitrogen / areaOfField;
        }

        /// <summary>
        /// Equation 2.6.8-12
        /// </summary>
        public double CaclulateSummaryOfNitrousOxideEmissions(List<double> emissions)
        {
            return emissions.Sum();
        }

        /// <summary>
        /// Equation 2.6.8-13
        /// </summary>
        public double CaclulateSummaryOfNitricOxideEmissions(List<double> emissions)
        {
            return emissions.Sum();
        }

        /// <summary>
        /// Equation 2.6.8-14
        /// </summary>
        public double CaclulateSummaryOfNitrateEmissions(List<double> emissions)
        {
            return emissions.Sum();
        }

        /// <summary>
        /// Equation 2.6.8-15
        /// </summary>
        public double CaclulateSummaryOfAmmoniumEmissions(List<double> emissions)
        {
            return emissions.Sum();
        }

        /// <summary>
        /// Equation 2.6.8-16
        /// </summary>
        public double CaclulateSummaryOfNitrogenEmissions(List<double> emissions)
        {
            return emissions.Sum();
        }

        /// <summary>
        /// Equation 2.6.8-17
        /// </summary>
        public double CaclulateAnnualSummaryOfNitrousOxideEmissions(List<double> emissions)
        {
            return emissions.Sum();
        }

        /// <summary>
        /// Equation 2.6.8-18
        /// </summary>
        public double CaclulateAnnualSummaryOfNitricOxideEmissions(List<double> emissions)
        {
            return emissions.Sum();
        }

        /// <summary>
        /// Equation 2.6.8-19
        /// </summary>
        public double CaclulateAnnualSummaryOfNitrateEmissions(List<double> emissions)
        {
            return emissions.Sum();
        }

        /// <summary>
        /// Equation 2.6.8-20
        /// </summary>
        public double CaclulateAnnualSummaryOfAmmoniumEmissions(List<double> emissions)
        {
            return emissions.Sum();
        }

        /// <summary>
        /// Equation 2.6.8-21
        /// </summary>
        public double CaclulateAnnualSummaryOfNitrogenEmissions(List<double> emissions)
        {
            return emissions.Sum();
        }

        /// <summary>
        /// No equation number in alogorithm document.
        /// </summary>
        public double CaculateSumOfExportedNitrousOxideEmissions(
            double nitrousOxideEmissionsFromExportedResidues,
            double nitrousOxideEmissionsFromExportedOrganics)
        {
            return nitrousOxideEmissionsFromExportedResidues + nitrousOxideEmissionsFromExportedOrganics;
        }

        /// <summary>
        /// No equation number in alogorithm document.
        /// </summary>
        public double CaculateSumOfExportedNitricOxideEmissions(
            double nitricOxideEmissionsFromExportedResidues,
            double nitricOxideEmissionsFromExportedOrganics)
        {
            return nitricOxideEmissionsFromExportedResidues + nitricOxideEmissionsFromExportedOrganics;
        }

        /// <summary>
        /// Equation 2.6.8-22
        /// </summary>
        public double ConvertNitrousOxide(double emission)
        {
            return emission * (44.0 / 28.0);
        }

        /// <summary>
        /// Equation 2.6.8-23
        /// </summary>
        public double ConvertNitricOxide(double emission)
        {
            return emission * (30.0 / 14.0);
        }

        /// <summary>
        /// Equation 2.6.8-24
        /// </summary>
        public double ConvertNitrate(double emission)
        {
            return emission * (62.0 / 14.0);
        }

        /// <summary>
        /// Equation 2.6.8-25
        /// </summary>
        public double ConvertAmmoniumToNitrousOxide(double emission)
        {
            return emission * (18.0 / 14.0);
        }

        /// <summary>
        /// Equation 2.6.8-26
        /// </summary>
        public double ConvertNitrogen(double emission)
        {
            return emission * (14.0 / 28.0);
        }

        /// <summary>
        /// Equation 2.6.8-27
        /// </summary>
        public double CalculateMonthlyEmission(double emission, double monthlyPercentage)
        {
            return emission * (monthlyPercentage / 100);
        }
       
        #endregion
    }
}