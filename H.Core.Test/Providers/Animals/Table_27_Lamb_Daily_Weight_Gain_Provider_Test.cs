using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class Table_27_Lamb_Daily_Weight_Gain_Provider_Test
    {
        private Table_27_Lamb_Daily_Weight_Gain_Provider _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new Table_27_Lamb_Daily_Weight_Gain_Provider();
        }

        [TestMethod]
        public void GetDailyWeightGain()
        {
            var result = _provider.GetDailyWeightGain(0.67);
            Assert.AreEqual(0.233, result);
        }
    }
}
