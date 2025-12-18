using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Converters;
using H.Core.Enumerations;

namespace H.Core.Test.Converters
{
    [TestClass]
    public class EconomicMeasurementStringConverterTest
    {
        private EconomicsMeasurementStringConverter _converter;

        [TestInitialize]
        public void Initialize()
        {
            _converter = new EconomicsMeasurementStringConverter();
        }

        [TestMethod]
        public void ConvertTest ()
        {
            Assert.AreEqual(EconomicMeasurementUnits.HundredWeight, _converter.Convert("cwt"));
            Assert.AreEqual(EconomicMeasurementUnits.Pound, _converter.Convert("lb"));
            Assert.AreEqual(EconomicMeasurementUnits.None, _converter.Convert("na"));
            Assert.AreEqual(EconomicMeasurementUnits.Tonne, _converter.Convert("t"));
            Assert.AreEqual(EconomicMeasurementUnits.Bushel, _converter.Convert("bu"));
        }

        [TestMethod]
        public void ConvertReturnsNoneWhenNotFound()
        {
            Assert.AreEqual(EconomicMeasurementUnits.None, _converter.Convert("bob"));
        }
    }
}
