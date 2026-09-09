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

        #region Standing biomass conversion

        [TestMethod]
        public void SmallAreaDataPerennialYieldIsGrossedUpForHarvestLossesAndPutOnTheStandingBasis()
        {
            // Algorithm document, note under Eq. 2.1.2-1: a Small Area Data perennial yield is a hay yield - the
            // biomass taken off the field, which already excludes what was left behind - so it takes two steps to
            // become the total standing biomass:
            //
            //     Yield_standing = [ Yield_hay / (1 - Sp/100) ] x [ (1 - moisture_hay) / (1 - moisture_standing) ]
            //
            // at Sp = 35%, moisture_hay = 13%, moisture_standing = 80%. Only the moisture ratio was applied, which left
            // the standing biomass about 35% short. The gross-up had been happening downstream by accident, through
            // Eq. 2.1.2-1 dividing by (1 - Sp/100) while these years still carried the 35% perennial return default;
            // giving such a year its correct 100% return removed that, and nothing here replaced it.
            var farm = new Farm();
            farm.Defaults.PercentageOfProductReturnedToSoilForPerennials = 35;

            var method = typeof(CropInitializationService).GetMethod(
                "CalculateStandingBiomassAdjustment",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(method, "the conversion this test pins has been renamed or removed");

            var result = (double)method.Invoke(new CropInitializationService(), new object[] {1000.0, farm});

            // 1000 / 0.65 = 1538.46, then x (0.87 / 0.20) = 6692.31
            Assert.AreEqual(6692.3077, result, 0.001);

            // Without the gross-up this returned 4350 - the moisture ratio alone.
            Assert.AreNotEqual(4350, result, 0.001);
        }

        #endregion

        #region InitializeUtilization

        [TestMethod]
        public void InitializeUtilizationResetsEveryGrazingItemToTheCropDefault()
        {
            // Utilization is a user input, so nothing resets it automatically and rebuilding the grazing items from the
            // animal components deliberately preserves whatever is there. That protects a deliberate entry but leaves no
            // way back from a mistaken one - and a mistaken one is costly, because grazing carbon is grossed up by
            // dividing by the rate, so a value near zero inflates the field's production enormously. This is the way
            // back, reached from the reset defaults window.
            var farm = new Farm();
            var field = new FieldSystemComponent();
            var crop = new CropViewItem {CropType = CropType.TameGrass, Year = 1985};

            crop.GrazingViewItems.Add(new GrazingViewItem {Start = new DateTime(1985, 6, 1), Utilization = 1});
            crop.GrazingViewItems.Add(new GrazingViewItem {Start = new DateTime(1985, 8, 1), Utilization = 0});

            field.CropViewItems.Add(crop);
            farm.Components.Add(field);

            _cropsInitializationService.InitializeUtilization(farm);

            // Table 60: 60% for tame grass, as a percentage.
            foreach (var grazingViewItem in crop.GrazingViewItems)
            {
                Assert.AreEqual(60, grazingViewItem.Utilization, 0.0001);
            }
        }

        [TestMethod]
        public void InitializeUtilizationPrefersTheFarmsOwnRateWhenOneIsSet()
        {
            // A farm whose grazing systems all differ from Table 60 had to correct every grazing item by hand. One
            // number on the farm's settings now covers them, and the reset applies it.
            var farm = new Farm();
            farm.Defaults.UseCustomGrazingUtilizationRate = true;
            farm.Defaults.CustomGrazingUtilizationRate = 45;

            var field = new FieldSystemComponent();
            var tameGrass = new CropViewItem {CropType = CropType.TameGrass, Year = 1985};
            tameGrass.GrazingViewItems.Add(new GrazingViewItem {Start = new DateTime(1985, 6, 1), Utilization = 1});
            var rangeland = new CropViewItem {CropType = CropType.RangelandNative, Year = 1985};
            rangeland.GrazingViewItems.Add(new GrazingViewItem {Start = new DateTime(1985, 6, 1), Utilization = 1});
            field.CropViewItems.Add(tameGrass);
            field.CropViewItems.Add(rangeland);
            farm.Components.Add(field);

            _cropsInitializationService.InitializeUtilization(farm);

            // The farm's rate overrides the per-crop table, so both crops take it.
            Assert.AreEqual(45, tameGrass.GrazingViewItems[0].Utilization, 0.0001);
            Assert.AreEqual(45, rangeland.GrazingViewItems[0].Utilization, 0.0001);
        }

        [TestMethod]
        public void InitializeUtilizationFallsBackToTheTableWhenTheFarmsRateIsOffOrUnset()
        {
            // Off by default, so nothing changes for farms that never touch the setting. A zero rate is refused too:
            // grazing carbon is grossed up by dividing by it, so honouring zero would be worse than ignoring it.
            foreach (var (useCustom, rate) in new[] {(false, 45.0), (true, 0.0)})
            {
                var farm = new Farm();
                farm.Defaults.UseCustomGrazingUtilizationRate = useCustom;
                farm.Defaults.CustomGrazingUtilizationRate = rate;

                var field = new FieldSystemComponent();
                var crop = new CropViewItem {CropType = CropType.TameGrass, Year = 1985};
                crop.GrazingViewItems.Add(new GrazingViewItem {Start = new DateTime(1985, 6, 1), Utilization = 1});
                field.CropViewItems.Add(crop);
                farm.Components.Add(field);

                _cropsInitializationService.InitializeUtilization(farm);

                Assert.AreEqual(60, crop.GrazingViewItems[0].Utilization, 0.0001,
                    $"useCustom={useCustom} rate={rate}");
            }
        }

        [TestMethod]
        public void InitializeUtilizationUsesTheRateForEachCropType()
        {
            var farm = new Farm();
            var field = new FieldSystemComponent();

            var tameGrass = new CropViewItem {CropType = CropType.TameGrass, Year = 1985};
            tameGrass.GrazingViewItems.Add(new GrazingViewItem {Start = new DateTime(1985, 6, 1), Utilization = 1});

            var rangeland = new CropViewItem {CropType = CropType.RangelandNative, Year = 1985};
            rangeland.GrazingViewItems.Add(new GrazingViewItem {Start = new DateTime(1985, 6, 1), Utilization = 1});

            field.CropViewItems.Add(tameGrass);
            field.CropViewItems.Add(rangeland);
            farm.Components.Add(field);

            _cropsInitializationService.InitializeUtilization(farm);

            Assert.AreEqual(60, tameGrass.GrazingViewItems[0].Utilization, 0.0001);
            Assert.AreEqual(40, rangeland.GrazingViewItems[0].Utilization, 0.0001);
        }

        #endregion
    }
}
