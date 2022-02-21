using H.Core.Enumerations;
using H.Core.Providers.Plants;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Plants
{
    [TestClass]
    public class TillageRatioFactorsForDirectSoilNitrousOxideEmissionsProvider_Table_8Test
    {
        private TillageRatioFactorsForDirectSoilNitrousOxideEmissionsProvider_Table_8 _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new TillageRatioFactorsForDirectSoilNitrousOxideEmissionsProvider_Table_8();
        }

        [TestMethod]
        public void GetRatioFactor()
        {
            var result = _provider.GetRatioFactor(Province.Alberta, TillageType.Intensive);
            Assert.AreEqual(result, 1.0);
        }
    }
}
