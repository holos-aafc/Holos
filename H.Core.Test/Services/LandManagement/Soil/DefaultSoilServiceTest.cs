using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using H.Core.Models;
using H.Core.Providers;
using H.Core.Providers.Soil;
using H.Core.Services.LandManagement.Soil;
using Moq;

namespace H.Core.Test.Services.LandManagement.Soil
{
    [TestClass]
    public class DefaultSoilServiceTest
    {
        #region Fields

        private ISoilService _sut;
        private Mock<IGeographicDataProvider> _mockGeographicDataProvider;

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
            _mockGeographicDataProvider = new Mock<IGeographicDataProvider>();

            _sut = new DefaultSoilService(_mockGeographicDataProvider.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void Test()
        {
            var farm = new Farm();

            _mockGeographicDataProvider.Setup(x => x.GetGeographicalData(It.IsAny<int>())).Returns(new GeographicData() {SoilDataForAllComponentsWithinPolygon = new List<SoilData>() {new SoilData()}});

            _sut.SetGeographicalData(farm);
        }

        #endregion
    }
}
