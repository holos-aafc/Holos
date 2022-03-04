using System.Linq;
using System.Threading;
using H.Core.Enumerations;
using H.Core.Providers.Fertilizer;
using H.Core.Providers.Soil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Soil
{
    [TestClass]
    public class SmallAreaYieldProviderTest
    {
        #region Fields

        private SmallAreaYieldProvider _provider;
        private ManualResetEvent _manualResetEvent;

        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new SmallAreaYieldProvider();

            _manualResetEvent = new ManualResetEvent(false);

            _provider.FinishedReadingFile += (sender, args) =>
            {
                _manualResetEvent.Set();
            };

            _provider.InitializeAsync();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void GetDataReturnsNonEmptyList()
        {
            // Wait until the async reading of the file has completed
            _manualResetEvent.WaitOne(5000, false);

            var result = _provider.GetData();

            Assert.IsTrue(result.Count() > 1000000);
        }

        [TestMethod]
        public void GetDataReturnsNonNull()
        {
            const int year = 2018;
            const Province province = Province.PrinceEdwardIsland;
            const int polygon = 538001;

            // Wait until the async reading of the file has completed
            _manualResetEvent.WaitOne(5000, false);

            Assert.AreEqual(3522, _provider.GetData(year, polygon, CropType.Barley, province).Yield);
            Assert.AreEqual(2231, _provider.GetData(year, polygon, CropType.Canola, province).Yield);
            Assert.AreEqual(8737, _provider.GetData(year, polygon, CropType.GrainCorn, province).Yield);
            Assert.AreEqual(1400, _provider.GetData(year, polygon, CropType.FlaxSeed, province).Yield);
            Assert.AreEqual(2439, _provider.GetData(year, polygon, CropType.Oats, province).Yield);
            Assert.AreEqual(2755, _provider.GetData(year, polygon, CropType.Soybeans, province).Yield);
            Assert.AreEqual(3758, _provider.GetData(year, polygon, CropType.Durum, province).Yield);
            Assert.AreEqual(3400, _provider.GetData(year, polygon, CropType.SpringWheat, province).Yield);
            Assert.AreEqual(4486, _provider.GetData(year, polygon, CropType.WinterWheat, province).Yield);
            Assert.AreEqual(1180, _provider.GetData(year, polygon, CropType.Buckwheat, province).Yield);
            Assert.AreEqual(1390, _provider.GetData(year, polygon, CropType.CanarySeed, province).Yield);
            Assert.AreEqual(513, _provider.GetData(year, polygon, CropType.Caraway, province).Yield);
            Assert.AreEqual(36250, _provider.GetData(year, polygon, CropType.SilageCorn, province).Yield);
            Assert.AreEqual(2960, _provider.GetData(year, polygon, CropType.FabaBeans, province).Yield);
            Assert.AreEqual(1395, _provider.GetData(year, polygon, CropType.Lentils, province).Yield);
            Assert.AreEqual(3284, _provider.GetData(year, polygon, CropType.MixedGrains, province).Yield);
            Assert.AreEqual(880, _provider.GetData(year, polygon, CropType.Mustard, province).Yield);
            Assert.AreEqual(3500, _provider.GetData(year, polygon, CropType.DryPeas, province).Yield);
            Assert.AreEqual(2464, _provider.GetData(year, polygon, CropType.Rye, province).Yield);
            Assert.AreEqual(2535, _provider.GetData(year, polygon, CropType.FallRye, province).Yield);
            Assert.AreEqual(2464, _provider.GetData(year, polygon, CropType.SpringRye, province).Yield);
            Assert.AreEqual(0, _provider.GetData(year, polygon, CropType.Safflower, province).Yield);
            Assert.AreEqual(75595, _provider.GetData(year, polygon, CropType.SugarBeets, province).Yield);
            Assert.AreEqual(2132, _provider.GetData(year, polygon, CropType.Sunflower, province).Yield);
            Assert.AreEqual(4480, _provider.GetData(year, polygon, CropType.TamePasture, province).Yield);
            Assert.AreEqual(3001, _provider.GetData(year, polygon, CropType.Triticale, province).Yield);
            Assert.AreEqual(3758, _provider.GetData(year, polygon, CropType.Wheat, province).Yield);
            Assert.AreEqual(31965, _provider.GetData(year, polygon, CropType.Potatoes, province).Yield);
            Assert.AreEqual(1300, _provider.GetData(year, polygon, CropType.BeansDryField, province).Yield);
        }

        [TestMethod]
        public void GetDataReturnsNull()
        {
            const int year = 2018;
            const Province province = Province.PrinceEdwardIsland;
            const int polygon = 538001;

            // Wait until the async reading of the file has completed
            _manualResetEvent.WaitOne(5000, false);

            var result = _provider.GetData(1900, polygon, CropType.Barley, province);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetDataReturnsNullWhenPolygonIsNotFound()
        {
            const int year = 2008;
            const Province province = Province.PrinceEdwardIsland;
            const int polygon = 538001;

            // Wait until the async reading of the file has completed
            _manualResetEvent.WaitOne(5000, false);

            var result = _provider.GetData(1900, polygon, CropType.Barley, province);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetDataReturnsNullForPerennial()
        {
            const int year = 2018;
            const Province province = Province.PrinceEdwardIsland;
            const int polygon = 538001;
            const CropType cropType = CropType.TameGrass;

            // Wait until the async reading of the file has completed
            _manualResetEvent.WaitOne(5000, false);

            var result = _provider.GetData(year, polygon, cropType, province);

            Assert.IsTrue(result.Yield > 0);
        }

        #endregion
    }
}