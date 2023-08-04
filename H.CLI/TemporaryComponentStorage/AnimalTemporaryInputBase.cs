using System;
using H.CLI.UserInput;
using System.Reflection;
using H.Core.Enumerations;
using H.Core.Models.Animals.Beef;
using H.Core.Models;
using H.Views.ComponentViews.LandManagement.FieldSystem.Controls;

namespace H.CLI.TemporaryComponentStorage
{
    public abstract class AnimalTemporaryInputBase : TemporaryInputBase
    {
        #region Properties
        
        public string GroupName { get; set; }
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
        public int GroupId { get; set; }
        public AnimalType GroupType { get; set; }
        public double DailyManureMethaneEmissionRate { get; set; }
        public double MethaneProducingCapacityOfManure { get; set; }
        public double MethaneConversionFactor { get; set; }
        public double VolatileSolids { get; set; }
        public int NumberOfLambs { get; set; }
        public double InitialWeight { get; set; }
        public double FinalWeight { get; set; }
        public double ADG { get; set; }
        public double EnergyRequiredToProduceWool { get; set; }
        public double EnergyRequiredToProduceMilk { get; set; }
        public double WoolProduction { get; set; }
        public double FeedIntake { get; set; }
        public DietAdditiveType DietAdditiveType { get; set; }
        public double MethaneConversionFactorOfDiet { get; set; }
        public double MethaneConversionFactorAdjusted { get; set; }
        public double CrudeProtein { get; set; }
        public double Forage { get; set; }
        public double TDN { get; set; }
        public double Starch { get; set; }
        public double Fat { get; set; }
        public double ME { get; set; }
        public double NDF { get; set; }
        public HousingType HousingType { get; set; }
        public string PastureLocation { get; set; }
        public double ActivityCoefficient { get; set; }
        public double GainCoefficientA { get; set; }
        public double GainCoefficientB { get; set; }
        public double MaintenanceCoefficient { get; set; }
        public ManureStateType ManureManagement { get; set; }
        public double MethaneConversionFactorOfManure { get; set; }
        public double AshContent { get; set; }
        public double EmissionFactorLeaching { get; set; }
        public double EmissionFactorVolatilization { get; set; }
        public double FractionLeaching { get; set; }
        public double VolatileSolidAdjusted { get; set; }
        public double NitrogenExcretionAdjusted { get; set; }
        public double CA { get; set; }
        public double CFTemp { get; set; }
        public double VolatileSolidsExcretion { get; set; }
        public double StartWeight { get; set; }
        public double EndWeight { get; set; }
        public double AverageDailyGain { get; set; }
        public double MilkProduction { get; set; }
        public double MilkFatContent { get; set; }
        public double MilkProtein { get; set; }
        public double GainCoefficient { get; set; }
        public int CowCalfPairingNumber { get; set; }
        public int NumberOfCalves { get; set; }
        public double InitialWeightOfCalves { get; set; }
        public double FinalWeightOfCalves{ get; set; }

        #endregion

        #region Public Methods

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

        public override void InputDataReflectionHandler(PropertyInfo propertyInfo, ImperialUnitsOfMeasurement? units, string prop, string value, string filePath, int col, int row)
        {
            base.InputDataReflectionHandler(propertyInfo, units, prop, value, filePath, col, row);

            switch (propertyInfo.PropertyType)
            {
                case Type _ when propertyInfo.PropertyType == typeof(Type):
                    {
                        try
                        {
                            if (_inputHelper.IsNotApplicableInput(value))
                            {
                                this.ComponentType = typeof(CowCalfComponent);
                            }
                            else if (prop == nameof(ComponentType))
                            {
                                var assembly = Assembly.GetAssembly(typeof(ComponentBase)).FullName.Split(new char[] { ',' })[0];

                                var type = Type.GetType($"{value}, {assembly}");

                                this.ComponentType = type;
                            }
                        }
                        catch
                        {
                            this.ThrowInvalidInput(Properties.Resources.InvalidDataInput, value, row + 1, col + 1, filePath);
                        }

                        break;
                    }

                case Type _ when propertyInfo.PropertyType == typeof(AnimalType):
                    {
                        try
                        {
                            if (_inputHelper.IsNotApplicableInput(value))
                            {
                                this.GroupType = AnimalType.NotSelected;
                            }
                            else
                            {
                                this.GroupType = (AnimalType)Enum.Parse(typeof(AnimalType), value, true);
                            }
                        }
                        catch
                        {
                            this.ThrowInvalidInput(Properties.Resources.InvalidDataInput, value, row + 1, col + 1, filePath);
                        }

                        break;
                    }

                case Type _ when propertyInfo.PropertyType == typeof(HousingType):
                {
                    try
                    {
                        if (_inputHelper.IsNotApplicableInput(value))
                        {
                            this.HousingType = HousingType.NotSelected;
                        }
                        else
                        {
                            this.HousingType = (HousingType)Enum.Parse(typeof(HousingType), value, true);
                        }
                    }
                    catch
                    {
                        this.ThrowInvalidInput(Properties.Resources.InvalidDataInput, value, row + 1, col + 1, filePath);
                    }

                    break;
                }

                case Type _ when propertyInfo.PropertyType == typeof(DietAdditiveType):
                    {
                        try
                        {
                            if (_inputHelper.IsNotApplicableInput(value))
                            {
                                this.DietAdditiveType = DietAdditiveType.None;
                            }
                            else
                            {
                                this.DietAdditiveType = (DietAdditiveType)Enum.Parse(typeof(DietAdditiveType), value, true);
                            }
                        }
                        catch
                        {
                            this.ThrowInvalidInput(Properties.Resources.InvalidDataInput, value, row + 1, col + 1, filePath);
                        }

                        break;
                    }

                case Type _ when propertyInfo.PropertyType == typeof(DateTime):
                    {
                        try
                        {
                            if (_inputHelper.IsNotApplicableInput(value))
                            {
                                this.ManagementPeriodStartDate = DateTime.Now;
                                this.ManagementPeriodEndDate = DateTime.Now;
                            }
                            else if (prop == nameof(ManagementPeriodStartDate))
                            {
                                this.ManagementPeriodStartDate = Convert.ToDateTime(value, CLILanguageConstants.culture);
                            }
                            else
                            {
                                this.ManagementPeriodEndDate = Convert.ToDateTime(value, CLILanguageConstants.culture);
                            }
                        }
                        catch (Exception)
                        {
                            this.ThrowInvalidInput(Properties.Resources.InvalidDate, value, row + 1, col + 1, filePath);
                        }

                        break;
                    }

                default:
                    {
                        propertyInfo.SetValue(this, Convert.ChangeType(value, propertyInfo.PropertyType, CLILanguageConstants.culture), null);

                        break;
                    }   
            }
        }
        #endregion

        #region Private Methods

        private void ThrowInvalidInput(string message, string value, int row, int col, string filePath)
        {
            throw new FormatException(String.Format(Properties.Resources.InvalidDataInput, value, row + 1, col + 1, filePath));
        }

        #endregion
    }
}