﻿using H.Core.Calculators.Infrastructure;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Infrastructure;
using H.Core.Providers.Climate;

namespace H.Core.Test.Calculators.Infrastructure;

[TestClass]
public class ADCalculatorTest : UnitTestBase
{
    #region Fields

    private ADCalculator _sut;
    private Farm _farm;
    private List<AnimalComponentEmissionsResults> _animalComponentResults;
    private GroupEmissionsByDay _day1Emissions;
    private GroupEmissionsByDay _day2Emissions;
    private AnaerobicDigestionComponent _component;
    private ManagementPeriod _managementPeriod1;
    private ManagementPeriod _managementPeriod2;
    private FarmResiduesSubstrateViewItem _farmResidue1;
    private FarmResiduesSubstrateViewItem _farmResidue2;
    private BiogasAndMethaneProductionParametersData _biogasData;

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
        _sut = new ADCalculator();

        _farm = new Farm();

        var residuesSubstrateViewItem = new FarmResiduesSubstrateViewItem
            { FarmResidueType = FarmResidueType.BarleyStraw };
        residuesSubstrateViewItem.FlowRate = 0.5;
        residuesSubstrateViewItem.TotalSolids = 0.5;
        residuesSubstrateViewItem.VolatileSolids = 0.1;
        residuesSubstrateViewItem.TotalNitrogen = 0.5;
        residuesSubstrateViewItem.TotalCarbon = 0.5;
        residuesSubstrateViewItem.BiomethanePotential = 0.5;
        residuesSubstrateViewItem.MethaneFraction = 0.5;

        var manureSubstrateViewItem = new ManureSubstrateViewItem
            { AnimalType = AnimalType.Beef, ManureSubstrateState = ManureSubstrateState.Fresh };
        manureSubstrateViewItem.FlowRate = 1.25;
        manureSubstrateViewItem.TotalSolids = 0.75;
        manureSubstrateViewItem.VolatileSolids = 0.1;
        manureSubstrateViewItem.TotalNitrogen = 0.33;
        manureSubstrateViewItem.TotalCarbon = 0.11;
        manureSubstrateViewItem.BiomethanePotential = 0.22;
        manureSubstrateViewItem.MethaneFraction = 2;

        _biogasData = new BiogasAndMethaneProductionParametersData
        {
            TotalSolids = 0.75,
            VolatileSolids = 0.1,
            TotalNitrogen = 0.33,
            BioMethanePotential = 0.22,
            MethaneFraction = 2
        };

        _farmResidue1 = new FarmResiduesSubstrateViewItem { FarmResidueType = FarmResidueType.BarleyStraw };
        _farmResidue2 = new FarmResiduesSubstrateViewItem { FarmResidueType = FarmResidueType.GrassClippings };

        _component = new AnaerobicDigestionComponent();
        _component.HydraulicRetentionTimeInDays = 30;
        _component.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems.Add(residuesSubstrateViewItem);
        _component.AnaerobicDigestionViewItem.ManureSubstrateViewItems.Add(manureSubstrateViewItem);

        _farm.Components.Add(_component);

        _managementPeriod1 = new ManagementPeriod();
        _managementPeriod1.AnimalType = AnimalType.Beef;
        _managementPeriod1.ManureDetails.StateType = ManureStateType.AnaerobicDigester;

        _managementPeriod2 = new ManagementPeriod();
        _managementPeriod2.AnimalType = AnimalType.Dairy;
        _managementPeriod2.ManureDetails.StateType = ManureStateType.AnaerobicDigester;

        var dailyEmissions = new List<GroupEmissionsByDay>();

        _day1Emissions = new GroupEmissionsByDay();
        _day1Emissions.DateTime = DateTime.Now;
        _day1Emissions.TotalVolumeOfManureAvailableForLandApplication = 100;
        _day1Emissions.VolatileSolids = 0.01;
        _day1Emissions.AmountOfNitrogenExcreted = 10;
        _day1Emissions.AmountOfNitrogenAddedFromBedding = 10;
        _day1Emissions.AmountOfCarbonInStoredManure = 10;
        _day1Emissions.OrganicNitrogenInStoredManure = 20;
        _day1Emissions.TanExcretion = 10;
        _day1Emissions.CarbonFromManureAndBedding = 12;

        _day2Emissions = new GroupEmissionsByDay();
        _day1Emissions.DateTime = DateTime.Now.AddDays(-20);
        _day2Emissions.TotalVolumeOfManureAvailableForLandApplication = 50;
        _day2Emissions.VolatileSolids = 0.02;
        _day2Emissions.AmountOfNitrogenExcreted = 20;
        _day2Emissions.AmountOfNitrogenAddedFromBedding = 20;
        _day2Emissions.OrganicNitrogenInStoredManure = 30;
        _day1Emissions.AmountOfCarbonInStoredManure = 10;
        _day2Emissions.TanExcretion = 20;
        _day2Emissions.CarbonFromManureAndBedding = 16;

        dailyEmissions.Add(_day1Emissions);
        dailyEmissions.Add(_day2Emissions);

        _animalComponentResults = new List<AnimalComponentEmissionsResults>();

        var beefComponentEmissionResults = new AnimalComponentEmissionsResults
        {
            EmissionResultsForAllAnimalGroupsInComponent = new List<AnimalGroupEmissionResults>
            {
                new()
                {
                    GroupEmissionsByMonths = new List<GroupEmissionsByMonth>
                    {
                        new(new MonthsAndDaysData { ManagementPeriod = _managementPeriod1 },
                            dailyEmissions)
                    }
                }
            }
        };

        var dairyComponentEmissionResults = new AnimalComponentEmissionsResults
        {
            EmissionResultsForAllAnimalGroupsInComponent = new List<AnimalGroupEmissionResults>
            {
                new()
                {
                    GroupEmissionsByMonths = new List<GroupEmissionsByMonth>
                    {
                        new(new MonthsAndDaysData { ManagementPeriod = _managementPeriod2 },
                            dailyEmissions)
                    }
                }
            }
        };

        _animalComponentResults.Add(beefComponentEmissionResults);
        _animalComponentResults.Add(dairyComponentEmissionResults);
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    #endregion

    #region Tests

    [TestMethod]
    public void CombineSubstrateFlowsOfSameTypeOnSameDayReturnsCorrectNumberOfItems()
    {
        var substrateFlows = new List<SubstrateFlowInformation>
        {
            new() { DateCreated = DateTime.Now },
            new() { DateCreated = DateTime.Now },
            new() { DateCreated = DateTime.Now.AddDays(23) }
        };

        var result = _sut.CombineSubstrateFlowsOfSameTypeOnSameDay(substrateFlows, _component, _farm);

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetFreshManureFlowRatesFromAnimals()
    {
        var results = _sut.GetFreshManureFlowRate(_component, _day1Emissions,
            new ADManagementPeriodViewItem { ManagementPeriod = _managementPeriod1 }, new Farm());

        Assert.AreEqual(10, results.CarbonFlowOfSubstrate, 0.00001);
    }

    [TestMethod]
    public void GetStoredManureFlowRateTest()
    {
        _managementPeriod1.ManureDetails.StateType = ManureStateType.DeepBedding;

        var results = _sut.GetStoredManureFlowRate(_component, _day1Emissions,
            new ADManagementPeriodViewItem { ManagementPeriod = _managementPeriod1 }, new GroupEmissionsByDay(),
            GetTestFarm());

        Assert.AreEqual(0.07, results.VolatileSolidsFlowOfSubstrate, 0.00001);
    }

    [TestMethod]
    public void GetFlowsFromDailyResultsReturnsCorrectNumberOfItems()
    {
        _component.ManagementPeriodViewItems.Add(new ADManagementPeriodViewItem
            { ManagementPeriod = _managementPeriod1, IsSelected = true });
        _component.ManagementPeriodViewItems.Add(new ADManagementPeriodViewItem
            { ManagementPeriod = _managementPeriod2, IsSelected = true });

        var results = _sut.GetDailyManureFlowRates(_farm, _animalComponentResults, _component);

        // 2 daily emissions from the beef results, 2 from the dairy results
        Assert.AreEqual(4, results.Count);
    }

    [TestMethod]
    public void GetDailyFarmResidueFlowRatesReturnsCorrectNumberOfItemsForFarmResidues()
    {
        _component.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems.Clear();
        _component.AnaerobicDigestionViewItem.ManureSubstrateViewItems.Clear();

        _farmResidue1.StartDate = DateTime.Now;
        _farmResidue1.EndDate = DateTime.Now.AddDays(3);
        _component.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems.Add(_farmResidue1);

        var results = _sut.GetDailyFarmResidueFlowRates(_component);

        Assert.AreEqual(4, results.Count);
    }

    [TestMethod]
    public void CalculateTotalFlowsTest()
    {
        var flowRates = new List<SubstrateFlowInformation>
        {
            new() { TotalMassFlowOfSubstrate = 30 },
            new() { TotalMassFlowOfSubstrate = 20 }
        };

        var dailyOutput = new DigestorDailyOutput();

        _sut.CalculateTotalProductionFromAllSubstratesOnSameDay(dailyOutput, flowRates);

        Assert.AreEqual(50, dailyOutput.FlowRateOfAllSubstratesInDigestate);
    }

    [TestMethod]
    public void CombineSubstrateFlowsOfSameTypeOnSameDaySetsCorrectDate()
    {
        var flow1 = new SubstrateFlowInformation { DateCreated = DateTime.Now };
        var flow2 = new SubstrateFlowInformation { DateCreated = DateTime.Now.AddDays(30) };

        var flowRates = new List<SubstrateFlowInformation>
        {
            flow1, flow2
        };

        var result = _sut.CombineSubstrateFlowsOfSameTypeOnSameDay(flowRates, _component, _farm);

        Assert.IsTrue(result.Any(x => x.Date.Date.Equals(flow1.DateCreated.Date)));
        Assert.IsTrue(result.Any(x => x.Date.Date.Equals(flow2.DateCreated.Date)));
    }

    [TestMethod]
    public void GetFarmResidueFlowRatesReturnsCorrectNumberOfFlows()
    {
        _component.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems.Clear();

        _farmResidue1.StartDate = DateTime.Now;
        _farmResidue1.EndDate = DateTime.Now.AddDays(1);
        _component.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems.Add(_farmResidue1);

        _farmResidue2.StartDate = DateTime.Now;
        _farmResidue2.EndDate = DateTime.Now.AddDays(1);
        _component.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems.Add(_farmResidue2);

        var flows = _sut.GetDailyFarmResidueFlowRates(_component);

        Assert.AreEqual(4, flows.Count);
    }

    [TestMethod]
    public void GetFarmResidueFlowRatesReturnsCalculatedValues()
    {
        _component.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems.Clear();

        _farmResidue1.FlowRate = 5;
        _farmResidue1.TotalSolids = 10;
        _farmResidue1.VolatileSolids = 14;
        _farmResidue1.TotalNitrogen = 10;
        _farmResidue1.TotalCarbon = 0.5;
        _farmResidue1.MethaneFraction = 0.1;
        _farmResidue1.BiomethanePotential = 0.22;

        _farmResidue2.FlowRate = 20;
        _farmResidue2.TotalSolids = 30;
        _farmResidue2.VolatileSolids = 7;
        _farmResidue2.TotalNitrogen = 5;
        _farmResidue2.TotalCarbon = 0.25;
        _farmResidue2.MethaneFraction = 0.05;
        _farmResidue2.BiomethanePotential = 0.11;

        _farmResidue1.StartDate = DateTime.Now;
        _farmResidue1.EndDate = DateTime.Now.AddDays(1);
        _component.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems.Add(_farmResidue1);

        _farmResidue2.StartDate = DateTime.Now;
        _farmResidue2.EndDate = DateTime.Now.AddDays(1);
        _component.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems.Add(_farmResidue2);

        var flows = _sut.GetDailyFarmResidueFlowRates(_component);

        Assert.AreEqual(4, flows.Count);

        Assert.AreEqual(0.025, flows[0].CarbonFlowOfSubstrate);
        Assert.AreEqual(0.05, flows[1].NitrogenFlowOfSubstrate);
    }

    [TestMethod]
    public void GetAmountOfNitrogenInFieldApplicationTest()
    {
    }

    [TestMethod]
    public void GetDailyCropResidueFlowRatesTestReturnsNonEmptyList()
    {
        var adComponent = new AnaerobicDigestionComponent();
        adComponent.AnaerobicDigestionViewItem.CropResiduesSubstrateViewItems.Add(
            new CropResidueSubstrateViewItem());

        var result = _sut.GetDailyCropResidueFlowRates(adComponent);

        Assert.AreEqual(2, result.Count);
    }

    #endregion
}