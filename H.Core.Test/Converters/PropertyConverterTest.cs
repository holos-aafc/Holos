using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Converters;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Core.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Converters
{

    [TestClass]
    public class PropertyConverterTest
    {
        private PropertyConverter<Field> _converter;
        private Field _field;
        [TestInitialize]
        public void TestInitialize()
        {
            _field = new Field()
            {
                Yield = 1000,
                Area = 100,
            };
            _converter = new PropertyConverter<Field>(_field);
        }

        [TestMethod]
        public void TestGetSystemValueFromBinding()
        {
            Settings.Default.MeasurementSystem = MeasurementSystemType.Metric;
            var convertedYield = _converter.GetSystemValueFromBinding(nameof(Field.Yield));
            var convertedArea = _converter.GetSystemValueFromBinding(nameof(Field.Area));

            Assert.AreEqual(1000, convertedYield, 1);
            Assert.AreEqual(100, convertedArea, 1);

            Settings.Default.MeasurementSystem = MeasurementSystemType.Imperial;

            //both property values converted to metric
            convertedYield = _converter.GetSystemValueFromBinding(nameof(Field.Yield));
            convertedArea = _converter.GetSystemValueFromBinding(nameof(Field.Area));

            //kg/ha
            Assert.AreEqual(1000 * 2.471 / 2.205, convertedYield, 1);
            //ha
            Assert.AreEqual(100 / 2.471, convertedArea, 1);
        }

        [TestMethod]
        public void TestGetBindingValueFromSystem()
        {
            Settings.Default.MeasurementSystem = MeasurementSystemType.Metric;
            var bindingYield = _converter.GetBindingValueFromSystem(nameof(Field.Yield));
            var bindingArea = _converter.GetBindingValueFromSystem(nameof(Field.Area));

            var expectedYield = 1000.0;
            var expectedArea = 100.0;
            Assert.AreEqual(expectedYield, bindingYield, 1);
            Assert.AreEqual(expectedArea, bindingArea, 1);

            Settings.Default.MeasurementSystem = MeasurementSystemType.Imperial;
            bindingYield = _converter.GetBindingValueFromSystem(nameof(Field.Yield));
            bindingArea = _converter.GetBindingValueFromSystem(nameof(Field.Area));

            //ac
            expectedArea = 100 * 2.471;
            //lb/ac
            expectedYield = 1000 / 2.471 * 2.205;

            Assert.AreEqual(expectedYield, bindingYield, 1);
            //ac
            Assert.AreEqual(expectedArea, bindingArea, 1);

        }


    }

    public class Field
    {
        [Units(MetricUnitsOfMeasurement.KilogramsPerHectare)]
        public double Yield { get; set; }

        [Units(MetricUnitsOfMeasurement.Hectares)]
        public double Area { get; set; }
    }

}
