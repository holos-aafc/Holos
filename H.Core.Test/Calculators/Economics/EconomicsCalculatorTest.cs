using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Calculators.Economics;
using H.Core.Enumerations;
using H.Core.Providers.Economics;
using System.Text;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using H.Core.Emissions.Results;
using H.Core.Models.Results;
using H.Core.Services;
using H.Core.Services.LandManagement;
using Moq;

namespace H.Core.Test.Calculators.Economics
{
    [TestClass]
    public class EconomicsCalculatorTest
    {
        private EconomicsCalculator _calculator;
        private Farm _barleyFarm;
        private Mock<IFieldResultsService> _mockFieldResultsService;
        private CropEconomicsProvider _econProvider;
        private const double MetricArea = 45;
        private const double ImperialArea = 111.1974;

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
            _mockFieldResultsService = new Mock<IFieldResultsService>();
            _econProvider = new CropEconomicsProvider();

            _barleyFarm = new Farm()
            {
                Name = "Farm #1",
                MeasurementSystemType = MeasurementSystemType.Metric,
            };
            _calculator = new EconomicsCalculator(_mockFieldResultsService.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Non-equation Tests


        [TestMethod]
        public void GetTotalProfitRevenueTest()
        {
            var resultViewItems = new List<EconomicsResultsViewItem>()
            {
                new EconomicsResultsViewItem()
                {
                    Profit = 490,
                    Revenues = 800,
                },
                new EconomicsResultsViewItem()
                {
                    Profit = 100,
                    Revenues = 400,
                },
            };
            var totalRevenues = _calculator.GetTotalRevenues(resultViewItems);
            var totalProfits = _calculator.GetTotalProfit(resultViewItems);

            Assert.AreEqual(590, totalProfits);
            Assert.AreEqual(1200, totalRevenues);
        }

        [TestMethod]
        public void ExportEconDataTest()
        {
            _mockFieldResultsService.Setup(service => service.CalculateResultsForFieldComponent(It.IsAny<Farm>())).Returns(new List<FieldComponentEmissionResults>()
            {
                new FieldComponentEmissionResults()
                {
                    FieldSystemComponent = new FieldSystemComponent()
                    {
                        CropViewItems = new ObservableCollection<CropViewItem>()
                        {
                            new CropViewItem()
                            {
                                CropType = CropType.Barley,
                                CropEconomicData = new CropEconomicData(),
                            }
                        }
                    },

                    HarvestViewItems = new ObservableCollection<EstimatesOfProductionResultsViewItem>()
                    {
                        new EstimatesOfProductionResultsViewItem(),
                    }
                }
            });

            #region Barley
            var barleyField = new FieldSystemComponent();
            barleyField.Name = "Field #1";
            var barley = new CropViewItem()
            {
                CropType = CropType.Barley,
                Yield = 100,
            };
            barleyField.CropViewItems.Add(barley);
            _barleyFarm.Components.Add(barleyField);
            #endregion

            var barleyMetricFile = "barleyMetricEcon.csv";
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var path = Path.Combine(localAppData, "Temp");

            var applicationData = new ApplicationData();
            applicationData.DisplayUnitStrings = new DisplayUnitStrings();
            applicationData.DisplayUnitStrings.SetStrings(MeasurementSystemType.Metric);

            var metricPath = Path.Combine(path, barleyMetricFile);
            _calculator.ExportEconomicsDataToFile(_barleyFarm, metricPath, true, applicationData);
            var metricLines = File.ReadLines(metricPath).ToList();
            //this should be 4: 1 line for the header, 1 line for the data row, 1 line empty, 1 line for the total revenue
            Assert.AreEqual(4, metricLines.Count);
        }

        [TestMethod]
        public void CalculateCropResultsTest()
        {
            var field1 = new FieldSystemComponent { Name = "Field #1", FieldArea = 100 };
            var cropViewItem = new CropViewItem() { CropType = CropType.Barley, };


            var field2 = new FieldSystemComponent { Name = "Field #2", FieldArea = 100 };
            var cropViewItem2 = new CropViewItem() { CropType = CropType.Barley, };

            var econData = _econProvider.Get(CropType.Barley, SoilFunctionalCategory.Brown, Province.Alberta);

            var noEconDataFarm = new Farm() { Name = "No Econ Farm", MeasurementSystemType = MeasurementSystemType.Metric };
            var noEconField = new FieldSystemComponent() { Name = "No econ field", FieldArea = 1 };
            var noEconViewItem = new CropViewItem() { CropType = CropType.Barley };


            cropViewItem.CropEconomicData = econData;
            cropViewItem2.CropEconomicData = econData;

            //for old farms that don't have econ data
            noEconViewItem.CropEconomicData = null;

            field1.CropViewItems.Add(cropViewItem);
            field2.CropViewItems.Add(cropViewItem2);
            noEconField.CropViewItems.Add(noEconViewItem);

            _barleyFarm.Components.Add(field1);
            _barleyFarm.Components.Add(field2);

            noEconDataFarm.Components.Add(noEconField);

            var fieldComponentEmissionResults = new List<FieldComponentEmissionResults>()
            {
                new FieldComponentEmissionResults()
                {
                    FieldSystemComponent = field1,

                    HarvestViewItems = new ObservableCollection<EstimatesOfProductionResultsViewItem>()
                    {
                        new EstimatesOfProductionResultsViewItem(),
                    },
                },
            };

            var noEconFieldComponentEmissionResults = new List<FieldComponentEmissionResults>()
            {
                new FieldComponentEmissionResults()
                {
                    FieldSystemComponent = noEconField,

                    HarvestViewItems = new ObservableCollection<EstimatesOfProductionResultsViewItem>()
                    {
                        new EstimatesOfProductionResultsViewItem(),
                    },
                },
            };


            _mockFieldResultsService.Setup(service => service.CalculateResultsForFieldComponent(It.IsAny<Farm>())).Returns(fieldComponentEmissionResults);

            var result = _calculator.CalculateCropResults(_mockFieldResultsService.Object, _barleyFarm);

            _mockFieldResultsService.Setup(service => service.CalculateResultsForFieldComponent(It.IsAny<Farm>())).Returns(noEconFieldComponentEmissionResults);
            //old farms with no econ data should have any info
            var noEconResult = _calculator.CalculateCropResults(_mockFieldResultsService.Object, noEconDataFarm);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, noEconResult.Count);
        }

        [TestMethod]
        public void CalculateProfitTest()
        {
            var resultViewItem = new EconomicsResultsViewItem()
            {
                CropEconomicData = _econProvider.Get(CropType.Barley, SoilFunctionalCategory.Brown, Province.Alberta),
                Area = 2,
            };
            var units = MeasurementSystemType.Metric;
            _calculator.CalculateFieldComponentsProfit(resultViewItem, units);
            Assert.AreNotEqual(0, resultViewItem.Profit);
        }

        #endregion

        #region Equations Tests

        /// <summary>
        /// Equation 1.0
        /// </summary>
        [TestMethod]
        public void CalculateRevenueTest()
        {
            var econData = new CropEconomicData()
            {
                ExpectedMarketPrice = 10.1,
                Unit = EconomicMeasurementUnits.Bushel,
                CropType = CropType.FeedBarley,
            };
            var expected = 10.1 * 2300;
            var result = _calculator.CalculateRevenue(econData.ExpectedMarketPrice, 2300);
            Assert.AreEqual(expected, Math.Round(result, 2));
        }

        /// <summary>
        /// Equation 1.1
        /// </summary>
        [TestMethod]
        public void CalculateFixedCostTest()
        {
            // assuming that this will always be in imperial unit
            var fixedCostPerUnitArea = 4;
            var result = _calculator.CalculateTotalFixedCost(fixedCostPerUnitArea, MetricArea);
            var expected = 4 * 45;
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Equation 1.2
        /// </summary>
        [TestMethod]
        public void CalculateIrrigatedFixedCostsTest()
        {
            var irrigationCost = 35;
            var result = _calculator.CalculateIrrigatedFixedCosts(irrigationCost, MetricArea);
            var expected = 35 * 45;
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Equation 1.3
        /// </summary>
        [TestMethod]
        public void CalculateVariableCostsTest()
        {
            var variableCostPerAcre = 14;
            var result = _calculator.CalculateTotalVariableCost(variableCostPerAcre, MetricArea);
            var expected = 14 * 45;
            Assert.AreEqual(expected, result);

        }

        /// <summary>
        /// Equation 1.4
        /// </summary>
        [TestMethod]
        public void CalculateIrrigatedVariableCostTest()
        {
            var variableIrrigationCostPerAcre = 34;
            var result = _calculator.CalculateIrrigatedVariableCost(variableIrrigationCostPerAcre, MetricArea);

            var expected = 34 * 45;
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Equation 1.5
        /// </summary>
        [TestMethod]
        public void CalculateNitrogenVariableCostTest()
        {
            var nitrogenCostPerTonne = 489;
            var fertilizerRateMetric = 1.2;
            var fertilizerRateImperial = 1.0707;
            var result = _calculator.CalculateNitrogenVariableCost(nitrogenCostPerTonne, fertilizerRateMetric, MetricArea, MeasurementSystemType.Metric);
            var imperialResult = _calculator.CalculateNitrogenVariableCost(nitrogenCostPerTonne, fertilizerRateImperial, ImperialArea, MeasurementSystemType.Imperial);
            var expected = 26.406;

            Assert.AreEqual(expected, result, 1);
            Assert.AreEqual(expected, imperialResult, 1);
        }

        /// <summary>
        /// Equation 1.6
        /// </summary>
        [TestMethod]
        public void CalculatePhosphorusVariableCostTest()
        {
            var phosphorusCostPerTonne = 700;
            var fertilizerRateMetric = 2;
            var fertilizerRateImperial = 1.78;
            var result = _calculator.CalculatePhosphorusVariableCost(phosphorusCostPerTonne, fertilizerRateMetric, MetricArea, MeasurementSystemType.Metric);
            var imperialResult = _calculator.CalculatePhosphorusVariableCost(phosphorusCostPerTonne, fertilizerRateImperial, ImperialArea, MeasurementSystemType.Imperial);

            var expected = 63;
            Assert.AreEqual(expected, result, 1);
            Assert.AreEqual(expected, imperialResult, 1);
        }

        /// <summary>
        /// Equation 1.7
        /// </summary>
        [TestMethod]
        public void CalculateHerbicideVariableCostTest()
        {
            var variableHerbicideCostPerAcre = 11.86;
            var variableHerbicideCostPerHectare = variableHerbicideCostPerAcre * 2.4711;
            var result = _calculator.CalculateHerbicideVariableCost(variableHerbicideCostPerHectare, MetricArea);
            var imperialResult = _calculator.CalculateHerbicideVariableCost(variableHerbicideCostPerAcre, ImperialArea);

            var expected = 1318.80;
            Assert.AreEqual(expected, result, 1);
            Assert.AreEqual(expected, imperialResult, 1);
        }

        /// <summary>
        /// Equation 1.8
        /// </summary>
        [TestMethod]
        public void CalculateFixedLabourCostTest()
        {

            var labourCostPerAcre = 23;
            var result = _calculator.CalculateFixedLabourCost(labourCostPerAcre, MetricArea);

            var expected = 23 * 45;
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Equation 1.9
        /// </summary>
        [TestMethod]
        public void CalculateFixedLabourCostIrrigationTest()
        {
            var labourCostForIrrigationPerAcre = 10;
            var result = _calculator.CalculateFixedLabourCostIrrigation(labourCostForIrrigationPerAcre, MetricArea);
            var expected = 10 * 45;

            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Equation 1.10
        /// </summary>
        [TestMethod]
        public void CalculateNOHerbicideFixedCost()
        {
            var metricFixedCostPerUnitArea = 4;
            var imperialFixedCostPerUnitArea = metricFixedCostPerUnitArea / 2.4711;
            var numberOfPasses = 11;
            var result = _calculator.CalculateNOHerbicideFixedCost(numberOfPasses, metricFixedCostPerUnitArea, MetricArea, MeasurementSystemType.Metric);
            var imperialResult = _calculator.CalculateNOHerbicideFixedCost(numberOfPasses, imperialFixedCostPerUnitArea, ImperialArea, MeasurementSystemType.Imperial);

            var expected = -1562.4;
            Assert.AreEqual(expected, result, 1);
            Assert.AreEqual(expected, imperialResult, 1);
        }

        /// <summary>
        /// Equation 1.11
        /// </summary>
        [TestMethod]
        public void CalculateNOHerbicideVariableCostTest()
        {
            var variableCostsPerAcre = 14;
            var variableCostPerHectare = variableCostsPerAcre * 2.4711;
            var numberOfPasses = 20;
            var result = _calculator.CalculateNOHerbicideVariableCost(variableCostPerHectare, numberOfPasses, MetricArea, MeasurementSystemType.Metric);
            var imperialResult = _calculator.CalculateNOHerbicideVariableCost(variableCostsPerAcre, numberOfPasses, ImperialArea, MeasurementSystemType.Imperial);

            var expected = -1197.2;
            Assert.AreEqual(expected, result, 1);
            Assert.AreEqual(expected, imperialResult, 1);
        }

        /// <summary>
        /// Equation 1.12
        /// </summary>
        [TestMethod]
        public void CalculateNOHerbicideLabourCostTest()
        {
            var labourCostsPerAcre = 4;
            var labourCostsPerHectare = labourCostsPerAcre * 2.4711;
            var numberOfPasses = 11;
            var result = _calculator.CalculateNOHerbicideLabourCost(labourCostsPerHectare, numberOfPasses, MetricArea, MeasurementSystemType.Metric);
            var imperialResult = _calculator.CalculateNOHerbicideLabourCost(labourCostsPerAcre, numberOfPasses, ImperialArea, MeasurementSystemType.Imperial);
            var expected = 162.6396;
            Assert.AreEqual(expected, result, 1);
            Assert.AreEqual(expected, imperialResult, 1);
        }

        #endregion

    }
}
