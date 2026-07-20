using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Soil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Soil
{
    [TestClass]
    public class Table_13_Soil_N2O_Emission_Factors_Provider_Test
    {
        private Table_13_Soil_N2O_Emission_Factors_Provider _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new Table_13_Soil_N2O_Emission_Factors_Provider();
        }

        [TestMethod]
        public void GetRatioFactor()
        {
            var cropViewItem = new CropViewItem();
            cropViewItem.TillageType = TillageType.Intensive;
            var result = _provider.GetFactorForTillagePractice(Region.EasternCanada, cropViewItem);
            Assert.AreEqual(result, 1.0);
        }

        #region GetReductionFactorBasedOnApplicationMethod

        // RF_AM values (Table 16) per the meta-analysis "effect of improved nitrogen management
        // practices on nitrous oxide emissions in Canadian studies". The factor is used as a MULTIPLIER
        // on the N2O emission factor, so each value is the remaining fraction (1 - reduction):
        //   slow/controlled-release -6.41% -> 0.9359; nitrification inhibitor -22.9% -> 0.771;
        //   urease inhibitor +9.6% -> 1.096; dual inhibitor -15.1% -> 0.849.
        // Any value not explicitly mapped (None, Custom, or an out-of-range enum) resolves to 1.0.

        [TestMethod]
        public void GetReductionFactorBasedOnApplicationMethod_ControlledRelease_Returns0Point9359()
        {
            Assert.AreEqual(
                0.9359,
                _provider.GetReductionFactorBasedOnApplicationMethod(SoilReductionFactors.ControlledRelease),
                1e-9);
        }

        [TestMethod]
        public void GetReductionFactorBasedOnApplicationMethod_NitrificationInhibitor_Returns0Point771()
        {
            Assert.AreEqual(
                0.771,
                _provider.GetReductionFactorBasedOnApplicationMethod(SoilReductionFactors.NitrificationInhibitor),
                1e-9);
        }

        [TestMethod]
        public void GetReductionFactorBasedOnApplicationMethod_UreaseInhibitor_Returns1Point096()
        {
            Assert.AreEqual(
                1.096,
                _provider.GetReductionFactorBasedOnApplicationMethod(SoilReductionFactors.UreaseInhibitor),
                1e-9);
        }

        [TestMethod]
        public void GetReductionFactorBasedOnApplicationMethod_NitrificationAndUreaseInhibitor_Returns0Point849()
        {
            Assert.AreEqual(
                0.849,
                _provider.GetReductionFactorBasedOnApplicationMethod(SoilReductionFactors.NitrificationAndUreaseInhibitor),
                1e-9);
        }

        [TestMethod]
        public void GetReductionFactorBasedOnApplicationMethod_None_ReturnsDefaultOfOne()
        {
            Assert.AreEqual(
                1.0,
                _provider.GetReductionFactorBasedOnApplicationMethod(SoilReductionFactors.None),
                1e-9);
        }

        [TestMethod]
        public void GetReductionFactorBasedOnApplicationMethod_Custom_IsUnmapped_ReturnsDefaultOfOne()
        {
            // Custom has no explicit case, so it falls through to the default branch.
            Assert.AreEqual(
                1.0,
                _provider.GetReductionFactorBasedOnApplicationMethod(SoilReductionFactors.Custom),
                1e-9);
        }

        [TestMethod]
        public void GetReductionFactorBasedOnApplicationMethod_UnknownValue_ReturnsDefaultOfOne()
        {
            // An out-of-range enum value hits the default branch (logs a trace error, returns 1.0).
            Assert.AreEqual(
                1.0,
                _provider.GetReductionFactorBasedOnApplicationMethod((SoilReductionFactors)(-1)),
                1e-9);
        }

        #endregion
    }
}
