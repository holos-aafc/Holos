using H.Core.Calculators.Nitrogen;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using System;

namespace H.Core.Calculators.Carbon
{
    public partial class ICBMSoilCarbonCalculator
    {
        #region Fields

        private readonly SingleYearNitrousOxideCalculator _singleYearNitrogenEmissionsCalculator = new SingleYearNitrousOxideCalculator();

        #endregion

        #region Properties

        public double OldPoolNitrogenRequirement { get; set; }

        #endregion

        #region Public Methods

        public void CalculateNitrogenAtInterval(
           CropViewItem previousYearResults,
           CropViewItem currentYearResults,
           CropViewItem nextYearResults,
           Farm farm,
           int yearIndex)
        {
            this.CurrentYearResults = currentYearResults;
            this.PreviousYearResults = previousYearResults;
            this.YearIndex = yearIndex;
            this.Year = this.YearIndex + farm.CarbonModellingEquilibriumYear;

            var climateParameterOrManagementFactor = farm.Defaults.UseClimateParameterInsteadOfManagementFactor ? currentYearResults.ClimateParameter : currentYearResults.ManagementFactor;

            base.SetPoolStartStates(farm);

            // Equation 2.6.3-5
            base.MineralPool = this.CalculateMineralizedNitrogenFromDecompositionOfOldCarbon(
                oldPoolSoilCarbonAtPreviousInterval: previousYearResults.OldPoolSoilCarbon,
                oldPoolDecompositionRate: farm.Defaults.DecompositionRateConstantOldPool,
                climateParameter: climateParameterOrManagementFactor,
                oldCarbonNitrogen: farm.Defaults.OldPoolCarbonN);

            base.TotalInputsBeforeReductions();
            base.CalculateDirectEmissions(farm, currentYearResults);
            base.CalculateIndirectEmissions(farm, currentYearResults);
            base.AdjustPools();
            base.CloseNitrogenBudget(currentYearResults);

            base.CalculatePoolRatio();

            // Equation 2.6.8-7
            this.OldPoolNitrogenRequirement = this.CalculateNitrogenRequirementForCarbonTransitionFromYoungToOldPool(
                youngPoolHumificationConstantAboveGround: farm.Defaults.HumificationCoefficientAboveGround,
                youngPoolHumificationConstantBelowGround: farm.Defaults.HumificationCoefficientBelowGround,
                youngPoolHumificationConstantManure: farm.Defaults.HumificationCoefficientManure,
                decompositionRateConstantYoungPool: farm.Defaults.DecompositionRateConstantYoungPool,
                decompositionRateConstantOldPool: farm.Defaults.DecompositionRateConstantOldPool,
                youngPoolSoilOrganicCarbonAboveGroundAtPreviousInterval: previousYearResults.YoungPoolSoilCarbonAboveGround,
                youngPoolSoilOrganicCarbonBelowGroundAtPreviousInterval: previousYearResults.YoungPoolSoilCarbonBelowGround,
                youngPoolSoilOrganicCarbonManureAtPreviousInterval: previousYearResults.YoungPoolManureCarbon,
                aboveGroundResidueCarbonInputAtPreviousInterval: previousYearResults.AboveGroundCarbonInput,
                belowGroundResidueCarbonInputAtPreviousInterval: previousYearResults.BelowGroundCarbonInput,
                manureCarbonInputAtPreviousInterval: previousYearResults.ManureCarbonInputsPerHectare,
                oldCarbonN: farm.Defaults.OldPoolCarbonN,
                climateParameter: climateParameterOrManagementFactor);

            // This is the first adjustment after the old pool demand has been determined
            base.AdjustPoolsAfterDemandCalculation(this.OldPoolNitrogenRequirement);

            this.CurrentYearResults.MicrobialPoolAfterOldPoolDemandAdjustment = base.MicrobePool;

            // Need to ensure we are passing in 100% of returns since the N uptake is before the harvest

            // Equation 2.6.7-12
            base.CropNitrogenDemand = this.CalculateCropNitrogenDemand(
                carbonInputFromProduct: currentYearResults.PlantCarbonInAgriculturalProduct,
                carbonInputFromStraw: currentYearResults.CarbonInputFromStraw,
                carbonInputFromRoots: currentYearResults.CarbonInputFromRoots,
                carbonInputFromExtraroots: currentYearResults.CarbonInputFromExtraroots,
                moistureContentOfCropFraction: currentYearResults.MoistureContentOfCrop,
                nitrogenConcentrationInTheProduct: currentYearResults.NitrogenContentInProduct,
                nitrogenConcentrationInTheStraw: currentYearResults.NitrogenContentInStraw,
                nitrogenConcentrationInTheRoots: currentYearResults.NitrogenContentInRoots,
                nitrogenConcentrationInExtraroots: currentYearResults.NitrogenContentInExtraroot,
                nitrogenFixation: farm.Defaults.DefaultNitrogenFixation,
                carbonConcentration: farm.Defaults.CarbonConcentration);

            // This is the second adjustment after the crop demand has been determined
            base.AdjustPoolsAfterDemandCalculation(base.CropNitrogenDemand);

            base.CurrentYearResults.MicrobialPoolAfterCropDemandAdjustment = base.MicrobePool;

            base.BalancePools(farm.Defaults.MicrobeDeath);

            // Equation 2.6.9-30
            base.CurrentYearResults.TotalUptake = base.CropNitrogenDemand + this.OldPoolNitrogenRequirement;
            this.AssignFinalValues();

            base.SumEmissions();
        }

        /// <summary>
        /// Equation 2.6.2-1
        /// </summary>
        public double CalculateAboveGroundResidueNitrogenAtEquilibrium(double carbonInputFromProduct,
            double nitrogenConcentrationInProduct,
            double carbonInputFromStraw,
            double nitrogenConcentrationInStraw,
            double climateFactor,
            double youngPoolDecompositionRate, double carbonConcentration)
        {
            return ((carbonInputFromProduct / carbonConcentration * nitrogenConcentrationInProduct +
                    carbonInputFromStraw / carbonConcentration * nitrogenConcentrationInStraw) *
                   Math.Exp(-1 * youngPoolDecompositionRate * climateFactor)) /
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
        public double CalculateBelowGroundResidueNitrogenAtEquilibrium(double carbonInputFromRoots,
            double nitrogenConcentrationInRoots,
            double carbonInputFromExtraroots,
            double nitrogenConcentrationInExtraroots,
            double youngPoolDecompositionRate,
            double climateFactor,
            double carbonConcentration)
        {
            return ((carbonInputFromRoots / carbonConcentration * nitrogenConcentrationInRoots +
                    carbonInputFromExtraroots / carbonConcentration * nitrogenConcentrationInExtraroots) *
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
        /// Equation 2.6.3-1
        /// Equation 2.6.3-2
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
        /// Equation 2.6.4-1
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
        /// Equation 2.6.4-2
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
        /// Equation 2.6.4-4
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
        /// Equation 2.6.8-7
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

        #region Overrides

        protected override void SetCropResiduesStartState(Farm farm)
        {
            // Equation 2.5.2-4
            var grainNitrogen = _singleYearNitrogenEmissionsCalculator.CalculateGrainNitrogen(
                carbonInputFromProduct: this.PreviousYearResults.CarbonInputFromProduct,
                nitrogenConcentrationInProduct: this.PreviousYearResults.NitrogenContentInProduct);

            // Equation 2.5.2-5
            var strawNitrogen = _singleYearNitrogenEmissionsCalculator.CalculateStrawNitrogen(
                carbonInputFromStraw: this.PreviousYearResults.CarbonInputFromStraw,
                nitrogenConcentrationInStraw: this.PreviousYearResults.NitrogenContentInStraw);

            // Equation 2.5.2-6
            var rootNitrogen = _singleYearNitrogenEmissionsCalculator.CalculateRootNitrogen(
                carbonInputFromRoots: this.PreviousYearResults.CarbonInputFromRoots,
                nitrogenConcentrationInRoots: this.PreviousYearResults.NitrogenContentInRoots);

            // Equation 2.5.2-7
            var extrarootNitrogen = _singleYearNitrogenEmissionsCalculator.CalculateExtrarootNitrogen(
                carbonInputFromExtraroots: this.PreviousYearResults.CarbonInputFromExtraroots,
                nitrogenConcentrationInExtraroots: this.PreviousYearResults.NitrogenContentInExtraroot);

            // Equation 2.6.2-2
            var aboveGroundResidueNitrogenForCropAtPreviousInterval = _singleYearNitrogenEmissionsCalculator.CalculateAboveGroundResidueNitrogen(
                nitrogenContentOfGrainReturned: grainNitrogen,
                nitrogenContentOfStrawReturned: strawNitrogen);

            // Equation 2.6.2-5
            var belowGroundResidueNitrogenForCropAtPreviousInterval = _singleYearNitrogenEmissionsCalculator.CalculateBelowGroundResidueNitrogen(
                nitrogenContentOfRootReturned: rootNitrogen,
                nitrogenContentOfExtrarootReturned: extrarootNitrogen);

            // Crop residue N inputs from crop are not adjusted later on, so we can display them at this point
            this.CurrentYearResults.AboveGroundNitrogenResidueForCrop = aboveGroundResidueNitrogenForCropAtPreviousInterval;
            this.CurrentYearResults.BelowGroundResidueNitrogenForCrop = belowGroundResidueNitrogenForCropAtPreviousInterval;

            var climateParameterOrManagementFactor = farm.Defaults.UseClimateParameterInsteadOfManagementFactor ? this.CurrentYearResults.ClimateParameter : this.CurrentYearResults.ManagementFactor;

            if (this.YearIndex == 0)
            {
                // Calculate the above and below ground starting crop residue pools for the field (t = 0)
                this.AboveGroundResidueN = this.CalculateAboveGroundResidueNitrogenAtEquilibrium(
                    carbonInputFromProduct: this.PreviousYearResults.CarbonInputFromProduct,
                    nitrogenConcentrationInProduct: this.PreviousYearResults.NitrogenContentInProduct,
                    carbonInputFromStraw: this.PreviousYearResults.CarbonInputFromStraw,
                    nitrogenConcentrationInStraw: this.PreviousYearResults.NitrogenContentInStraw,
                    climateFactor: climateParameterOrManagementFactor,
                    youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool,
                    carbonConcentration: farm.Defaults.CarbonConcentration);

                this.BelowGroundResidueN = this.CalculateBelowGroundResidueNitrogenAtEquilibrium(
                    carbonInputFromRoots: this.PreviousYearResults.CarbonInputFromRoots,
                    nitrogenConcentrationInRoots: this.PreviousYearResults.NitrogenContentInRoots,
                    carbonInputFromExtraroots: this.PreviousYearResults.CarbonInputFromExtraroots,
                    nitrogenConcentrationInExtraroots: this.PreviousYearResults.NitrogenContentInExtraroot,
                    youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool,
                    climateFactor: climateParameterOrManagementFactor,
                    carbonConcentration: farm.Defaults.CarbonConcentration);

                base.CropResiduePool = this.CalculateCropResiduesAtStartingPoint(
                    aboveGroundResidueNitrogenForCropAtStartingPoint: aboveGroundResidueNitrogenForCropAtPreviousInterval,
                    belowGroundResidueNitrogenForCropAtStartingPoint: belowGroundResidueNitrogenForCropAtPreviousInterval,
                    decompositionRateConstantYoungPool: farm.Defaults.DecompositionRateConstantYoungPool,
                    climateParameter: climateParameterOrManagementFactor);
            }
            else
            {
                // Calculate the above and below ground crop residue pools for the field on subsequent years (t > 0)
                base.AboveGroundResidueN = this.CalculateAboveGroundResidueNitrogenForFieldAtInterval(
                    aboveGroundResidueNitrogenForFieldAtPreviousInterval: this.PreviousYearResults.AboveGroundResiduePool_AGresidueN,
                    aboveGroundResidueNitrogenForCropAtPreviousInterval: aboveGroundResidueNitrogenForCropAtPreviousInterval,
                    climateManagementFactor: climateParameterOrManagementFactor,
                    decompositionRateYoungPool: farm.Defaults.DecompositionRateConstantYoungPool);

                base.BelowGroundResidueN = this.CalculateBelowGroundResidueNitrogenForFieldAtInterval(
                    belowGroundResidueNitrogenForFieldAtPreviousInterval: this.PreviousYearResults.BelowGroundResiduePool_BGresidueN,
                    belowGroundResidueNitrogenForCropAtPreviousInterval: belowGroundResidueNitrogenForCropAtPreviousInterval,
                    climateManagementFactor: climateParameterOrManagementFactor,
                    decompositionRateYoungPool: farm.Defaults.DecompositionRateConstantYoungPool);

                base.CropResiduePool = this.CalculateCropResiduesAtInterval(
                    aboveGroundResidueNitrogenForFieldAtCurrentInterval: base.CurrentYearResults.AboveGroundResiduePool_AGresidueN,
                    aboveGroundResidueNitrogenForFieldAtPreviousInterval: base.PreviousYearResults.AboveGroundResiduePool_AGresidueN,
                    aboveGroundResidueNitrogenForCropAtPreviousInterval: base.PreviousYearResults.AboveGroundNitrogenResidueForCrop,
                    belowGroundResidueNitrogenForFieldAtCurrentInterval: base.CurrentYearResults.BelowGroundResiduePool_BGresidueN,
                    belowGroundResidueNitrogenForFieldAtPreviousInterval: base.PreviousYearResults.BelowGroundResiduePool_BGresidueN,
                    belowGroundResidueNitrogenForCropAtPreviousInterval: base.PreviousYearResults.BelowGroundResidueNitrogenForCrop);
            }

            base.CurrentYearResults.CropResiduesBeforeAdjustment = base.CropResiduePool;
        }

        protected override void SetManurePoolStartState(Farm farm)
        {
            var climateParameterOrManagementFactor = farm.Defaults.UseClimateParameterInsteadOfManagementFactor ? this.CurrentYearResults.ClimateParameter : this.CurrentYearResults.ManagementFactor;

            base.ManurePool = this.CalculateManureResidueNitrogenPool(
                manureResidueNitrogenPoolAtPreviousInterval: base.PreviousYearResults.ManureResiduePool_ManureN,
                amountOfManureAppliedAtPreviousInterval: base.PreviousYearResults.AmountOfManureApplied,
                decompositionRateYoungPool: farm.Defaults.DecompositionRateConstantYoungPool,
                climateParameter: climateParameterOrManagementFactor);

            if (this.YearIndex == 0)
            {
                var organicNitrogenPool_N_ON = this.CalculateAvailabilityOfNitrogenFromManureDecompositionAtStartingPoint(
                    manureResiduePoolAtEquilibrium: base.ManurePool,
                    decompositionRateConstantYoungPool: farm.Defaults.DecompositionRateConstantYoungPool,
                    climateParameter: climateParameterOrManagementFactor);
            }
            else
            {
                var organicNitrogenPool_N_ON = this.CalculateAvailabilityOfNitrogenFromManureDecompositionAtInterval(
                    manureResiduePoolAtPreviousInterval: base.PreviousYearResults.ManureResiduePool_ManureN,
                    manureResiduePoolAtCurrentInterval: base.ManurePool,
                    amountOfManureAppliedAtPreviousInterval: base.PreviousYearResults.AmountOfManureApplied);
            }

            base.CurrentYearResults.OrganicNitrogenResiduesBeforeAdjustment = base.ManurePool;
        }

        protected override void AssignFinalValues()
        {
            base.AssignFinalValues();

            this.CurrentYearResults.OldPoolNitrogenRequirement = this.OldPoolNitrogenRequirement;
        }

        #endregion

        #endregion
    }
}