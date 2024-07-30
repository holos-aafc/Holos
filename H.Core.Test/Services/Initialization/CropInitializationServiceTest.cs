using H.Core.Models;
using H.Core.Services.Initialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.Initialization.Crops;

namespace H.Core.Test.Services.Initialization
{
    [TestClass]
    public class CropInitializationServiceTest : UnitTestBase
    {
        #region Fields

        private ICropInitializationService _cropsInitializationService;
        private Farm _farm;
        private FieldSystemComponent _field;
        private CropViewItem _crop;
        private GlobalSettings _globalSettings;

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
            _farm = new Farm();
            _farm.PolygonId = 244061;

            _field = new FieldSystemComponent();
            _crop = new CropViewItem();
            _globalSettings = new GlobalSettings();

            _field.CropViewItems.Add(_crop);
            _farm.Components.Add(_field);
            

            _cropsInitializationService = new CropInitializationService();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void InitializeCropDefaultTest()
        {
            _crop.Year = 1999;
            _crop.CropType = CropType.Barley;

            _cropsInitializationService.InitializeCropDefaults(_crop, _farm, _globalSettings);

            Assert.AreEqual(2214, _crop.Yield);
        } 

        #endregion
    }
}
