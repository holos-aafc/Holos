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
using H.Core.Models.Animals.Beef;

namespace H.CLI.Test.Converters
{
    [TestClass]
    public class BeefConverterTest
    {
        private DirectoryHandler directoryHandler = new DirectoryHandler();
        private ComponentConverterHandler componentConverterHandler = new ComponentConverterHandler();

        [TestInitialize]
        public void Initialize()
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));
        }

        [TestMethod]
        public void BeefConverter_ExpectBeefComponentsInFarm()
        {
            var allBeefTemporaryInputs = new List<List<IComponentTemporaryInput>>();
            var tempBeefRow1 = new BeefCattleTemporaryInput()
            {
                Name = "Beef1",
                GroupName = "BCG1",
                GroupType = AnimalType.BeefCowLactating,

                ManagementPeriodStartDate = Convert.ToDateTime("04/25/1996"),
                ManagementPeriodDays = 60,
                NumberOfAnimals = 60,
          
                InitialWeight = 20,
                FinalWeight = 30,
                ADG = 0.8,
        
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
               
                MaintenanceCoefficient = 2,
                N2ODirectEmissionFactor = 10,
                VolatilizationFraction = 10,
                MethaneConversionFactorOfManure = 10,
                MethaneProducingCapacityOfManure = 10,
                AshContent = 10,
            };

            var tempBeefRow2 = new BeefCattleTemporaryInput()
            {
                Name = "Beef1",
                GroupName = "BCG2",
                GroupType = AnimalType.BeefCowLactating,

                ManagementPeriodStartDate = Convert.ToDateTime("04/25/1996"),
                ManagementPeriodDays = 60,
                NumberOfAnimals = 60,

                InitialWeight = 20,
                FinalWeight = 30,
                ADG = 0.8,


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

                MaintenanceCoefficient = 2,
                N2ODirectEmissionFactor = 10,
                VolatilizationFraction = 10,
                MethaneConversionFactorOfManure = 2,
                MethaneProducingCapacityOfManure = 2,
                AshContent = 10,
            };

            var tempBeefRow3 = new BeefCattleTemporaryInput()
            {
                Name = "Beef1",
                GroupName = "BCG3",
                GroupType = AnimalType.BeefCowLactating,

                ManagementPeriodStartDate = Convert.ToDateTime("04/25/1996"),
                ManagementPeriodDays = 60,
                NumberOfAnimals = 60,

                InitialWeight = 20,
                FinalWeight = 30,
                ADG = 0.8,


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

                MaintenanceCoefficient = 2,
                N2ODirectEmissionFactor = 10,
                VolatilizationFraction = 10,
                MethaneConversionFactorOfManure = 2,
                MethaneProducingCapacityOfManure = 2,
                AshContent = 10,
            };

            var beefList = new List<IComponentTemporaryInput>()
            {
                tempBeefRow1,
                tempBeefRow2,
                tempBeefRow3
            };

            allBeefTemporaryInputs.Add(beefList);

            var farm = new Farm();
            var beefListGuidSet = componentConverterHandler.SetComponentListGuid(allBeefTemporaryInputs);
            var beefComponents = componentConverterHandler.StartComponentConversion("Beef", farm, allBeefTemporaryInputs);

            var castedBeefComponents = beefComponents.Cast<CowCalfComponent>().ToList();

            //Beef Component - Cow Calf
            Assert.AreEqual(castedBeefComponents[0].Name, "Beef1");

            //Beef Groups. There are 3 distinct groups
            Assert.AreEqual(castedBeefComponents[0].Groups.Count(), 3);

            //Group 1
            Assert.AreEqual(castedBeefComponents[0].Groups[0].Name, "BCG1");
            Assert.AreEqual(castedBeefComponents[0].Groups[0].GroupType, AnimalType.BeefCowLactating);
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods.Count(), 1);

            //Group 1, Management Period 1
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods[0].NumberOfAnimals, 60);
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods[0].Start, new DateTime(1996, 4, 25));
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods[0].NumberOfDays, 60);
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods[0].PeriodDailyGain, 0.8);
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods[0].FeedIntakeAmount, 10);


            //Group 1, Management Period 1, Diet Details
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods[0].SelectedDiet.CrudeProtein, 10);
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods[0].SelectedDiet.Fat, 10);
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods[0].SelectedDiet.Starch, 10);
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods[0].SelectedDiet.TotalDigestibleNutrient, 10);
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods[0].SelectedDiet.Ndf, 10);
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods[0].SelectedDiet.Forage, 10);
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods[0].SelectedDiet.MetabolizableEnergy, 10);

            //Group 1, Management Period 1, Housing Details
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods[0].HousingDetails.HousingType, HousingType.ConfinedNoBarn);
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods[0].HousingDetails.ActivityCeofficientOfFeedingSituation, 10);


            //Group 1, Management Period 1, Manure Details
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods[0].ManureDetails.AshContentOfManure, 10);
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods[0].ManureDetails.N2ODirectEmissionFactor, 10);
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods[0].ManureDetails.MethaneConversionFactor, 10);
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods[0].ManureDetails.N2ODirectEmissionFactor, 10);
            Assert.AreEqual(castedBeefComponents[0].Groups[0].ManagementPeriods[0].ManureDetails.VolatilizationFraction, 10);

            //Assert that the different swine starter group's management periods do not point to the same swine starter group
            Assert.AreNotEqual(castedBeefComponents[0].Groups[1].ManagementPeriods[0].AnimalGroupGuid, castedBeefComponents[0].Groups[0].ManagementPeriods[0].AnimalGroupGuid);
        }
    }
}
