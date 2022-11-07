using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
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
        public enum CalculationModes
        {
            Carbon,
            Nitrogen,
        }

        #region Fields

        private readonly Table_12_Nitrogen_Lignin_Content_In_Crops_Provider _slopeProvider = new Table_12_Nitrogen_Lignin_Content_In_Crops_Provider();
        private readonly Table_11_Globally_Calibrated_Model_Parameters_Provider _globallyCalibratedModelParametersProvider = new Table_11_Globally_Calibrated_Model_Parameters_Provider();        

        #endregion

        public IPCCTier2SoilCarbonCalculator()
        {
            this.CalculationMode = CalculationModes.Carbon;
        }

        #region Properties

        public CalculationModes CalculationMode { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Allow the user to specify a custom starting carbon value for the simulation.
        /// </summary>
        public void AssignCustomStartPoint(CropViewItem equilibriumYear, Farm farm, CropViewItem currentYearViewItem)
        {
            var soc = equilibriumYear.SoilCarbon;

            // Active pool fraction
            var activePool = equilibriumYear.IpccTier2CarbonResults.ActivePool;
            var activePoolFraction = activePool / soc;

            // Passive pool fraction
            var passivePool = equilibriumYear.IpccTier2CarbonResults.PassivePool;
            var passivePoolFraction = passivePool / soc;

            // Slow pool fraction
            var slowPool = equilibriumYear.IpccTier2CarbonResults.SlowPool;
            var slowPoolFraction = slowPool / soc;

            var customStartingActivePool = activePoolFraction * farm.StartingSoilOrganicCarbon;
            var customStartingPassivePool = passivePoolFraction * farm.StartingSoilOrganicCarbon;
            var customStartingSlowPool = slowPoolFraction * farm.StartingSoilOrganicCarbon;

            equilibriumYear.IpccTier2CarbonResults.ActivePool = customStartingActivePool;
            equilibriumYear.IpccTier2CarbonResults.PassivePool = customStartingPassivePool;
            equilibriumYear.IpccTier2CarbonResults.SlowPool = customStartingSlowPool;

            // Equation 2.2.2-28
            currentYearViewItem.IpccTier2CarbonResults.ActivePool = equilibriumYear.IpccTier2CarbonResults.ActivePool;

            // Equation 2.2.2-29
            currentYearViewItem.IpccTier2CarbonResults.SlowPool = equilibriumYear.IpccTier2CarbonResults.SlowPool;

            // Equation 2.2.2-30
            currentYearViewItem.IpccTier2CarbonResults.PassivePool = equilibriumYear.IpccTier2CarbonResults.PassivePool;

            currentYearViewItem.SoilCarbon = farm.StartingSoilOrganicCarbon;
        }

        public void CalculateResults(
            Farm farm, 
            List<CropViewItem> viewItemsByField, 
            FieldSystemComponent fieldSystemComponent)
        {
            var runInPeriodItems = fieldSystemComponent.RunInPeriodItems.ToList();
            if (runInPeriodItems.Any() == false)
            {
                // Will occur with some old farms. User will have to rebuild detail view items
                return;
            }

            var runInPeriod = this.CalculateRunInPeriod(
                farm: farm,
                runInPeriodItems: runInPeriodItems);

            var nonRunInPeriodItems = viewItemsByField.OrderBy(x => x.Year).ToList();

            for (int i = 0; i < nonRunInPeriodItems.Count; i++)
            {
                CropViewItem currentYearViewItem = nonRunInPeriodItems.ElementAt(i);
                CropViewItem previousYearViewItem;

                if (i > 0)
                {
                    previousYearViewItem = nonRunInPeriodItems.ElementAt(i - 1);
                }
                else
                {
                    previousYearViewItem = runInPeriod;
                }

                this.CalculateClimateAdjustments(
                    currentYearViewItem: currentYearViewItem,
                    farm: farm);

                this.CalculatePools(
                    currentYearViewItem: currentYearViewItem,
                    previousYearViewItem: previousYearViewItem,
                    farm: farm) ;

                if (i == 0 && farm.UseCustomStartingSoilOrganicCarbon)
                {
                    // Override the calculated starting points with custom user defined fractions of each the pools
                    AssignCustomStartPoint(runInPeriod, farm, currentYearViewItem);
                }

                this.CalculationMode = CalculationModes.Nitrogen;
                this.CalculateNitrogenAtInterval(previousYearViewItem, currentYearViewItem, null, farm, i);

                // Change back to C for next iteration
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

            // Equation 2.2.3-1
            var harvestRatio = this.CalculateHarvestRatio(
                slope: slope,
                freshWeightOfYield: viewItem.Yield,
                intercept: intercept,
                moistureContentAsPercentage: viewItem.MoistureContentOfCropPercentage);

            // Equation 2.2.3-2
            var aboveGroundResidueDryMatter = this.CalculateAboveGroundResidueDryMatter(
                freshWeightOfYield: viewItem.Yield,
                harvestRatio: harvestRatio,
                moistureContentOfCropAsPercentage: viewItem.MoistureContentOfCropPercentage);

            viewItem.AboveGroundResidueDryMatter = aboveGroundResidueDryMatter;

            var fractionRenewed = viewItem.CropType.IsAnnual() ? 1 : 1 / viewItem.PerennialStandLength;

            // Equation 2.2.3-3
            var aboveGroundResidue = this.CalculateAnnualAboveGroundResidue(
                aboveGroundResidueForCrop: aboveGroundResidueDryMatter,
                area: viewItem.Area,
                fractionRenewed: fractionRenewed,
                fractionBurned: 0, 
                fractionRemoved: 0, 
                combustionFactor: 0);

            const double AboveGroundCarbonContent = 0.42;

            var totalAboveGroundCarbonInputsForField = aboveGroundResidue * AboveGroundCarbonContent;

            // Note that eq. 2.2.3-3 is the residue for the entire field, we report per ha on the details screen so we divide by the area here
            viewItem.AboveGroundCarbonInput = totalAboveGroundCarbonInputsForField / viewItem.Area;

            var supplementalFeedingAmount = this.CalculateInputsFromSupplementalHayFedToGrazingAnimals(
                previousYearViewItem: null,
                currentYearViewItem: viewItem,
                nextYearViewItems: null,
                farm: farm);

            viewItem.AboveGroundCarbonInput += supplementalFeedingAmount;

            var rootToShoot = cropData.RSTRatio;
            
            // Equation 2.2.2-4
            var belowGroundResidue = this.CalculateAnnualBelowGroundResidue(
                aboveGroundResideDryMatterForCrop: aboveGroundResidueDryMatter,
                shootToRootRatio: rootToShoot,
                area: viewItem.Area,
                fractionRenewed: fractionRenewed);

            viewItem.BelowGroundResidueDryMatter = belowGroundResidue;

            const double BelowGroundCarbonContent = 0.42;

            var totalBelowGroundCarbonInputsForField = belowGroundResidue * BelowGroundCarbonContent;

            // Note that eq. 2.2.3-3 is the residue for the entire field, we report per ha on the details screen so we divide by the area here
            viewItem.BelowGroundCarbonInput = totalBelowGroundCarbonInputsForField / viewItem.Area;

            viewItem.ManureCarbonInputsPerHectare = this.CalculateManureCarbonInputPerHectare(viewItem, farm);

            // Equation 2.2.3-5
            viewItem.TotalCarbonInputs = viewItem.AboveGroundCarbonInput + viewItem.BelowGroundCarbonInput + viewItem.ManureCarbonInputsPerHectare;  
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

            var evapotranspirationsForYear = climateData.GetMonthlyEvapotranspirationsForYear(
                year: currentYearViewItem.Year).Select(x => x.Value).ToList();

            var temperaturesForYear = climateData.GetMonthlyTemperaturesForYear(
                year: currentYearViewItem.Year).Select(x => x.Value).ToList();

            // Equation 2.2.2-12            
            currentYearViewItem.TFac = this.CalculateAverageAnnualTemperatureFactor(
                monthlyAverageTemperatures: temperaturesForYear,
                maximumTemperatureForDecomposition: maximumTemperature.Value,
                optimumTemperatureForDecomposition: optimumTemperature.Value);

            var slopeParameter = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(
                parameter: ModelParameters.SlopeParameter,
                tillageType: currentYearViewItem.TillageType);

            // Equation 2.2.2-15            
            currentYearViewItem.WFac = this.CalculateAnnualWaterFactor(
                monthlyTotalPrecipitations: precipitationsForYear,
                monthlyTotalEvapotranspirations: evapotranspirationsForYear,
                slopeParameter: slopeParameter.Value);            
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

            var totalStock  = this.CalculateTotalStocks(
                activePool: currentYearIpccTier2Results.ActivePool,
                passivePool: currentYearIpccTier2Results.PassivePool,
                slowPool: currentYearIpccTier2Results.SlowPool);

            if (this.CalculationMode == CalculationModes.Carbon)
            {
                currentYearViewItem.SoilCarbon = totalStock;
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
        /// Equation 2.2.3-1
        /// </summary>
        /// <param name="slope">(unitless)</param>
        /// <param name="freshWeightOfYield">The yield of the harvest (wet/fresh weight) (kg ha^-1)</param>
        /// <param name="intercept">(unitless)</param>
        /// <param name="moistureContentAsPercentage">The moisture content of the yield (%)</param>
        /// <returns>The harvest ratio</returns>
        public double CalculateHarvestRatio(
            double slope,
            double freshWeightOfYield,
            double intercept,
            double moistureContentAsPercentage)
        {
            return slope * ((freshWeightOfYield / 1000) * (1 - moistureContentAsPercentage / 100)) + intercept;
        }

        /// <summary>
        /// Equation 2.2.2-2
        /// </summary>
        /// <param name="freshWeightOfYield">The yield of the harvest (wet/fresh weight) (kg ha^-1)</param>
        /// <param name="harvestRatio">The harvest ratio (kg ha^-1)</param>
        /// <param name="moistureContentOfCropAsPercentage">The moisture content of the yield (%)</param>
        /// <returns>Above ground residue dry matter for crop (kg ha^-1)</returns>
        public double CalculateAboveGroundResidueDryMatter(
            double freshWeightOfYield,
            double harvestRatio,
            double moistureContentOfCropAsPercentage)
        {
            if (harvestRatio <= 0)
            {
                return 0;
            }

            return (freshWeightOfYield * (1 - moistureContentOfCropAsPercentage / 100) ) / harvestRatio;
        }

        /// <summary>
        /// Equation 2.2.2-3
        /// </summary>
        /// <param name="aboveGroundResidueForCrop">Above ground residue dry matter for crop (kg ha^-1)</param>
        /// <param name="area">Area of field (ha)</param>
        /// <param name="fractionRenewed">(unitless)</param>
        /// <param name="fractionBurned">(unitless)</param>
        /// <param name="fractionRemoved">(unitless)</param>
        /// <param name="combustionFactor">(unitless)</param>
        /// <returns>Annual total amount of above-ground residue (kg year^-1)</returns>
        public double CalculateAnnualAboveGroundResidue(
            double aboveGroundResidueForCrop,
            double area,
            double fractionRenewed,
            double fractionBurned,
            double fractionRemoved, double combustionFactor)
        {
            // Not considering burned residues right now
            return aboveGroundResidueForCrop * area * fractionRenewed * (1 - fractionRemoved - (fractionBurned * combustionFactor));
        }

        /// <summary>
        /// Equation 2.2.2-4
        /// </summary>        
        /// <param name="aboveGroundResideDryMatterForCrop">Above ground residue dry matter for crop (kg ha^-1)</param>
        /// <param name="shootToRootRatio">Ratio of below-ground root biomass to above-ground shoot biomass (kg dm ha^-1 (kg dm ha^-1)^-1)</param>
        /// <param name="area">Area of field (ha)</param>
        /// <param name="fractionRenewed">(unitless)</param>
        /// <returns>Annual total amount of below-ground residue (kg year^-1)</returns>
        public double CalculateAnnualBelowGroundResidue(
            double aboveGroundResideDryMatterForCrop,
            double shootToRootRatio,
            double area,
            double fractionRenewed)
        {
            return aboveGroundResideDryMatterForCrop * shootToRootRatio * area * fractionRenewed;
        }

        /// <summary>
        /// Equation 2.2.2-11
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
            var temperatureFraction = (maximumMonthlyTemperatureForDecomposition - monthlyTemperature) / (maximumMonthlyTemperatureForDecomposition - optimumTemperatureForDecomposition);
            var firstTerm = Math.Pow(temperatureFraction, 0.2);
            var powerTerm = 1 - Math.Pow(temperatureFraction, 2.63);
            var exponentTerm = Math.Exp(0.076 * powerTerm);

            var result = firstTerm * exponentTerm;

            return result;
        }

        /// <summary>
        /// Equation 2.2.2-12
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
            var monthlyValues = new List<double>();

            // There might be an imcomplete year where there isn't 12 values for precipitation or evapotranspiration, take minimum and use that 
            // to average since we need both a precipitation and a evapotranspiration for each month
            var numberOfMonths = monthlyAverageTemperatures.Count;

            for (int i = 0; i < numberOfMonths; i++)
            {
                var temperatureAtMonth = monthlyAverageTemperatures.ElementAtOrDefault(i);

                var temperatureEffectForMonth = this.CalculateMonthlyTemperatureEffectOnDecomposition(
                    maximumMonthlyTemperatureForDecomposition: maximumTemperatureForDecomposition,
                    monthlyTemperature: temperatureAtMonth,
                    optimumTemperatureForDecomposition: optimumTemperatureForDecomposition);

                if (temperatureAtMonth > 45)
                {
                    temperatureEffectForMonth = 0;
                }

                monthlyValues.Add(temperatureEffectForMonth);
            }
                        
            var sum = monthlyValues.Sum();

            // There might be an imcomplete year where there isn't 12 values for temperature
            var annualTemperatureFactor = sum / numberOfMonths;

            return annualTemperatureFactor;
        }

        /// <summary>
        /// Equation 2.2.2-13
        /// Equation 2.2.2-14
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
        /// Equation 2.2.2-15
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
            var monthlyValues = new List<double>();

            // There might be an incomplete year where there isn't 12 values for precipitation or evapotranspiration, take minimum and use that 
            // to average since we need both a precipitation and a evapotranspiration for each month
            var numberOfMonths = Math.Min(monthlyTotalPrecipitations.Count, monthlyTotalEvapotranspirations.Count);

            for (int i = 0; i < numberOfMonths; i++)
            {
                var precipitationAtMonth = monthlyTotalPrecipitations.ElementAtOrDefault(i);
                var evapotranspirationAtMonth = monthlyTotalEvapotranspirations.ElementAtOrDefault(i);

                var waterEffectForMonth = this.CalculateMonthlyWaterEffectOnDecomposition(
                    monthlyTotalPrecipitation: precipitationAtMonth,
                    monthlyTotalEvapotranspiration: evapotranspirationAtMonth,
                    slopeParameter: slopeParameter);

                monthlyValues.Add(waterEffectForMonth);
            }

            var total = monthlyValues.Sum();

            var average = total / numberOfMonths;

            var annualWaterEffect = 1.5 * average;

            return annualWaterEffect;
        }

        /// <summary>
        /// Equation 2.2.2-16
        /// Equation 2.2.2-39
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
        /// Equation 2.2.2-17
        /// Equation 2.2.2-28
        /// </summary>
        /// <param name="totalCarbon">Total carbon input (tonnes C ha^-1 year^-1)</param>
        /// <param name="nitrogenFraction">Nitrogen fraction of the carbon input, (unitless)</param>
        /// <param name="ligninContent">Lignin content of carbon input, (unitless)</param>
        /// <returns>Carbon input to the metabolic dead organic matter carbon component (tonnes C ha^-1 year^-1)</returns>
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
        /// Equation 2.2.2-18
        /// Equation 2.2.2-29
        /// </summary>
        /// <param name="inputToDeadMatter">Caron input to the metabolic dead organic matter component (tonnes C ha^-1 year^-1)</param>
        /// <param name="f1">Fraction of metabolic dead organic matter decay products transferred to the active sub-pool (unitless)</param>
        /// <param name="f2">Fraction of structural dead organic matter decay products transferred to the active sub-pool (unitless)</param>
        /// <param name="f3">Fraction of structural dead organic matter decay products transferred to the slow sub-pool (unitless)</param>
        /// <param name="f4">Fraction of active sub-pool decay products transferred to the slow sub-pool (unitless)</param>
        /// <param name="f5">Fraction of active sub-pool decay products transferred to the passive sub-pool (unitless)</param>
        /// <param name="f6">Fraction of slow sub-pool decay products transferred to the passive sub-pool (unitless)</param>
        /// <param name="f7">Fraction of slow sub-pool decay products transferred to the active sub-pool (unitless)</param>
        /// <param name="f8">Fraction of passive sub-pool decay products transferred to the active sub-pool (unitless)</param>
        /// <param name="totalInputs">Total carbon input (tonnes C ha^-1 year^-1)</param>
        /// <param name="ligninContent">Lignin content of carbon input (unitless)</param>
        /// <returns>Carbon input to the active soil carbon sub-pool (tonnes C ha^-1)</returns>
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
        /// Equation 2.2.2-19
        /// Equation 2.2.2-42
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
        /// Equation 2.2.2-20
        /// Equation 2.2.2-38
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
        /// Equation 2.2.2-21
        /// Equation 2.2.2-35
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
        /// Equation 2.2.2-22
        /// Equation 2.2.2-43
        /// Equation 2.7.3-3
        /// </summary>
        /// <param name="inputsToActiveSubPool">Carbon input to the active SOC sub-pool, (tonnes C ha^-1 year^-1)</param>
        /// <param name="decayRateForActivePool">Decay rate for active SOC sub-pool (unitless)</param>
        /// <returns>Steady state active sub-pool SOC stock given conditions in year y, (tonnes C ha^-1)</returns>
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
        /// Equation 2.2.2-23
        /// Equation 2.2.2-44
        /// Equation 2.7.3-4
        /// </summary>
        /// <param name="activePoolAtPreviousInterval">Active sub-pool SOC stock in previous year, (tonnes C ha^-1)</param>
        /// <param name="activePoolSteadyState">Steady state active sub-pool SOC stock given conditions in year y, (tonnes C ha^-1)</param>
        /// <param name="decayRateForActivePool">decay rate for active SOC sub-pool, (year^-1)</param>
        /// <returns>Active sub-pool SOC stock in year y, (tonnes C ha^-1)</returns>
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
        /// Equation 2.2.2-24
        /// Equation 2.2.2-40
        /// Equation 2.7.3-5
        /// </summary>
        /// <param name="totalInputs">Total carbon input, (tonnes C ha^-1)</param>
        /// <param name="ligninContent">Lignin content of carbon input, (unitless)</param>
        /// <param name="f3">Fraction of structural dead organic matter decay products transferred to the slow sub-pool (unitless)</param>
        /// <param name="steadyStateActivePool">Steady state active sub-pool SOC stock given conditions in year y, (tonnes C ha^-1)</param>
        /// <param name="activePoolDecayRate">Decay rate for active carbon sub-pool in the soil, (year^-1)</param>
        /// <param name="f4">Fraction of active sub-pool decay products transferred to the slow sub-pool (unitless)</param>
        /// <param name="decayRateSlowPool">Decay rate for slow SOC sub-pool, (year^-1)</param>
        /// <returns>Steady state slow sub-pool SOC stock given conditions in year y, (tonnes C ha^-1)</returns>
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
        /// Equation 2.2.2-25
        /// Equation 2.2.2-41
        /// Equation 2.7.3-6
        /// </summary>
        /// <param name="slowPoolAtPreviousInterval">Slow sub-pool SOC stock in previous year, (tonnes C ha^-1)</param>
        /// <param name="slowPoolSteadyState">Steady state slow sub-pool SOC stock given conditions in year y, (tonnes C ha^-1)</param>
        /// <param name="slowPoolDecayRate">Decay rate for slow SOC sub-pool, (year^-1)</param>
        /// <returns>Slow sub-pool SOC stock in y, (tonnes C ha^-1)</returns>
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
        /// Equation 2.2.2-26
        /// Equation 2.2.2-36
        /// Equation 2.7.3-7
        /// </summary>
        /// <param name="activePoolSteadyState">Steady state active sub-pool SOC stock given conditions in year y, (tonnes C ha^-1)</param>
        /// <param name="activePoolDecayRate">Decay rate for active carbon sub-pool in the soil, (year^-1)</param>
        /// <param name="f5">Fraction of active sub-pool decay products transferred to the slow sub-pool, (unitless)</param>
        /// <param name="slowPoolSteadyState">Steady state slow sub-pool SOC stock given conditions in year y, (tonnes C ha^-1)</param>
        /// <param name="slowPoolDecayRate">Decay rate for slow SOC sub-pool, (year^-1)</param>
        /// <param name="f6">Fraction of slow sub-pool decay products transferred to the passive sub-pool, (unitless)</param>
        /// <param name="passivePoolDecayRate">Decay rate for passive SOC sub-pool, (year^-1)(</param>
        /// <returns>Steady state passive sub-pool SOC given conditions in year y, (tonnes C ha^-1)</returns>
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
        /// Equation 2.2.2-27
        /// Equation 2.2.2-37
        /// Equation 2.7.3-8
        /// </summary>
        /// <param name="passivePoolAtPreviousInterval">Passive sub-pool SOC stock in previous year, (tonnes C ha^-1)</param>
        /// <param name="passivePoolSteadyState">Steady state passive sub-pool SOC given conditions in year y, (tonnes C ha^-1)</param>
        /// <param name="passivePoolDecayRate">Decay rate for passive SOC sub-pool, (year^-1)</param>
        /// <returns>Passive sub-pool SOC stock in year y, (tonnes C ha^-1)</returns>
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
        /// Equation 2.2.2-45
        /// Equation 2.7.3-14
        /// </summary>
        /// <param name="activePool">Active sub-pool SOC stock in year y for grid cell or region, (tonnes C ha^-1)</param>
        /// <param name="passivePool">Passive sub-pool SOC stock in year y for grid cell or region, (tonnes C ha^-1)</param>
        /// <param name="slowPool">Slow sub-pool SOC stock in year y for grid cell or region, (tonnes C ha^-1)</param>
        /// <returns>SOC stock at the end of the current year y for grid cell or region , (tonnes C ha^-1)</returns>
        public double CalculateTotalStocks(
            double activePool,
            double passivePool,
            double slowPool)
        {
            return activePool + slowPool + passivePool;
        }

        /// <summary>
        /// Equation 2.2.2.46
        /// Equation 2.7.3-15
        /// </summary>
        /// <param name="socAtYear">SOC stock at the end of the current year y for grid cell or region, (tonnes C ha^-1)</param>
        /// <param name="socAtPreviousYear">SOC stock at the end of the previous year for grid cell or region, (tonnes C ha^-1)</param>
        /// <returns>Annual stock change factor for mineral soils in grid cell or region i, (tonnes C ha^-1)</returns>
        public double CalculateStockChange(
            double socAtYear,
            double socAtPreviousYear)
        {
            return socAtYear - socAtPreviousYear;
        }

        #endregion
    }
}