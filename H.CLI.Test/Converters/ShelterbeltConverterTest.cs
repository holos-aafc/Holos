using H.Core.Calculators.Shelterbelt;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.Converters;
using H.CLI.FileAndDirectoryAccessors;
using H.CLI.Interfaces;
using H.CLI.TemporaryComponentStorage;
using H.CLI.UserInput;
using H.Core.Models;
using H.Core.Models.LandManagement.Shelterbelt;

namespace H.CLI.Test.Converters
{
    [TestClass]
    public class ShelterbeltConverterTest
    {

        [TestMethod]
        public void TestShelterbeltConverter()
        {
            CLILanguageConstants.culture = CultureInfo.CreateSpecificCulture("en-CA");
            CLILanguageConstants.Delimiter = ",";
            var shelterBeltConverter = new ShelterbeltConverter();
            var directoryHandler = new DirectoryHandler();
            var componentConverterHandler = new ComponentConverterHandler();
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\TestOutputs");
            var outputDirectory = @"H.CLI.TestFiles\TestOutputs";
            string farmDirectoryPath = @"C:\Users\HolosEN\Farms\Farm1";

            directoryHandler.InitializeDirectoriesAndFilesForComponents(outputDirectory);
            
            var allShelterbelts = new List<List<IComponentTemporaryInput>>();
            var tempShelterbeltRow1 = new ShelterBeltTemporaryInput()
            {
                GroupId = 1,
                HardinessZone = Core.Enumerations.HardinessZone.H3b,
                EcodistrictID = 754,
                YearOfObservation = 2019,
                Name = "My Caragana Shelterbelt",
              
                RowName = "Caragana/White Spruce",
                RowID = 1,
                RowLength = 100,
                PlantYear = 1996,
                CutYear = 2019,
                Species = Core.Enumerations.TreeSpecies.Caragana,
                PlantedTreeCount = 138,
                PlantedTreeSpacing = 0,
                LiveTreeCount = 113,
                AverageCircumference = 32.22222222
            };

            var tempShelterbeltRow2 = new ShelterBeltTemporaryInput()
            {
                GroupId = 2,
                HardinessZone = Core.Enumerations.HardinessZone.H3b,
                EcodistrictID = 754,
                YearOfObservation = 2019,
                Name = "My Caragana Shelterbelt",
               
                RowName = "Caragana/White Spruce",
                RowID = 1,
                RowLength = 100,
                PlantYear = 1996,
                CutYear = 2019,
                Species = Core.Enumerations.TreeSpecies.WhiteSpruce,
                PlantedTreeCount = 138,
                PlantedTreeSpacing = 0,
                LiveTreeCount = 113,
                AverageCircumference = 34.508448722251423
            };

            var tempShelterbeltRow3 = new ShelterBeltTemporaryInput()
            {
                GroupId = 1,
                HardinessZone = Core.Enumerations.HardinessZone.H3b,
                EcodistrictID = 754,
                YearOfObservation = 2019,
                Name = "My Caragana Shelterbelt",
         
                RowName = "Hybrid Poplar",
                RowID = 2,
                RowLength = 66,
                PlantYear = 1996,
                CutYear = 2019,
                Species = Core.Enumerations.TreeSpecies.HybridPoplar,
                PlantedTreeCount = 138,
                PlantedTreeSpacing = 0,
                LiveTreeCount = 113,
                AverageCircumference = 37.77777
            };



            var shelterBeltList = new List<IComponentTemporaryInput>()
            {
               tempShelterbeltRow1,
               tempShelterbeltRow2,
               tempShelterbeltRow3
            };

            allShelterbelts.Add(shelterBeltList);
            var allShelterbeltsWithGuidSet = componentConverterHandler.SetComponentListGuid(allShelterbelts);
            var convertedShelterbelts = componentConverterHandler.StartComponentConversion("Shelterbelts", new Farm(), allShelterbeltsWithGuidSet);
            var castedConvertedShelterbelts = convertedShelterbelts.Cast<ShelterbeltComponent>().ToList();

            //Shelterbelt
            Assert.AreEqual(castedConvertedShelterbelts[0].FakeName, "My Caragana Shelterbelt");
            Assert.AreEqual(castedConvertedShelterbelts[0].YearOfObservation, DateTime.Now.Year);
            Assert.AreEqual(castedConvertedShelterbelts[0].HardinessZone, HardinessZone.H3b);
            Assert.AreEqual(castedConvertedShelterbelts[0].EcoDistrictId, 754);

            //Row 1
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].YearOfObservation, 2019);
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].Name, "Caragana/White Spruce");
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].Length, 100);
            
            //Row 2
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[1].YearOfObservation, 2019);
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[1].Name, "Hybrid Poplar");
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[1].Length, 66);

            //Rows Have Same Parent Shelterbelt
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].ParentShelterbeltComponent, castedConvertedShelterbelts[0].RowData[1].ParentShelterbeltComponent);

            //Row 1 Tree Group Data, Tree Group 0
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].TreeGroupData.Count(), 2);
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].TreeGroupData[0].TreeSpecies, TreeSpecies.Caragana);
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].TreeGroupData[0].PlantYear, 1996);
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].TreeGroupData[0].CutYear, 2019);
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].TreeGroupData[0].PlantedTreeCount, 138);
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].TreeGroupData[0].PlantedTreeSpacing, 0);
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].TreeGroupData[0].CircumferenceData.UserCircumference, 32.22222222);
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].TreeGroupData[0].LiveTreeCount, 113);

            //Row 1 Tree Group Data, Tree Group 1
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].TreeGroupData[1].TreeSpecies, TreeSpecies.WhiteSpruce);
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].TreeGroupData[1].PlantYear, 1996);
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].TreeGroupData[1].CutYear, 2019);
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].TreeGroupData[1].PlantedTreeCount, 138);
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].TreeGroupData[1].PlantedTreeSpacing, 0);
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].TreeGroupData[1].CircumferenceData.UserCircumference, 34.508448722251423);
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].TreeGroupData[1].LiveTreeCount, 113);

            //Same Parent Shelterbelt and Parent Row
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].TreeGroupData[0].GrandParentShelterbeltComponent, castedConvertedShelterbelts[0].RowData[0].TreeGroupData[1].GrandParentShelterbeltComponent);
            Assert.AreEqual(castedConvertedShelterbelts[0].RowData[0].TreeGroupData[0].ParentRowData, castedConvertedShelterbelts[0].RowData[0].TreeGroupData[1].ParentRowData);




        }
    }
}


       

    

    

