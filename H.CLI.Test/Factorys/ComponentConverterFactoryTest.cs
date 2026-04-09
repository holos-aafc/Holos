using H.CLI.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using H.CLI.Converters;

namespace H.CLI.Test.Factorys
{
    [TestClass]
    public class ComponentConverterFactoryTest
    {
        [TestMethod]
        public void TestGetComponentConverter_InputShelterbelt_Expect_ShelterbeltConverter()
        {
            var componentConverterFactory = new ComponentConverterFactory();
            var result = componentConverterFactory.GetComponentConverter("Shelterbelts");
            Assert.IsInstanceOfType(result, typeof(ShelterbeltConverter));
        }

    }
}
