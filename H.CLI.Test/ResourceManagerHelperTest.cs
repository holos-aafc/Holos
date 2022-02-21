using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.CLI.Test
{
    [TestClass]
    public class ResourceManagerHelperTest
    {

        [TestMethod]
        public void GetKeyGivenValueInResourceFile_FrenchStringForName_ExpectNameStringToBeReturnedGivenNom()
        {
            var result = Properties.Resources.ResourceManager.GetResourceName("nom", CultureInfo.GetCultureInfo("fr-CA"));
            Assert.AreEqual("Name", result);       
        }

        [TestMethod]
        public void GetKeyGivenValueInResourceFiles()
        {
            var result = Properties.Resources.ResourceManager.GetResourceName("Shelterbelts", CultureInfo.GetCultureInfo("en-CA"));
            Assert.AreEqual("DefaultShelterbeltInputFolder", result);
        }

        [TestMethod]
        public void GetKeyGivenValueInResourceFiles_ExpectCaseInsensitivity()
        {
            var result = Properties.Resources.ResourceManager.GetResourceName("SHElteRbelts", CultureInfo.GetCultureInfo("en-CA"));
            Assert.AreEqual("DefaultShelterbeltInputFolder", result);
        }

        [TestMethod]
        public void GetKeyGivenValueInResourceFiles_ExpectExceptionThrown_InvalidResourceValue()
        {
            Assert.ThrowsException<Exception>(() => Properties.Resources.ResourceManager.GetResourceName("NOTAVALIDRESOURCEVALUE", CultureInfo.GetCultureInfo("en-CA")));
            
        }

        [TestMethod]
        public void GetKeyGivenValueInResourceFiles_FrenchStringForShelterbelts_ExpectShelterbeltsStringToBeReturned()
        {
            var result = Properties.Resources.ResourceManager.GetResourceName("Brise-vent", CultureInfo.GetCultureInfo("fr-CA"));
            Assert.AreEqual("Shelterbelt", result);
        }

        [TestMethod]
        public void GetKeyGivenValueInResourceFiles_FrenchStringForOutputDirectoryPath_ExpectAccentsToBeValid()
        {
            var result = Properties.Resources.ResourceManager.GetResourceName(@"Répertoire de soRtie = C:\\Holos\\InterfaceDeLigneDeCommandeHolos\\Sorties", CultureInfo.GetCultureInfo("fr-CA"));
            Assert.AreEqual("OutputDirectory", result);
        }

       

    }
}
