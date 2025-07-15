﻿using System;
using H.Core.Emissions.Results;

namespace H.Core.Models.LandManagement.Fields
{
    public partial class CropViewItem
    {
        #region Fields

        private IPCCTier2Results _carbonResults;
        private IPCCTier2Results _nitrogenResults;

        private MonthlyValueBase<double> _monthlyIpccTier2WaterFactors;
        private MonthlyValueBase<double> _monthlyIpccTier2TemperatureFactors;

        #endregion

        #region Properties

        public double NO3NFromSyntheticFertilizerLeaching;
        public double NO3NFromResiduesLeaching;
        public double NO3NFromManureAndDigestateLeaching;
        public double TotalN2ONFromManureAndDigestateLeaching;
        public double TotalN2ONFromManureAndDigestateLeachingExcludingRemainingAmounts;
        
        /// <summary>
        /// Equation 2.5.4-1
        ///
        /// k = SN = Synthetic Nitrogen
        /// </summary>
        public double EF_SN;

        /// <summary>
        /// Equation 2.5.4-1
        ///
        /// k = CRN = Crop Residue Nitrogen
        /// </summary>
        public double EF_CRN;

        /// <summary>
        /// Equation 2.5.4-1
        ///
        /// k = ON = Organic Nitrogen
        /// </summary>
        public double EF_ON;

        /// <summary>
        /// (kg C ha^-1)
        /// </summary>
        public double ChangeInCarbon { get; set; }

        /// <summary>
        /// (kg N ha^-1)
        /// </summary>
        public double ChangeInNitrogenStock { get; set; }

        /// <summary>
        /// Soil C at beginning of year
        /// 
        /// (kg C ha^-1)
        /// </summary>
        public double SoilCarbon { get; set; }

        /// <summary>
        /// Soil C at end of year
        /// 
        /// (kg C ha^-1)
        /// </summary>
        public double SoilCarbonEndOfYear { get; set; }

        public double ActualMeasuredSoilCarbon { get; set; }
        public double DifferenceOfCurrentYearCalculatedCarbonAndStartYearCalculatedCarbon { get; set; }
        public double DifferenceOfCurrentYearMeasuredCarbonAndStartYearMeasuredCarbon { get; set; }
        public double AverageSoilCarbonAcrossAllFieldsInFarm { get; set; }
        public double OldPoolSoilCarbon { get; set; }

        /// <summary>
        /// Young pool aboveground at start of year
        /// 
        /// (kg C ha^-1)
        /// </summary>
        public double YoungPoolSoilCarbonAboveGround { get; set; }

        /// <summary>
        /// Young pool aboveground at end of year
        /// 
        /// (kg C ha^-1)
        /// </summary>
        public double YoungPoolSoilCarbonAboveGroundEndOfYear { get; set; }

        /// <summary>
        /// Young pool belowground at start of year
        /// 
        /// (kg C ha^-1)
        /// </summary>
        public double YoungPoolSoilCarbonBelowGround { get; set; }

        /// <summary>
        /// Young pool belowground at end of year
        /// 
        /// (kg C ha^-1)
        /// </summary>
        public double YoungPoolSoilCarbonBelowGroundEndOfYear { get; set; }

        /// <summary>
        /// Young pool manure at start of year
        /// 
        /// (kg C ha^-1)
        /// </summary>
        public double YoungPoolManureCarbon { get; set; }

        /// <summary>
        /// Young pool manure at end of year
        /// 
        /// (kg C ha^-1)
        /// </summary>
        public double YoungPoolManureCarbonEndOfYear { get; set; }

        public double YoungPoolSteadyStateManure { get; set; }
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
        /// Manure N inputs from field applied manure
        /// 
        /// (kg N ha^-1)
        /// </summary>
        public double ManureResidueN { get; set; }

        /// <summary>
        /// (kg N2O-N field^-1)
        /// </summary>
        public double DirectNitrousOxideEmissionsFromSyntheticNitrogenForArea { get; set; }

        /// <summary>
        /// (kg N2O-N field^-1)
        /// </summary>
        public double DirectNitrousOxideEmissionsFromCropResiduesForArea { get; set; }

        /// <summary>
        /// (kg N2O-N field^-1)
        /// </summary>
        public double DirectNitrousOxideEmissionsFromMineralizedNitrogenForArea { get; set; }

        /// <summary>
        /// (kg N2O-N field^-1)
        /// </summary>
        public double DirectNitrousOxideEmissionsFromOrganicNitrogenForArea { get; set; }

        /// <summary>
        /// (kg N2O-N field^-1)
        /// </summary>
        public double DirectNitrousOxideEmissionsFromOrganicNitrogenForAreaExcludingRemainingAmounts { get; set; }

        /// <summary>
        /// (kg N2O-N field^-1)
        /// </summary>
        public double TotalDirectNitrousOxideForArea { get; set; }

        /// <summary>
        /// (kg N2O-N ha^-1)
        /// </summary>
        public double TotalDirectN2ONFromCropExports { get; set; }

        /// <summary>
        /// (kg N2O-N ha^-1)
        /// </summary>
        public double TotalIndirectN2ONFromExportedManure { get; set; }

        /// <summary>
        /// (kg NO3-N ha^-1)
        /// </summary>
        public double TotalNO3NFromExportedManure { get; set; }

        /// <summary>
        /// (kg NH3-N ha^-1)
        /// </summary>
        public double TotalNH3NFromExportedManure { get; set; }

        /// <summary>
        /// (kg NO-N ha^-1)
        /// </summary>
        public double TotalNONFromExportedCropResidues { get; set; }

        /// <summary>
        /// (kg N2O-N field^-1)
        /// </summary>
        public double TotalNitrousOxideForArea { get; set; }

        /// <summary>
        /// (kg N2O-N field^-1)
        /// </summary>
        public double IndirectNitrousOxideEmissionsFromSyntheticNitrogenForArea { get; set; }

        /// <summary>
        /// (kg N2O-N field^-1)
        /// </summary>
        public double IndirectNitrousOxideEmissionsFromCropResiduesForArea { get; set; }

        /// <summary>
        /// (kg N2O-N field^-1)
        /// </summary>
        public double IndirectNitrousOxideEmissionsFromMineralizedNitrogenForArea { get; set; }

        /// <summary>
        /// Leaching
        /// 
        /// (kg N2O-N field^-1)
        /// </summary>
        public double IndirectNitrousOxideEmissionsFromOrganicNitrogenForArea { get; set; }

        /// <summary>
        /// Leaching
        /// 
        /// (kg N2O-N field^-1)
        /// </summary>
        public double IndirectNitrousOxideLeachingEmissionsFromOrganicNitrogenForAreaExcludingRemainingManure { get; set; }

        /// <summary>
        /// (kg N2O-N field^-1)
        /// </summary>
        public double IndirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForArea { get; set; }

        /// <summary>
        /// (kg N2O-N field^-1)
        /// </summary>
        public double IndirectNitrousOxideEmissionsFromVolatilizationOfOrganicNitrogenForArea { get; set; }
        
        /// <summary>
        /// (kg N2O-N field^-1)
        /// </summary>
        public double IndirectNitrousOxideEmissionsFromVolatilizationOfOrganicNitrogenForAreaExcludingRemainingAmounts { get; set; }

        /// <summary>
        /// (kg N2O-N field^-1)
        /// </summary>
        public double TotalIndirectNitrousOxideForArea { get; set; }

        /// <summary>
        /// (kg N2O-N field^-1)
        /// </summary>
        public double TotalIndirectNitrousOxideForAreaExcludingRemainingAmounts { get; set; }

        /// <summary>
        /// (kg NO-N field^-1)
        /// </summary>
        public double TotalNitricOxideForArea { get; set; }

        /// <summary>
        /// (kg NO-N field^-1)
        /// </summary>
        public double DirectNitricOxideEmissionsFromSyntheticNitrogenForArea { get; set; }

        /// <summary>
        /// (kg NO-N field^-1)
        /// </summary>
        public double DirectNitricOxideEmissionsFromCropResiduesForArea { get; set; }

        /// <summary>
        /// (kg NO-N field^-1)
        /// </summary>
        public double DirectNitricOxideEmissionsFromMineralizedNitrogenForArea { get; set; }

        /// <summary>
        /// (kg NO-N field^-1)
        /// </summary>
        public double DirectNitricOxideEmissionsFromOrganicNitrogenForArea { get; set; }

        /// <summary>
        /// (kg NO3-N field^-1)
        /// </summary>
        public double TotalNitrateLeachingForArea { get; set; }

        /// <summary>
        /// (kg NO3-N field^-1)
        /// </summary>
        public double IndirectNitrateFromSyntheticNitrogenForArea { get; set; }

        /// <summary>
        /// (kg NO3-N field^-1)
        /// </summary>
        public double IndirectNitrateFromCropResiduesForArea { get; set; }

        /// <summary>
        /// (kg NO3-N field^-1)
        /// </summary>
        public double IndirectNitrateFromMineralizedNitrogenForArea { get; set; }

        /// <summary>
        /// (kg NO3-N field^-1)
        /// </summary>
        public double IndirectNitrateFromOrganicNitrogenForArea { get; set; }

        /// <summary>
        /// (kg NH4-N field^-1)
        /// </summary>
        public double TotalAmmoniaForArea { get; set; }

        /// <summary>
        /// (kg NH4-N field^-1)
        /// </summary>
        public double IndirectAmmoniumEmissionsFromVolatilizationOfOrganicNitrogenForArea { get; set; }

        /// <summary>
        /// (kg NH4-N field^-1)
        /// </summary>
        public double IndirectAmmoniumEmissionsFromVolatilizationOfSyntheticNitrogenForArea { get; set; }

        /// <summary>
        /// (kg N2-N field^-1)
        /// </summary>
        public double DenitrificationForArea { get; set; }

        /// <summary>
        /// kg N ha^-1
        /// </summary>
        public double AboveGroundNitrogenResidueForCrop { get; set; }

        /// <summary>
        /// (kg N ha^-1)
        /// </summary>
        public double ExportedNitrogenResidueForCrop { get; set; }

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
        /// (kg N ha^-1)
        /// </summary>
        public double TotalNitrogenInputs { get; set; }

        /// <summary>
        /// Total organic nitrogen (residue) inputs for IPCC Tier 2 pool calculation. Includes above ground residue, below ground residue, and manure residues.
        ///
        /// (kg N ha^-1)
        /// </summary>
        public double TotalNitrogenInputsForIpccTier2 { get; set; }

        /// <summary>
        /// Sum total of all emission types for the field (N2O-N, NO-N, NO3-N, and NH4-N)
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

        public MonthlyValueBase<double> MonthlyIpccTier2WaterFactors
        {
            get => _monthlyIpccTier2WaterFactors;
            set => SetProperty(ref _monthlyIpccTier2WaterFactors, value);
        }

        public MonthlyValueBase<double> MonthlyIpccTier2TemperatureFactors
        {
            get => _monthlyIpccTier2TemperatureFactors;
            set => SetProperty(ref _monthlyIpccTier2TemperatureFactors, value);
        }

        public IPCCTier2Results IpccTier2CarbonResults
        {
            get
            {
                return _carbonResults;
            }
            set
            {
                SetProperty(ref _carbonResults, value);
            }
        }

        public IPCCTier2Results IpccTier2NitrogenResults
        {
            get
            {
                return _nitrogenResults;
            }
            set
            {
                SetProperty(ref _nitrogenResults, value);
            }
        }

        public double ActivePoolCarbon { get; set; }
        public double SlowPoolCarbon { get; set; }
        public double PassivePoolCarbon { get; set; }

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
        /// Nitrogen fraction of the carbon input (from IPCC Tier 2).
        /// 
        /// (unitless)
        /// </summary>
        public double NitrogenContent { get; set; }

        /// <summary>
        /// SOC stock at the end of the current year y for grid cell or region
        /// 
        /// (kg C ha^-1)
        /// </summary>
        public double Soc { get; set; }

        /// <summary>
        /// SOC-N stock at the end of the current year y for grid cell or region
        /// 
        /// (kg N ha^-1)
        /// </summary>
        public double SoilNitrogenStock { get; set; }

        /// <summary>
        /// Annual stock change factor for mineral soils in grid cell or region
        /// 
        /// (tonnes C ha^-1)
        /// </summary>
        public double SocDiff { get; set; }

        /// <summary>
        /// (kg ha^-1)
        /// </summary>
        public double AboveGroundResidueDryMatter { get; set; }

        /// <summary>
        /// (kg ha^-1)
        /// </summary>
        public double AboveGroundResidueDryMatterExported { get; set; }

        /// <summary>
        /// (kg ha^-1)
        /// </summary>
        public double BelowGroundResidueDryMatter { get; set; }

        /// <summary>
        /// (kg N2O ha^-1)
        /// </summary>
        public double TotalDirectNitrousOxidePerHectare { get; set; }

        /// <summary>
        /// (kg N2O ha^-1)
        /// </summary>
        public double TotalDirectNitrousOxidePerHectareExcludingRemainingAmounts { get; set; }

        /// <summary>
        /// (kg N2O ha^-1)
        /// </summary>
        public double TotalIndirectNitrousOxidePerHectare { get; set; }

        /// <summary>
        /// (kg N2O ha^-1)
        /// </summary>
        public double TotalIndirectNitrousOxidePerHectareExcludingRemainingAmounts { get; set; }

        public CropEnergyResults CropEnergyResults { get; set; }

        /// <summary>
        /// (kg NH3-N ha^-1)
        /// </summary>
        public double AdjustedAmmoniacalLossFromLandAppliedManurePerHectare { get; set; }

        public double FractionOfLandOccupiedByLowerPortionsOfLandscape { get; set; }
        public double WeightedModifierBasedOnTexture { get; set; }
        public double SocNRequirement { get; set; }
        public double MineralPool { get; set; }
        public double YoungPoolAboveGroundResidueN { get; set; }
        public double YoungPoolBelowGroundResidueN { get; set; }
        public double FractionOfNitrogenLostByLeachingAndRunoff { get; set; }

        #endregion
    }
}