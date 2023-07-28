using H.Core.Enumerations;
using H.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using H.Core.Models.Animals.Beef;

namespace H.CLI.TemporaryComponentStorage
{
    public abstract class TemporaryInputBase
    {
        protected readonly InputHelper _inputHelper = new InputHelper();
        public Type ComponentType { get; set; }

        public virtual void InputDataReflectionHandler(PropertyInfo propertyInfo, ImperialUnitsOfMeasurement? units, string prop, string value, string filePath, int col, int row)
        {
            //This exception is only for the Developer, if you add a new property, please make sure it is in the correct format.
            //The compiler cannot find the property in the list of properties for this class and therefore it is null.
            if (propertyInfo == null)
            {
                throw new InvalidPropertyNameException(String.Format
                (Properties.Resources.InvalidPropertyName,
                    row + 1, col + 1, filePath));
            }
        }

        /// <summary>
        /// Get the type of component from the input file
        /// </summary>
        public void ParseType(PropertyInfo propertyInfo, ImperialUnitsOfMeasurement? units, string prop, string value,
            string filePath, int col, int row)
        {
            try
            {
                if (_inputHelper.IsNotApplicableInput(value))
                {
                    this.ComponentType = typeof(CowCalfComponent);
                    return;
                }

                if (prop == nameof(ComponentType))
                {
                    var assembly = Assembly.GetAssembly(typeof(ComponentBase)).FullName.Split(new char[] { ',' })[0];

                    var type = Type.GetType($"{value}, {assembly}");

                    this.ComponentType = type;

                    return;
                }
            }

            catch
            {
                throw new FormatException(String.Format(Properties.Resources.InvalidDataInput, value, row + 1, col + 1, filePath));
            }
        }
    }
}
