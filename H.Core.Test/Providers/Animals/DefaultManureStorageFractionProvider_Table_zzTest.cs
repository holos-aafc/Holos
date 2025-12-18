using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class DefaultManureStorageFractionProvider_Table_zzTest
    {
        private DefaultManureStorageFractionProvider_Table_zz _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new DefaultManureStorageFractionProvider_Table_zz();
        }

        [TestMethod]
        public void GetDataByManureStorageType()
        {
            var result = _provider.GetDataByManureStorageType(ManureStateType.Composted);
            Assert.AreEqual(ManureStateType.Composted, result.StorageType);
            Assert.AreEqual(0.46, result.FractionMineralizedAsTan);
            Assert.AreEqual(0, result.FractionImmobilized);
            Assert.AreEqual(0.250, result.FractionNitrified);
        }
    }
}
