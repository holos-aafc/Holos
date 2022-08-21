using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using H.Core.Providers.Soil;
using H.Core.Enumerations;

namespace H.Core.Test.Providers.Soil
{
    /// <summary>
    /// Summary description for Table_11_Globally_Calibrated_Model_Parameters_Provider_Test
    /// </summary>
    [TestClass]
    public class Table_11_Globally_Calibrated_Model_Parameters_Provider_Test
    {
        #region Fields

        private static Table_11_Globally_Calibrated_Model_Parameters_Provider _provider;

        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _provider = new Table_11_Globally_Calibrated_Model_Parameters_Provider();
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
        public void GetTillFacReducedValue()
        {
            Table_11_Globally_Calibrated_Model_Parameters_Data data = _provider.GetGloballyCalibratedModelParametersInstance(ModelParameters.TillageModifier, TillageType.Reduced);

            Assert.AreEqual(2.075, data.Value);
        }

        [TestMethod]
        public void GetFractionMetabolicDMActivePool()
        {
            Table_11_Globally_Calibrated_Model_Parameters_Data data = _provider.GetGloballyCalibratedModelParametersInstance(ModelParameters.FractionMetabolicDMActivePool, TillageType.Reduced);

            Assert.AreEqual(0.0719, data.StandardDeviation);
        }

        [TestMethod]
        public void GetDecayRatePassiveSubPool()
        {
            Table_11_Globally_Calibrated_Model_Parameters_Data data = _provider.GetGloballyCalibratedModelParametersInstance(ModelParameters.DecayRatePassive, TillageType.Intensive);
            Assert.AreEqual(0.01, data.MaxValue);
        }

        [TestMethod]
        public void CheckValueNotAvailable()
        {
            Table_11_Globally_Calibrated_Model_Parameters_Data data = _provider.GetGloballyCalibratedModelParametersInstance(ModelParameters.MaximumAvgTemperature, TillageType.NoTill);
            Assert.AreEqual(0.0, data.StandardDeviation);
        }

        [TestMethod]
        public void TestOptimalTemperatureAllTillage()
        {
            Table_11_Globally_Calibrated_Model_Parameters_Data data = _provider.GetGloballyCalibratedModelParametersInstance(ModelParameters.OptimumTemperature, TillageType.Reduced);
            Assert.AreEqual(33.69, data.Value);
        }

        #endregion
    }
}
