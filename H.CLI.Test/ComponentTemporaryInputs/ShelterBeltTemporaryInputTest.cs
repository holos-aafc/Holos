using H.CLI.UserInput;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using H.CLI.TemporaryComponentStorage;

namespace H.CLI.Test.ComponentTemporaryInputs
{
    [TestClass]
    public class ShelterBeltTemporaryInputTest
    {

        [TestMethod]
        public void TestConvertToComponentProperties_TreeSpeciesConversion_ExpectSpeciesToBeSetToAverageConifer()
        {
            var shelterTempInput = new ShelterBeltTemporaryInput();
            shelterTempInput.ConvertToComponentProperties("Species", null, "Average Conifer", 1, 1, "fileName");
            Assert.AreEqual(shelterTempInput.Species, TreeSpecies.AverageConifer);
        }

        [TestMethod]
        public void TestConvertToComponentProperties_HardinessZoneConversion_ExpectHardinessZoneToBeSetToH0a()
        {
            var shelterTempInput = new ShelterBeltTemporaryInput();
            shelterTempInput.ConvertToComponentProperties("HardinessZone", null, "H0a", 1, 1, "fileName");
            Assert.AreEqual(shelterTempInput.HardinessZone, HardinessZone.H0a);
        }

        [TestMethod]
        public void TestConvertToComponentProperties_IntToIntConversion_ExpectYearOfObservationToBeSetTo1996()
        {
            var shelterTempInput = new ShelterBeltTemporaryInput();
            shelterTempInput.ConvertToComponentProperties("YearOfObservation", null, "1996", 1, 1, "fileName");
            Assert.AreEqual(shelterTempInput.YearOfObservation, 1996);
        }


        public void TestConvertToComponentProperties_StringToStringConversion_ExpectYearOfObservationToBeSetTo1996()
        {
            var shelterTempInput = new ShelterBeltTemporaryInput();
            shelterTempInput.ConvertToComponentProperties("RowName", null, "TestRowName", 1, 1, "fileName");
            Assert.AreEqual(shelterTempInput.RowName, "TestRowName");
        }

        [TestMethod]
        public void TestConvertToComponentProperties_ValueIsNotAValidIntegerAttemptIntToString_Expect_ThrowFormatException()
        {
            var shelterTempInput = new ShelterBeltTemporaryInput();

            Assert.ThrowsException<FormatException>(() =>
                                  shelterTempInput.ConvertToComponentProperties("YearOfObservation", null, "NotAValidNumber", 1, 1, "fileName"));
        }


        [TestMethod]
        public void TestConvertToComponentProperties_NegativeInteger_ExpectYearOfObservationToBeSetTo1996()
        {
            var shelterTempInput = new ShelterBeltTemporaryInput();
            Assert.ThrowsException<FormatException>(() => shelterTempInput.ConvertToComponentProperties("YearOfObservation", null, "-1996", 1, 1, "fileName"));
        }

        [TestMethod]
        public void TestConvertToComponentProperties_ExpectException_InvalidTreeSpecies_ExpectInvalidTreeSpecies()
        {
            var shelterTempInput = new ShelterBeltTemporaryInput();
            Assert.ThrowsException<InvalidCastException>(() =>
                                  shelterTempInput.ConvertToComponentProperties("Species", null, "NotAValidTree", 1, 1, "fileName"));
        }

        [TestMethod]
        public void TestConvertToComponentProperties_ExpectException_InvalidHardinessZone_ExpectFormatException()
        {
            var shelterTempInput = new ShelterBeltTemporaryInput();
            Assert.ThrowsException<FormatException>(() =>
                                  shelterTempInput.ConvertToComponentProperties("HardinessZone", null, "NotAValidHardinessZone", 1, 1, "fileName"));
        }


        [TestMethod]
        public void TestConvertToComponentProperties_ValueIsAnEmptyString_ExpectFormatException()
        {
            var shelterTempInput = new ShelterBeltTemporaryInput();

            Assert.ThrowsException<InvalidPropertyNameException>(() =>
                                  shelterTempInput.ConvertToComponentProperties("", null, "1996", 1, 1, "fileName"));
        }

   


    }
}
