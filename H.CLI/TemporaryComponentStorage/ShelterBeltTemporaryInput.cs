using System;
using System.Reflection;
using H.CLI.Interfaces;
using H.CLI.UserInput;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Converters;
using H.Core.Enumerations;

namespace H.CLI.TemporaryComponentStorage
{
    public class ShelterBeltTemporaryInput : IComponentTemporaryInput
    {
        private UnitsOfMeasurementCalculator _unitsOfMeasurementCalculator = new UnitsOfMeasurementCalculator();
        private InputHelper _inputHelper = new InputHelper();

        #region Constructor
        public ShelterBeltTemporaryInput() { }
        #endregion

        #region Public Methods
        /// <summary>
        /// Takes in the property we want to set, the value we want to set the property to, the row + 1 and column where the value
        /// is located and the path of the file we are currently processing. These are all determined by the Parser.
        /// If the data field is empty at (row + 1 x column), throw a format exception stating there is no data input.
        /// If the property contains units, indicated by parentheses (ie, Area(ha)), then the property will be adjusted 
        /// by removing the parentheses.
        /// Then, we get the propertyInfo based on the key (ie, if the key is "YearOfObservation" then propertyInfo will be of type int)
        /// We will delegate the setting of properties and edge case checking to another function called InputDataReflectionHandler
        /// </summary>
        public void ConvertToComponentProperties(string key, ImperialUnitsOfMeasurement? units, string value, int row, int col, string filePath)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            if (String.IsNullOrEmpty(value))
            {
                Console.WriteLine(String.Format(Properties.Resources.EmptyDataInput, row + 1, col + 1, filePath));
                throw new FormatException(String.Format(Properties.Resources.EmptyDataInput, row + 1, col + 1, filePath));
            }
  
            var propertyInfo = this.GetType().GetProperty(key);
            InputDataReflectionHandler(propertyInfo, units, key, value, filePath, col, row);

            //The GroupId is just used for our GUID Component Handler class
            GroupId = RowID; 
        }

        /// <summary>
        /// Responsible for settings the properties using reflection and checking edge cases (ie, not a valid integer, or not a valid Enum)
        /// Throws the appropriate exception based on the edge case that occurs.
        /// </summary>
        public void InputDataReflectionHandler(PropertyInfo propertyInfo, ImperialUnitsOfMeasurement? units, string prop, string value, string filePath, int col, int row)
        {
            //This exception is only for the Developer, if you add a new property, please make sure it is in the correct format.
            //The compiler cannot find the property in the list of properties for this class and therefore it is null.
            if (propertyInfo == null)
            {
                throw new InvalidPropertyNameException(String.Format
                                 (Properties.Resources.InvalidPropertyName,
                                 row + 1, col + 1, filePath));
            }


            if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(double))
            {     
                var convertedIntValue = double.Parse(value, CLILanguageConstants.culture);
                if (convertedIntValue < 0)
                {
                    Console.WriteLine(String.Format
                                               (Properties.Resources.NegativeInteger, value,
                                               row + 1, col + 1, filePath));

                    throw new FormatException(String.Format
                                               (Properties.Resources.NegativeInteger, value,
                                               row + 1, col + 1, filePath));
                }

                if (CLIUnitsOfMeasurementConstants.measurementSystem == MeasurementSystemType.Imperial && units != null)
                {
                    convertedIntValue = _unitsOfMeasurementCalculator.ConvertValueToMetricFromImperial(units, convertedIntValue);
                    propertyInfo.SetValue(this, Convert.ChangeType(convertedIntValue, propertyInfo.PropertyType, CLILanguageConstants.culture), null);
                    return;
                }


               
            }  

            if (propertyInfo.PropertyType == typeof(TreeSpecies))
            {
                try
                {
                    if (_inputHelper.IsNotApplicableInput(value))
                    {
                        this.Species = TreeSpecies.Caragana;
                        return;
                    }

                    var tree = new TreeSpeciesStringConverter();
                    this.Species = tree.Convert(value);
                    return;
                }

                catch (Exception)
                {
                    Console.WriteLine(String.Format(Properties.Resources.InvalidTreeSpecies, value, row + 1, col + 1, filePath));
                }
               
            }

            if (propertyInfo.PropertyType == typeof(HardinessZone))
            {
                try
                {
                    if (_inputHelper.IsNotApplicableInput(value))
                    {
                        this.HardinessZone = HardinessZone.NotAvailable;
                        return;
                    }

                    this.HardinessZone = (HardinessZone)Enum.Parse(typeof(HardinessZone), value, true);
                    return;
                }

                catch (Exception)
                {
                    Console.WriteLine(String.Format(Properties.Resources.InvalidHardinessZone, value, row + 1, col + 1, filePath));
                    throw new FormatException(String.Format(Properties.Resources.InvalidHardinessZone, value, row + 1, col + 1, filePath));
                }
            }

            else
                propertyInfo.SetValue(this, Convert.ChangeType(value, propertyInfo.PropertyType, CLILanguageConstants.culture), null);
        }

        public void FinalSettings(IComponentKeys componentKeys)
        {
        }
        #endregion

        //When adding a new property, follow the format: NewProperty. If you add a new property here, make sure to add the
        //appropriate key to the list of keys in the ShelterBeltKeys class.
        #region Properties
        public string Name { get; set; }
        //The GUID refers to the Row ID, the GroupId is just used for our GUIDComponentHandler
        public Guid Guid { get; set; }
        public int GroupId { get; set; }
        public int RowID { get; set; }
        public int PlantYear { get; set; }
        public int CutYear { get; set; }
        public int EcodistrictID { get; set; }
        public HardinessZone HardinessZone { get; set; }
        public int YearOfObservation { get; set; }
        public int ShelterbeltID { get; set; }
        public string RowName { get; set; }
        public TreeSpecies Species { get; set; }
        public double RowLength { get; set; }
        public double PlantedTreeSpacing { get; set; }
        public int PlantedTreeCount { get; set; }
        public int LiveTreeCount { get; set; }
        public double AverageCircumference { get; set; }
        public string GroupName { get; set; }
        public AnimalType GroupType { get; set; }
        #endregion


    }
}

