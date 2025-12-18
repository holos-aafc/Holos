using H.Core.Providers.Soil;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using H.Core.Enumerations;

namespace H.Core.Test.Providers.Soil
{
    [TestClass]
    public class DefaultYieldProviderTest
    {
        private DefaultYieldProvider _provider;

        [TestInitialize]
        public void TestInitialize()
        {

            _provider = new DefaultYieldProvider();
        }

        [TestMethod]
        public void GetRowByCarIdYearAndCropType()
        {
            var result = _provider.GetRowByCarIdYearAndCropType(2413, 1985, CropType.Oats);
            
            Assert.AreEqual(result.PrId, 24);
            Assert.AreEqual(result.CarId, 2413);
            Assert.AreEqual(result.PrSad, 2409);
            Assert.AreEqual(result.Year, 1985);
            Assert.AreEqual(result.CropType, CropType.Oats);
            Assert.AreEqual(result.Seeded, -99);
            Assert.AreEqual(result.ESeed, -99);
            Assert.AreEqual(result.Harv, -99);
            Assert.AreEqual(result.EHarv, -99);
            Assert.AreEqual(result.PerHarv, -99);
            Assert.AreEqual(result.Yield, -99);
            Assert.AreEqual(result.YldLbs, -99);
            Assert.AreEqual(result.EYield, -99);
            Assert.AreEqual(result.ProdN, -99);
            Assert.AreEqual(result.PrdLbs, -99);
            Assert.AreEqual(result.EProdN, -99);
            Assert.AreEqual(result.NYield, -99);
            Assert.AreEqual(result.NYldLbs, -99);
            Assert.AreEqual(result.NEYield, -99);
            Assert.AreEqual(result.PPYield, -99);
            Assert.AreEqual(result.CSad, "Montérégie Sud-ouest, 10 - Québec");
        }

        [TestMethod]
        public void GetRowByCarIdYearAndCropTypeReturnsDefaultValueWhenNonExistantValueIsEntered()
        {
            var result = _provider.GetRowByCarIdYearAndCropType(-1, 1985, CropType.CanarySeed);
            Assert.AreEqual(result.PrId, 0);
            Assert.AreEqual(result.CarId, 0);
            Assert.AreEqual(result.PrSad, 0);
            Assert.AreEqual(result.Year, 0);
            Assert.AreEqual(result.CropType, CropType.Barley);
            Assert.AreEqual(result.Seeded, 0);
            Assert.AreEqual(result.ESeed, 0);
            Assert.AreEqual(result.Harv, 0);
            Assert.AreEqual(result.EHarv, 0);
            Assert.AreEqual(result.PerHarv, 0);
            Assert.AreEqual(result.Yield, 0);
            Assert.AreEqual(result.YldLbs, 0);
            Assert.AreEqual(result.EYield, 0);
            Assert.AreEqual(result.ProdN, 0);
            Assert.AreEqual(result.PrdLbs, 0);
            Assert.AreEqual(result.EProdN, 0);
            Assert.AreEqual(result.NYield, 0);
            Assert.AreEqual(result.NYldLbs, 0);
            Assert.AreEqual(result.NEYield, 0);
            Assert.AreEqual(result.PPYield, 0);
        }
    }
}
        
