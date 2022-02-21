using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Providers.Climate;

namespace H.Core.Test.Providers.Climate
{
    [TestClass]
    public class ClimateProviderTest
    {
        #region Fields

        private ClimateProvider _climateProvider;

        #endregion

        [TestInitialize]
        public void Initialize()
        {
            _climateProvider = new ClimateProvider();
        }


        #region Tests

        [TestMethod]
        public void ChangeNormals()
        {
            var climateData = _climateProvider.Get(50.99, -80.00, Enumerations.TimeFrame.NineteenEightyToNineteenNinety);

            // If Nasa service is offline we will get null and a failing test. Return from test in this case since we need data to test normals
            if (climateData == null)
            {
                return;
            }

            var changedClimateData = _climateProvider.AdjustClimateNormalsForTimeFrame(climateData.DailyClimateData, Enumerations.TimeFrame.NineteenNinetyToTwoThousand);

            Assert.AreNotEqual(climateData.EvapotranspirationData, changedClimateData.EvapotranspirationData);
            Assert.AreNotEqual(climateData.PrecipitationData, changedClimateData.PrecipitationData);
            Assert.AreNotEqual(climateData.TemperatureData, changedClimateData.TemperatureData);
        }

        #endregion
    }
}
