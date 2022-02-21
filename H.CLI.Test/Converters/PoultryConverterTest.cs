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
using H.Core.Models.Animals.Poultry;

namespace H.CLI.Test.Converters
{
    [TestClass]
    public class PoultryConverterTest
    {
        private DirectoryHandler directoryHandler = new DirectoryHandler();
        private ComponentConverterHandler componentConverterHandler = new ComponentConverterHandler();

        [TestInitialize]
        public void Initialize()
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));
        }

        [TestMethod]
        public void PoultryConverter_ExpectPoultryComponentsInFarm()
        {
            var allPoultryTemporaryInputs = new List<List<IComponentTemporaryInput>>();
            var tempPoultryRow1 = new PoultryTemporaryInput()
            {
                Name = "Poultry1",
                GroupName = "LDG1",
                GroupType = AnimalType.LayersDryPoultry,
                NumberOfAnimals = 60,
                ManagementPeriodStartDate = Convert.ToDateTime("04/25/1996"),
                ManagementPeriodDays = 60,
                N2ODirectEmissionFactor = 10,
                VolatilizationFraction = 10,
                YearlyManureMethaneRate = 10,
                YearlyEntericMethaneRate = 10,
                YearlyNitrogenExcretionRate = 10,
            };

            var tempPoultryRow2 = new PoultryTemporaryInput()
            {
                Name = "Poultry1",
                GroupName = "LDG1",
                GroupType = AnimalType.LayersDryPoultry,
                NumberOfAnimals = 60,
                ManagementPeriodStartDate = Convert.ToDateTime("04/25/1996"),
                ManagementPeriodDays = 60,
                N2ODirectEmissionFactor = 10,
                VolatilizationFraction = 10,
                YearlyManureMethaneRate = 10,
                YearlyEntericMethaneRate = 10,
                YearlyNitrogenExcretionRate = 10,
            };

            var tempPoultryRow3 = new PoultryTemporaryInput()
            {
                Name = "Poultry1",
                GroupName = "LDG2",
                GroupType = AnimalType.LayersDryPoultry,
                NumberOfAnimals = 60,
                ManagementPeriodStartDate = Convert.ToDateTime("04/25/1996"),
                ManagementPeriodDays = 60,
                N2ODirectEmissionFactor = 10,
                VolatilizationFraction = 10,
                YearlyManureMethaneRate = 10,
                YearlyEntericMethaneRate = 10,
                YearlyNitrogenExcretionRate = 10,
            };

            var poultryList = new List<IComponentTemporaryInput>()
            {
                tempPoultryRow1,
                tempPoultryRow2,
                tempPoultryRow3
            };

            allPoultryTemporaryInputs.Add(poultryList);
            var farm = new Farm();
            var poultryListGuidSet = componentConverterHandler.SetComponentListGuid(allPoultryTemporaryInputs);
            var poultryComponents = componentConverterHandler.StartComponentConversion("Poultry", farm, allPoultryTemporaryInputs);
            var castedPoultryComponents = poultryComponents.Cast<PoultryLayersDryComponent>().ToList();

            //Poultry
            Assert.AreEqual(castedPoultryComponents[0].Name, "Poultry1");

            //Poultry Groups - There are only two distinct groups, group 1 should have 2 management periods
            Assert.AreEqual(castedPoultryComponents[0].Groups.Count(), 2);

           //Group 1
            Assert.AreEqual(castedPoultryComponents[0].Groups[0].Name, "LDG1");
            Assert.AreEqual(castedPoultryComponents[0].Groups[0].GroupType, AnimalType.LayersDryPoultry);
            Assert.AreEqual(castedPoultryComponents[0].Groups[0].ManagementPeriods.Count(), 2);

            //Group 1, Management Period 1
            Assert.AreEqual(castedPoultryComponents[0].Groups[0].ManagementPeriods[0].NumberOfAnimals, 60);
            Assert.AreEqual(castedPoultryComponents[0].Groups[0].ManagementPeriods[0].Start, new DateTime(1996, 4, 25));
            Assert.AreEqual(castedPoultryComponents[0].Groups[0].ManagementPeriods[0].NumberOfDays, 60);

            Assert.AreEqual(castedPoultryComponents[0].Groups[0].ManagementPeriods[0].ManureDetails.YearlyEntericMethaneRate, 10);
            Assert.AreEqual(castedPoultryComponents[0].Groups[0].ManagementPeriods[0].ManureDetails.YearlyManureMethaneRate, 10);
            Assert.AreEqual(castedPoultryComponents[0].Groups[0].ManagementPeriods[0].ManureDetails.NitrogenExretionRate, 10);


        }
    }
}
