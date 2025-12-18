using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using H.Core.CustomAttributes;
using H.Core.Enumerations;

namespace H.Core.Test.CustomAttributes
{
    [TestClass]
    public class ConvertUnitsAttributeTest
    {
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double weight { get; set; }

        [TestMethod]
        public void TestAttribute()
        {
            var type = this.GetType();

            var memberInfo = type.GetMember(nameof(weight));
            var attrs = memberInfo[0].GetCustomAttributes(typeof(UnitsAttribute), false);
            var units = ((UnitsAttribute)attrs[0]).SourceUnit;
            Assert.AreEqual(MetricUnitsOfMeasurement.Kilograms, units);
        }


    }
}
