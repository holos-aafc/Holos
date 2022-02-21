using System;
using System.Reflection;
using H.Core.Enumerations;

namespace H.CLI.Interfaces
{
    public interface IComponentTemporaryInput
    {
        #region Properties
        int GroupId { get; set; }
        string Name { get; set; }
        string GroupName { get; set; }
        Guid Guid { get; set; }
        AnimalType GroupType { get; set; } 
        #endregion

        #region Methods
        void ConvertToComponentProperties(string key, ImperialUnitsOfMeasurement? units, string value, int row, int col, string filePath);
        void InputDataReflectionHandler(PropertyInfo propertyInfo, ImperialUnitsOfMeasurement? units, string prop, string value, string filePath, int col, int row);
        /// <summary>
        /// If the user uses an old CSV component file that is missing some headers then this function will input default values instead.
        /// </summary>
        /// <param name="componentKeys">The component keys for the ComponentTemporaryInput</param>
        void FinalSettings(IComponentKeys componentKeys);
        #endregion

    }
}
