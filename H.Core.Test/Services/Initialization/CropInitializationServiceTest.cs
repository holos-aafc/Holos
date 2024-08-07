using H.Core.Models;
using H.Core.Services.Initialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using H.Core.Calculators.Carbon;
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

            _cropsInitializationService.InitializeCrop(_crop, _farm, _globalSettings);

            Assert.AreEqual(2214, _crop.Yield);
        }

        [TestMethod]
        public void InitializeAvailableSoilTypesDoesNotAddOrganicType()
        {
            var soilData = base.GetTestSoilData();
            soilData.SoilFunctionalCategory = SoilFunctionalCategory.Organic;

            _farm.GeographicData = base.GetTestGeographicData();
            _farm.GeographicData.SoilDataForAllComponentsWithinPolygon.Clear();
            _farm.GeographicData.SoilDataForAllComponentsWithinPolygon.Add(soilData);
            
            var field = base.GetTestFieldComponent();

            _cropsInitializationService.InitializeAvailableSoilTypes(_farm, field);

            Assert.AreEqual(0, field.SoilDataAvailableForField.Count);
        }

        [TestMethod]
        public void InitializeAvailableSoilTypesAddSoilToTypesAvailable()
        {
            var soilData = base.GetTestSoilData();
            soilData.SoilFunctionalCategory = SoilFunctionalCategory.Black;

            _farm.GeographicData = base.GetTestGeographicData();
            _farm.GeographicData.SoilDataForAllComponentsWithinPolygon.Clear();
            _farm.GeographicData.SoilDataForAllComponentsWithinPolygon.Add(soilData);

            var field = base.GetTestFieldComponent();

            _cropsInitializationService.InitializeAvailableSoilTypes(_farm, field);

            Assert.AreEqual(1, field.SoilDataAvailableForField.Count);
        }

        [TestMethod]
        public void InitializeAvailableSoilTypesAddsMultipleSoilsToTypesAvailable()
        {
            var soilData = base.GetTestSoilData();
            soilData.SoilFunctionalCategory = SoilFunctionalCategory.Black;
            soilData.SoilGreatGroup = SoilGreatGroupType.BrownChernozem;

            var soilData2 = base.GetTestSoilData();
            soilData2.SoilFunctionalCategory = SoilFunctionalCategory.Black;
            soilData2.SoilGreatGroup = SoilGreatGroupType.BlackChernozem;

            _farm.GeographicData = base.GetTestGeographicData();
            _farm.GeographicData.SoilDataForAllComponentsWithinPolygon.Clear();
            _farm.GeographicData.SoilDataForAllComponentsWithinPolygon.Add(soilData);
            _farm.GeographicData.SoilDataForAllComponentsWithinPolygon.Add(soilData2);

            var field = base.GetTestFieldComponent();

            _cropsInitializationService.InitializeAvailableSoilTypes(_farm, field);

            Assert.AreEqual(2, field.SoilDataAvailableForField.Count);
        }

        [TestMethod]
        public void InitializeDefaultSoilForFieldSetsNonNullValueForSoilData()
        {
            var field = base.GetTestFieldComponent();
            field.SoilData = null;

            var soilData = base.GetTestSoilData();
            soilData.SoilFunctionalCategory = SoilFunctionalCategory.Black;
            soilData.SoilGreatGroup = SoilGreatGroupType.BrownChernozem;
            _farm.GeographicData = base.GetTestGeographicData();
            _farm.GeographicData.SoilDataForAllComponentsWithinPolygon.Clear();
            _farm.GeographicData.SoilDataForAllComponentsWithinPolygon.Add(soilData);

            _cropsInitializationService.InitializeDefaultSoilForField(_farm, field);

            Assert.IsNotNull(field.SoilData);
        }

        [TestMethod]
        public void InitializeManureApplicationMethodForBeefCattleTypes()
        {
            var viewItem = base.GetTestCropViewItem();
            var manureApplication = base.GetTestDairyCattleManureApplicationViewItemUsingImportedManure();
            manureApplication.AnimalType = AnimalType.BeefBackgrounder;

            var validApplicationTypes = new List<ManureApplicationTypes>();

            viewItem.ManureApplicationViewItems.Add(manureApplication);

            _cropsInitializationService.InitializeManureApplicationMethod(viewItem, manureApplication, validApplicationTypes);

            Assert.AreEqual(ManureApplicationTypes.TilledLandSolidSpread, manureApplication.ManureApplicationMethod);
            Assert.AreEqual(2, manureApplication.AvailableManureApplicationTypes.Count);
            Assert.AreEqual(ManureApplicationTypes.UntilledLandSolidSpread, manureApplication.AvailableManureApplicationTypes[0]);
            Assert.AreEqual(ManureApplicationTypes.TilledLandSolidSpread, manureApplication.AvailableManureApplicationTypes[1]);
        }

        [TestMethod]
        public void InitializeManureApplicationMethodForDairyCattleTypes()
        {
            var viewItem = base.GetTestCropViewItem();
            var manureApplication = base.GetTestDairyCattleManureApplicationViewItemUsingImportedManure();

            var validApplicationTypes = new List<ManureApplicationTypes>();

            viewItem.ManureApplicationViewItems.Add(manureApplication);

            _cropsInitializationService.InitializeManureApplicationMethod(viewItem, manureApplication, validApplicationTypes);

            Assert.AreEqual(ManureApplicationTypes.TilledLandSolidSpread, manureApplication.ManureApplicationMethod);
            Assert.AreEqual(6, manureApplication.AvailableManureApplicationTypes.Count);
        }

        [TestMethod]
        public void InitializeFertilizerApplicationMethodForAnnuals()
        {
            var viewItem = base.GetTestCropViewItem();
            var fertilizerApplication = base.GetTestFertilizerApplicationViewItem();

            _cropsInitializationService.InitializeFertilizerApplicationMethod(viewItem, fertilizerApplication);

            Assert.AreEqual(FertilizerApplicationMethodologies.IncorporatedOrPartiallyInjected, fertilizerApplication.FertilizerApplicationMethodology);
        }

        [TestMethod]
        public void InitializeFertilizerApplicationMethodForPerennials()
        {
            var viewItem = base.GetTestCropViewItem();
            viewItem.CropType = CropType.TameGrass;

            var fertilizerApplication = base.GetTestFertilizerApplicationViewItem();

            _cropsInitializationService.InitializeFertilizerApplicationMethod(viewItem, fertilizerApplication);

            Assert.AreEqual(FertilizerApplicationMethodologies.Broadcast, fertilizerApplication.FertilizerApplicationMethodology);
        }

        [TestMethod]
        public void InitializeAmountOfBlendedProductTest()
        {
            var viewItem = base.GetTestCropViewItem();
            viewItem.CropType = CropType.Wheat;

            _cropsInitializationService.InitializeBiomassCoefficients(viewItem, _farm);
            _cropsInitializationService.InitializeNitrogenContent(viewItem, _farm);
            _cropsInitializationService.InitializePercentageReturns(_farm, viewItem);

            var fertilizerApplication = base.GetTestFertilizerApplicationViewItem();

            _cropsInitializationService.InitializeAmountOfBlendedProduct(_farm, viewItem, fertilizerApplication);

            Assert.AreEqual(53.4, fertilizerApplication.AmountOfBlendedProductApplied, 1);
        }

        #endregion
    }
}
