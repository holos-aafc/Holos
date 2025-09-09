using System;
using H.Core.Calculators.Nitrogen;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Climate;

namespace H.Core.Calculators.Carbon
{
    public abstract partial class CarbonCalculatorBase
    {
        #region Public Methods

        /// <summary>
        ///     Equation 2.6.8-12
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
            return carbonInputFromProduct / carbonConcentration * (1 - moistureContentOfCropFraction) *
                nitrogenConcentrationInTheProduct +
                carbonInputFromStraw / carbonConcentration * (1 - moistureContentOfCropFraction) *
                nitrogenConcentrationInTheStraw +
                carbonInputFromRoots / carbonConcentration * (1 - moistureContentOfCropFraction) *
                nitrogenConcentrationInTheRoots +
                carbonInputFromExtraroots / carbonConcentration * (1 - moistureContentOfCropFraction) *
                nitrogenConcentrationInExtraroots - (1 - nitrogenFixation);
        }

        #endregion

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
        ///     AG_N
        ///     (kg N ha^-1)
        /// </summary>
        public double AboveGroundResidueN { get; set; }

        /// <summary>
        ///     BGR_N
        ///     (kg N ha^-1)
        /// </summary>
        public double BelowGroundResidueN { get; set; }

        /// <summary>
        ///     N_m
        /// </summary>
        public double ManurePool { get; set; }

        /// <summary>
        ///     N_SN
        /// </summary>
        public double SyntheticNitrogenPool { get; set; }

        /// <summary>
        ///     N_CropResidues
        /// </summary>
        public double CropResiduePool { get; set; }

        /// <summary>
        ///     N_min
        /// </summary>
        public double MineralPool { get; set; }

        /// <summary>
        ///     N_ON
        /// </summary>
        public double OrganicPool { get; set; }

        /// <summary>
        ///     N_microbeN - Availability of N in the microbial pool
        ///     (kg N ha^-1)
        /// </summary>
        public double MicrobePool { get; set; }

        /// <summary>
        ///     Ratio between mineral and microbial N
        ///     (unitless)
        /// </summary>
        public double PoolRatio { get; set; }

        public double CropNitrogenDemand { get; set; }

        /// <summary>
        ///     N_microbeDeathN - Release of N from the microbial pool
        ///     (kg N ha^-1)
        /// </summary>
        public double MicrobeDeathPool { get; set; }

        /// <summary>
        ///     N_mineralN - Availability of mineral N on the field
        ///     (kg N ha^-1)
        /// </summary>
        public double AvailabilityOfMineralN { get; set; }

        /// <summary>
        ///     N_mineralNBalance - Interannual N balance in the mineral overflow pool on the field
        ///     (kg N ha^-1)
        /// </summary>
        public double MineralNBalance { get; set; }

        public double YoungPoolAboveGroundResidueN { get; set; }
        public double YoungPoolBelowGroundResidueN { get; set; }

        /// <summary>
        ///     N_microbeN - Interannual N balance in the microbe overflow pool on the field
        ///     (kg N ha^-1)
        /// </summary>
        public double MicrobeNBalance { get; set; }

        /// <summary>
        ///     N2-Nloss - Denitrification of mineral N as N2 on the field
        ///     (kg N2-N ha^-1)
        /// </summary>
        public double N2Loss { get; set; }

        /// <summary>
        ///     (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromSyntheticFertilizer { get; set; }

        /// <summary>
        ///     (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromResidues { get; set; }

        /// <summary>
        ///     Direct N2O-N emissions from crop reside exports
        ///     (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromExportedCropResidues { get; set; }

        /// <summary>
        ///     Direct N2O-N emissions from manure exports
        ///     (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromExportedManure { get; set; }

        /// <summary>
        ///     (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromMineralization { get; set; }

        /// <summary>
        ///     (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromOrganicNitrogen { get; set; }

        /// <summary>
        ///     (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromOrganicNitrogenExcludeRemainingAmounts { get; set; }

        /// <summary>
        ///     (kg NO-N ha^-1)
        /// </summary>
        public double NO_NFromSyntheticFertilizer { get; set; }

        /// <summary>
        ///     (kg NO-N ha^-1)
        /// </summary>
        public double NO_NFromResidues { get; set; }

        /// <summary>
        ///     (kg NO-N ha^-1)
        /// </summary>
        public double NO_NFromExportedCropResidues { get; set; }

        public double N2O_NFromExportedNitrogen { get; set; }

        /// <summary>
        ///     (kg NO-N ha^-1)
        /// </summary>
        public double NO_NFromMineralization { get; set; }

        /// <summary>
        ///     (kg NO-N ha^-1)
        /// </summary>
        public double NO_NFromOrganicNitrogen { get; set; }

        /// <summary>
        ///     (kg NO-N ha^-1)
        /// </summary>
        public double NO_NFromOrganicNitrogenExcludingRemainingAmounts { get; set; }

        /// <summary>
        ///     (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromSyntheticFertilizerLeaching { get; set; }

        /// <summary>
        ///     (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromResiduesLeaching { get; set; }

        /// <summary>
        ///     (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromMineralizationLeaching { get; set; }

        /// <summary>
        ///     (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromOrganicNitrogenLeaching { get; set; }

        /// <summary>
        ///     (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromOrganicNitrogenLeachingExcludingRemainingAmounts { get; set; }

        /// <summary>
        ///     (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromOrganicNitrogenLeachingExported { get; set; }

        /// <summary>
        ///     (kg NO3-N ha^-1)
        /// </summary>
        public double NO3FromSyntheticFertilizerLeaching { get; set; }

        /// <summary>
        ///     (kg NO3-N ha^-1)
        /// </summary>
        public double NO3FromResiduesLeaching { get; set; }

        /// <summary>
        ///     (kg NO3-M ha^-1)
        /// </summary>
        public double NO3FromMineralizationLeaching { get; set; }

        /// <summary>
        ///     (kg NO3-N ha^-1)
        /// </summary>
        public double NO3FromOrganicNitrogenLeaching { get; set; }

        /// <summary>
        ///     (kg NO3-N ha^-1)
        /// </summary>
        public double NO3FromOrganicNitrogenLeachingExported { get; set; }

        public double N2O_NSyntheticNitrogenVolatilization { get; set; }
        public double N2O_NOrganicNitrogenVolatilization { get; set; }
        public double N2O_NOrganicNitrogenVolatilizationExcludingRemainingAmounts { get; set; }

        /// <summary>
        ///     Volatilization from manure exports
        ///     (kg N2O-N ha^-1)
        /// </summary>
        public double N2O_NFromVolatilizationForExports { get; set; }

        /// <summary>
        ///     (kg NH4-N ha^-1)
        /// </summary>
        public double NH4FromSyntheticNitogenVolatilized { get; set; }

        public double NH4FromOrganicNitogenVolatilized { get; set; }

        /// <summary>
        ///     (kg NH3-N ha^-1)
        /// </summary>
        public double NH4FromExports { get; set; }

        /// <summary>
        ///     (kg NH3-N ha^-1)
        /// </summary>
        public double AdjustedAmmoniacalLossFromLandAppliedManureAndDigestate { get; set; }

        #endregion

        #region Protected Methods

        protected void SetSyntheticNStartState(CropViewItem currentYearResults)
        {
            // Equation 2.6.1-1
            // Equation 2.7.1-1
            SyntheticNitrogenPool += currentYearResults.NitrogenFertilizerRate;

            // Equation 2.6.1-2
            // Equation 2.7.1-2
            SyntheticNitrogenPool += currentYearResults.NitrogenDepositionAmount;
        }

        protected void SetAvailabilityOfMineralNState(CropViewItem previousYearResults)
        {
            // Use the previous years' mineral N pool as the starting point for this year
            AvailabilityOfMineralN = previousYearResults.MineralNitrogenPool_N_mineralN;
        }

        protected void SetMicrobePoolStartingState(CropViewItem previousYearResults, int yearIndex)
        {
            // Use the previous years' microbial pool as the starting point for this year (or a starting value if in equilibrium year)
            if (yearIndex == 0)
                // Microbe pool always starts with 25 kg N.
                MicrobePool = 25;
            else
                MicrobePool = previousYearResults.MicrobeNitrogenPool_N_microbeN;
        }

        protected void SetPoolStartStates(Farm farm)
        {
            if (YearIndex == 0)
            {
                SyntheticNitrogenPool = 0d;
                CropResiduePool = 0d;
                MineralPool = 0d;
                OrganicPool = 0d;
                MicrobePool = 0d;
            }

            SetSyntheticNStartState(CurrentYearResults);
            SetAvailabilityOfMineralNState(PreviousYearResults);
            SetMicrobePoolStartingState(PreviousYearResults, YearIndex);

            // Manure pool must be set before crop residue, and organic nitrogen pool initializations
            SetManurePoolStartState(farm);
            SetOrganicNitrogenPoolStartState();
            SetCropResiduesStartState(farm);
        }

        /// <summary>
        ///     Add to total input summation before the pools are reduced. Note that the crop residue N pool is added as an input
        ///     and not the AGresidueN or the BGresidueN pools
        /// </summary>
        protected void TotalInputsBeforeReductions()
        {
            CurrentYearResults.TotalNitrogenInputs = 0;

            // Equation 2.6.9-33
            // Equation 2.7.8-28
            CurrentYearResults.TotalNitrogenInputs += SyntheticNitrogenPool;
            CurrentYearResults.TotalNitrogenInputs += CropResiduePool;
            CurrentYearResults.TotalNitrogenInputs += MineralPool;
            CurrentYearResults.TotalNitrogenInputs += OrganicPool;

            // Display the pools before they are adjusted
            CurrentYearResults.SyntheticInputsBeforeAdjustment = SyntheticNitrogenPool;
            CurrentYearResults.CropResiduesBeforeAdjustment = CropResiduePool;
            CurrentYearResults.OrganicNitrogenResiduesBeforeAdjustment = OrganicPool;
            CurrentYearResults.N_min_FromDecompositionOfOldCarbon = MineralPool;
        }

        protected void CalculateDirectNitrousOxide(
            CropViewItem currentYearResults,
            Farm farm,
            CropViewItem previousYearResults)
        {
            var emissionFactorForSyntheticFertilizer =
                N2OEmissionFactorCalculator.CalculateSyntheticNitrogenEmissionFactor(currentYearResults, farm);
            var emissionFactorForCropResidues =
                N2OEmissionFactorCalculator.GetEmissionFactorForCropResidues(currentYearResults, farm);
            var emissionFactorForOrganicNitrogen =
                N2OEmissionFactorCalculator.CalculateOrganicNitrogenEmissionFactor(currentYearResults, farm);

            CurrentYearResults.EF_SN = emissionFactorForSyntheticFertilizer;
            CurrentYearResults.EF_CRN = emissionFactorForCropResidues;
            CurrentYearResults.EF_ON = emissionFactorForOrganicNitrogen;

            // Emissions from land applied manure
            var directN2ONFromLandAppliedManure =
                N2OEmissionFactorCalculator.CalculateDirectN2ONFromFieldAppliedManure(farm, currentYearResults);
            var directN2ONFromLandAppliedManureExcludingRemaining =
                N2OEmissionFactorCalculator.CalculateDirectN2ONFromFieldAppliedManure(farm, currentYearResults, false);

            var directN2ONFromGrazingAnimals =
                N2OEmissionFactorCalculator.GetDirectN2ONFromGrazingAnimals(farm, currentYearResults,
                    AnimalComponentEmissionsResults);

            // Emissions from land applied digestate
            var directN2ONFromLandAppliedDigestate =
                N2OEmissionFactorCalculator.CalculateDirectN2ONFromFieldAppliedDigestate(farm, currentYearResults,
                    true);
            var directN2ONFromLandAppliedDigestateExludingRemaining =
                N2OEmissionFactorCalculator.CalculateDirectN2ONFromFieldAppliedDigestate(farm, currentYearResults,
                    false);

            // Equation 2.6.5-1
            // Equation 2.7.4-1
            N2O_NFromSyntheticFertilizer = SyntheticNitrogenPool * emissionFactorForSyntheticFertilizer;

            var ipccInputs = previousYearResults != null ? previousYearResults.CombinedResidueNitrogen() : 0;
            var icbmInputs = CurrentYearResults.CropResiduesBeforeAdjustment;

            // Only IPCC uses previous year inputs, not ICBM
            var residueInputs = farm.Defaults.CarbonModellingStrategy == CarbonModellingStrategies.ICBM
                ? icbmInputs
                : ipccInputs;

            // Equation 2.6.5-2
            // Equation 2.7.4-2
            N2O_NFromResidues = residueInputs * emissionFactorForCropResidues;

            // Equation 2.6.5-3
            // Equation 2.7.4-3
            N2O_NFromExportedCropResidues =
                N2OEmissionFactorCalculator.CalculateN2OFromCropResidueExports(previousYearResults, farm);

            // Equation 2.6.5-4
            // Equation 2.7.4-4
            N2O_NFromMineralization = MineralPool * emissionFactorForCropResidues;

            // Equation 2.6.5-5
            // Equation 2.7.4-5
            N2O_NFromOrganicNitrogen =
                OrganicPool * emissionFactorForOrganicNitrogen +
                (directN2ONFromLandAppliedManure + directN2ONFromLandAppliedDigestate + directN2ONFromGrazingAnimals) /
                CurrentYearResults.Area;

            N2O_NFromOrganicNitrogenExcludeRemainingAmounts =
                OrganicPool * emissionFactorForOrganicNitrogen +
                (directN2ONFromLandAppliedManureExcludingRemaining +
                 directN2ONFromLandAppliedDigestateExludingRemaining + directN2ONFromGrazingAnimals) /
                CurrentYearResults.Area;

            // Equation 2.6.5-6
            // Equation 2.6.4-6
            N2O_NFromExportedManure =
                N2OEmissionFactorCalculator.CalculateTotalDirectN2ONFromExportedManureByYear(farm,
                    currentYearResults.Year);
        }

        protected void CalculateNitricOxide(double nORatio)
        {
            // Equation 2.6.5-7
            // Equation 2.7.4-7
            NO_NFromSyntheticFertilizer = N2O_NFromSyntheticFertilizer * nORatio;

            // Equation 2.6.5-8
            // Equation 2.7.4-8
            NO_NFromResidues = N2O_NFromResidues * nORatio;

            // Equation 2.6.5-9
            // Equation 2.7.4-9
            NO_NFromExportedCropResidues = N2O_NFromExportedCropResidues * nORatio;

            // Equation 2.6.5-10
            // Equation 2.7.4-10
            NO_NFromMineralization = N2O_NFromMineralization * nORatio;

            // Equation 2.6.5-11
            // Equation 2.7.4-11
            NO_NFromOrganicNitrogen = N2O_NFromOrganicNitrogen * nORatio;

            NO_NFromOrganicNitrogenExcludingRemainingAmounts =
                N2O_NFromOrganicNitrogenExcludeRemainingAmounts * nORatio;

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
            N2O_NFromSyntheticFertilizerLeaching =
                SyntheticNitrogenPool * fractionLeach * emissionFactorLeaching;

            // Equation 2.6.6-4
            // Equation 2.7.5-4
            //
            // Removed on 4-28-2023 until further discussion on crop residue contributions to leaching
            // this.N2O_NFromResiduesLeaching = this.CropResiduePool * fractionLeach * emissionFactorLeaching;
            N2O_NFromResiduesLeaching = 0;

            // Equation 2.6.6-5
            // Equation 2.7.5-5
            N2O_NFromMineralizationLeaching = MineralPool * fractionLeach * emissionFactorLeaching;

            // Equation 2.6.6-6
            // Equation 2.7.5-6
            var manureLeachingEmissions =
                N2OEmissionFactorCalculator.CalculateTotalN2ONFromManureLeachingForField(farm, CurrentYearResults) /
                CurrentYearResults.Area;
            var digestateLeachingEmissions =
                N2OEmissionFactorCalculator.CalculateTotalN2ONFromDigestateLeachingForField(farm, CurrentYearResults) /
                CurrentYearResults.Area;

            var remainingManureLeachingEmissions =
                N2OEmissionFactorCalculator.CalculateTotalN2ONLeachingFromLeftOverManureLeachingForField(farm,
                    CurrentYearResults) / CurrentYearResults.Area;
            var remainingDigestateLeachingEmissions =
                N2OEmissionFactorCalculator.CalculateTotalN2ONLeachingFromLeftOverDigestateLeachingForField(farm,
                    CurrentYearResults) / CurrentYearResults.Area;

            var leachingN2ONFromGrazingAnimals =
                N2OEmissionFactorCalculator.GetLeachingN2ONFromGrazingAnimals(farm, CurrentYearResults,
                    AnimalComponentEmissionsResults) / CurrentYearResults.Area;

            N2O_NFromOrganicNitrogenLeaching = OrganicPool * fractionLeach * emissionFactorLeaching +
                                               manureLeachingEmissions +
                                               digestateLeachingEmissions +
                                               remainingManureLeachingEmissions +
                                               remainingDigestateLeachingEmissions +
                                               leachingN2ONFromGrazingAnimals;

            N2O_NFromOrganicNitrogenLeachingExcludingRemainingAmounts =
                OrganicPool * fractionLeach * emissionFactorLeaching +
                manureLeachingEmissions +
                digestateLeachingEmissions +
                leachingN2ONFromGrazingAnimals;

            CurrentYearResults.TotalN2ONFromManureAndDigestateLeaching = N2O_NFromOrganicNitrogenLeaching;
            CurrentYearResults.TotalN2ONFromManureAndDigestateLeachingExcludingRemainingAmounts =
                N2O_NFromOrganicNitrogenLeachingExcludingRemainingAmounts;

            // Equation 2.6.6-7
            // Equation 2.7.5-7
            var leachingEmissionsFromExportedManure =
                N2OEmissionFactorCalculator.CalculateTotalLeachingN2ONFromExportedManure(farm, Year);
            N2O_NFromOrganicNitrogenLeachingExported =
                leachingEmissionsFromExportedManure / farm.GetTotalAreaOfFarm(false, Year);
        }

        protected void CalculateActualAmountsLeached(double fractionLeach,
            double emissionFactorLeaching,
            Farm farm)
        {
            // Equation 2.6.6-8
            // Equation 2.7.5-8
            NO3FromSyntheticFertilizerLeaching = SyntheticNitrogenPool * fractionLeach * (1 - emissionFactorLeaching);

            CurrentYearResults.NO3NFromSyntheticFertilizerLeaching = NO3FromSyntheticFertilizerLeaching;

            // Equation 2.6.6-9
            // Equation 2.7.5-9
            //
            // Removed on 4-28-2023 until further discussion on crop residue contributions to leaching
            // this.NO3FromResiduesLeaching = this.CropResiduePool * fractionLeach * (1 - emissionFactorLeaching);
            NO3FromResiduesLeaching = 0;

            CurrentYearResults.NO3NFromResiduesLeaching = NO3FromResiduesLeaching;

            // Equation 2.6.6-10
            // Equation 2.7.5-10
            NO3FromMineralizationLeaching = MineralPool * fractionLeach * (1 - emissionFactorLeaching);

            CurrentYearResults.NO3NFromMineralizationLeaching = NO3FromMineralizationLeaching;

            // Equation 2.6.6-11
            // Equation 2.7.5-11
            var nitrateLeachedFromManure =
                N2OEmissionFactorCalculator.CalculateTotalManureNitrateLeached(farm, CurrentYearResults) /
                CurrentYearResults.Area;
            var nitrateLeachedFromDigestate =
                N2OEmissionFactorCalculator.CalculateTotalDigestateNitrateLeached(farm, CurrentYearResults) /
                CurrentYearResults.Area;

            var nitrateLeachedFromRemainingManure =
                N2OEmissionFactorCalculator.CalculateTotalNitrateLeachedFromLeftOverManureForField(farm,
                    CurrentYearResults) / CurrentYearResults.Area;
            var nitrateLeachedFromRemainingDigestate =
                N2OEmissionFactorCalculator.CalculateTotalNitrateLeachedFromLeftOverDigestateForField(farm,
                    CurrentYearResults) / CurrentYearResults.Area;

            var nitrateLeachedFromGrazingAnimals =
                N2OEmissionFactorCalculator.GetActualLeachingN2ONFromGrazingAnimals(farm, CurrentYearResults,
                    AnimalComponentEmissionsResults) / CurrentYearResults.Area;

            NO3FromOrganicNitrogenLeaching = OrganicPool * fractionLeach * (1 - emissionFactorLeaching) +
                                             nitrateLeachedFromManure +
                                             nitrateLeachedFromDigestate +
                                             nitrateLeachedFromRemainingManure +
                                             nitrateLeachedFromRemainingDigestate +
                                             nitrateLeachedFromGrazingAnimals;

            CurrentYearResults.NO3NFromManureAndDigestateLeaching = nitrateLeachedFromManure +
                                                                    nitrateLeachedFromDigestate;
        }

        protected void CalculateVolatilization(
            double volatilizationFraction,
            double volatilizationEmissionFactor,
            Farm farm)
        {
            // Equation 2.6.6-14
            // Equation 2.7.5-14
            N2O_NSyntheticNitrogenVolatilization =
                SyntheticNitrogenPool * volatilizationFraction * volatilizationEmissionFactor;

            var manureVolatilization =
                N2OEmissionFactorCalculator.CalculateTotalManureN2ONVolatilizationForField(CurrentYearResults, farm,
                    Year) / CurrentYearResults.Area;
            var manureVolatilizationExcludingRemainingAmount =
                N2OEmissionFactorCalculator.CalculateTotalManureN2ONVolatilizationForField(CurrentYearResults, farm,
                    Year, false) / CurrentYearResults.Area;
            var digestateVolatilization =
                N2OEmissionFactorCalculator.CalculateTotalDigestateN2ONVolatilizationForField(CurrentYearResults, farm,
                    Year, true) / CurrentYearResults.Area;
            var digestateVolatilizationExcludingRemainingAmount =
                N2OEmissionFactorCalculator.CalculateTotalDigestateN2ONVolatilizationForField(CurrentYearResults, farm,
                    Year, false) / CurrentYearResults.Area;
            var volatilizationFromGrazingAnimals =
                N2OEmissionFactorCalculator.GetVolatilizationN2ONFromGrazingAnimals(farm, CurrentYearResults,
                    AnimalComponentEmissionsResults) / CurrentYearResults.Area;

            // Equation 2.6.6-15
            // Equation 2.7.5-15
            N2O_NOrganicNitrogenVolatilization = OrganicPool * volatilizationFraction * volatilizationEmissionFactor +
                                                 manureVolatilization +
                                                 digestateVolatilization +
                                                 volatilizationFromGrazingAnimals;

            N2O_NOrganicNitrogenVolatilizationExcludingRemainingAmounts =
                OrganicPool * volatilizationFraction * volatilizationEmissionFactor +
                manureVolatilizationExcludingRemainingAmount +
                digestateVolatilizationExcludingRemainingAmount +
                volatilizationFromGrazingAnimals;

            // Equation 2.6.6-16
            // Equation 2.7.5-16
            N2O_NFromVolatilizationForExports =
                N2OEmissionFactorCalculator
                    .CalculateVolatilizationEmissionsFromExportedManureForFarmAndYear(farm, Year) /
                farm.GetTotalAreaOfFarm(false, Year);
        }

        protected void CalculateActualVolatilization(
            double volatilizationFraction,
            double volatilizationEmissionFactor,
            Farm farm)
        {
            // TODO: rename these to NH3-N

            // Equation 2.6.6-17
            // Equation 2.7.5-17
            NH4FromSyntheticNitogenVolatilized = farm.Defaults.CarbonModellingStrategy == CarbonModellingStrategies.ICBM
                ? SyntheticNitrogenPool * volatilizationFraction * (1 - volatilizationEmissionFactor)
                : SyntheticNitrogenPool * volatilizationFraction * volatilizationEmissionFactor;

            CurrentYearResults.NH4FromSyntheticNitogenVolatilized = NH4FromSyntheticNitogenVolatilized;

            var ammoniaFromManureApplications =
                N2OEmissionFactorCalculator.CalculateTotalManureAmmoniaEmissionsForField(farm, CurrentYearResults,
                    Year);
            var ammoniaFromDigestateApplications =
                N2OEmissionFactorCalculator.CalculateTotalDigestateAmmoniaEmissionsForField(farm, CurrentYearResults,
                    Year);
            var ammoniaFromGrazing =
                N2OEmissionFactorCalculator.GetVolatilizationNH3FromGrazingAnimals(farm, CurrentYearResults,
                    AnimalComponentEmissionsResults);

            AdjustedAmmoniacalLossFromLandAppliedManureAndDigestate =
                (ammoniaFromManureApplications + ammoniaFromDigestateApplications + ammoniaFromGrazing) /
                CurrentYearResults.Area;

            // Equation 2.6.6-18
            // Equation 2.7.5-18
            NH4FromOrganicNitogenVolatilized =
                OrganicPool * volatilizationFraction * (1 - volatilizationEmissionFactor) +
                AdjustedAmmoniacalLossFromLandAppliedManureAndDigestate;

            // Equation 2.6.6-19
            // Equation 2.7.5-19
            NH4FromExports =
                N2OEmissionFactorCalculator.CalculateTotalAdjustedAmmoniaEmissionsForFarmAndYear(farm, Year) /
                farm.GetTotalAreaOfFarm(false, Year);
        }

        protected void AdjustSyntheticNitrogenPool()
        {
            // Equation 2.6.7-1
            // Equation 2.7.6-1
            SyntheticNitrogenPool -= N2O_NFromSyntheticFertilizer + NO_NFromSyntheticFertilizer;

            // Equation 2.6.7-2
            // Equation 2.7.6-2
            SyntheticNitrogenPool -= N2O_NFromSyntheticFertilizerLeaching + NO3FromSyntheticFertilizerLeaching;

            // Equation 2.6.7-3
            // Equation 2.7.6-3
            SyntheticNitrogenPool -= N2O_NSyntheticNitrogenVolatilization + NH4FromSyntheticNitogenVolatilized;
        }

        protected void AdjustResiduePool()
        {
            // Equation 2.6.7-4
            // Equation 2.7.6-4
            CropResiduePool -= N2O_NFromResidues + NO_NFromResidues;

            // Equation 2.6.7-5
            // Equation 2.7.6-5
            CropResiduePool -= N2O_NFromResiduesLeaching + NO3FromResiduesLeaching;
        }

        protected void AdjustMineralizedNitrogenPool()
        {
            // Equation 2.6.7-6
            // Equation 2.7.6-6
            MineralPool -= N2O_NFromMineralization + NO_NFromMineralization;

            // Equation 2.6.7-7
            // Equation 2.7.6-7
            MineralPool -= N2O_NFromMineralizationLeaching + NO3FromMineralizationLeaching;
        }

        public void AdjustPoolsAfterDemandCalculation(double nitrogenDemand)
        {
            if (Math.Abs(AvailabilityOfMineralN) < double.Epsilon ||
                MicrobePool > AvailabilityOfMineralN)
            {
                // Equation 2.6.8-10
                // Equation 2.6.8-15
                // Equation 2.7.7-14
                AvailabilityOfMineralN -= nitrogenDemand * PoolRatio;

                // Equation 2.6.8-11
                // Equation 2.6.8-16
                // Equation 2.7.7-15
                MicrobePool -= nitrogenDemand * (1 - PoolRatio);
            }
            else
            {
                // Equation 2.6.8-8
                // Equation 2.6.8-13
                // Equation 2.7.7-12
                AvailabilityOfMineralN -= nitrogenDemand * (1 - PoolRatio);

                // Equation 2.6.8-9
                // Equation 2.6.8-14
                // Equation 2.7.7-13
                MicrobePool -= nitrogenDemand * PoolRatio;
            }
        }

        protected void BalancePools(double microbeDeath)
        {
            if (MicrobePool > 0)
                // Equation 2.6.8-17
                // Equation 2.7.7-16
                MicrobeDeathPool = MicrobePool * microbeDeath;
            else
                // Equation 2.6.8-18
                // Equation 2.7.7-17
                MicrobeDeathPool = 0;

            // Equation 2.6.8-19
            // Equation 2.7.7-18
            MicrobePool -= MicrobeDeathPool;

            // Equation 2.6.8-20
            // Equation 2.7.7-19
            AvailabilityOfMineralN += MicrobeDeathPool;

            if (AvailabilityOfMineralN > 0)
                // Equation 2.6.8-21
                // Equation 2.7.7-20
                N2Loss = AvailabilityOfMineralN - (N2O_NFromSyntheticFertilizer +
                                                   N2O_NFromResidues +
                                                   N2O_NFromMineralization +
                                                   N2O_NFromOrganicNitrogen +
                                                   N2O_NFromSyntheticFertilizerLeaching +
                                                   N2O_NFromResiduesLeaching +
                                                   N2O_NFromMineralizationLeaching +
                                                   N2O_NFromOrganicNitrogenLeaching +
                                                   N2O_NSyntheticNitrogenVolatilization +
                                                   N2O_NOrganicNitrogenVolatilization) * (0.92 / 0.08);
            else
                // Equation 2.6.8-22
                // Equation 2.7.7-21
                N2Loss = 0;

            // Equation 2.6.8-23
            // Equation 2.7.7-22
            AvailabilityOfMineralN -= N2Loss;

            if (YearIndex == 0)
            {
                // Equation 2.6.8-24
                // Equation 2.7.7-23
                MineralNBalance = AvailabilityOfMineralN;

                // Equation 2.6.8-25
                // Equation 2.7.7-24
                MicrobeNBalance = MicrobePool - 25;
            }
            else
            {
                // Equation 2.6.8-26
                // Equation 2.7.7-25
                MineralNBalance =
                    AvailabilityOfMineralN - PreviousYearResults.MineralNitrogenPool_N_mineralN;

                // Equation 2.6.8-27
                // Equation 2.7.7-26
                MicrobeNBalance = MicrobePool - PreviousYearResults.MicrobeNitrogenPool_N_microbeN;
            }

            if (AvailabilityOfMineralN < 0)
            {
                // Equation 2.6.8-28
                // Equation 2.7.7-27
                MicrobePool += AvailabilityOfMineralN;

                // Equation 2.6.8-29
                // Equation 2.7.7-28
                AvailabilityOfMineralN = 0;
            }
        }

        protected void CalculateIndirectEmissions(Farm farm, CropViewItem currentYearResults)
        {
            var fractionLeach = icbmNitrogenInputCalculator.CalculateFractionOfNitrogenLostByLeachingAndRunoff(
                farm.GetGrowingSeasonPrecipitation(currentYearResults.Year),
                farm.GetGrowingSeasonEvapotranspiration(currentYearResults.Year));

            CurrentYearResults.FractionOfNitrogenLostByLeachingAndRunoff = fractionLeach;

            var emissionFactorLeaching = farm.Defaults.EmissionFactorForLeachingAndRunoff;

            CalculateLeachingEmissions(fractionLeach, emissionFactorLeaching, farm);
            CalculateActualAmountsLeached(fractionLeach, emissionFactorLeaching, farm);

            var volatilizationFractionSoil =
                N2OEmissionFactorCalculator.CalculateFractionOfNitrogenLostByVolatilization(currentYearResults, farm);
            var emissionFactorForVolatilization =
                farm.DefaultSoilData.Province.GetRegion() == Region.WesternCanada ? 0.005 : 0.014;

            CalculateVolatilization(volatilizationFractionSoil, emissionFactorForVolatilization, farm);
            CalculateActualVolatilization(volatilizationFractionSoil, emissionFactorForVolatilization, farm);
        }

        protected void CalculateDirectEmissions(Farm farm, CropViewItem currentYearResults,
            CropViewItem previousYearResults)
        {
            CalculateDirectNitrousOxide(currentYearResults, farm, previousYearResults);
            CalculateNitricOxide(farm.Defaults.NORatio);
        }

        protected void AdjustPools()
        {
            AdjustSyntheticNitrogenPool();
            AdjustResiduePool();
            AdjustMineralizedNitrogenPool();
            AdjustOrganicPool();
        }

        protected void CloseNitrogenBudget(CropViewItem currentYearResults)
        {
            // Equation 2.6.8-1
            // Equation 2.7.7-1
            MicrobePool += SyntheticNitrogenPool;

            // Equation 2.6.8-2
            // Equation 2.7.7-2
            MicrobePool += MineralPool;

            // Equation 2.6.8-3
            // Equation 2.7.7-3
            MicrobePool += CropResiduePool;

            // Equation 2.6.8-4
            // Equation 2.7.7-4
            MicrobePool += OrganicPool;

            currentYearResults.MicrobialPoolAfterCloseOfBudget = MicrobePool;

            // See section 2.6.8.1
            SyntheticNitrogenPool = 0;
            CropResiduePool = 0;
            MineralPool = 0;
            OrganicPool = 0;
        }

        protected void CalculatePoolRatio()
        {
            var absoluteMineralN = Math.Abs(AvailabilityOfMineralN);
            var absoluteMicrobeN = Math.Abs(MicrobePool);

            if (absoluteMineralN > absoluteMicrobeN)
                // Equation 2.6.8-5
                // Equation 2.7.7-5
                PoolRatio = Math.Abs(1.0 / (absoluteMineralN / absoluteMicrobeN));

            if (absoluteMicrobeN > absoluteMineralN)
                // Equation 2.6.8-6
                // Equation 2.7.7-6
                PoolRatio = Math.Abs(absoluteMineralN / absoluteMicrobeN);

            CurrentYearResults.Ratio = PoolRatio;
        }

        protected void SumNitrousOxide()
        {
            CurrentYearResults.TotalNitrogenEmissions = 0;

            // Equation 2.6.9-1
            // Equation 2.7.8-1
            var totalDirectNitrousOxide = N2O_NFromSyntheticFertilizer +
                                          N2O_NFromResidues +
                                          N2O_NFromMineralization +
                                          N2O_NFromOrganicNitrogen;

            var totalDirectNitrousOxideExcludingRemainingManure = N2O_NFromSyntheticFertilizer +
                                                                  N2O_NFromResidues +
                                                                  N2O_NFromMineralization +
                                                                  N2O_NFromOrganicNitrogenExcludeRemainingAmounts;


            // Equation 2.6.9-2
            // Equation 2.7.8-2
            var totalIndirectNitrousOxide = N2O_NFromSyntheticFertilizerLeaching +
                                            N2O_NFromResiduesLeaching +
                                            N2O_NFromMineralizationLeaching +
                                            N2O_NFromOrganicNitrogenLeaching +
                                            N2O_NSyntheticNitrogenVolatilization +
                                            N2O_NOrganicNitrogenVolatilization;

            var totalIndirectNitrousOxideExcludingRemainingAmounts = N2O_NFromSyntheticFertilizerLeaching +
                                                                     N2O_NFromResiduesLeaching +
                                                                     N2O_NFromMineralizationLeaching +
                                                                     N2O_NFromOrganicNitrogenLeachingExcludingRemainingAmounts +
                                                                     N2O_NSyntheticNitrogenVolatilization +
                                                                     N2O_NOrganicNitrogenVolatilizationExcludingRemainingAmounts;

            // Equation 2.6.9-3
            // Equation 2.7.8-3
            // Equation 2.6.9-34 (a)
            // Equation 2.7.8-29 (a)
            CurrentYearResults.TotalNitrogenEmissions += totalDirectNitrousOxide + totalIndirectNitrousOxide;

            var area = CurrentYearResults.Area;

            // Equation 2.6.9-4
            // Equation 2.7.8-4
            CurrentYearResults.TotalNitrousOxideForArea = CurrentYearResults.TotalNitrogenEmissions * area;

            CurrentYearResults.DirectNitrousOxideEmissionsFromSyntheticNitrogenForArea =
                N2O_NFromSyntheticFertilizer * area;
            CurrentYearResults.DirectNitrousOxideEmissionsFromCropResiduesForArea = N2O_NFromResidues * area;
            CurrentYearResults.DirectNitrousOxideEmissionsFromMineralizedNitrogenForArea =
                N2O_NFromMineralization * area;

            CurrentYearResults.DirectNitrousOxideEmissionsFromOrganicNitrogenForArea = N2O_NFromOrganicNitrogen * area;
            CurrentYearResults.DirectNitrousOxideEmissionsFromOrganicNitrogenForAreaExcludingRemainingAmounts =
                N2O_NFromOrganicNitrogenExcludeRemainingAmounts * area;

            CurrentYearResults.TotalDirectNitrousOxidePerHectare = CoreConstants.ConvertToN2O(totalDirectNitrousOxide);
            CurrentYearResults.TotalDirectNitrousOxidePerHectareExcludingRemainingAmounts =
                CoreConstants.ConvertToN2O(totalDirectNitrousOxideExcludingRemainingManure);

            CurrentYearResults.AdjustedAmmoniacalLossFromLandAppliedManurePerHectare =
                AdjustedAmmoniacalLossFromLandAppliedManureAndDigestate;

            CurrentYearResults.TotalDirectNitrousOxideForArea = totalDirectNitrousOxide * area;

            CurrentYearResults.IndirectNitrousOxideEmissionsFromSyntheticNitrogenForArea =
                N2O_NFromSyntheticFertilizerLeaching * area;
            CurrentYearResults.IndirectNitrousOxideEmissionsFromCropResiduesForArea =
                N2O_NFromResiduesLeaching * area;
            CurrentYearResults.IndirectNitrousOxideEmissionsFromMineralizedNitrogenForArea =
                N2O_NFromMineralizationLeaching * area;

            CurrentYearResults.IndirectNitrousOxideEmissionsFromOrganicNitrogenForArea =
                N2O_NFromOrganicNitrogenLeaching * area;
            CurrentYearResults.IndirectNitrousOxideLeachingEmissionsFromOrganicNitrogenForAreaExcludingRemainingManure =
                N2O_NFromOrganicNitrogenLeachingExcludingRemainingAmounts * area;

            CurrentYearResults.IndirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForArea =
                N2O_NSyntheticNitrogenVolatilization * area;

            CurrentYearResults.IndirectNitrousOxideEmissionsFromVolatilizationOfOrganicNitrogenForArea =
                N2O_NOrganicNitrogenVolatilization * area;
            CurrentYearResults
                    .IndirectNitrousOxideEmissionsFromVolatilizationOfOrganicNitrogenForAreaExcludingRemainingAmounts =
                N2O_NOrganicNitrogenVolatilizationExcludingRemainingAmounts * area;

            CurrentYearResults.TotalIndirectNitrousOxidePerHectare =
                CoreConstants.ConvertToN2O(totalIndirectNitrousOxide);
            CurrentYearResults.TotalIndirectNitrousOxidePerHectareExcludingRemainingAmounts =
                CoreConstants.ConvertToN2O(totalIndirectNitrousOxideExcludingRemainingAmounts);

            CurrentYearResults.TotalIndirectNitrousOxideForArea = totalIndirectNitrousOxide * area;
            CurrentYearResults.TotalIndirectNitrousOxideForAreaExcludingRemainingAmounts =
                totalIndirectNitrousOxideExcludingRemainingAmounts * area;
        }

        protected void SumNitricOxide()
        {
            // Equation 2.6.9-5
            // Equation 2.7.8-5
            var totalNitricOxide = NO_NFromSyntheticFertilizer +
                                   NO_NFromResidues +
                                   NO_NFromMineralization +
                                   NO_NFromOrganicNitrogen;

            // Equation 2.6.9-34 (b)
            // Equation 2.7.8-29 (b)
            CurrentYearResults.TotalNitrogenEmissions += totalNitricOxide;

            var area = CurrentYearResults.Area;

            // Equation 2.6.9-6
            // Equation 2.7.8-6
            CurrentYearResults.TotalNitricOxideForArea = totalNitricOxide * area;

            CurrentYearResults.DirectNitricOxideEmissionsFromSyntheticNitrogenForArea =
                NO_NFromSyntheticFertilizer * area;
            CurrentYearResults.DirectNitricOxideEmissionsFromCropResiduesForArea = NO_NFromResidues * area;
            CurrentYearResults.DirectNitricOxideEmissionsFromMineralizedNitrogenForArea =
                NO_NFromMineralization * area;
            CurrentYearResults.DirectNitricOxideEmissionsFromOrganicNitrogenForArea =
                NO_NFromOrganicNitrogen * area;
        }

        protected void SumNitrateLeaching()
        {
            // Equation 2.6.9-7
            // Equation 2.7.8-7
            var totalNitrateLeaching = NO3FromSyntheticFertilizerLeaching +
                                       NO3FromResiduesLeaching +
                                       NO3FromMineralizationLeaching +
                                       NO3FromOrganicNitrogenLeaching;

            // Equation 2.6.9-34 (c)
            // Equation 2.7.8-29 (c)
            CurrentYearResults.TotalNitrogenEmissions += totalNitrateLeaching;

            var area = CurrentYearResults.Area;

            // Equation 2.6.9-8
            // Equation 2.7.8-8
            CurrentYearResults.TotalNitrateLeachingForArea = totalNitrateLeaching * area;

            CurrentYearResults.IndirectNitrateFromSyntheticNitrogenForArea = NO3FromSyntheticFertilizerLeaching * area;
            CurrentYearResults.IndirectNitrateFromCropResiduesForArea = NO3FromResiduesLeaching * area;
            CurrentYearResults.IndirectNitrateFromMineralizedNitrogenForArea = NO3FromMineralizationLeaching * area;
            CurrentYearResults.IndirectNitrateFromOrganicNitrogenForArea = NO3FromOrganicNitrogenLeaching * area;
        }

        protected void SumAmmoniaVolatilization()
        {
            // Equation 2.6.9-9
            // Equation 2.7.8-9
            var totalAmmoniaVolatilization = NH4FromSyntheticNitogenVolatilized +
                                             NH4FromOrganicNitogenVolatilized;

            // Equation 2.6.9-34 (d)
            // Equation 2.7.8-29 (d)
            CurrentYearResults.TotalNitrogenEmissions += totalAmmoniaVolatilization;

            var area = CurrentYearResults.Area;

            // Equation 2.7.8-10
            CurrentYearResults.TotalAmmoniaForArea = totalAmmoniaVolatilization * area;

            CurrentYearResults.IndirectAmmoniumEmissionsFromVolatilizationOfSyntheticNitrogenForArea =
                NH4FromSyntheticNitogenVolatilized * area;
            CurrentYearResults.IndirectAmmoniumEmissionsFromVolatilizationOfOrganicNitrogenForArea =
                NH4FromOrganicNitogenVolatilized * area;
        }

        protected void SumExportEmissions()
        {
            /*
             * Direct N2O
             */

            // Equation 2.6.9-22
            // Equation 2.7.8-22
            CurrentYearResults.TotalDirectN2ONFromCropExports = N2O_NFromExportedCropResidues;

            // Equation 2.6.9-23
            // Equation 2.7.8-23
            CurrentYearResults.TotalNONFromExportedCropResidues = NO_NFromExportedCropResidues;

            /*
             * Indirect N2O
             */

            /*
             * Note: Can't attribute indirect N2O emissions from exported manure to any one particular field. The following amounts are not reported on the
             * multi year carbon modelling report. For this reason, only the amount from 2.7.8-24 (or the equivalent 2.6.9-24) will be shown and only on the detailed emissions from (export emissions section)
             *
             * Equations 2.6.9-24, 2.7.8-24 are found in the N2OEmissionFactorCalculator class
             */

            // Equation 2.6.9-25
            // Equation 2.7.8-25
            CurrentYearResults.TotalNO3NFromExportedManure = NO3FromOrganicNitrogenLeachingExported;

            // Equation 2.6.9-26
            // Equation 2.7.8-26
            CurrentYearResults.TotalNH3NFromExportedManure = NH4FromExports;
        }

        protected void SumEmissions()
        {
            SumNitrousOxide();
            SumNitricOxide();
            SumNitrateLeaching();
            SumAmmoniaVolatilization();
            SumExportEmissions();

            // Equation 2.7.8-11
            CurrentYearResults.DenitrificationForArea = N2Loss * CurrentYearResults.Area;
        }

        protected virtual void AssignFinalValues()
        {
            CurrentYearResults.YoungPoolAboveGroundResidueN = YoungPoolAboveGroundResidueN;
            CurrentYearResults.YoungPoolBelowGroundResidueN = YoungPoolBelowGroundResidueN;
            CurrentYearResults.MicrobeDeath = MicrobeDeathPool;
            CurrentYearResults.MineralizedNitrogenPool_N_min = MineralPool;
            CurrentYearResults.OrganicNitrogenPool_N_ON = OrganicPool;
            CurrentYearResults.ManureResiduePool_ManureN = ManurePool;
            CurrentYearResults.CropResidueNitrogenPool_N_CropResidues = CropResiduePool;
            CurrentYearResults.AboveGroundResiduePool_AGresidueN = AboveGroundResidueN;
            CurrentYearResults.BelowGroundResiduePool_BGresidueN = BelowGroundResidueN;
            CurrentYearResults.SyntheticNitrogenPool_N_SN = SyntheticNitrogenPool;
            CurrentYearResults.CropNitrogenDemand = CropNitrogenDemand;
            CurrentYearResults.MicrobeNitrogenPool_N_microbeN = MicrobePool;
            CurrentYearResults.MineralNitrogenPool_N_mineralN = AvailabilityOfMineralN;

            CurrentYearResults.SumOfMineralAndMicrobialPools = AvailabilityOfMineralN + MicrobePool;
            CurrentYearResults.MineralNitrogenBalance = MineralNBalance;
            CurrentYearResults.MicrobialNitrogenBalance = MicrobeNBalance;

            // Equation 2.6.9-36
            // Equation 2.7.8-31
            CurrentYearResults.TotalNitrogenOutputs = CurrentYearResults.TotalUptake +
                                                      CurrentYearResults.TotalNitrogenEmissions;

            CurrentYearResults.DifferenceBetweenInputsAndOutputs = CurrentYearResults.TotalNitrogenInputs -
                                                                   CurrentYearResults.TotalNitrogenOutputs;

            // Equation 2.7.8-32
            // Equation 2.6.9-37
            CurrentYearResults.Overflow = AvailabilityOfMineralN + MicrobePool - N2Loss;
        }

        /// <summary>
        ///     Equation 4.7.2-1
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

            var result = totalNitrogenAppliedToField + totalNitrogenRemainingForField - (totalDirectN2ON +
                totalAmmoniaLossFromLandApplication + totalN2ONFromLeaching + totalNO3NFromLeaching);

            return result;
        }

        /// <summary>
        ///     Equation 2.7.2-11
        ///     Calculates the amount of N from manure (field applications and/or from grazing animals) and/or digestate added to
        ///     the field after losses from emissions have been calculated on a per hectare basis
        /// </summary>
        /// <returns>Amount of N from manure and/or digestate (kg N ha^-1)</returns>
        protected double GetManureAndDigestateNitrogenResiduesForYear(
            Farm farm,
            CropViewItem cropViewItem)
        {
            // These will be the totals for the entire field
            var totalDirectN2ONFromLandAppliedManure =
                N2OEmissionFactorCalculator.CalculateDirectN2ONEmissionsFromFieldSpecificManureSpreadingForField(
                    cropViewItem, farm);
            var totalDirectN2ONFromLandAppliedDigestate =
                N2OEmissionFactorCalculator.CalculateDirectN2ONEmissionsFromFieldSpecificDigestateSpreadingForField(
                    cropViewItem, farm);

            // Convert to amount per hectare
            var combinedDirectN2ON = (totalDirectN2ONFromLandAppliedManure + totalDirectN2ONFromLandAppliedDigestate) /
                                     cropViewItem.Area;

            var totalNitrogenFromManureLandApplications =
                N2OEmissionFactorCalculator.GetAmountOfManureNitrogenUsed(cropViewItem) / cropViewItem.Area;
            var totalNitrogenFromDigestateLandApplications =
                N2OEmissionFactorCalculator.GetAmountOfDigestateNitrogenUsed(cropViewItem) / cropViewItem.Area;
            var combinedNitrogenApplied =
                totalNitrogenFromManureLandApplications + totalNitrogenFromDigestateLandApplications;

            var manureAmmoniacalLoss =
                N2OEmissionFactorCalculator.CalculateAmmoniacalLossFromManureForField(farm, cropViewItem) /
                cropViewItem.Area;
            var digestateAmmoniacalLoss =
                N2OEmissionFactorCalculator.CalculateNH3NLossFromFarmSourcedLandAppliedDigestateForField(farm,
                    cropViewItem, Year) / cropViewItem.Area;
            var combinedAmmoniacalLoss = manureAmmoniacalLoss + digestateAmmoniacalLoss;

            var manureLeachingLoss =
                N2OEmissionFactorCalculator.CalculateTotalN2ONFromManureLeachingForField(farm, cropViewItem) /
                cropViewItem.Area;
            var digestateLeachingLoss =
                N2OEmissionFactorCalculator.CalculateTotalN2ONFromDigestateLeachingForField(farm, cropViewItem) /
                cropViewItem.Area;
            var combinedLeachingLoss = manureLeachingLoss + digestateLeachingLoss;

            var manureNitrateLeaching =
                N2OEmissionFactorCalculator.CalculateTotalManureNitrateLeached(farm, cropViewItem) / cropViewItem.Area;
            var digestateNitrateLeaching =
                N2OEmissionFactorCalculator.CalculateTotalDigestateNitrateLeached(farm, cropViewItem) /
                cropViewItem.Area;
            var combinedNO3NLeachingLoss = manureNitrateLeaching + digestateNitrateLeaching;

            var manureNitrogenRemaining =
                N2OEmissionFactorCalculator.GetManureNitrogenRemainingForField(cropViewItem, farm);
            var digestateNitrogenRemaining =
                N2OEmissionFactorCalculator.GetDigestateNitrogenRemainingForField(cropViewItem, farm);
            var totalRemainingNitrogen = manureNitrogenRemaining + digestateNitrogenRemaining;

            var nitrogenAppliedToSoilAfterLosses = CalculateAmountOfNitrogenAppliedToSoilAfterLosses(
                combinedNitrogenApplied,
                combinedDirectN2ON,
                combinedAmmoniacalLoss,
                combinedLeachingLoss,
                totalRemainingNitrogen,
                combinedNO3NLeachingLoss);

            // Inputs from grazing animals will already have emissions subtracted and so we are adding the remaining N from grazing animals here.
            if (farm.CropHasGrazingAnimals(cropViewItem))
                nitrogenAppliedToSoilAfterLosses +=
                    cropViewItem.TotalNitrogenInputFromManureFromAnimalsGrazingOnPasture;

            return nitrogenAppliedToSoilAfterLosses;
        }

        #endregion

        #region Abstract Methods

        protected abstract void SetCropResiduesStartState(Farm farm);
        protected abstract void SetManurePoolStartState(Farm farm);
        protected abstract void SetOrganicNitrogenPoolStartState();
        protected abstract void AdjustOrganicPool();

        #endregion
    }
}