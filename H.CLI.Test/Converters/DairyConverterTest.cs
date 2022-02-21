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
using H.Core.Models.Animals.Dairy;
using H.Core.Models.LandManagement.Fields;

namespace H.CLI.Test.Converters
{
    [TestClass]
    public class DairyConverterTest
    {
        [TestMethod]
        public void TestDairyConverter()
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));
            var dairyConverter = new DairyConverter();
            var directoryHandler = new DirectoryHandler();
            var componentConverterHandler = new ComponentConverterHandler();
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\TestOutputs");
            var outputDirectory = @"H.CLI.TestFiles\TestOutputs";
            string farmDirectoryPath = @"C:\Users\HolosEN\Farms\Farm1";

            directoryHandler.InitializeDirectoriesAndFilesForComponents(outputDirectory);

            var allDairys = new List<List<IComponentTemporaryInput>>();
            var tempDairyRow1 = new DairyTemporaryInput()
            {
                Name = "ExampleDairyComponentName",
                GroupName = "DairyHeifers1",
                GroupType = AnimalType.DairyHeifers,

                ManagementPeriodName = "ExampleManagementPeriodName",
                ManagementPeriodStartDate = Convert.ToDateTime("04/25/1996"),
                ManagementPeriodDays = 120,
                NumberOfAnimals = 100,
                StartWeight = 20,
                EndWeight = 30,
                MilkFatContent = 2,
                MilkProduction = 2,
                MilkProtein = 2,
                DietAdditiveType = DietAdditiveType.FourPercentFat,
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
                ActivityCoefficient = 20,
                MaintenanceCoefficient = 20,

                MethaneConversionFactorOfManure = 20,
                MethaneConversionFactorAdjusted = 0.4,
                MethaneProducingCapacityOfManure = 0.7,
                N2ODirectEmissionFactor = 20,
                VolatilizationFraction = 20,
                AshContent = 20,    
            };


            var tempDairyRow3 = new DairyTemporaryInput()
            {
      
                Name = "ExampleDairyComponentName",
                GroupName = "DairyHeifers1",
                GroupType = AnimalType.DairyHeifers,
            
                ManagementPeriodName = "ExampleManagementPeriodName",
                ManagementPeriodStartDate = Convert.ToDateTime("04/25/1996"),
                ManagementPeriodDays = 120,
                NumberOfAnimals = 100,
                StartWeight = 20,
                EndWeight = 30,
                MilkFatContent = 2,
                MilkProduction = 2,
                MilkProtein = 2,
                DietAdditiveType = DietAdditiveType.FourPercentFat,
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
                HousingType = HousingType.EnclosedPasture,
                ActivityCoefficient = 20,
                MaintenanceCoefficient = 20,

                MethaneConversionFactorOfManure = 20,
                MethaneConversionFactorAdjusted = 0.4,
                MethaneProducingCapacityOfManure = 0.7,
                N2ODirectEmissionFactor = 20,
                VolatilizationFraction = 20,
                AshContent = 20,
            };
 
            var dairyComponent = new List<IComponentTemporaryInput>
            {
               tempDairyRow1,
             
               tempDairyRow3,
               
            };

            allDairys.Add(dairyComponent);

            var farm = new Farm();
            var fieldComponent = new FieldSystemComponent()
            {
                Name = "ExampleFieldName5",
                ComponentType = ComponentType.Field
                
            };
            farm.Components.Add(fieldComponent);

            var dairyListGuidSet = componentConverterHandler.SetComponentListGuid(allDairys);
            var dairyComponents = componentConverterHandler.StartComponentConversion("Dairy", farm, dairyListGuidSet);
            var castedDairyList = dairyComponents.Cast<DairyComponent>().ToList();

            //DairyStarterComponent
            Assert.AreEqual(castedDairyList[0].Name, "ExampleDairyComponentName");

            //Dairy Groups. 
            //But for every different group, we will create a new Dairy Component corresponding to that group (i.e DairyHeifersComponent)
            Assert.AreEqual(castedDairyList[0].Groups.Count(), 1);
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods.Count(), 2);
            Assert.AreEqual(castedDairyList.Count(), 1);

            //Dairy Starter Group 1
            Assert.AreEqual(castedDairyList[0].Groups[0].Name, "DairyHeifers1");
            Assert.AreEqual(castedDairyList[0].Groups[0].GroupType, AnimalType.DairyHeifers);
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods.Count(), 2);

            //Group 1, Management Period 1
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].Name, "ExampleManagementPeriodName");
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].NumberOfAnimals, 100);
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].Start, new DateTime(1996, 4, 25));
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].NumberOfDays, 120);                        
           
            //Group 1, Management Period 1, Diet Details
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].SelectedDiet.CrudeProtein, 20);
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].SelectedDiet.Fat, 20);
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].SelectedDiet.Starch, 20);
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].SelectedDiet.TotalDigestibleNutrient, 20);
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].SelectedDiet.NitrogenExcretionAdjustFactorForDiet, 20);
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].SelectedDiet.Ndf, 20);
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].SelectedDiet.VolatileSolidsAdjustmentFactorForDiet, 20);
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].SelectedDiet.Forage, 20);
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].SelectedDiet.MetabolizableEnergy, 20);

            //Group 1, Management Period 1, Housing Details
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].HousingDetails.HousingType, HousingType.ConfinedNoBarn);
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].HousingDetails.ActivityCeofficientOfFeedingSituation, 20);
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].HousingDetails.BaselineMaintenanceCoefficient, 20);

            //Group 1, Management Period 1, Manure Details
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].ManureDetails.AshContentOfManure, 20);
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].ManureDetails.N2ODirectEmissionFactor, 20);
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].ManureDetails.MethaneConversionFactor, 20);
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].ManureDetails.N2ODirectEmissionFactor, 20);
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].ManureDetails.VolatilizationFraction, 20);

            //Dairy Group 1's Management Periods Refer to 
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[0].AnimalGroupGuid, castedDairyList[0].Groups[0].ManagementPeriods[1].AnimalGroupGuid);

            //If Housing Type is Pasture, NameOfPastureLocation is set to a Field Name. PastureLocation refers to an actual Field Component
            //whereas the NameOfPastureLocation refers to the name of that component to be used during processing
            Assert.AreEqual(castedDairyList[0].Groups[0].ManagementPeriods[1].HousingDetails.HousingType, HousingType.EnclosedPasture);
          
        }

    }
}
