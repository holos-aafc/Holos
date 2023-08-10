using H.Core.Enumerations;
using System.Collections.Generic;

namespace H.CLI.ComponentKeys
{
    /// <summary>
    /// The dictionary below takes in a string - the header and a nullable ImperialUnitsOfMeasurement?. We do not include
    /// the MetricUnitsOfMeasurement because the calculations performed later demand that the values be in Metric.
    /// Therefore, in our parser, we need to convert all Imperial units to Metric units and to do that, we only need
    /// to know what the Imperial units are (because if its metric, we don't need to do anything because the data is already
    /// in Metric). The Imperial Units will be used in our ConvertToMetricFromImperial method using a switch statement based
    /// on the ImperialUnitsOfMeasurement here and convert the data to Metric for the calculations.
    /// </summary>
    public abstract class AnimalKeyBase
    {
        #region Properties

        public Dictionary<string, ImperialUnitsOfMeasurement?> Keys { get; set; }
        public Dictionary<string, bool> MissingHeaders { get; set; } = new Dictionary<string, bool>();

        #endregion

        #region Constructors

        protected AnimalKeyBase()
        {
            this.Keys = new Dictionary<string, ImperialUnitsOfMeasurement?>();
        }

        #endregion

        #region Public Methods

        public bool IsHeaderOptional(string s)
        {
            return false;
        }

        #endregion

        #region Private Methods

        #endregion
    }
}