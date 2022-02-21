using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Enumerations;
using H.Core.Providers.Economics;

namespace H.Core.Test.Models.LandManagement.Fields
{
    [TestClass]
    public class CropEconomicDataTest
    {
        [TestMethod]
        public void SetUserDefinedVariableCostTest()
        {
            var userDefinedEconData = new CropEconomicData()
            {
                SeedCleaningAndTreatment = 23,
                Fertilizer = 12,
                Chemical = 14,
                HailCropInsurance = 45,
            };

            var defaultEconData = new CropEconomicData();
            defaultEconData.SetUserDefinedVariableCostPerUnit();
            Assert.AreEqual(0, defaultEconData.TotalVariableCostPerUnit);

            userDefinedEconData.SetUserDefinedVariableCostPerUnit();
            var expected = 23 + 12 + 14 + 45;
            Assert.AreEqual(expected, userDefinedEconData.TotalVariableCostPerUnit);

        }

        [TestMethod]
        public void SetUserDefinedFixedCostTest()
        {
            var measurementSystem = MeasurementSystemType.Imperial;
            #region crops
            //saskatchewan
            var blackSaskEconDat = new CropEconomicData()
            {
                Province = Province.Saskatchewan,
                SoilFunctionalCategory = SoilFunctionalCategory.Black,
            };
            var darkBrownSaskEconData = new CropEconomicData()
            {
                Province = Province.Saskatchewan,
                SoilFunctionalCategory = SoilFunctionalCategory.DarkBrown,
            };
            var brownSaskEconData = new CropEconomicData()
            {
                Province = Province.Saskatchewan,
                SoilFunctionalCategory = SoilFunctionalCategory.Brown,
            };
            brownSaskEconData.SetUserDefinedFixedCostPerUnit(measurementSystem);
            darkBrownSaskEconData.SetUserDefinedFixedCostPerUnit(measurementSystem);
            blackSaskEconDat.SetUserDefinedFixedCostPerUnit(measurementSystem);
            Assert.AreEqual(95.98, brownSaskEconData.TotalFixedCostPerUnit);
            Assert.AreEqual(110.27, darkBrownSaskEconData.TotalFixedCostPerUnit);
            Assert.AreEqual(126.51, blackSaskEconDat.TotalFixedCostPerUnit);
            brownSaskEconData.SetUserDefinedFixedCostPerUnit(MeasurementSystemType.Metric);
            darkBrownSaskEconData.SetUserDefinedFixedCostPerUnit(MeasurementSystemType.Metric);
            blackSaskEconDat.SetUserDefinedFixedCostPerUnit(MeasurementSystemType.Metric);
            Assert.AreEqual(95.98 * 2.4711, brownSaskEconData.TotalFixedCostPerUnit);
            Assert.AreEqual(110.27 * 2.4711, darkBrownSaskEconData.TotalFixedCostPerUnit);
            Assert.AreEqual(126.51 * 2.4711, blackSaskEconDat.TotalFixedCostPerUnit);


            //alberta
            var brownAlbertaEconData = new CropEconomicData()
            {
                Province = Province.Alberta,
                SoilFunctionalCategory = SoilFunctionalCategory.Brown,
                ViewItemCropType = CropType.Barley,
            };
            var darkBrownAlbertaEconData = new CropEconomicData()
            {
                Province = Province.Alberta,
                SoilFunctionalCategory = SoilFunctionalCategory.DarkBrown,
                ViewItemCropType = CropType.Barley,
            };
            var blackAlbertaEconData = new CropEconomicData()
            {
                Province = Province.Alberta,
                SoilFunctionalCategory = SoilFunctionalCategory.Black,
                ViewItemCropType = CropType.Barley,
            };
            brownAlbertaEconData.SetUserDefinedFixedCostPerUnit(measurementSystem);
            darkBrownAlbertaEconData.SetUserDefinedFixedCostPerUnit(measurementSystem);
            blackAlbertaEconData.SetUserDefinedFixedCostPerUnit(measurementSystem);
            Assert.AreEqual(91.99, brownAlbertaEconData.TotalFixedCostPerUnit);
            Assert.AreEqual(98.13, darkBrownAlbertaEconData.TotalFixedCostPerUnit);
            Assert.AreEqual(154.98, blackAlbertaEconData.TotalFixedCostPerUnit);
            brownAlbertaEconData.SetUserDefinedFixedCostPerUnit(MeasurementSystemType.Metric);
            darkBrownAlbertaEconData.SetUserDefinedFixedCostPerUnit(MeasurementSystemType.Metric);
            blackAlbertaEconData.SetUserDefinedFixedCostPerUnit(MeasurementSystemType.Metric);
            Assert.AreEqual(91.99 * 2.4711, brownAlbertaEconData.TotalFixedCostPerUnit);
            Assert.AreEqual(98.13 * 2.4711, darkBrownAlbertaEconData.TotalFixedCostPerUnit);
            Assert.AreEqual(154.98 * 2.4711, blackAlbertaEconData.TotalFixedCostPerUnit);
            //manitoba
            var manitobaEconData = new CropEconomicData()
            {
                Province = Province.Manitoba,
            };
            manitobaEconData.SetUserDefinedFixedCostPerUnit(measurementSystem);
            Assert.AreEqual(138.66, manitobaEconData.TotalFixedCostPerUnit);
            manitobaEconData.SetUserDefinedFixedCostPerUnit(MeasurementSystemType.Metric);
            Assert.AreEqual(138.66 * 2.4711, manitobaEconData.TotalFixedCostPerUnit);
            #endregion

            #region forages
            //forages in alberta
            var blackSilageAlbertaEconData = new CropEconomicData()
            {
                Province = Province.Alberta,
                SoilFunctionalCategory = SoilFunctionalCategory.Black,
                ViewItemCropType = CropType.CornSilage,
            };
            var darkBrownSilageAlbertaEconData = new CropEconomicData()
            {
                Province = Province.Alberta,
                SoilFunctionalCategory = SoilFunctionalCategory.DarkBrown,
                ViewItemCropType = CropType.CornSilage,
            };
            var brownSilageAlbertaEconData = new CropEconomicData()
            {
                Province = Province.Alberta,
                SoilFunctionalCategory = SoilFunctionalCategory.Brown,
                ViewItemCropType = CropType.CornSilage,
            };
            blackSilageAlbertaEconData.SetUserDefinedFixedCostPerUnit(MeasurementSystemType.Metric);
            darkBrownSilageAlbertaEconData.SetUserDefinedFixedCostPerUnit(MeasurementSystemType.Metric);
            brownSilageAlbertaEconData.SetUserDefinedFixedCostPerUnit(MeasurementSystemType.Metric);
            Assert.AreEqual(107.53 * 2.4711, blackSilageAlbertaEconData.TotalFixedCostPerUnit);
            Assert.AreEqual(73.06 * 2.4711, darkBrownSilageAlbertaEconData.TotalFixedCostPerUnit);
            Assert.AreEqual(80.87 * 2.4711, brownSilageAlbertaEconData.TotalFixedCostPerUnit);

            blackSilageAlbertaEconData.SetUserDefinedFixedCostPerUnit(measurementSystem);
            darkBrownSilageAlbertaEconData.SetUserDefinedFixedCostPerUnit(measurementSystem);
            brownSilageAlbertaEconData.SetUserDefinedFixedCostPerUnit(measurementSystem);
            Assert.AreEqual(107.53, blackSilageAlbertaEconData.TotalFixedCostPerUnit);
            Assert.AreEqual(73.06, darkBrownSilageAlbertaEconData.TotalFixedCostPerUnit);
            Assert.AreEqual(80.87, brownSilageAlbertaEconData.TotalFixedCostPerUnit);

            #endregion

        }
    }
}
