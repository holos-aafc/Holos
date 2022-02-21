using H.CLI.UserInput;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.TemporaryComponentStorage;

namespace H.CLI.Test.ComponentTemporaryInputs
{
    [TestClass]
    public class ShelterbeltFrenchTemporaryInputTest
    {

        [TestMethod]
        public void TestConvertToComponentProperties_FrenchInputValueAsAComma() 
        {
            CLILanguageConstants.culture = new CultureInfo("fr-CA");
            CLIUnitsOfMeasurementConstants.measurementSystem = MeasurementSystemType.Imperial;
            var shelterTempInput = new ShelterBeltTemporaryInput();
            shelterTempInput.ConvertToComponentProperties("PlantedTreeSpacing", ImperialUnitsOfMeasurement.Yards, "30,4", 1, 1, "fileName");
            Assert.AreEqual(27.798, Math.Round(shelterTempInput.PlantedTreeSpacing, 3));
        }

        [TestMethod]
        public void TestConvertToComponentProperties_EnglishInputValueAsAComma()
        {
            CLILanguageConstants.culture = new CultureInfo("en-CA");
            CLIUnitsOfMeasurementConstants.measurementSystem = MeasurementSystemType.Imperial;
            var shelterTempInput = new ShelterBeltTemporaryInput();
            shelterTempInput.ConvertToComponentProperties("PlantedTreeSpacing", ImperialUnitsOfMeasurement.Yards, "30.4", 1, 1, "fileName");
            Assert.AreEqual(27.798, Math.Round(shelterTempInput.PlantedTreeSpacing, 3));
        }

        [TestMethod]
        public void TestConvertToComponentProperties_FrenchInputValueAsANegative()
        {
            CLILanguageConstants.culture = new CultureInfo("fr-CA");
            CLIUnitsOfMeasurementConstants.measurementSystem = MeasurementSystemType.Imperial;
            var shelterTempInput = new ShelterBeltTemporaryInput();
            Assert.ThrowsException<FormatException>(() => shelterTempInput.ConvertToComponentProperties("RowLength", ImperialUnitsOfMeasurement.Yards, "-54,67", 1, 1, "fileName"));
        }
    }
}
