using H.Core.Enumerations;
using System.Reflection;
using H.CLI.Interfaces;

namespace H.CLI.TemporaryComponentStorage
{
    public class OtherLiveStockTemporaryInput : AnimalTemporaryInputBase, IComponentTemporaryInput
    {
        #region Public Methods

        public override void InputDataReflectionHandler(PropertyInfo propertyInfo, ImperialUnitsOfMeasurement? units, string prop, string value, string filePath, int col, int row)
        {
            base.InputDataReflectionHandler(propertyInfo, units, prop, value, filePath, col, row);
        } 

        #endregion
    }
}
