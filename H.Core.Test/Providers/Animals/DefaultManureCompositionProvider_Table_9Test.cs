using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class DefaultManureCompositionProvider_Table_9_Test
    {
        private DefaultManureCompositionProvider_Table_9 _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new DefaultManureCompositionProvider_Table_9();
        }

        [TestMethod]
        public void GetManureCompositionData()
        {            
            var result = _provider.Data.SingleOrDefault(x => x.AnimalType == AnimalType.Beef && 
            x.ManureStateType == ManureStateType.DeepBedding);

            Assert.AreEqual(AnimalType.Beef, result.AnimalType);
            Assert.AreEqual(ManureStateType.DeepBedding, result.ManureStateType);
            Assert.AreEqual(60.08, result.MoistureContent);
            Assert.AreEqual(0.715, result.NitrogenFraction);
            Assert.AreEqual(12.63, result.CarbonFraction);
            Assert.AreEqual(18.79, result.CarbonToNitrogenRatio);
        }
    }
}
