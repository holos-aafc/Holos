using H.Core.Providers.Climate;

namespace H.Avalonia.Test
{
    [TestClass]
    public class NasaClimateProviderTest
    {
        private static NasaClimateProvider _nasaClimateProvider;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _nasaClimateProvider = new NasaClimateProvider();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public void TestTotalPET()
        {
            var totalPET = _nasaClimateProvider.GetTotalPET(1988, 52.466667, -113.75);
            Assert.AreEqual(582.55, Math.Round(totalPET, 2));
            totalPET = _nasaClimateProvider.GetTotalPET(1994, 52.38274971, -114.6030042);
            Assert.AreEqual(553.27, Math.Round(totalPET, 2));
        }

        [TestMethod]
        public void TestTotalPPT()
        {
            var totalPPT = _nasaClimateProvider.GetTotalPPT(1984, 50.26666667, -107.7333333);
            Assert.AreEqual(245.73, Math.Round(totalPPT, 2));

            totalPPT = _nasaClimateProvider.GetTotalPPT(1996, 49.833333, -99.95);
            Assert.AreEqual(507.96, Math.Round(totalPPT, 2));

        }

        [TestMethod]
        public void TestMonthlyPPT()
        {
            var rangePPT = _nasaClimateProvider.GetMonthlyPPT(2000, 121, 273, 52.466667, -113.75);
            Assert.AreEqual(395.13, Math.Round(rangePPT, 2));


            rangePPT = _nasaClimateProvider.GetMonthlyPPT(1996, 121, 273, 45.916667, -66.6);
            Assert.AreEqual(551.2, Math.Round(rangePPT, 2));
        }
    }
}