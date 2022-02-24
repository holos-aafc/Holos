using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Providers.Shelterbelt;

namespace H.Core.Test.Providers.Shelterbelt
{
    [TestClass]
    public class DomProviderTest
    {
        [TestMethod]
        public void TestAllTreeSpeciesGetReturnedFromProvider()
        {
            var result = ShelterbeltCarbonDataProvider.GetData();

            Assert.IsTrue(result.Any(x => x.Species == TreeSpecies.Caragana));
            Assert.IsTrue(result.Any(x => x.Species == TreeSpecies.GreenAsh));
            Assert.IsTrue(result.Any(x => x.Species == TreeSpecies.HybridPoplar));
            Assert.IsTrue(result.Any(x => x.Species == TreeSpecies.ManitobaMaple));
            Assert.IsTrue(result.Any(x => x.Species == TreeSpecies.ScotsPine));
            Assert.IsTrue(result.Any(x => x.Species == TreeSpecies.WhiteSpruce));
        }

        [TestMethod]
        public void GetColumn()
        {
            var result = ShelterbeltCarbonDataProvider.GetLookupValue(
                treeSpecies: TreeSpecies.WhiteSpruce,
                ecodistrictId: 689,
                percentMortality: 50,
                mortalityLow: 30,
                mortalityHigh: 50,
                age: 5,
                column: ShelterbeltCarbonDataProviderColumns.Dom_Mg_C_km);

            Assert.AreEqual(result, 99.79055023);
        }
    }
}
 