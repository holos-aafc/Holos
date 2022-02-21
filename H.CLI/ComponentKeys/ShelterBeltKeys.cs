using System.Collections.Generic;
using H.CLI.Interfaces;
using H.Core.Enumerations;

namespace H.CLI.ComponentKeys
{
    /// <summary>
    /// The dictionary below takes in a string - the header and a nullable ImperialUnitsOfMeasurement?. We do not include
    /// the MetricUnitsOfMeasurement because the calculations performed later demand that the values be in Metric.
    /// Therefore, in our parser, we need to convert all Imperial units to Metric units and to do that, we only need
    /// to know what the Imperial units are (because if its metric, we don't need to do anything because the data is already
    /// in Metric). The Imperial Units will be used in our ConvertToMetricFromImperial method using a switch statement based
    /// on the ImperialUnitsOfMeasurement here.
    /// </summary>
    public class ShelterBeltKeys : IComponentKeys
    {
        /// <summary>
        /// When you modify the key, remember to add a new property corresponding to the new key that you have added 
        /// below to the ShelterBeltTemporaryInput class in the format: "Example Format",
        /// In this case, please add a new property in the format: ExampleFormat to the concrete ShelterbeltTemporaryInput class.
        /// The order of the keys below is the order in which they will be written when creating the template files for a Shelterbelt
        /// </summary>

        public Dictionary<string, ImperialUnitsOfMeasurement?> keys { get; set; } = new Dictionary<string, ImperialUnitsOfMeasurement?>
        {
            {Properties.Resources.HardinessZone, null},
            {Properties.Resources.EcodistrictId, null },
            {Properties.Resources.YearOfObservation, null},
            {Properties.Resources.Key_Name, null},
            {Properties.Resources.RowName, null},
            {Properties.Resources.RowId, null},
            {Properties.Resources.RowLength, ImperialUnitsOfMeasurement.Yards},
            {Properties.Resources.PlantYear, null },
            {Properties.Resources.CutYear, null },
            {Properties.Resources.Species, null },
            {Properties.Resources.PlantedTreeCount, null },
            {Properties.Resources.LiveTreeCount, null },
            {Properties.Resources.PlantedTreeSpacing, ImperialUnitsOfMeasurement.Yards },
            {Properties.Resources.AverageCircumference, ImperialUnitsOfMeasurement.InchesToCm },
        };

        public bool IsHeaderOptional(string s)
        {
            return false;
        }
        public Dictionary<string, bool> MissingHeaders { get; set; } = new Dictionary<string, bool>();
    }

}
