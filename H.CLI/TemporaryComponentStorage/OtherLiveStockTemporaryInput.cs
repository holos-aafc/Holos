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
    public class OtherLiveStockTemporaryInput : IComponentTemporaryInput
    {
        private InputHelper _inputHelper = new InputHelper();

        public void ConvertToComponentProperties(string key, ImperialUnitsOfMeasurement? units, string value, int row, int col, string filePath)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            if (String.IsNullOrEmpty(value))
            {
                Console.WriteLine(String.Format(Properties.Resources.EmptyDataInput, row + 1, col + 1, filePath));
                throw new FormatException(String.Format(Properties.Resources.EmptyDataInput, row + 1, col + 1, filePath));
            }

            if (value.ToUpper() == "N/A")
            {
                return;
            }
            var propertyInfo = this.GetType().GetProperty(key);
            InputDataReflectionHandler(propertyInfo, units, key, value, filePath, col, row);
        }

        public void InputDataReflectionHandler(PropertyInfo propertyInfo, ImperialUnitsOfMeasurement? units, string prop, string value, string filePath, int col, int row)
        {
            if (propertyInfo.PropertyType == typeof(AnimalType))
            {
                try
                {
                    if (_inputHelper.IsNotApplicableInput(value))
                    {
                        this.GroupType = AnimalType.NotSelected;
                        return;
                    }

                    this.GroupType = (AnimalType)Enum.Parse(typeof(AnimalType), value, true);
                    return;
                }

                catch
                {
                    throw new FormatException(String.Format(Properties.Resources.InvalidDataInput, value, row + 1, col + 1, filePath));
                }
            }
            if (propertyInfo.PropertyType == typeof(DateTime))
            {
                try
                {
                    if (_inputHelper.IsNotApplicableInput(value))
                    {
                        this.ManagementPeriodStartDate = DateTime.Now;
                        this.ManagementPeriodEndDate = DateTime.Now;
                        return;
                    }

                    if (prop == nameof(ManagementPeriodStartDate))
                    {
                        this.ManagementPeriodStartDate = Convert.ToDateTime(value, CLILanguageConstants.culture);
                        return;
                    }

                    else
                        this.ManagementPeriodEndDate = Convert.ToDateTime(value, CLILanguageConstants.culture);
                    return;
                }

                catch (Exception)
                {
                    Console.WriteLine(String.Format(Properties.Resources.InvalidDate, value, row + 1, col + 1, filePath));
                    throw new FormatException(String.Format(Properties.Resources.InvalidDate, value, row + 1, col + 1, filePath));
                }
            }

            else
                propertyInfo.SetValue(this, Convert.ChangeType(value, propertyInfo.PropertyType, CLILanguageConstants.culture), null);
        }

        public void FinalSettings(IComponentKeys componentKeys)
        {
        }

        public string Name { get; set; }
        public string GroupName { get; set; }
        public AnimalType GroupType { get; set; }
        public int NumberOfAnimals { get; set; }
        public string ManagementPeriodName { get; set; }
        public DateTime ManagementPeriodStartDate { get; set; }
        public DateTime ManagementPeriodEndDate { get; set; }
        public int ManagementPeriodDays { get; set; }
        public double N2ODirectEmissionFactor { get; set; }
        public double VolatilizationFraction { get; set; }
        public double YearlyManureMethaneRate { get; set; }
        public double YearlyNitrogenExcretionRate { get; set; }
        public double YearlyEntericMethaneRate { get; set; }
        public Guid Guid { get; set; }
        public int GroupId { get; set; }
  
    }
}
