using H.Core.Enumerations;
using H.Core.Providers.AnaerobicDigestion;
using H.Core.Providers.Climate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.AnaerobicDigestion
{
    [TestClass]
    public class Table_46_Biogas_Methane_Production_Parameters_Provider_Test
    {
        #region Fields
        private static Table_46_Biogas_Methane_Production_Parameters_Provider _provider;
        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _provider = new Table_46_Biogas_Methane_Production_Parameters_Provider();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void GetBiogasMethanePotentialManureType()
        {
            BiogasAndMethaneProductionParametersData data = _provider.GetBiogasMethaneProductionInstance(AnimalType.Beef, BeddingMaterialType.None);
            Assert.AreEqual(308, data.BioMethanePotential);
        }

        [TestMethod]
        public void GetMethaneFractionManureType()
        {
            BiogasAndMethaneProductionParametersData data = _provider.GetBiogasMethaneProductionInstance(AnimalType.Horses, BeddingMaterialType.Straw);
            Assert.AreEqual(0.6, data.MethaneFraction);
        }

        [TestMethod]
        public void GetMethaneFractionFarmResidueType()
        {
            BiogasAndMethaneProductionParametersData data = _provider.GetBiogasMethaneProductionInstance(FarmResidueType.VegetableOil);
            Assert.AreEqual(0.8, data.MethaneFraction);
        }

        [TestMethod]
        public void GetVolitileSolidManureType()
        {
            BiogasAndMethaneProductionParametersData data = _provider.GetBiogasMethaneProductionInstance(AnimalType.Turkeys, BeddingMaterialType.None);
            Assert.AreEqual(0, data.VolatileSolids);
        }

        [TestMethod]
        public void CheckIncorrectManureType()
        {
            BiogasAndMethaneProductionParametersData data = _provider.GetBiogasMethaneProductionInstance(AnimalType.Llamas, BeddingMaterialType.None);
            Assert.AreEqual(0, data.VolatileSolids);
        }

        [TestMethod]
        public void CheckIncorrectBeddingMaterialType()
        {
            BiogasAndMethaneProductionParametersData data = _provider.GetBiogasMethaneProductionInstance(AnimalType.Turkeys, BeddingMaterialType.WoodChip);
            Assert.AreEqual(0, data.VolatileSolids);
        }

        [TestMethod]
        public void GetTotalSolidsValueManureType()
        {
            BiogasAndMethaneProductionParametersData data = _provider.GetBiogasMethaneProductionInstance(AnimalType.Goats, BeddingMaterialType.None);
            Assert.AreEqual(0, data.TotalSolids);
        }

        #endregion
    }
}
