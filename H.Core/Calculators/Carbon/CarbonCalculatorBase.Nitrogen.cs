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
using H.Core.Providers.Climate;

namespace H.Core.Calculators.Carbon
{
    public abstract partial class CarbonCalculatorBase
    {
        #region Fields

        public N2OEmissionFactorCalculator N2OEmissionFactorCalculator { get; set; }
        protected IClimateProvider _climateProvider;

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
        /// (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromOrganicNitrogenExcludeRemainingAmounts { get; set; }

        /// <summary>
        /// (kg NO-N ha^-1)
        /// </summary>
        public double NO_NFromSyntheticFertilizer { get; set; }

        /// <summary>
        /// (kg NO-N ha^-1)
        /// </summary>
        public double NO_NFromResidues { get; set; }

        public double N2O_NFromExportedNitrogen { get; set; }

        /// <summary>
        /// (kg NO-N ha^-1)
        /// </summary>
        public double NO_NFromMineralization { get; set; }

        /// <summary>
        /// (kg NO-N ha^-1)
        /// </summary>
        public double NO_NFromOrganicNitrogen { get; set; }

        /// <summary>
        /// (kg NO-N ha^-1)
        /// </summary>
        public double NO_NFromOrganicNitrogenExcludingRemainingAmounts { get; set; }

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
        /// (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromOrganicNitrogenLeachingExcludingRemainingAmounts { get; set; }

        public double N2O_NFromOrganicNitrogenLeachingExported { get; set; }

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

        /// <summary>
        /// (kg NO3-N)
        /// </summary>
        public double NO3FromOrganicNitrogenLeachingExported { get; set; }

        public double N2O_NSyntheticNitrogenVolatilization { get; set; }
        public double N2O_NOrganicNitrogenVolatilization { get; set; }
        public double N2O_NOrganicNitrogenVolatilizationExcludingRemainingAmounts { get; set; }
        public double N2O_NFromVolatilizationForExports { get; set; }

        /// <summary>
        /// (kg NH4-N ha^-1)
        /// </summary>
        public double NH4FromSyntheticNitogenVolatilized { get; set; }
        public double NH4FromOrganicNitogenVolatilized { get; set; }

        /// <summary>
        /// (kg NH3-N)
        /// </summary>
        public double NH4FromExports { get; set; }

        /// <summary>
        /// (kg NH3-N ha^-1)
        /// </summary>
        public double AdjustedAmmoniacalLossFromLandAppliedManureAndDigestate { get; set; }

        #endregion

        #region Public Methods



        /// <summary>
        /// Equation 2.6.8-12
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

            // Equation 2.6.9-33
            // Equation 2.7.8-28
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

        protected void CalculateDirectNitrousOxide(CropViewItem currentYearResults, Farm farm)
        {
            var emissionFactorForSyntheticFertilizer = N2OEmissionFactorCalculator.CalculateSyntheticNitrogenEmissionFactor(currentYearResults, farm);
            var emissionFactorForCropResidues = N2OEmissionFactorCalculator.GetEmissionFactorForCropResidues(currentYearResults, farm);
            var emissionFactorForOrganicNitrogen = N2OEmissionFactorCalculator.CalculateOrganicNitrogenEmissionFactor(currentYearResults, farm);

            // Emissions from land applied manure
            var directN2ONFromLandAppliedManure = N2OEmissionFactorCalculator.CalculateDirectN2ONFromFieldAppliedManure(farm, currentYearResults, includeRemainingAmounts: true);
            var directN2ONFromLandAppliedManureExcludingRemaining = N2OEmissionFactorCalculator.CalculateDirectN2ONFromFieldAppliedManure(farm, currentYearResults, includeRemainingAmounts: false);

            var directN2ONFromGrazingAnimals  = N2OEmissionFactorCalculator.GetDirectN2ONFromGrazingAnimals(farm, currentYearResults, this.AnimalComponentEmissionsResults);

            // Emissions from land applied digestate
            var directN2ONFromLandAppliedDigestate = N2OEmissionFactorCalculator.CalculateDirectN2ONEmissionsFromFieldSpecificDigestateSpreading(currentYearResults, farm);
            var directN2ONFromLandAppliedDigestateNotAppliedToAnyField = N2OEmissionFactorCalculator.CalculateDirectN2ONFromLeftOverDigestateForField(currentYearResults, farm);

            // Equation 2.6.5-1
            // Equation 2.7.4-1
            this.N2O_NFromSyntheticFertilizer = this.SyntheticNitrogenPool * emissionFactorForSyntheticFertilizer;

            // Equation 2.6.5-2
            // Equation 2.7.4-2
            this.N2O_NFromResidues = this.CropResiduePool * emissionFactorForCropResidues;

            // Equation 2.6.5-3
            // Equation 2.7.4-3
            // Not implemented.

            // Equation 2.6.5-4
            // Equation 2.7.4-4
            this.N2O_NFromMineralization = this.MineralPool * emissionFactorForCropResidues;

            // Equation 2.6.5-5
            // Equation 2.7.4-5
            this.N2O_NFromOrganicNitrogen =
                (this.OrganicPool *
                 emissionFactorForOrganicNitrogen)+ ((directN2ONFromLandAppliedManure + directN2ONFromLandAppliedDigestate + directN2ONFromGrazingAnimals + directN2ONFromLandAppliedDigestateNotAppliedToAnyField) / this.CurrentYearResults.Area);

            this.N2O_NFromOrganicNitrogenExcludeRemainingAmounts =
                (this.OrganicPool *
                 emissionFactorForOrganicNitrogen) + ((directN2ONFromLandAppliedManureExcludingRemaining + directN2ONFromLandAppliedDigestate + directN2ONFromGrazingAnimals) / this.CurrentYearResults.Area);

            // Equation 2.6.5-6
            // Equation 2.7.4-6
            this.N2O_NFromExportedNitrogen = (N2OEmissionFactorCalculator.CalculateTotalDirectN2ONFromExportedManureForFarmAndYear(farm, this.Year) / farm.GetTotalAreaOfFarm(false, this.Year));
        }

        protected void CalculateNitricOxide(double nORatio)
        {
            // Equation 2.6.5-7
            // Equation 2.7.4-7
            this.NO_NFromSyntheticFertilizer = this.N2O_NFromSyntheticFertilizer * nORatio;

            // Equation 2.6.5-8
            // Equation 2.7.4-8
            this.NO_NFromResidues = this.N2O_NFromResidues * nORatio;

            // Equation 2.6.5-9
            // Equation 2.7.4-9
            // Not implemented.

            // Equation 2.6.5-10
            // Equation 2.7.4-10
            this.NO_NFromMineralization = this.N2O_NFromMineralization * nORatio;

            // Equation 2.6.5-11
            // Equation 2.7.4-11
            this.NO_NFromOrganicNitrogen = this.N2O_NFromOrganicNitrogen * nORatio;

            this.NO_NFromOrganicNitrogenExcludingRemainingAmounts = this.N2O_NFromOrganicNitrogenExcludeRemainingAmounts * nORatio;

            // Equation 2.6.5-12
            // Equation 2.7.4-12
            // Not implemented.
        }

        protected void CalculateLeachingEmissions(double fractionLeach,
            double emissionFactorLeaching,
            Farm farm)
        {
            // Equation 2.6.6-3
            // Equation 2.7.5-3
            this.N2O_NFromSyntheticFertilizerLeaching =
                this.SyntheticNitrogenPool * fractionLeach * emissionFactorLeaching;

            // Equation 2.6.6-4
            // Equation 2.7.5-4
            //
            // Removed on 4-28-2023 until further discussion on crop residue contributions to leaching
            // this.N2O_NFromResiduesLeaching = this.CropResiduePool * fractionLeach * emissionFactorLeaching;
            this.N2O_NFromResiduesLeaching = 0;

            // Equation 2.6.6-5
            // Equation 2.7.5-5
            this.N2O_NFromMineralizationLeaching = this.MineralPool * fractionLeach * emissionFactorLeaching;

            // Equation 2.6.6-6
            // Equation 2.7.5-6
            var manureLeachingEmissions = N2OEmissionFactorCalculator.CalculateTotalN2ONFromManureLeachingForField(farm, this.CurrentYearResults) / this.CurrentYearResults.Area;
            var digestateLeachingEmissions = N2OEmissionFactorCalculator.CalculateTotalN2ONFromDigestateLeachingForField(farm, this.CurrentYearResults) / this.CurrentYearResults.Area;

            var remainingManureLeachingEmissions = N2OEmissionFactorCalculator.CalculateTotalN2ONLeachingFromLeftOverManureLeachingForField(farm, this.CurrentYearResults) / this.CurrentYearResults.Area;
            var remainingDigestateLeachingEmissions = N2OEmissionFactorCalculator.CalculateTotalN2ONLeachingFromLeftOverDigestateLeachingForField(farm, this.CurrentYearResults) / this.CurrentYearResults.Area;

            var leachingN2ONFromGrazingAnimals = N2OEmissionFactorCalculator.GetLeachingN2ONFromGrazingAnimals(farm, this.CurrentYearResults, this.AnimalComponentEmissionsResults) / this.CurrentYearResults.Area;

            this.N2O_NFromOrganicNitrogenLeaching = (this.OrganicPool * fractionLeach * emissionFactorLeaching) +
                                                    manureLeachingEmissions + 
                                                    digestateLeachingEmissions +
                                                    remainingManureLeachingEmissions +
                                                    remainingDigestateLeachingEmissions +
                                                    leachingN2ONFromGrazingAnimals;

            this.N2O_NFromOrganicNitrogenLeachingExcludingRemainingAmounts = (this.OrganicPool * fractionLeach * emissionFactorLeaching) + 
                                                                             manureLeachingEmissions + 
                                                                             digestateLeachingEmissions +
                                                                             leachingN2ONFromGrazingAnimals;

            this.CurrentYearResults.TotalN2ONFromManureAndDigestateLeaching = this.N2O_NFromOrganicNitrogenLeaching;
            this.CurrentYearResults.TotalN2ONFromManureAndDigestateLeachingExcludingRemainingAmounts = this.N2O_NFromOrganicNitrogenLeachingExcludingRemainingAmounts;

            // Equation 2.6.6-7
            // Equation 2.7.5-7
            var leachingEmissionsFromExportedManure = N2OEmissionFactorCalculator.CalculateTotalLeachingN2ONFromExportedManure(farm, this.Year);
            this.N2O_NFromOrganicNitrogenLeachingExported = leachingEmissionsFromExportedManure / farm.GetTotalAreaOfFarm(includeNativeGrasslands: false, this.Year);
        }

        protected void CalculateActualAmountsLeached(double fractionLeach,
            double emissionFactorLeaching,
            Farm farm)
        {
            // Equation 2.6.6-8
            // Equation 2.7.5-8
            this.NO3FromSyntheticFertilizerLeaching = this.SyntheticNitrogenPool * fractionLeach * (1 - emissionFactorLeaching);

            this.CurrentYearResults.NO3NFromSyntheticFertilizerLeaching = this.NO3FromSyntheticFertilizerLeaching;

            // Equation 2.6.6-9
            // Equation 2.7.5-9
            //
            // Removed on 4-28-2023 until further discussion on crop residue contributions to leaching
            // this.NO3FromResiduesLeaching = this.CropResiduePool * fractionLeach * (1 - emissionFactorLeaching);
            this.NO3FromResiduesLeaching = 0;

            this.CurrentYearResults.NO3NFromResiduesLeaching = this.NO3FromResiduesLeaching;

            // Equation 2.6.6-10
            // Equation 2.7.5-10
            this.NO3FromMineralizationLeaching = this.MineralPool * fractionLeach * (1 - emissionFactorLeaching);

            this.CurrentYearResults.NO3NFromMineralizationLeaching = NO3FromMineralizationLeaching;

            // Equation 2.6.6-11
            // Equation 2.7.5-11
            var nitrateLeachedFromManure = this.N2OEmissionFactorCalculator.CalculateTotalManureNitrateLeached(farm, this.CurrentYearResults) / this.CurrentYearResults.Area;
            var nitrateLeachedFromDigestate = this.N2OEmissionFactorCalculator.CalculateTotalDigestateNitrateLeached(farm, this.CurrentYearResults) / this.CurrentYearResults.Area;

            var nitrateLeachedFromRemainingManure = this.N2OEmissionFactorCalculator.CalculateTotalNitrateLeachedFromLeftOverManureForField(farm, this.CurrentYearResults) / this.CurrentYearResults.Area;
            var nitrateLeachedFromRemainingDigestate = this.N2OEmissionFactorCalculator.CalculateTotalNitrateLeachedFromLeftOverDigestateForField(farm, this.CurrentYearResults) / this.CurrentYearResults.Area;

            var nitrateLeachedFromGrazingAnimals = this.N2OEmissionFactorCalculator.GetActualLeachingN2ONFromGrazingAnimals(farm, this.CurrentYearResults, this.AnimalComponentEmissionsResults) / this.CurrentYearResults.Area;

            this.NO3FromOrganicNitrogenLeaching = (this.OrganicPool * fractionLeach * (1 - emissionFactorLeaching)) +
                                                  nitrateLeachedFromManure +
                                                  nitrateLeachedFromDigestate +
                                                  nitrateLeachedFromRemainingManure +
                                                  nitrateLeachedFromRemainingDigestate +
                                                  nitrateLeachedFromGrazingAnimals;

            this.CurrentYearResults.NO3NFromManureAndDigestateLeaching = nitrateLeachedFromManure +
                                                                        nitrateLeachedFromDigestate;

            // Equation 2.6.6-12
            // Equation 2.7.5-12
            this.NO3FromOrganicNitrogenLeachingExported = this.N2OEmissionFactorCalculator.CalculateTotalNitrateLeachedFromExportedManureForFarmAndYear(farm, this.Year) / farm.GetTotalAreaOfFarm(false, this.Year);
        }

        protected void CalculateVolatilization(
            double volatilizationFraction,
            double volatilizationEmissionFactor,
            Farm farm)
        {
            // Equation 2.6.6-14
            // Equation 2.7.5-14
            this.N2O_NSyntheticNitrogenVolatilization = this.SyntheticNitrogenPool * volatilizationFraction * volatilizationEmissionFactor;

            var manureVolatilization = this.N2OEmissionFactorCalculator.CalculateTotalManureN2ONVolatilizationForField(this.CurrentYearResults, farm, this.Year, includeRemainingAmounts: true) / this.CurrentYearResults.Area;
            var manureVolatilizationExcludingRemainingAmount = this.N2OEmissionFactorCalculator.CalculateTotalManureN2ONVolatilizationForField(this.CurrentYearResults, farm, this.Year, includeRemainingAmounts: false) / this.CurrentYearResults.Area;
            var digestateVolatilization = this.N2OEmissionFactorCalculator.CalculateTotalDigestateN2ONVolatilizationForField(this.CurrentYearResults, farm, this.Year) / this.CurrentYearResults.Area;
            var volatilizationFromGrazingAnimals = this.N2OEmissionFactorCalculator.GetVolatilizationN2ONFromGrazingAnimals(farm, this.CurrentYearResults, this.AnimalComponentEmissionsResults) / this.CurrentYearResults.Area;

            // Equation 2.6.6-15
            // Equation 2.7.5-15
            this.N2O_NOrganicNitrogenVolatilization = (this.OrganicPool * volatilizationFraction * volatilizationEmissionFactor) +
                                                      manureVolatilization +
                                                      digestateVolatilization +
                                                      volatilizationFromGrazingAnimals;

            this.N2O_NOrganicNitrogenVolatilizationExcludingRemainingAmounts = (this.OrganicPool * volatilizationFraction * volatilizationEmissionFactor) +
                                                                               manureVolatilizationExcludingRemainingAmount +
                                                                               digestateVolatilization +
                                                                               volatilizationFromGrazingAnimals;

            // Equation 2.6.6-16
            // Equation 2.7.5-17
            this.N2O_NFromVolatilizationForExports = this.N2OEmissionFactorCalculator.CalculateVolatilizationEmissionsFromExportedManureForFarmAndYear(farm, this.Year) / farm.GetTotalAreaOfFarm(false, this.Year);
        }

        protected void CalculateActualVolatilization(
            double volatilizationFraction,
            double volatilizationEmissionFactor,
            Farm farm)
        {
            // TODO: rename these to NH3-N

            // Equation 2.6.6-17
            // Equation 2.7.5-17
            this.NH4FromSyntheticNitogenVolatilized = farm.Defaults.CarbonModellingStrategy == CarbonModellingStrategies.ICBM ? this.SyntheticNitrogenPool * volatilizationFraction * (1 - volatilizationEmissionFactor) : this.SyntheticNitrogenPool * volatilizationFraction * (volatilizationEmissionFactor);

            this.CurrentYearResults.NH4FromSyntheticNitogenVolatilized = NH4FromSyntheticNitogenVolatilized;

            var ammoniaFromManureApplications = N2OEmissionFactorCalculator.CalculateTotalManureAmmoniaEmissionsForField(farm, this.CurrentYearResults, this.Year);
            var ammoniaFromDigestateApplications = N2OEmissionFactorCalculator.CalculateTotalDigestateAmmoniaEmissionsForField(farm, this.CurrentYearResults, this.Year);
            var ammoniaFromGrazing = N2OEmissionFactorCalculator.GetVolatilizationNH3FromGrazingAnimals(farm, this.CurrentYearResults, this.AnimalComponentEmissionsResults);

            this.AdjustedAmmoniacalLossFromLandAppliedManureAndDigestate = (ammoniaFromManureApplications + ammoniaFromDigestateApplications + ammoniaFromGrazing) / this.CurrentYearResults.Area;

            // Equation 2.6.6-18
            // Equation 2.7.5-18
            this.NH4FromOrganicNitogenVolatilized =
                (this.OrganicPool * volatilizationFraction * (1 - volatilizationEmissionFactor)) +
                this.AdjustedAmmoniacalLossFromLandAppliedManureAndDigestate;

            // Equation 2.6.6-19
            // Equation 2.7.5-19
            this.NH4FromExports = this.N2OEmissionFactorCalculator.CalculateTotalAdjustedAmmoniaEmissionsForFarmAndYear(farm, this.Year) / farm.GetTotalAreaOfFarm(false, this.Year);
        }

        protected void AdjustSyntheticNitrogenPool()
        {
            // Equation 2.6.7-1
            // Equation 2.7.6-1
            this.SyntheticNitrogenPool -= (this.N2O_NFromSyntheticFertilizer + this.NO_NFromSyntheticFertilizer);

            // Equation 2.6.7-2
            // Equation 2.7.6-2
            this.SyntheticNitrogenPool -= (this.N2O_NFromSyntheticFertilizerLeaching + this.NO3FromSyntheticFertilizerLeaching);

            // Equation 2.6.7-3
            // Equation 2.7.6-3
            this.SyntheticNitrogenPool -= (this.N2O_NSyntheticNitrogenVolatilization + this.NH4FromSyntheticNitogenVolatilized);
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

        public void AdjustPoolsAfterDemandCalculation(double nitrogenDemand)
        {
            if ((Math.Abs(this.AvailabilityOfMineralN) < double.Epsilon) ||
                this.MicrobePool > this.AvailabilityOfMineralN)
            {
                // Equation 2.6.8-10
                // Equation 2.6.8-15
                // Equation 2.7.7-14
                this.AvailabilityOfMineralN -= nitrogenDemand * this.PoolRatio;

                // Equation 2.6.8-11
                // Equation 2.6.8-16
                // Equation 2.7.7-15
                this.MicrobePool -= nitrogenDemand * (1 - this.PoolRatio);
            }
            else
            {
                // Equation 2.6.8-8
                // Equation 2.6.8-13
                // Equation 2.7.7-12
                this.AvailabilityOfMineralN -= nitrogenDemand * (1 - this.PoolRatio);

                // Equation 2.6.8-9
                // Equation 2.6.8-14
                // Equation 2.7.7-13
                this.MicrobePool -= nitrogenDemand * this.PoolRatio;
            }
        }

        protected void BalancePools(double microbeDeath)
        {
            if (this.MicrobePool > 0)
            {
                // Equation 2.6.8-17
                // Equation 2.7.7-16
                this.MicrobeDeathPool = this.MicrobePool * microbeDeath;
            }
            else
            {
                // Equation 2.6.8-18
                // Equation 2.7.7-17
                this.MicrobeDeathPool = 0;
            }

            // Equation 2.6.8-19
            // Equation 2.7.7-18
            this.MicrobePool -= this.MicrobeDeathPool;

            // Equation 2.6.8-20
            // Equation 2.7.7-19
            this.AvailabilityOfMineralN += this.MicrobeDeathPool;

            if (this.AvailabilityOfMineralN > 0)
            {
                // Equation 2.6.8-21
                // Equation 2.7.7-20
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
                // Equation 2.7.7-21
                this.N2Loss = 0;
            }

            // Equation 2.6.8-23
            // Equation 2.7.7-22
            this.AvailabilityOfMineralN -= this.N2Loss;

            if (this.YearIndex == 0)
            {
                // Equation 2.6.8-24
                // Equation 2.7.7-23
                this.MineralNBalance = this.AvailabilityOfMineralN;

                // Equation 2.6.8-25
                // Equation 2.7.7-24
                this.MicrobeNBalance = this.MicrobePool - 25;
            }
            else
            {
                // Equation 2.6.8-26
                // Equation 2.7.7-25
                this.MineralNBalance =
                    this.AvailabilityOfMineralN - this.PreviousYearResults.MineralNitrogenPool_N_mineralN;

                // Equation 2.6.8-27
                // Equation 2.7.7-26
                this.MicrobeNBalance = this.MicrobePool - this.PreviousYearResults.MicrobeNitrogenPool_N_microbeN;
            }

            if (this.AvailabilityOfMineralN < 0)
            {
                // Equation 2.6.8-28
                // Equation 2.7.7-27
                this.MicrobePool += this.AvailabilityOfMineralN;

                // Equation 2.6.8-29
                // Equation 2.7.7-28
                this.AvailabilityOfMineralN = 0;
            }
        }

        protected void CalculateIndirectEmissions(Farm farm, CropViewItem currentYearResults)
        {
            var fractionLeach = N2OEmissionFactorCalculator.CalculateFractionOfNitrogenLostByLeachingAndRunoff(
                farm.GetGrowingSeasonPrecipitation(currentYearResults.Year),
                farm.GetGrowingSeasonEvapotranspiration(currentYearResults.Year));

            var emissionFactorLeaching = farm.Defaults.EmissionFactorForLeachingAndRunoff;

            this.CalculateLeachingEmissions(fractionLeach, emissionFactorLeaching, farm);
            this.CalculateActualAmountsLeached(fractionLeach,  emissionFactorLeaching, farm);

            var volatilizationFractionSoil = N2OEmissionFactorCalculator.CalculateFractionOfNitrogenLostByVolatilization(currentYearResults, farm);
            var emissionFactorForVolatilization = farm.DefaultSoilData.Province.GetRegion() == Region.WesternCanada ? 0.005 : 0.014;

            this.CalculateVolatilization(volatilizationFractionSoil, emissionFactorForVolatilization, farm);
            this.CalculateActualVolatilization(volatilizationFractionSoil, emissionFactorForVolatilization, farm);
        }

        protected void CalculateDirectEmissions(Farm farm, CropViewItem currentYearResults)
        {
            this.CalculateDirectNitrousOxide(currentYearResults, farm);
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
            this.MicrobePool += this.MineralPool;

            // Equation 2.6.8-3
            // Equation 2.7.7-3
            this.MicrobePool += this.CropResiduePool;

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

            var totalDirectNitrousOxideExcludingRemainingManure = this.N2O_NFromSyntheticFertilizer +
                                                                  this.N2O_NFromResidues +
                                                                  this.N2O_NFromMineralization +
                                                                  this.N2O_NFromOrganicNitrogenExcludeRemainingAmounts;


            // Equation 2.6.9-2
            // Equation 2.7.8-2
            var totalIndirectNitrousOxide = this.N2O_NFromSyntheticFertilizerLeaching +
                                            this.N2O_NFromResiduesLeaching +
                                            this.N2O_NFromMineralizationLeaching +
                                            this.N2O_NFromOrganicNitrogenLeaching +
                                            this.N2O_NSyntheticNitrogenVolatilization +
                                            this.N2O_NOrganicNitrogenVolatilization;

            var totalIndirectNitrousOxideExcludingRemainingAmounts = this.N2O_NFromSyntheticFertilizerLeaching +
                                                                     this.N2O_NFromResiduesLeaching +
                                                                     this.N2O_NFromMineralizationLeaching +
                                                                     this.N2O_NFromOrganicNitrogenLeachingExcludingRemainingAmounts +
                                                                     this.N2O_NSyntheticNitrogenVolatilization +
                                                                     this.N2O_NOrganicNitrogenVolatilizationExcludingRemainingAmounts;

            // Equation 2.6.9-3
            // Equation 2.7.8-3
            // Equation 2.6.9-34 (a)
            // Equation 2.7.8-29 (a)
            this.CurrentYearResults.TotalNitrogenEmissions += (totalDirectNitrousOxide + totalIndirectNitrousOxide);

            var area = this.CurrentYearResults.Area;

            // Equation 2.6.9-4
            // Equation 2.7.8-4
            this.CurrentYearResults.TotalNitrousOxideForArea = this.CurrentYearResults.TotalNitrogenEmissions * area;

            this.CurrentYearResults.DirectNitrousOxideEmissionsFromSyntheticNitrogenForArea =
                this.N2O_NFromSyntheticFertilizer * area;
            this.CurrentYearResults.DirectNitrousOxideEmissionsFromCropResiduesForArea = this.N2O_NFromResidues * area;
            this.CurrentYearResults.DirectNitrousOxideEmissionsFromMineralizedNitrogenForArea =
                this.N2O_NFromMineralization * area;

            this.CurrentYearResults.DirectNitrousOxideEmissionsFromOrganicNitrogenForArea = this.N2O_NFromOrganicNitrogen * area;
            this.CurrentYearResults.DirectNitrousOxideEmissionsFromOrganicNitrogenForAreaExcludingRemainingAmounts = this.N2O_NFromOrganicNitrogenExcludeRemainingAmounts * area;

            this.CurrentYearResults.TotalDirectNitrousOxidePerHectare = CoreConstants.ConvertToN2O(totalDirectNitrousOxide);
            this.CurrentYearResults.TotalDirectNitrousOxidePerHectareExcludingRemainingAmounts = CoreConstants.ConvertToN2O(totalDirectNitrousOxideExcludingRemainingManure);

            this.CurrentYearResults.AdjustedAmmoniacalLossFromLandAppliedManurePerHectare = this.AdjustedAmmoniacalLossFromLandAppliedManureAndDigestate;

            this.CurrentYearResults.TotalDirectNitrousOxideForArea = totalDirectNitrousOxide * area;

            this.CurrentYearResults.IndirectNitrousOxideEmissionsFromSyntheticNitrogenForArea =
                this.N2O_NFromSyntheticFertilizerLeaching * area;
            this.CurrentYearResults.IndirectNitrousOxideEmissionsFromCropResiduesForArea =
                this.N2O_NFromResiduesLeaching * area;
            this.CurrentYearResults.IndirectNitrousOxideEmissionsFromMineralizedNitrogenForArea =
                this.N2O_NFromMineralizationLeaching * area; 
            
            this.CurrentYearResults.IndirectNitrousOxideEmissionsFromOrganicNitrogenForArea = this.N2O_NFromOrganicNitrogenLeaching * area;
            this.CurrentYearResults.IndirectNitrousOxideLeachingEmissionsFromOrganicNitrogenForAreaExcludingRemainingManure = this.N2O_NFromOrganicNitrogenLeachingExcludingRemainingAmounts * area;

            this.CurrentYearResults.IndirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForArea =
                this.N2O_NSyntheticNitrogenVolatilization * area;
            
            this.CurrentYearResults.IndirectNitrousOxideEmissionsFromVolatilizationOfOrganicNitrogenForArea = this.N2O_NOrganicNitrogenVolatilization * area;
            this.CurrentYearResults.IndirectNitrousOxideEmissionsFromVolatilizationOfOrganicNitrogenForAreaExcludingRemainingAmounts = this.N2O_NOrganicNitrogenVolatilizationExcludingRemainingAmounts * area;

            this.CurrentYearResults.TotalIndirectNitrousOxidePerHectare = CoreConstants.ConvertToN2O(totalIndirectNitrousOxide);
            this.CurrentYearResults.TotalIndirectNitrousOxidePerHectareExcludingRemainingAmounts = CoreConstants.ConvertToN2O(totalIndirectNitrousOxideExcludingRemainingAmounts);

            this.CurrentYearResults.TotalIndirectNitrousOxideForArea = totalIndirectNitrousOxide * area;
            this.CurrentYearResults.TotalIndirectNitrousOxideForAreaExcludingRemainingAmounts = totalIndirectNitrousOxideExcludingRemainingAmounts * area;
        }

        protected void SumNitricOxide()
        {
            // Equation 2.6.9-5
            // Equation 2.7.8-5
            var totalNitricOxide = this.NO_NFromSyntheticFertilizer +
                                   this.NO_NFromResidues +
                                   this.NO_NFromMineralization +
                                   this.NO_NFromOrganicNitrogen;

            // Equation 2.6.9-34 (b)
            // Equation 2.7.8-29 (b)
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

            // Equation 2.6.9-34 (c)
            // Equation 2.7.8-29 (c)
            this.CurrentYearResults.TotalNitrogenEmissions += totalNitrateLeaching;

            var area = this.CurrentYearResults.Area;

            // Equation 2.6.9-8
            // Equation 2.7.8-8
            this.CurrentYearResults.TotalNitrateLeachingForArea = totalNitrateLeaching * area;

            this.CurrentYearResults.IndirectNitrateFromSyntheticNitrogenForArea = this.NO3FromSyntheticFertilizerLeaching * area;
            this.CurrentYearResults.IndirectNitrateFromCropResiduesForArea = this.NO3FromResiduesLeaching * area;
            this.CurrentYearResults.IndirectNitrateFromMineralizedNitrogenForArea = this.NO3FromMineralizationLeaching * area;
            this.CurrentYearResults.IndirectNitrateFromOrganicNitrogenForArea = this.NO3FromOrganicNitrogenLeaching * area;
        }

        protected void SumAmmoniaVolatilization()
        {
            // Equation 2.6.9-9
            // Equation 2.7.8-9
            var totalAmmoniaVolatilization = this.NH4FromSyntheticNitogenVolatilized +
                                             this.NH4FromOrganicNitogenVolatilized;

            // Equation 2.6.9-34 (d)
            // Equation 2.7.8-29 (d)
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

            // Equation 2.6.9-36
            // Equation 2.7.8-31
            this.CurrentYearResults.TotalNitrogenOutputs = this.CurrentYearResults.TotalUptake +
                                                           this.CurrentYearResults.TotalNitrogenEmissions;

            this.CurrentYearResults.DifferenceBetweenInputsAndOutputs = this.CurrentYearResults.TotalNitrogenInputs -
                                                                        this.CurrentYearResults.TotalNitrogenOutputs;

            // Equation 2.7.8-32
            // Equation 2.6.9-37
            this.CurrentYearResults.Overflow = this.AvailabilityOfMineralN + this.MicrobePool - this.N2Loss;
        }

        /// <summary>
        /// Equation 4.7.2-1
        /// </summary>
        /// <returns>Amount of N added to soil</returns>
        protected double CalculateAmountOfNitrogenAppliedToSoilAfterLosses(
            double totalNitrogenAppliedToField,
            double totalDirectN2ON,
            double totalAmmoniaLossFromLandApplication,
            double totalN2ONFromLeaching,
            double totalNitrogenRemainingForField, 
            double totalNO3NFromLeaching)
        {
            // Note we don't divide by the total volume of all manure produced here (as specified in 4.7.2-1) since the manure and/or digestate application(s) already consider
            // the fraction being used as compared to the total volume of manure/digestate produced

            var result = (totalNitrogenAppliedToField + totalNitrogenRemainingForField) - (totalDirectN2ON + totalAmmoniaLossFromLandApplication + totalN2ONFromLeaching + totalNO3NFromLeaching);

            return result;
        }

        /// <summary>
        /// Equation 2.7.2-11
        /// 
        /// Calculates the amount of N from manure (field applications and/or from grazing animals) and/or digestate added to the field after losses from emissions have been calculated on a per hectare basis
        /// </summary>
        /// <returns>Amount of N from manure and/or digestate (kg N ha^-1)</returns>
        protected double GetManureAndDigestateNitrogenResiduesForYear(
            Farm farm, 
            CropViewItem cropViewItem)
        {
            // These will be the totals for the entire field
            var totalDirectN2ONFromLandAppliedManure = N2OEmissionFactorCalculator.CalculateDirectN2ONEmissionsFromFieldSpecificManureSpreadingForField(cropViewItem, farm);
            var totalDirectN2ONFromLandAppliedDigestate = N2OEmissionFactorCalculator.CalculateDirectN2ONEmissionsFromFieldSpecificDigestateSpreading(cropViewItem, farm);

            // Convert to amount per hectare
            var combinedDirectN2ON = (totalDirectN2ONFromLandAppliedManure + totalDirectN2ONFromLandAppliedDigestate)/ cropViewItem.Area;

            var totalNitrogenFromManureLandApplications = N2OEmissionFactorCalculator.GetAmountOfManureNitrogenUsed(cropViewItem) / cropViewItem.Area;
            var totalNitrogenFromDigestateLandApplications = N2OEmissionFactorCalculator.GetAmountOfDigestateNitrogenUsed(cropViewItem) / cropViewItem.Area;
            var combinedNitrogenApplied = totalNitrogenFromManureLandApplications + totalNitrogenFromDigestateLandApplications;

            var manureAmmoniacalLoss = N2OEmissionFactorCalculator.CalculateAmmoniacalLossFromManureForField(farm, cropViewItem) / cropViewItem.Area;
            var digestateAmmoniacalLoss = N2OEmissionFactorCalculator.CalculateNH3NLossFromFarmSourcedLandAppliedDigestateForField(farm, cropViewItem, this.Year) / cropViewItem.Area;
            var combinedAmmoniacalLoss = manureAmmoniacalLoss + digestateAmmoniacalLoss;

            var manureLeachingLoss = this.N2OEmissionFactorCalculator.CalculateTotalN2ONFromManureLeachingForField(farm, cropViewItem) / cropViewItem.Area;
            var digestateLeachingLoss = this.N2OEmissionFactorCalculator.CalculateTotalN2ONFromDigestateLeachingForField(farm, cropViewItem) / cropViewItem.Area;
            var combinedLeachingLoss = manureLeachingLoss + digestateLeachingLoss;

            var manureNitrateLeaching = this.N2OEmissionFactorCalculator.CalculateTotalManureNitrateLeached(farm, cropViewItem) / cropViewItem.Area;
            var digestateNitrateLeaching = this.N2OEmissionFactorCalculator.CalculateTotalDigestateNitrateLeached(farm, cropViewItem) / cropViewItem.Area;
            var combinedNO3NLeachingLoss = manureNitrateLeaching + digestateNitrateLeaching;

            var manureNitrogenRemaining = this.N2OEmissionFactorCalculator.GetManureNitrogenRemainingForField(cropViewItem, farm);
            var digestateNitrogenRemaining = this.N2OEmissionFactorCalculator.GetDigestateNitrogenRemainingForField(cropViewItem, farm);
            var totalRemainingNitrogen = manureNitrogenRemaining + digestateNitrogenRemaining;

            var nitrogenAppliedToSoilAfterLosses = CalculateAmountOfNitrogenAppliedToSoilAfterLosses(
                totalNitrogenAppliedToField: combinedNitrogenApplied,
                totalDirectN2ON: combinedDirectN2ON,
                totalAmmoniaLossFromLandApplication: combinedAmmoniacalLoss,
                totalN2ONFromLeaching: combinedLeachingLoss, 
                totalNitrogenRemainingForField: totalRemainingNitrogen, 
                totalNO3NFromLeaching: combinedNO3NLeachingLoss);

            // Inputs from grazing animals will already have emissions subtracted and so we are adding the remaining N from grazing animals here.
            if (farm.CropHasGrazingAnimals(cropViewItem))
            {
                nitrogenAppliedToSoilAfterLosses += cropViewItem.TotalNitrogenInputFromManureFromAnimalsGrazingOnPasture;
            }

            return nitrogenAppliedToSoilAfterLosses;
        }

        #endregion

        #region Abstract Methods

        protected abstract void SetCropResiduesStartState(Farm farm);
        protected abstract void SetManurePoolStartState(Farm farm);
        protected abstract void SetOrganicNitrogenPoolStartState();
        protected abstract void AdjustOrganicPool();

        #endregion

        #region Private Methods

        #endregion
    }
}