using H.CLI.UserInput;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.CLI.Test.UserInput
{
    [TestClass]
    public class CLILanguageConstantsTest
    {

        [TestMethod]
        public void TestSetCultureAndLanguageDefaults_EnglishCanada()
        {
            var userCulture = CultureInfo.GetCultureInfo("en-CA");
            CLILanguageConstants.SetCulture(userCulture);

            Assert.AreEqual(CLILanguageConstants.Delimiter, ",");
            Assert.AreEqual(CLILanguageConstants.OutputLanguageAddOn, "-en-CA.csv");
            Assert.AreEqual(CLILanguageConstants.DisplayDataSeparator, ".");
            Assert.AreEqual(CLILanguageConstants.culture.Name, "en-CA");
        }

        [TestMethod]
        public void TestSetCultureAndLanguageDefaults_InvalidUserCulture()
        {
            var userCulture = CultureInfo.GetCultureInfo("fr-EU");
            Assert.ThrowsException<NotSupportedException>(() => CLILanguageConstants.SetCulture(userCulture));
        }

    }
}
