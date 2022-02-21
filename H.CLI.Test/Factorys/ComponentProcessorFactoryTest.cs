using H.CLI.Factorys;
using H.CLI.Processors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Models.LandManagement.Shelterbelt;

namespace H.CLI.Test.Factorys
{
    [TestClass]
    public class ComponentProcessorFactoryTest
    {
        [TestMethod]
        public void TestComponentProcessorFactory()
        {
            var componentProcessorFactory = new ComponentProcessorFactory();
            var result = componentProcessorFactory.GetComponentProcessor(new ShelterbeltComponent().GetType());
            Assert.IsInstanceOfType(result, typeof(ShelterbeltProcessor));

        }

    }
}
