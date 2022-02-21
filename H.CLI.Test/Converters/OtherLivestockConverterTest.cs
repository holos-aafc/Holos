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
using H.Core.Models.Animals.OtherAnimals;

namespace H.CLI.Test.Converters
{
    [TestClass]
    public class OtherLivestockConverterTest
    {
        private DirectoryHandler directoryHandler = new DirectoryHandler();
        private ComponentConverterHandler componentConverterHandler = new ComponentConverterHandler();

        [TestInitialize]
        public void Initialize()
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));
        }

        [TestMethod]
        public void OtherLivestockConverter_ExpectOtherLivestockComponentsInFarm()
        {
            var allOtherLivestockTemporaryInputs = new List<List<IComponentTemporaryInput>>();
            var tempOtherLivestockRow1 = new OtherLiveStockTemporaryInput()
            {
                Name = "OtherLivestock1",
                GroupName = "MulesG1",
                GroupType = AnimalType.Mules,
                NumberOfAnimals = 60,
                ManagementPeriodStartDate = Convert.ToDateTime("04/25/1996"),
                ManagementPeriodDays = 60,
                N2ODirectEmissionFactor = 10,
                VolatilizationFraction = 10,
                YearlyManureMethaneRate = 10,
                YearlyEntericMethaneRate = 10,
                YearlyNitrogenExcretionRate = 10,
            };

            var tempOtherLivestockRow2 = new OtherLiveStockTemporaryInput()
            {
                Name = "OtherLivestock1",
                GroupName = "MulesG2",
                GroupType = AnimalType.Mules,
                NumberOfAnimals = 60,
                ManagementPeriodStartDate = Convert.ToDateTime("04/25/1996"),
                ManagementPeriodDays = 60,
                N2ODirectEmissionFactor = 10,
                VolatilizationFraction = 10,
                YearlyManureMethaneRate = 10,
                YearlyEntericMethaneRate = 10,
                YearlyNitrogenExcretionRate = 10,
            };

            var tempOtherLivestockRow3 = new OtherLiveStockTemporaryInput()
            {
                Name = "OtherLivestock1",
                GroupName = "MulesG3",
                GroupType = AnimalType.Mules,
                NumberOfAnimals = 60,
                ManagementPeriodStartDate = Convert.ToDateTime("04/25/1996"),
                ManagementPeriodDays = 60,
                N2ODirectEmissionFactor = 10,
                VolatilizationFraction = 10,
                YearlyManureMethaneRate = 10,
                YearlyEntericMethaneRate = 10,
                YearlyNitrogenExcretionRate = 10,
            };

            var otherLivestockList = new List<IComponentTemporaryInput>()
            {
                tempOtherLivestockRow1,
                tempOtherLivestockRow2,
                tempOtherLivestockRow3
            };

            allOtherLivestockTemporaryInputs.Add(otherLivestockList);
            var farm = new Farm();
            var otherLivestockListGuidSet = componentConverterHandler.SetComponentListGuid(allOtherLivestockTemporaryInputs);
            var otherLivestockComponents = componentConverterHandler.StartComponentConversion("Other Livestock", farm, allOtherLivestockTemporaryInputs);
            var castedOtherLivestockComponents = otherLivestockComponents.Cast<MulesComponent>().ToList();

            //Other Livestock Components - Mules
            Assert.AreEqual(castedOtherLivestockComponents[0].Name, "OtherLivestock1");

            //Other Livestock Groups - There are 3 different groups
            Assert.AreEqual(castedOtherLivestockComponents[0].Groups.Count(), 3);

            //Group 1
            Assert.AreEqual(castedOtherLivestockComponents[0].Groups[0].Name, "MulesG1");
            Assert.AreEqual(castedOtherLivestockComponents[0].Groups[0].GroupType, AnimalType.Mules);
            Assert.AreEqual(castedOtherLivestockComponents[0].Groups[0].ManagementPeriods.Count(), 1);

            //Group 1, Management Period 1

            Assert.AreEqual(castedOtherLivestockComponents[0].Groups[0].ManagementPeriods[0].NumberOfAnimals, 60);
            Assert.AreEqual(castedOtherLivestockComponents[0].Groups[0].ManagementPeriods[0].Start, new DateTime(1996, 4, 25));
            Assert.AreEqual(castedOtherLivestockComponents[0].Groups[0].ManagementPeriods[0].NumberOfDays, 60);

            Assert.AreEqual(castedOtherLivestockComponents[0].Groups[0].ManagementPeriods[0].ManureDetails.YearlyEntericMethaneRate, 10);
            Assert.AreEqual(castedOtherLivestockComponents[0].Groups[0].ManagementPeriods[0].ManureDetails.YearlyManureMethaneRate, 10);
            Assert.AreEqual(castedOtherLivestockComponents[0].Groups[0].ManagementPeriods[0].ManureDetails.NitrogenExretionRate, 10);


        }
    }
}
