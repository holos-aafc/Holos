using H.Core.Enumerations;
using H.Core.Providers.Animals.Table_69;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class Table_69_Volatilization_Fractions_From_Land_Applied_Dairy_Manure_Provider_Test
    {
        #region Fields
        
        private IVolatilizationFractionsFromLandAppliedManureProvider _sut; 

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
            _sut = new Table_69_Volatilization_Fractions_From_Land_Applied_Dairy_Manure_Provider();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests
        
        [TestMethod]
        public void GetDataReturnsZeroWhenIncorrectAnimalTypeIsUsed()
        {
            var result = _sut.GetData(AnimalType.Swine, Province.Alberta, 2000);

            Assert.AreEqual(0, result.ImpliedEmissionFactor);
        }

        [TestMethod]
        public void GetDataReturnsNonZeroWhenForProvinceAndYear()
        {
            var result = _sut.GetData(AnimalType.Dairy, Province.Quebec, 2017);

            Assert.AreEqual(0.15, result.ImpliedEmissionFactor);
        } 

        #endregion
    }
}
