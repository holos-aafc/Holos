using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using H.CLI.Converters;
using H.CLI.FileAndDirectoryAccessors;
using H.CLI.Interfaces;
using H.CLI.TemporaryComponentStorage;
using H.CLI.UserInput;

namespace H.CLI.Test.Converters
{
    [TestClass]
    public class ComponentConverterHandlerTest
    {
        DirectoryHandler directoryHandler = new DirectoryHandler();

        [TestMethod]
        public void TestStartComponentConversion_ExpectConverterStratToBeSetToShelterbelt()
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));

            string outputDirectory = "TestOutputs";
            string farmName = "FarmName1";
            //directoryHandler.ValidateUserInputedOutputDirectory(outputDirectory, farmName);
            
            var tempShelterBeltInput = new ShelterBeltTemporaryInput()
            {
                HardinessZone = Core.Enumerations.HardinessZone.H3b,
                EcodistrictID = 754,
                YearOfObservation = 2019,
                Name = "My Caragana Shelterbelt",
                RowName = "Caragana",
                RowID = 1,
                RowLength = 100,
                PlantYear = 1996,
                CutYear = 2019,
                Species = Core.Enumerations.TreeSpecies.Caragana,
                PlantedTreeCount = 138,
                PlantedTreeSpacing = 0,
                LiveTreeCount = 113,
                AverageCircumference = 34.508448722251423

            };
            var listOfTempShelterBeltInputs = new List<ShelterBeltTemporaryInput>()
            {
                tempShelterBeltInput
            };

         var allShelterbelts = new List<List<IComponentTemporaryInput>>();
         allShelterbelts.Add(listOfTempShelterBeltInputs.Cast<IComponentTemporaryInput>().ToList());
         var converterHandler = new ComponentConverterHandler();
         var allShelterbeltsWithGuidSet = converterHandler.SetComponentListGuid(allShelterbelts);
        // converterHandler.StartComponentConversion("Shelterbelts", allShelterbeltsWithGuidSet, outputDirectory, farmDirectoryPath, new Models.Defaults());
        // Assert.IsInstanceOfType(converterHandler._converterStrat._converter, typeof(ShelterbeltConverter));
        }

        [TestMethod]
        public void TestSetComponentListGuid_ExpectShelterbelt1andShelterbelt2ToHaveSameRowGuid()
        {
            string outputDirectory = "TestOutputs";
            string farmName =  "FarmName1";
            //directoryHandler.ValidateUserInputedOutputDirectory(outputDirectory, farmName);

            var tempShelterBeltRow1 = new ShelterBeltTemporaryInput()
            {
                HardinessZone = Core.Enumerations.HardinessZone.H3b,
                EcodistrictID = 754,
                YearOfObservation = 2019,
                Name = "My Caragana Shelterbelt",
                RowName = "Caragana",
                RowID = 1,
                RowLength = 100,
                PlantYear = 1996,
                CutYear = 2019,
                Species = Core.Enumerations.TreeSpecies.Caragana,
                PlantedTreeCount = 138,
                PlantedTreeSpacing = 0,
                LiveTreeCount = 113,
                AverageCircumference = 34.508448722251423,
                GroupId = 1
        };

            var tempShelterBeltRow2 = new ShelterBeltTemporaryInput()
            {
                HardinessZone = Core.Enumerations.HardinessZone.H3b,
                EcodistrictID = 754,
                YearOfObservation = 2019,
                Name = "My Caragana Shelterbelt",
                RowName = "Caragana",
                RowID = 1,
                RowLength = 100,
                PlantYear = 1996,
                CutYear = 2019,
                Species = Core.Enumerations.TreeSpecies.Caragana,
                PlantedTreeCount = 138,
                PlantedTreeSpacing = 0,
                LiveTreeCount = 113,
                AverageCircumference = 34.508448722251423,
                GroupId = 1
        };

            var tempShelterBeltRow3 = new ShelterBeltTemporaryInput()
            {
                HardinessZone = Core.Enumerations.HardinessZone.H3b,
                EcodistrictID = 754,
                YearOfObservation = 2019,
                Name = "My Caragana Shelterbelt",
                RowName = "Caragana",
                RowID = 2,
                RowLength = 100,
                PlantYear = 1996,
                CutYear = 2019,
                Species = Core.Enumerations.TreeSpecies.Caragana,
                PlantedTreeCount = 138,
                PlantedTreeSpacing = 0,
                LiveTreeCount = 113,
                AverageCircumference = 34.508448722251423,
                GroupId = 2
            };


            var listOfTempShelterBeltInputs = new List<ShelterBeltTemporaryInput>()
            {
                tempShelterBeltRow1,
                tempShelterBeltRow2,
                tempShelterBeltRow3
            };

            var allShelterbelts = new List<List<IComponentTemporaryInput>>();
            allShelterbelts.Add(listOfTempShelterBeltInputs.Cast<IComponentTemporaryInput>().ToList());
            var converterHandler = new ComponentConverterHandler();
            var allShelterbeltsWithGuidSet = converterHandler.SetComponentListGuid(allShelterbelts);

            Assert.AreEqual(allShelterbeltsWithGuidSet[0][0].Guid, allShelterbeltsWithGuidSet[0][1].Guid);
            Assert.AreNotEqual(allShelterbeltsWithGuidSet[0][0].Guid, allShelterbeltsWithGuidSet[0][2].Guid);

        }
    }

}

