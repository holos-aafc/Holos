using H.CLI.Processors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.CLI.Test.Processors
{
    [TestClass]
    public class ProccessorHandlerTest
    {
        [TestMethod]
        public void TestSetProcessor()
        {
            var processorHandler = new ProcessorHandler();
            processorHandler.SetProccessor(new ShelterbeltProcessor());
            Assert.IsInstanceOfType(processorHandler._processor, typeof(ShelterbeltProcessor));
        }
    }
}
