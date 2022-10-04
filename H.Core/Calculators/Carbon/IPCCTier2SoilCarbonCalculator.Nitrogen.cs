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

            base.SetPoolStartStates(farm);

            // Set this before calculating pools
            currentYearResults.TotalNitrogenInputsForIpccTier2 = base.CropResiduePool;

            this.CalculatePools(currentYearResults, previousYearResults, farm);

            // Equation 2.7.3-9
            base.MineralPool = this.PreviousYearResults.ActivePool * this.CurrentYearResults.ActivePoolDecayRate +
                               this.PreviousYearResults.SlowPool * this.CurrentYearResults.SlowPoolDecayRate +
                               this.PreviousYearResults.PassivePool * this.CurrentYearResults.PassivePoolDecayRate;

            // Equation 2.7.3-10
            this.SocNRequirement = this.PreviousYearResults.ActivePoolSteadyState * this.CurrentYearResults.ActivePoolDecayRate +
                                   this.PreviousYearResults.SlowPoolSteadyState * this.CurrentYearResults.SlowPoolDecayRate +
                                   this.PreviousYearResults.PassivePoolSteadyState * this.CurrentYearResults.PassivePoolDecayRate;

            // Equation 2.7.3-14 is calculated when calling CalculatePools()
            // Equation 2.7.3-15 is calculated when calling CalculatePools()

            this.TotalInputsBeforeReductions();
            this.CalculateDirectEmissions(farm, currentYearResults);
            this.CalculateIndirectEmissions(farm, currentYearResults);
            this.AdjustPools();
            base.CloseNitrogenBudget(currentYearResults);

            base.CalculatePoolRatio();

            /*
             * Calculate the N demand from carbon pools
             */

            base.AdjustPoolsAfterDemandCalculation(this.SocNRequirement);

            this.CurrentYearResults.MicrobialPoolAfterOldPoolDemandAdjustment = base.MicrobePool;

            base.CropNitrogenDemand = (this.CurrentYearResults.AboveGroundCarbonInput + this.CurrentYearResults.BelowGroundCarbonInput + this.CurrentYearResults.ManureCarbonInputsPerHectare) *
                    this.CurrentYearResults.NitrogenContent;

            // Equation 2.7.7-12
            //base.CropNitrogenDemand = this.CalculateCropNitrogenDemand(
            //    carbonInputFromProduct: currentYearResults.PlantCarbonInAgriculturalProduct,
            //    carbonInputFromStraw: currentYearResults.CarbonInputFromStraw,
            //    carbonInputFromRoots: currentYearResults.CarbonInputFromRoots,
            //    carbonInputFromExtraroots: currentYearResults.CarbonInputFromExtraroots,
            //    moistureContentOfCropFraction: currentYearResults.MoistureContentOfCrop,
            //    nitrogenConcentrationInTheProduct: currentYearResults.NitrogenContentInProduct,
            //    nitrogenConcentrationInTheStraw: currentYearResults.NitrogenContentInStraw,
            //    nitrogenConcentrationInTheRoots: currentYearResults.NitrogenContentInRoots,
            //    nitrogenConcentrationInExtraroots: currentYearResults.NitrogenContentInExtraroot,
            //    nitrogenFixation: farm.Defaults.DefaultNitrogenFixation,
            //    carbonConcentration: farm.Defaults.CarbonConcentration);

            base.AdjustPoolsAfterDemandCalculation(this.CropNitrogenDemand);

            base.CurrentYearResults.MicrobialPoolAfterCropDemandAdjustment = base.MicrobePool;

            base.BalancePools(farm.Defaults.MicrobeDeath);

            // Equation 2.7.8-32
            base.CurrentYearResults.TotalUptake = base.CropNitrogenDemand + this.SocNRequirement;
            base.AssignFinalValues();

            base.SumEmissions();
        }

        #endregion
    }
}