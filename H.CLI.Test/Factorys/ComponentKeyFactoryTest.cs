using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using H.CLI.ComponentKeys;
using H.CLI.Factorys;
using H.CLI.UserInput;

namespace H.CLI.Test.Factorys
{
    [TestClass]
    public class ComponentKeyFactoryTest
    {
        [TestMethod]
        public void TestComponentKeys_ExpectShelterBeltKeysObject_English()
        {
            CLILanguageConstants.culture = CultureInfo.CreateSpecificCulture("en-CA");
            var componentFactory = new ComponentKeyFactory();
            var result = componentFactory.ComponentKeysCreator(Properties.Resources.DefaultShelterbeltInputFolder);
            Assert.IsInstanceOfType(result, typeof(ShelterBeltKeys));
        }

        [TestMethod]
        public void TestComponentKeys_ExpectShelterBeltKeysObject_French()
        {
            CLILanguageConstants.culture = CultureInfo.CreateSpecificCulture("fr-CA");
            var componentFactory = new ComponentKeyFactory();
            var result = componentFactory.ComponentKeysCreator(Properties.Resources.DefaultShelterbeltInputFolder);
            Assert.IsInstanceOfType(result, typeof(ShelterBeltKeys));
        }
    }
}
