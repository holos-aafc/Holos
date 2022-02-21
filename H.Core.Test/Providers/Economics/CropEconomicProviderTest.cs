using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Providers.Economics;

namespace H.Core.Test.Providers.Economics
{
    [TestClass]
    public class CropEconomicProviderTest
    {
        #region Fields

        private CropEconomicsProvider _provider;

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
            _provider = new CropEconomicsProvider();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void Get_DirectMatchTest()
        {
            var fieldPeas = _provider.Get(CropType.FieldPeas, SoilFunctionalCategory.Black, Province.Alberta);
            Assert.AreEqual(7, fieldPeas.ExpectedMarketPrice);
            Assert.AreEqual(520, fieldPeas.NitrogenCostPerTonne);
            Assert.AreEqual(645, fieldPeas.PhosphorusCostPerTonne);
            Assert.AreEqual(241.98, fieldPeas.TotalVariableCostPerUnit);
            Assert.AreEqual(154.98, fieldPeas.TotalFixedCostPerUnit);

            var flax = _provider.Get(CropType.Flax, SoilFunctionalCategory.Black, Province.Saskatchewan);
            Assert.AreEqual(13.97, flax.ExpectedMarketPrice);
            Assert.AreEqual(520, flax.NitrogenCostPerTonne);
            Assert.AreEqual(645, flax.PhosphorusCostPerTonne);
            Assert.AreEqual(227.59, flax.TotalVariableCostPerUnit);
            Assert.AreEqual(119.57, flax.TotalFixedCostPerUnit);

            var soybeans = _provider.Get(CropType.Soybeans, SoilFunctionalCategory.Black, Province.Manitoba);
            Assert.AreEqual(11.40, soybeans.ExpectedMarketPrice);
            Assert.AreEqual(790, soybeans.NitrogenCostPerTonne);
            Assert.AreEqual(700, soybeans.PhosphorusCostPerTonne);
            Assert.AreEqual(216.25, soybeans.TotalVariableCostPerUnit);
            Assert.AreEqual(136.6, soybeans.TotalFixedCostPerUnit);

            //test for the summerfallow included by the Alberta Gov.
            var brownFallow = _provider.Get(CropType.Fallow, SoilFunctionalCategory.Brown, Province.Alberta);
            Assert.AreEqual(0, brownFallow.ExpectedMarketPrice);
            Assert.AreEqual(43.64, brownFallow.TotalVariableCostPerUnit);
            Assert.AreEqual(80.87, brownFallow.TotalFixedCostPerUnit);
            Assert.AreEqual(0, brownFallow.Fertilizer);
            Assert.AreEqual(12.07, brownFallow.Chemical);
            Assert.AreEqual(7.73, brownFallow.FuelOilLube);

            //test for the summerfallow/fallow that I added into the dataset (all 0's)
            var allZeroFallow = _provider.Get(CropType.Fallow, SoilFunctionalCategory.DarkBrown, Province.Alberta);
            Assert.AreEqual(0, allZeroFallow.ExpectedMarketPrice);
            Assert.AreEqual(0, allZeroFallow.TotalVariableCostPerUnit);
            Assert.AreEqual(0, allZeroFallow.TotalFixedCostPerUnit);
            Assert.AreEqual(0, allZeroFallow.Fertilizer);
            Assert.AreEqual(0, allZeroFallow.Chemical);
            Assert.AreEqual(0, allZeroFallow.FuelOilLube);

            //durum isn't in the main crop list
            var notSelected = _provider.Get(CropType.Durum, SoilFunctionalCategory.Black, Province.Alberta);
            Assert.AreEqual(0, notSelected.ExpectedMarketPrice);
            Assert.AreEqual(CropType.NotSelected, notSelected.CropType);

            var ontarioBarley = _provider.Get(CropType.Barley, SoilFunctionalCategory.EasternCanada, Province.Ontario);
            Assert.AreEqual(5.3, ontarioBarley.ExpectedMarketPrice);
            Assert.AreEqual(360.26, ontarioBarley.TotalVariableCostPerUnit);
            Assert.AreEqual(70.2, ontarioBarley.TotalFixedCostPerUnit);
            Assert.AreEqual(84.69, ontarioBarley.Fertilizer);
            Assert.AreEqual(36.55, ontarioBarley.Chemical);
            Assert.AreEqual(34.75, ontarioBarley.FuelOilLube);


            //BC will not pull from AB right now so there should be no econ data
            var britishColumbia =
                _provider.Get(CropType.Barley, SoilFunctionalCategory.Black, Province.BritishColumbia);
            Assert.AreEqual(Province.BritishColumbia, britishColumbia.Province);
            Assert.AreEqual(0, britishColumbia.ExpectedMarketPrice);
            Assert.AreEqual(0, britishColumbia.TotalVariableCostPerUnit);
            Assert.AreEqual(0, britishColumbia.TotalFixedCostPerUnit);
            Assert.AreEqual(0, britishColumbia.Fertilizer);
            Assert.AreEqual(0, britishColumbia.Chemical);
            Assert.AreEqual(0, britishColumbia.FuelOilLube);
        }

        [TestMethod]
        public void Get_MatchAcrossProvincesTest()
        {
            //no soybeans in alberta but they are in saskatchewan
            var soyBeans = _provider.Get(CropType.Soybeans, SoilFunctionalCategory.Brown, Province.Alberta);
            Assert.AreEqual(Province.Saskatchewan, soyBeans.Province);
            Assert.AreEqual(SoilFunctionalCategory.Brown, soyBeans.SoilFunctionalCategory);
            Assert.AreEqual(10.93, soyBeans.ExpectedMarketPrice);
            Assert.AreEqual(520, soyBeans.NitrogenCostPerTonne);
            Assert.AreEqual(645, soyBeans.PhosphorusCostPerTonne);
            Assert.AreEqual(241.46, soyBeans.TotalVariableCostPerUnit);
            Assert.AreEqual(95.34, soyBeans.TotalFixedCostPerUnit);
            Assert.AreEqual(13.83, soyBeans.Fertilizer);
            Assert.AreEqual(0, soyBeans.Labour);

            //Sask. doesn't have any silage crops but Alberta does
            var saskSilage = _provider.Get(CropType.BarleySilage, SoilFunctionalCategory.Brown, Province.Saskatchewan);
            Assert.AreEqual(Province.Alberta, saskSilage.Province);
            Assert.AreEqual(SoilFunctionalCategory.DarkBrown, saskSilage.SoilFunctionalCategory);
            Assert.AreEqual(58.56, saskSilage.ExpectedMarketPrice);
            Assert.AreEqual(520, saskSilage.NitrogenCostPerTonne);
            Assert.AreEqual(645, saskSilage.PhosphorusCostPerTonne);
            Assert.AreEqual(175.58, saskSilage.TotalVariableCostPerUnit);
            Assert.AreEqual(73.06, saskSilage.TotalFixedCostPerUnit);
            Assert.AreEqual(40, saskSilage.Fertilizer);
            Assert.AreEqual(10.22, saskSilage.Labour);

            //Manitoba doesn't have any silage crops but Alberta does
            var manitobaSilage = _provider.Get(CropType.BarleySilage, SoilFunctionalCategory.Black, Province.Manitoba);
            Assert.AreEqual(Province.Alberta, manitobaSilage.Province);
            Assert.AreEqual(SoilFunctionalCategory.Black, manitobaSilage.SoilFunctionalCategory);
            Assert.AreEqual(58.56, manitobaSilage.ExpectedMarketPrice);
            Assert.AreEqual(520, manitobaSilage.NitrogenCostPerTonne);
            Assert.AreEqual(645, manitobaSilage.PhosphorusCostPerTonne);
            Assert.AreEqual(203.23, manitobaSilage.TotalVariableCostPerUnit);
            Assert.AreEqual(107.53, manitobaSilage.TotalFixedCostPerUnit);
            Assert.AreEqual(45, manitobaSilage.Fertilizer);
            Assert.AreEqual(33.1, manitobaSilage.Labour);

            //Manitoba doesn't have any mustard crops but Saskatchewan does
            var manitobaMustard = _provider.Get(CropType.Mustard, SoilFunctionalCategory.Black, Province.Manitoba);
            Assert.AreEqual(Province.Saskatchewan, manitobaMustard.Province);
            Assert.AreEqual(SoilFunctionalCategory.Brown, manitobaMustard.SoilFunctionalCategory);
            Assert.AreEqual(0.42, manitobaMustard.ExpectedMarketPrice);
            Assert.AreEqual(520, manitobaMustard.NitrogenCostPerTonne);
            Assert.AreEqual(645, manitobaMustard.PhosphorusCostPerTonne);
            Assert.AreEqual(178.29, manitobaMustard.TotalVariableCostPerUnit);
            Assert.AreEqual(95.34, manitobaMustard.TotalFixedCostPerUnit);
            Assert.AreEqual(38.19, manitobaMustard.Fertilizer);
            Assert.AreEqual(0, manitobaMustard.Labour);
        }

        [TestMethod]
        public void Get_MatchAcrossSoilZonesTest()
        {

            //DarkBrown soil in AB doesn't have chickPeas but brown soil does.
            var chickPeasDarkBrown = _provider.Get(CropType.Chickpeas, SoilFunctionalCategory.DarkBrown, Province.Alberta);
            Assert.AreEqual(SoilFunctionalCategory.Brown, chickPeasDarkBrown.SoilFunctionalCategory);
            Assert.AreEqual(0.3, chickPeasDarkBrown.ExpectedMarketPrice);
            Assert.AreEqual(520, chickPeasDarkBrown.NitrogenCostPerTonne);
            Assert.AreEqual(645, chickPeasDarkBrown.PhosphorusCostPerTonne);
            Assert.AreEqual(235.62, chickPeasDarkBrown.TotalVariableCostPerUnit);
            Assert.AreEqual(91.99, chickPeasDarkBrown.TotalFixedCostPerUnit);
            Assert.AreEqual(16.50, chickPeasDarkBrown.Fertilizer);
            Assert.AreEqual(15.32, chickPeasDarkBrown.Labour);

            //black soil in AB doesn't have chickPeas but brown soil does.
            var chickPeasBrown = _provider.Get(CropType.Chickpeas, SoilFunctionalCategory.Black, Province.Alberta);
            Assert.AreEqual(SoilFunctionalCategory.Brown, chickPeasBrown.SoilFunctionalCategory);
            Assert.AreEqual(0.3, chickPeasBrown.ExpectedMarketPrice);
            Assert.AreEqual(520, chickPeasBrown.NitrogenCostPerTonne);
            Assert.AreEqual(645, chickPeasBrown.PhosphorusCostPerTonne);
            Assert.AreEqual(235.62, chickPeasBrown.TotalVariableCostPerUnit);
            Assert.AreEqual(91.99, chickPeasBrown.TotalFixedCostPerUnit);
            Assert.AreEqual(16.50, chickPeasBrown.Fertilizer);
            Assert.AreEqual(15.32, chickPeasBrown.Labour);

            //brown soil in AB doesn't have cerealSilage but DarkBrown soil does.
            var cerealSilage = _provider.Get(CropType.BarleySilage, SoilFunctionalCategory.Brown, Province.Alberta);
            Assert.AreEqual(SoilFunctionalCategory.DarkBrown, cerealSilage.SoilFunctionalCategory);
            Assert.AreEqual(58.56, cerealSilage.ExpectedMarketPrice);
            Assert.AreEqual(520, cerealSilage.NitrogenCostPerTonne);
            Assert.AreEqual(645, cerealSilage.PhosphorusCostPerTonne);
            Assert.AreEqual(175.58, cerealSilage.TotalVariableCostPerUnit);
            Assert.AreEqual(73.06, cerealSilage.TotalFixedCostPerUnit);
            Assert.AreEqual(40, cerealSilage.Fertilizer);
            Assert.AreEqual(10.22, cerealSilage.Labour);


        }

        [TestMethod]
        public void HasCropDataTest()
        {
            bool notSelected = _provider.HasDataForCropType(CropType.NotSelected);
            Assert.IsFalse(notSelected);

            bool fieldPeas = _provider.HasDataForCropType(CropType.FieldPeas);
            Assert.IsTrue(fieldPeas);

            bool oatSilage = _provider.HasDataForCropType(CropType.OatSilage);
            Assert.IsFalse(oatSilage);
        }

        [TestMethod]
        public void HasProvinceDataTest()
        {
            bool alberta = _provider.HasDataForProvince(Province.Alberta);
            Assert.IsTrue(alberta);

            bool bc = _provider.HasDataForProvince(Province.BritishColumbia);
            Assert.IsFalse(bc);
        }

        [TestMethod]
        public void FarmCropToEconCropTest()
        {
            var saskSuccess = _provider.FarmCropTypeToEconCropType(CropType.CanarySeed, SoilFunctionalCategory.Brown, Province.Saskatchewan);
            Assert.AreEqual(CropType.CanarySeed, saskSuccess);

            var manSuccess = _provider.FarmCropTypeToEconCropType(CropType.Oats, SoilFunctionalCategory.NotApplicable, Province.Manitoba);
            Assert.AreEqual(CropType.Oats, manSuccess);

            var abSuccess = _provider.FarmCropTypeToEconCropType(CropType.OatSilage, SoilFunctionalCategory.Brown, Province.Alberta);
            Assert.AreEqual(CropType.CerealSilage, abSuccess);

            var abNotSelected = _provider.FarmCropTypeToEconCropType(CropType.Potatoes, SoilFunctionalCategory.Brown, Province.Alberta);
            Assert.AreEqual(CropType.NotSelected, abNotSelected);

            var saskNotSelected = _provider.FarmCropTypeToEconCropType(CropType.Potatoes, SoilFunctionalCategory.Brown, Province.Saskatchewan);
            Assert.AreEqual(CropType.NotSelected, saskNotSelected);

            var manNotSelected = _provider.FarmCropTypeToEconCropType(CropType.Potatoes, SoilFunctionalCategory.NotApplicable, Province.Manitoba);
            Assert.AreEqual(CropType.NotSelected, manNotSelected);

            var onNotSelected = _provider.FarmCropTypeToEconCropType(CropType.Potatoes, SoilFunctionalCategory.NotApplicable, Province.Ontario);
            Assert.AreEqual(CropType.NotSelected, onNotSelected);

            var manSummerFallow = _provider.FarmCropTypeToEconCropType(CropType.SummerFallow,
                SoilFunctionalCategory.NotApplicable, Province.Manitoba);
            Assert.AreEqual(CropType.SummerFallow, manSummerFallow);

            var saskSummerFallow = _provider.FarmCropTypeToEconCropType(CropType.SummerFallow,
                SoilFunctionalCategory.NotApplicable, Province.Saskatchewan);
            Assert.AreEqual(CropType.SummerFallow, saskSummerFallow);

            var abSummerFallow = _provider.FarmCropTypeToEconCropType(CropType.SummerFallow,
                SoilFunctionalCategory.NotApplicable, Province.Alberta);
            Assert.AreEqual(CropType.SummerFallow, abSummerFallow);
        }


        [TestMethod]
        public void GetCropEconomicDataByProvinceAndSoilCategoryTest()
        {
            var result = _provider.GetCropEconomicDataByProvinceAndSoilZone(Province.Alberta, SoilFunctionalCategory.Brown).ToList();
            foreach (var data in result)
            {
                Assert.AreEqual(Province.Alberta, data.Province);
                Assert.AreEqual(SoilFunctionalCategory.Brown, data.SoilFunctionalCategory);
            }

            var crops = result.Select(x => x.CropType).ToList();
            Assert.IsFalse(crops.Contains(CropType.CerealSilage));
        }

        [Ignore]
        [TestMethod]
        public void GetAverageFixedCostsSaskBySoilGroupTest()
        {
            var brown = _provider.GetCropEconomicDataByProvinceAndSoilZone(Province.Saskatchewan, SoilFunctionalCategory.Brown);

            var darkBrown = _provider.GetCropEconomicDataByProvinceAndSoilZone(Province.Saskatchewan, SoilFunctionalCategory.DarkBrown);

            var black = _provider.GetCropEconomicDataByProvinceAndSoilZone(Province.Saskatchewan, SoilFunctionalCategory.Black);

            var brownAverage = brown.Average(x => x.TotalFixedCostPerUnit);
            var darkBrownAverage = darkBrown.Average(x => x.TotalFixedCostPerUnit);
            var blackAverage = black.Average(x => x.TotalFixedCostPerUnit);

            Trace.TraceInformation($"{nameof(brownAverage)}: {brownAverage}");
            Trace.TraceInformation($"{nameof(darkBrownAverage)}: {darkBrownAverage}");
            Trace.TraceInformation($"{nameof(blackAverage)}: {blackAverage}");
        }
        #endregion
    }
}
