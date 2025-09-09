using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Carbon
{
    public partial class IPCCTier2SoilCarbonCalculator
    {
        #region Properties

        public double SocNRequirement { get; set; }

        #endregion

        #region Public Methods

        public void CalculateNitrogenAtInterval(
            CropViewItem previousYearResults,
            CropViewItem currentYearResults,
            CropViewItem nextYearResults,
            Farm farm,
            int yearIndex)
        {
            N2OEmissionFactorCalculator.Initialize(farm, AnimalComponentEmissionsResults);

            YearIndex = yearIndex;
            CurrentYearResults = currentYearResults;
            PreviousYearResults = previousYearResults;
            Year = CurrentYearResults.Year;

            SetPoolStartStates(farm);

            // Set this before calculating pools
            currentYearResults.TotalNitrogenInputsForIpccTier2 = CropResiduePool;

            CalculatePools(currentYearResults, previousYearResults, farm);

            // Equation 2.7.3-9
            MineralPool = PreviousYearResults.IpccTier2NitrogenResults.ActivePool *
                          CurrentYearResults.IpccTier2NitrogenResults.ActivePoolDecayRate +
                          PreviousYearResults.IpccTier2NitrogenResults.SlowPool *
                          CurrentYearResults.IpccTier2NitrogenResults.SlowPoolDecayRate +
                          PreviousYearResults.IpccTier2NitrogenResults.PassivePool *
                          CurrentYearResults.IpccTier2NitrogenResults.PassivePoolDecayRate;

            CurrentYearResults.MineralPool = MineralPool;

            // Equation 2.7.3-10
            SocNRequirement = PreviousYearResults.IpccTier2NitrogenResults.ActivePoolSteadyState *
                              CurrentYearResults.IpccTier2NitrogenResults.ActivePoolDecayRate +
                              PreviousYearResults.IpccTier2NitrogenResults.SlowPoolSteadyState *
                              CurrentYearResults.IpccTier2NitrogenResults.SlowPoolDecayRate +
                              PreviousYearResults.IpccTier2NitrogenResults.PassivePoolSteadyState *
                              CurrentYearResults.IpccTier2NitrogenResults.PassivePoolDecayRate;

            CurrentYearResults.SocNRequirement = SocNRequirement;

            // Equation 2.7.3-14 is calculated when calling CalculatePools()
            // Equation 2.7.3-15 is calculated when calling CalculatePools()
            // Equation 2.7.3-16 is derived from the calculation of 2.7.3-15

            TotalInputsBeforeReductions();
            CalculateDirectEmissions(farm, currentYearResults, previousYearResults);
            CalculateIndirectEmissions(farm, currentYearResults);
            AdjustPools();
            CloseNitrogenBudget(currentYearResults);

            CalculatePoolRatio();

            /*
             * Calculate the N demand from carbon pools
             */

            AdjustPoolsAfterDemandCalculation(SocNRequirement);

            CurrentYearResults.MicrobialPoolAfterOldPoolDemandAdjustment = MicrobePool;

            // Equation 2.7.7-11
            CropNitrogenDemand =
                (CurrentYearResults.AboveGroundResidueDryMatter * CurrentYearResults.NitrogenContentInStraw +
                 CurrentYearResults.BelowGroundResidueDryMatter / CurrentYearResults.Area *
                 CurrentYearResults.NitrogenContentInRoots) * (1 - CurrentYearResults.NitrogenFixation);

            AdjustPoolsAfterDemandCalculation(CropNitrogenDemand);

            CurrentYearResults.MicrobialPoolAfterCropDemandAdjustment = MicrobePool;

            // Summation of emissions must occur before balancing pools so nitrification can be calculated using total N2O-N emissions
            SumEmissions();

            BalancePools(farm.Defaults.MicrobeDeath);

            // Equation 2.7.8-32
            CurrentYearResults.TotalUptake = CropNitrogenDemand + SocNRequirement;
            base.AssignFinalValues();
        }

        #endregion

        #region Overrides

        protected override void SetOrganicNitrogenPoolStartState()
        {
            // Equation 2.7.1-3 - this does not include manure
            OrganicPool = CurrentYearResults.GetTotalOrganicNitrogenInYear() / CurrentYearResults.Area;
        }

        protected override void SetManurePoolStartState(Farm farm)
        {
            // In the IPCC Tier 2 there is no separate manure pool and so we just set it to zero here. Manure nitrogen is considered when it is added to the crop residues in 2.7.2-12
            ManurePool = 0;
        }

        protected override void AdjustOrganicPool()
        {
            // Equation 2.7.6-8
            OrganicPool -= N2O_NFromOrganicNitrogen + NO_NFromOrganicNitrogen;

            // Equation 2.7.6-9
            OrganicPool -= N2O_NFromOrganicNitrogenLeaching + NO3FromOrganicNitrogenLeaching;

            // Equation 2.7.6-10
            OrganicPool -= N2O_NOrganicNitrogenVolatilization + NH4FromOrganicNitogenVolatilized;
        }

        protected override void SetCropResiduesStartState(Farm farm)
        {
            AboveGroundResidueN = CurrentYearResults.CombinedAboveGroundResidueNitrogen;
            BelowGroundResidueN = CurrentYearResults.CombinedBelowGroundResidueNitrogen;

            CurrentYearResults.ExportedNitrogenResidueForCrop = CurrentYearResults.AboveGroundResidueDryMatterExported *
                                                                CurrentYearResults.NitrogenContent;

            // Crop residue N inputs from crop are not adjusted later on, so we can display them at this point
            CurrentYearResults.AboveGroundNitrogenResidueForCrop = AboveGroundResidueN;
            CurrentYearResults.BelowGroundResidueNitrogenForCrop = BelowGroundResidueN;

            var manureAndDigestateResidues = GetManureAndDigestateNitrogenResiduesForYear(farm, CurrentYearResults);

            // 2.7.2-13
            // Manure inputs are spread across the pools - as opposed to the ICBM approach where manure is added the dedicated manure pool
            CropResiduePool = AboveGroundResidueN + BelowGroundResidueN;
            CurrentYearResults.CropResiduesBeforeAdjustment = CropResiduePool;
        }

        #endregion
    }
}