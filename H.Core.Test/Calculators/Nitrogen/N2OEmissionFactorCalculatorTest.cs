using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using H.CLI.TemporaryComponentStorage;
using H.Core.Calculators.Nitrogen;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using H.Core.Providers.Animals;
using H.Views.ComponentViews.LandManagement.FieldSystem.Controls;
using Moq;
using NitrogenFertilizerType = H.Core.Enumerations.NitrogenFertilizerType;
using H.Core.Calculators.Carbon;
using H.Core.Providers.Climate;
using H.Core.Services.LandManagement;

namespace H.Core.Test.Calculators.Nitrogen
{
    [TestClass]
    public class N2OEmissionFactorCalculatorTest : UnitTestBase
    {
        #region Fields

        private N2OEmissionFactorCalculator _sut;
        private Table_36_Livestock_Emission_Conversion_Factors_Data _emissionFactors;

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
            _sut.ManureService = base._mockManureServiceObject;
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
                    new List<Tuple<double, AnimalType>>() {new Tuple<double, AnimalType>(100, AnimalType.Beef)});

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
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

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

        /// <summary>
        ///    Equation 2.5.3-1
        /// </summary>
        [TestMethod]
        public void CalculateFractionOfNitrogenLostByLeachingAndRunoffReturnsCorrectValue()
        {
            var growingSeasonPrecipitation = 34.2;
            var growingSeasonEvapotranspiration = 0.65;
            var result =
                _sut.CalculateFractionOfNitrogenLostByLeachingAndRunoff(
                    growingSeasonPrecipitation,
                    growingSeasonEvapotranspiration);

            Assert.AreEqual(0.3, result);
        }

        [TestMethod]
        public void CalculateTopographyEmissionsReturnsZeroWhenNoClimateDataAvailable()
        {
            var result = _sut.CalculateTopographyEmissions(
                fractionOfLandOccupiedByLowerPortionsOfLandscape: 0.1,
                growingSeasonPrecipitation: 0,
                growingSeasonEvapotranspiration: 0);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void CalculateSyntheticFertilizerApplied()
        {
            var firstRate = _sut.CalculateSyntheticFertilizerApplied(
                nitrogenContentOfGrainReturnedToSoil: 100,
                nitrogenContentOfStrawReturnedToSoil: 200,
                nitrogenContentOfRootReturnedToSoil: 300,
                nitrogenContentOfExtrarootReturnedToSoil: 200,
                fertilizerEfficiencyFraction: 0.5,
                soilTestN: 10,
                isNitrogenFixingCrop: false,
                nitrogenFixationAmount: 0,
                atmosphericNitrogenDeposition: 0);

            Assert.AreEqual(1580, firstRate);

            // Increasing efficiency should reduce the required amount of fertilizer
            var secondRate = _sut.CalculateSyntheticFertilizerApplied(
                nitrogenContentOfGrainReturnedToSoil: 100,
                nitrogenContentOfStrawReturnedToSoil: 200,
                nitrogenContentOfRootReturnedToSoil: 300,
                nitrogenContentOfExtrarootReturnedToSoil: 200,
                fertilizerEfficiencyFraction: 0.75,
                soilTestN: 10,
                isNitrogenFixingCrop: false,
                nitrogenFixationAmount: 0,
                atmosphericNitrogenDeposition: 0);

            Assert.IsTrue(secondRate < firstRate);
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

            var result = _sut.CalculateTotalDirectN2ONFromExportedManureForFarmAndYear(100, 0.5);

            Assert.AreEqual(result, 50);
        }

        [TestMethod]
        public void CalculateTotalIndirectEmissionsFromFieldSpecificManureApplications()
        {
            var farm = base.GetTestFarm();
            var viewItem = base.GetTestCropViewItem();

            var manureNitrogenFromLandApplication = _sut.GetAmountOfManureNitrogenUsed(viewItem);
            Assert.AreEqual(100, manureNitrogenFromLandApplication, 2);

            var ammoniacalLoss = _sut.CalculateAmmoniacalLossFromManureForField(farm, viewItem);
            Assert.AreEqual(25, ammoniacalLoss, 2);

            var ammoniaLoss = _sut.CalculateAmmoniaLossFromManureForField(farm, viewItem);
            Assert.AreEqual(30.35, ammoniaLoss, 2);

            var n2oNFromManureVolatilized =
                _sut.CalculateN2ONFromVolatilizationOfFarmSourcedLandAppliedManureForField(viewItem.Year, farm, viewItem);
            Assert.AreEqual(5, n2oNFromManureVolatilized, 2);

            var n2oFromManureVolatilized = _sut.CalculateTotalManureN2OVolatilizationForField(viewItem, farm, viewItem.Year);
            Assert.AreEqual(7.85, n2oFromManureVolatilized, 2);

            var adjustedAmmoniacalLoss = _sut.CalculateTotalAdjustedAmmoniaEmissionsFromLandAppliedManureForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(25 - 5, adjustedAmmoniacalLoss);

            var adjustedNH3Loss =
                _sut.CalculateTotalAdjustedAmmoniaEmissionsFromManureForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(CoreConstants.ConvertToNH3(adjustedAmmoniacalLoss), adjustedNH3Loss);

            var totalN2OFormManureLeaching = _sut.CalculateTotalN2ONFromManureLeachingForField(farm, viewItem);
            Assert.AreEqual(0.33, totalN2OFormManureLeaching, 2);

            var totalNitrateLeached = _sut.CalculateTotalManureNitrateLeached(farm, viewItem);
            Assert.AreEqual(29.67, totalNitrateLeached, 2);

            var totalIndirectEmissions =
                _sut.CalculateTotalIndirectEmissionsFromManureForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(29.67 + 5, totalIndirectEmissions);
        }

        [TestMethod]
        public void CalculateTotalIndirectEmissionsFromFieldSpecificManureApplicationsUsingMultipleAnimalResults()
        {
            var farm = base.GetTestFarm();
            var viewItem = base.GetTestCropViewItem();

            var dairyApplication = base.GetTestBeefCattleManureApplicationViewItemUsingOnLivestockManure();
            dairyApplication.AnimalType = AnimalType.DairyLactatingCow;
            dairyApplication.AmountOfManureAppliedPerHectare = 6000;

            viewItem.ManureApplicationViewItems.Add(dairyApplication);

            var manureNitrogenFromLandApplication = _sut.GetAmountOfManureNitrogenUsed(viewItem);
            Assert.AreEqual(200, manureNitrogenFromLandApplication, 2);

            var ammoniacalLoss = _sut.CalculateAmmoniacalLossFromManureForField(farm, viewItem);
            Assert.AreEqual(50, ammoniacalLoss, 2);

            var ammoniaLoss = _sut.CalculateAmmoniaLossFromManureForField(farm, viewItem);
            Assert.AreEqual(60.71, ammoniaLoss, 2);

            var n2oNFromManureVolatilized =
                _sut.CalculateN2ONFromVolatilizationOfFarmSourcedLandAppliedManureForField(viewItem.Year, farm, viewItem);
            Assert.AreEqual(10, n2oNFromManureVolatilized, 2);

            var n2oFromManureVolatilized = _sut.CalculateTotalManureN2OVolatilizationForField(viewItem, farm, viewItem.Year);
            Assert.AreEqual(15.71, n2oFromManureVolatilized, 2);

            var adjustedAmmoniacalLoss = _sut.CalculateTotalAdjustedAmmoniaEmissionsFromLandAppliedManureForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(50 - 10, adjustedAmmoniacalLoss);

            var adjustedNH3Loss =
                _sut.CalculateTotalAdjustedAmmoniaEmissionsFromManureForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(CoreConstants.ConvertToNH3(adjustedAmmoniacalLoss), adjustedNH3Loss);

            var totalN2OFormManureLeaching = _sut.CalculateTotalN2ONFromManureLeachingForField(farm, viewItem);
            Assert.AreEqual(0.66, totalN2OFormManureLeaching, 2);

            var totalNitrateLeached = _sut.CalculateTotalManureNitrateLeached(farm, viewItem);
            Assert.AreEqual(59.34, totalNitrateLeached, 2);

            var totalIndirectEmissions =
                _sut.CalculateTotalIndirectEmissionsFromManureForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(59.34 + 10, totalIndirectEmissions);
        }

        [TestMethod]
        public void CalculateAmmoniaFromLandApplicationForImportedManure()
        {
            var farm = base.GetTestFarm();
            var viewItem = base.GetTestCropViewItem();
            viewItem.ManureApplicationViewItems.Clear();
            viewItem.ManureApplicationViewItems.Add(base.GetTestBeefCattleManureApplicationViewItemUsingImportedManure());

            var manureNitrogenFromLandApplication = _sut.GetAmountOfManureNitrogenUsed(viewItem);
            Assert.AreEqual(50, manureNitrogenFromLandApplication, 2);

            var ammoniacalLoss = _sut.CalculateAmmoniacalLossFromManureForField(farm, viewItem);
            Assert.AreEqual(5, ammoniacalLoss, 2);

            var ammoniaLoss = _sut.CalculateAmmoniaLossFromManureForField(farm, viewItem);
            Assert.AreEqual(6.07, ammoniaLoss, 2);

            var n2oNFromManureVolatilized =
                _sut.CalculateAmmoniaEmissionsFromImportedManureForField(farm, viewItem, viewItem.Year)
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
            Assert.AreEqual(14.84 + 5, totalIndirectEmissions, 2);
        }

        [TestMethod]
        public void CalculateAmmoniaFromLandApplicationForMultipleImportedManureApplications()
        {
            var farm = base.GetTestFarm();
            var viewItem = base.GetTestCropViewItem();
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
                _sut.CalculateAmmoniaEmissionsFromImportedManureForField(farm, viewItem, viewItem.Year)
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
            Assert.AreEqual(39.67, totalIndirectEmissions, 2);
        }

        [TestMethod]
        public void CalculateAmmoniaFromLandApplicationForDigestate()
        {
            var farm = base.GetTestFarm();
            var viewItem = base.GetTestCropViewItem();
            viewItem.DigestateApplicationViewItems.Clear();
            viewItem.DigestateApplicationViewItems.Add(base.GetTestRawDigestateApplicationViewItem());

            var digestateN = _sut.GetAmountOfManureNitrogenUsed(viewItem);
            Assert.AreEqual(100, digestateN, 2);

            var ammoniacalLoss = _sut.CalculateNH3NLossFromFarmSourcedLandAppliedDigestateForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(10, ammoniacalLoss, 2);

            var ammoniaLoss = _sut.CalculateAmmoniaLossFromManureForField(farm, viewItem);
            Assert.AreEqual(30.35, ammoniaLoss, 2);

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
                _sut.CalculateTotalIndirectEmissionsFromDigestateForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(16.54, totalIndirectEmissions, 2);
        }

        [TestMethod]
        public void CalculateAmmoniaFromLandApplicationForMultipleDigestateApplications()
        {
            var farm = base.GetTestFarm();
            var viewItem = base.GetTestCropViewItem();
            viewItem.DigestateApplicationViewItems.Clear();
            viewItem.DigestateApplicationViewItems.Add(base.GetTestRawDigestateApplicationViewItem());
            viewItem.DigestateApplicationViewItems.Add(base.GetTestLiquidDigestateApplicationViewItem());

            var digestateN = _sut.GetAmountOfManureNitrogenUsed(viewItem);
            Assert.AreEqual(100, digestateN, 2);

            var ammoniacalLoss = _sut.CalculateNH3NLossFromFarmSourcedLandAppliedDigestateForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(93.77, ammoniacalLoss, 2);

            var ammoniaLoss = _sut.CalculateAmmoniaLossFromManureForField(farm, viewItem);
            Assert.AreEqual(30.35, ammoniaLoss, 2);

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
                _sut.CalculateTotalIndirectEmissionsFromDigestateForField(farm, viewItem, viewItem.Year);
            Assert.AreEqual(181.94, totalIndirectEmissions, 2);
        }

        [TestMethod]
        public void CalculateAmmoniaEmissionsFromExportedManure()
        {
            var farm = base.GetTestFarm();

            _mockManureService.Setup(x => x.GetManureTypesExported(It.IsAny<Farm>(), It.IsAny<int>())).Returns(new List<AnimalType>() {AnimalType.Beef});
            _mockManureService.Setup(x => x.GetTotalNitrogenFromExportedManure(It.IsAny<int>(), It.IsAny<Farm>(), It.IsAny<AnimalType>())).Returns(100);

            var result = _sut.CalculateAmmoniaEmissionsFromExportedManureForFarmAndYear(
                farm: farm,
                DateTime.Now.Year);

            Assert.AreEqual(10, result.Sum(x => x.Value));
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

            var result = _sut.CalculateAmmoniaEmissionsFromImportedManureForField(
                farm: farm,
                cropViewItem: cropViewItem,
                DateTime.Now.Year);

            Assert.AreEqual(5, result.Sum(x => x.Value));
        }

        #endregion
    }
}
