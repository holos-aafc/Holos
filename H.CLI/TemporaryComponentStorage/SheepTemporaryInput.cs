using H.Core.Enumerations;
using System.Reflection;
using H.CLI.Interfaces;

namespace H.CLI.TemporaryComponentStorage
{
    public class SheepTemporaryInput : AnimalTemporaryInputBase, IComponentTemporaryInput
    {

        public override void InputDataReflectionHandler(PropertyInfo propertyInfo, ImperialUnitsOfMeasurement? units, string prop, string value, string filePath, int col, int row)
        {
            base.InputDataReflectionHandler(propertyInfo, units, prop, value, filePath, col, row);
        }
    }
}
