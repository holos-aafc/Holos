using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using H.Core.Calculators.Carbon;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Soil;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using H.Core.Test.Calculators.Carbon;
using H.Core.Providers.Climate;
using H.Core.Providers;
using H.Core.Calculators.Nitrogen;
using H.Core.Services.LandManagement;

namespace H.Core.Test.Calculators.Carbon
{
    /// <summary>
    /// Uses the supplied Tier 2 carbon modelling Excel workbook to ensure calculations are being made correctly
    /// </summary>
    [TestClass]
    public  class IPCCTier2SoilCarbonCalculatorTest :UnitTestBase
    {
        #region Inner Classes

        public class TestClimateData
        {
            public int Month { get; set; }
            public int Year { get; set; }
            public double AverageTemperature { get; set; }
            public double Precipitation { get; set; }
            public double Evapotranspiration { get; set; }

            // Calculated values from sample data file used for comparison
            public double TemperatureEffectOnDecompositionForMonth { get; set; }
            public double MappetForMonth { get; set; }
            public double WaterEffectOnDecompositionForMonth { get; set; }
        }

        public class TestFactorData
        {
            public int Year { get; set; }
            public double TemperatureEffectForYear { get; set; }
            public double WaterEffectForYear { get; set; }
        }

        #endregion

        #region Fields

        private IPCCTier2SoilCarbonCalculator _sut;

        private static List<TestClimateData> _monthlyTestData;
        private static List<TestFactorData> _annualTestData;
        private static List<CropViewItem> _annualCarbonData;

        private static  ClimateData climateData;

        private Table_8_Globally_Calibrated_Model_Parameters_Provider _globallyCalibratedModelParametersProvider;
        private static TillageType _tillageType;

        private double _f1;
        private double _f2;
        private double _f3;
        private double _f5;
        private double _f6;
        private double _f7;
        private double _f8;
        private double _activePoolDecayRateConstant;
        private double _slowPoolDecayRateConstant;
        private double _passivePoolDecayRateConstant;
        private double _tillageFactor;
        private static CropViewItem _averageRunInValuesAndInitialStocks;

        private const double Latitude = 49.6;
        private const double Longitude = 112.8;

        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            const double latitude = 49.6;
            const double longitude = 112.8;

            climateData = _climateProvider.Get(
                    latitude: latitude,
                    longitude: longitude,
                    climateNormalTimeFrame: TimeFrame.NineteenNinetyToTwoThousand);

            _monthlyTestData = new List<TestClimateData>();
            _annualTestData = new List<TestFactorData>();
            _annualCarbonData = new List<CropViewItem>();

            _tillageType = TillageType.Intensive;

            // Read in monthly test data
            var data = Resource1.Tier2TestData;
            var lines = data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines.Skip(1))
            {
                var dataRow = new TestClimateData();

                // Read in climate values
                var columns = line.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                dataRow.Year = int.Parse(columns[0]);
                dataRow.Month = int.Parse(columns[1]);
                dataRow.AverageTemperature = double.Parse(columns[3]);
                dataRow.Precipitation = double.Parse(columns[4]);
                dataRow.Evapotranspiration = double.Parse(columns[5]);
                dataRow.TemperatureEffectOnDecompositionForMonth = double.Parse(columns[6]);
                dataRow.MappetForMonth = double.Parse(columns[7]);
                dataRow.WaterEffectOnDecompositionForMonth = double.Parse(columns[8]);

                _monthlyTestData.Add(dataRow);
            }

            // Read in annual test data
            for (int i = 1; i < 69; i++)
            {
                var annualTestData = new TestFactorData();

                var line = lines[i];
                var columns = line.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                annualTestData.Year = int.Parse(columns[12]);
                annualTestData.TemperatureEffectForYear = double.Parse(columns[13]);
                annualTestData.WaterEffectForYear = double.Parse(columns[14]);

                _annualTestData.Add(annualTestData);
            }

            // Read in soc test data
            data = Resource1.outputs;
            lines = data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines.Skip(1))
            {
                var carbonTestData = new CropViewItem();
                var columns = line.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                carbonTestData.Year = int.Parse(columns[1]);
                carbonTestData.TFac = double.Parse(columns[2]);
                carbonTestData.WFac = double.Parse(columns[3]);
                carbonTestData.TotalCarbonInputs = double.Parse(columns[4]);
                carbonTestData.Sand = double.Parse(columns[5]);
                carbonTestData.LigninContent = double.Parse(columns[6]);
                carbonTestData.NitrogenContent = double.Parse(columns[7]);
                carbonTestData.IpccTier2CarbonResults.Beta = double.Parse(columns[8]);
                carbonTestData.IpccTier2CarbonResults.Alpha = double.Parse(columns[9]);

                carbonTestData.IpccTier2CarbonResults.ActivePoolDecayRate = double.Parse(columns[10]);
                carbonTestData.IpccTier2CarbonResults.ActivePoolSteadyState = double.Parse(columns[11]);
                carbonTestData.IpccTier2CarbonResults.ActivePool = double.Parse(columns[12]);
                carbonTestData.IpccTier2CarbonResults.ActivePoolDiff = double.Parse(columns[13]);

                carbonTestData.IpccTier2CarbonResults.SlowPoolDecayRate = double.Parse(columns[14]);
                carbonTestData.IpccTier2CarbonResults.SlowPoolSteadyState = double.Parse(columns[15]);
                carbonTestData.IpccTier2CarbonResults.SlowPool = double.Parse(columns[16]);
                carbonTestData.IpccTier2CarbonResults.SlowPoolDiff = double.Parse(columns[17]);

                carbonTestData.IpccTier2CarbonResults.PassivePoolDecayRate = double.Parse(columns[18]);
                carbonTestData.IpccTier2CarbonResults.PassivePoolSteadyState = double.Parse(columns[19]);
                carbonTestData.IpccTier2CarbonResults.PassivePool = double.Parse(columns[20]);
                carbonTestData.IpccTier2CarbonResults.PassivePoolDiff = double.Parse(columns[21]);

                carbonTestData.Soc = double.Parse(columns[22]);
                carbonTestData.SocDiff = double.Parse(columns[23]);

                _annualCarbonData.Add(carbonTestData);
            }

            // These are the starting values using for t-1 when starting the simulation at t
            _averageRunInValuesAndInitialStocks = new CropViewItem()
            {
            };

            _averageRunInValuesAndInitialStocks.IpccTier2CarbonResults.ActivePool = 0.34;
            _averageRunInValuesAndInitialStocks.IpccTier2CarbonResults.PassivePool = 58.80;
            _averageRunInValuesAndInitialStocks.IpccTier2CarbonResults.SlowPool = 3.6;
            _averageRunInValuesAndInitialStocks.Soc = 62.75;

            
            var n2oEmissionFactorCalculator = new N2OEmissionFactorCalculator(_climateProvider);
            
            var ipcc = new IPCCTier2SoilCarbonCalculator(_climateProvider, n2oEmissionFactorCalculator);

            _sut = ipcc;
            _globallyCalibratedModelParametersProvider = new Table_8_Globally_Calibrated_Model_Parameters_Provider();

            _f1 = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.FractionMetabolicDMActivePool, _tillageType).Value;
            _f2 = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.FractionStructuralDMActivePool, _tillageType).Value;
            _f3 = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.FractionStructuralDMSlowPool, _tillageType).Value;
            _f5 = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.FractionActiveDecayToPassive, _tillageType).Value;
            _f6 = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.FractionSlowDecayToPassive, _tillageType).Value;
            _f7 = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.FractionSlowDecayToActive, _tillageType).Value;
            _f8 = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.FractionPassiveDecayToActive, _tillageType).Value;

            _activePoolDecayRateConstant = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.DecayRateActive, _tillageType).Value;
            _slowPoolDecayRateConstant = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.DecayRateSlow, _tillageType).Value;
            _passivePoolDecayRateConstant = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.DecayRatePassive, _tillageType).Value;

            _tillageFactor = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(ModelParameters.TillageModifier, _tillageType).Value;
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests      



        [TestMethod]
        public void CalculateMonthlyTemperatureFactor()
        {
            var distinctYears = _monthlyTestData.Select(x => x.Year).Distinct();
            foreach (var distinctYear in distinctYears)
            {
                var optimumTemperature = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(
                    parameter: ModelParameters.OptimumTemperature,
                    tillageType: _tillageType).Value;

                var maximumTemperature = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(
                    parameter: ModelParameters.MaximumAvgTemperature,
                    tillageType: _tillageType).Value;

                var monthsForYear = _monthlyTestData.Where(x => x.Year == distinctYear).OrderBy(y => y.Year).ToList();
                for (var index = 0; index < monthsForYear.Count; index++)
                {
                    var temperature = monthsForYear.ElementAt(index).AverageTemperature;

                    var calculated = _sut.CalculateMonthlyTemperatureEffectOnDecomposition(maximumTemperature, temperature, optimumTemperature);
                    var expected = monthsForYear.ElementAt(index).TemperatureEffectOnDecompositionForMonth;

                    Assert.AreEqual(expected, calculated, 0.003);
                }
            }
        }

        [TestMethod]
        public void CalculateWaterEffectForMonth()
        {
            var distinctYears = _monthlyTestData.Select(x => x.Year).Distinct();
            foreach (var distinctYear in distinctYears)
            {
                var monthsForYear = _monthlyTestData.Where(x => x.Year == distinctYear).OrderBy(y => y.Year).ToList();
                for (var index = 0; index < monthsForYear.Count; index++)
                {
                    var precipitationForMonth = monthsForYear.ElementAt(index).Precipitation;
                    var evapotranspiration = monthsForYear.ElementAt(index).Evapotranspiration;

                    var slopeParameter = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(
                        parameter: ModelParameters.SlopeParameter,
                        tillageType: TillageType.Intensive).Value;

                    var calculated = _sut.CalculateMonthlyWaterEffectOnDecomposition(
                        monthlyTotalPrecipitation: precipitationForMonth,
                        monthlyTotalEvapotranspiration: evapotranspiration,
                        slopeParameter: slopeParameter);

                    var expected = monthsForYear.ElementAt(index).WaterEffectOnDecompositionForMonth;

                    Assert.AreEqual(expected, calculated, 0.001);
                }
            }
        }

        [TestMethod]
        public void CalculateAnnualTemperatureFactor()
        {
            foreach (var distinctYear in _annualTestData)
            {
                var monthsForYear = _monthlyTestData.Where(x => x.Year == distinctYear.Year).OrderBy(y => y.Year).ToList();

                var maximumTemperatureForDecomposition = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(
                    parameter: ModelParameters.MaximumAvgTemperature,
                    tillageType: TillageType.Intensive).Value;

                var optimumTemperatureForDecomposition = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(
                    parameter: ModelParameters.OptimumTemperature,
                    tillageType: TillageType.Intensive).Value;

                var calculated = _sut.CalculateAverageAnnualTemperatureFactor(monthsForYear.Select(x => x.AverageTemperature).ToList(), maximumTemperatureForDecomposition, optimumTemperatureForDecomposition);
                var expected = distinctYear.TemperatureEffectForYear;

                Assert.AreEqual(expected, calculated, 0.01);
            }
        }

        [TestMethod]
        public void CalculateAnnualWaterFactor()
        {
            foreach (var distinctYear in _annualTestData)
            {
                var monthsForYear = _monthlyTestData.Where(x => x.Year == distinctYear.Year).OrderBy(y => y.Year).ToList();


                var slopeParameter = _globallyCalibratedModelParametersProvider.GetGloballyCalibratedModelParametersInstance(
                    parameter: ModelParameters.SlopeParameter,
                    tillageType: TillageType.Intensive).Value;

                var calculated = _sut.CalculateAnnualWaterFactor(monthsForYear.Select(x => x.Precipitation).ToList(), monthsForYear.Select(x => x.Evapotranspiration).ToList(), slopeParameter);
                var expected = distinctYear.WaterEffectForYear;

                Assert.AreEqual(expected, calculated, 0.01);
            }
        }

        [TestMethod]
        public void CalculateBeta()
        {
            foreach (var year in _annualCarbonData)
            {
                var calculatedValue = _sut.CalculateAmountToDeadMatterComponent(
                    totalInputs: year.TotalCarbonInputs,
                    nitrogenFraction: year.NitrogenContent,
                    ligninContent: year.LigninContent);

                Assert.AreEqual(_annualCarbonData.Single(x => x.Year == year.Year).IpccTier2CarbonResults.Beta, calculatedValue, 0.01);
            }
        }

        [TestMethod]
        public void CalculateAlpha()
        {
            foreach (var year in _annualCarbonData)
            {
                var beta = _sut.CalculateAmountToDeadMatterComponent(
                    totalInputs: year.TotalCarbonInputs,
                    nitrogenFraction: year.NitrogenContent,
                    ligninContent: year.LigninContent);

                var f4 = _sut.CalculateAmountToSlowPool(
                    fractionDecayActivePoolToPassivePool: _f5,
                    sand: year.Sand);

                var calculatedValue = _sut.CalculateAmountToActivePool(
                    inputToDeadMatter: beta,
                    f1: _f1,
                    f2: _f2,
                    f3: _f3,
                    f4: f4,
                    f5: _f5,
                    f6: _f6,
                    f7: _f7,
                    f8: _f8,
                    totalInputs: year.TotalCarbonInputs,
                    ligninContent: year.LigninContent);

                Assert.AreEqual(year.IpccTier2CarbonResults.Alpha, calculatedValue, 0.1);
            }
        }

        [TestMethod]
        public void CalculateDecayRateForActiveSubPool()
        {
            foreach (var year in _annualCarbonData)
            {
                var calculateValue = _sut.CalculateActivePoolDecayRate(
                    activePoolDecayRateConstant: _activePoolDecayRateConstant,
                    temperatureFactor: year.TFac,
                    waterFactor: year.WFac,
                    sand: year.Sand,
                    tillageFactor: _tillageFactor);

                Assert.AreEqual(year.IpccTier2CarbonResults.ActivePoolDecayRate, calculateValue, 0.015);
            }
        }

        [TestMethod]
        public void CalculateSteadyStateForActiveSubPool()
        {
            foreach (var year in _annualCarbonData)
            {
                var decayRate = _sut.CalculateActivePoolDecayRate(
                  activePoolDecayRateConstant: _activePoolDecayRateConstant,
                  temperatureFactor: year.TFac,
                  waterFactor: year.WFac,
                  sand: year.Sand,
                  tillageFactor: _tillageFactor);

                var calculateValue = _sut.CalculateSteadyStateActivePool(
                    inputsToActiveSubPool: year.IpccTier2CarbonResults.Alpha,
                    decayRateForActivePool: decayRate);

                Assert.AreEqual(year.IpccTier2CarbonResults.ActivePoolSteadyState, calculateValue, 0.01);
            }
        }

        [TestMethod]
        public void CalculateActiveSubPool()
        {
            var calclatedResults = new List<double>();

            for (int i = 0; i < _annualCarbonData.Count; i++)
            {
                var carbonDataAtYear = _annualCarbonData.ElementAt(i);
                var previousYearResults = i == 0 ? _averageRunInValuesAndInitialStocks.IpccTier2CarbonResults.ActivePool : calclatedResults.ElementAt(i - 1);

                var calculateValue = _sut.CalculateActivePoolAtCurrentInterval(
                    activePoolAtPreviousInterval: previousYearResults,
                    activePoolSteadyState: carbonDataAtYear.IpccTier2CarbonResults.ActivePoolSteadyState,
                    decayRateForActivePool: carbonDataAtYear.IpccTier2CarbonResults.ActivePoolDecayRate);

                calclatedResults.Add(calculateValue);

                Assert.AreEqual(carbonDataAtYear.IpccTier2CarbonResults.ActivePool, calculateValue);
            }
        }

        [TestMethod]
        public void CalculateActiveSubPoolDiff()
        {
            var calclatedResults = new List<double>();

            for (int i = 0; i < _annualCarbonData.Count; i++)
            {
                var carbonDataAtYear = _annualCarbonData.ElementAt(i);
                var previousYearResults = i == 0 ? _averageRunInValuesAndInitialStocks.IpccTier2CarbonResults.ActivePool : calclatedResults.ElementAt(i - 1);

                var calculateValue = _sut.CalculateActivePoolAtCurrentInterval(
                    activePoolAtPreviousInterval: previousYearResults,
                    activePoolSteadyState: carbonDataAtYear.IpccTier2CarbonResults.ActivePoolSteadyState,
                    decayRateForActivePool: carbonDataAtYear.IpccTier2CarbonResults.ActivePoolDecayRate);

                calclatedResults.Add(calculateValue);

                var diff = calculateValue - previousYearResults;

                Assert.AreEqual(carbonDataAtYear.IpccTier2CarbonResults.ActivePoolDiff, diff, 0.015);
            }
        }

        [TestMethod]
        public void CalculateDecayRateForSlowSubPool()
        {
            foreach (var year in _annualCarbonData)
            {
                var calculateValue = _sut.CalculateSlowPoolDecayRate(
                    slowPoolDecayRateConstant: _slowPoolDecayRateConstant,
                    temperatureFactor: year.TFac,
                    waterFactor: year.WFac,
                    tillageFactor: _tillageFactor);

                Assert.AreEqual(year.IpccTier2CarbonResults.SlowPoolDecayRate, calculateValue, 0.001);
            }
        }

        [TestMethod]
        public void CalculateSteadyStateForSlowSubPool()
        {
            foreach (var year in _annualCarbonData)
            {               
                var f4 = _sut.CalculateAmountToSlowPool(
                    fractionDecayActivePoolToPassivePool: _f5,
                    sand: year.Sand);

                var calculateValue = _sut.CalculateSteadyStateSlowPool(
                    totalInputs: year.TotalCarbonInputs,
                    ligninContent: year.LigninContent,
                    f3: _f3,
                    steadyStateActivePool: year.IpccTier2CarbonResults.ActivePoolSteadyState,
                    activePoolDecayRate: year.IpccTier2CarbonResults.ActivePoolDecayRate,
                    f4: f4,
                    decayRateSlowPool: year.IpccTier2CarbonResults.SlowPoolDecayRate);

                Assert.AreEqual(year.IpccTier2CarbonResults.SlowPoolSteadyState, calculateValue, 0.1);
            }
        }

        [TestMethod]
        public void CalculateSlowSubPool()
        {
            var calclatedResults = new List<double>();

            for (int i = 0; i < _annualCarbonData.Count; i++)
            {
                var carbonDataAtYear = _annualCarbonData.ElementAt(i);
                var previousYearResults = i == 0 ? _averageRunInValuesAndInitialStocks.IpccTier2CarbonResults.SlowPool : calclatedResults.ElementAt(i - 1);

                var calculateValue = _sut.CalculateSlowPoolAtInterval(
                    slowPoolAtPreviousInterval: previousYearResults,
                    slowPoolSteadyState: carbonDataAtYear.IpccTier2CarbonResults.SlowPoolSteadyState,
                    slowPoolDecayRate: carbonDataAtYear.IpccTier2CarbonResults.SlowPoolDecayRate);

                calclatedResults.Add(calculateValue);

                Assert.AreEqual(carbonDataAtYear.IpccTier2CarbonResults.SlowPool, calculateValue, 0.2);
            }
        }

        [TestMethod]
        public void CalculateSlowSubPoolDiff()
        {
            var calclatedResults = new List<double>();

            for (int i = 0; i < _annualCarbonData.Count; i++)
            {
                var carbonDataAtYear = _annualCarbonData.ElementAt(i);
                var previousYearResults = i == 0 ? _averageRunInValuesAndInitialStocks.IpccTier2CarbonResults.SlowPool : calclatedResults.ElementAt(i - 1);

                var calculateValue = _sut.CalculateSlowPoolAtInterval(
                    slowPoolAtPreviousInterval: previousYearResults,
                    slowPoolSteadyState: carbonDataAtYear.IpccTier2CarbonResults.SlowPoolSteadyState,
                    slowPoolDecayRate: carbonDataAtYear.IpccTier2CarbonResults.SlowPoolDecayRate);

                calclatedResults.Add(calculateValue);

                var diff = calculateValue - previousYearResults;

                Assert.AreEqual(carbonDataAtYear.IpccTier2CarbonResults.SlowPoolDiff, diff, 0.01);
            }
        }

        [TestMethod]
        public void CalculateDecayRateForPassiveSubPool()
        {
            foreach (var year in _annualCarbonData)
            {
                var calculateValue = _sut.CalculatePassivePoolDecayRate(
                    passivePoolDecayRateConstant: _passivePoolDecayRateConstant,
                    temperatureFactor: year.TFac,
                    waterFactor: year.WFac);

                Assert.AreEqual(year.IpccTier2CarbonResults.PassivePoolDecayRate, calculateValue, 0.0001);
            }
        }

        [TestMethod]
        public void CalculateSteadyStateForPassiveSubPool()
        {
            foreach (var year in _annualCarbonData)
            {
                var calculateValue = _sut.CalculatePassivePoolSteadyState(
                    activePoolSteadyState: year.IpccTier2CarbonResults.ActivePoolSteadyState,
                    activePoolDecayRate: year.IpccTier2CarbonResults.ActivePoolDecayRate,
                    f5: _f5,
                    slowPoolSteadyState: year.IpccTier2CarbonResults.SlowPoolSteadyState,
                    slowPoolDecayRate: year.IpccTier2CarbonResults.SlowPoolDecayRate,
                    f6: _f6,
                    passivePoolDecayRate: year.IpccTier2CarbonResults.PassivePoolDecayRate);

                Assert.AreEqual(year.IpccTier2CarbonResults.PassivePoolSteadyState, calculateValue, 1.5);
            }
        }

        [TestMethod]
        public void CalculatePassiveSubPool()
        {
            var calclatedResults = new List<double>();

            for (int i = 0; i < _annualCarbonData.Count; i++)
            {
                var carbonDataAtYear = _annualCarbonData.ElementAt(i);
                var previousYearResults = i == 0 ? _averageRunInValuesAndInitialStocks.IpccTier2CarbonResults.PassivePool : calclatedResults.ElementAt(i - 1);

                var calculateValue = _sut.CalculatePassivePoolAtInterval(
                    passivePoolAtPreviousInterval: previousYearResults,
                    passivePoolSteadyState: carbonDataAtYear.IpccTier2CarbonResults.PassivePoolSteadyState,
                    passivePoolDecayRate: carbonDataAtYear.IpccTier2CarbonResults.PassivePoolDecayRate);

                calclatedResults.Add(calculateValue);

                Assert.AreEqual(carbonDataAtYear.IpccTier2CarbonResults.PassivePool, calculateValue, 0.01);
            }
        }

        [TestMethod]
        public void CalculatePassiveSubPoolDiff()
        {
            var calclatedResults = new List<double>();

            for (int i = 0; i < _annualCarbonData.Count; i++)
            {
                var carbonDataAtYear = _annualCarbonData.ElementAt(i);
                var previousYearResults = i == 0 ? _averageRunInValuesAndInitialStocks.IpccTier2CarbonResults.PassivePool : calclatedResults.ElementAt(i - 1);

                var calculateValue = _sut.CalculatePassivePoolAtInterval(
                   passivePoolAtPreviousInterval: previousYearResults,
                   passivePoolSteadyState: carbonDataAtYear.IpccTier2CarbonResults.PassivePoolSteadyState,
                   passivePoolDecayRate: carbonDataAtYear.IpccTier2CarbonResults.PassivePoolDecayRate);

                calclatedResults.Add(calculateValue);

                var diff = calculateValue - previousYearResults;

                Assert.AreEqual(carbonDataAtYear.IpccTier2CarbonResults.PassivePoolDiff, diff, 0.01);
            }
        }

        [TestMethod]
        public void CalculateSoc()
        {            
            for (int i = 0; i < _annualCarbonData.Count; i++)
            {
                var carbonDataAtYear = _annualCarbonData.ElementAt(i);
                
                var calculatedValue = _sut.CalculateTotalStocks(
                    activePool: carbonDataAtYear.IpccTier2CarbonResults.ActivePool,
                    passivePool: carbonDataAtYear.IpccTier2CarbonResults.PassivePool,
                    slowPool: carbonDataAtYear.IpccTier2CarbonResults.SlowPool);                
               
                Assert.AreEqual(carbonDataAtYear.Soc, calculatedValue, 0.02);
            }
        }

        [TestMethod]
        public void CalculateSocDiff()
        {
            var calclatedResults = new List<double>();

            for (int i = 0; i < _annualCarbonData.Count; i++)
            {
                var carbonDataAtYear = _annualCarbonData.ElementAt(i);
                var previousYearResults = i == 0 ? _averageRunInValuesAndInitialStocks.Soc : calclatedResults.ElementAt(i - 1);
                
                var calculateValue = _sut.CalculateStockChange(
                    socAtYear: carbonDataAtYear.Soc,
                    socAtPreviousYear: previousYearResults);

                calclatedResults.Add(carbonDataAtYear.Soc);                

                Assert.AreEqual(carbonDataAtYear.SocDiff, calculateValue, 0.1);
            }
        }

        [TestMethod]
        public void AverageNonPoolInputsForRunInPeriod()
        {
            const int startYear = 1990;
            const double NitrogenContent = 0.021;
            const double LigninContent = 0.111;

            var soilData = new SoilData()
            {
                ProportionOfSandInSoil = 0.25,
            };

            var farm = new Farm()
            {
                Latitude = Latitude,
                Longitude = Longitude,
                ClimateData = climateData,
                GeographicData = new Core.Providers.GeographicData
                {
                    DefaultSoilData = soilData,
                }
            };

            var defaults = new Defaults();

            var fieldSystemComponent = new FieldSystemComponent()
            {
                CropViewItems = new System.Collections.ObjectModel.ObservableCollection<CropViewItem>()
                {                    
                }
            };

            // Set inputs for each view item
            var viewItems = new List<CropViewItem>()
            {                
                new CropViewItem() 
                {                    
                    Year = startYear,
                    TotalCarbonInputs = 3.569,
                    Sand = farm.DefaultSoilData.ProportionOfSandInSoil,
                    TFac = 0.359,
                    WFac = 0.628,
                    NitrogenContent = NitrogenContent,
                    LigninContent = LigninContent,
                },

                new CropViewItem() 
                {
                    Year = startYear + 1,
                    TotalCarbonInputs = 3.805,
                    TFac = 0.335,
                    WFac = 0.875,
                    Sand = farm.DefaultSoilData.ProportionOfSandInSoil,
                    NitrogenContent = NitrogenContent,
                    LigninContent = LigninContent,
                },

                new CropViewItem()
                {
                    Year = startYear + 2,
                    TotalCarbonInputs = 3.718,
                    TFac = 0.371,
                    WFac = 0.826,
                    Sand = farm.DefaultSoilData.ProportionOfSandInSoil,
                    NitrogenContent = NitrogenContent,
                    LigninContent = LigninContent,
                },

                new CropViewItem()
                {
                    Year = startYear + 3,
                    TotalCarbonInputs = 3.096,
                    TFac = 0.349,
                    WFac = 0.837,
                    Sand = farm.DefaultSoilData.ProportionOfSandInSoil,
                    NitrogenContent = NitrogenContent,
                    LigninContent = LigninContent,
                },

                new CropViewItem()
                {
                    Year = startYear + 4,
                    TotalCarbonInputs = 3.702,
                    TFac = 0.352,
                    WFac = 0.879,
                    Sand = farm.DefaultSoilData.ProportionOfSandInSoil,
                    NitrogenContent = NitrogenContent,
                    LigninContent = LigninContent,
                },

                new CropViewItem()
                {
                    Year = startYear + 5,
                    TotalCarbonInputs = 3.635,
                    TFac = 0.315,
                    WFac = 0.859,
                    Sand = farm.DefaultSoilData.ProportionOfSandInSoil,
                    NitrogenContent = NitrogenContent,
                    LigninContent = LigninContent,
                },

                new CropViewItem()
                {
                    Year = startYear + 6,
                    TotalCarbonInputs = 3.805,
                    TFac = 0.364,
                    WFac = 0.747,
                    Sand = farm.DefaultSoilData.ProportionOfSandInSoil,
                    NitrogenContent = NitrogenContent,
                    LigninContent = LigninContent,
                },

                new CropViewItem()
                {
                    Year = startYear + 7,
                    TotalCarbonInputs = 2.766,
                    TFac = 0.34,
                    WFac = 0.828,
                    Sand = farm.DefaultSoilData.ProportionOfSandInSoil,
                    NitrogenContent = NitrogenContent,
                    LigninContent = LigninContent,
                },

                new CropViewItem()
                {
                    Year = startYear + 8,
                    TotalCarbonInputs = 3.12,
                    TFac = 0.358,
                    WFac = 0.618,
                    Sand = farm.DefaultSoilData.ProportionOfSandInSoil,
                    NitrogenContent = NitrogenContent,
                    LigninContent = LigninContent,
                },

                new CropViewItem()
                {
                    Year = startYear + 9,
                    TotalCarbonInputs = 3.201,
                    TFac = 0.359,
                    WFac = 0.934,
                    Sand = farm.DefaultSoilData.ProportionOfSandInSoil,
                    NitrogenContent = NitrogenContent,
                    LigninContent = LigninContent,
                },

                new CropViewItem()
                {
                    Year = startYear + 10,
                    TotalCarbonInputs = 3.562,
                    TFac = 0.334,
                    WFac = 0.798,
                    Sand = farm.DefaultSoilData.ProportionOfSandInSoil,
                    NitrogenContent = NitrogenContent,
                    LigninContent = LigninContent,
                },
            };

            // Calculate averages of user inputs for the run in period
            var runInPeriodYear = _sut.AverageNonPoolInputsForRunInPeriod(viewItems);            

            /*
             * All comparison values are from original IPCC Tier 2 workbook (Time sequence sheet)
             */

            Assert.AreEqual(0.349, runInPeriodYear.TFac, 0.001);
            Assert.AreEqual(0.803, runInPeriodYear.WFac, 0.01);
            Assert.AreEqual(3.453, runInPeriodYear.TotalCarbonInputs, 0.1);
            Assert.AreEqual(0.25, runInPeriodYear.Sand, 0.1);
            Assert.AreEqual(0.111, runInPeriodYear.LigninContent, 0.01);
            Assert.AreEqual(0.021, runInPeriodYear.NitrogenContent, 0.01);

            // Calculate results (non-input values) of the run in period

            runInPeriodYear.TillageType = TillageType.Intensive;

            _sut.CalculatePools(
                runInPeriodYear,
                null,
                farm,
                true);            

            Assert.AreEqual(2.606, runInPeriodYear.IpccTier2CarbonResults.Beta, 0.001);
            Assert.AreEqual(1.74, runInPeriodYear.IpccTier2CarbonResults.Alpha, 0.01);

            Assert.AreEqual(2.751, runInPeriodYear.IpccTier2CarbonResults.ActivePoolDecayRate, 0.01);
            Assert.AreEqual(0.63, runInPeriodYear.IpccTier2CarbonResults.ActivePoolSteadyState, 0.01);
            Assert.AreEqual(0.63, runInPeriodYear.IpccTier2CarbonResults.ActivePool, 0.01);

            Assert.AreEqual(0.178, runInPeriodYear.IpccTier2CarbonResults.SlowPoolDecayRate, 0.01);
            Assert.AreEqual(6.62, runInPeriodYear.IpccTier2CarbonResults.SlowPoolSteadyState, 0.01);
            Assert.AreEqual(6.62, runInPeriodYear.IpccTier2CarbonResults.SlowPool, 0.01);

            Assert.AreEqual(0.0019, runInPeriodYear.IpccTier2CarbonResults.PassivePoolDecayRate, 0.0001);
            Assert.AreEqual(108, runInPeriodYear.IpccTier2CarbonResults.PassivePoolSteadyState, 1);
            Assert.AreEqual(108, runInPeriodYear.IpccTier2CarbonResults.PassivePool, 1);
        }

        [TestMethod]
        public void CalculateInputs()
        {
            var viewItem = new CropViewItem()
            {
                CropType = CropType.Barley,
                Yield = 7018,
                PercentageOfStrawReturnedToSoil = 100,
                MoistureContentOfCrop = 0.12,
                BiomassCoefficientStraw = 0.2,
                BiomassCoefficientProduct = 0.6,
            };

            _sut.CalculateInputs(
                viewItem: viewItem, farm: new Farm());

            Assert.AreEqual(2157.45888556357, viewItem.AboveGroundCarbonInput, 1);
            Assert.AreEqual(997.775453968351, viewItem.BelowGroundCarbonInput, 1);
            Assert.AreEqual(3155.23433953193, viewItem.TotalCarbonInputs, 1);
        }

        [TestMethod]
        public void CalculateResults()
        {
            var farm = new Farm()
            {
                Defaults = new Defaults()
                {
                    CarbonModellingStrategy = CarbonModellingStrategies.IPCCTier2,
                    DefaultRunInPeriod = 2,
                },

                GeographicData = new Core.Providers.GeographicData()
                {
                    DefaultSoilData = new SoilData()
                    {
                        ProportionOfSandInSoil = 0.25,
                    }
                }
            };
            
            var viewItems = new List<CropViewItem>()
            {                
                new CropViewItem() { Year = 1985, CropType = CropType.Barley, Yield = 5, LigninContent = 0.1, NitrogenContent = 0.3 , AboveGroundResidueDryMatter = 20 , NitrogenContentInStraw = 0.8, NitrogenContentInRoots = 0.7, PercentageOfStrawReturnedToSoil = 100          ,      BiomassCoefficientStraw = 0.2,
                    BiomassCoefficientProduct = 0.6,},
                new CropViewItem() { Year = 1986, CropType = CropType.Barley, Yield = 2, LigninContent = 0.1, NitrogenContent = 0.3 , AboveGroundNitrogenResidueForCrop = 20 , NitrogenContentInStraw = 0.8, NitrogenContentInRoots = 0.7, PercentageOfStrawReturnedToSoil = 100,                BiomassCoefficientStraw = 0.2,
                    BiomassCoefficientProduct = 0.6,},
                new CropViewItem() { Year = 1987, CropType = CropType.Barley, Yield = 3, LigninContent = 0.1, NitrogenContent = 0.3, AboveGroundResidueDryMatter = 20 , NitrogenContentInStraw = 0.8, NitrogenContentInRoots = 0.7, PercentageOfStrawReturnedToSoil = 100,                BiomassCoefficientStraw = 0.2,
                    BiomassCoefficientProduct = 0.6,},
            };

            var fieldSystemComponent = new FieldSystemComponent();

            foreach (var viewItem in viewItems)
            {
                _sut.CalculateInputs(viewItem, new Farm());
            }

            _sut.CropResiduePool = 100;

            _sut.CalculateResults(
                farm: farm, 
                viewItemsByField: viewItems, 
                fieldSystemComponent: fieldSystemComponent, 
                runInPeriodItems: new List<CropViewItem>() {new CropViewItem()});

            // The non-run in period items (only 1 in this case) should have calculate values for all pools
            var firstItemInSimulation = viewItems.Single(x => x.Year == 1987);

            Assert.IsTrue(firstItemInSimulation.IpccTier2CarbonResults.ActivePoolDecayRate > 0);
            Assert.IsTrue(firstItemInSimulation.IpccTier2CarbonResults.ActivePoolSteadyState > 0);
            Assert.IsTrue(firstItemInSimulation.IpccTier2CarbonResults.ActivePool > 0);
            Assert.IsTrue(firstItemInSimulation.IpccTier2CarbonResults.ActivePoolDiff != 0);

            Assert.IsTrue(firstItemInSimulation.IpccTier2CarbonResults.PassivePoolDecayRate > 0);
            Assert.IsTrue(firstItemInSimulation.IpccTier2CarbonResults.PassivePoolSteadyState > 0);
            Assert.IsTrue(firstItemInSimulation.IpccTier2CarbonResults.PassivePool > 0);
            Assert.IsTrue(firstItemInSimulation.IpccTier2CarbonResults.PassivePoolDiff != 0);

            Assert.IsTrue(firstItemInSimulation.IpccTier2CarbonResults.SlowPoolDecayRate > 0);
            Assert.IsTrue(firstItemInSimulation.IpccTier2CarbonResults.SlowPoolSteadyState > 0);
            Assert.IsTrue(firstItemInSimulation.IpccTier2CarbonResults.SlowPool > 0);
            Assert.IsTrue(firstItemInSimulation.IpccTier2CarbonResults.SlowPoolDiff != 0);
        }

        [TestMethod]
        public void CanCalculateInputsForCropReturnsTrue()
        {
            var result = _sut.CanCalculateInputsForCrop(new CropViewItem() { CropType = CropType.Barley });

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanCalculateInputsForCropReturnsFalse()
        {
            var result = _sut.CanCalculateInputsForCrop(new CropViewItem() { CropType = CropType.TameMixed });

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UseCustomStartingPoint()
        {
            var runInPeriodItems = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Wheat, TotalCarbonInputs = 1000,},
                new CropViewItem() {CropType = CropType.Oats, TotalCarbonInputs = 5000,},
                new CropViewItem() {CropType = CropType.Barley, TotalCarbonInputs = 200,}
            };

            var farm = new Farm()
            {
                StartingSoilOrganicCarbon = 30000,
                UseCustomStartingSoilOrganicCarbon = true,
            };

            // Run spin up method once and get the starting points for each pool
            var equilibriumYear = _sut.CalculateRunInPeriod(farm, runInPeriodItems);

            // Total SOC for starting point
            var soc = equilibriumYear.SoilCarbon;

            // Get ratio of each pool to starting point

            // Active pool fraction
            var activePool = equilibriumYear.IpccTier2CarbonResults.ActivePool;
            var activePoolFraction = activePool / soc;

            // Passive pool fraction
            var passivePool = equilibriumYear.IpccTier2CarbonResults.PassivePool;
            var passivePoolFraction = passivePool / soc;

            // Slow pool fraction
            var slowPool = equilibriumYear.IpccTier2CarbonResults.SlowPool;
            var slowPoolFraction = slowPool / soc;

            Assert.AreEqual(1.0, slowPoolFraction + passivePoolFraction + activePoolFraction);

            // Use these fraction to create starting pool values
            var customStartingActivePool = activePoolFraction * farm.StartingSoilOrganicCarbon;
            var customStartingPassivePool = passivePoolFraction * farm.StartingSoilOrganicCarbon;
            var customStartingSlowPool = slowPoolFraction * farm.StartingSoilOrganicCarbon;

            Assert.AreEqual(farm.StartingSoilOrganicCarbon, customStartingActivePool + customStartingPassivePool + customStartingSlowPool);

            // Inject these starting points into the first year item

            var firstYear = new CropViewItem();

            // Begin non-run in period simulation
            _sut.CalculatePools(firstYear,equilibriumYear, farm);

            var viewItemsByField = new List<CropViewItem>()
            {
                new CropViewItem() {Year = 1985,},
                new CropViewItem() {Year = 1986},
            };

            var fieldSystemComponent = new FieldSystemComponent();

            _sut.CalculateResults(farm, viewItemsByField, fieldSystemComponent, new List<CropViewItem>() {new CropViewItem()});

            Assert.AreEqual(viewItemsByField.First().SoilCarbon, farm.StartingSoilOrganicCarbon);
        }

        [TestMethod]
        public void GetMonthlyTemperatureEffectTestReturnsZeroWhenTemperatureIsAboveMaximum()
        {
            var monthlyTemperatures = new List<double>() {12, 4, 52};

            var result = _sut.CalculateMonthlyTemperatureEffectOnDecomposition(
                maximumMonthlyTemperatureForDecomposition: 30,
                monthlyTemperature: 52,
                optimumTemperatureForDecomposition: 12);

                Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetMonthlyTemperatureEffectTest()
        {
            var result = _sut.CalculateMonthlyTemperatureEffectOnDecomposition(45, 22, 10);

            Assert.AreEqual(0.9673, result, 0.0001);
        }

        [TestMethod]
        public void CalculateMonthlyWaterEffect()
        {
            var precipitation = 1;
            var evapotranspiration = 11;

            var result = _sut.CalculateMonthlyWaterEffect(precipitation, evapotranspiration, 3);

            Assert.AreEqual(0.4836, result, 0.0001);
        }

        [TestMethod]
        public void SetMonthlyWaterFactorsTest()
        {
            var precipitations = new List<double>() { 3, 4, 1 };
            var evapotranspirations = new List<double>() { 19, 22, 11 };
            var viewItem = new CropViewItem();

            _sut.SetMonthlyWaterFactors(
                monthlyTotalPrecipitations: precipitations,
                monthlyTotalEvapotranspirations: evapotranspirations,
                slopeParameter: 2,
                viewItem);

            Assert.AreEqual(0.5226, viewItem.MonthlyIpccTier2WaterFactors.January, 0.0001);
            Assert.AreEqual(0.5685, viewItem.MonthlyIpccTier2WaterFactors.February, 0.0001);
            Assert.AreEqual(0.3927, viewItem.MonthlyIpccTier2WaterFactors.March, 0.0001);
        }


        [TestMethod]
        public void SetMonthlyTemperatureFactorsTest()
        {
            var averageTemperatures = new List<double>() { 22, 21, 10 };
            var viewItem = new CropViewItem();

            _sut.SetMonthlyTemperatureFactors(averageTemperatures, 30, 18, viewItem);

            Assert.AreEqual(0.9692, viewItem.MonthlyIpccTier2TemperatureFactors.January, 0.0001);
            Assert.AreEqual(0.9829, viewItem.MonthlyIpccTier2TemperatureFactors.February, 0.0001);
            Assert.AreEqual(0.8930, viewItem.MonthlyIpccTier2TemperatureFactors.March, 0.0001);
        }

        #endregion
    }
}