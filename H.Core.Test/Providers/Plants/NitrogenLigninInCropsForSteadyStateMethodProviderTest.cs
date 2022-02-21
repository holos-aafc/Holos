using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Providers.Plants;
using H.Core.Enumerations;

namespace H.Core.Test.Providers.Plants
{
    [TestClass]
    public class NitrogenLigninInCropsForSteadyStateMethodProviderTest
    {
        #region Fields
        private static NitrogenLigninInCropsForSteadyStateMethodProvider_Table_12 _provider;
        #endregion

        #region Initialization
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _provider = new NitrogenLigninInCropsForSteadyStateMethodProvider_Table_12();
        }

        [ClassCleanup]
        public static void ClassCleanup() { }

        [TestInitialize]
        public void TestInitialize() { }

        [TestCleanup]
        public void TestCleanup() { }

        #endregion

        #region Tests
        [TestMethod]
        public void GetContentsInstance()
        {
            NitrogenLigninInCropsForSteadyStateMethodData data = _provider.GetDataByCropType(CropType.NFixingForages);
            Assert.AreEqual(0.025, data.NitrogenContentResidues);
        }

        [TestMethod]
        public void GetInstanceWrongCrop()
        {
            NitrogenLigninInCropsForSteadyStateMethodData data = _provider.GetDataByCropType(CropType.KabuliChickpea);
            Assert.AreEqual(0, data.LigninContentResidues);
        }

        [TestMethod]
        public void CheckMoistureContent()
        {
            NitrogenLigninInCropsForSteadyStateMethodData data = _provider.GetDataByCropType(CropType.SugarBeets);
            Assert.AreEqual(80.0, data.MoistureContent);
        }

        [TestMethod]
        public void CheckLigninContentOfResidues()
        {
            NitrogenLigninInCropsForSteadyStateMethodData data = _provider.GetDataByCropType(CropType.OtherDryFieldBeans);
            Assert.AreEqual(0.075, data.LigninContentResidues);
        }

        [TestMethod]
        public void CheckRootToShootRatioOfCrop()
        {
            NitrogenLigninInCropsForSteadyStateMethodData data = _provider.GetDataByCropType(CropType.Rice);
            Assert.AreEqual(0.0, data.RSTRatio);
        }

        [TestMethod]
        public void CheckSlopeValueOfCrop()
        {
            NitrogenLigninInCropsForSteadyStateMethodData data = _provider.GetDataByCropType(CropType.Grains);
            Assert.AreEqual(0.02, data.SlopeValue);
        }

        [TestMethod]
        public void CheckInterceptValueOfCrop()
        {
            NitrogenLigninInCropsForSteadyStateMethodData data = _provider.GetDataByCropType(CropType.WhiteBeans);
            Assert.AreEqual(0.279, data.InterceptValue);
        }
        #endregion
    }
}
