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
            var result = ShelterbeltCarbonDataProvider.GetData(1990);

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
            var result = ShelterbeltCarbonDataProvider.GetInterpolatedValue(
                treeSpecies: TreeSpecies.WhiteSpruce,
                hardinessZone: HardinessZone.H3b,
                ecodistrictId: 709,
                percentMortality: 50,
                mortalityLow: 30,
                mortalityHigh: 50,
                age: 5,
                column: ShelterbeltCarbonDataProvider.Columns.Dom_Mg_C_km, 
                year: 1990);

            Assert.AreEqual(result, 94.65483856);
        }
    }
}
 