using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class DefaultLambsDailyWeightGainProvider_Table_32Test
    {
        private DefaultLambsDailyWeightGainProvider_Table_23 _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new DefaultLambsDailyWeightGainProvider_Table_23();
        }

        [TestMethod]
        public void GetDailyWeightGain()
        {
            var result = _provider.GetDailyWeightGain(0.67);
            Assert.AreEqual(0.233, result);
        }
    }
}
