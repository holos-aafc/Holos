using H.CLI.Converters;
using H.CLI.TemporaryComponentStorage;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.FileAndDirectoryAccessors;
using H.CLI.Interfaces;
using H.CLI.UserInput;
using H.Core.Models;
using H.Core.Models.Animals.Swine;
using H.Core.Models.LandManagement.Fields;

namespace H.CLI.Test.Converters
{
    [TestClass]
    public class SwineConverterTest
    {
        [TestMethod]
        public void TestSwineConverter()
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));
            var swineConverter = new SwineConverter();
            var directoryHandler = new DirectoryHandler();
            var componentConverterHandler = new ComponentConverterHandler();
 
            var allSwineStarters = new List<List<IComponentTemporaryInput>>();
            var tempSwineStarterRow1 = new SwineTemporaryInput()
            {
            
                Name = "SwineStarterName1",
                GroupName = "SwineStarter1",
                GroupType = AnimalType.SwineStarter,
              
                ManagementPeriodName = "Period1ForSwineStarter1",
                ManagementPeriodStartDate = Convert.ToDateTime("04/25/1996"),
                ManagementPeriodDays = 60,
                NumberOfAnimals = 60,
                FeedIntake = 5,
                CrudeProtein = 10,
                Forage = 10,
                TDN = 10,
                Starch = 10,
                Fat = 10,
                ME = 10,
                NDF = 10,
                VolatileSolidAdjusted = 10,
                NitrogenExcretionAdjusted = 10,
                HousingType = HousingType.ConfinedNoBarn,
                CA = 10,
                CFTemp = 10,
         
                MethaneConversionFactor = 10,
                N2ODirectEmissionFactor = 10,
                VolatilizationFraction = 10,
                AshContent = 10,
                VolatileSolidsExcretion = 10,
            };

            var tempSwineStarterRow2 = new SwineTemporaryInput()
            {
        
                Name = "SwineStarterName1",
                GroupName = "SwineStarter1",
                GroupType = AnimalType.SwineStarter,
         
                ManagementPeriodName = "Period2ForSwineStarter1",
                ManagementPeriodStartDate = Convert.ToDateTime("04/25/1996"),
                ManagementPeriodDays = 90,
                NumberOfAnimals = 60,
                FeedIntake = 5,
                CrudeProtein = 15,
                Forage = 15,
                TDN = 15,
                Starch = 15,
                Fat = 15,
                ME = 15,
                NDF = 15,
                VolatileSolidAdjusted = 15,
                NitrogenExcretionAdjusted = 15,
                HousingType = HousingType.Pasture,
                PastureLocation = "ExampleFieldName5",
                CA = 15,
                CFTemp = 15,
               
                MethaneConversionFactor = 15,
                N2ODirectEmissionFactor = 15,
                VolatilizationFraction = 15,
                AshContent = 15,
                VolatileSolidsExcretion = 15,
            };

            var tempSwineStarterRow3 = new SwineTemporaryInput()
            {
          
                Name = "SwineStarterName1",
                GroupName = "SwineStarter2",
                GroupType = AnimalType.SwineStarter,
         
                ManagementPeriodName = "Period1ForSwineStarter2",
                ManagementPeriodStartDate = Convert.ToDateTime("04/25/1996"),
                ManagementPeriodDays = 120,
                NumberOfAnimals = 100,
                FeedIntake = 5,
                CrudeProtein = 20,
                Forage = 20,
                TDN = 20,
                Starch = 20,                
                Fat = 20,
                ME = 20,
                NDF = 20,
                VolatileSolidAdjusted = 20,
                NitrogenExcretionAdjusted = 20,
                HousingType = HousingType.ConfinedNoBarn,
                CA = 20,
                CFTemp = 20,
              
                MethaneConversionFactor = 20,
                N2ODirectEmissionFactor = 20,
                VolatilizationFraction = 20,
                AshContent = 20,
                VolatileSolidsExcretion = 20,       
            };

            var swineStartersList = new List<IComponentTemporaryInput>()
            {
                tempSwineStarterRow1,
                tempSwineStarterRow2,
                tempSwineStarterRow3
            };

            allSwineStarters.Add(swineStartersList);

            var farm = new Farm();

            //If the housing type is on a pasture, set the field
            var fieldComponent = new FieldSystemComponent()
            {
                Name = "ExampleFieldName5",
                ComponentType = ComponentType.Field

            };
            farm.Components.Add(fieldComponent);

            var swineListGuidSet = componentConverterHandler.SetComponentListGuid(allSwineStarters);
            var swineComponents = componentConverterHandler.StartComponentConversion("Swine", farm, swineListGuidSet);
            var castedSwineList = swineComponents.Cast<FarrowToWeanComponent>().ToList();

            //SwineStarterComponent
            Assert.AreEqual(castedSwineList[0].Name, "SwineStarterName1");

            //Swine Starter Groups. There shuold be a group for SwineStarter1 and SwineStarter2
            Assert.AreEqual(castedSwineList[0].Groups.Count(), 2);
          
            //Swine Starter Group 1
            Assert.AreEqual(castedSwineList[0].Groups[0].Name, "SwineStarter1");
            Assert.AreEqual(castedSwineList[0].Groups[0].GroupType, AnimalType.SwineStarter);
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods.Count(), 2);
            //Group 1, Management Period 0
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].Name, "Period1ForSwineStarter1");
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].NumberOfAnimals, 60);
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].Start, new DateTime(1996, 4, 25));
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].NumberOfDays, 60);            
                         
            //Group 1, Management Period 0, Diet Details
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].SelectedDiet.CrudeProtein, 10);
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].SelectedDiet.NitrogenExcretionAdjustFactorForDiet, 10);
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].SelectedDiet.Fat, 10);
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].SelectedDiet.Starch, 10);
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].SelectedDiet.VolatileSolidsAdjustmentFactorForDiet, 10);
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].SelectedDiet.TotalDigestibleNutrient, 10);
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].SelectedDiet.Ndf, 10);
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].SelectedDiet.Forage, 10);
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].SelectedDiet.MetabolizableEnergy, 10);

            //Group 1, Management Period 0, Housing Details
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].HousingDetails.HousingType, HousingType.ConfinedNoBarn);
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].HousingDetails.ActivityCeofficientOfFeedingSituation, 10);            
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].HousingDetails.BaselineMaintenanceCoefficient, 10);

            //Group 1, Management Period 0, Manure Details
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].ManureDetails.AshContentOfManure, 10);
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].ManureDetails.N2ODirectEmissionFactor, 10);
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].ManureDetails.MethaneConversionFactor, 10);
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].ManureDetails.N2ODirectEmissionFactor, 10);
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].ManureDetails.VolatilizationFraction, 10);

            //Swine Group 1's Management Periods Refer to the Same Swine Group
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[0].AnimalGroupGuid,castedSwineList[0].Groups[0].ManagementPeriods[1].AnimalGroupGuid);

            //If Housing Type is Pasture, NameOfPastureLocation is set to a Field Name. PastureLocation refers to an actual Field Component
            //whereas the NameOfPastureLocation refers to the name of that component to be used during processing
            Assert.AreEqual(castedSwineList[0].Groups[0].ManagementPeriods[1].HousingDetails.HousingType, HousingType.Pasture);            

            //Swine Starter Group 2
            Assert.AreEqual(castedSwineList[0].Groups[1].Name, "SwineStarter2");
            Assert.AreEqual(castedSwineList[0].Groups[1].GroupType, AnimalType.SwineStarter);
            Assert.AreEqual(castedSwineList[0].Groups[1].ManagementPeriods.Count(), 1);

            //Assert that the different swine starter group's management periods do not point to the same swine starter group
            Assert.AreNotEqual(castedSwineList[0].Groups[1].ManagementPeriods[0].AnimalGroupGuid, castedSwineList[0].Groups[0].ManagementPeriods[0].AnimalGroupGuid);         
        }
    }
}
