using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Enumerations;
using H.Core.Providers.AnaerobicDigestion;

namespace H.Core.Test.Providers.AnaerobicDigestion
{
    [TestClass]
    public class Table_50_Solid_Liquid_Separation_Coefficients_Provider_Test
    {
        #region Fields
        Table_50_Solid_Liquid_Separation_Coefficients_Provider _provider;
        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext) { }

        [ClassCleanup]
        public static void ClassCleanup() { }

        [TestInitialize]
        public void TestIntialize()
        {
            _provider = new Table_50_Solid_Liquid_Separation_Coefficients_Provider();
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
