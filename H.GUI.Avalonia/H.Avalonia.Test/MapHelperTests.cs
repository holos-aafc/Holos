using H.Avalonia.Infrastructure;
using Mapsui;
using SharpKml.Dom;

namespace H.Avalonia.Test
{
    [TestClass]
    public class MapHelperTests
    {
        private static MapHelpers? _mapHelpers;
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _mapHelpers = new MapHelpers();
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
        public async Task TestGetPointFromFullAddress()
        {
            var address = "5403 1 Ave S, Lethbridge, AB T1J 4B1";
            var point = await _mapHelpers?.GetLocationFromAddressAsync(address)!;
            Assert.AreEqual("49.7", $"{point.latitude:0.0}");
            Assert.AreEqual("-112.8", $"{point.longitude:0.0}");
        }

        [TestMethod]
        public async Task TestGetPointFromPostalCode()
        {
            var address = "T1J4B1";
            var point = await _mapHelpers?.GetLocationFromAddressAsync(address)!;
            Assert.AreEqual("49.69", $"{point.latitude:0.00}");
            Assert.AreEqual("-112.83", $"{point.longitude:0.00}");
        }

        [TestMethod]
        public async Task TestGetPointFromCity()
        {
            var address = "lethbridge";
            var point = await _mapHelpers?.GetLocationFromAddressAsync(address)!;
            Assert.AreEqual(-112.83, Math.Round(point.longitude, 2));
            Assert.AreEqual(49.69, Math.Round(point.latitude, 2));
        }

        [TestMethod]
        public async Task TestGetAddressFromCoordinate()
        {
            var address = await _mapHelpers?.GetAddressFromLocationAsync(longitude: -112.85, latitude: 49.70)!;
            Assert.AreEqual("102 Scenic Dr N, Lethbridge, AB T1H 5L9, Canada", address);
        }

        [TestMethod]
        public void TestLongLatToSphericalMercator()
        {
            var point = _mapHelpers?.ConvertLatLongtToSphericalMercator(45, -100);
            Assert.AreEqual("5621521.49", $"{point?.y:0.00}");
            Assert.AreEqual("-11131949.08", $"{point?.x:0.00}");
        }

        [TestMethod]
        public void TestSphericalMercatorToLongLat()
        {
            var point = _mapHelpers?.ConvertSphericalMercatorToCoordinate(new MPoint(-11131949.08, 5621521.49));
            Assert.AreEqual("45", $"{point?.latitude:0}");
            Assert.AreEqual("-100", $"{point?.longitude:0}");
        }
    }
}