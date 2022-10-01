using H.Core.Calculators.Nitrogen;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using H.Core.Providers.Soil;
using H.Core.Services.LandManagement;

namespace H.Core.Calculators.Carbon
{
    public partial class IPCCTier2SoilCarbonCalculator
    {
        #region Fields

        #endregion

        #region Properties

        public double SocNRequirement { get; set; }

        #endregion

        #region Overrides

        protected override void SetManurePoolStartState(Farm farm)
        {
            // TODO: get this input
            // Equation 2.7.2-11
            base.ManurePool = 0;
        }

        protected override void SetCropResiduesStartState(Farm farm)
        {
            if (this.CanCalculateInputsForCrop(this.CurrentYearResults))
            {
                // Equation 2.7.2-1
                base.AboveGroundResidueN = this.CurrentYearResults.AboveGroundResidueDryMatter * farm.Defaults.CarbonConcentration * this.CurrentYearResults.NitrogenContentInStraw;

                // Equation 2.7.2-2
                base.BelowGroundResidueN = this.CurrentYearResults.BelowGroundResidueDryMatter * farm.Defaults.CarbonConcentration * this.CurrentYearResults.NitrogenContentInRoots;
            }
            else
            {
                base.AboveGroundResidueN = this.CalculateAboveGroundResidueNitrogen(this.CurrentYearResults, farm);
                base.BelowGroundResidueN = this.CalculateBelowGroundResidueNitrogen(this.CurrentYearResults, farm);
            }

            // Crop residue N inputs from crop are not adjusted later on, so we can display them at this point
            this.CurrentYearResults.AboveGroundNitrogenResidueForCrop = base.AboveGroundResidueN;
            this.CurrentYearResults.BelowGroundResidueNitrogenForCrop = base.BelowGroundResidueN;

            base.CropResiduePool = base.AboveGroundResidueN + base.BelowGroundResidueN + base.ManurePool; // Note: this is in kg N/ha but algorithm document converts to t N/ha
            base.CurrentYearResults.CropResiduesBeforeAdjustment = base.CropResiduePool;
        }

        #endregion

        #region Public Methods

        public double CalculateAboveGroundResidueNitrogen(CropViewItem currentYearViewItem, Farm farm)
        {
            if (currentYearViewItem.CropType.IsAnnual() || currentYearViewItem.CropType.IsPerennial())
            {
                // Equation 2.7.2-3
                return (currentYearViewItem.CarbonInputFromProduct / farm.Defaults.CarbonConcentration) * currentYearViewItem.NitrogenContentInProduct +
                       (currentYearViewItem.CarbonInputFromStraw / farm.Defaults.CarbonConcentration) * currentYearViewItem.NitrogenContentInStraw;
            }

            if (currentYearViewItem.CropType.IsRootCrop())
            {
                // Equation 2.7.2-5
                return (currentYearViewItem.CarbonInputFromStraw / farm.Defaults.CarbonConcentration) * currentYearViewItem.NitrogenContentInStraw;
            }

            if (currentYearViewItem.CropType.IsSilageCrop() || currentYearViewItem.CropType.IsCoverCrop())
            {
                // Equation 2.7.2-7
                return (currentYearViewItem.CarbonInputFromProduct / farm.Defaults.CarbonConcentration) * currentYearViewItem.NitrogenContentInProduct;
            }

            // Equation 2.7.2-9
            return 0;
        }

        public double CalculateBelowGroundResidueNitrogen(CropViewItem currentYearViewItem, Farm farm)
        {
            if (currentYearViewItem.CropType.IsAnnual() || currentYearViewItem.CropType.IsPerennial())
            {
                // Equation 2.7.2-4
                return (currentYearViewItem.CarbonInputFromRoots / farm.Defaults.CarbonConcentration) * currentYearViewItem.NitrogenContentInRoots +
                       (currentYearViewItem.CarbonInputFromExtraroots / farm.Defaults.CarbonConcentration) * currentYearViewItem.NitrogenContentInExtraroot;
            }

            if (currentYearViewItem.CropType.IsRootCrop())
            {
                // Equation 2.7.2-6
                return (currentYearViewItem.CarbonInputFromProduct / farm.Defaults.CarbonConcentration) * currentYearViewItem.NitrogenContentInProduct +
                       (currentYearViewItem.CarbonInputFromExtraroots / farm.Defaults.CarbonConcentration) * currentYearViewItem.NitrogenContentInExtraroot;
            }

            if (currentYearViewItem.CropType.IsSilageCrop() || currentYearViewItem.CropType.IsCoverCrop())
            {
                // Equation 2.7.2-8
                return (currentYearViewItem.CarbonInputFromRoots / farm.Defaults.CarbonConcentration) * currentYearViewItem.NitrogenContentInRoots +
                       (currentYearViewItem.CarbonInputFromExtraroots / farm.Defaults.CarbonConcentration) * currentYearViewItem.NitrogenContentInExtraroot;
            }

            // Equation 2.7.2-10
            return 0;
        }

        public void CalculateNitrogenAtInterval(
            CropViewItem previousYearResults,
            CropViewItem currentYearResults,
            CropViewItem nextYearResults,
            Farm farm,
            int yearIndex)
        {
            this.YearIndex = yearIndex;
            this.Year = this.YearIndex + farm.CarbonModellingEquilibriumYear;
            this.CurrentYearResults = currentYearResults;
            this.PreviousYearResults = previousYearResults;

            base.SyntheticNitrogenPool = 0d;
            base.CropResiduePool = 0d;
            base.MineralPool = 0d;
            base.OrganicPool = 0d;
            base.MicrobePool = 0d;

            base.SetPoolStartStates(farm);

            // Set this before calculating pools
            currentYearResults.TotalNitrogenInputsForIpccTier2 = base.CropResiduePool;

            this.CalculatePools(currentYearResults, previousYearResults, farm);

            // Equation 2.7.3-9
            base.MineralPool = previousYearResults.ActivePool * currentYearResults.ActivePoolDecayRate +
                               previousYearResults.SlowPool * currentYearResults.SlowPoolDecayRate +
                               previousYearResults.PassivePool * currentYearResults.PassivePoolDecayRate;

            // Equation 2.7.3-10
            this.SocNRequirement = previousYearResults.ActivePoolSteadyState * currentYearResults.ActivePoolDecayRate +
                                   previousYearResults.SlowPoolSteadyState * currentYearResults.SlowPoolDecayRate +
                                   previousYearResults.PassivePoolSteadyState * currentYearResults.PassivePoolDecayRate;

            // Equation 2.7.3-14 is calculated when calling CalculatePools()
            // Equation 2.7.3-15 is calculated when calling CalculatePools()

            this.TotalInputsBeforeReductions();
            this.CalculateDirectEmissions(farm, currentYearResults);
            this.CalculateIndirectEmissions(farm, currentYearResults);
            this.AdjustPools();
            base.CloseNitrogenBudget(currentYearResults);

            base.SyntheticNitrogenPool = 0;
            base.MineralPool = 0;
            base.CropResiduePool = 0;
            base.OrganicPool = 0;

            base.CalculatePoolRatio();

            /*
             * Calculate the N demand from carbon pools
             */

            if (base.AvailabilityOfMineralN > base.MicrobePool)
            {
                // Equation 2.7.7-8
                base.AvailabilityOfMineralN -= this.SocNRequirement * (1 - base.PoolRatio);

                // Equation 2.7.7-9
                base.MicrobePool -= this.SocNRequirement * base.PoolRatio;
            }

            if (base.MicrobePool > base.AvailabilityOfMineralN)
            {
                // Equation 2.7.7-10
                base.AvailabilityOfMineralN -= this.SocNRequirement * base.PoolRatio;

                // Equation 2.7.7-11
                base.MicrobePool -= this.SocNRequirement * (1 - base.PoolRatio);
            }

            var productTerm = ((currentYearResults.PlantCarbonInAgriculturalProduct / farm.Defaults.CarbonConcentration) * (1 - currentYearResults.MoistureContentOfCrop)) * currentYearResults.NitrogenContentInProduct;
            var strawTerm = ((currentYearResults.CarbonInputFromStraw / farm.Defaults.CarbonConcentration) * (1 - currentYearResults.MoistureContentOfCrop)) * currentYearResults.NitrogenContentInStraw;
            var rootsTerm = ((currentYearResults.CarbonInputFromRoots / farm.Defaults.CarbonConcentration) * (1 - currentYearResults.MoistureContentOfCrop)) * currentYearResults.NitrogenContentInRoots;
            var extrarootsTerm = ((currentYearResults.CarbonInputFromExtraroots / farm.Defaults.CarbonConcentration) * (1 - currentYearResults.MoistureContentOfCrop)) * currentYearResults.NitrogenContentInExtraroot;

            // Equation 2.7.7-12
            base.CropNitrogenDemand = (productTerm + strawTerm + rootsTerm + extrarootsTerm) * (1 - currentYearResults.NitrogenFixation);

            base.AdjustPoolsAfterDemandCalculation(this.CropNitrogenDemand);
            base.BalancePools(farm.Defaults.MicrobeDeath);

            // Equation 2.7.8-32
            base.CurrentYearResults.TotalUptake = base.CropNitrogenDemand + this.SocNRequirement;
            base.AssignFinalValues();

            base.SumEmissions();
        }

        #endregion
    }
}