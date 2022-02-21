using H.CLI.TemporaryComponentStorage;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.Converters;
using H.CLI.FileAndDirectoryAccessors;
using H.CLI.Interfaces;
using H.CLI.UserInput;
using H.Core.Models;
using H.Core.Models.Animals.Sheep;

namespace H.CLI.Test.Converters
{
       
    [TestClass]
    public class SheepConverterTest
    {
        private DirectoryHandler directoryHandler = new DirectoryHandler();
        private ComponentConverterHandler componentConverterHandler = new ComponentConverterHandler();

        [TestInitialize]
        public void Initialize()
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));
        }

        [TestMethod]
        public void SheepConverter_ExpectSheepComponentsInFarm()
        {
            var allSheepTemporaryInputs = new List<List<IComponentTemporaryInput>>();
            var tempSheepRow1 = new SheepTemporaryInput()
            {
                Name = "SheepComponent1",
                GroupName = "WLG1",
                GroupType = AnimalType.WeanedLamb,
                ManagementPeriodStartDate = Convert.ToDateTime("04/25/1996"),
                ManagementPeriodDays = 60,
                NumberOfAnimals = 60,
                NumberOfLambs = 60,
                InitialWeight = 20,
                FinalWeight = 30,
                ADG = 0.8,
                WoolProduction = 10,
                EnergyRequiredToProduceWool = 10,
                EnergyRequiredToProduceMilk = 10,
                
                DietAdditiveType = DietAdditiveType.FourPercentFat,
                MethaneConversionFactorOfDiet = 10,
                MethaneConversionFactorAdjusted = 10,
                FeedIntake = 10,
                CrudeProtein = 10,
                Forage = 10,
                TDN = 10,
                Starch = 10,
                Fat = 10,
                ME = 10,
                NDF = 10,
              
                HousingType = HousingType.ConfinedNoBarn,
                ActivityCoefficient = 10,   
                GainCoefficientA = 10,
                GainCoefficientB = 10,
                MaintenanceCoefficient = 10,
                N2ODirectEmissionFactor = 10,
                VolatilizationFraction = 10,
                MethaneConversionFactorOfManure = 10,
                MethaneProducingCapacityOfManure = 10,
                AshContent = 10,
            };

            var tempSheepRow2 = new SheepTemporaryInput()
            {
                Name = "SheepComponent1",
                GroupName = "WLG2",
                GroupType = AnimalType.WeanedLamb,

                ManagementPeriodStartDate = Convert.ToDateTime("04/25/1996"),
                ManagementPeriodDays = 60,
                NumberOfAnimals = 60,
                NumberOfLambs = 60,
                InitialWeight = 20,
                FinalWeight = 30,
                ADG = 0.8,
                WoolProduction = 5,
                EnergyRequiredToProduceWool = 5,
                EnergyRequiredToProduceMilk = 5,

                DietAdditiveType = DietAdditiveType.FourPercentFat,
                MethaneConversionFactorOfDiet = 10,
                MethaneConversionFactorAdjusted = 10,
                FeedIntake = 5,
                CrudeProtein = 10,
                Forage = 10,
                TDN = 10,
                Starch = 10,
                Fat = 10,
                ME = 10,
                NDF = 10,

                HousingType = HousingType.ConfinedNoBarn,
                ActivityCoefficient = 10,
                GainCoefficientA = 2,
                GainCoefficientB = 2,
                MaintenanceCoefficient = 2,
                N2ODirectEmissionFactor = 10,
                VolatilizationFraction = 10,
                MethaneConversionFactorOfManure = 2,
                MethaneProducingCapacityOfManure = 2,
                AshContent = 10,

            };

            var tempSheepRow3 = new SheepTemporaryInput()
            {
                Name = "SheepComponent1",
                GroupName = "WLG3",
                GroupType = AnimalType.WeanedLamb,

                ManagementPeriodStartDate = Convert.ToDateTime("04/25/1996"),
                ManagementPeriodDays = 60,
                NumberOfAnimals = 60,
                NumberOfLambs = 60,
                InitialWeight = 20,
                FinalWeight = 30,
                ADG = 0.8,
                WoolProduction = 10,
                EnergyRequiredToProduceWool = 10,
                EnergyRequiredToProduceMilk = 10,

                DietAdditiveType = DietAdditiveType.FourPercentFat,
                MethaneConversionFactorOfDiet = 10,
                MethaneConversionFactorAdjusted = 10,
                FeedIntake = 10,
                CrudeProtein = 10,
                Forage = 10,
                TDN = 10,
                Starch = 10,
                Fat = 10,
                ME = 10,
                NDF = 10,

                HousingType = HousingType.ConfinedNoBarn,
                ActivityCoefficient = 10,
                GainCoefficientA = 10,
                GainCoefficientB = 10,
                MaintenanceCoefficient = 10,
                N2ODirectEmissionFactor = 10,
                VolatilizationFraction = 10,
                MethaneConversionFactorOfManure = 10,
                MethaneProducingCapacityOfManure = 10,
                AshContent = 10,
            };

            var sheepList = new List<IComponentTemporaryInput>()
            {
                tempSheepRow1,
                tempSheepRow2,
                tempSheepRow3
            };

            allSheepTemporaryInputs.Add(sheepList);

            var farm = new Farm();
            var sheepListGuidSet = componentConverterHandler.SetComponentListGuid(allSheepTemporaryInputs);
            var sheepComponents = componentConverterHandler.StartComponentConversion("Sheep", farm, allSheepTemporaryInputs);

            var castedSheepComponents = sheepComponents.Cast<EwesAndLambsComponent>().ToList();

            //Sheep Component - Weaned Lamb
            Assert.AreEqual(castedSheepComponents[0].Name, "SheepComponent1");

            //Sheep Component Groups. There are 3 distinct groups
            Assert.AreEqual(castedSheepComponents[0].Groups.Count(), 3);

            //Group 1
            Assert.AreEqual(castedSheepComponents[0].Groups[0].Name, "WLG1");
            Assert.AreEqual(castedSheepComponents[0].Groups[0].GroupType, AnimalType.WeanedLamb);
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods.Count(), 1);
           
            //Group 1, Management Period 1
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].NumberOfAnimals, 60);            
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].Start, new DateTime(1996, 4, 25));
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].NumberOfDays, 60);
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].PeriodDailyGain, 0.8);
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].FeedIntakeAmount, 10);
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].WoolProduction, 10);
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].EnergyRequiredForMilk, 10);
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].EnergyRequiredForWool, 10);

            //Group 1, Management Period 1, Diet Details
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].SelectedDiet.CrudeProtein, 10);
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].SelectedDiet.Fat, 10);
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].SelectedDiet.Starch, 10);
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].SelectedDiet.TotalDigestibleNutrient, 10);
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].SelectedDiet.Ndf, 10);
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].SelectedDiet.Forage, 10);
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].SelectedDiet.MetabolizableEnergy, 10);

            //Group 1, Management Period 1, Housing Details
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].HousingDetails.HousingType, HousingType.ConfinedNoBarn);
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].HousingDetails.ActivityCeofficientOfFeedingSituation, 10);
          

            //Group 1, Management Period 1, Manure Details
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].GainCoefficientA, 10);
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].GainCoefficientB, 10);
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].ManureDetails.AshContentOfManure, 10);
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].ManureDetails.N2ODirectEmissionFactor, 10);
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].ManureDetails.MethaneConversionFactor, 10);
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].ManureDetails.N2ODirectEmissionFactor, 10);
            Assert.AreEqual(castedSheepComponents[0].Groups[0].ManagementPeriods[0].ManureDetails.VolatilizationFraction, 10);

            //Check animal group GUID to make sure they are not the same4
            Assert.AreNotEqual(castedSheepComponents[0].Groups[1].ManagementPeriods[0].AnimalGroupGuid, castedSheepComponents[0].Groups[0].ManagementPeriods[0].AnimalGroupGuid);
        }

    }
}
