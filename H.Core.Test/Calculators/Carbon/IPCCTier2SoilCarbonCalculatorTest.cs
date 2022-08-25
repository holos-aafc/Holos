using System;
using System.Collections.Generic;
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

namespace H.Core.Test.Calculators.Carbon
{
    /// <summary>
    /// Uses the supplied Tier 2 carbon modelling Excel workbook to ensure calculations are being made correctly
    /// </summary>
    [TestClass]
    public  class IPCCTier2SoilCarbonCalculatorTest
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

        private static readonly ClimateProvider _climateProvider = new ClimateProvider();
        private static  ClimateData climateData;

        private Table_11_Globally_Calibrated_Model_Parameters_Provider _globallyCalibratedModelParametersProvider;
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
                carbonTestData.Beta = double.Parse(columns[8]);
                carbonTestData.Alpha = double.Parse(columns[9]);

                carbonTestData.ActivePoolDecayRate = double.Parse(columns[10]);
                carbonTestData.ActivePoolSteadyState = double.Parse(columns[11]);
                carbonTestData.ActivePool = double.Parse(columns[12]);
                carbonTestData.ActivePoolDiff = double.Parse(columns[13]);

                carbonTestData.SlowPoolDecayRate = double.Parse(columns[14]);
                carbonTestData.SlowPoolSteadyState = double.Parse(columns[15]);
                carbonTestData.SlowPool = double.Parse(columns[16]);
                carbonTestData.SlowPoolDiff = double.Parse(columns[17]);

                carbonTestData.PassivePoolDecayRate = double.Parse(columns[18]);
                carbonTestData.PassivePoolSteadyState = double.Parse(columns[19]);
                carbonTestData.PassivePool = double.Parse(columns[20]);
                carbonTestData.PassivePoolDiff = double.Parse(columns[21]);

                carbonTestData.Soc = double.Parse(columns[22]);
                carbonTestData.SocDiff = double.Parse(columns[23]);

                _annualCarbonData.Add(carbonTestData);
            }

            // These are the starting values using for t-1 when starting the simulation at t
            _averageRunInValuesAndInitialStocks = new CropViewItem()
            {
                ActivePool = 0.34,
                PassivePool = 58.80,
                SlowPool = 3.6,
                Soc = 62.75,
            };
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _sut = new IPCCTier2SoilCarbonCalculator();
            _globallyCalibratedModelParametersProvider = new Table_11_Globally_Calibrated_Model_Parameters_Provider();

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
                    totalCarbonInput: year.TotalCarbonInputs,
                    nitrogenFraction: year.NitrogenContent,
                    ligninContent: year.LigninContent);

                Assert.AreEqual(_annualCarbonData.Single(x => x.Year == year.Year).Beta, calculatedValue, 0.01);
            }
        }

        [TestMethod]
        public void CalculateAlpha()
        {
            foreach (var year in _annualCarbonData)
            {
                var beta = _sut.CalculateAmountToDeadMatterComponent(
                    totalCarbonInput: year.TotalCarbonInputs,
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
                    totalCarbon: year.TotalCarbonInputs,
                    ligninContent: year.LigninContent);

                Assert.AreEqual(year.Alpha, calculatedValue, 0.1);
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

                Assert.AreEqual(year.ActivePoolDecayRate, calculateValue, 0.015);
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
                    carbonInputToActiveSubPool: year.Alpha,
                    decayRateForActivePool: decayRate);

                Assert.AreEqual(year.ActivePoolSteadyState, calculateValue, 0.01);
            }
        }

        [TestMethod]
        public void CalculateActiveSubPool()
        {
            var calclatedResults = new List<double>();

            for (int i = 0; i < _annualCarbonData.Count; i++)
            {
                var carbonDataAtYear = _annualCarbonData.ElementAt(i);
                var previousYearResults = i == 0 ? _averageRunInValuesAndInitialStocks.ActivePool : calclatedResults.ElementAt(i - 1);

                var calculateValue = _sut.CalculateActivePoolAtCurrentInterval(
                    activePoolAtPreviousInterval: previousYearResults,
                    activePoolSteadyState: carbonDataAtYear.ActivePoolSteadyState,
                    decayRateForActivePool: carbonDataAtYear.ActivePoolDecayRate);

                calclatedResults.Add(calculateValue);

                Assert.AreEqual(carbonDataAtYear.ActivePool, calculateValue);
            }
        }

        [TestMethod]
        public void CalculateActiveSubPoolDiff()
        {
            var calclatedResults = new List<double>();

            for (int i = 0; i < _annualCarbonData.Count; i++)
            {
                var carbonDataAtYear = _annualCarbonData.ElementAt(i);
                var previousYearResults = i == 0 ? _averageRunInValuesAndInitialStocks.ActivePool : calclatedResults.ElementAt(i - 1);

                var calculateValue = _sut.CalculateActivePoolAtCurrentInterval(
                    activePoolAtPreviousInterval: previousYearResults,
                    activePoolSteadyState: carbonDataAtYear.ActivePoolSteadyState,
                    decayRateForActivePool: carbonDataAtYear.ActivePoolDecayRate);

                calclatedResults.Add(calculateValue);

                var diff = calculateValue - previousYearResults;

                Assert.AreEqual(carbonDataAtYear.ActivePoolDiff, diff, 0.015);
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

                Assert.AreEqual(year.SlowPoolDecayRate, calculateValue, 0.001);
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
                    carbonInput: year.TotalCarbonInputs,
                    ligninContent: year.LigninContent,
                    f3: _f3,
                    steadyStateActivePool: year.ActivePoolSteadyState,
                    activePoolDecayRate: year.ActivePoolDecayRate,
                    f4: f4,
                    decayRateSlowPool: year.SlowPoolDecayRate);

                Assert.AreEqual(year.SlowPoolSteadyState, calculateValue, 0.1);
            }
        }

        [TestMethod]
        public void CalculateSlowSubPool()
        {
            var calclatedResults = new List<double>();

            for (int i = 0; i < _annualCarbonData.Count; i++)
            {
                var carbonDataAtYear = _annualCarbonData.ElementAt(i);
                var previousYearResults = i == 0 ? _averageRunInValuesAndInitialStocks.SlowPool : calclatedResults.ElementAt(i - 1);

                var calculateValue = _sut.CalculateSlowPoolAtInterval(
                    slowPoolAtPreviousInterval: previousYearResults,
                    slowPoolSteadyState: carbonDataAtYear.SlowPoolSteadyState,
                    slowPoolDecayRate: carbonDataAtYear.SlowPoolDecayRate);

                calclatedResults.Add(calculateValue);

                Assert.AreEqual(carbonDataAtYear.SlowPool, calculateValue, 0.2);
            }
        }

        [TestMethod]
        public void CalculateSlowSubPoolDiff()
        {
            var calclatedResults = new List<double>();

            for (int i = 0; i < _annualCarbonData.Count; i++)
            {
                var carbonDataAtYear = _annualCarbonData.ElementAt(i);
                var previousYearResults = i == 0 ? _averageRunInValuesAndInitialStocks.SlowPool : calclatedResults.ElementAt(i - 1);

                var calculateValue = _sut.CalculateSlowPoolAtInterval(
                    slowPoolAtPreviousInterval: previousYearResults,
                    slowPoolSteadyState: carbonDataAtYear.SlowPoolSteadyState,
                    slowPoolDecayRate: carbonDataAtYear.SlowPoolDecayRate);

                calclatedResults.Add(calculateValue);

                var diff = calculateValue - previousYearResults;

                Assert.AreEqual(carbonDataAtYear.SlowPoolDiff, diff, 0.01);
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

                Assert.AreEqual(year.PassivePoolDecayRate, calculateValue, 0.0001);
            }
        }

        [TestMethod]
        public void CalculateSteadyStateForPassiveSubPool()
        {
            foreach (var year in _annualCarbonData)
            {
                var calculateValue = _sut.CalculatePassivePoolSteadyState(
                    activePoolSteadyState: year.ActivePoolSteadyState,
                    activePoolDecayRate: year.ActivePoolDecayRate,
                    f5: _f5,
                    slowPoolSteadyState: year.SlowPoolSteadyState,
                    slowPoolDecayRate: year.SlowPoolDecayRate,
                    f6: _f6,
                    passivePoolDecayRate: year.PassivePoolDecayRate);

                Assert.AreEqual(year.PassivePoolSteadyState, calculateValue, 1.5);
            }
        }

        [TestMethod]
        public void CalculatePassiveSubPool()
        {
            var calclatedResults = new List<double>();

            for (int i = 0; i < _annualCarbonData.Count; i++)
            {
                var carbonDataAtYear = _annualCarbonData.ElementAt(i);
                var previousYearResults = i == 0 ? _averageRunInValuesAndInitialStocks.PassivePool : calclatedResults.ElementAt(i - 1);

                var calculateValue = _sut.CalculatePassivePoolAtInterval(
                    passivePoolAtPreviousInterval: previousYearResults,
                    passivePoolSteadyState: carbonDataAtYear.PassivePoolSteadyState,
                    passivePoolDecayRate: carbonDataAtYear.PassivePoolDecayRate);

                calclatedResults.Add(calculateValue);

                Assert.AreEqual(carbonDataAtYear.PassivePool, calculateValue, 0.01);
            }
        }

        [TestMethod]
        public void CalculatePassiveSubPoolDiff()
        {
            var calclatedResults = new List<double>();

            for (int i = 0; i < _annualCarbonData.Count; i++)
            {
                var carbonDataAtYear = _annualCarbonData.ElementAt(i);
                var previousYearResults = i == 0 ? _averageRunInValuesAndInitialStocks.PassivePool : calclatedResults.ElementAt(i - 1);

                var calculateValue = _sut.CalculatePassivePoolAtInterval(
                   passivePoolAtPreviousInterval: previousYearResults,
                   passivePoolSteadyState: carbonDataAtYear.PassivePoolSteadyState,
                   passivePoolDecayRate: carbonDataAtYear.PassivePoolDecayRate);

                calclatedResults.Add(calculateValue);

                var diff = calculateValue - previousYearResults;

                Assert.AreEqual(carbonDataAtYear.PassivePoolDiff, diff, 0.01);
            }
        }

        [TestMethod]
        public void CalculateSoc()
        {            
            for (int i = 0; i < _annualCarbonData.Count; i++)
            {
                var carbonDataAtYear = _annualCarbonData.ElementAt(i);
                
                var calculatedValue = _sut.CalculateSoc(
                    activePool: carbonDataAtYear.ActivePool,
                    passivePool: carbonDataAtYear.PassivePool,
                    slowPool: carbonDataAtYear.SlowPool);                
               
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
                
                var calculateValue = _sut.CalculateSocChange(
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
                farm);            

            Assert.AreEqual(2.606, runInPeriodYear.Beta, 0.001);
            Assert.AreEqual(1.74, runInPeriodYear.Alpha, 0.01);

            Assert.AreEqual(2.751, runInPeriodYear.ActivePoolDecayRate, 0.01);
            Assert.AreEqual(0.63, runInPeriodYear.ActivePoolSteadyState, 0.01);
            Assert.AreEqual(0.63, runInPeriodYear.ActivePool, 0.01);

            Assert.AreEqual(0.178, runInPeriodYear.SlowPoolDecayRate, 0.01);
            Assert.AreEqual(6.62, runInPeriodYear.SlowPoolSteadyState, 0.01);
            Assert.AreEqual(6.62, runInPeriodYear.SlowPool, 0.01);

            Assert.AreEqual(0.0019, runInPeriodYear.PassivePoolDecayRate, 0.0001);
            Assert.AreEqual(108, runInPeriodYear.PassivePoolSteadyState, 1);
            Assert.AreEqual(108, runInPeriodYear.PassivePool, 1);
        }

        [TestMethod]
        public void CalculateInputs()
        {
            var viewItem = new CropViewItem()
            {
                CropType = CropType.Barley,
                Yield = 7018,
                MoistureContentOfCrop = 0.12,
            };

            _sut.CalculateInputs(
                viewItem: viewItem);

            Assert.AreEqual(4751, viewItem.AboveGroundCarbonInput, 1);
            Assert.AreEqual(997, viewItem.BelowGroundCarbonInput, 1);
            Assert.AreEqual(5749, viewItem.TotalCarbonInputs, 1);
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
                new CropViewItem() { Year = 1985, CropType = CropType.Barley, Yield = 5, LigninContent = 0.1, NitrogenContent = 0.3 },
                new CropViewItem() { Year = 1986, CropType = CropType.Barley, Yield = 2, LigninContent = 0.1, NitrogenContent = 0.3 },
                new CropViewItem() { Year = 1987, CropType = CropType.Barley, Yield = 3, LigninContent = 0.1, NitrogenContent = 0.3 },
            };

            var fieldSystemComponent = new FieldSystemComponent();

            foreach (var viewItem in viewItems)
            {
                _sut.CalculateInputs(viewItem);
            }

            fieldSystemComponent.RunInPeriodItems.Add(new CropViewItem() { Year = 1985, CropType = CropType.Barley, Yield = 5, LigninContent = 0.1, NitrogenContent = 0.3 });

            _sut.CalculateResults(
                farm: farm, 
                viewItemsByField: viewItems, 
                fieldSystemComponent: fieldSystemComponent);

            // The non-run in period items (only 1 in this case) should have calculate values for all pools
            var firstItemInSimulation = viewItems.Single(x => x.Year == 1987);

            Assert.IsTrue(firstItemInSimulation.ActivePoolDecayRate > 0);
            Assert.IsTrue(firstItemInSimulation.ActivePoolSteadyState > 0);
            Assert.IsTrue(firstItemInSimulation.ActivePool > 0);
            Assert.IsTrue(firstItemInSimulation.ActivePoolDiff != 0);

            Assert.IsTrue(firstItemInSimulation.PassivePoolDecayRate > 0);
            Assert.IsTrue(firstItemInSimulation.PassivePoolSteadyState > 0);
            Assert.IsTrue(firstItemInSimulation.PassivePool > 0);
            Assert.IsTrue(firstItemInSimulation.PassivePoolDiff != 0);

            Assert.IsTrue(firstItemInSimulation.SlowPoolDecayRate > 0);
            Assert.IsTrue(firstItemInSimulation.SlowPoolSteadyState > 0);
            Assert.IsTrue(firstItemInSimulation.SlowPool > 0);
            Assert.IsTrue(firstItemInSimulation.SlowPoolDiff != 0);
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

        #endregion
    }
}