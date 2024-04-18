using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows;
using H.Core.Calculators.Nitrogen;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;
using H.Core.Providers.Climate;
using H.Core.Providers.Plants;
using H.Core.Providers.Soil;

namespace H.Core.Calculators.Carbon
{
    public partial class IPCCTier2SoilCarbonCalculator : CarbonCalculatorBase
    {
        #region Enumerations

        public enum CalculationModes
        {
            Carbon,
            Nitrogen,
        }

        #endregion

        #region Fields

        private readonly Table_9_Nitrogen_Lignin_Content_In_Crops_Provider _slopeProvider = new Table_9_Nitrogen_Lignin_Content_In_Crops_Provider();
        private readonly Table_8_Globally_Calibrated_Model_Parameters_Provider _globallyCalibratedModelParametersProvider = new Table_8_Globally_Calibrated_Model_Parameters_Provider();

        #endregion

        #region Constructors

        public IPCCTier2SoilCarbonCalculator(IClimateProvider climateProvider, N2OEmissionFactorCalculator n2OEmissionFactorCalculator)
        {
            this.CalculationMode = CalculationModes.Carbon;

            if (climateProvider != null)
            {
                _climateProvider = climateProvider;
            }
            else
            {
                throw new ArgumentNullException(nameof(climateProvider));
            }

            if (n2OEmissionFactorCalculator != null)
            {
                this.N2OEmissionFactorCalculator = n2OEmissionFactorCalculator;
            }
            else
            {
                throw new ArgumentNullException(nameof(n2OEmissionFactorCalculator));
            }
        }

        #endregion

        #region Properties

        public CalculationModes CalculationMode { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Allow the user to specify a custom starting (measured) carbon value for the simulation. When specifying a custom starting point, a run-in period
        /// calculation will have been made and used to determine what fraction of the stocks goes to the active, passive, and slow pools. These fractions
        /// are then used to determine what fractions of the user-defined starting (measured) stocks are distributed to the active, passive, and slow pools.
        /// </summary>
        public void AssignCustomStartPoint(CropViewItem runInPeriodResults, Farm farm, CropViewItem currentYearViewItem)
        {
            // Active pool fraction as determined by the run-in period
            var activePoolFraction = runInPeriodResults.IpccTier2CarbonResults.ActivePool / runInPeriodResults.SoilCarbon;

            // Passive pool fraction as determined by the run-in period
            var passivePoolFraction = runInPeriodResults.IpccTier2CarbonResults.PassivePool / runInPeriodResults.SoilCarbon;

            // Slow pool fraction as determined by the run-in period
            var slowPoolFraction = runInPeriodResults.IpccTier2CarbonResults.SlowPool / runInPeriodResults.SoilCarbon;

            runInPeriodResults.IpccTier2CarbonResults.ActivePool = farm.StartingSoilOrganicCarbon * activePoolFraction;
            runInPeriodResults.IpccTier2CarbonResults.PassivePool = farm.StartingSoilOrganicCarbon * passivePoolFraction;
            runInPeriodResults.IpccTier2CarbonResults.SlowPool = farm.StartingSoilOrganicCarbon * slowPoolFraction;

            // Equation 2.2.10-1
            currentYearViewItem.IpccTier2CarbonResults.ActivePool = farm.StartingSoilOrganicCarbon * activePoolFraction;

            // Equation 2.2.10-2
            currentYearViewItem.IpccTier2CarbonResults.SlowPool = farm.StartingSoilOrganicCarbon * slowPoolFraction;

            // Equation 2.2.10-3
            currentYearViewItem.IpccTier2CarbonResults.PassivePool = farm.StartingSoilOrganicCarbon * passivePoolFraction;

            // Equation 2.7.3-11
            currentYearViewItem.IpccTier2NitrogenResults.ActivePool = activePoolFraction * farm.StartingSoilOrganicCarbon * farm.Defaults.ActiveCarbonN;

            // Equation 2.7.3-12
            currentYearViewItem.IpccTier2NitrogenResults.SlowPool = activePoolFraction * farm.StartingSoilOrganicCarbon * farm.Defaults.SlowCarbonN;

            // Equation 2.7.3-13
            currentYearViewItem.IpccTier2NitrogenResults.PassivePool = activePoolFraction * farm.StartingSoilOrganicCarbon * farm.Defaults.OldPoolCarbonN;

            currentYearViewItem.SoilCarbon = farm.StartingSoilOrganicCarbon;
        }

        public void CalculateResults(
            Farm farm,
            List<CropViewItem> viewItemsByField,
            FieldSystemComponent fieldSystemComponent,
            List<CropViewItem> runInPeriodItems)
        {
            var runInPeriod = this.CalculateRunInPeriod(
                farm: farm,
                runInPeriodItems: runInPeriodItems);

            var nonRunInPeriodItems = viewItemsByField.OrderBy(x => x.Year).ToList();

            for (int i = 0; i < nonRunInPeriodItems.Count; i++)
            {
                // The first year of the simulation (i.e. the first year of user-specified field history)
                CropViewItem currentYearViewItem = nonRunInPeriodItems.ElementAt(i);
                CropViewItem previousYearViewItem;

                if (i > 0)
                {
                    // If we are not considering the first year of the simulation, we can get the previous year of the user-specified field history
                    previousYearViewItem = nonRunInPeriodItems.ElementAt(i - 1);
                }
                else
                {
                    // We are considering the first year of the field history, we need to use the run-in period as a 'stand-in' for the previous year
                    previousYearViewItem = runInPeriod;
                }

                // Calculate climate adjustments for current year. Climate adjustments are already performed for the run-in period if we are at the start year
                this.CalculateClimateAdjustments(
                    currentYearViewItem: currentYearViewItem,
                    farm: farm);

                // Calculate carbon stocks for this year
                this.CalculatePools(
                    currentYearViewItem: currentYearViewItem,
                    previousYearViewItem: previousYearViewItem,
                    farm: farm);

                if (i == 0 && farm.UseCustomStartingSoilOrganicCarbon)
                {
                    /*
                     * Overwrite the calculated starting points with calculated fractions of each of the pools. To calculate the fractions to the active, passive, and slow pools, we
                     * need the results from the run-in period.
                     */

                    AssignCustomStartPoint(runInPeriod, farm, currentYearViewItem);
                }

                this.CalculationMode = CalculationModes.Nitrogen;

                // Calculate nitrogen stocks for this year
                this.CalculateNitrogenAtInterval(previousYearViewItem, currentYearViewItem, null, farm, i);

                // Change back to C mode for next iteration 
                this.CalculationMode = CalculationModes.Carbon;
            }
        }

        /// <summary>
        /// The IPCC Tier 2 approach can only estimate carbon inputs for a small set of crop types. If a crop type does not have values for intercept, slope, etc.
        /// from Table 12 then we cannot use the Tier 2 approach for calculating carbon inputs.
        /// </summary>
        public bool CanCalculateInputsForCrop(CropViewItem cropViewItem)
        {
            var slope = _slopeProvider.GetDataByCropType(cropViewItem.CropType);

            return slope.SlopeValue > 0;
        }

        public void CalculateInputs(CropViewItem viewItem, Farm farm)
        {
            var cropData = _slopeProvider.GetDataByCropType(viewItem.CropType);

            var slope = cropData.SlopeValue;
            var intercept = cropData.InterceptValue;

            // Note that the yield must be converted to tons here since the curve equation expects a yield in tons when multiplying by slope
            var harvestIndex = this.CalculateHarvestIndex(
                slope: slope,
                freshWeightOfYield: viewItem.Yield,
                intercept: intercept,
                moistureContentAsPercentage: viewItem.MoistureContentOfCropPercentage);


            if (viewItem.HarvestMethod == HarvestMethods.Swathing && farm.CropHasGrazingAnimals(viewItem))
            {
                viewItem.PercentageOfProductYieldReturnedToSoil = (100 - viewItem.GetAverageUtilizationFromGrazingAnimals());
            }

            viewItem.AboveGroundResidueDryMatter = this.CalculateAboveGroundResidueDryMatter(harvestIndex: harvestIndex, viewItem: viewItem);

            viewItem.AboveGroundResidueDryMatterExported = this.CalculateAboveGroundResidueDryMatterExported(
                freshWeightOfYield: viewItem.Yield,
                harvestIndex: harvestIndex,
                moistureContentOfCropAsPercentage: viewItem.MoistureContentOfCropPercentage,
                percentageOfStrawReturned: viewItem.PercentageOfStrawReturnedToSoil);

            var fractionRenewed = viewItem.CropType.IsAnnual() ? 1 : 1 / viewItem.PerennialStandLength;

            var finalAboveGroundResidue = this.CalculateAnnualAboveGroundResidue(
                aboveGroundResidueDryMatterForCrop: viewItem.AboveGroundResidueDryMatter,
                area: viewItem.Area,
                fractionRenewed: fractionRenewed,
                fractionBurned: 0,
                fractionRemoved: 0,
                combustionFactor: 0);

            const double AboveGroundCarbonContent = 0.42;

            // Note that eq. 2.2.3-3 is the residue for the entire field, we report per ha on the details screen so we divide by the area here
            viewItem.AboveGroundCarbonInput = (finalAboveGroundResidue * AboveGroundCarbonContent) / viewItem.Area;

            var supplementalFeedingAmount = this.CalculateInputsFromSupplementalHayFedToGrazingAnimals(
                previousYearViewItem: null,
                currentYearViewItem: viewItem,
                nextYearViewItems: null,
                farm: farm);

            viewItem.AboveGroundCarbonInput += supplementalFeedingAmount;

            viewItem.BelowGroundResidueDryMatter = this.CalculateBelowGroundResidueDryMatter(shootToRootRatio: cropData.RSTRatio,
                fractionRenewed: fractionRenewed,
                harvestIndex: harvestIndex, 
                cropViewItem: viewItem);

            const double BelowGroundCarbonContent = 0.42;

            // Note that eq. 2.2.3-4 is the residue for the entire field, we report per ha on the details screen so we divide by the area here
            viewItem.BelowGroundCarbonInput = (viewItem.BelowGroundResidueDryMatter * BelowGroundCarbonContent) / viewItem.Area;

            if (farm.IsCommandLineMode == false)
            {
                viewItem.ManureCarbonInputsPerHectare = this.N2OEmissionFactorCalculator.ManureService.GetTotalManureCarbonInputsForField(farm, viewItem.Year, viewItem);
            }

            viewItem.ManureCarbonInputsPerHectare += viewItem.TotalCarbonInputFromManureFromAnimalsGrazingOnPasture;

            viewItem.DigestateCarbonInputsPerHectare = this.CalculateDigestateCarbonInputPerHectare(viewItem, farm);

            /*
             * Equation 2.2.2-12 (kg C will be used in pool calculations instead of tons C). Algorithm document converts to tons before inputs are used in pool calculations but inputs are kept in kg C
             * here. We report results in kg C on graphs so the conversion to tons is not performed here.
             *
             * Since we report ICBM in kg C (not tons), we do not convert to tons here so output of pool calculations on chart can be compared to ICBM chart on same scale (i.e. kg C and not T C).
             */

            viewItem.TotalCarbonInputs = viewItem.AboveGroundCarbonInput + viewItem.BelowGroundCarbonInput + viewItem.ManureCarbonInputsPerHectare + viewItem.DigestateCarbonInputsPerHectare;
        }

        /// <summary>
        /// The run in period is the 'year' that is the averages carbon inputs, climate effects etc. and is used as the 'year 0' item when
        /// calculating the pools.
        /// </summary>
        public CropViewItem CalculateRunInPeriod(
            Farm farm,
            List<CropViewItem> runInPeriodItems)
        {
            // Calculate TFac, WFac, etc. for each item in the run in period
            foreach (var item in runInPeriodItems)
            {
                this.CalculateClimateAdjustments(
                    currentYearViewItem: item,
                    farm: farm);
            }

            // Calculate averages of all inputs from the items in the run-in period
            var result = this.AverageNonPoolInputsForRunInPeriod(runInPeriodItems: runInPeriodItems.ToList());

            // Calculate the initial stocks for the run-in period item
            this.CalculatePools(
                currentYearViewItem: result,    // Use the run-in period item
                previousYearViewItem: null,     // There is no previous year
                farm: farm,
                isEquilibriumYear: true);

            return result;
        }

        /// <summary>
        /// Creates a single view item that will store an average of all inputs in the run-in period that will be used to calculate the starting state of all the pools which
        /// will be used to start the simulation.
        /// </summary>
        public CropViewItem AverageNonPoolInputsForRunInPeriod(List<CropViewItem> runInPeriodItems)
        {
            var result = new CropViewItem();

            result.WFac = runInPeriodItems.Average(x => x.WFac);
            result.TFac = runInPeriodItems.Average(x => x.TFac);
            result.TotalCarbonInputs = runInPeriodItems.Average(y => y.TotalCarbonInputs);
            result.TotalNitrogenInputsForIpccTier2 = runInPeriodItems.Average(x => x.TotalNitrogenInputsForIpccTier2);
            result.Sand = runInPeriodItems.Average(x => x.Sand);
            result.LigninContent = runInPeriodItems.Average(x => x.LigninContent);
            result.NitrogenContent = runInPeriodItems.Average(x => x.NitrogenContent);

            // Need average of above ground residue dry matter... etc.
            result.AboveGroundResidueDryMatter = runInPeriodItems.Average(x => x.AboveGroundResidueDryMatter);
            result.BelowGroundResidueDryMatter = runInPeriodItems.Average(x => x.BelowGroundResidueDryMatter);

            result.TillageType = runInPeriodItems.First().TillageType;

            return result;
        }

        /// <summary>
        /// Calculates the factors that are used to determine the effects of temperature and precipitation on the pools.
        /// </summary>
        public void CalculateClimateAdjustments(CropViewItem currentYearViewItem, Farm farm)
        {
            var optimumTemperature = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(
               parameter: ModelParameters.OptimumTemperature,
               tillageType: currentYearViewItem.TillageType);

            var maximumTemperature = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(
                parameter: ModelParameters.MaximumAvgTemperature,
                tillageType: currentYearViewItem.TillageType);

            var climateData = farm.ClimateData;

            var precipitationsForYear = climateData.GetMonthlyPrecipitationsForYear(
                year: currentYearViewItem.Year).Select(x => x.Value).ToList();

            var evapotranspirationsForYear = climateData.GetMonthlyEvapotranspirationForYear(
                year: currentYearViewItem.Year).Select(x => x.Value).ToList();

            var temperaturesForYear = climateData.GetMonthlyTemperaturesForYear(
                year: currentYearViewItem.Year).Select(x => x.Value).ToList();

            currentYearViewItem.TFac = this.CalculateAverageAnnualTemperatureFactor(
                monthlyAverageTemperatures: temperaturesForYear,
                maximumTemperatureForDecomposition: maximumTemperature.Value,
                optimumTemperatureForDecomposition: optimumTemperature.Value);

            this.SetMonthlyTemperatureFactors(
                monthlyAverageTemperatures: temperaturesForYear,
                maximumTemperatureForDecomposition: maximumTemperature.Value,
                optimumTemperatureForDecomposition: optimumTemperature.Value,
                currentYearViewItem);

            var slopeParameter = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(
                parameter: ModelParameters.SlopeParameter,
                tillageType: currentYearViewItem.TillageType);

            currentYearViewItem.WFac = this.CalculateAnnualWaterFactor(
                monthlyTotalPrecipitations: precipitationsForYear,
                monthlyTotalEvapotranspirations: evapotranspirationsForYear,
                slopeParameter: slopeParameter.Value);

            this.SetMonthlyWaterFactors(
                monthlyTotalPrecipitations: precipitationsForYear,
                monthlyTotalEvapotranspirations: evapotranspirationsForYear,
                slopeParameter: slopeParameter.Value,
                viewItem: currentYearViewItem);
        }

        public void CalculatePools(
            CropViewItem currentYearViewItem,
            CropViewItem previousYearViewItem,
            Farm farm,
            bool isEquilibriumYear = false)
        {
            IPCCTier2Results currentYearIpccTier2Results = new IPCCTier2Results();
            IPCCTier2Results previousYearIpccTier2Results = new IPCCTier2Results();

            var inputs = 0d;
            if (this.CalculationMode == CalculationModes.Carbon)
            {
                inputs = currentYearViewItem.TotalCarbonInputs;
                currentYearIpccTier2Results = currentYearViewItem.IpccTier2CarbonResults;
            }
            else
            {
                inputs = currentYearViewItem.TotalNitrogenInputsForIpccTier2;
                currentYearIpccTier2Results = currentYearViewItem.IpccTier2NitrogenResults;
            }

            if (isEquilibriumYear == false)
            {
                if (this.CalculationMode == CalculationModes.Carbon)
                {
                    previousYearIpccTier2Results = previousYearViewItem.IpccTier2CarbonResults;
                }
                else
                {
                    previousYearIpccTier2Results = previousYearViewItem.IpccTier2NitrogenResults;
                }
            }

            var f1 = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.FractionMetabolicDMActivePool, currentYearViewItem.TillageType).Value;
            var f2 = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.FractionStructuralDMActivePool, currentYearViewItem.TillageType).Value;
            var f3 = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.FractionStructuralDMSlowPool, currentYearViewItem.TillageType).Value;
            var f5 = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.FractionActiveDecayToPassive, currentYearViewItem.TillageType).Value;
            var f6 = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.FractionSlowDecayToPassive, currentYearViewItem.TillageType).Value;
            var f7 = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.FractionSlowDecayToActive, currentYearViewItem.TillageType).Value;
            var f8 = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.FractionPassiveDecayToActive, currentYearViewItem.TillageType).Value;

            var f4 = this.CalculateAmountToSlowPool(
                fractionDecayActivePoolToPassivePool: f5,
                sand: currentYearViewItem.Sand);

            currentYearIpccTier2Results.Beta = this.CalculateAmountToDeadMatterComponent(
                totalInputs: inputs,
                nitrogenFraction: currentYearViewItem.NitrogenContent,
                ligninContent: currentYearViewItem.LigninContent);

            currentYearIpccTier2Results.Alpha = this.CalculateAmountToActivePool(
                inputToDeadMatter: currentYearIpccTier2Results.Beta,
                f1: f1,
                f2: f2,
                f3: f3,
                f4: f4,
                f5: f5,
                f6: f6,
                f7: f7,
                f8: f8,
                totalInputs: inputs,
                ligninContent: currentYearViewItem.LigninContent);

            var activePoolDecayRateConstant = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.DecayRateActive, currentYearViewItem.TillageType).Value;
            var tillageFactor = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.TillageModifier, currentYearViewItem.TillageType).Value;

            currentYearIpccTier2Results.ActivePoolDecayRate = this.CalculateActivePoolDecayRate(
                activePoolDecayRateConstant: activePoolDecayRateConstant,
                temperatureFactor: currentYearViewItem.TFac,
                waterFactor: currentYearViewItem.WFac,
                sand: currentYearViewItem.Sand,
                tillageFactor: tillageFactor);

            var slowPoolDecayRateConstant = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.DecayRateSlow, currentYearViewItem.TillageType).Value;

            currentYearIpccTier2Results.SlowPoolDecayRate = this.CalculateSlowPoolDecayRate(
                slowPoolDecayRateConstant: slowPoolDecayRateConstant,
                temperatureFactor: currentYearViewItem.TFac,
                waterFactor: currentYearViewItem.WFac,
                tillageFactor: tillageFactor);

            var passivePoolDecayRateConstant = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.DecayRatePassive, currentYearViewItem.TillageType).Value;

            currentYearIpccTier2Results.PassivePoolDecayRate = this.CalculatePassivePoolDecayRate(
                passivePoolDecayRateConstant: passivePoolDecayRateConstant,
                temperatureFactor: currentYearViewItem.TFac,
                waterFactor: currentYearViewItem.WFac);

            currentYearIpccTier2Results.ActivePoolSteadyState = this.CalculateSteadyStateActivePool(
                inputsToActiveSubPool: currentYearIpccTier2Results.Alpha,
                decayRateForActivePool: currentYearIpccTier2Results.ActivePoolDecayRate);

            if (isEquilibriumYear == false)
            {
                currentYearIpccTier2Results.ActivePool = this.CalculateActivePoolAtCurrentInterval(
                    activePoolAtPreviousInterval: previousYearIpccTier2Results.ActivePool,
                    activePoolSteadyState: currentYearIpccTier2Results.ActivePoolSteadyState,
                    decayRateForActivePool: currentYearIpccTier2Results.ActivePoolDecayRate);
            }
            else
            {
                // When calculating run-in period, steady state and pool are equal
                currentYearIpccTier2Results.ActivePool = currentYearIpccTier2Results.ActivePoolSteadyState;
            }

            currentYearIpccTier2Results.SlowPoolSteadyState = this.CalculateSteadyStateSlowPool(
                totalInputs: inputs,
                ligninContent: currentYearViewItem.LigninContent,
                f3: f3,
                steadyStateActivePool: currentYearIpccTier2Results.ActivePoolSteadyState,
                activePoolDecayRate: currentYearIpccTier2Results.ActivePoolDecayRate,
                f4: f4,
                decayRateSlowPool: currentYearIpccTier2Results.SlowPoolDecayRate);

            if (isEquilibriumYear == false)
            {
                currentYearIpccTier2Results.SlowPool = this.CalculateSlowPoolAtInterval(
                    slowPoolAtPreviousInterval: previousYearIpccTier2Results.SlowPool,
                    slowPoolSteadyState: currentYearIpccTier2Results.SlowPoolSteadyState,
                    slowPoolDecayRate: currentYearIpccTier2Results.SlowPoolDecayRate);
            }
            else
            {
                // When calculating run-in period, steady state and pool are equal
                currentYearIpccTier2Results.SlowPool = currentYearIpccTier2Results.SlowPoolSteadyState;
            }

            currentYearIpccTier2Results.PassivePoolSteadyState = this.CalculatePassivePoolSteadyState(
                activePoolSteadyState: currentYearIpccTier2Results.ActivePoolSteadyState,
                activePoolDecayRate: currentYearIpccTier2Results.ActivePoolDecayRate,
                f5: f5,
                slowPoolSteadyState: currentYearIpccTier2Results.SlowPoolSteadyState,
                slowPoolDecayRate: currentYearIpccTier2Results.SlowPoolDecayRate,
                f6: f6,
                passivePoolDecayRate: currentYearIpccTier2Results.PassivePoolDecayRate);

            if (isEquilibriumYear == false)
            {
                currentYearIpccTier2Results.PassivePool = this.CalculatePassivePoolAtInterval(
                    passivePoolAtPreviousInterval: previousYearIpccTier2Results.PassivePool,
                    passivePoolSteadyState: currentYearIpccTier2Results.PassivePoolSteadyState,
                    passivePoolDecayRate: currentYearIpccTier2Results.PassivePoolDecayRate);
            }
            else
            {
                // When calculating run-in period, steady state and pool are equal
                currentYearIpccTier2Results.PassivePool = currentYearIpccTier2Results.PassivePoolSteadyState;
            }

            if (isEquilibriumYear == false)
            {
                currentYearIpccTier2Results.ActivePoolDiff = this.CalculateStockChange(
                    socAtYear: currentYearIpccTier2Results.ActivePool,
                    socAtPreviousYear: previousYearIpccTier2Results.ActivePool);

                currentYearIpccTier2Results.SlowPoolDiff = this.CalculateStockChange(
                    socAtYear: currentYearIpccTier2Results.SlowPool,
                    socAtPreviousYear: previousYearIpccTier2Results.SlowPool);

                currentYearIpccTier2Results.PassivePoolDiff = this.CalculateStockChange(
                    socAtYear: currentYearIpccTier2Results.PassivePool,
                    socAtPreviousYear: previousYearIpccTier2Results.PassivePool);
            }
            else
            {
                currentYearIpccTier2Results.ActivePoolDiff = 0;
                currentYearIpccTier2Results.SlowPoolDiff = 0;
                currentYearIpccTier2Results.PassivePoolDiff = 0;
            }

            var totalStock = this.CalculateTotalStocks(
                activePool: currentYearIpccTier2Results.ActivePool,
                passivePool: currentYearIpccTier2Results.PassivePool,
                slowPool: currentYearIpccTier2Results.SlowPool);

            if (this.CalculationMode == CalculationModes.Carbon)
            {
                currentYearViewItem.SoilCarbon = totalStock;
                currentYearViewItem.ActivePoolCarbon = currentYearIpccTier2Results.ActivePool;
                currentYearViewItem.SlowPoolCarbon = currentYearIpccTier2Results.SlowPool;
                currentYearViewItem.PassivePoolCarbon = currentYearIpccTier2Results.PassivePool;
            }
            else
            {
                currentYearViewItem.SoilNitrogenStock = totalStock;
            }

            if (previousYearViewItem != null)
            {
                if (this.CalculationMode == CalculationModes.Carbon)
                {
                    currentYearViewItem.ChangeInCarbon = this.CalculateStockChange(currentYearViewItem.SoilCarbon, previousYearViewItem.SoilCarbon);
                }
                else
                {
                    currentYearViewItem.ChangeInNitrogenStock = this.CalculateStockChange(currentYearViewItem.SoilNitrogenStock, previousYearViewItem.SoilNitrogenStock);
                }
            }
        }

        #endregion

        #region Equations

        /// <summary>
        /// Equation 2.2.2-1
        /// </summary>
        /// <param name="slope">(unitless)</param>
        /// <param name="freshWeightOfYield">The yield of the harvest (wet/fresh weight) (kg ha^-1)</param>
        /// <param name="intercept">(unitless)</param>
        /// <param name="moistureContentAsPercentage">The moisture content of the yield (%)</param>
        /// <returns>The harvest index</returns>
        public double CalculateHarvestIndex(
            double slope,
            double freshWeightOfYield,
            double intercept,
            double moistureContentAsPercentage)
        {
            return slope * ((freshWeightOfYield / 1000) * (1 - (moistureContentAsPercentage / 100.0))) + intercept;
        }

        /// <summary>
        /// Equation 2.2.2-2
        /// </summary>
        /// <param name="harvestIndex">The harvest index (kg DM ha^-1)</param>
        /// <param name="viewItem"></param>
        /// <returns>Above ground residue dry matter for crop (kg ha^-1)</returns>
        public double CalculateAboveGroundResidueDryMatter(
            double harvestIndex,
            CropViewItem viewItem)
        {
            if (harvestIndex <= 0)
            {
                return 0;
            }

            var freshWeightOfYield = viewItem.Yield;
            var moistureContentOfCropAsPercentage = viewItem.MoistureContentOfCropPercentage;
            var moistureContentFraction = moistureContentOfCropAsPercentage / 100.0;
            var moistureFractionDifference = 1 - moistureContentFraction;

            var strawReturnedToSoilFraction = viewItem.PercentageOfStrawReturnedToSoil / 100.0;
            var productReturnedToSoilFraction = viewItem.PercentageOfProductYieldReturnedToSoil / 100.0;

            var leftFirstTerm = (freshWeightOfYield * moistureFractionDifference) / harvestIndex;
            var leftSecondTerm = freshWeightOfYield * moistureFractionDifference;
            var leftInnerDifference = leftFirstTerm - leftSecondTerm;
            var leftResult = leftInnerDifference * strawReturnedToSoilFraction;

            var rightFirstTerm = freshWeightOfYield * moistureFractionDifference;
            var rightResult = rightFirstTerm * productReturnedToSoilFraction;

            var finalResult = leftResult + rightResult;

            return finalResult;
        }

        /// <summary>
        /// Equation 2.2.2-3
        /// </summary>
        /// <param name="freshWeightOfYield">The yield of the harvest (wet/fresh weight) (kg ha^-1)</param>
        /// <param name="harvestIndex">The harvest index (kg DM ha^-1)</param>
        /// <param name="moistureContentOfCropAsPercentage">The moisture content of the yield (%)</param>
        /// <param name="percentageOfStrawReturned"></param>
        /// <returns>Above ground residue dry matter for crop (kg ha^-1)</returns>
        public double CalculateAboveGroundResidueDryMatterExported(
            double freshWeightOfYield,
            double harvestIndex,
            double moistureContentOfCropAsPercentage,
            double percentageOfStrawReturned)
        {
            if (harvestIndex <= 0)
            {
                return 0;
            }

            return (((freshWeightOfYield * (1 - moistureContentOfCropAsPercentage / 100.0)) / harvestIndex) - ((freshWeightOfYield * (1 - moistureContentOfCropAsPercentage / 100.0)))) * (1 - (percentageOfStrawReturned / 100.0));
        }

        /// <summary>
        /// Equation 2.2.2-4
        /// </summary>
        /// <param name="aboveGroundResidueDryMatterForCrop">Above ground residue dry matter for crop (kg ha^-1)</param>
        /// <param name="area">Area of field (ha)</param>
        /// <param name="fractionRenewed">(unitless)</param>
        /// <param name="fractionBurned">(unitless)</param>
        /// <param name="fractionRemoved">(unitless)</param>
        /// <param name="combustionFactor">(unitless)</param>
        /// <returns>Annual total amount of above-ground residue (kg year^-1)</returns>
        public double CalculateAnnualAboveGroundResidue(
            double aboveGroundResidueDryMatterForCrop,
            double area,
            double fractionRenewed,
            double fractionBurned,
            double fractionRemoved,
            double combustionFactor)
        {
            // Not considering burned residues right now
            return aboveGroundResidueDryMatterForCrop * area * fractionRenewed * (1 - fractionRemoved - (fractionBurned * combustionFactor));
        }

        /// <summary>
        /// Equation 2.2.2-5
        /// Equation 2.2.2-6
        /// </summary>
        /// <param name="shootToRootRatio">Ratio of below-ground root biomass to above-ground shoot biomass (RS(T)) (kg dm ha^-1 (kg dm ha^-1)^-1)</param>
        /// <param name="fractionRenewed">(unitless)</param>
        /// <param name="harvestIndex">Harvest ratio/index (R_AG(T))</param>
        /// <param name="cropViewItem"></param>
        /// <returns>Annual total amount of below-ground residue (kg year^-1)</returns>
        public double CalculateBelowGroundResidueDryMatter(
            double shootToRootRatio,
            double fractionRenewed,
            double harvestIndex,
            CropViewItem cropViewItem)
        {
            var cropArea = cropViewItem.Area;
            var freshWeight = cropViewItem.Yield;
            var moisturePercentage = cropViewItem.MoistureContentOfCropPercentage;
            var harvestMethod = cropViewItem.HarvestMethod;
            var moistureContentFraction = moisturePercentage / 100.0;
            var moistureContentDifference = 1 - moistureContentFraction;

            if (harvestIndex <= 0)
            {
                harvestIndex = 1;
            }

            var result = 0d;
            if (harvestMethod == HarvestMethods.CashCrop)
            {
                var firstTerm = (freshWeight * moistureContentDifference) / harvestIndex;

                result = firstTerm * shootToRootRatio * cropArea * fractionRenewed;
            }
            else
            {
                // Swathing, silage, green manure harvests
                var innerResult = (freshWeight * moistureContentDifference);

                result = innerResult * shootToRootRatio * cropArea * fractionRenewed;
            }

            return result;
        }

        /// <summary>
        /// Equation 2.2.3-1
        /// </summary>
        /// <param name="maximumMonthlyTemperatureForDecomposition">Maximum monthly air temperature for decomposition (degrees C)</param>
        /// <param name="monthlyTemperature">Average temperature for the month (degrees C)</param>
        /// <param name="optimumTemperatureForDecomposition">Optimum temperature for decomposition (degrees C)</param>
        /// <returns>Monthly average air temperature effect on decomposition (degrees C)</returns>
        public double CalculateMonthlyTemperatureEffectOnDecomposition(
            double maximumMonthlyTemperatureForDecomposition,
            double monthlyTemperature,
            double optimumTemperatureForDecomposition)
        {
            if (monthlyTemperature > 45)
            {
                return 0;
            }

            var temperatureFraction = (maximumMonthlyTemperatureForDecomposition - monthlyTemperature) / (maximumMonthlyTemperatureForDecomposition - optimumTemperatureForDecomposition);
            var firstTerm = Math.Pow(temperatureFraction, 0.2);
            var powerTerm = 1 - Math.Pow(temperatureFraction, 2.63);
            var exponentTerm = Math.Exp(0.076 * powerTerm);

            var result = firstTerm * exponentTerm;

            return result;
        }

        public void SetMonthlyWaterFactors(
            List<double> monthlyTotalPrecipitations,
            List<double> monthlyTotalEvapotranspirations,
            double slopeParameter,
            CropViewItem viewItem)
        {
            var monthlyWaterFactors = this.GetMonthlyWaterEffects(
                monthlyTotalPrecipitations: monthlyTotalPrecipitations,
                monthlyTotalEvapotranspirations: monthlyTotalEvapotranspirations,
                slopeParameter: slopeParameter);

            for (int i = 0; i < monthlyWaterFactors.Count; i++)
            {
                var month = (Months)(i + 1);
                var waterFactorAtMonth = monthlyWaterFactors[i];
                viewItem.MonthlyIpccTier2WaterFactors.AssignValueByMonth(waterFactorAtMonth, month);
            }
        }

        public void SetMonthlyTemperatureFactors(
            List<double> monthlyAverageTemperatures,
            double maximumTemperatureForDecomposition,
            double optimumTemperatureForDecomposition,
            CropViewItem cropViewItem)
        {
            var monthlyTemperatureFactors = this.CalculateMonthlyTemperatureFactors(
                monthlyAverageTemperatures: monthlyAverageTemperatures,
                maximumTemperatureForDecomposition: maximumTemperatureForDecomposition,
                optimumTemperatureForDecomposition: optimumTemperatureForDecomposition);

            for (int i = 0; i < monthlyTemperatureFactors.Count; i++)
            {
                var month = (Months)(i + 1);
                var monthlyTemperatureFactor = monthlyTemperatureFactors[i];
                cropViewItem.MonthlyIpccTier2TemperatureFactors.AssignValueByMonth(monthlyTemperatureFactor, month);
            }
        }


        /// <summary>
        /// Equation 2.2.3-2
        /// </summary>
        /// <param name="monthlyAverageTemperatures">Average temperatures for each month (degrees C)</param>
        /// <param name="maximumTemperatureForDecomposition">Maximum temperature for decomposition (degrees C)</param>
        /// <param name="optimumTemperatureForDecomposition">Optimum temperature for decomposition (degrees C)</param>
        /// <returns>Annual average air temperature effect on decomposition, (unitless)</returns>
        public double CalculateAverageAnnualTemperatureFactor(
            List<double> monthlyAverageTemperatures,
            double maximumTemperatureForDecomposition,
            double optimumTemperatureForDecomposition)
        {
            var monthlyValues = this.CalculateMonthlyTemperatureFactors(
                monthlyAverageTemperatures: monthlyAverageTemperatures,
                maximumTemperatureForDecomposition: maximumTemperatureForDecomposition,
                optimumTemperatureForDecomposition: optimumTemperatureForDecomposition);

            var sum = monthlyValues.Sum();

            // There might be an incomplete year where there isn't 12 values for temperature
            var annualTemperatureFactor = sum / monthlyValues.Count;

            return annualTemperatureFactor;
        }

        public List<double> CalculateMonthlyTemperatureFactors(
            List<double> monthlyAverageTemperatures,
            double maximumTemperatureForDecomposition,
            double optimumTemperatureForDecomposition)
        {
            var result = new List<double>();

            // There might be an incomplete year where there isn't 12 values for precipitation or evapotranspiration, take minimum and use that 
            // to average since we need both a precipitation and a evapotranspiration for each month
            var numberOfMonths = monthlyAverageTemperatures.Count;

            for (int monthNumber = 0; monthNumber < numberOfMonths; monthNumber++)
            {
                var temperatureAtMonth = monthlyAverageTemperatures.ElementAtOrDefault(monthNumber);

                var temperatureEffectForMonth = this.CalculateMonthlyTemperatureEffectOnDecomposition(
                    maximumMonthlyTemperatureForDecomposition: maximumTemperatureForDecomposition,
                    monthlyTemperature: temperatureAtMonth,
                    optimumTemperatureForDecomposition: optimumTemperatureForDecomposition);

                result.Add(temperatureEffectForMonth);
            }

            return result;
        }

        /// <summary>
        /// Equation 2.2.3-3
        /// Equation 2.2.3-4
        /// </summary>
        /// <param name="monthlyTotalPrecipitation">Total precipitation for month (mm)</param>
        /// <param name="monthlyTotalEvapotranspiration">Total evapotranspiration for month (mm)</param>
        /// <param name="slopeParameter">(unitless)</param>
        /// <returns>Monthly water effect on decomposition (unitless)</returns>
        public double CalculateMonthlyWaterEffectOnDecomposition(
            double monthlyTotalPrecipitation,
            double monthlyTotalEvapotranspiration,
            double slopeParameter)
        {
            var fraction = 0.0;
            if (monthlyTotalEvapotranspiration > 0)
            {
                fraction = monthlyTotalPrecipitation / monthlyTotalEvapotranspiration;
            }

            var ratio = Math.Min(1.25, fraction);
            var monthlyWaterEffectOnDecomposition = 0.2129 + (slopeParameter * ratio) - (0.2413 * Math.Pow(ratio, 2));

            return monthlyWaterEffectOnDecomposition;
        }

        /// <summary>
        /// Equation 2.2.3-5
        /// </summary>
        /// <param name="monthlyTotalPrecipitations">Total precipitation for each month (mm)</param>
        /// <param name="monthlyTotalEvapotranspirations">Total evapotranspiration for each month (mm)</param>
        /// <param name="slopeParameter">(unitless)</param>
        /// <returns>Annual water effect on decomposition (unitless)</returns>
        public double CalculateAnnualWaterFactor(
            List<double> monthlyTotalPrecipitations,
            List<double> monthlyTotalEvapotranspirations,
            double slopeParameter)
        {
            var monthlyValues = this.GetMonthlyWaterEffects(
                monthlyTotalPrecipitations: monthlyTotalPrecipitations,
                monthlyTotalEvapotranspirations: monthlyTotalEvapotranspirations,
                slopeParameter: slopeParameter);

            var total = monthlyValues.Sum();
            var average = total / monthlyValues.Count;
            var annualWaterEffect = 1.5 * average;

            return annualWaterEffect;
        }

        public List<double> GetMonthlyWaterEffects(
            List<double> monthlyTotalPrecipitations,
            List<double> monthlyTotalEvapotranspirations, 
            double slopeParameter)
        {
            var monthlyValues = new List<double>();

            // There might be an incomplete year where there isn't 12 values for precipitation or evapotranspiration, take minimum and use that 
            // to average since we need both a precipitation and a evapotranspiration for each month
            var numberOfMonths = Math.Min(monthlyTotalPrecipitations.Count, monthlyTotalEvapotranspirations.Count);

            for (int monthNumber = 0; monthNumber < numberOfMonths; monthNumber++)
            {
                var precipitationAtMonth = monthlyTotalPrecipitations.ElementAtOrDefault(monthNumber);
                var evapotranspirationAtMonth = monthlyTotalEvapotranspirations.ElementAtOrDefault(monthNumber);

                var waterEffectForMonth = this.CalculateMonthlyWaterEffect(
                    monthlyTotalPrecipitation: precipitationAtMonth,
                    monthlyTotalEvapotranspiration: evapotranspirationAtMonth,
                    slopeParameter: slopeParameter);

                monthlyValues.Add(waterEffectForMonth);
            }

            return monthlyValues;
        }

        public double CalculateMonthlyWaterEffect(
            double monthlyTotalPrecipitation,
            double monthlyTotalEvapotranspiration,
            double slopeParameter)
        {
            var waterEffectForMonth = this.CalculateMonthlyWaterEffectOnDecomposition(
                monthlyTotalPrecipitation: monthlyTotalPrecipitation,
                monthlyTotalEvapotranspiration: monthlyTotalEvapotranspiration,
                slopeParameter: slopeParameter);

            return waterEffectForMonth;
        }

        /// <summary>
        /// Equation 2.2.3-6
        /// </summary>
        /// <param name="fractionDecayActivePoolToPassivePool">Fraction of active sub-pool decay products transferred to the slow sub-pool (proportion)</param>
        /// <param name="sand">Fraction of 0-30 cm soil mass that is sand (0.05 - 2 mm particles) (proportion)</param>
        /// <returns>Fraction of active sub-pool decay products transferred to the slow sub-pool (proportion)</returns>
        public double CalculateAmountToSlowPool(
            double fractionDecayActivePoolToPassivePool,
            double sand)
        {
            var result = 1 - fractionDecayActivePoolToPassivePool - (0.17 + 0.68 * sand);

            return result;
        }

        /// <summary>
        /// <para>Equation 2.2.3-7</para>
        /// 
        /// <para>Equation 2.7.3-1 (where totalInputs = Total organic N input (t N ha-1))</para>
        /// </summary>
        /// <param name="totalInputs">Total carbon input (kg C ha^-1 year^-1)</param>
        /// <param name="nitrogenFraction">Nitrogen fraction of the carbon input, (unitless)</param>
        /// <param name="ligninContent">Lignin content of carbon input, (unitless)</param>
        /// <returns>Carbon input to the metabolic dead organic matter carbon component (kg C ha^-1 year^-1)</returns>
        public double CalculateAmountToDeadMatterComponent(
            double totalInputs,
            double nitrogenFraction,
            double ligninContent)
        {
            var ratio = 0.0;
            if (nitrogenFraction > 0)
            {
                ratio = ligninContent / nitrogenFraction;
            }

            var innerTerm = 0.85 - 0.018 * (ratio);

            var result = totalInputs * innerTerm;

            return result;
        }

        /// <summary>
        /// <para>Equation 2.2.3-8</para>
        /// <para>Equation 2.7.3-2</para>
        /// </summary>
        /// <param name="inputToDeadMatter">Carbon input to the metabolic dead organic matter component (kg C ha^-1 year^-1)</param>
        /// <param name="f1">Fraction of metabolic dead organic matter decay products transferred to the active sub-pool (unitless)</param>
        /// <param name="f2">Fraction of structural dead organic matter decay products transferred to the active sub-pool (unitless)</param>
        /// <param name="f3">Fraction of structural dead organic matter decay products transferred to the slow sub-pool (unitless)</param>
        /// <param name="f4">Fraction of active sub-pool decay products transferred to the slow sub-pool (unitless)</param>
        /// <param name="f5">Fraction of active sub-pool decay products transferred to the passive sub-pool (unitless)</param>
        /// <param name="f6">Fraction of slow sub-pool decay products transferred to the passive sub-pool (unitless)</param>
        /// <param name="f7">Fraction of slow sub-pool decay products transferred to the active sub-pool (unitless)</param>
        /// <param name="f8">Fraction of passive sub-pool decay products transferred to the active sub-pool (unitless)</param>
        /// <param name="totalInputs">Total carbon input (kg C ha^-1 year^-1) for equation 2.2.3-8 and Total organic N input (t N ha-1) for equation 2.7.3-2</param>
        /// <param name="ligninContent">Lignin content of carbon input (unitless)</param>
        /// <returns>Carbon input to the active soil carbon sub-pool (kg C ha^-1)</returns>
        public double CalculateAmountToActivePool(
            double inputToDeadMatter,
            double f1,
            double f2,
            double f3,
            double f4,
            double f5,
            double f6,
            double f7,
            double f8,
            double totalInputs,
            double ligninContent)
        {
            var a = inputToDeadMatter * f1;
            var b = (totalInputs * (1 - ligninContent) - inputToDeadMatter) * f2;
            var c = ((totalInputs * ligninContent) * f3) * (f7 + f6 * f8);
            var d = 1 - (f4 * f7) - (f5 * f8) - (f4 * f6 * f8);

            var result = (a + b + c) / d;

            return result;
        }

        /// <summary>
        /// Equation 2.2.3-9
        /// </summary>
        /// <param name="activePoolDecayRateConstant">Decay rate constant under optimal conditions for decomposition of the active SOC sub-pool (unitless)</param>
        /// <param name="temperatureFactor">Temperature effect on decomposition (unitless)</param>
        /// <param name="waterFactor">Water effect on decomposition (unitless)</param>
        /// <param name="sand">Fraction of 0-30 cm soil mass that is sand (0.050 – 2mm particles), (unitless)</param>
        /// <param name="tillageFactor">Tillage disturbance modifier on decay rate for active and slow sub-pools (unitless)</param>
        /// <returns>Decay rate for active SOC sub-pool, (year^-1)</returns>
        public double CalculateActivePoolDecayRate(
            double activePoolDecayRateConstant,
            double temperatureFactor,
            double waterFactor,
            double sand,
            double tillageFactor)
        {
            var result = activePoolDecayRateConstant * temperatureFactor * waterFactor * (0.25 + (0.75 * sand)) * tillageFactor;

            return result;
        }

        /// <summary>
        /// Equation 2.2.3-10
        /// </summary>
        /// <param name="slowPoolDecayRateConstant">Decay rate constant under optimal condition for decomposition of the slow carbon sub-pool, (year^-1)</param>
        /// <param name="temperatureFactor">Temperature effect on decomposition (unitless)</param>
        /// <param name="waterFactor">Water effect on decomposition (unitless)</param>
        /// <param name="tillageFactor">Tillage disturbance modifier on decay rate for active and slow sub-pools (unitless)</param>
        /// <returns></returns>
        public double CalculateSlowPoolDecayRate(
            double slowPoolDecayRateConstant,
            double temperatureFactor,
            double waterFactor,
            double tillageFactor)
        {
            var result = slowPoolDecayRateConstant * temperatureFactor * waterFactor * tillageFactor;

            return result;
        }

        /// <summary>
        /// Equation 2.2.3-11
        /// </summary>
        /// <param name="passivePoolDecayRateConstant">Decay rate constant under optimal conditions for decomposition of the slow carbon sub-pool, (year^1)</param>
        /// <param name="temperatureFactor">Temperature effect on decomposition, (unitless)</param>
        /// <param name="waterFactor">Water effect on decomposition, (unitless)</param>
        /// <returns>Decay rate for passive SOC sub-pool, (year^-1)</returns>
        public double CalculatePassivePoolDecayRate(
            double passivePoolDecayRateConstant,
            double temperatureFactor,
            double waterFactor)
        {
            var result = passivePoolDecayRateConstant * temperatureFactor * waterFactor;

            return result;
        }

        /// <summary>
        /// <para>Equation 2.2.3-12</para>
        /// <para>Equation 2.7.3-3</para>
        /// </summary>
        /// <param name="inputsToActiveSubPool"><para>Equation 2.2.3-12 = Carbon input to the active SOC sub-pool, (kg C ha^-1 year^-1)</para>
        /// <para>Equation 2.7.3-3 = Carbon input to the active SOC sub-pool, (kg C ha^-1 year^-1)</para></param>
        /// <param name="decayRateForActivePool">N input to the active soil C sub-pool (t N ha-1)</param>
        /// <returns>Steady state active sub-pool SOC stock given conditions in year y, (kg C ha^-1)</returns>
        public double CalculateSteadyStateActivePool(
            double inputsToActiveSubPool,
            double decayRateForActivePool)
        {
            if (decayRateForActivePool == 0)
            {
                return 0;
            }

            var result = inputsToActiveSubPool / decayRateForActivePool;

            return result;
        }

        /// <summary>
        /// <para>Equation 2.2.3-13</para>
        /// <para>Equation 2.7.3-4</para>
        /// </summary>
        /// <param name="activePoolAtPreviousInterval">Active sub-pool SOC stock in previous year, (kg C ha^-1)</param>
        /// <param name="activePoolSteadyState">Steady state active sub-pool SOC stock given conditions in year y, (kg C ha^-1)</param>
        /// <param name="decayRateForActivePool">decay rate for active SOC sub-pool, (year^-1)</param>
        /// <returns>Active sub-pool SOC stock in year y, (kg C ha^-1)</returns>
        public double CalculateActivePoolAtCurrentInterval(
            double activePoolAtPreviousInterval,
            double activePoolSteadyState,
            double decayRateForActivePool)
        {
            if (decayRateForActivePool > 1)
            {
                decayRateForActivePool = 1;
            }

            var result = activePoolAtPreviousInterval + (activePoolSteadyState - activePoolAtPreviousInterval) * decayRateForActivePool;

            return result;
        }

        /// <summary>
        /// <para>Equation 2.2.3-14</para>
        /// <para>Equation 2.7.3-5</para>
        /// </summary>
        /// <param name="totalInputs"><para>Equation 2.2.3-14 = Total carbon input, (kg C ha^-1)</para>
        /// <para>Equation 2.7.3-5 = Total organic N input (t N ha-1)</para></param>
        /// <param name="ligninContent">Lignin content of carbon input, (unitless)</param>
        /// <param name="f3">Fraction of structural dead organic matter decay products transferred to the slow sub-pool (unitless)</param>
        /// <param name="steadyStateActivePool">Steady state active sub-pool SOC stock given conditions in year y, (kg C ha^-1)</param>
        /// <param name="activePoolDecayRate">Decay rate for active carbon sub-pool in the soil, (year^-1)</param>
        /// <param name="f4">Fraction of active sub-pool decay products transferred to the slow sub-pool (unitless)</param>
        /// <param name="decayRateSlowPool">Decay rate for slow SOC sub-pool, (year^-1)</param>
        /// <returns>Steady state slow sub-pool SOC stock given conditions in year y, (kg C ha^-1)</returns>
        public double CalculateSteadyStateSlowPool(
            double totalInputs,
            double ligninContent,
            double f3,
            double steadyStateActivePool,
            double activePoolDecayRate,
            double f4,
            double decayRateSlowPool)
        {
            var a = (totalInputs * ligninContent) * f3;
            var b = (steadyStateActivePool * activePoolDecayRate) * f4;

            var result = (a + b) / decayRateSlowPool;

            return result;
        }

        /// <summary>
        /// <para>Equation 2.2.3-15</para>
        /// <para>Equation 2.7.3-6</para>
        /// </summary>
        /// <param name="slowPoolAtPreviousInterval">Slow sub-pool SOC stock in previous year, (kg C ha^-1)</param>
        /// <param name="slowPoolSteadyState">Steady state slow sub-pool SOC stock given conditions in year y, (kg C ha^-1)</param>
        /// <param name="slowPoolDecayRate">Decay rate for slow SOC sub-pool, (year^-1)</param>
        /// <returns>Slow sub-pool SOC stock in y, (kg C ha^-1)</returns>
        public double CalculateSlowPoolAtInterval(
            double slowPoolAtPreviousInterval,
            double slowPoolSteadyState,
            double slowPoolDecayRate)
        {
            if (slowPoolDecayRate > 1)
            {
                slowPoolDecayRate = 1;
            }

            var result = slowPoolAtPreviousInterval + (slowPoolSteadyState - slowPoolAtPreviousInterval) * slowPoolDecayRate;

            return result;
        }

        /// <summary>
        /// <para>Equation 2.2.3-16</para>
        /// <para>Equation 2.7.3-7</para>
        /// </summary>
        /// <param name="activePoolSteadyState">Steady state active sub-pool SOC stock given conditions in year y, (kg C ha^-1)</param>
        /// <param name="activePoolDecayRate">Decay rate for active carbon sub-pool in the soil, (year^-1)</param>
        /// <param name="f5">Fraction of active sub-pool decay products transferred to the slow sub-pool, (unitless)</param>
        /// <param name="slowPoolSteadyState">Steady state slow sub-pool SOC stock given conditions in year y, (kg C ha^-1)</param>
        /// <param name="slowPoolDecayRate">Decay rate for slow SOC sub-pool, (year^-1)</param>
        /// <param name="f6">Fraction of slow sub-pool decay products transferred to the passive sub-pool, (unitless)</param>
        /// <param name="passivePoolDecayRate">Decay rate for passive SOC sub-pool, (year^-1)(</param>
        /// <returns>Steady state passive sub-pool SOC given conditions in year y, (kg C ha^-1)</returns>
        public double CalculatePassivePoolSteadyState(
            double activePoolSteadyState,
            double activePoolDecayRate,
            double f5,
            double slowPoolSteadyState,
            double slowPoolDecayRate,
            double f6,
            double passivePoolDecayRate)
        {
            if (passivePoolDecayRate > 1)
            {
                passivePoolDecayRate = 1;
            }

            var a = (activePoolSteadyState * activePoolDecayRate) * f5;
            var b = (slowPoolSteadyState * slowPoolDecayRate) * f6;

            var result = (a + b) / passivePoolDecayRate;

            return result;
        }

        /// <summary>
        /// <para>Equation 2.2.3-17</para>
        /// <para>Equation 2.7.3-8</para>
        /// </summary>
        /// <param name="passivePoolAtPreviousInterval">Passive sub-pool SOC stock in previous year, (kg C ha^-1)</param>
        /// <param name="passivePoolSteadyState">Steady state passive sub-pool SOC given conditions in year y, (kg C ha^-1)</param>
        /// <param name="passivePoolDecayRate">Decay rate for passive SOC sub-pool, (year^-1)</param>
        /// <returns>Passive sub-pool SOC stock in year y, (kg C ha^-1)</returns>
        public double CalculatePassivePoolAtInterval(
            double passivePoolAtPreviousInterval,
            double passivePoolSteadyState,
            double passivePoolDecayRate)
        {
            if (passivePoolDecayRate > 1)
            {
                passivePoolDecayRate = 1;
            }

            var result = passivePoolAtPreviousInterval + (passivePoolSteadyState - passivePoolAtPreviousInterval) * passivePoolDecayRate;

            return result;
        }

        /// <summary>
        /// Equation 2.2.10-4
        /// </summary>
        /// <param name="activePool">Active sub-pool SOC stock in year y for grid cell or region, (kg C ha^-1)</param>
        /// <param name="passivePool">Passive sub-pool SOC stock in year y for grid cell or region, (kg C ha^-1)</param>
        /// <param name="slowPool">Slow sub-pool SOC stock in year y for grid cell or region, (kg C ha^-1)</param>
        /// <returns>SOC stock at the end of the current year y for grid cell or region , (kg C ha^-1)</returns>
        public double CalculateTotalStocks(
            double activePool,
            double passivePool,
            double slowPool)
        {
            return activePool + slowPool + passivePool;
        }

        /// <summary>
        /// Equation 2.2.10-5
        /// </summary>
        /// <param name="socAtYear">SOC stock at the end of the current year y for grid cell or region, (kg C ha^-1)</param>
        /// <param name="socAtPreviousYear">SOC stock at the end of the previous year for grid cell or region, (kg C ha^-1)</param>
        /// <returns>Annual stock change factor for mineral soils in grid cell or region i, (kg C ha^-1)</returns>
        public double CalculateStockChange(
            double socAtYear,
            double socAtPreviousYear)
        {
            return socAtYear - socAtPreviousYear;
        }

        #endregion
    }
}