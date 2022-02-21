using System;

namespace H.Core.Models.LandManagement.Fields
{
    public partial class CropViewItem
    {
        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// kg ha^-1
        /// </summary>
        public double ChangeInCarbon { get; set; }

        /// <summary>
        /// kg ha^-1
        /// </summary>
        public double SoilCarbon { get; set; }
        public double ActualMeasuredSoilCarbon { get; set; }
        public double DifferenceOfCurrentYearCalculatedCarbonAndStartYearCalculatedCarbon { get; set; }
        public double DifferenceOfCurrentYearMeasuredCarbonAndStartYearMeasuredCarbon { get; set; }
        public double AverageSoilCarbonAcrossAllFieldsInFarm { get; set; }
        public double OldPoolSoilCarbon { get; set; }

        /// <summary>
        /// kg C ha^-1
        /// </summary>
        public double YoungPoolSoilCarbonBelowGround { get; set; }

        /// <summary>
        /// kg C ha^-1
        /// </summary>
        public double YoungPoolSoilCarbonAboveGround { get; set; }
        public double YoungPoolSteadyStateManure { get; set; }
        public double YoungPoolManureCarbon { get; set; }
        public double AmountOfCarbonAppliedFromManure { get; set; }
        public double AmountOfNitrogenAppliedFromManure { get; set; }
        public double AmountOfPhosphorusAppliedFromManure { get; set; }
        public double MoistureOfManure { get; set; }
        public double ChangeInSoilOrganicCarbonForFieldAtInterval { get; set; }

        [Obsolete("Use YoungPoolSoilCarbonAboveGround")]
        public double YoungPoolSteadyStateAboveGround { get; set; }

        [Obsolete("Use YoungPoolSoilCarbonBelowGround")]
        public double YoungPoolSteadyStateBelowGround { get; set; }

        /*
         * Nitrogen pools
         */

        public double SyntheticNitrogenPool_N_SN { get; set; }
        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double MineralNitrogenPool_N_mineralN { get; set; }
        public double ManureResiduePool_ManureN { get; set; }
        public double CropResidueNitrogenPool_N_CropResidues { get; set; }
        public double OrganicNitrogenPool_N_ON { get; set; }
        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double MicrobeNitrogenPool_N_microbeN { get; set; }
        /// <summary>
        /// kg N
        /// </summary>
        public double N_min_FromDecompositionOfOldCarbon { get; set; }
        public double MineralizedNitrogenPool_N_min { get; set; }

        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double AboveGroundResiduePool_AGresidueN { get; set; }

        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double BelowGroundResiduePool_BGresidueN { get; set; }

        /// <summary>
        /// kg N2O-N ha^-1
        /// </summary>
        public double DirectNitrousOxideEmissionsFromSyntheticNitrogenForArea { get; set; }

        /// <summary>
        /// kg N2O-N ha^-1
        /// </summary>
        public double DirectNitrousOxideEmissionsFromCropResiduesForArea { get; set; }

        /// <summary>
        /// kg N2O-N ha^-1
        /// </summary>
        public double DirectNitrousOxideEmissionsFromMineralizedNitrogenForArea { get; set; }

        /// <summary>
        /// kg N2O-N ha^-1
        /// </summary>
        public double DirectNitrousOxideEmissionsFromOrganicNitrogenForArea { get; set; }

        /// <summary>
        /// kg N2O-N ha^-1
        /// </summary>
        public double TotalDirectNitrousOxideForArea { get; set; }
        public double TotalNitrousOxideForArea { get; set; }
        public double IndirectNitrousOxideEmissionsFromSyntheticNitrogenForArea { get; set; }
        public double IndirectNitrousOxideEmissionsFromCropResiduesForArea { get; set; }
        public double IndirectNitrousOxideEmissionsFromMineralizedNitrogenForArea { get; set; }
        public double IndirectNitrousOxideEmissionsFromOrganicNitrogenForArea { get; set; }
        public double IndirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForArea { get; set; }
        public double TotalIndirectNitrousOxideForArea { get; set; }

        /// <summary>
        /// kg NO3N ha^-1
        /// </summary>
        public double TotalNitricOxideForArea { get; set; }
        public double DirectNitricOxideEmissionsFromSyntheticNitrogenForArea { get; set; }
        public double DirectNitricOxideEmissionsFromCropResiduesForArea { get; set; }
        public double DirectNitricOxideEmissionsFromMineralizedNitrogenForArea { get; set; }
        public double DirectNitricOxideEmissionsFromOrganicNitrogenForArea { get; set; }
        public double TotalNitrateLeachingForArea { get; set; }
        public double IndirectNitrateFromSyntheticNitrogenForArea { get; set; }
        public double IndirectNitrateFromCropResiduesForArea { get; set; }
        public double IndirectNitrateFromMineralizedNitrogenForArea { get; set; }
        public double IndirectNitrateFromOrganicNitrogenForArea { get; set; }
        public double TotalAmmoniaForArea { get; set; }
        public double IndirectAmmoniumEmissionsFromVolatilizationOfOrganicNitrogenForArea { get; set; }
        public double IndirectAmmoniumEmissionsFromVolatilizationOfSyntheticNitrogenForArea { get; set; }
        public double DenitrificationForArea { get; set; }

        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double AboveGroundNitrogenResidueForCrop { get; set; }

        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double BelowGroundResidueNitrogenForCrop { get; set; }
        /// <summary>
        /// kg N
        /// </summary>
        public double OldPoolNitrogenRequirement { get; set; }

        /// <summary>
        /// kg ha^-1
        /// </summary>
        public double CropNitrogenDemand { get; set; }

        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double TotalNitrogenInputs { get; set; }
        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double TotalNitrogenEmissions { get; set; }
        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double TotalNitrogenOutputs { get; set; }
        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double DifferenceBetweenInputsAndOutputs { get; set; }
        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double SumOfMineralAndMicrobialPools { get; set; }

        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double MineralNitrogenBalance { get; set; }
        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double MicrobialNitrogenBalance { get; set; }
        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double SyntheticInputsBeforeAdjustment { get; set; }

        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double CropResiduesBeforeAdjustment { get; set; }

        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double OrganicNitrogenResiduesBeforeAdjustment { get; set; }
        /// <summary>
        /// kg N
        /// </summary>
        public double TotalUptake { get; set; }
        public double Overflow { get; set; }

        /// <summary>
        /// unitless
        /// </summary>
        public double Ratio { get; set; }
        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double MicrobeDeath { get; set; }
        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double MicrobialPoolAfterCloseOfBudget { get; set; }
        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double MicrobialPoolAfterOldPoolDemandAdjustment { get; set; }
        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double MicrobialPoolAfterCropDemandAdjustment { get; set; }
        public double SoilCarbonRootMeanSquareError { get; set; }
        public double MeanAbsoluteError { get; set; }

        #endregion

        #region IPCC Tier 2

        /// <summary>
        /// Annual average air temperature effect on decomposition
        /// 
        /// (unitless)
        /// </summary>
        public double TFac { get; set; }

        /// <summary>
        /// Annual water effect on decomposition 
        /// 
        /// (unitless)
        /// </summary>
        public double WFac { get; set; }       

        /// <summary>
        /// Fraction of 0-30 cm soil mass that is sand (0.050 – 2mm particles)
        /// 
        /// (unitless)
        /// </summary>
        public double Sand { get; set; }

        /// <summary>
        /// Nitrogen fraction of the carbon input.
        /// 
        /// (unitless)
        /// </summary>
        public double NitrogenContent { get; set; }

        /// <summary>
        /// Carbon input to the metabolic dead organic matter carbon component 
        /// 
        /// (tonnes C ha^-1 year^-1)
        /// </summary>
        public double Beta { get; set; }

        /// <summary>
        /// Carbon input to the active soil carbon sub-pool 
        /// 
        /// (tonnes C ha^-1)
        /// </summary>
        public double Alpha { get; set; }

        /// <summary>
        /// Decay rate for active SOC sub-pool 
        /// 
        /// (year^-1)
        /// </summary>
        public double ActivePoolDecayRate { get; set; }

        /// <summary>
        /// Steady state active sub-pool SOC stock given conditions in year y
        /// 
        /// (tonnes C ha^-1)
        /// </summary>
        public double ActivePoolSteadyState { get; set; }

        /// <summary>
        /// Active sub-pool SOC stock in year y
        /// 
        /// (tonnes C ha^-1)
        /// </summary>
        public double ActivePool { get; set; }

        /// <summary>
        /// Annual change in active sub-pool
        /// 
        /// (tonnes C ha^-1)
        /// </summary>
        public double ActivePoolDiff { get; set; }

        /// <summary>
        /// Decay rate for passive SOC sub-pool
        /// 
        /// (year^-1)
        /// </summary>
        public double PassivePoolDecayRate { get; set; }

        /// <summary>
        /// Steady state passive sub-pool SOC given conditions in year y
        /// 
        /// (tonnes C ha^-1)
        /// </summary>
        public double PassivePoolSteadyState { get; set; }

        /// <summary>
        /// Passive sub-pool SOC stock in year y 
        /// 
        /// (tonnes C ha^-1)
        /// </summary>
        public double PassivePool { get; set; }

        /// <summary>
        /// Annual change in passive sub-pool
        /// 
        /// (tonnes C ha^-1)
        /// </summary>
        public double PassivePoolDiff { get; set; }

        /// <summary>
        /// Decay rate for slow SOC sub-pool
        /// 
        /// (year^-1)
        /// </summary>
        public double SlowPoolDecayRate { get; set; }

        /// <summary>
        /// Steady state slow sub-pool SOC given conditions in year y
        /// 
        /// (tonnes C ha^-1)
        /// </summary>
        public double SlowPoolSteadyState { get; set; }

        /// <summary>
        /// Slow sub-pool SOC stock in year y 
        /// 
        /// (tonnes C ha^-1)
        /// </summary>
        public double SlowPool { get; set; }

        /// <summary>
        /// Annual change in slow sub-pool
        /// 
        /// (tonnes C ha^-1)
        /// </summary>
        public double SlowPoolDiff { get; set; }

        /// <summary>
        /// SOC stock at the end of the current year y for grid cell or region
        /// 
        /// (tonnes C ha^-1)
        /// </summary>
        public double Soc { get; set; }

        /// <summary>
        /// Annual stock change factor for mineral soils in grid cell or region
        /// 
        /// (tonnes C ha^-1)
        /// </summary>
        public double SocDiff { get; set; }


        #endregion
    }
}