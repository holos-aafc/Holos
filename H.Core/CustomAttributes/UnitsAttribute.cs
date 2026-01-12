using System;
using H.Core.Enumerations;

namespace H.Core.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UnitsAttribute : Attribute
    {
        public UnitsAttribute(MetricUnitsOfMeasurement unit)
        {
            this.SourceUnit = unit;
        }

        public MetricUnitsOfMeasurement SourceUnit { get; }
    }
}
