using System;
using H.Core.Enumerations;

namespace H.Core.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UnitsAttribute : Attribute
    {
        public UnitsAttribute(MetricUnitsOfMeasurement unit)
        {
            SourceUnit = unit;
        }

        public MetricUnitsOfMeasurement SourceUnit { get; }
    }
}