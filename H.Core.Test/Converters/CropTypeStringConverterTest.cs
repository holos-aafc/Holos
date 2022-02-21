using System;
using H.Core.Converters;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Converters
{
    /// <summary>
    /// Summary description for CropTypeStringConverterTest
    /// </summary>
    [TestClass]
    public class CropTypeStringConverterTest
    {
        private CropTypeStringConverter converter;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            converter = new CropTypeStringConverter();
        }

        [TestMethod]
        public void ConvertReturnsNoSelectedException()
        {
            var result = converter.Convert("Quebec");
            
            Assert.AreEqual(CropType.NotSelected, result);
        }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        //[TestMethod]
        //public void ConvertReturnsAll()
        //{
        //    Assert.AreEqual(CropType.All, converter.Convert("All"));
        //}

        //[TestMethod]
        //public void ConvertReturnsBarley()
        //{
        //    Assert.AreEqual(CropType.Barley, converter.Convert("Barley"));
        //}

        //[TestMethod]
        //public void ConvertReturnsBarleySilage()
        //{
        //    Assert.AreEqual(CropType.BarleySilage, converter.Convert("BarleySilage"));
        //}

        //[TestMethod]
        //public void ConvertReturnsBuckwheat()
        //{
        //    Assert.AreEqual(CropType.Buckwheat, converter.Convert("Buckwheat"));
        //}

        //[TestMethod]
        //public void ConvertReturnsCanarySeed()
        //{
        //    Assert.AreEqual(CropType.CanarySeed, converter.Convert("CanarySeed"));
        //}

        //[TestMethod]
        //public void ConvertReturnsCanola()
        //{
        //    Assert.AreEqual(CropType.Canola, converter.Convert("Canola"));
        //}

        //[TestMethod]
        //public void ConvertReturnsChickpeas()
        //{
        //    Assert.AreEqual(CropType.Chickpeas, converter.Convert("Chickpeas"));
        //}

        //[TestMethod]
        //public void ConvertReturnsColouredWhiteFabaBeans()
        //{
        //    Assert.AreEqual(CropType.ColouredWhiteFabaBeans, converter.Convert("ColouredWhiteFabaBeans"));
        //}

        //[TestMethod]
        //public void ConvertReturnsDryPeas()
        //{
        //    Assert.AreEqual(CropType.DryPeas, converter.Convert("DryPeas"));
        //}

        //[TestMethod]
        //public void ConvertReturnsDurum()
        //{
        //    Assert.AreEqual(CropType.Durum, converter.Convert("Durum"));
        //}

        //[TestMethod]
        //public void ConvertReturnsFallow()
        //{
        //    Assert.AreEqual(CropType.Fallow, converter.Convert("Fallow"));
        //}

        //[TestMethod]
        //public void ConvertReturnsFlax()
        //{
        //    Assert.AreEqual(CropType.Flax, converter.Convert("Flax"));
        //}

        //[TestMethod]
        //public void ConvertReturnsFodderCorn()
        //{
        //    Assert.AreEqual(CropType.FodderCorn, converter.Convert("FodderCorn"));
        //}

        //[TestMethod]
        //public void ConvertReturnsForage()
        //{
        //    Assert.AreEqual(CropType.Forage, converter.Convert("Forage"));
        //}

        //[TestMethod]
        //public void ConvertReturnsGrainSorghum()
        //{
        //    Assert.AreEqual(CropType.GrainSorghum, converter.Convert("GrainSorghum"));
        //}

        //[TestMethod]
        //public void ConvertReturnsGrainCorn()
        //{
        //    Assert.AreEqual(CropType.GrainCorn, converter.Convert("GrainCorn"));
        //}

        //[TestMethod]
        //public void ConvertReturnsHayGrass()
        //{
        //    Assert.AreEqual(CropType.HayGrass, converter.Convert("HayGrass"));
        //}

        //[TestMethod]
        //public void ConvertReturnsHayLegume()
        //{
        //    Assert.AreEqual(CropType.HayLegume, converter.Convert("HayLegume"));
        //}

        //[TestMethod]
        //public void ConvertReturnsHayMixed()
        //{
        //    Assert.AreEqual(CropType.HayMixed, converter.Convert("HayMixed"));
        //}

        //[TestMethod]
        //public void ConvertReturnsHayAndForageSeed()
        //{
        //    Assert.AreEqual(CropType.HayAndForageSeed, converter.Convert("HayAndForageSeed"));
        //}

        //[TestMethod]
        //public void ConvertReturnsLentils()
        //{
        //    Assert.AreEqual(CropType.Lentils, converter.Convert("Lentils"));
        //}

        //[TestMethod]
        //public void ConvertReturnsMixedGrains()
        //{
        //    Assert.AreEqual(CropType.MixedGrains, converter.Convert("MixedGrains"));
        //}

        //[TestMethod]
        //public void ConvertReturnsMustard()
        //{
        //    Assert.AreEqual(CropType.Mustard, converter.Convert("Mustard"));
        //}

        //[TestMethod]
        //public void ConvertReturnsOats()
        //{
        //    Assert.AreEqual(CropType.Oats, converter.Convert("Oats"));
        //}

        //[TestMethod]
        //public void ConvertReturnsOilseeds()
        //{
        //    Assert.AreEqual(CropType.Oilseeds, converter.Convert("Oilseeds"));
        //}

        //[TestMethod]
        //public void ConvertReturnsPasture()
        //{
        //    Assert.AreEqual(CropType.Pasture, converter.Convert("Pasture"));
        //}

        //[TestMethod]
        //public void ConvertReturnsPotatoes()
        //{
        //    Assert.AreEqual(CropType.Potatoes, converter.Convert("Potatoes"));
        //}

        //[TestMethod]
        //public void ConvertReturnsPulseCrops()
        //{
        //    Assert.AreEqual(CropType.PulseCrops, converter.Convert("PulseCrops"));
        //}

        //[TestMethod]
        //public void ConvertReturnsRye()
        //{
        //    Assert.AreEqual(CropType.Rye, converter.Convert("Rye"));
        //}

        //[TestMethod]
        //public void ConvertReturnsSafflower()
        //{
        //    Assert.AreEqual(CropType.Safflower, converter.Convert("Safflower"));
        //}

        //[TestMethod]
        //public void ConvertReturnsSmallGrainCereals()
        //{
        //    Assert.AreEqual(CropType.SmallGrainCereals, converter.Convert("SmallGrainCereals"));
        //}

        //[TestMethod]
        //public void ConvertReturnsSoybeans()
        //{
        //    Assert.AreEqual(CropType.Soybeans, converter.Convert("Soybeans"));
        //}

        //[TestMethod]
        //public void ConvertReturnsSpringWheat()
        //{
        //    Assert.AreEqual(CropType.SpringWheat, converter.Convert("SpringWheat"));
        //}

        //[TestMethod]
        //public void ConvertReturnsSunflowerSeed()
        //{
        //    Assert.AreEqual(CropType.SunflowerSeed, converter.Convert("SunflowerSeed"));
        //}

        //[TestMethod]
        //public void ConvertReturnsTriticale()
        //{
        //    Assert.AreEqual(CropType.Triticale, converter.Convert("Triticale"));
        //}

        //[TestMethod]
        //public void ConvertReturnsUndersownBarley()
        //{
        //    Assert.AreEqual(CropType.UndersownBarley, converter.Convert("UndersownBarley"));
        //}

        //[TestMethod]
        //public void ConvertReturnsWheatBolinder()
        //{
        //    Assert.AreEqual(CropType.WheatBolinder, converter.Convert("WheatBolinder"));
        //}

        //[TestMethod]
        //public void ConvertReturnsWheatGan()
        //{
        //    Assert.AreEqual(CropType.WheatGan, converter.Convert("WheatGan"));
        //}

        //[TestMethod]
        //public void ConvertReturnsWinterWheat()
        //{
        //    Assert.AreEqual(CropType.WinterWheat, converter.Convert("WinterWheat"));
        //}
    }
}