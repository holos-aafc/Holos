using Microsoft.VisualStudio.TestTools.UnitTesting;
using H.Core.Calculators.Economics;
using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Providers.Economics;

namespace H.Core.Test.Calculators.Economics
{
    [TestClass()]
    public class EconomicsHelperTests
    {
        private EconomicsHelper _economicsHelper;
        private CropEconomicsProvider _economicsProvider;
        private Farm _farm;

        [TestInitialize]
        public void Initialize()
        {
            _economicsHelper = new EconomicsHelper();
            _economicsProvider = new CropEconomicsProvider();
            _farm = new Farm() { MeasurementSystemType = MeasurementSystemType.Metric };
        }

        [TestMethod()]
        public void ConvertValuesToMetricIfNecessaryTest()
        {
            var econData = _economicsProvider.Get(CropType.Canola, SoilFunctionalCategory.NotApplicable, Province.Manitoba);
            _economicsHelper.ConvertValuesToMetricIfNecessary(econData, _farm);

            var unconvertedEconData = _economicsProvider.Get(CropType.Canola, SoilFunctionalCategory.NotApplicable, Province.Manitoba);

            Assert.AreNotSame(econData, unconvertedEconData);

            var propertyList = new List<string>()
            {
                nameof(econData.CropSalesPerAcre),
                nameof(econData.SeedCleaningAndTreatment),
                nameof(econData.Fertilizer),
                nameof(econData.Chemical),
                nameof(econData.HailCropInsurance),
                nameof(econData.TruckingMarketing),
                nameof(econData.FuelOilLube),
                nameof(econData.MachineryRepairs),
                nameof(econData.BuildingRepairs),
                nameof(econData.CustomWork),
                nameof(econData.Labour),
                nameof(econData.Utilities),
                nameof(econData.OperatingInterest),
                nameof(econData.TotalCost),
                nameof(econData.TotalFixedCostPerUnit),
                nameof(econData.TotalVariableCostPerUnit),
                nameof(econData.PumpingCosts),
                nameof(econData.HerbicideCost)
            };
            foreach (var prop in econData.GetType().GetProperties())
            {
                if (propertyList.Contains(prop.Name))
                {
                    var unconvertedProp = unconvertedEconData.GetType().GetProperties().First(x => x.Name == prop.Name);
                    if ((double)unconvertedProp.GetValue(unconvertedEconData) == 0) continue;

                    Assert.AreNotEqual(unconvertedProp.GetValue(unconvertedEconData), prop.GetValue(econData));
                }
            }
        }

        [TestMethod()]
        public void ConvertExpectedMarketPriceToMetricIfNecessaryTest()
        {

            //bushels
            var canolaBushels = _economicsProvider.Get(CropType.Canola, SoilFunctionalCategory.Brown, Province.Alberta);
            _economicsHelper.ConvertExpectedMarketPriceToMetricIfNecessary(canolaBushels, _farm);
            var canolaPricePerKilo = 0.46;

            //pounds
            var yellowMustardPounds =
                _economicsProvider.Get(CropType.Mustard, SoilFunctionalCategory.Brown, Province.Alberta);
            _economicsHelper.ConvertExpectedMarketPriceToMetricIfNecessary(yellowMustardPounds, _farm);
            var mustardPricePerKilo = 0.77;

            //tons
            var mixedHayTons =
                _economicsProvider.Get(CropType.TameMixed, SoilFunctionalCategory.Brown, Province.Alberta);
            _economicsHelper.ConvertExpectedMarketPriceToMetricIfNecessary(mixedHayTons, _farm);
            var mixedHayPricePerKilo = 0.08;

            //hundredweight(cwt)
            var colouredBeansHundredWeight = new CropEconomicData()
            {
                Unit = EconomicMeasurementUnits.HundredWeight,
                ExpectedMarketPrice = 36
            };
            _economicsHelper.ConvertExpectedMarketPriceToMetricIfNecessary(colouredBeansHundredWeight, _farm);
            var colouredBeansPricePerKilo = 0.79;

            Assert.AreEqual(canolaPricePerKilo, Math.Round(canolaBushels.ExpectedMarketPrice, 2));
            Assert.AreEqual(mustardPricePerKilo, Math.Round(yellowMustardPounds.ExpectedMarketPrice, 2));
            Assert.AreEqual(mixedHayPricePerKilo, Math.Round(mixedHayTons.ExpectedMarketPrice, 2));
            Assert.AreEqual(colouredBeansPricePerKilo, Math.Round(colouredBeansHundredWeight.ExpectedMarketPrice, 2));
        }
    }
}