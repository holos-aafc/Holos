using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using H.CLI.TemporaryComponentStorage;
using H.Core.Calculators.Nitrogen;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using H.Core.Providers.Animals;
using Moq;

namespace H.Core.Test.Calculators.Nitrogen
{
    [TestClass]
    public class N2OEmissionFactorCalculatorTest : UnitTestBase
    {
        #region Fields

        private N2OEmissionFactorCalculator _sut;

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
        public void CalculateLeftOverEmissionsForField()
        {
            var farm = new Farm();
            var stageState = new FieldSystemDetailsStageState();
            farm.StageStates.Add(stageState);

            var viewItem1 = new CropViewItem() {Year = 2021};
            stageState.DetailsScreenViewCropViewItems.Add(viewItem1);

            var viewItem2 = new CropViewItem() { Year = 2021 };
            stageState.DetailsScreenViewCropViewItems.Add(viewItem2);

            var manureApplication = new ManureApplicationViewItem()
            {
                DateOfApplication = new DateTime(2021, 1, 1),
                ManureLocationSourceType = ManureLocationSourceType.Livestock,
                AmountOfNitrogenAppliedPerHectare = 50
            };

            viewItem1.ManureApplicationViewItems.Add(manureApplication);

            var groupEmissionsByDay = new GroupEmissionsByDay() { AdjustedAmountOfTanInStoredManureOnDay = 50, OrganicNitrogenCreatedOnDay = 50};
            var animalResults = new AnimalComponentEmissionsResults()
            {
                EmissionResultsForAllAnimalGroupsInComponent = new List<AnimalGroupEmissionResults>()
                {
                    new AnimalGroupEmissionResults()
                    {
                        GroupEmissionsByMonths = new List<GroupEmissionsByMonth>()
                        {
                            new GroupEmissionsByMonth(new MonthsAndDaysData(),
                                new List<GroupEmissionsByDay>() {groupEmissionsByDay})
                        }
                    }
                }
            };

            var result = _sut.CalculateDirectN2ONFromLeftOverManure(
                new List<AnimalComponentEmissionsResults>() {animalResults}, 
                farm, 
                viewItem1,
                0.25);

            Assert.AreEqual(6.25, result);
        }

        [TestMethod]
        public void CalculateTotalN2ONFromExportedManure()
        {
            var farm = base.GetTestFarm();
            farm.StageStates.Add(base.GetFieldStageState());

            var result = _sut.CalculateTotalDirectN2ONFromExportedManure(100, 0.5);

            Assert.AreEqual(result, 50);
        }

        [TestMethod]
        public void CalculateAmountOfTANFromExportedManureTest()
        {
            var farm = base.GetTestFarm();
            var animalResults = base.GetNonEmptyTestAnimalComponentEmissionsResults();

            _sut = new N2OEmissionFactorCalculator();

            var result = _sut.CalculateAmountOfTANFromExportedManure(farm, new List<AnimalComponentEmissionsResults>() {animalResults}, DateTime.Now.Year);

            Assert.AreEqual(100.03, result);
        }

        [TestMethod]
        public void CalculateIndirectEmissionsFromFieldAppliedManure()
        {
            var farm = base.GetTestFarm();
            var viewItem = base.GetTestCropViewItem();
            var animalResults = new List<AnimalComponentEmissionsResults>(){base.GetNonEmptyTestAnimalComponentEmissionsResults()};

            _mockManureService.Setup(x => x.GetTotalVolumeCreated(It.IsAny<int>(), It.IsAny<AnimalType>())).Returns(100);
            _mockManureService.Setup(x => x.GetTotalTANCreated(It.IsAny<int>(), It.IsAny<AnimalType>())).Returns(50);
            _mockManureService.Setup(x => x.GetTotalNitrogenCreated(It.IsAny<int>(), It.IsAny<AnimalType>())).Returns(75);

            _mockClimateProvider.Setup(x => x.GetMeanTemperatureForDay(It.IsAny<Farm>(), It.IsAny<DateTime>())).Returns(19);
            _mockClimateProvider.Setup(x => x.GetAnnualPrecipitation(It.IsAny<Farm>(), It.IsAny<DateTime>())).Returns(12);
            _mockClimateProvider.Setup(x => x.GetAnnualEvapotranspiration(It.IsAny<Farm>(), It.IsAny<DateTime>())).Returns(8);
            _mockClimateProvider.Setup(x => x.GetGrowingSeasonEvapotranspiration(It.IsAny<Farm>(), It.IsAny<DateTime>())).Returns(2);
            _mockClimateProvider.Setup(x => x.GetGrowingSeasonPrecipitation(It.IsAny<Farm>(), It.IsAny<DateTime>())).Returns(3);

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

            var firstItem = result[0];

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(37.5, firstItem.ActualAmountOfNitrogenAppliedFromLandApplication);
            Assert.AreEqual(29.44, firstItem.AdjustedAmmoniaLoss, 2);
            Assert.AreEqual(24.25, firstItem.AdjustedAmmoniacalLoss, 2);
            Assert.AreEqual(30.35, firstItem.AmmoniaLoss, 2);
            Assert.AreEqual(25, firstItem.AmmoniacalLoss, 2);
            Assert.AreEqual(10.01, firstItem.TotalIndirectN2OEmissions, 2);
            Assert.AreEqual(6.37, firstItem.TotalIndirectN2ONEmissions, 2);
            Assert.AreEqual(1.17, firstItem.TotalN2OFromManureVolatilized, 2);
            Assert.AreEqual(5.62, firstItem.TotalN2ONFromManureLeaching, 2);
            Assert.AreEqual(0.75, firstItem.TotalN2ONFromManureVolatilized, 2);
            Assert.AreEqual(5.62, firstItem.TotalNitrateLeached, 2);
        }

        #endregion
    }
}
