using H.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Providers;
using SharpKml.Base;

namespace H.Infrastructure.Test
{
    [TestClass]
    public class KmlHelpersTest
    {
        private static GeographicDataProvider _geographicDataProvider;
        private static KmlHelpers _kmlHelpers;


        private List<Vector> CoordinateList = new List<Vector>()
        {
            new Vector(52.30845604, -112.5191813),
            new Vector(55.01, -120.34),
            new Vector(56.564167, 133.901111),
        };

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _geographicDataProvider = new GeographicDataProvider();
            _geographicDataProvider.Initialize();
            _kmlHelpers = new KmlHelpers();
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

        [Ignore]
        [TestMethod]
        public void TestGetPolygonFromCoordinateAlberta()
        {
            var coordinate = CoordinateList[0];
            var polygon = _kmlHelpers.GetPolygonFromCoordinate(coordinate.Latitude, coordinate.Longitude);
            var polygonData = _geographicDataProvider.GetGeographicalData(polygon);
            var soilData = polygonData.SoilDataForAllComponentsWithinPolygon;
            var firstSoil = soilData[0];

            Assert.AreEqual(SoilGreatGroupType.SolodizedSolonetz, firstSoil.SoilGreatGroup);
            Assert.AreEqual(SoilTexture.Fine, firstSoil.SoilTexture);
            Assert.AreEqual(18, firstSoil.ProportionOfClayInSoilAsPercentage);
            Assert.AreEqual(5.5, firstSoil.SoilPh);
            Assert.AreEqual(3.5, firstSoil.ProportionOfSoilOrganicCarbon);
        }

        [Ignore]
        [TestMethod]
        public void TestGetPolygonFromCoordinateBC()
        {
            var coordinate = CoordinateList[1];
            var polygon = _kmlHelpers.GetPolygonFromCoordinate(coordinate.Latitude, coordinate.Longitude);
            var polygonData = _geographicDataProvider.GetGeographicalData(polygon);
            var soilData = polygonData.SoilDataForAllComponentsWithinPolygon;
            var firstSoil = soilData[0];
            Assert.AreEqual(Province.BritishColumbia, firstSoil.Province);
            Assert.AreEqual(SoilGreatGroupType.GrayLuvisol, firstSoil.SoilGreatGroup);
            Assert.AreEqual(SoilTexture.Medium, firstSoil.SoilTexture);
            Assert.AreEqual(3, firstSoil.ProportionOfClayInSoilAsPercentage);
            Assert.AreEqual(5.9, firstSoil.SoilPh);
            Assert.AreEqual(0.80, firstSoil.ProportionOfSoilOrganicCarbon);
            Assert.AreEqual(1.31, firstSoil.BulkDensity);
        }

        [Ignore]
        [TestMethod]
        public void TestGetPolygonFromCoordinateInvalid()
        {
            var coordinate = CoordinateList[2];
            var polygon = _kmlHelpers.GetPolygonFromCoordinate(coordinate.Latitude, coordinate.Longitude);
            Assert.AreEqual(0, polygon);
        }
    }
}
