using H.Core.Calculators.Carbon;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.Initialization;
using H.Core.Services.Initialization.Crops;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
        public void AssignPerennialStandIdsConsidersCoverCrop()
        {
            var crops = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Lentils, Year = 2019}, // Main crop grown
                new CropViewItem() {CropType = CropType.AlfalfaMedicagoSativaL, Year = 2019, IsSecondaryCrop = true}, // Cover crop used in same year
                new CropViewItem() {CropType = CropType.TameLegume, Year = 2020},
                new CropViewItem() {CropType = CropType.None, Year = 2020, IsSecondaryCrop = true},
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(crops) };

            var result = _cropsInitializationService.AssignPerennialStandIds(
                viewItems: crops, fieldSystemComponent: component);

            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void AssignPerennialStandPositionalYears()
        {
            var guid = Guid.NewGuid();

            var crops = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Wheat, Year = 2019, PerennialStandGroupId = guid},
                new CropViewItem() {CropType = CropType.TameLegume, Year = 2019, IsSecondaryCrop = true, PerennialStandGroupId = guid},
                new CropViewItem() {CropType = CropType.TameLegume, Year = 2020, PerennialStandGroupId = guid},
                new CropViewItem() {CropType = CropType.TameLegume, Year = 2020, IsSecondaryCrop = true, PerennialStandGroupId = guid},
                new CropViewItem() {CropType = CropType.TameLegume, Year = 2021, PerennialStandGroupId = guid},
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(crops) };

            _cropsInitializationService.AssignPerennialStandPositionalYears(
                viewItems: crops, fieldSystemComponent: component);
        }


        [TestMethod]
        public void AssignPerennialStandIdsConsidersOnlyPerennials()
        {
            var crops = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Lentils, Year = 2019},
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(crops) };

            var result = _cropsInitializationService.AssignPerennialStandIds(
                viewItems: crops, fieldSystemComponent: component);

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void AssignPerennialStandIdsAppliesCorrectIdsWhenUndersownCropsUsed()
        {
            // User has entered lentil, (haygrass (undersown)), (haygrass) as the sequence. Need to consider different stand ids when undersown crops
            // are used since we want different stand ids not the same stand id for all items in the sequence. The second HayGrass stand should have a different
            // id than the first stand.

            var crops = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Lentils, Year = 2000},
                new CropViewItem() {CropType = CropType.TameGrass, Year = 2000, UnderSownCropsUsed = true, IsSecondaryCrop = true},
                new CropViewItem() {CropType = CropType.TameGrass, Year = 2001},
                new CropViewItem() {CropType = CropType.None, Year = 2001, IsSecondaryCrop = true},
                new CropViewItem() {CropType = CropType.Lentils, Year = 2002},
                new CropViewItem() {CropType = CropType.TameGrass, Year = 2002, UnderSownCropsUsed = true, IsSecondaryCrop = true},
                new CropViewItem() {CropType = CropType.TameGrass, Year = 2003},
                new CropViewItem() {CropType = CropType.None, Year = 2003, IsSecondaryCrop = true},
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(crops) };

            var result = _cropsInitializationService.AssignPerennialStandIds(crops, component);

            Assert.AreEqual(2, result.Count());

            // Ensure all view items in same stand have the same stand GUID
            foreach (var group in result)
            {
                var key = group.Key;

                Assert.IsTrue(group.All(x => x.PerennialStandGroupId == key));
            }
        }

        [TestMethod]
        public void AssignPerennialStandIds()
        {
            var crops = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.TameLegume, Year = 2000},
                new CropViewItem() {CropType = CropType.TameLegume, Year = 2000, IsSecondaryCrop = true},
                new CropViewItem() {CropType = CropType.TameLegume, Year = 2001},
                new CropViewItem() {CropType = CropType.TameLegume, Year = 2001, IsSecondaryCrop = true},
                new CropViewItem() {CropType = CropType.ForageForSeed, Year = 2002},
                new CropViewItem() {CropType = CropType.ForageForSeed, Year = 2002, IsSecondaryCrop = true},
                new CropViewItem() {CropType = CropType.ForageForSeed, Year = 2003},
                new CropViewItem() {CropType = CropType.ForageForSeed, Year = 2003, IsSecondaryCrop = true},
                new CropViewItem() {CropType = CropType.Barley, Year = 2004},
                new CropViewItem() {CropType = CropType.None, Year = 2004, IsSecondaryCrop = true},
                new CropViewItem() {CropType = CropType.TameMixed, Year = 2005},
                new CropViewItem() {CropType = CropType.TameMixed, Year = 2005, IsSecondaryCrop = true},
                new CropViewItem() {CropType = CropType.TameMixed, Year = 2006},
                new CropViewItem() {CropType = CropType.TameMixed, Year = 2006, IsSecondaryCrop = true},
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(crops) };

            var result = _cropsInitializationService.AssignPerennialStandIds(crops, component);

            Assert.AreEqual(3, result.Count());

            // Ensure all view items in same stand have the same stand GUID
            foreach (var group in result)
            {
                var key = group.Key;

                Assert.IsTrue(group.All(x => x.PerennialStandGroupId == key));
            }
        }

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
            viewItem.Yield = 1000;

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
