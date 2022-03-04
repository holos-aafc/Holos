using H.CLI.Converters;
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
using H.CLI.TemporaryComponentStorage;
using H.CLI.UserInput;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.CLI.Test.Converters
{
    [TestClass]
    [Ignore]
    public class FieldConverterTest
    {
        [TestMethod]
        public void TestFieldConverter()
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));
            var fieldConverter = new FieldSystemInputConverter();
            var componentConverterHandler = new ComponentConverterHandler();
            var directoryHandler = new DirectoryHandler();
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\TestOutputs");
            var outputDirectory = @"H.CLI.TestFiles\TestOutputs";
            string farmDirectoryPath = @"C:\Users\HolosEN\Farms\Farm1";
            directoryHandler.InitializeDirectoriesAndFilesForComponents(outputDirectory);
            var allFields = new List<List<IComponentTemporaryInput>>();
            var pernnialGuid = Guid.NewGuid();

            var tempFieldRow1 = new FieldTemporaryInput()
            {
                PhaseNumber = 1,
                GroupId = 1,
                Name = "Example Field Name",
                Area = 100,
                CurrentYear = 2019,
                CropYear = 2018,
                CropType = CropType.TameGrass,
                TillageType = TillageType.Reduced,
                YearInPerennialStand = 1,
                PerennialStandID = pernnialGuid,
                PerennialStandLength = 2,
                BiomassCoefficientProduct = 10,
                BiomassCoefficientStraw = 10,
                BiomassCoefficientRoots = 10,
                BiomassCoefficientExtraroot = 10,
                NitrogenContentInProduct = 10,
                NitrogenContentInStraw = 10,
                NitrogenContentInRoots = 10,
                NitrogenContentInExtraroot = 10,
                CarbonConcentration = 5,
                Yield = 100,
                HarvestMethod = HarvestMethods.Silage,
                NitrogenFertilizerRate = 15,
                PhosphorousFertilizerRate = 15,
                IsIrrigated = Response.Yes,
                IrrigationType = IrrigationType.Irrigated,
                AmountOfIrrigation = 30,
                MoistureContentOfCrop = 20,
                PercentageOfStrawReturnedToSoil = 15,
                PercentageOfRootsReturnedToSoil = 15,
                PercentageOfProductYieldReturnedToSoil = 15,
                IsPesticideUsed = Response.Yes,
                NumberOfPesticidePasses = 2,
                ManureApplied = true,
                AmountOfManureApplied = 20,
                ManureApplicationType = ManureApplicationTypes.OptionA,
                ManureAnimalSourceType = ManureAnimalSourceTypes.BeefManure,
                ManureStateType = ManureStateType.Composted,
                ManureLocationSourceType = ManureLocationSourceType.Imported,
                CoverCropsUsed = true,
                CoverCropType = CoverCropTypes.OptionA,
                CoverCropTerminationType = CoverCropTerminationType.OptionA,
                UnderSownCropsUsed = true,
                CropIsGrazed = true
            };

            var tempFieldRow2 = new FieldTemporaryInput()
            {
                PhaseNumber = 1,
                GroupId = 1,
                Name = "Example Field Name",
                Area = 100,
                CurrentYear = 2019,
                CropYear = 2018,
                CropType = CropType.TameGrass,
                TillageType = TillageType.Reduced,
                YearInPerennialStand = 2,
                PerennialStandID = pernnialGuid,
                PerennialStandLength = 2,
                BiomassCoefficientProduct = 10,
                BiomassCoefficientStraw = 10,
                BiomassCoefficientRoots = 10,
                BiomassCoefficientExtraroot = 10,
                NitrogenContentInProduct = 10,
                NitrogenContentInStraw = 10,
                NitrogenContentInRoots = 10,
                NitrogenContentInExtraroot = 10,
                CarbonConcentration = 5,
                Yield = 100,
                HarvestMethod = HarvestMethods.Silage,
                NitrogenFertilizerRate = 15,
                PhosphorousFertilizerRate = 15,
                IsIrrigated = Response.Yes,
                IrrigationType = IrrigationType.Irrigated,
                AmountOfIrrigation = 30,
                MoistureContentOfCrop = 20,
                PercentageOfStrawReturnedToSoil = 15,
                PercentageOfRootsReturnedToSoil = 15,
                PercentageOfProductYieldReturnedToSoil = 15,
                IsPesticideUsed = Response.Yes,
                NumberOfPesticidePasses = 2,
                ManureApplied = true,
                AmountOfManureApplied = 20,
                ManureApplicationType = ManureApplicationTypes.OptionA,
                ManureAnimalSourceType = ManureAnimalSourceTypes.BeefManure,
                ManureStateType = ManureStateType.Composted,
                ManureLocationSourceType = ManureLocationSourceType.Imported,
                CoverCropsUsed = true,
                CoverCropType = CoverCropTypes.OptionA,
                CoverCropTerminationType = CoverCropTerminationType.OptionA,
                UnderSownCropsUsed = true,
                CropIsGrazed = true
            };

            var tempFieldRow3 = new FieldTemporaryInput()
            {
                PhaseNumber = 1,
                GroupId = 0,
                Name = "Example Field Name",
                Area = 100,
                CurrentYear = 2019,
                CropYear = 2018,
                CropType = CropType.Barley,
                TillageType = TillageType.Reduced,
                YearInPerennialStand = 0,
                PerennialStandID = pernnialGuid,
                PerennialStandLength = 0,
                BiomassCoefficientProduct = 10,
                BiomassCoefficientStraw = 10,
                BiomassCoefficientRoots = 10,
                BiomassCoefficientExtraroot = 10,
                NitrogenContentInProduct = 10,
                NitrogenContentInStraw = 10,
                NitrogenContentInRoots = 10,
                NitrogenContentInExtraroot = 10,
                CarbonConcentration = 5,
                Yield = 100,
                HarvestMethod = HarvestMethods.Silage,
                NitrogenFertilizerRate = 15,
                PhosphorousFertilizerRate = 15,
                IsIrrigated = Response.Yes,
                IrrigationType = IrrigationType.Irrigated,
                AmountOfIrrigation = 30,
                MoistureContentOfCrop = 20,
                PercentageOfStrawReturnedToSoil = 15,
                PercentageOfRootsReturnedToSoil = 15,
                PercentageOfProductYieldReturnedToSoil = 15,
                IsPesticideUsed = Response.Yes,
                NumberOfPesticidePasses = 2,
                ManureApplied = true,
                AmountOfManureApplied = 20,
                ManureApplicationType = ManureApplicationTypes.OptionA,
                ManureAnimalSourceType = ManureAnimalSourceTypes.BeefManure,
                ManureStateType = ManureStateType.Composted,
                ManureLocationSourceType = ManureLocationSourceType.Imported,
                CoverCropsUsed = true,
                CoverCropType = CoverCropTypes.OptionA,
                CoverCropTerminationType = CoverCropTerminationType.OptionA,
                UnderSownCropsUsed = true,
                CropIsGrazed = true
            };

            var fieldTemporaryInputList = new List<IComponentTemporaryInput>()
            {
                tempFieldRow1,
                tempFieldRow2,
                tempFieldRow3
            };

            allFields.Add(fieldTemporaryInputList);  
            var allFieldsWithGuidSet = componentConverterHandler.SetComponentListGuid(allFields);
            var convertedFields = componentConverterHandler.StartComponentConversion("Fields", new Farm(), allFieldsWithGuidSet);
            var convertedFieldExample = convertedFields.Cast<FieldSystemComponent>().ToList();
            //Testing Perennial Stand Group Guid
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[0].PerennialStandGroupId, convertedFieldExample[0].CropViewItems[1].PerennialStandGroupId);
            Assert.AreNotEqual(convertedFieldExample[0].CropViewItems[0].PerennialStandGroupId, convertedFieldExample[0].CropViewItems[2].PerennialStandGroupId);
            //Testing Other Data
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].PhaseNumber, 1);
            Assert.AreEqual(convertedFieldExample[0].Name, "Example Field Name");
            Assert.AreEqual(convertedFieldExample[0].FieldArea, 100);
            Assert.AreEqual(convertedFieldExample[0].YearOfObservation, 2019);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].Area, 100);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].CropType, CropType.Barley);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].TillageType, TillageType.Reduced);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].YearInPerennialStand, 0);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].PerennialStandLength, 1);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].BiomassCoefficientExtraroot, 10);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].BiomassCoefficientProduct, 10);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].BiomassCoefficientRoots, 10);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].BiomassCoefficientStraw, 10);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].NitrogenContentInExtraroot, 10);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].NitrogenContentInProduct, 10);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].NitrogenContentInRoots, 10);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].NitrogenContentInStraw, 10);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].CarbonConcentration, 5);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].Yield, 100);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].HarvestMethod, HarvestMethods.Silage);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].NitrogenFertilizerRate, 15);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].PhosphorusFertilizerRate, 15);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].IsIrrigated, Response.Yes);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].IrrigationType, IrrigationType.Irrigated);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].AmountOfIrrigation, 30);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].MoistureContentOfCrop, 20);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].PercentageOfProductYieldReturnedToSoil, 15);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].PercentageOfRootsReturnedToSoil, 15);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].PercentageOfStrawReturnedToSoil, 15);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].IsPesticideUsed, Response.Yes);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].NumberOfPesticidePasses, 2);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].ManureApplied, true);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].AmountOfManureApplied, 20);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].ManureApplicationType, ManureApplicationTypes.OptionA);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].ManureAnimalSourceType, ManureAnimalSourceTypes.BeefManure);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].ManureStateType, ManureStateType.Composted);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].ManureLocationSourceType, ManureLocationSourceType.Imported);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].UnderSownCropsUsed, true);
            Assert.AreEqual(convertedFieldExample[0].CropViewItems[2].CropIsGrazed, true);
        }    
    }
}
