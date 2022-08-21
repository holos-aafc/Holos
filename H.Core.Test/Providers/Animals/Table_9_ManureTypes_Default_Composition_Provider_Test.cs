using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class Table_9_ManureTypes_Default_Composition_Provider_Test
    {
        private static Table_9_ManureTypes_Default_Composition_Provider _provider;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _provider = new Table_9_ManureTypes_Default_Composition_Provider();

        }

        [TestMethod]
        public void GetManureCompositionData()
        {            
            var result = _provider.ManureCompositionData.SingleOrDefault(x => x.AnimalType == AnimalType.Beef && 
            x.ManureStateType == ManureStateType.DeepBedding);

            Assert.AreEqual(AnimalType.Beef, result.AnimalType);
            Assert.AreEqual(ManureStateType.DeepBedding, result.ManureStateType);
            Assert.AreEqual(60.08, result.MoistureContent);
            Assert.AreEqual(0.715, result.NitrogenFraction);
            Assert.AreEqual(12.63, result.CarbonFraction);
            Assert.AreEqual(17.66, result.CarbonToNitrogenRatio);
        }

        [TestMethod]
        public void TestDairyCattleData()
        {
            var data = _provider.GetManureCompositionDataByType(AnimalType.Dairy, ManureStateType.SolidStorage);

            Assert.AreEqual(77.3, data.MoistureContent);
            Assert.AreEqual(0.392, data.NitrogenFraction);
            Assert.AreEqual(2.99, data.CarbonFraction);
            Assert.AreEqual(7.63, data.CarbonToNitrogenRatio);
        }

        [TestMethod]
        public void TestSheepCattleData()
        {
            var data = _provider.GetManureCompositionDataByType(AnimalType.Sheep, ManureStateType.Pasture);

            Assert.AreEqual(75,data.MoistureContent);
            Assert.AreEqual(0.765, data.NitrogenFraction);
            Assert.AreEqual(0.211, data.PhosphorusFraction);
            Assert.AreEqual(CoreConstants.ValueNotDetermined, data.CarbonFraction);
            Assert.AreEqual(CoreConstants.ValueNotDetermined, data.CarbonToNitrogenRatio);
        }

        [TestMethod]
        public void TestPoultryData()
        {
            var data = _provider.GetManureCompositionDataByType(AnimalType.Poultry, ManureStateType.Slurry);

            Assert.AreEqual(89.510, data.MoistureContent);
            Assert.AreEqual(0.836, data.NitrogenFraction);
            Assert.AreEqual(2.92, data.CarbonFraction);
            Assert.AreEqual(0.268, data.PhosphorusFraction);
            Assert.AreEqual(3.49, data.CarbonToNitrogenRatio);
        }

        [TestMethod]
        public void TestSwineData()
        {
            var data = _provider.GetManureCompositionDataByType(AnimalType.Swine, ManureStateType.LiquidWithNaturalCrust);

            Assert.AreEqual(95.16, data.MoistureContent);
            Assert.AreEqual(0.325, data.NitrogenFraction);
            Assert.AreEqual(1.29, data.CarbonFraction);
            Assert.AreEqual(3.25, data.CarbonToNitrogenRatio);
        }

        [TestMethod]
        public void TestSwineLiquidNaturalCrustData()
        {
            var data = _provider.GetManureCompositionDataByType(AnimalType.Swine,
                ManureStateType.LiquidWithNaturalCrust);

            Assert.AreEqual(95.16, data.MoistureContent);
            Assert.AreEqual(0.325, data.NitrogenFraction);
            Assert.AreEqual(1.29, data.CarbonFraction);
            Assert.AreEqual(0.118, data.PhosphorusFraction);
            Assert.AreEqual(3.25, data.CarbonToNitrogenRatio);
        }


        [TestMethod]
        public void TestIncorrectAnimalType()
        {
            var data = _provider.GetManureCompositionDataByType(AnimalType.Cattle, ManureStateType.Pasture);

            Assert.IsNull(data);
        }

        [TestMethod]
        public void TestIncorrectManureStateType()
        {
            var data = _provider.GetManureCompositionDataByType(AnimalType.Poultry, ManureStateType.DeepBedding);

            Assert.IsNull(data);
        }
    }
}
