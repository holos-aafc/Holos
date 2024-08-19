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

        protected override void SetOrganicNitrogenPoolStartState()
        {
            // Equation 2.7.1-3 - this does not include manure
            this.OrganicPool = (this.CurrentYearResults.GetTotalOrganicNitrogenInYear() / this.CurrentYearResults.Area);
        }

        protected override void SetManurePoolStartState(Farm farm)
        {
            // In the IPCC Tier 2 there is no separate manure pool and so we just set it to zero here. Manure nitrogen is considered when it is added to the crop residues in 2.7.2-12
            base.ManurePool = 0;
        }

        protected override void AdjustOrganicPool()
        {
            // Equation 2.7.6-8
            this.OrganicPool -= (this.N2O_NFromOrganicNitrogen + this.NO_NFromOrganicNitrogen);

            // Equation 2.7.6-9
            this.OrganicPool -= (this.N2O_NFromOrganicNitrogenLeaching + this.NO3FromOrganicNitrogenLeaching);

            // Equation 2.7.6-10
            this.OrganicPool -= (this.N2O_NOrganicNitrogenVolatilization + this.NH4FromOrganicNitogenVolatilized);
        }

        protected override void SetCropResiduesStartState(Farm farm)
        {
            base.AboveGroundResidueN = this.CurrentYearResults.CombinedAboveGroundResidueNitrogen;
            base.BelowGroundResidueN = this.CurrentYearResults.CombinedBelowGroundResidueNitrogen;
            
            this.CurrentYearResults.ExportedNitrogenResidueForCrop = this.CurrentYearResults.AboveGroundResidueDryMatterExported * this.CurrentYearResults.NitrogenContent;

            // Crop residue N inputs from crop are not adjusted later on, so we can display them at this point
            this.CurrentYearResults.AboveGroundNitrogenResidueForCrop = base.AboveGroundResidueN;
            this.CurrentYearResults.BelowGroundResidueNitrogenForCrop = base.BelowGroundResidueN;

            var manureAndDigestateResidues = base.GetManureAndDigestateNitrogenResiduesForYear(farm, this.CurrentYearResults);

            // 2.7.2-13
            // Manure inputs are spread across the pools - as opposed to the ICBM approach where manure is added the dedicated manure pool
            base.CropResiduePool = base.AboveGroundResidueN + base.BelowGroundResidueN;
            base.CurrentYearResults.CropResiduesBeforeAdjustment = base.CropResiduePool;
        }

        #endregion

        #region Public Methods

        public void CalculateNitrogenAtInterval(
            CropViewItem previousYearResults,
            CropViewItem currentYearResults,
            CropViewItem nextYearResults,
            Farm farm,
            int yearIndex)
        {
            N2OEmissionFactorCalculator.Initialize(farm);

            this.YearIndex = yearIndex;
            this.CurrentYearResults = currentYearResults;
            this.PreviousYearResults = previousYearResults;
            this.Year = this.CurrentYearResults.Year;

            base.SetPoolStartStates(farm);

            // Set this before calculating pools
            currentYearResults.TotalNitrogenInputsForIpccTier2 = base.CropResiduePool;

            this.CalculatePools(currentYearResults, previousYearResults, farm);

            // Equation 2.7.3-9
            base.MineralPool = this.PreviousYearResults.IpccTier2NitrogenResults.ActivePool * this.CurrentYearResults.IpccTier2NitrogenResults.ActivePoolDecayRate +
                               this.PreviousYearResults.IpccTier2NitrogenResults.SlowPool * this.CurrentYearResults.IpccTier2NitrogenResults.SlowPoolDecayRate +
                               this.PreviousYearResults.IpccTier2NitrogenResults.PassivePool * this.CurrentYearResults.IpccTier2NitrogenResults.PassivePoolDecayRate;

            // Equation 2.7.3-10
            this.SocNRequirement = this.PreviousYearResults.IpccTier2NitrogenResults.ActivePoolSteadyState * this.CurrentYearResults.IpccTier2NitrogenResults.ActivePoolDecayRate +
                                   this.PreviousYearResults.IpccTier2NitrogenResults.SlowPoolSteadyState * this.CurrentYearResults.IpccTier2NitrogenResults.SlowPoolDecayRate +
                                   this.PreviousYearResults.IpccTier2NitrogenResults.PassivePoolSteadyState * this.CurrentYearResults.IpccTier2NitrogenResults.PassivePoolDecayRate;

            // Equation 2.7.3-14 is calculated when calling CalculatePools()
            // Equation 2.7.3-15 is calculated when calling CalculatePools()
            // Equation 2.7.3-16 is derived from the calculation of 2.7.3-15

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

            // Equation 2.7.7-11
            base.CropNitrogenDemand = (this.AboveGroundResidueN + this.BelowGroundResidueN) * (1 - this.CurrentYearResults.NitrogenFixation);

            base.AdjustPoolsAfterDemandCalculation(this.CropNitrogenDemand);

            base.CurrentYearResults.MicrobialPoolAfterCropDemandAdjustment = base.MicrobePool;

            // Summation of emissions must occur before balancing pools so nitrification can be calculated using total N2O-N emissions
            base.SumEmissions();

            base.BalancePools(farm.Defaults.MicrobeDeath);

            // Equation 2.7.8-32
            base.CurrentYearResults.TotalUptake = base.CropNitrogenDemand + this.SocNRequirement;
            base.AssignFinalValues();
        }

        #endregion
    }
}