using H.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using H.CLI.Interfaces;
using H.CLI.UserInput;

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
