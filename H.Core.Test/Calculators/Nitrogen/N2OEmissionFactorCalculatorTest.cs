using H.Core.Calculators.Nitrogen;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using H.Core.Providers.Evapotranspiration;
using H.Core.Providers.Precipitation;
using H.Core.Providers.Soil;
using NitrogenFertilizerType = H.Core.Enumerations.NitrogenFertilizerType;

namespace H.Core.Test.Calculators.Nitrogen
{
    [TestClass]
    public class N2OEmissionFactorCalculatorTest : UnitTestBase
    {
        #region Fields

        private N2OEmissionFactorCalculator _sut;
        private Table_36_Livestock_Emission_Conversion_Factors_Data _emissionFactors;
        private Farm _farm;
        private CropViewItem _viewItem;

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
            var n2oEmissionFactorCalculator = new N2OEmissionFactorCalculator(_climateProvider);

            _sut = n2oEmissionFactorCalculator;
            _sut.ClimateProvider = base._mockClimateProviderObject;
            _sut.LivestockEmissionConversionFactorsProvider = base._mockEmissionDataProviderObject;
            _sut.AnimalAmmoniaEmissionFactorProvider = base._mockAnimalAmmoniaEmissionFactorProviderObject;

            _mockManureService.Setup(x => x.GetTotalVolumeCreated(It.IsAny<int>(), It.IsAny<AnimalType>())).Returns(100);
            _mockManureService.Setup(x => x.GetTotalTANCreated(It.IsAny<int>(), It.IsAny<AnimalType>())).Returns(50);
            _mockManureService.Setup(x => x.GetTotalNitrogenCreated(It.IsAny<int>(), It.IsAny<AnimalType>())).Returns(75);
            _mockManureService.Setup(x => x.GetTotalNitrogenCreated(It.IsAny<int>())).Returns(75);
            _mockManureService.Setup(x => x.GetAmountOfTanUsedDuringLandApplication(It.IsAny<CropViewItem>(), It.IsAny<ManureApplicationViewItem>())).Returns(25);
            _mockManureService
                .Setup(x => x.GetTotalTanAppliedToAllFields(It.IsAny<int>(), It.IsAny<List<CropViewItem>>())).Returns(
                    new List<Tuple<double, AnimalType>>() { new Tuple<double, AnimalType>(100, AnimalType.Beef) });

            _mockManureService.Setup(x => x.GetManureCategoriesProducedOnFarm(It.IsAny<Farm>())).Returns(
                new List<AnimalType>()
                {
                    AnimalType.Beef, AnimalType.Dairy
                });

            _mockClimateProvider.Setup(x => x.GetMeanTemperatureForDay(It.IsAny<Farm>(), It.IsAny<DateTime>())).Returns(19);
            _mockClimateProvider.Setup(x => x.GetAnnualPrecipitation(It.IsAny<Farm>(), It.IsAny<DateTime>())).Returns(12);
            _mockClimateProvider.Setup(x => x.GetAnnualEvapotranspiration(It.IsAny<Farm>(), It.IsAny<DateTime>())).Returns(8);
            _mockClimateProvider.Setup(x => x.GetGrowingSeasonEvapotranspiration(It.IsAny<Farm>(), It.IsAny<DateTime>())).Returns(2);
            _mockClimateProvider.Setup(x => x.GetGrowingSeasonPrecipitation(It.IsAny<Farm>(), It.IsAny<DateTime>())).Returns(3);
            _mockClimateProvider.Setup(x => x.GetAnnualPrecipitation(It.IsAny<Farm>(), It.IsAny<int>())).Returns(12);
            _mockClimateProvider.Setup(x => x.GetAnnualEvapotranspiration(It.IsAny<Farm>(), It.IsAny<int>())).Returns(8);
            _mockClimateProvider.Setup(x => x.GetGrowingSeasonEvapotranspiration(It.IsAny<Farm>(), It.IsAny<int>())).Returns(2);
            _mockClimateProvider.Setup(x => x.GetGrowingSeasonPrecipitation(It.IsAny<Farm>(), It.IsAny<int>())).Returns(3);

            _emissionFactors = new Table_36_Livestock_Emission_Conversion_Factors_Data()
            {
                VolatilizationFraction = 0.10,
                LeachingFraction = 0.75,
                EmissionFactorVolatilization = 0.2,
                EmissionFactorLeach = 0.22,
            };

            _mockEmissionDataProvider
                .Setup(x => x.GetLandApplicationFactors(It.IsAny<Farm>(), It.IsAny<double>(), It.IsAny<double>(),
                    It.IsAny<AnimalType>(), It.IsAny<int>()))
                .Returns(_emissionFactors);

            _mockEmissionDataProvider.Setup(x => x.GetVolatilizationFractionForLandApplication(It.IsAny<AnimalType>(), It.IsAny<Province>(), It.IsAny<int>())).Returns(0.1);
            _mockEmissionDataProvider.Setup(x => x.GetEmissionFactorForVolatilizationBasedOnClimate(It.IsAny<double>(), It.IsAny<double>())).Returns(0.2);

            _mockAnimalAmmoniaEmissionFactorProvider.Setup(x => x.GetAmmoniaEmissionFactorForSolidAppliedManure(It.IsAny<TillageType>())).Returns(1);
            _mockAnimalAmmoniaEmissionFactorProvider.Setup(x => x.GetAmmoniaEmissionFactorForLiquidAppliedManure(It.IsAny<ManureApplicationTypes>())).Returns(1);

            _viewItem = base.GetTestCropViewItem();
            _viewItem.CropType = CropType.Wheat;
            var field = base.GetTestFieldComponent();

            _viewItem.FieldSystemComponentGuid = field.Guid;
            
            field.CropViewItems.Clear();
            field.CropViewItems.Add(_viewItem);
            
            var soilData = base.GetTestSoilData();
            field.SoilData = soilData;

            _farm = base.GetTestFarm();
            _farm.Components.Clear();

            _farm.Components.Add(field);

            var climateData = base.GetTestClimateData();
            _farm.ClimateData = climateData;
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        /// <summary>
        /// Ensure cache key uses irrigation: changing AmountOfIrrigation yields a different value.
        /// Also returns the same as original when reverted (cache hit for original key).
        /// </summary>
        [TestMethod]
        public void CalculateBaseEcodistrictFactor_ReturnsDifferent_When_IrrigationChanges()
        {
            var year = _viewItem.Year;

            var soilData = new SoilData();
            soilData.EcodistrictId = 371;
            soilData.Province = Province.Ontario;

            _farm.GeographicData.DefaultSoilData = soilData;

            // Simulate dry environment to ensure irrigation has an effect
            _farm.ClimateData.PrecipitationData = new PrecipitationData() {GrowingSeasonPrecipitation = 100 };
            _farm.ClimateData.EvapotranspirationData = new EvapotranspirationData() { GrowingSeasonEvapotranspiration = 200 };

            _viewItem.AmountOfIrrigation =0.0;
            var noIrr = _sut.CalculateBaseEcodistrictFactor(_farm, _viewItem, year);

            _viewItem.AmountOfIrrigation =10.0;
            var withIrr = _sut.CalculateBaseEcodistrictFactor(_farm, _viewItem, year);

            Assert.AreNotEqual(noIrr, withIrr,1e-12, "Expected different EF when irrigation changes, verifying fresh calculation (new cache key).");

            // Revert to original irrigation to ensure cached/original value is returned consistently
            _viewItem.AmountOfIrrigation =0.0;
            var noIrrAgain = _sut.CalculateBaseEcodistrictFactor(_farm, _viewItem, year);
            Assert.AreEqual(noIrr, noIrrAgain,1e-12, "Reverting irrigation should yield the original value (cache hit).");
        }

        /// <summary>
        /// Ensure cache key uses soil texture: changing texture yields a different value.
        /// </summary>
        [TestMethod]
        public void CalculateBaseEcodistrictFactor_ReturnsDifferent_When_SoilTextureChanges()
        {
            var year = _viewItem.Year;

            var soilData = new SoilData();
            soilData.EcodistrictId = 371;
            soilData.Province = Province.Ontario;

            _farm.GeographicData.DefaultSoilData = soilData;

            soilData.SoilTexture = SoilTexture.Coarse;

            var coarseVal = _sut.CalculateBaseEcodistrictFactor(_farm, _viewItem, year);

            soilData.SoilTexture = SoilTexture.Fine;

            var fineVal = _sut.CalculateBaseEcodistrictFactor(_farm, _viewItem, year);

            Assert.AreNotEqual(coarseVal, fineVal,1e-12, "Expected different EF when soil texture changes, verifying fresh calculation (new cache key).");
        }

        /// <summary>
        /// Ensure cache key uses ecodistrict: changing ecodistrict yields a different value.
        /// </summary>
        [TestMethod]
        public void CalculateBaseEcodistrictFactor_ReturnsDifferent_When_EcodistrictIdChanges()
        {
            var year = _viewItem.Year;

            var soilData = new SoilData();
            soilData.EcodistrictId = 371;
            soilData.Province = Province.Ontario;

            _farm.GeographicData.DefaultSoilData = soilData;

            // Simulate dry environment to ensure the FTopo value (based on Ecodistrict) has an effect
            _farm.ClimateData.PrecipitationData = new PrecipitationData() { GrowingSeasonPrecipitation = 100 };
            _farm.ClimateData.EvapotranspirationData = new EvapotranspirationData() { GrowingSeasonEvapotranspiration = 200 };

            var result1 = _sut.CalculateBaseEcodistrictFactor(_farm, _viewItem, year);

            soilData.EcodistrictId = 377;

            var results2 = _sut.CalculateBaseEcodistrictFactor(_farm, _viewItem, year);

            Assert.AreNotEqual(result1, results2, 1e-12, "Expected different EF when ecodistrict changes, verifying fresh calculation (new cache key).");

            // Revert to original ecodistrict to ensure cached/original value is returned consistently
            
            soilData.EcodistrictId = 371;
            var result1Again = _sut.CalculateBaseEcodistrictFactor(_farm, _viewItem, year);
            Assert.AreEqual(result1, result1Again, 1e-12, "Reverting ecodistrict should yield the original value (cache hit).");
        }

        /// <summary>
        /// Ensure cache key uses province: changing province yields a different value.
        /// </summary>
        [TestMethod]
        public void CalculateBaseEcodistrictFactor_ReturnsDifferent_When_ProvinceChanges()
        {
            var year = _viewItem.Year;

            var soilData = new SoilData();
            soilData.EcodistrictId = 371;
            soilData.Province = Province.Ontario;

            _farm.GeographicData.DefaultSoilData = soilData;

            // Simulate dry environment to ensure the FTopo value (based on Ecodistrict) has an effect. FTopo is province dependent.
            _farm.ClimateData.PrecipitationData = new PrecipitationData() { GrowingSeasonPrecipitation = 100 };
            _farm.ClimateData.EvapotranspirationData = new EvapotranspirationData() { GrowingSeasonEvapotranspiration = 200 };

            var result1 = _sut.CalculateBaseEcodistrictFactor(_farm, _viewItem, year);

            soilData.Province = Province.Manitoba;
            soilData.EcodistrictId = 380;

            var results2 = _sut.CalculateBaseEcodistrictFactor(_farm, _viewItem, year);

            Assert.AreNotEqual(result1, results2, 1e-12, "Expected different EF when province changes, verifying fresh calculation (new cache key).");

            // Revert to original province and ecodistrict to ensure cached/original value is returned consistently

            soilData.Province = Province.Ontario;
            soilData.EcodistrictId = 371;

            var result1Again = _sut.CalculateBaseEcodistrictFactor(_farm, _viewItem, year);
            Assert.AreEqual(result1, result1Again, 1e-12, "Reverting province should yield the original value (cache hit).");
        }

        /// <summary>
        /// Ensure cache key uses year: changing year yields a different value.
        /// </summary>
        [TestMethod]
        public void CalculateBaseEcodistrictFactor_ReturnsDifferent_When_YearChanges()
        {
            var year = _viewItem.Year;

            var soilData = new SoilData();
            soilData.EcodistrictId = 371;
            soilData.Province = Province.Ontario;

            _farm.GeographicData.DefaultSoilData = soilData;

            // Simulate dry environment to ensure the FTopo value (based on Ecodistrict) has an effect. FTopo is province dependent.
            _farm.ClimateData.PrecipitationData = new PrecipitationData() { GrowingSeasonPrecipitation = 100 };
            _farm.ClimateData.EvapotranspirationData = new EvapotranspirationData() { GrowingSeasonEvapotranspiration = 200 };

            var result1 = _sut.CalculateBaseEcodistrictFactor(_farm, _viewItem, year);

            // Change the year
            year += 1;

            // Simulate humid environment. Since we are in a different year. The year is only used to lookup climate data and so a change in year would result in different climate data
            _farm.ClimateData.PrecipitationData = new PrecipitationData() { GrowingSeasonPrecipitation = 500 };
            _farm.ClimateData.EvapotranspirationData = new EvapotranspirationData() { GrowingSeasonEvapotranspiration = 200 };

            var results2 = _sut.CalculateBaseEcodistrictFactor(_farm, _viewItem, year);

            Assert.AreNotEqual(result1, results2, 1e-12, "Expected different climate data when year changes, verifying fresh calculation (new cache key).");

            // Revert to original province and year and climate data to ensure cached/original value is returned consistently

            year -= 1;
            _farm.ClimateData.PrecipitationData = new PrecipitationData() { GrowingSeasonPrecipitation = 100 };
            _farm.ClimateData.EvapotranspirationData = new EvapotranspirationData() { GrowingSeasonEvapotranspiration = 200 };

            var result1Again = _sut.CalculateBaseEcodistrictFactor(_farm, _viewItem, year);
            Assert.AreEqual(result1, result1Again, 1e-12, "Reverting year and climate data should yield the original value (cache hit).");
        }

        /// <summary>
        /// Equation 2.5.1-1
        /// </summary>
        [TestMethod]
        public void CalculatedEmissionFactorReturnsCorrectValue()
        {
            var result = _sut.CalculateEcodistrictEmissionFactor(500.0, 600.0);

            Assert.AreEqual(0.012881024751743584, result);
        }

        /// <summary>
        /// Equation 2.5.2-1
        /// </summary>
        [TestMethod]
        public void CalculateNitrogenInputsFromFertilizerAppliedReturnsCorrectValue()
        {
        }


        [TestMethod]
        public void CalculateTopographyEmissionsReturnsZeroWhenNoClimateDataAvailable()
        {
            var result = _sut.CalculateTopographyEmissions(
                fractionOfLandOccupiedByLowerPortionsOfLandscape: 0.1,
                growingSeasonPrecipitation: 0,
                growingSeasonEvapotranspiration: 0, amountOfIrrigation: 0);

            Assert.AreEqual(0, result);
        }

        #region CalculateFractionOfNitrogenLostByVolatilization Tests

        [TestMethod]
        public void CalculateFractionOfNitrogenLostByVolatilization()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.Barley,
                NitrogenFertilizerType = NitrogenFertilizerType.Urea,
                FertilizerApplicationMethodology = FertilizerApplicationMethodologies.FullyInjected,
            };
            var farm = new Farm()
            {
                DefaultSoilData =
                {
                    SoilPh = 6,
                    SoilCec = 122,
                }
            };

            var result = _sut.CalculateFractionOfNitrogenLostByVolatilization(
                cropViewItem: currentYearViewItem,
                farm: farm);

            Assert.IsTrue(result > 0);
        }

        #endregion

        [TestMethod]
        public void CalculateTotalN2ONFromExportedManure()
        {
            var farm = base.GetTestFarm();
            farm.StageStates.Add(base.GetFieldStageState());

            var result = _sut.CalculateTotalDirectN2ONFromExportedManure(100, 0.5);

            Assert.AreEqual(result, 50);
        }

        [TestMethod]
        public void CalculateTotalIndirectEmissionsFromFieldSpecificManureApplications()
        {
            var farm = base.GetTestFarm();
            var viewItem = base.GetTestCropViewItem();
            var stageState = farm.GetFieldSystemDetailsStageState();
            stageState.DetailsScreenViewCropViewItems.Add(viewItem);
            farm.StageStates.Add(stageState);
            var field = new FieldSystemComponent() { Guid = viewItem.Guid };
            viewItem.FieldSystemComponentGuid = field.Guid;
            field.CropViewItems.Add(viewItem);
            farm.Components.Add(field);

            _sut.Initialize(farm);

            var manureNitrogenFromLandApplication = _sut.GetAmountOfManureNitrogenUsed(viewItem);
            Assert.AreEqual(100, manureNitrogenFromLandApplication, 2);

            var ammoniacalLoss = _sut.CalculateAmmoniacalLossFromManureForField(farm, viewItem);
            Assert.AreEqual(0.1525, ammoniacalLoss, 2);

            var ammoniaLoss = _sut.CalculateAmmoniaLossFromManureForField(farm, viewItem);
            Assert.AreEqual(0.185, ammoniaLoss, 2);

            var n2oNFromManureVolatilized =
                _sut.CalculateN2ONFromVolatilizationOfFarmSourcedLandAppliedManureForField(viewItem.Year, farm, viewItem);
            Assert.AreEqual(0.0305, n2oNFromManureVolatilized, 2);

            var n2oFromManureVolatilized = _sut.CalculateTotalManureN2OVolatilizationForField(viewItem, farm, viewItem.Year);
            Assert.AreEqual(960.521370938372, n2oFromManureVolatilized, 0.0001);

            var adjustedAmmoniacalLoss = _sut.CalculateTotalAdjustedAmmoniaEmissionsFromLandAppliedManureForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(0.122, adjustedAmmoniacalLoss, 0.001);

            var adjustedNH3Loss =
                _sut.CalculateTotalAdjustedAmmoniaEmissionsFromManureForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(CoreConstants.ConvertToNH3(adjustedAmmoniacalLoss), adjustedNH3Loss);

            var totalN2OFormManureLeaching = _sut.CalculateTotalN2ONFromManureLeachingForField(farm, viewItem);
            Assert.AreEqual(0.33, totalN2OFormManureLeaching, 2);

            var totalNitrateLeached = _sut.CalculateTotalManureNitrateLeached(farm, viewItem);
            Assert.AreEqual(29.67, totalNitrateLeached, 2);

            var totalIndirectEmissions =
                _sut.CalculateTotalIndirectEmissionsFromManureForField(farm, viewItem, viewItem.Year);

            Assert.AreEqual(11600.3723034133, totalIndirectEmissions, 0.0001);
        }

        [TestMethod]
        public void CalculateTotalIndirectEmissionsFromFieldSpecificManureApplicationsUsingMultipleAnimalResults()
        {
            var farm = base.GetTestFarm();
            var viewItem = base.GetTestCropViewItem();
            var stageState = farm.GetFieldSystemDetailsStageState();
            stageState.DetailsScreenViewCropViewItems.Add(viewItem);
            farm.StageStates.Add(stageState);
            var field = new FieldSystemComponent() { Guid = viewItem.Guid };
            viewItem.FieldSystemComponentGuid = field.Guid;
            field.CropViewItems.Add(viewItem);
            farm.Components.Add(field);

            _sut.Initialize(farm);

            var dairyApplication = base.GetTestBeefCattleManureApplicationViewItemUsingOnLivestockManure();
            dairyApplication.AnimalType = AnimalType.DairyLactatingCow;
            dairyApplication.AmountOfManureAppliedPerHectare = 6000;

            viewItem.ManureApplicationViewItems.Add(dairyApplication);

            var manureNitrogenFromLandApplication = _sut.GetAmountOfManureNitrogenUsed(viewItem);
            Assert.AreEqual(200, manureNitrogenFromLandApplication, 2);

            var ammoniacalLoss = _sut.CalculateAmmoniacalLossFromManureForField(farm, viewItem);
            Assert.AreEqual(0.1525, ammoniacalLoss, 2);

            var ammoniaLoss = _sut.CalculateAmmoniaLossFromManureForField(farm, viewItem);
            Assert.AreEqual(0.185, ammoniaLoss, 0.001);

            var n2oNFromManureVolatilized =
                _sut.CalculateN2ONFromVolatilizationOfFarmSourcedLandAppliedManureForField(viewItem.Year, farm, viewItem);
            Assert.AreEqual(0.0305, n2oNFromManureVolatilized, 0.001);

            var n2oFromManureVolatilized = _sut.CalculateTotalManureN2OVolatilizationForField(viewItem, farm, viewItem.Year);
            Assert.AreEqual(960.521370938372, n2oFromManureVolatilized, 0.0001);

            var adjustedAmmoniacalLoss = _sut.CalculateTotalAdjustedAmmoniaEmissionsFromLandAppliedManureForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(0.122, adjustedAmmoniacalLoss, 0.001);

            var adjustedNH3Loss =
                _sut.CalculateTotalAdjustedAmmoniaEmissionsFromManureForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(CoreConstants.ConvertToNH3(adjustedAmmoniacalLoss), adjustedNH3Loss);

            var totalN2OFormManureLeaching = _sut.CalculateTotalN2ONFromManureLeachingForField(farm, viewItem);
            Assert.AreEqual(0.66, totalN2OFormManureLeaching, 2);

            var totalNitrateLeached = _sut.CalculateTotalManureNitrateLeached(farm, viewItem);
            Assert.AreEqual(59.34, totalNitrateLeached, 2);

            var totalIndirectEmissions =
                _sut.CalculateTotalIndirectEmissionsFromManureForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(11600.3723034133, totalIndirectEmissions, 0.0001);
        }

        [TestMethod]
        public void CalculateAmmoniaFromLandApplicationForImportedManure()
        {
            var farm = base.GetTestFarm();
            var viewItem = base.GetTestCropViewItem();
            var stageState = farm.GetFieldSystemDetailsStageState();
            stageState.DetailsScreenViewCropViewItems.Add(viewItem);
            farm.StageStates.Add(stageState);
            var field = new FieldSystemComponent() { Guid = viewItem.Guid };
            viewItem.FieldSystemComponentGuid = field.Guid;
            field.CropViewItems.Add(viewItem);
            farm.Components.Add(field);
            viewItem.ManureApplicationViewItems.Clear();
            viewItem.ManureApplicationViewItems.Add(base.GetTestBeefCattleManureApplicationViewItemUsingImportedManure());

            var manureNitrogenFromLandApplication = _sut.GetAmountOfManureNitrogenUsed(viewItem);
            Assert.AreEqual(50, manureNitrogenFromLandApplication, 2);

            var ammoniacalLoss = _sut.CalculateAmmoniacalLossFromManureForField(farm, viewItem);
            Assert.AreEqual(5, ammoniacalLoss, 2);

            var ammoniaLoss = _sut.CalculateAmmoniaLossFromManureForField(farm, viewItem);
            Assert.AreEqual(6.07, ammoniaLoss, 2);

            var n2oNFromManureVolatilized =
                _sut.CalculateAmmoniaEmissionsFromVolatilizationOfImportedManureForField(farm, viewItem, viewItem.Year)
                    .Sum(x => x.Value);
            Assert.AreEqual(5, n2oNFromManureVolatilized, 2);

            var n2oFromManureVolatilized =
                _sut.CalculateVolatilizationEmissionsFromImportedManureForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(1, n2oFromManureVolatilized, 2);

            var adjustedAmmoniacalLoss = _sut.CalculateTotalAdjustedAmmoniacalEmissionsFromImportsForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(5 - 1, adjustedAmmoniacalLoss);

            var adjustedNH3Loss =
                _sut.CalculateTotalAdjustedAmmoniaEmissionsFromImportsForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(CoreConstants.ConvertToNH3(4), adjustedNH3Loss);

            var totalN2OFormManureLeaching = _sut.CalculateTotalN2ONFromManureLeachingForField(farm, viewItem);
            Assert.AreEqual(0.66, totalN2OFormManureLeaching, 2);

            var totalNitrateLeached = _sut.CalculateTotalManureNitrateLeached(farm, viewItem);
            Assert.AreEqual(14.83, totalNitrateLeached, 2);

            var totalIndirectEmissions =
                _sut.CalculateTotalIndirectEmissionsFromManureForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(15.84, totalIndirectEmissions, 2);
        }

        [TestMethod]
        public void CalculateAmmoniaFromLandApplicationForMultipleImportedManureApplications()
        {
            var farm = base.GetTestFarm();
            var viewItem = base.GetTestCropViewItem();
            var stageState = farm.GetFieldSystemDetailsStageState();
            stageState.DetailsScreenViewCropViewItems.Add(viewItem);
            farm.StageStates.Add(stageState);
            var field = new FieldSystemComponent() { Guid = viewItem.Guid };
            viewItem.FieldSystemComponentGuid = field.Guid;
            field.CropViewItems.Add(viewItem);
            farm.Components.Add(field);
            viewItem.ManureApplicationViewItems.Clear();
            viewItem.ManureApplicationViewItems.Add(base.GetTestBeefCattleManureApplicationViewItemUsingImportedManure());
            viewItem.ManureApplicationViewItems.Add(base.GetTestDairyCattleManureApplicationViewItemUsingImportedManure());

            var manureNitrogenFromLandApplication = _sut.GetAmountOfManureNitrogenUsed(viewItem);
            Assert.AreEqual(100, manureNitrogenFromLandApplication, 2);

            var ammoniacalLoss = _sut.CalculateAmmoniacalLossFromManureForField(farm, viewItem);
            Assert.AreEqual(10, ammoniacalLoss, 2);

            var ammoniaLoss = _sut.CalculateAmmoniaLossFromManureForField(farm, viewItem);
            Assert.AreEqual(12.14, ammoniaLoss, 2);

            var n2oNFromManureVolatilized =
                _sut.CalculateAmmoniaEmissionsFromVolatilizationOfImportedManureForField(farm, viewItem, viewItem.Year)
                    .Sum(x => x.Value);
            Assert.AreEqual(10, n2oNFromManureVolatilized, 2);

            var n2oFromManureVolatilized =
                _sut.CalculateVolatilizationEmissionsFromImportedManureForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(1, n2oFromManureVolatilized, 2);

            var adjustedAmmoniacalLoss = _sut.CalculateTotalAdjustedAmmoniacalEmissionsFromImportsForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(8, adjustedAmmoniacalLoss);

            var adjustedNH3Loss =
                _sut.CalculateTotalAdjustedAmmoniaEmissionsFromImportsForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(9.71, adjustedNH3Loss, 2);

            var totalN2OFormManureLeaching = _sut.CalculateTotalN2ONFromManureLeachingForField(farm, viewItem);
            Assert.AreEqual(0.66, totalN2OFormManureLeaching, 2);

            var totalNitrateLeached = _sut.CalculateTotalManureNitrateLeached(farm, viewItem);
            Assert.AreEqual(29.67, totalNitrateLeached, 2);

            var totalIndirectEmissions =
                _sut.CalculateTotalIndirectEmissionsFromManureForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(31.67, totalIndirectEmissions, 2);
        }

        [TestMethod]
        public void CalculateAmmoniaFromLandApplicationForDigestate()
        {
            var farm = base.GetTestFarm();
            var viewItem = base.GetTestCropViewItem();
            var stageState = farm.GetFieldSystemDetailsStageState();
            stageState.DetailsScreenViewCropViewItems.Add(viewItem);
            farm.StageStates.Add(stageState);

            var digestateApplication = base.GetTestRawDigestateApplicationViewItem();
            viewItem.DigestateApplicationViewItems.Clear();
            viewItem.DigestateApplicationViewItems.Add(digestateApplication);

            digestateApplication.DateCreated = viewItem.DateCreated;
            

            var field = base.GetTestFieldComponent();
            farm.Components.Add(field);

            viewItem.FieldSystemComponentGuid = field.Guid;
            field.CropViewItems.Add(viewItem);

            _sut.Initialize(farm);

            var digestateN = _sut.GetAmountOfManureNitrogenUsed(viewItem);
            Assert.AreEqual(100, digestateN, 2);

            var ammoniacalLoss = _sut.CalculateNH3NLossFromFarmSourcedLandAppliedDigestateForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(10, ammoniacalLoss, 2);

            var ammoniaLoss = _sut.CalculateAmmoniaLossFromManureForField(farm, viewItem);
            Assert.AreEqual(0.185, ammoniaLoss, 0.001);

            var n2oNFromManureVolatilized =
                _sut.CalculateN2ONFromVolatilizationOfFarmSourcedLandAppliedDigestateForField(viewItem.Year, farm,
                    viewItem);
            Assert.AreEqual(1.7, n2oNFromManureVolatilized, 2);

            var n2oFromManureVolatilized =
                _sut.CalculateN2OFromVolatilizationOfFarmSourcedLandAppliedDigestateForField(viewItem.Year, farm, viewItem);
            Assert.AreEqual(2.67, n2oFromManureVolatilized, 2);

            var adjustedAmmoniacalLoss = _sut.CalculateAdjustedDigestateNH3NEmissionsForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(6.82, adjustedAmmoniacalLoss, 2);

            var adjustedNH3Loss =
                _sut.CalculateAdjustedDigestateNH3EmissionsForField(farm, viewItem);
            Assert.AreEqual(9.71, adjustedNH3Loss, 2);

            var totalN2OFormManureLeaching = _sut.CalculateTotalN2ONFromDigestateLeachingForField(farm, viewItem);
            Assert.AreEqual(0.66, totalN2OFormManureLeaching, 2);

            var totalNitrateLeached = _sut.CalculateTotalDigestateNitrateLeached(farm, viewItem);
            Assert.AreEqual(14.83, totalNitrateLeached, 2);

            var totalIndirectEmissions =
                _sut.CalculateTotalIndirectEmissionsFromDigestateForFarm(farm, viewItem.Year);
            Assert.AreEqual(1.87, totalIndirectEmissions, 2);
        }

        [TestMethod]
        public void CalculateAmmoniaFromLandApplicationForMultipleDigestateApplications()
        {
            var farm = base.GetTestFarm();
            var viewItem = base.GetTestCropViewItem();
            var stageState = farm.GetFieldSystemDetailsStageState();
            stageState.DetailsScreenViewCropViewItems.Add(viewItem);
            farm.StageStates.Add(stageState);
            viewItem.DigestateApplicationViewItems.Clear();

            var digestateApplication = base.GetTestRawDigestateApplicationViewItem();
            var digestateApplication2 = base.GetTestLiquidDigestateApplicationViewItem();

            digestateApplication.DateCreated = viewItem.DateCreated;
            digestateApplication2.DateCreated = viewItem.DateCreated;

            viewItem.DigestateApplicationViewItems.Add(digestateApplication2);
            viewItem.DigestateApplicationViewItems.Add(digestateApplication);

            var field = base.GetTestFieldComponent();
            farm.Components.Add(field);

            viewItem.FieldSystemComponentGuid = field.Guid;
            field.CropViewItems.Add(viewItem);

            _sut.Initialize(farm);

            var digestateN = _sut.GetAmountOfManureNitrogenUsed(viewItem);
            Assert.AreEqual(100, digestateN, 2);

            var ammoniacalLoss = _sut.CalculateNH3NLossFromFarmSourcedLandAppliedDigestateForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(93.77, ammoniacalLoss, 2);

            var ammoniaLoss = _sut.CalculateAmmoniaLossFromManureForField(farm, viewItem);
            Assert.AreEqual(0.185, ammoniaLoss, 0.001);

            var n2oNFromManureVolatilized =
                _sut.CalculateN2ONFromVolatilizationOfFarmSourcedLandAppliedDigestateForField(viewItem.Year, farm,
                    viewItem);
            Assert.AreEqual(18.75, n2oNFromManureVolatilized, 2);

            var n2oFromManureVolatilized =
                _sut.CalculateN2OFromVolatilizationOfFarmSourcedLandAppliedDigestateForField(viewItem.Year, farm, viewItem);
            Assert.AreEqual(29.47, n2oFromManureVolatilized, 2);

            var adjustedAmmoniacalLoss = _sut.CalculateAdjustedDigestateNH3NEmissionsForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(75.02, adjustedAmmoniacalLoss, 2);

            var adjustedNH3Loss =
                _sut.CalculateAdjustedDigestateNH3EmissionsForField(farm, viewItem);
            Assert.AreEqual(91.09, adjustedNH3Loss, 2);

            var totalN2OFormManureLeaching = _sut.CalculateTotalN2ONFromDigestateLeachingForField(farm, viewItem);
            Assert.AreEqual(0.66, totalN2OFormManureLeaching, 2);

            var totalNitrateLeached = _sut.CalculateTotalDigestateNitrateLeached(farm, viewItem);
            Assert.AreEqual(163.18, totalNitrateLeached, 2);

            var totalIndirectEmissions =
                _sut.CalculateTotalIndirectEmissionsFromDigestateForFarm(farm, viewItem.Year);
            Assert.AreEqual(20.57, totalIndirectEmissions, 2);
        }

        [TestMethod]
        public void CalculateAmmoniaEmissionsFromExportedManure()
        {
            var farm = base.GetTestFarm();

            _mockManureService.Setup(x => x.GetManureTypesExported(It.IsAny<Farm>(), It.IsAny<int>())).Returns(new List<AnimalType>() { AnimalType.Beef });
            _mockManureService.Setup(x => x.GetTotalNitrogenFromExportedManure(It.IsAny<int>(), It.IsAny<Farm>(), It.IsAny<AnimalType>())).Returns(100);

            // Can't inject manure service anymore, need to mock this a different way

            var result = _sut.CalculateAmmoniaEmissionsFromExportedManureForFarmAndYear(
                farm: farm,
                DateTime.Now.Year);

            Assert.AreEqual(150, result.Sum(x => x.Value));
        }

        [TestMethod]
        public void CalculateAmmoniaEmissionsFromImportedManure()
        {
            var farm = base.GetTestFarm();
            var cropViewItem = base.GetTestCropViewItem();
            var application = base.GetTestDairyCattleManureApplicationViewItemUsingImportedManure();
            cropViewItem.ManureApplicationViewItems.Add(application);

            _mockManureService.Setup(x => x.GetManureTypesImported(It.IsAny<Farm>(), It.IsAny<int>())).Returns(new List<AnimalType>() { AnimalType.Dairy });
            _mockManureService.Setup(x => x.GetTotalNitrogenFromManureImports(It.IsAny<int>(), It.IsAny<Farm>(), It.IsAny<AnimalType>())).Returns(100);

            var result = _sut.CalculateAmmoniaEmissionsFromVolatilizationOfImportedManureForField(
                farm: farm,
                cropViewItem: cropViewItem,
                DateTime.Now.Year);

            Assert.AreEqual(5, result.Sum(x => x.Value));
        }

        [TestMethod]
        public void GetManureNitrogenRemainingForFieldTest()
        {
            _mockManureService.Setup(x => x.GetTotalManureNitrogenRemainingForFarmAndYear(It.IsAny<int>(), It.IsAny<Farm>())).Returns(600);

            var currentYear = DateTime.Now.Year;

            var cropViewItem = new CropViewItem() {Year = currentYear, Area = 20};
            var farm = base.GetTestFarm();
            var field = base.GetTestFieldComponent();
            field.FieldArea = 100;
            farm.Components.Add(field);

            var field2 = new FieldSystemComponent();
            field2.FieldArea = 300;
            farm.Components.Add(field2);

            var detailViewItem1 = new CropViewItem() {Year = currentYear, CropType = CropType.Barley};
            var detailViewItem2 = new CropViewItem() {Year = currentYear, CropType = CropType.Wheat};
            var detailViewItem3 = new CropViewItem() { Year = (currentYear - 3) ,CropType = CropType.Beans};

            var stageState = new FieldSystemDetailsStageState();
            stageState.DetailsScreenViewCropViewItems.Add(detailViewItem1);
            stageState.DetailsScreenViewCropViewItems.Add(detailViewItem2);
            stageState.DetailsScreenViewCropViewItems.Add(detailViewItem3);

            farm.StageStates.Add(stageState);

            _sut.Initialize(farm);

            var result = _sut.GetManureNitrogenRemainingForField(cropViewItem, farm);

            Assert.AreEqual(1851.8927251428993, result, 1);
        }

        [TestMethod]
        public void GetTotalDigestateNitrogenAppliedFromLivestockAndImportsInYearWhenFieldNotFound()
        {
            var viewItem = base.GetTestCropViewItem();
            var farm = base.GetTestFarm();

            var result = _sut.GetTotalDigestateNitrogenAppliedFromLivestockAndImportsInYear(viewItem, farm);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetTotalDigestateNitrogenAppliedFromLivestockAndImportsInYearWhenNoLocalOrImportedDigestateApplicationsMade()
        {
            var viewItem = base.GetTestCropViewItem();
            var farm = base.GetTestFarm();
            var field = base.GetTestFieldComponent();
            viewItem.FieldSystemComponentGuid = field.Guid;
            farm.Components.Add(field);

            var result = _sut.GetTotalDigestateNitrogenAppliedFromLivestockAndImportsInYear(viewItem, farm);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetTotalDigestateNitrogenAppliedFromLivestockAndImportsInYearReturnsNonZeroWhenLocalApplicationMade()
        {
            var viewItem = base.GetTestCropViewItem();
            var farm = base.GetTestFarm();
            var field = base.GetTestFieldComponent();
            field.CropViewItems.Clear();

            viewItem.FieldSystemComponentGuid = field.Guid;
            viewItem.Area = 50;
            farm.Components.Add(field);
            field.CropViewItems.Add(viewItem);
            

            viewItem.DigestateApplicationViewItems.Add(new DigestateApplicationViewItem() {ManureLocationSourceType = ManureLocationSourceType.Livestock, DateCreated = DateTime.Now, AmountOfNitrogenAppliedPerHectare = 100});

            var result = _sut.GetTotalDigestateNitrogenAppliedFromLivestockAndImportsInYear(viewItem, farm);

            Assert.AreEqual(50 * 100, result);
        }

        [TestMethod]
        public void GetTotalDigestateNitrogenAppliedFromLivestockAndImportsInYearReturnsNonZeroWhenImportedApplicationMade()
        {
            var viewItem = base.GetTestCropViewItem();
            var farm = base.GetTestFarm();
            var field = base.GetTestFieldComponent();
            field.CropViewItems.Clear();

            viewItem.FieldSystemComponentGuid = field.Guid;
            viewItem.Area = 50;
            farm.Components.Add(field);
            field.CropViewItems.Add(viewItem);


            viewItem.DigestateApplicationViewItems.Add(new DigestateApplicationViewItem() { ManureLocationSourceType = ManureLocationSourceType.Imported, DateCreated = DateTime.Now, AmountOfNitrogenAppliedPerHectare = 100 });

            var result = _sut.GetTotalDigestateNitrogenAppliedFromLivestockAndImportsInYear(viewItem, farm);

            Assert.AreEqual(50 * 100, result);
        }

        [TestMethod]
        public void GetTotalDigestateNitrogenAppliedFromLivestockAndImportsInYearReturnsNonZeroWhenImportedApplicationAndLocalMade()
        {
            var viewItem = base.GetTestCropViewItem();
            var farm = base.GetTestFarm();
            var field = base.GetTestFieldComponent();
            field.CropViewItems.Clear();

            viewItem.FieldSystemComponentGuid = field.Guid;
            viewItem.Area = 50;
            farm.Components.Add(field);
            field.CropViewItems.Add(viewItem);

            viewItem.DigestateApplicationViewItems.Add(new DigestateApplicationViewItem() { ManureLocationSourceType = ManureLocationSourceType.Imported, DateCreated = DateTime.Now, AmountOfNitrogenAppliedPerHectare = 100 });
            viewItem.DigestateApplicationViewItems.Add(new DigestateApplicationViewItem() { ManureLocationSourceType = ManureLocationSourceType.Livestock, DateCreated = DateTime.Now, AmountOfNitrogenAppliedPerHectare = 100 });

            var result = _sut.GetTotalDigestateNitrogenAppliedFromLivestockAndImportsInYear(viewItem, farm);

            Assert.AreEqual((50 * 100) * 2, result);
        }

        [TestMethod]
        public void CalculateDirectN2ONEmissionsFromFieldSpecificDigestateSpreadingForFieldReturnsZeroWhenFieldIsNativeRangeland()
        {
            var viewItem = base.GetTestCropViewItem();
            viewItem.CropType = CropType.RangelandNative;

            var farm = base.GetTestFarm();

            var result = _sut.CalculateDirectN2ONEmissionsFromFieldSpecificDigestateSpreadingForField(viewItem, farm);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void CalculateDirectN2ONEmissionsFromFieldSpecificDigestateSpreadingForFieldReturnsZeroWhenNoApplicationsMade()
        {
            var result = _sut.CalculateDirectN2ONEmissionsFromFieldSpecificDigestateSpreadingForField(_viewItem, _farm);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void CalculateDirectN2ONEmissionsFromFieldSpecificDigestateSpreadingForFieldReturnsNonZeroWhenLivestockApplicationsMade()
        {
            _viewItem.DigestateApplicationViewItems.Add(new DigestateApplicationViewItem() { ManureLocationSourceType = ManureLocationSourceType.Livestock, DateCreated = DateTime.Now, AmountOfNitrogenAppliedPerHectare = 100 });

            var result = _sut.CalculateDirectN2ONEmissionsFromFieldSpecificDigestateSpreadingForField(_viewItem, _farm);

            Assert.AreEqual(0.056904037428100511, result);
        }

        [TestMethod]
        public void CalculateDirectN2ONEmissionsFromFieldSpecificDigestateSpreadingForFieldReturnsNonZeroWhenLivestockAndImportApplicationsMade()
        {
            _viewItem.DigestateApplicationViewItems.Add(new DigestateApplicationViewItem() { ManureLocationSourceType = ManureLocationSourceType.Imported, DateCreated = DateTime.Now, AmountOfNitrogenAppliedPerHectare = 100 });
            _viewItem.DigestateApplicationViewItems.Add(new DigestateApplicationViewItem() { ManureLocationSourceType = ManureLocationSourceType.Livestock, DateCreated = DateTime.Now, AmountOfNitrogenAppliedPerHectare = 100 });

            var result = _sut.CalculateDirectN2ONEmissionsFromFieldSpecificDigestateSpreadingForField(_viewItem, _farm);

            Assert.AreEqual(0.113808074856201, result, 0.00001);
        }

        [TestMethod]
        public void GetTotalDigestateVolumeAppliedFromImportsInYear()
        {
            _viewItem.Area = 50;
            _viewItem.DigestateApplicationViewItems.Add(new DigestateApplicationViewItem() { ManureLocationSourceType = ManureLocationSourceType.Imported, DateCreated = DateTime.Now, AmountAppliedPerHectare = 100 });

            var result = _sut.GetTotalDigestateVolumeAppliedFromLivestockAndImportsInYear(_viewItem, _farm);

            Assert.AreEqual(50 * 100, result);
        }

        [TestMethod]
        public void GetTotalDigestateVolumeAppliedFromLivestockInYear()
        {
            _viewItem.Area = 50;
            _viewItem.DigestateApplicationViewItems.Add(new DigestateApplicationViewItem() { ManureLocationSourceType = ManureLocationSourceType.Livestock, DateCreated = DateTime.Now, AmountAppliedPerHectare = 100 });

            var result = _sut.GetTotalDigestateVolumeAppliedFromLivestockAndImportsInYear(_viewItem, _farm);

            Assert.AreEqual(50 * 100, result);
        }

        [TestMethod]
        public void GetTotalDigestateVolumeAppliedFromLivestockAndImportsInYear()
        {
            _viewItem.Area = 50;
            _viewItem.DigestateApplicationViewItems.Add(new DigestateApplicationViewItem() { ManureLocationSourceType = ManureLocationSourceType.Livestock, DateCreated = DateTime.Now, AmountAppliedPerHectare = 100 });
            _viewItem.DigestateApplicationViewItems.Add(new DigestateApplicationViewItem() { ManureLocationSourceType = ManureLocationSourceType.Imported, DateCreated = DateTime.Now, AmountAppliedPerHectare = 100 });

            var result = _sut.GetTotalDigestateVolumeAppliedFromLivestockAndImportsInYear(_viewItem, _farm);

            Assert.AreEqual((50 * 100) * 2, result);
        }

        [TestMethod]
        public void GetTotalDigestateVolumeAppliedFromLivestockAndImportsInYearReturnsZero()
        {
            _viewItem.Area = 50;
            _viewItem.DigestateApplicationViewItems.Clear();

            var result = _sut.GetTotalDigestateVolumeAppliedFromLivestockAndImportsInYear(_viewItem, _farm);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetTotalManureVolumeAppliedFromLivestockAndImportsInYearReturnsZero()
        {
            _viewItem.Area = 50;
            _viewItem.ManureApplicationViewItems.Clear();

            var result = _sut.GetTotalManureVolumeAppliedFromLivestockAndImportsInYear(_viewItem, _farm);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetTotalManureVolumeAppliedFromLivestockAndImportsInYear()
        {
            _viewItem.Area = 50;
            _viewItem.ManureApplicationViewItems.Clear();

            _viewItem.ManureApplicationViewItems.Add(new ManureApplicationViewItem() { ManureLocationSourceType = ManureLocationSourceType.Livestock, DateOfApplication = DateTime.Now, AmountOfManureAppliedPerHectare = 100 });
            _viewItem.ManureApplicationViewItems.Add(new ManureApplicationViewItem() { ManureLocationSourceType = ManureLocationSourceType.Imported, DateOfApplication = DateTime.Now, AmountOfManureAppliedPerHectare = 100 });

            var result = _sut.GetTotalManureVolumeAppliedFromLivestockAndImportsInYear(_viewItem, _farm);

            Assert.AreEqual((50 * 100) * 2, result);
        }

        [TestMethod]
        public void GetTotalManureVolumeAppliedFromLivestockInYear()
        {
            _viewItem.Area = 50;
            _viewItem.ManureApplicationViewItems.Clear();

            _viewItem.ManureApplicationViewItems.Add(new ManureApplicationViewItem() { ManureLocationSourceType = ManureLocationSourceType.Livestock, DateOfApplication = DateTime.Now, AmountOfManureAppliedPerHectare = 100 });

            var result = _sut.GetTotalManureVolumeAppliedFromLivestockAndImportsInYear(_viewItem, _farm);

            Assert.AreEqual((50 * 100), result);
        }

        [TestMethod]
        public void GetTotalManureVolumeAppliedFromImportsInYear()
        {
            _viewItem.Area = 50;
            _viewItem.ManureApplicationViewItems.Clear();

            _viewItem.ManureApplicationViewItems.Add(new ManureApplicationViewItem() { ManureLocationSourceType = ManureLocationSourceType.Imported, DateOfApplication = DateTime.Now, AmountOfManureAppliedPerHectare = 100 });

            var result = _sut.GetTotalManureVolumeAppliedFromLivestockAndImportsInYear(_viewItem, _farm);

            Assert.AreEqual((50 * 100), result);
        }

        [TestMethod]
        public void CalculateWeightedOrganicNitrogenEmissionFactor()
        {
            var viewItem1 = new CropViewItem();
            var viewItem2 = new CropViewItem();

            var items = new List<CropViewItem>();
            items.Add(viewItem1);
            items.Add(viewItem2);

            var field = base.GetTestFieldComponent();
            _farm.Components.Add(field);

            field.CropViewItems.Add(viewItem1);
            field.CropViewItems.Add(viewItem2);

            viewItem1.FieldSystemComponentGuid = field.Guid;
            viewItem2.FieldSystemComponentGuid = field.Guid;

            var result = _sut.CalculateWeightedOrganicNitrogenEmissionFactor(items, _farm);

            Assert.AreEqual(0.00010811767111339097, result);
        }

        [TestMethod]
        public void CalculateDirectN2ONFromLeftOverManureForField()
        {
            var farm = base.GetTestFarm();
            farm.GeographicData = base.GetTestGeographicData();
            var climate = base.GetTestClimateData();
            farm.ClimateData = climate;
            ;

            var viewItem = base.GetTestCropViewItem();

            var stageState = farm.GetFieldSystemDetailsStageState();
            stageState.DetailsScreenViewCropViewItems.Add(viewItem);
            farm.StageStates.Add(stageState);

            var fieldWithManureApplication = base.GetTestFieldComponent();
            fieldWithManureApplication.FieldArea = 133;

            viewItem.FieldSystemComponentGuid = fieldWithManureApplication.Guid;
            
            farm.Components.Add(fieldWithManureApplication);
            viewItem.FieldSystemComponentGuid = fieldWithManureApplication.Guid;

            var fieldWithOutManureApplications = new FieldSystemComponent();
            fieldWithOutManureApplications.FieldArea = 222;
            fieldWithOutManureApplications.CropViewItems.Add(viewItem);

            farm.Components.Add(fieldWithOutManureApplications);

            _sut.Initialize(farm);

            var result = _sut.CalculateDirectN2ONFromLeftOverManureForField(farm, viewItem);

            Assert.AreEqual(7.65433909810864, result, 0.0001);
        }

        [TestMethod]
        public void CalculateTotalIndirectN2ONFromExportedManureTest()
        {
            var farm = new Farm();
            var manureExportViewItem = base.GetTestManureExportViewItem();
            var year = DateTime.Now.Year;

            var result = _sut.CalculateTotalIndirectN2ONFromExportedManure(farm, manureExportViewItem);

            Assert.AreEqual(1.7475, result);
        }

        [TestMethod]
        public void CalculateAdjustedNH3NLossFromManureExports()
        {
            var farm = new Farm();
            var year = DateTime.Now.Year;
            var manureExportViewItem = base.GetTestManureExportViewItem();

            var result = _sut.CalculateAdjustedNH3NLossFromManureExports(farm, year, manureExportViewItem);

            Assert.AreEqual(6, result);
        }

        #endregion

        [TestMethod]
        public void CalculateTopographyEmissions_Cache_Used_For_Irrigated_Branch()
        {
            // Arrange
            var fTopo =0.25; // fraction of land in lower landscape
            var precip =50.0;
            var evap =50.0; // equal would also trigger irrigated branch due to equality, but we set irrigation >0 to be explicit
            var irrigation =10.0; // >0 ensures irrigated branch

            var cacheField = typeof(N2OEmissionFactorCalculator).GetField("_topographyCalculationCache", BindingFlags.Instance | BindingFlags.NonPublic);
            var cache = (System.Collections.IDictionary)cacheField.GetValue(_sut);
            var before = cache.Count;

            // Act
            var v1 = _sut.CalculateTopographyEmissions(fTopo, precip, evap, irrigation);
            var afterFirst = cache.Count;
            var v2 = _sut.CalculateTopographyEmissions(fTopo, precip, evap, irrigation);
            var afterSecond = cache.Count;

            // Assert
            Assert.AreEqual(v1, v2,0.0, "Cached value should be stable for identical inputs (irrigated branch).");
            Assert.IsTrue(afterFirst >= before +1, "Expected cache to add an entry on first call.");
            Assert.AreEqual(afterFirst, afterSecond, "Second identical call should not grow the cache.");
        }

        [TestMethod]
        public void CalculateTopographyEmissions_Cache_Used_For_Humid_Branch()
        {
            var fTopo =0.30;
            var precip =200.0;
            var evap =100.0; // precip/evap >1 => humid
            var irrigation =0.0;

            var cacheField = typeof(N2OEmissionFactorCalculator).GetField("_topographyCalculationCache", BindingFlags.Instance | BindingFlags.NonPublic);
            var cache = (System.Collections.IDictionary)cacheField.GetValue(_sut);
            var before = cache.Count;

            var v1 = _sut.CalculateTopographyEmissions(fTopo, precip, evap, irrigation);
            var afterFirst = cache.Count;
            var v2 = _sut.CalculateTopographyEmissions(fTopo, precip, evap, irrigation);
            var afterSecond = cache.Count;

            Assert.AreEqual(v1, v2,0.0, "Cached value should be stable for identical inputs (humid branch).");
            Assert.IsTrue(afterFirst >= before +1, "Expected cache to add an entry on first call (humid).");
            Assert.AreEqual(afterFirst, afterSecond, "Second identical call should not grow the cache (humid).");
        }

        [TestMethod]
        public void CalculateTopographyEmissions_Cache_Used_For_Dry_Branch()
        {
            var fTopo =0.40;
            var precip =40.0;
            var evap =100.0; // precip/evap <=1 => dry
            var irrigation =0.0;

            var cacheField = typeof(N2OEmissionFactorCalculator).GetField("_topographyCalculationCache", BindingFlags.Instance | BindingFlags.NonPublic);
            var cache = (System.Collections.IDictionary)cacheField.GetValue(_sut);
            var before = cache.Count;

            var v1 = _sut.CalculateTopographyEmissions(fTopo, precip, evap, irrigation);
            var afterFirst = cache.Count;
            var v2 = _sut.CalculateTopographyEmissions(fTopo, precip, evap, irrigation);
            var afterSecond = cache.Count;

            Assert.AreEqual(v1, v2,0.0, "Cached value should be stable for identical inputs (dry branch).");
            Assert.IsTrue(afterFirst >= before +1, "Expected cache to add an entry on first call (dry).");
            Assert.AreEqual(afterFirst, afterSecond, "Second identical call should not grow the cache (dry).");
        }

        [TestMethod]
        public void CalculateTopographyEmissions_NewKey_When_InputsChange()
        {
            var cacheField = typeof(N2OEmissionFactorCalculator).GetField("_topographyCalculationCache", BindingFlags.Instance | BindingFlags.NonPublic);
            var cache = (System.Collections.IDictionary)cacheField.GetValue(_sut);
            var before = cache.Count;

            var fTopo =0.20;
            var precip =60.0;
            var evap =120.0;
            var irrigation =0.0;

            var v1 = _sut.CalculateTopographyEmissions(fTopo, precip, evap, irrigation);
            var afterFirst = cache.Count;

            // Change one component of the cache key (precipitation) to force a new entry
            var precip2 =61.0;
            var v2 = _sut.CalculateTopographyEmissions(fTopo, precip2, evap, irrigation);
            var afterSecond = cache.Count;

            Assert.IsTrue(afterFirst >= before +1, "Expected cache to add an entry on first call.");
            Assert.IsTrue(afterSecond >= afterFirst +1, "Changing an input should create a new cache entry.");

            // Values should typically differ for different precipitation; if equal due to function behavior, cache size assertions still validate new key
            if (Math.Abs(v1 - v2) <1e-12)
            {
                Assert.Inconclusive("Topography emission values are equal for very similar inputs, but a distinct cache key was created as expected.");
            }
        }

        [TestMethod]
        public void CalculateBaseEcodistrictValue_Cache_Stable_Across_Identical_Calls()
        {
            // Arrange
            double topo =0.0123;
            var texture = SoilTexture.Coarse;
            var region = Region.WesternCanada;

            var cacheField = typeof(N2OEmissionFactorCalculator).GetField("_baseEmissionFactorCalculationCache", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var cache = (System.Collections.IDictionary)cacheField.GetValue(_sut);
            var before = cache.Count;

            // Act
            var v1 = _sut.CalculateBaseEcodistrictValue(topo, texture, region);
            var afterFirst = cache.Count;
            var v2 = _sut.CalculateBaseEcodistrictValue(topo, texture, region);
            var afterSecond = cache.Count;

            // Assert
            Assert.AreEqual(v1, v2,0.0, "Cached base ecodistrict value should be stable across identical calls.");
            Assert.IsTrue(afterFirst >= before +1, "First call should add a cache entry.");
            Assert.AreEqual(afterFirst, afterSecond, "Second identical call should not grow the cache.");
        }

        [TestMethod]
        public void CalculateBaseEcodistrictValue_NewEntry_When_InputsChange()
        {
            // Arrange
            double topo1 =0.02;
            double topo2 =0.03; // different topography emission
            var texture = SoilTexture.Fine;
            var region = Region.EasternCanada;

            var cacheField = typeof(N2OEmissionFactorCalculator).GetField("_baseEmissionFactorCalculationCache", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var cache = (System.Collections.IDictionary)cacheField.GetValue(_sut);
            var before = cache.Count;

            // Act
            var v1 = _sut.CalculateBaseEcodistrictValue(topo1, texture, region);
            var afterFirst = cache.Count;
            var v2 = _sut.CalculateBaseEcodistrictValue(topo2, texture, region);
            var afterSecond = cache.Count;

            // Assert
            Assert.IsTrue(afterFirst >= before +1, "First call should add a cache entry.");
            Assert.IsTrue(afterSecond >= afterFirst +1, "Changing an input should create a new cache entry.");

            if (Math.Abs(v1 - v2) <1e-12)
            {
                Assert.Inconclusive("Values equal for these inputs, but cache recorded a distinct key as expected.");
            }
        }
    }
}
