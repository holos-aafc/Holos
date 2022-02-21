using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Core.Properties;

namespace H.Core.Converters
{
    public class PropertyConverter<T> : IPropertyConverter
    {
        #region Fields

        private readonly UnitsOfMeasurementCalculator _unitsCalculator;

        #endregion

        #region Constructors

        /// <summary>
        /// A converter to be used alongside the <see cref="UnitsAttribute"/>
        /// </summary>
        /// <param name="instance">any type that has properties using attribute <see cref="UnitsAttribute"/></param>
        public PropertyConverter(T instance)
        {
            //sometimes we will get null instances so lets handle that
            if (instance != null)
            {
                this.Instance = instance;
                this.Type = instance.GetType();
                this.PropertyInfos = this.Type.GetProperties().Where(x => Attribute.IsDefined(x, typeof(UnitsAttribute))).ToList();
                _unitsCalculator = new UnitsOfMeasurementCalculator();
            }
        }

        #endregion

        #region Properties

        public T Instance { get; set; }
        public Type Type { get; set; }
        public List<PropertyInfo> PropertyInfos { get; set; }

        #endregion

        #region Public Methods

        public double GetSystemValueFromBinding(string propertyName)
        {
            if (PropertyInfos != null)
            {
                //check that the property is something that we can work on
                var prop = this.PropertyInfos.FirstOrDefault(x => x.Name == propertyName);
                if (prop == null)
                {
                    Trace.TraceInformation($"{nameof(PropertyConverter<T>)}.{nameof(GetSystemValueFromBinding)}: {propertyName} cannot be converted or doesn't exist. Returning 0");
                    return 0;
                }

                //using reflection we will set the value of the property prop
                return this.GetSystemValueFromBinding(prop);
            }

            Trace.TraceInformation($"{nameof(PropertyConverter<T>)}.{nameof(GetSystemValueFromBinding)}: {nameof(PropertyInfos)} is null. Returning 0");
            return 0;
        }

        public double GetBindingValueFromSystem(string propertyName)
        {
            if (PropertyInfos != null)
            {
                var prop = this.PropertyInfos.FirstOrDefault(x => x.Name == propertyName);
                if (prop == null)
                {
                    Trace.TraceInformation($"{nameof(PropertyConverter<T>)}.{nameof(GetBindingValueFromSystem)}: {propertyName} cannot be converted or doesn't exist. Returning 0");
                    return 0;
                }

                return this.GetBindingValueFromSystem(prop);
            }

            Trace.TraceInformation($"{nameof(PropertyConverter<T>)}.{nameof(GetSystemValueFromBinding)}: {nameof(PropertyInfos)} is null. Returning 0");
            return 0;
        }


        public double GetBindingValueFromSystem(PropertyInfo prop)
        {
            //the list of attributes
            var attrs = prop.GetCustomAttributes(typeof(UnitsAttribute), false);

            if (Settings.Default.MeasurementSystem == MeasurementSystemType.Metric)
            {
                //the gui is in metric so just return system value
                return (double)prop.GetValue(this.Instance);
            }
            //convert for imperial
            if (this.Instance != null && attrs.Length > 0)
            {
                //I now have the metricUnit of the property in the system
                var metricUnit = ((UnitsAttribute)attrs[0]).SourceUnit;

                //now I need to get the value of the property
                var propValue = (double)prop.GetValue(this.Instance);

                //the imperial value for the binding
                var imperialValue = _unitsCalculator.ConvertValueToImperialFromMetric(metricUnit, propValue);

                return imperialValue;
            }
            Trace.TraceError($"{nameof(PropertyConverter<T>)}.{nameof(GetSystemValueFromBinding)}: unable to convert {prop.Name} value, returning 0.");
            return 0;
        }


        public double GetSystemValueFromBinding(PropertyInfo prop)
        {
            //nothing to convert and return
            if (Settings.Default.MeasurementSystem == MeasurementSystemType.Metric)
            {
                return (double)prop.GetValue(this.Instance);
            }

            //get the attribute on the property first
            var attrs = prop.GetCustomAttributes(typeof(UnitsAttribute), false);
            if (this.Instance != null && attrs.Length > 0)
            {
                //I now have the metricUnit of the property
                var metricUnit = ((UnitsAttribute)attrs[0]).SourceUnit;

                //the unit to convert from (i.e. lbs -> kg)
                var imperialUnit = _unitsCalculator.GetImperialUnitsOfMeasurement(metricUnit);

                //now I need to get the value of the property
                var propValue = (double)prop.GetValue(this.Instance);

                //Convert to Metric the value entered from imperial to metric
                var convertedValue = _unitsCalculator.ConvertValueToMetricFromImperial(imperialUnit, propValue, metricUnit);
                return convertedValue;
            }
            Trace.TraceError($"{nameof(PropertyConverter<T>)}.{nameof(GetSystemValueFromBinding)}: unable to convert {prop.Name} value, returning 0.");
            return 0;
        }

        #endregion
    }
}
