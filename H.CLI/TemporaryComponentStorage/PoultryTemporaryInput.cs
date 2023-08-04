using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using H.CLI.Interfaces;
using H.CLI.UserInput;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals.Beef;

namespace H.CLI.TemporaryComponentStorage
{
    public class PoultryTemporaryInput : AnimalTemporaryInputBase, IComponentTemporaryInput
    {
        #region Public Methods

        public override void InputDataReflectionHandler(PropertyInfo propertyInfo, ImperialUnitsOfMeasurement? units, string prop, string value, string filePath, int col, int row)
        {
            base.InputDataReflectionHandler(propertyInfo, units, prop, value, filePath, col, row);
        }

        #endregion
    }
}
