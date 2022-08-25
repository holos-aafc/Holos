using H.CLI.Results;
using H.CLI.UserInput;
using H.Core.Emissions;
using H.Core.Enumerations;
using H.Core.Providers;
using H.Core.Providers.Feed;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using H.Core.Models.Animals.Swine;
using H.Core.Services;

namespace H.CLI.Test.Results
{
    
    [TestClass]
    public class ComponentResultsProcessorTest
    {
        private Storage storage = new Storage();

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));
            CLIUnitsOfMeasurementConstants.measurementSystem = MeasurementSystemType.Metric;
        }

        [ClassCleanup]
        public static void ClassCleanUp()
        {
            
        }

        [TestInitialize]
        public void TestInitialize()
        {
            storage.ApplicationData = new ApplicationData();

            var swineStarterGroup = new AnimalGroup()
            {
                Name = "SwineStarterGroup1",
                GroupType = AnimalType.SwineStarter,
                ManagementPeriods =
                {
                    new ManagementPeriod
                    {
                        DietAdditive = DietAdditiveType.FourPercentFat,
                        AnimalType = AnimalType.SwineStarter,
                        NumberOfAnimals = 100,
                        Name = "SwineStarterGroup1Period1",
                        Start = new DateTime(1996, 4, 25),
                        End = new DateTime(1996, 5, 25),
                                                                
                        FeedIntakeAmount = 1,

                        SelectedDiet = new Diet()
                        {
                            CrudeProtein = 1,
                            Forage = 1,
                            TotalDigestibleNutrient = 1,
                            Starch = 1,
                            NitrogenExcretionAdjustFactorForDiet = 1,
                            Fat = 1,
                            MetabolizableEnergy = 1,
                            Ndf = 1,
                            VolatileSolidsAdjustmentFactorForDiet = 1,
                            MethaneConversionFactor = 1,
                        },

                        HousingDetails = new HousingDetails()
                        {
                            HousingType = HousingType.ConfinedNoBarn,
                            BaselineMaintenanceCoefficient = 1,
                            ActivityCeofficientOfFeedingSituation = 1,
                        },

                        ManureDetails = new ManureDetails()
                        {
                            StateType = ManureStateType.CompostIntensive,
                            N2ODirectEmissionFactor = 1,
                            VolatilizationFraction = 0.2,
                            AshContentOfManure = 1,
                            MethaneConversionFactor = 1,
                            VolatileSolidExcretion = 1,
                            YearlyEntericMethaneRate = 1,
                            MethaneProducingCapacityOfManure = 1,
                        }
                    }
                },
            };

            var swineFinisherGroup = new AnimalGroup()
            {
                Name = "SwineStarterGroup1",
                GroupType = AnimalType.SwineFinisher,
                ManagementPeriods =
                {
                    new ManagementPeriod
                    {
                        DietAdditive = DietAdditiveType.FourPercentFat,
                        AnimalType = AnimalType.SwineFinisher,
                        NumberOfAnimals = 100,
                        Name = "SwineFinisherGroup1Period1",
                        Start = new DateTime(1996, 4, 25),
                        End = new DateTime(1996, 6, 25),
                                                                
                        FeedIntakeAmount = 1,

                        SelectedDiet = new Diet()
                        {
                            CrudeProtein = 1,
                            Forage = 1,
                            TotalDigestibleNutrient = 1,
                            Starch = 1,
                            VolatileSolidsAdjustmentFactorForDiet = 1,
                            NitrogenExcretionAdjustFactorForDiet = 1,
                            Fat = 1,
                            MetabolizableEnergy = 1,
                            Ndf = 1,
                            MethaneConversionFactor = 1,
                        },

                        HousingDetails = new HousingDetails()
                        {
                            HousingType = HousingType.ConfinedNoBarn,
                            BaselineMaintenanceCoefficient = 1,
                            ActivityCeofficientOfFeedingSituation = 1,
                        },

                        ManureDetails = new ManureDetails()
                        {
                            StateType = ManureStateType.CompostIntensive,
                            N2ODirectEmissionFactor = 1,
                            VolatilizationFraction = 0.2,
                            AshContentOfManure = 1,
                            MethaneConversionFactor = 1,
                            VolatileSolidExcretion = 1,
                            YearlyEntericMethaneRate = 1,
                            MethaneProducingCapacityOfManure = 1,
                        }
                    }
                }
            };

            var farm1 = new Farm()
            {
                Name = "Farm1",
                PolygonId = 100,
                SettingsFileName = "settingsfile1",
                Components = new ObservableCollection<ComponentBase>()
                {
                    new FarrowToWeanComponent()
                    {
                       Name = "Swine1",
                       Groups = {swineStarterGroup},
                       ComponentType = ComponentType.Swine,
                       ComponentCategory = ComponentCategory.Swine,
                       IsInitialized = true,
                    },

                    new SwineFinishersComponent()
                    {
                        Name = "Swine1",
                        Groups = {swineFinisherGroup},
                        ComponentType = ComponentType.Swine,
                        ComponentCategory = ComponentCategory.Swine,
                        IsInitialized = true,
                    },
                },
            };

            var farm2 = new Farm()
            {
                Name = "Farm2",
                SettingsFileName = "settingsfile1",
                Components = new ObservableCollection<ComponentBase>()
                {
                    new FarrowToWeanComponent()
                    {
                       Name = "Swine1",
                       Groups = {swineStarterGroup},
                       ComponentType = ComponentType.Swine,
                       ComponentCategory = ComponentCategory.Swine,
                       IsInitialized = true,
                    },

                    new SwineFinishersComponent()
                    {
                        Name = "Swine1",
                        Groups = {swineFinisherGroup},
                        ComponentType = ComponentType.Swine,
                        ComponentCategory = ComponentCategory.Swine,
                        IsInitialized = true,
                    },
                },
            };

            storage.ApplicationData.Farms.Add(farm1);
            storage.ApplicationData.Farms.Add(farm2);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            storage = new Storage();
        }

        [TestMethod]
        [Ignore]
        public void TestWriteEmissionsToFileForEstimatesOfProduction()
        {
           
            var results = new ComponentResultsProcessor(storage, new TimePeriodHelper());
            results.ProcessFarms(storage);

            var emissionsForAllFarms = results._animalEmissionResultsForAllFarms;

            var totals = new EstimatesOfProductionResults();
            var roundingDigits = 2;

            Directory.CreateDirectory("H.CLI.TestFiles");
            var pathToOutputResults = "H.CLI.TestFiles";

            var applicationData = new ApplicationData();
            applicationData.DisplayUnitStrings = new DisplayUnitStrings();
            applicationData.DisplayUnitStrings.SetStrings(MeasurementSystemType.Metric);
            results.WriteEmissionsToFiles(applicationData);

            //////////////Totals/////////////////////////

            /////////////Totals For All Farms ///////////////////////
            totals.CalculateTotalsForAllFarms(emissionsForAllFarms);
            Assert.AreEqual(Math.Round(totals.AllFarmsLandManure, roundingDigits), 833.28);
            Assert.AreEqual(Math.Round(totals.AllFarmsBeefProduced, roundingDigits), 0);
            Assert.AreEqual(Math.Round(totals.AllFarmsLambProduced, roundingDigits), 0);
            Assert.AreEqual(Math.Round(totals.AllFarmsMilkProduced, roundingDigits), 0);
            Assert.AreEqual(Math.Round(totals.AllFarmsFatAndProteinCorrectedMilkProduction, roundingDigits), 0);

            ///////////////Totals For Farm 1////////////////////////
            var _uncertaintyCalculator = new Table_68_69_Expression_Of_Uncertainty_Calculator();
            var allGroupedComponentsByFarm = results._animalEmissionResultsForAllFarms.GroupBy(x => x.Key).ToList();
            var groupedComponentsForAFarm = allGroupedComponentsByFarm[0];
            var filteredFarmComponents = groupedComponentsForAFarm.SelectMany(x => x.Value.Where(y => y.Component.ComponentType != ComponentType.Rams &&
                                                                                        y.Component.ComponentType != ComponentType.DairyBulls &&
                                                                                        y.Component.ComponentType != ComponentType.DairyDry &&
                                                                                        y.Component.ComponentType != ComponentType.DairyCalf &&
                                                                                        y.Component.ComponentType != ComponentType.DairyHeifer &&
                                                                                        y.Component.ComponentType != ComponentType.DairyBulls &&
                                                                                        y.Component.ComponentCategory != ComponentCategory.OtherLivestock));
            totals.CalculateTotalsForAFarm(filteredFarmComponents);
            //Assert.AreEqual(Math.Round(totals.FarmLandManure, roundingDigits), 833.28);
            //Assert.AreEqual(Math.Round(totals.FarmBeefProduced, roundingDigits), 0);
            //Assert.AreEqual(Math.Round(totals.FarmLambProduced, roundingDigits), 0);
            //Assert.AreEqual(Math.Round(totals.FarmMilkProduced, roundingDigits), 0);
            //Assert.AreEqual(Math.Round(totals.FarmFatAndProteinCorrectedMilkProduction, roundingDigits), 0);

            ////////////Farm 1 Swine Component Totals/////////////////////////////////////////////////////
            //var allGroupedComponents = groupedComponentsForAFarm.SelectMany(x => x.Value).GroupBy(y => y.Component.ComponentCategory);
            //var componentGroup = allGroupedComponents.First();

            //totals.CalculateTotalsForOneComponent(componentGroup);
            //Assert.AreEqual(Math.Round(totals.ComponentLandManure, roundingDigits), 833.28);
            //Assert.AreEqual(Math.Round(totals.ComponentBeefProduced, roundingDigits), 0);
            //Assert.AreEqual(Math.Round(totals.ComponentLambProduced, roundingDigits), 0);
            //Assert.AreEqual(Math.Round(totals.ComponentMilkProduced, roundingDigits), 0);
            //Assert.AreEqual(Math.Round(totals.ComponentFatAndProteinCorrectedMilkProduction, roundingDigits), 0);
   
            ////////////Farm 1 Swine Starter Sub-Group Totals//////////////////////////////////////////////////
            //var componentAnimalGroups = componentGroup.SelectMany(x => x.EmissionResultsForAllAnimalGroupsInComponent).GroupBy(y => y.AnimalGroup.GroupType).ToList();
            //var animalGroup = componentAnimalGroups[0];
            //totals.CalculateTotalsForOneAnimalGroup(animalGroup);
            //Assert.AreEqual(Math.Round(totals.AnimalGroupLandManure, roundingDigits), 277.76);
            //Assert.AreEqual(Math.Round(totals.AnimalGroupBeefProduced, roundingDigits), 0);
            //Assert.AreEqual(Math.Round(totals.AnimalGroupLambProduced, roundingDigits), 0);
            //Assert.AreEqual(Math.Round(totals.AnimalGroupMilkProduced, roundingDigits), 0);
            //Assert.AreEqual(Math.Round(totals.AnimalGroupFatAndProteinCorrectedMilkProduction, roundingDigits), 0);
        }

        [TestMethod]
        public void TestWriteEmissionsToFileFeedEstimates()
        {
            var cowCalfGroup = new AnimalGroup()
            {
                Name = "CowCalfGroup1",
                GroupType = AnimalType.BeefCowLactating,
                ManagementPeriods =
                {
                    new ManagementPeriod
                    {
                        DietAdditive = DietAdditiveType.FourPercentFat,
                        AnimalType = AnimalType.BeefCowLactating,
                        NumberOfAnimals = 100,
                        Name = "CowCalfGroup1Period1",
                        Start = new DateTime(1996, 4, 25),
                        End = new DateTime(1996, 5, 25),
                        FeedIntakeAmount = 1,
                        StartWeight = 20,
                        EndWeight = 30,
                        SelectedDiet = new Diet()
                        {
                            CrudeProtein = 1,
                            Forage = 1,
                            TotalDigestibleNutrient = 1,
                            Starch = 1,
                            Fat = 1,
                            MetabolizableEnergy = 1,
                            NitrogenExcretionAdjustFactorForDiet = 1,
                            Ndf = 1,
                            VolatileSolidsAdjustmentFactorForDiet = 1,
                            MethaneConversionFactor = 1,
                        },

                        HousingDetails = new HousingDetails()
                        {
                            HousingType = HousingType.ConfinedNoBarn,
                            BaselineMaintenanceCoefficient = 1,
                            ActivityCeofficientOfFeedingSituation = 1,
                        },

                        ManureDetails = new ManureDetails()
                        {
                            StateType = ManureStateType.CompostIntensive,
                            N2ODirectEmissionFactor = 1,
                            VolatilizationFraction = 0.2,
                            AshContentOfManure = 1,
                            MethaneConversionFactor = 1,
                            VolatileSolidExcretion = 1,
                            YearlyEntericMethaneRate = 1,
                            MethaneProducingCapacityOfManure = 1,
                        }
                    }
                },
            };

            var heiferGroup = new AnimalGroup()
            {
                Name = "BackgrounderHeiferGroup1",
                GroupType = AnimalType.BeefBackgrounderHeifer,
                ManagementPeriods =
                {
                    new ManagementPeriod
                    {
                        DietAdditive = DietAdditiveType.FourPercentFat,
                        AnimalType = AnimalType.BeefBackgrounderHeifer,
                        NumberOfAnimals = 100,
                        Name = "HeiferGroup1Period1",
                        Start = new DateTime(1996, 4, 25),
                        NumberOfDays = 30,                                                
                        FeedIntakeAmount = 1,
                        StartWeight = 20,
                        EndWeight = 30,

                        SelectedDiet = new Diet()
                        {
                            CrudeProtein = 1,
                            Forage = 1,
                            TotalDigestibleNutrient = 1,
                            Starch = 1,
                            Fat = 1,
                            VolatileSolidsAdjustmentFactorForDiet = 1,
                            NitrogenExcretionAdjustFactorForDiet = 1,
                            MetabolizableEnergy = 1,
                            Ndf = 1,
                            MethaneConversionFactor = 1,
                        },

                        HousingDetails = new HousingDetails()
                        {
                            HousingType = HousingType.ConfinedNoBarn,
                            BaselineMaintenanceCoefficient = 1,
                            ActivityCeofficientOfFeedingSituation = 1,
                        },

                        ManureDetails = new ManureDetails()
                        {
                            StateType = ManureStateType.CompostIntensive,
                            N2ODirectEmissionFactor = 1,
                            VolatilizationFraction = 0.2,
                            AshContentOfManure = 1,
                            MethaneConversionFactor = 1,
                            VolatileSolidExcretion = 1,
                            YearlyEntericMethaneRate = 1,
                            MethaneProducingCapacityOfManure = 1,
                        }
                    }
                }
            };

            var farm1 = new Farm()
            {
                Name = "Farm1",
                SettingsFileName = "settingsfile1",
                GeographicData = new GeographicData(),
                Components = new ObservableCollection<ComponentBase>()
                {
                    new CowCalfComponent()
                    {
                       Name = "Beef1",
                       Groups = {cowCalfGroup},
                       ComponentType = ComponentType.CowCalf,
                       ComponentCategory = ComponentCategory.BeefProduction,
                       IsInitialized = true,
                    },

                    new BackgroundingComponent()
                    {
                        Name = "Beef1",
                        Groups = {heiferGroup},
                        ComponentType = ComponentType.Backgrounding,
                        ComponentCategory = ComponentCategory.BeefProduction,
                        IsInitialized = true,
                    },
                },
            };

            var farm2 = new Farm()
            {
                Name = "Farm2",
                SettingsFileName = "settingsfile1",
                GeographicData = new GeographicData(),
                Components = new ObservableCollection<ComponentBase>()
                {
                    new CowCalfComponent()
                    {
                       Name = "Beef1",
                       Groups = {cowCalfGroup},
                       ComponentType = ComponentType.CowCalf,
                       ComponentCategory = ComponentCategory.BeefProduction,
                       IsInitialized = true,
                    },

                    new BackgroundingComponent()
                    {
                        Name = "Beef1",
                        Groups = {heiferGroup},
                        ComponentType = ComponentType.Backgrounding,
                        ComponentCategory = ComponentCategory.BeefProduction,
                        IsInitialized = true,
                    },
                },
            };

            var _storage = new Storage()
            {
                ApplicationData = new ApplicationData(),
            };

            var geographicData = new GeographicData();
            _storage.ApplicationData.Farms.Clear();
            _storage.ApplicationData.Farms.Add(farm1);
            _storage.ApplicationData.Farms.Add(farm2);
            var results = new ComponentResultsProcessor(_storage, new TimePeriodHelper());
            results.ProcessFarms(_storage);

            var emissionsForAllFarms = results._animalEmissionResultsForAllFarms;

            var totals = new FeedEstimateResults();

            var roundingDigits = 2;

            Directory.CreateDirectory("H.CLI.TestFiles");
            var pathToOutputResults = "H.CLI.TestFiles";
            results.WriteEmissionsToFiles(_storage.ApplicationData);

            //////////////Totals/////////////////////////

            /////////////Totals For All Farms ///////////////////////
            totals.CalculateTotalsForAllFarms(emissionsForAllFarms);
            //Assert.AreEqual(Math.Round(totals.AllFarmsDryMatterIntake, roundingDigits), 853.54);

            ///////////////Totals For Farm 1////////////////////////

            //var allGroupedComponentsByFarm = results._animalEmissionResultsForAllFarms.GroupBy(x => x.Key).ToList();
            //var groupedComponentsForAFarm = allGroupedComponentsByFarm[0];
            //var filteredFarmComponents = groupedComponentsForAFarm.SelectMany(x => x.Value.Where(y => y.Component.ComponentType != ComponentType.Rams &&
            //                                                                            y.Component.ComponentType != ComponentType.DairyBulls &&
            //                                                                            y.Component.ComponentType != ComponentType.DairyDry &&
            //                                                                            y.Component.ComponentType != ComponentType.DairyCalf &&
            //                                                                            y.Component.ComponentType != ComponentType.DairyHeifer &&
            //                                                                            y.Component.ComponentType != ComponentType.DairyBulls &&
            //                                                                            y.Component.ComponentCategory != ComponentCategory.OtherLivestock));
            //totals.CalculateTotalsForAFarm(filteredFarmComponents);
            //Assert.AreEqual(Math.Round(totals.FarmDryMatterIntake, roundingDigits), -9.72);


            ////////////Farm 1 Beef Cow Calf Component Totals/////////////////////////////////////////////////////
            //var allGroupedComponents = groupedComponentsForAFarm.SelectMany(x => x.Value).GroupBy(y => y.Component.ComponentCategory);
            //var componentGroup = allGroupedComponents.First();

            //totals.CalculateTotalsForOneComponent(componentGroup);
            //Assert.AreEqual(Math.Round(totals.ComponentDryMatterIntake, roundingDigits), -9.72);

            ////////////Farm 1 Cow Calf Sub-Group Totals//////////////////////////////////////////////////
            //var componentAnimalGroups = componentGroup.SelectMany(x => x.EmissionResultsForAllAnimalGroupsInComponent).GroupBy(y => y.AnimalGroup.GroupType).ToList();
            //var animalGroup = componentAnimalGroups[0];
            //totals.CalculateTotalsForOneAnimalGroup(animalGroup);
            //Assert.AreEqual(Math.Round(totals.AnimalGroupDryMatterIntake, roundingDigits), -9.72);
        }
          
    }

}

