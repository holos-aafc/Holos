using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class DefaultAnnualEntericMethaneProvider_Table_29Test
    {
        private DefaultAnnualEntericMethaneProvider_Table_29 _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new DefaultAnnualEntericMethaneProvider_Table_29();
        }

        [TestMethod]
        public void GetAnnualEntericMethaneEmissionRateTest()
        {
            var result = _provider.GetAnnualEntericMethaneEmissionRate(AnimalType.SwineSows);
            Assert.AreEqual(1.5, result);
        }
    }
}
