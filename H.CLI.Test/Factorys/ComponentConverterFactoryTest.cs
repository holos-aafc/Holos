using H.CLI.Factorys;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
