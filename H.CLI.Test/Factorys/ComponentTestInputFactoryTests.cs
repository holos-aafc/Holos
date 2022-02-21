using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using H.CLI.Factorys;
using H.CLI.TemporaryComponentStorage;

namespace H.CLI.Test.Factorys
{
    [TestClass]
    public class ComponentTemporaryInputFactoryTests
    {

        [TestMethod]
        public void TestComponentInputFactory_ExpectShelterBeltTemporaryInputObject()
        {
            var inputFactory = new ComponentTemporaryInputFactory();
            
            var result = inputFactory.CreateComponentTemporaryInputs("ShelterBelts");
            Assert.IsInstanceOfType(result, typeof(ShelterBeltTemporaryInput));
        }
        
    }
}
