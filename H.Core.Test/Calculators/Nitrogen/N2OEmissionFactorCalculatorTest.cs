using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using H.Core.Calculators.Nitrogen;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;

namespace H.Core.Test.Calculators.Nitrogen
{
    [TestClass]
    public class N2OEmissionFactorCalculatorTest
    {
        #region Fields

        private N2OEmissionFactorCalculator _calculator;

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
            _calculator = new N2OEmissionFactorCalculator();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void CalculateTopographyEmissionsReturnsZeroWhenNoClimateDataAvailable()
        {
            var result = _calculator.CalculateTopographyEmissions(
                fractionOfLandOccupiedByLowerPortionsOfLandscape: 0.1,
                growingSeasonPrecipitation: 0,
                growingSeasonEvapotranspiration: 0);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void CalculateSyntheticFertilizerApplied()
        {
            var firstRate = _calculator.CalculateSyntheticFertilizerApplied(
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
            var secondRate = _calculator.CalculateSyntheticFertilizerApplied(
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

            var result = _calculator.CalculateFractionOfNitrogenLostByVolatilization(
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

            var groupEmissionsByDay = new GroupEmissionsByDay() {NitrogenAvailableForLandApplication = 100};
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

            var result = _calculator.CalculateLeftOverEmissionsForField(
                new List<AnimalComponentEmissionsResults>() {animalResults}, 
                farm, 
                viewItem1,
                0.25);

            Assert.AreEqual(6.25, result);
        }

        #endregion
    }
}
