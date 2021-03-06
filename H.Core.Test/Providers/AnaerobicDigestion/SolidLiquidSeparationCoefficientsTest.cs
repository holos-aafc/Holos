using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Enumerations;
using H.Core.Providers.AnaerobicDigestion;

namespace H.Core.Test.Providers.AnaerobicDigestion
{
    [TestClass]
    public class SolidLiquidSeparationCoefficientsTest
    {
        #region Fields
        SolidLiquidSeparationCoefficientsProvider_Table_49 _provider;
        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext) { }

        [ClassCleanup]
        public static void ClassCleanup() { }

        [TestInitialize]
        public void TestIntialize()
        {
            _provider = new SolidLiquidSeparationCoefficientsProvider_Table_49();
        }

        [TestCleanup]
        public void TestCleanup() { }
        #endregion

        #region Tests

        [TestMethod]
        public void GetRawMaterialCoefficientsSymbol()
        {
            var data = _provider.GetSolidLiquidSeparationCoefficientInstance(DigestateParameters.RawMaterial);
            Assert.AreEqual(SeparationCoefficients.FractionRawMaterials, data.SeparationCoefficient);
        }

        [TestMethod]
        public void GetVolatileSolidsCentrifuge()
        {
            var data = _provider.GetSolidLiquidSeparationCoefficientInstance(DigestateParameters.VolatileSolids);
            Assert.AreEqual(0.83, data.Centrifuge);
        }

        [TestMethod]
        public void GetOrganicNitrogenBeltPress()
        {
            var data = _provider.GetSolidLiquidSeparationCoefficientInstance(DigestateParameters.OrganicNitrogen);
            Assert.AreEqual(0.19, data.BeltPress);
        }

        [TestMethod]
        public void TestIncorrectParameter()
        {
            var data = _provider.GetSolidLiquidSeparationCoefficientInstance(DigestateParameters.TotalCarbon);
            Assert.AreEqual(0, data.Centrifuge);
            Assert.AreEqual(0, data.BeltPress);
        }

        #endregion
    }
}
