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
            _sut = new N2OEmissionFactorCalculator();
            _sut.ManureService = base._mockManureServiceObject;
            _sut.ClimateProvider = base._mockClimateProviderObject;
            _sut.LivestockEmissionConversionFactorsProvider = base._mockEmissionDataProviderObject;
            _sut.AnimalAmmoniaEmissionFactorProvider = base._mockAnimalAmmoniaEmissionFactorProviderObject;

            _mockManureService.Setup(x => x.GetTotalVolumeCreated(It.IsAny<int>(), It.IsAny<AnimalType>())).Returns(100);
            _mockManureService.Setup(x => x.GetTotalTANCreated(It.IsAny<int>(), It.IsAny<AnimalType>())).Returns(50);
            _mockManureService.Setup(x => x.GetTotalNitrogenCreated(It.IsAny<int>(), It.IsAny<AnimalType>())).Returns(75);
            _mockManureService.Setup(x => x.GetTotalNitrogenCreated(It.IsAny<int>())).Returns(75);
            _mockManureService.Setup(x => x.GetAmountOfTanUsedDuringLandApplication(It.IsAny<CropViewItem>(), It.IsAny<ManureApplicationViewItem>())).Returns(25);

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

        [TestMethod]
        public void CalculateAmmoniaEmissionsFromLandAppliedDigestateTest()
        {
            var viewItem = new CropViewItem();
            var farm = new Farm();

            var result = _sut.CalculateAmmoniaEmissionsFromLandAppliedDigestate(
                viewItem: viewItem,
                farm: farm);
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
        public void CalculateIndirectEmissionsFromFieldAppliedManureReturnsOneResultForOnlyManureApplicationCreated()
        {
            var farm = base.GetTestFarm();
            var viewItem = base.GetTestCropViewItem();
            var animalResults = new List<AnimalComponentEmissionsResults>(){base.GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults()};

            var emissionFactors = new Table_36_Livestock_Emission_Conversion_Factors_Data()
            {
                VolatilizationFraction = 0.10,
                EmissionFactorVolatilization = 0.2,
                EmissionFactorLeach = 0.5,
            };

            _mockEmissionDataProvider
                .Setup(x => x.GetLandApplicationFactors(It.IsAny<Farm>(), It.IsAny<double>(), It.IsAny<double>(),
                    It.IsAny<AnimalType>(), It.IsAny<int>()))
                .Returns(emissionFactors);

            _mockAnimalAmmoniaEmissionFactorProvider.Setup(x => x.GetAmmoniaEmissionFactorForSolidAppliedManure(It.IsAny<TillageType>())).Returns(1);
            _mockAnimalAmmoniaEmissionFactorProvider.Setup(x => x.GetAmmoniaEmissionFactorForLiquidAppliedManure(It.IsAny<ManureApplicationTypes>())).Returns(1);

            var result = _sut.CalculateIndirectEmissionsFromFieldAppliedManure(
                viewItem: viewItem,
                animalComponentEmissionsResults: animalResults,
                farm: farm);

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void CalculateIndirectEmissionsFromFieldAppliedManureReturnsTwoResults()
        {
            var farm = base.GetTestFarm();
            var viewItem = base.GetTestCropViewItem();
            viewItem.ManureApplicationViewItems.Add(base.GetTestBeefCattleManureApplicationViewItemUsingOnLivestockManure());

            var animalResults = new List<AnimalComponentEmissionsResults>() { base.GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults() };

            var emissionFactors = new Table_36_Livestock_Emission_Conversion_Factors_Data()
            {
                VolatilizationFraction = 0.10,
                EmissionFactorVolatilization = 0.2,
                EmissionFactorLeach = 0.5,
            };

            _mockEmissionDataProvider
                .Setup(x => x.GetLandApplicationFactors(It.IsAny<Farm>(), It.IsAny<double>(), It.IsAny<double>(),
                    It.IsAny<AnimalType>(), It.IsAny<int>()))
                .Returns(emissionFactors);

            _mockAnimalAmmoniaEmissionFactorProvider.Setup(x => x.GetAmmoniaEmissionFactorForSolidAppliedManure(It.IsAny<TillageType>())).Returns(1);
            _mockAnimalAmmoniaEmissionFactorProvider.Setup(x => x.GetAmmoniaEmissionFactorForLiquidAppliedManure(It.IsAny<ManureApplicationTypes>())).Returns(1);

            var result = _sut.CalculateIndirectEmissionsFromFieldAppliedManure(
                viewItem: viewItem,
                animalComponentEmissionsResults: animalResults,
                farm: farm);

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void CalculateIndirectEmissionsFromFieldAppliedManureReturnsCorrectEmissions()
        {
            var farm = base.GetTestFarm();
            var viewItem = base.GetTestCropViewItem();
            var animalResults = new List<AnimalComponentEmissionsResults>() { base.GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults() };

            var result = _sut.CalculateIndirectEmissionsFromFieldAppliedManure(
                viewItem: viewItem,
                animalComponentEmissionsResults: animalResults,
                farm: farm);

            var firstItem = result[0];

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(100, firstItem.ActualAmountOfNitrogenAppliedFromLandApplication);
            Assert.AreEqual(24.28, firstItem.AdjustedAmmoniaLoss, 2);
            Assert.AreEqual(20, firstItem.AdjustedAmmoniacalLoss, 2);
            Assert.AreEqual(30.35, firstItem.AmmoniaLoss, 2);
            Assert.AreEqual(25, firstItem.AmmoniacalLoss, 2);
            Assert.AreEqual(7.85, firstItem.TotalIndirectN2OEmissions, 2);
            Assert.AreEqual(5, firstItem.TotalIndirectN2ONEmissions, 2);
            Assert.AreEqual(7.85, firstItem.TotalN2OFromManureVolatilized, 2);
            Assert.AreEqual(0.33, firstItem.TotalN2ONFromManureLeaching, 2);
            Assert.AreEqual(5, firstItem.TotalN2ONFromManureVolatilized, 2);
            Assert.AreEqual(29.67, firstItem.TotalNitrateLeached, 2);
        }

        [TestMethod]
        public void CalculateTotalIndirectEmissionsFromFieldSpecificManureApplications()
        {
            var farm = base.GetTestFarm();
            var viewItem = base.GetTestCropViewItem();

            var animalResults = new List<AnimalComponentEmissionsResults>();

            var beefCattleResults = base.GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults();

            animalResults.Add(beefCattleResults);

            var result = _sut.CalculateTotalIndirectEmissionsFromFieldSpecificManureApplications(
                viewItem: viewItem,
                animalComponentEmissionsResults: animalResults,
                farm: farm);

            Assert.AreEqual(100, result.ActualAmountOfNitrogenAppliedFromLandApplication);
            Assert.AreEqual(24.28, result.AdjustedAmmoniaLoss, 2);
            Assert.AreEqual(20, result.AdjustedAmmoniacalLoss, 2);
            Assert.AreEqual(30.35, result.AmmoniaLoss, 2);
            Assert.AreEqual(25, result.AmmoniacalLoss, 2);
            Assert.AreEqual(8.37, result.TotalIndirectN2OEmissions, 2);
            Assert.AreEqual(5.33, result.TotalIndirectN2ONEmissions, 2);
            Assert.AreEqual(7.85, result.TotalN2OFromManureVolatilized, 2);
            Assert.AreEqual(0.33, result.TotalN2ONFromManureLeaching, 2);
            Assert.AreEqual(5, result.TotalN2ONFromManureVolatilized, 2);
            Assert.AreEqual(29.67, result.TotalNitrateLeached, 2);
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

            var animalResults = new List<AnimalComponentEmissionsResults>();

            var beefCattleResults = base.GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults();
            var dairyCattleResults = base.GetNonEmptyTestDairyCattleAnimalComponentEmissionsResults();

            animalResults.Add(beefCattleResults);
            animalResults.Add(dairyCattleResults);

            var result = _sut.CalculateTotalIndirectEmissionsFromFieldSpecificManureApplications(
                viewItem: viewItem,
                animalComponentEmissionsResults: animalResults,
                farm: farm);

            // Results should be sum from both manure applications
            Assert.AreEqual(200, result.ActualAmountOfNitrogenAppliedFromLandApplication);
            Assert.AreEqual(48.57, result.AdjustedAmmoniaLoss, 2);
            Assert.AreEqual(40, result.AdjustedAmmoniacalLoss, 2);
            Assert.AreEqual(60.71, result.AmmoniaLoss, 2);
            Assert.AreEqual(50, result.AmmoniacalLoss, 2);
            Assert.AreEqual(16.75, result.TotalIndirectN2OEmissions, 2);
            Assert.AreEqual(10.66, result.TotalIndirectN2ONEmissions, 2);
            Assert.AreEqual(15.71, result.TotalN2OFromManureVolatilized, 2);
            Assert.AreEqual(0.66, result.TotalN2ONFromManureLeaching, 2);
            Assert.AreEqual(10, result.TotalN2ONFromManureVolatilized, 2);
            Assert.AreEqual(59.34, result.TotalNitrateLeached, 2);
        }

        [TestMethod]
        public void CalculateAmmoniaFromLandApplicationForImportedManure()
        {
            var farm = base.GetTestFarm();
            var viewItem = base.GetTestCropViewItem();
            viewItem.ManureApplicationViewItems.Clear();
            viewItem.ManureApplicationViewItems.Add(base.GetTestBeefCattleManureApplicationViewItemUsingImportedManure());

            var beefCattleResults = base.GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults();

            var results = _sut.CalculateIndirectEmissionsFromFieldAppliedManure(viewItem, new List<AnimalComponentEmissionsResults>() {beefCattleResults}, farm);
            var firstItem = results[0];

            Assert.AreEqual(50, firstItem.ActualAmountOfNitrogenAppliedFromLandApplication);
            Assert.AreEqual(4.85, firstItem.AdjustedAmmoniaLoss, 2);
            Assert.AreEqual(4, firstItem.AdjustedAmmoniacalLoss, 2);
            Assert.AreEqual(6.07, firstItem.AmmoniaLoss, 2);
            Assert.AreEqual(5, firstItem.AmmoniacalLoss, 2);
            Assert.AreEqual(2, firstItem.TotalIndirectN2OEmissions, 2);
            Assert.AreEqual(0.165, firstItem.TotalIndirectN2ONEmissions, 2);
            Assert.AreEqual(1.57, firstItem.TotalN2OFromManureVolatilized, 2);
            Assert.AreEqual(0.165, firstItem.TotalN2ONFromManureLeaching, 2);
            Assert.AreEqual(1, firstItem.TotalN2ONFromManureVolatilized, 2);
            Assert.AreEqual(14.83, firstItem.TotalNitrateLeached, 2);
        }

        [TestMethod]
        public void CalculateAmmoniaFromLandApplicationForMultipleImportedManureApplications()
        {
            var farm = base.GetTestFarm();
            var viewItem = base.GetTestCropViewItem();
            viewItem.ManureApplicationViewItems.Clear();
            viewItem.ManureApplicationViewItems.Add(base.GetTestBeefCattleManureApplicationViewItemUsingImportedManure());
            viewItem.ManureApplicationViewItems.Add(base.GetTestDairyCattleManureApplicationViewItemUsingImportedManure());

            var animalResults = new List<AnimalComponentEmissionsResults>();

            var beefCattleResults = base.GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults();
            var dairyCattleResults = base.GetNonEmptyTestDairyCattleAnimalComponentEmissionsResults();

            animalResults.Add(beefCattleResults);
            animalResults.Add(dairyCattleResults);

            var importedManureResults = _sut.CalculateIndirectEmissionsFromFieldAppliedManure(viewItem, animalResults, farm);
            var totalResultsForField = _sut.ConvertPerFieldEmissionsToPerHectare(importedManureResults, viewItem);

            Assert.AreEqual(100, totalResultsForField.ActualAmountOfNitrogenAppliedFromLandApplication);
            Assert.AreEqual(9.71, totalResultsForField.AdjustedAmmoniaLoss, 2);
            Assert.AreEqual(8, totalResultsForField.AdjustedAmmoniacalLoss, 2);
            Assert.AreEqual(12.14, totalResultsForField.AmmoniaLoss, 2);
            Assert.AreEqual(10, totalResultsForField.AmmoniacalLoss, 2);
            Assert.AreEqual(3.66, totalResultsForField.TotalIndirectN2OEmissions, 2);
            Assert.AreEqual(2.33, totalResultsForField.TotalIndirectN2ONEmissions, 2);
            Assert.AreEqual(2, totalResultsForField.TotalN2OFromManureVolatilized, 2);
            Assert.AreEqual(0.32, totalResultsForField.TotalN2ONFromManureLeaching, 2);
            Assert.AreEqual(2, totalResultsForField.TotalN2ONFromManureVolatilized, 2);
            Assert.AreEqual(29.66, totalResultsForField.TotalNitrateLeached, 2);
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
