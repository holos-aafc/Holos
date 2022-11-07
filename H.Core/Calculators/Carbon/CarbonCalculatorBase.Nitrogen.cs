using H.Core.Calculators.Nitrogen;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using H.Core.Services.Animals;
using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Providers.Animals;

namespace H.Core.Calculators.Carbon
{
    public abstract partial class CarbonCalculatorBase
    {
        #region Fields

        protected readonly SingleYearNitrousOxideCalculator _singleYearNitrousOxideCalculator = new SingleYearNitrousOxideCalculator();
        

        #endregion

        #region Properties

        public CropViewItem PreviousYearResults { get; set; }
        public CropViewItem CurrentYearResults { get; set; }
        public int YearIndex { get; set; }
        public int Year { get; set; }

        /// <summary>
        /// AG_N
        ///
        /// (kg N ha^-1)
        /// </summary>
        public double AboveGroundResidueN { get; set; }

        /// <summary>
        /// BGR_N
        /// 
        /// (kg N ha^-1)
        /// </summary>
        public double BelowGroundResidueN { get; set; }

        /// <summary>
        /// N_m
        /// </summary>
        public double ManurePool { get; set; }

        /// <summary>
        /// N_SN
        /// </summary>
        public double SyntheticNitrogenPool { get; set; }

        /// <summary>
        /// N_CropResidues
        /// </summary>
        public double CropResiduePool { get; set; }

        /// <summary>
        /// N_min
        /// </summary>
        public double MineralPool { get; set; }

        /// <summary>
        /// N_ON
        /// </summary>
        public double OrganicPool { get; set; }

        /// <summary>
        /// N_microbeN - Availability of N in the microbial pool
        ///
        /// (kg N ha^-1)
        /// </summary>
        public double MicrobePool { get; set; }

        /// <summary>
        /// Ratio between mineral and microbial N
        ///
        /// (unitless)
        /// </summary>
        public double PoolRatio { get; set; }

        public double CropNitrogenDemand { get; set; }

        /// <summary>
        /// N_microbeDeathN - Release of N from the microbial pool
        ///
        /// (kg N ha^-1)
        /// </summary>
        public double MicrobeDeathPool { get; set; }

        /// <summary>
        /// N_mineralN - Availability of mineral N on the field
        ///
        /// (kg N ha^-1)
        /// </summary>
        public double AvailabilityOfMineralN { get; set; }

        /// <summary>
        /// N_mineralNBalance - Interannual N balance in the mineral overflow pool on the field
        ///
        /// (kg N ha^-1)
        /// </summary>
        public double MineralNBalance { get; set; }

        /// <summary>
        /// N_microbeN - Interannual N balance in the microbe overflow pool on the field
        ///
        /// (kg N ha^-1)
        /// </summary>
        public double MicrobeNBalance { get; set; }

        /// <summary>
        /// N2-Nloss - Denitrification of mineral N as N2 on the field
        ///
        /// (kg N2-N ha^-1)
        /// </summary>
        public double N2Loss { get; set; }

        /// <summary>
        /// (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromSyntheticFertilizer { get; set; }

        /// <summary>
        /// (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromResidues { get; set; }

        /// <summary>
        /// (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromMineralization { get; set; }

        /// <summary>
        /// (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromOrganicNitrogen { get; set; }

        /// <summary>
        /// (kg NO-N ha^-1)
        /// </summary>
        public double NO_NFromSyntheticFertilizer { get; set; }

        /// <summary>
        /// (kg NO-N ha^-1)
        /// </summary>
        public double NO_NFromResidues { get; set; }

        /// <summary>
        /// (kg NO-N ha^-1)
        /// </summary>
        public double NO_NFromMineralization { get; set; }

        /// <summary>
        /// (kg NO-N ha^-1)
        /// </summary>
        public double NO_NFromOrganicNitrogen { get; set; }

        /// <summary>
        /// (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromSyntheticFertilizerLeaching { get; set; }

        /// <summary>
        /// (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromResiduesLeaching { get; set; }

        /// <summary>
        /// (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromMineralizationLeaching { get; set; }

        /// <summary>
        /// (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromOrganicNitrogenLeaching { get; set; }

        /// <summary>
        /// (kg NO3-N ha^-1)
        /// </summary>
        public double NO3FromSyntheticFertilizerLeaching { get; set; }

        /// <summary>
        /// (kg NO3-N ha^-1)
        /// </summary>
        public double NO3FromResiduesLeaching { get; set; }

        /// <summary>
        /// (kg NO3-M ha^-1)
        /// </summary>
        public double NO3FromMineralizationLeaching { get; set; }

        /// <summary>
        /// (kg NO3-N ha^-1)
        /// </summary>
        public double NO3FromOrganicNitrogenLeaching { get; set; }

        public double N2O_NSyntheticNitrogenVolatilization { get; set; }
        public double N2O_NOrganicNitrogenVolatilization { get; set; }

        public double NH4FromSyntheticNitogenVolatilized { get; set; }
        public double NH4FromOrganicNitogenVolatilized { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Equation 2.6.8-12
        /// Equation 2.7.7-12
        /// </summary>
        public double CalculateCropNitrogenDemand(
            double carbonInputFromProduct,
            double carbonInputFromStraw,
            double carbonInputFromRoots,
            double carbonInputFromExtraroots,
            double moistureContentOfCropFraction,
            double nitrogenConcentrationInTheProduct,
            double nitrogenConcentrationInTheStraw,
            double nitrogenConcentrationInTheRoots,
            double nitrogenConcentrationInExtraroots,
            double nitrogenFixation,
            double carbonConcentration)
        {
            return ((carbonInputFromProduct / carbonConcentration * (1 - moistureContentOfCropFraction)) *
                    nitrogenConcentrationInTheProduct +
                    (carbonInputFromStraw / carbonConcentration * (1 - moistureContentOfCropFraction)) *
                    nitrogenConcentrationInTheStraw +
                    (carbonInputFromRoots / carbonConcentration * (1 - moistureContentOfCropFraction)) *
                    nitrogenConcentrationInTheRoots +

                    (carbonInputFromExtraroots / carbonConcentration * (1 - moistureContentOfCropFraction)) *
                    nitrogenConcentrationInExtraroots) - (1 - nitrogenFixation);
        }

        #endregion

        #region Protected Methods

        protected void SetSyntheticNStartState(CropViewItem currentYearResults)
        {
            // Equation 2.6.1-1
            // Equation 2.7.1-1
            this.SyntheticNitrogenPool += currentYearResults.NitrogenFertilizerRate;

            // Equation 2.6.1-2
            // Equation 2.7.1-2
            this.SyntheticNitrogenPool += currentYearResults.NitrogenDepositionAmount;
        }

        protected void SetAvailabilityOfMineralNState(CropViewItem previousYearResults)
        {
            // Use the previous years' mineral N pool as the starting point for this year
            this.AvailabilityOfMineralN = previousYearResults.MineralNitrogenPool_N_mineralN;
        }

        protected void SetMicrobePoolStartingState(CropViewItem previousYearResults, int yearIndex)
        {
            // Use the previous years' microbial pool as the starting point for this year (or a starting value if in equilibrium year)
            if (yearIndex == 0)
            {
                // Microbe pool always starts with 25 kg N.
                this.MicrobePool = 25;
            }
            else
            {

                this.MicrobePool = previousYearResults.MicrobeNitrogenPool_N_microbeN;
            }
        }

        protected void SetPoolStartStates(Farm farm)
        {
            if (this.YearIndex == 0)
            {
                this.SyntheticNitrogenPool = 0d;
                this.CropResiduePool = 0d;
                this.MineralPool = 0d;
                this.OrganicPool = 0d;
                this.MicrobePool = 0d;
            }

            this.SetSyntheticNStartState(this.CurrentYearResults);
            this.SetAvailabilityOfMineralNState(this.PreviousYearResults);
            this.SetMicrobePoolStartingState(this.PreviousYearResults, this.YearIndex);

            // Manure pool must be set before crop residue, and organic nitrogen pool initializations
            this.SetManurePoolStartState(farm);
            this.SetOrganicNitrogenPoolStartState();
            this.SetCropResiduesStartState(farm);
        }

        /// <summary>
        /// Add to total input summation before the pools are reduced. Note that the crop residue N pool is added as an input and not the AGresidueN or the BGresidueN pools
        /// </summary>
        protected void TotalInputsBeforeReductions()
        {
            this.CurrentYearResults.TotalNitrogenInputs = 0;

            // Equation 2.6.9-28
            // Equation 2.7.8-30
            this.CurrentYearResults.TotalNitrogenInputs += this.SyntheticNitrogenPool;
            this.CurrentYearResults.TotalNitrogenInputs += this.CropResiduePool;
            this.CurrentYearResults.TotalNitrogenInputs += this.MineralPool;
            this.CurrentYearResults.TotalNitrogenInputs += this.OrganicPool;

            // Display the pools before they are adjusted
            this.CurrentYearResults.SyntheticInputsBeforeAdjustment = this.SyntheticNitrogenPool;
            this.CurrentYearResults.CropResiduesBeforeAdjustment = this.CropResiduePool;
            this.CurrentYearResults.OrganicNitrogenResiduesBeforeAdjustment = this.OrganicPool;
            this.CurrentYearResults.N_min_FromDecompositionOfOldCarbon = this.MineralPool;
        }

        protected void CalculateNitrousOxide(CropViewItem currentYearResults, Farm farm)
        {
            var emissionFactorForSyntheticFertilizer =
                _singleYearNitrousOxideCalculator.CalculateSyntheticNitrogenEmissionFactor(currentYearResults, farm);
            var emissionFactorForCropResidues =
                _singleYearNitrousOxideCalculator.GetEmissionFactorForCropResidues(currentYearResults, farm);
            var emissionFactorForOrganicNitrogen =
                _singleYearNitrousOxideCalculator.CalculateOrganicNitrogenEmissionFactor(currentYearResults, farm);
            var directN2ONFromLandAppliedManure =
                _singleYearNitrousOxideCalculator.CalculateDirectN2ONEmissionsFromFieldSpecificManureSpreading(
                    currentYearResults, farm);

            // Equation 2.6.5-1
            // Equation 2.7.4-1
            this.N2O_NFromSyntheticFertilizer = this.SyntheticNitrogenPool * emissionFactorForSyntheticFertilizer;

            // Equation 2.6.5-2
            // Equation 2.7.4-2
            this.N2O_NFromResidues = this.CropResiduePool * emissionFactorForCropResidues;

            // TODO: calculate emissions from exports

            // Equation 2.6.5-4
            // Equation 2.7.4-4
            this.N2O_NFromMineralization = this.MineralPool * emissionFactorForCropResidues;

            // Equation 2.6.5-5
            // Equation 2.7.4-5
            this.N2O_NFromOrganicNitrogen =
                (this.OrganicPool * emissionFactorForOrganicNitrogen) + directN2ONFromLandAppliedManure;
        }

        protected void CalculateNitricOxide(double nORatio)
        {
            // Equation 2.6.5-7
            // Equation 2.7.4-7
            this.NO_NFromSyntheticFertilizer = this.N2O_NFromSyntheticFertilizer * nORatio;

            // Equation 2.6.5-8
            // Equation 2.7.4-8
            this.NO_NFromResidues = this.N2O_NFromResidues * nORatio;

            // Equation 2.6.5-10
            // Equation 2.7.4-10
            this.NO_NFromMineralization = this.N2O_NFromMineralization * nORatio;

            // Equation 2.6.5-11
            // Equation 2.7.4-11
            this.NO_NFromOrganicNitrogen = this.N2O_NFromOrganicNitrogen * nORatio;
        }

        protected void CalculateLeachingEmissions(
            double fractionLeach,
            LandApplicationEmissionResult indirectEmissionsFromLandAppliedManure,
            double emissionFactorLeaching)
        {
            // Equation 2.6.6-3
            // Equation 2.7.5-3
            this.N2O_NFromSyntheticFertilizerLeaching =
                this.SyntheticNitrogenPool * fractionLeach * emissionFactorLeaching;

            // Equation 2.6.6-4
            // Equation 2.7.5-4
            this.N2O_NFromResiduesLeaching = this.CropResiduePool * fractionLeach * emissionFactorLeaching;

            // Equation 2.6.6-5
            // Equation 2.7.5-5
            this.N2O_NFromMineralizationLeaching = this.MineralPool * fractionLeach * emissionFactorLeaching;

            // Equation 2.6.6-6
            // Equation 2.7.5-6
            this.N2O_NFromOrganicNitrogenLeaching = (this.OrganicPool * fractionLeach * emissionFactorLeaching) +
                                                    indirectEmissionsFromLandAppliedManure.TotalN2ONFromManureLeaching;
        }

        protected void CalculateActualAmountsLeached(
            double fractionLeach,
            LandApplicationEmissionResult indirectEmissionsFromLandAppliedManure,
            double emissionFactorLeaching)
        {
            // Equation 2.6.6-7
            // Equation 2.7.5-7
            this.NO3FromSyntheticFertilizerLeaching =
                this.SyntheticNitrogenPool * fractionLeach * (1 - emissionFactorLeaching);

            // Equation 2.6.6-8
            // Equation 2.7.5-8
            this.NO3FromResiduesLeaching = this.CropResiduePool * fractionLeach * (1 - emissionFactorLeaching);

            // Equation 2.6.6-9
            // Equation 2.7.5-9
            this.NO3FromMineralizationLeaching = this.MineralPool * fractionLeach * (1 - emissionFactorLeaching);

            // Equation 2.6.6-10
            // Equation 2.7.5-10
            this.NO3FromOrganicNitrogenLeaching = (this.OrganicPool * fractionLeach * (1 - emissionFactorLeaching)) +
                                                  indirectEmissionsFromLandAppliedManure.TotalNitrateLeached;
        }

        protected void CalculateVolatilization(
            double volatilizationFraction,
            double volatilizationEmissionFactor,
            LandApplicationEmissionResult totalIndirectEmissionsFromLandAppliedManure)
        {
            // Equation 2.6.6-12
            // Equation 2.7.5-12
            this.N2O_NSyntheticNitrogenVolatilization =
                this.SyntheticNitrogenPool * volatilizationFraction * volatilizationEmissionFactor;

            // Equation 2.6.6-13
            // Equation 2.7.5-13
            this.N2O_NOrganicNitrogenVolatilization =
                (this.OrganicPool * volatilizationFraction * volatilizationEmissionFactor) +
                totalIndirectEmissionsFromLandAppliedManure.TotalN2ONFromManureVolatilized;
        }

        protected void CalculateActualVolatilization(
            double volatilizationFraction,
            double volatilizationEmissionFactor,
            LandApplicationEmissionResult totalIndirectEmissionsFromLandAppliedManure)
        {
            // Equation 2.6.6-14
            // Equation 2.7.5-14
            this.NH4FromSyntheticNitogenVolatilized = this.SyntheticNitrogenPool * volatilizationFraction *
                                                      (1 - volatilizationEmissionFactor);

            // Equation 2.6.6-15
            // Equation 2.7.5-15
            this.NH4FromOrganicNitogenVolatilized =
                (this.OrganicPool * volatilizationFraction * (1 - volatilizationEmissionFactor)) +
                totalIndirectEmissionsFromLandAppliedManure.AdjustedAmmoniacalLoss;
        }

        protected void AdjustSyntheticNitrogenPool()
        {
            // Equation 2.6.6-1
            // Equation 2.7.6-1
            this.SyntheticNitrogenPool -= (this.N2O_NFromSyntheticFertilizer + this.NO_NFromSyntheticFertilizer);

            // Equation 2.6.6-2
            // Equation 2.7.6-2
            this.SyntheticNitrogenPool -=
                (this.N2O_NFromSyntheticFertilizerLeaching + NO3FromSyntheticFertilizerLeaching);

            // Equation 2.6.6-3
            // Equation 2.7.6-3
            this.SyntheticNitrogenPool -=
                (this.N2O_NSyntheticNitrogenVolatilization + this.NH4FromSyntheticNitogenVolatilized);
        }

        protected void AdjustResiduePool()
        {
            // Equation 2.6.7-4
            // Equation 2.7.6-4
            this.CropResiduePool -= (this.N2O_NFromResidues + this.NO_NFromResidues);

            // Equation 2.6.7-5
            // Equation 2.7.6-5
            this.CropResiduePool -= (this.N2O_NFromResiduesLeaching + this.NO3FromResiduesLeaching);
        }

        protected void AdjustMineralizedNitrogenPool()
        {
            // Equation 2.6.7-6
            // Equation 2.7.6-6
            this.MineralPool -= (this.N2O_NFromMineralization + this.NO_NFromMineralization);

            // Equation 2.6.7-7
            // Equation 2.7.6-7
            this.MineralPool -= (N2O_NFromMineralizationLeaching + NO3FromMineralizationLeaching);
        }

        protected void AdjustOrganicPool()
        {
            // Equation 2.6.7-8
            // Equation 2.7.6-8
            this.OrganicPool -= (this.N2O_NFromOrganicNitrogen + this.NO_NFromOrganicNitrogen);

            // Equation 2.6.7-9
            // Equation 2.7.6-9
            this.OrganicPool -= (this.N2O_NFromOrganicNitrogenLeaching + this.NO3FromOrganicNitrogenLeaching);

            // Equation 2.6.7-10
            // Equation 2.7.6-10
            this.OrganicPool -= (this.N2O_NOrganicNitrogenVolatilization + this.NH4FromOrganicNitogenVolatilized);
        }

        public void AdjustPoolsAfterDemandCalculation(double nitrogenDemand)
        {
            if ((Math.Abs(this.AvailabilityOfMineralN) < double.Epsilon) ||
                this.MicrobePool > this.AvailabilityOfMineralN)
            {
                // Equation 2.6.8-10
                // Equation 2.7.7-15
                this.AvailabilityOfMineralN -= nitrogenDemand * this.PoolRatio;

                // Equation 2.6.8-11
                // Equation 2.7.7-16
                this.MicrobePool -= nitrogenDemand * (1 - this.PoolRatio);
            }
            else
            {
                // Equation 2.6.8-8
                // Equation 2.7.7-13
                this.AvailabilityOfMineralN -= nitrogenDemand * (1 - this.PoolRatio);

                // Equation 2.6.8-9
                // Equation 2.7.7-14
                this.MicrobePool -= nitrogenDemand * this.PoolRatio;
            }
        }

        protected void BalancePools(double microbeDeath)
        {
            if (this.MicrobePool > 0)
            {
                // Equation 2.6.8-17
                // Equation 2.7.7-17
                this.MicrobeDeathPool = this.MicrobePool * microbeDeath;
            }
            else
            {
                // Equation 2.6.8-18
                // Equation 2.7.7-18
                this.MicrobeDeathPool = 0;
            }

            // Equation 2.6.8-19
            // Equation 2.7.7-19
            this.MicrobePool -= this.MicrobeDeathPool;

            // Equation 2.6.8-20
            // Equation 2.7.7-20
            this.AvailabilityOfMineralN += this.MicrobeDeathPool;

            if (this.AvailabilityOfMineralN > 0)
            {
                // Equation 2.6.8-21
                // Equation 2.7.7-21
                this.N2Loss = this.AvailabilityOfMineralN - ((this.N2O_NFromSyntheticFertilizer +
                                                              N2O_NFromResidues +
                                                              N2O_NFromMineralization +
                                                              N2O_NFromOrganicNitrogen +
                                                              N2O_NFromSyntheticFertilizerLeaching +
                                                              N2O_NFromResiduesLeaching +
                                                              N2O_NFromMineralizationLeaching +
                                                              N2O_NFromOrganicNitrogenLeaching +
                                                              N2O_NSyntheticNitrogenVolatilization +
                                                              N2O_NOrganicNitrogenVolatilization) * (0.92 / 0.08));
            }
            else
            {
                // Equation 2.6.8-22
                // Equation 2.7.7-22
                this.N2Loss = 0;
            }

            // Equation 2.6.8-23
            // Equation 2.7.7-23
            this.AvailabilityOfMineralN -= this.N2Loss;

            if (this.YearIndex == 0)
            {
                // Equation 2.6.8-24
                // Equation 2.7.7-24
                this.MineralNBalance = this.AvailabilityOfMineralN;

                // Equation 2.6.8-25
                // Equation 2.7.7-25
                this.MicrobeNBalance = this.MicrobePool - 25;
            }
            else
            {
                // Equation 2.6.8-26
                // Equation 2.7.7-26
                this.MineralNBalance =
                    this.AvailabilityOfMineralN - this.PreviousYearResults.MineralNitrogenPool_N_mineralN;

                // Equation 2.6.8-27
                // Equation 2.7.7-27
                this.MicrobeNBalance = this.MicrobePool - this.PreviousYearResults.MicrobeNitrogenPool_N_microbeN;
            }

            if (this.AvailabilityOfMineralN < 0)
            {
                // Equation 2.6.8-28
                // Equation 2.7.7-28
                this.MicrobePool += this.AvailabilityOfMineralN;

                // Equation 2.6.8-29
                // Equation 2.7.7-29
                this.AvailabilityOfMineralN = 0;
            }
        }

        protected void CalculateIndirectEmissions(Farm farm, CropViewItem currentYearResults)
        {
            var fractionLeach = _singleYearNitrousOxideCalculator.CalculateFractionOfNitrogenLostByLeachingAndRunoff(
                farm.ClimateData.PrecipitationData.GrowingSeasonPrecipitation,
                farm.ClimateData.EvapotranspirationData.GrowingSeasonEvapotranspiration);

            var totalIndirectEmissionsFromLandAppliedManure = _singleYearNitrousOxideCalculator.CalculateTotalIndirectEmissionsFromFieldSpecificManureSpreading(this.CurrentYearResults, this.AnimalComponentEmissionsResults, farm);
                    var emissionFactorLeaching = farm.Defaults.EmissionFactorForLeachingAndRunoff;

            this.CalculateLeachingEmissions(fractionLeach, totalIndirectEmissionsFromLandAppliedManure,
                emissionFactorLeaching);
            this.CalculateActualAmountsLeached(fractionLeach, totalIndirectEmissionsFromLandAppliedManure,
                emissionFactorLeaching);

            var volatilizationFractionSoil =
                _singleYearNitrousOxideCalculator.CalculateFractionOfNitrogenLostByVolatilization(currentYearResults,
                    farm);
            var emissionFactorForVolatilization = farm.Province.GetRegion() == Region.WesternCanada ? 0.005 : 0.014;

            this.CalculateVolatilization(volatilizationFractionSoil, emissionFactorForVolatilization,
                totalIndirectEmissionsFromLandAppliedManure);
            this.CalculateActualVolatilization(volatilizationFractionSoil, emissionFactorForVolatilization,
                totalIndirectEmissionsFromLandAppliedManure);
        }

        protected void CalculateDirectEmissions(Farm farm, CropViewItem currentYearResults)
        {
            this.CalculateNitrousOxide(currentYearResults, farm);
            this.CalculateNitricOxide(farm.Defaults.NORatio);
        }

        protected void AdjustPools()
        {
            this.AdjustSyntheticNitrogenPool();
            this.AdjustResiduePool();
            this.AdjustMineralizedNitrogenPool();
            this.AdjustOrganicPool();
        }

        protected void CloseNitrogenBudget(CropViewItem currentYearResults)
        {
            // Equation 2.6.8-1
            // Equation 2.7.7-1
            this.MicrobePool += this.SyntheticNitrogenPool;

            // Equation 2.6.8-2
            // Equation 2.7.7-2
            this.MicrobePool += this.CropResiduePool;

            // Equation 2.6.8-3
            // Equation 2.7.7-3
            this.MicrobePool += this.MineralPool;

            // Equation 2.6.8-4
            // Equation 2.7.7-4
            this.MicrobePool += this.OrganicPool;

            currentYearResults.MicrobialPoolAfterCloseOfBudget = this.MicrobePool;

            // See section 2.6.8.1
            this.SyntheticNitrogenPool = 0;
            this.CropResiduePool = 0;
            this.MineralPool = 0;
            this.OrganicPool = 0;
        }

        protected void CalculatePoolRatio()
        {
            var absoluteMineralN = Math.Abs(this.AvailabilityOfMineralN);
            var absoluteMicrobeN = Math.Abs(this.MicrobePool);

            if (absoluteMineralN > absoluteMicrobeN)
            {
                // Equation 2.6.8-5
                // Equation 2.7.7-5
                this.PoolRatio = Math.Abs(1.0 / (absoluteMineralN / absoluteMicrobeN));
            }

            if (absoluteMicrobeN > absoluteMineralN)
            {
                // Equation 2.6.8-6
                // Equation 2.7.7-6
                this.PoolRatio = Math.Abs(absoluteMineralN / absoluteMicrobeN);
            }

            this.CurrentYearResults.Ratio = this.PoolRatio;
        }

        protected void SumNitrousOxide()
        {
            this.CurrentYearResults.TotalNitrogenEmissions = 0;

            // Equation 2.6.9-1
            // Equation 2.7.8-1
            var totalDirectNitrousOxide = this.N2O_NFromSyntheticFertilizer +
                                          this.N2O_NFromResidues +
                                          this.N2O_NFromMineralization +
                                          this.N2O_NFromOrganicNitrogen;

            // Equation 2.6.9-2
            // Equation 2.7.8-2
            var totalIndirectNitrousOxide = this.N2O_NFromSyntheticFertilizerLeaching +
                                            this.N2O_NFromResiduesLeaching +
                                            this.N2O_NFromMineralizationLeaching +
                                            this.N2O_NFromOrganicNitrogenLeaching +
                                            this.N2O_NSyntheticNitrogenVolatilization +
                                            this.N2O_NOrganicNitrogenVolatilization;

            // Equation 2.7.9-3
            // Equation 2.7.8-3
            this.CurrentYearResults.TotalNitrogenEmissions += (totalDirectNitrousOxide + totalIndirectNitrousOxide);

            var area = this.CurrentYearResults.Area;

            // Equation 2.7.9-4
            // Equation 2.7.8-4
            this.CurrentYearResults.TotalNitrousOxideForArea = this.CurrentYearResults.TotalNitrogenEmissions * area;

            this.CurrentYearResults.DirectNitrousOxideEmissionsFromSyntheticNitrogenForArea =
                this.N2O_NFromSyntheticFertilizer * area;
            this.CurrentYearResults.DirectNitrousOxideEmissionsFromCropResiduesForArea = this.N2O_NFromResidues * area;
            this.CurrentYearResults.DirectNitrousOxideEmissionsFromMineralizedNitrogenForArea =
                this.N2O_NFromMineralization * area;
            this.CurrentYearResults.DirectNitrousOxideEmissionsFromOrganicNitrogenForArea =
                this.N2O_NFromOrganicNitrogen * area;

            this.CurrentYearResults.TotalDirectNitrousOxidePerHectare =
                totalDirectNitrousOxide * CoreConstants.ConvertN2ONToN2O;
            this.CurrentYearResults.TotalDirectNitrousOxideForArea = totalDirectNitrousOxide * area;

            this.CurrentYearResults.IndirectNitrousOxideEmissionsFromSyntheticNitrogenForArea =
                this.N2O_NFromSyntheticFertilizerLeaching * area;
            this.CurrentYearResults.IndirectNitrousOxideEmissionsFromCropResiduesForArea =
                this.N2O_NFromResiduesLeaching * area;
            this.CurrentYearResults.IndirectNitrousOxideEmissionsFromMineralizedNitrogenForArea =
                this.N2O_NFromMineralizationLeaching * area;
            this.CurrentYearResults.IndirectNitrousOxideEmissionsFromOrganicNitrogenForArea =
                this.N2O_NFromOrganicNitrogenLeaching * area;
            this.CurrentYearResults.IndirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForArea =
                this.N2O_NSyntheticNitrogenVolatilization * area;
            this.CurrentYearResults.IndirectNitrousOxideEmissionsFromVolatilizationOfOrganicNitrogenForArea =
                this.N2O_NOrganicNitrogenVolatilization * area;

            this.CurrentYearResults.TotalIndirectNitrousOxidePerHectare =
                totalIndirectNitrousOxide * CoreConstants.ConvertN2ONToN2O;
            this.CurrentYearResults.TotalIndirectNitrousOxideForArea = totalIndirectNitrousOxide * area;
        }

        protected void SumNitricOxide()
        {
            // Equation 2.6.9-5
            // Equation 2.7.8-5
            var totalNitricOxide = this.NO_NFromSyntheticFertilizer +
                                   this.NO_NFromResidues +
                                   this.NO_NFromMineralization +
                                   this.NO_NFromOrganicNitrogen;

            this.CurrentYearResults.TotalNitrogenEmissions += totalNitricOxide;

            var area = this.CurrentYearResults.Area;

            // Equation 2.6.9-6
            // Equation 2.7.8-6
            this.CurrentYearResults.TotalNitricOxideForArea = totalNitricOxide * area;

            this.CurrentYearResults.DirectNitricOxideEmissionsFromSyntheticNitrogenForArea =
                this.NO_NFromSyntheticFertilizer * area;
            this.CurrentYearResults.DirectNitricOxideEmissionsFromCropResiduesForArea = this.NO_NFromResidues * area;
            this.CurrentYearResults.DirectNitricOxideEmissionsFromMineralizedNitrogenForArea =
                this.NO_NFromMineralization * area;
            this.CurrentYearResults.DirectNitricOxideEmissionsFromOrganicNitrogenForArea =
                this.NO_NFromOrganicNitrogen * area;
        }

        protected void SumNitrateLeaching()
        {
            // Equation 2.6.9-7
            // Equation 2.7.8-7
            var totalNitrateLeaching = this.NO3FromSyntheticFertilizerLeaching +
                                       this.NO3FromResiduesLeaching +
                                       this.NO3FromMineralizationLeaching +
                                       this.NO3FromOrganicNitrogenLeaching;

            this.CurrentYearResults.TotalNitrogenEmissions += totalNitrateLeaching;

            var area = this.CurrentYearResults.Area;

            // Equation 2.6.9-8
            // Equation 2.7.8-8
            this.CurrentYearResults.TotalNitrateLeachingForArea = totalNitrateLeaching * area;

            this.CurrentYearResults.IndirectNitrateFromSyntheticNitrogenForArea =
                this.NO3FromSyntheticFertilizerLeaching * area;
            this.CurrentYearResults.IndirectNitrateFromCropResiduesForArea = this.NO3FromResiduesLeaching * area;
            this.CurrentYearResults.IndirectNitrateFromMineralizedNitrogenForArea =
                this.NO3FromMineralizationLeaching * area;
            this.CurrentYearResults.IndirectNitrateFromOrganicNitrogenForArea =
                this.NO3FromOrganicNitrogenLeaching * area;
        }

        protected void SumAmmoniaVolatilization()
        {
            // Equation 2.6.9-9
            // Equation 2.7.8-9
            var totalAmmoniaVolatilization = this.NH4FromSyntheticNitogenVolatilized +
                                             this.NH4FromOrganicNitogenVolatilized;

            this.CurrentYearResults.TotalNitrogenEmissions += totalAmmoniaVolatilization;

            var area = this.CurrentYearResults.Area;

            // Equation 2.7.8-10
            this.CurrentYearResults.TotalAmmoniaForArea = totalAmmoniaVolatilization * area;

            this.CurrentYearResults.IndirectAmmoniumEmissionsFromVolatilizationOfSyntheticNitrogenForArea =
                this.NH4FromSyntheticNitogenVolatilized * area;
            this.CurrentYearResults.IndirectAmmoniumEmissionsFromVolatilizationOfOrganicNitrogenForArea =
                this.NH4FromOrganicNitogenVolatilized * area;
        }

        protected void SumEmissions()
        {
            this.SumNitrousOxide();
            this.SumNitricOxide();
            this.SumNitrateLeaching();
            this.SumAmmoniaVolatilization();

            // Equation 2.7.8-11
            this.CurrentYearResults.DenitrificationForArea = this.N2Loss * this.CurrentYearResults.Area;
        }

        protected virtual void AssignFinalValues()
        {
            this.CurrentYearResults.MicrobeDeath = this.MicrobeDeathPool;
            this.CurrentYearResults.MineralizedNitrogenPool_N_min = this.MineralPool;
            this.CurrentYearResults.OrganicNitrogenPool_N_ON = this.OrganicPool;
            this.CurrentYearResults.ManureResiduePool_ManureN = this.ManurePool;
            this.CurrentYearResults.CropResidueNitrogenPool_N_CropResidues = this.CropResiduePool;
            this.CurrentYearResults.AboveGroundResiduePool_AGresidueN = this.AboveGroundResidueN;
            this.CurrentYearResults.BelowGroundResiduePool_BGresidueN = this.BelowGroundResidueN;
            this.CurrentYearResults.SyntheticNitrogenPool_N_SN = this.SyntheticNitrogenPool;
            this.CurrentYearResults.CropNitrogenDemand = this.CropNitrogenDemand;
            this.CurrentYearResults.MicrobeNitrogenPool_N_microbeN = this.MicrobePool;
            this.CurrentYearResults.MineralNitrogenPool_N_mineralN = this.AvailabilityOfMineralN;

            this.CurrentYearResults.SumOfMineralAndMicrobialPools = this.AvailabilityOfMineralN + this.MicrobePool;
            this.CurrentYearResults.MineralNitrogenBalance = this.MineralNBalance;
            this.CurrentYearResults.MicrobialNitrogenBalance = this.MicrobeNBalance;

            // Equation 2.6.9-31
            // Equation 2.7.8-33
            this.CurrentYearResults.TotalNitrogenOutputs = this.CurrentYearResults.TotalUptake +
                                                           this.CurrentYearResults.TotalNitrogenEmissions;
            this.CurrentYearResults.DifferenceBetweenInputsAndOutputs = this.CurrentYearResults.TotalNitrogenInputs -
                                                                        this.CurrentYearResults.TotalNitrogenOutputs;

            // Equation 2.6.9-32
            // Equation 2.7.8-34
            this.CurrentYearResults.Overflow = this.AvailabilityOfMineralN + this.MicrobePool - this.N2Loss;
        }

        /// <summary>
        /// Equation 4.7.2-1
        /// </summary>
        /// <param name="totalNitrogenAppliedToField"></param>
        /// <param name="totalDirectN2ON"></param>
        /// <param name="totalAmmoniaLossFromLandapplication"></param>
        /// <param name="totalN2ONFromLeaching"></param>
        /// <returns>Fraction of N in field-applied manure (kg N 1000 kg^-1 wet weight)</returns>
        protected double CalculateAmountOfNitrogenAppliedToSoilAfterLosses(double totalNitrogenAppliedToField,
            double totalDirectN2ON,
            double totalAmmoniaLossFromLandapplication,
            double totalN2ONFromLeaching)
        {
            var result = totalNitrogenAppliedToField -
                         (totalDirectN2ON + totalAmmoniaLossFromLandapplication + totalN2ONFromLeaching);

            return result;
        }

        protected double GetManureNitrogenResiduesForYear(Farm farm, CropViewItem cropViewItem)
        {
            var totalDirectN2ONFromLandAppliedManure =
                _singleYearNitrousOxideCalculator.CalculateDirectN2ONEmissionsFromFieldSpecificManureSpreading(
                    cropViewItem, farm);
            var totalIndirectEmissionsFromLandAppliedManure =
                _singleYearNitrousOxideCalculator.CalculateTotalIndirectEmissionsFromFieldSpecificManureSpreading(cropViewItem, this.AnimalComponentEmissionsResults, farm);

            var totalManureLeachingN2ON = totalIndirectEmissionsFromLandAppliedManure.TotalN2ONFromManureLeaching;
            var ammoniacalLoss = totalIndirectEmissionsFromLandAppliedManure.AmmoniacalLoss;
            var totalNitrogenAvailableForLandApplication = totalIndirectEmissionsFromLandAppliedManure
                .ActualAmountOfNitrogenAppliedFromLandApplication;

            var fractionOfNitrogenAppliedToSoil = CalculateAmountOfNitrogenAppliedToSoilAfterLosses(
                totalNitrogenAppliedToField: totalNitrogenAvailableForLandApplication,
                totalDirectN2ON: totalDirectN2ONFromLandAppliedManure,
                totalAmmoniaLossFromLandapplication: ammoniacalLoss,
                totalN2ONFromLeaching: totalManureLeachingN2ON);

            var result = fractionOfNitrogenAppliedToSoil / cropViewItem.Area;

            return result;
        }

        #endregion

        #region Abstract Methods

        protected abstract void SetCropResiduesStartState(Farm farm);
        protected abstract void SetManurePoolStartState(Farm farm);
        protected abstract void SetOrganicNitrogenPoolStartState();

        #endregion

        #region Private Methods

        #endregion

        /// <summary>
        /// Equation 2.1.2-34
        /// </summary>
        public double CalculateInputsFromSupplementalHayFedToGrazingAnimals(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            CropViewItem nextYearViewItems,
            Farm farm)
        {
            var result = 0.0;

            // Get total amount of supplemental hay added
            var hayImportViewItems = currentYearViewItem.HayImportViewItems;
            foreach (var hayImportViewItem in hayImportViewItems)
            {
                // Total dry matter weight
                var totalDryMatterWeight = hayImportViewItem.GetTotalDryMatterWeightOfAllBales();

                // Amount lost during feeding
                var loss = farm.Defaults.DefaultSupplementalFeedingLossPercentage / 100;

                // Total additional carbon that must be added to above ground inputs for the field - NOTE: moisture content is already considered in the above method call and so it
                // is not included here as it is in the equation from the algorithm docuemtn
                var totalCarbon = (totalDryMatterWeight * (1 - loss)) * currentYearViewItem.CarbonConcentration;

                result += totalCarbon;
            }

            return result;
        }
    }
}