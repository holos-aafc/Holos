using System;
using System.Globalization;
using System.Reflection;
using H.CLI.Interfaces;
using H.CLI.UserInput;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Enumerations;
using H.Core.Providers.Nitrogen;

namespace H.CLI.TemporaryComponentStorage
{

    public class FieldTemporaryInput : IComponentTemporaryInput
    {
        #region Fields

        private readonly UnitsOfMeasurementCalculator _unitsOfMeasurementCalculator = new UnitsOfMeasurementCalculator();
        private InputHelper _inputHelper = new InputHelper();
        private bool ThrowExceptionOnNegativeInput = false;

        #endregion

        #region Public Methods
        /// <summary>
        /// Takes in the property we want to set, the value we want to set the property to, the row + 1  and column where the value
        /// is located and the path of the file we are currently processing. These are all determined by the Parser.
        /// If the data field is empty at (row + 1 x column), throw a format exception stating there is no data input.
        /// Then, we get the propertyInfo based on the key (ie, if the key is "NitrogenFertilizerRate" then propertyInfo will be of type int)
        /// We will delegate the setting of properties and edge case checking to another function called InputDataReflectionHandler
        /// </summary>
        public void ConvertToComponentProperties(string prop, ImperialUnitsOfMeasurement? units, string value, int row, int col, string fileName)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            if (String.IsNullOrEmpty(value))
            {
                Console.WriteLine(String.Format(Properties.Resources.EmptyDataInput, row + 1, col + 1, fileName));
                throw new FormatException(String.Format(Properties.Resources.EmptyDataInput, row + 1, col + 1, fileName));
            }

            var propertyInfo = this.GetType().GetProperty(prop);
            InputDataReflectionHandler(propertyInfo, units, prop, value, fileName, col, row);
            ////The GroupId is used to set the GUID's for the list of components.
            //GroupId = PerennialStandID;
        }

        /// <summary>
        /// Responsible for setting the properties using reflection and checking edge cases (ie, not a valid integer, or not a valid Enum)
        /// Throws the appropriate exception based on the edge case that occurs.
        /// </summary>
        public void InputDataReflectionHandler(PropertyInfo propertyInfo, ImperialUnitsOfMeasurement? units, string prop, string value, string fileName, int col, int row)
        {
            //This exception is only for the Developer, if you add a new property, please make sure it is in the correct format.
            //The compiler cannot find the property in the list of properties for this class and therefore it is null.
            if (propertyInfo == null)
            {
                throw new InvalidPropertyNameException(String.Format
                                 (Properties.Resources.InvalidPropertyName,
                                 row + 1, col + 1, fileName));
            }

            if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(double))
            {
                if (_inputHelper.IsNotApplicableInput(value))
                {
                    propertyInfo.SetValue(this, Convert.ChangeType(0, propertyInfo.PropertyType));
                    return;
                }

                var convertedIntValue = double.Parse(value, CLILanguageConstants.culture);
                if (convertedIntValue < 0)
                {
                    if (ThrowExceptionOnNegativeInput)
                    {
                        Console.WriteLine(String.Format(Properties.Resources.NegativeInteger, value, row + 1, col + 1, fileName));

                        throw new FormatException(String.Format(Properties.Resources.NegativeInteger, value, row + 1, col + 1, fileName));
                    }
                    else
                    {
                        convertedIntValue = 0;
                    }
                }

                if (CLIUnitsOfMeasurementConstants.measurementSystem == MeasurementSystemType.Imperial && units != null)
                {
                    convertedIntValue = _unitsOfMeasurementCalculator.ConvertValueToMetricFromImperial(units, convertedIntValue);
                    propertyInfo.SetValue(this, Convert.ChangeType(convertedIntValue, propertyInfo.PropertyType, CLILanguageConstants.culture), null);

                    return;
                }
            }

            if (propertyInfo.PropertyType == typeof(Guid))
            {
                if (_inputHelper.IsNotApplicableInput(value))
                {
                    switch (prop)
                    {
                        case nameof(PerennialStandID):
                            PerennialStandID = Guid.Empty;
                            break;
                    }

                    return;
                }

                switch (prop)
                {
                    case nameof(PerennialStandID):
                        PerennialStandID = Guid.Parse(value);
                        break;
                }

                switch (prop)
                {
                    case nameof(FieldSystemComponentGuid):
                        FieldSystemComponentGuid = Guid.Parse(value);
                        break;
                }

                return;
            }

            if (propertyInfo.PropertyType == typeof(bool))
            {
                if (_inputHelper.IsNotApplicableInput(value))
                {
                    propertyInfo.SetValue(this, Convert.ChangeType(0, propertyInfo.PropertyType));
                    return;
                }

                //var convertedBoolean = bool.Parse(value);
                var convertedBoolean = value.ToLower(new CultureInfo("en-CA")) == "yes";
                propertyInfo.SetValue(this, Convert.ChangeType(convertedBoolean, propertyInfo.PropertyType, CLILanguageConstants.culture), null);

                return;
            }

            if (propertyInfo.PropertyType == typeof(Response))
            {
                try
                {
                    if (_inputHelper.IsNotApplicableInput(value))
                    {
                        switch (prop)
                        {
                            case nameof(IsIrrigated):
                                IsIrrigated = Response.No;
                                break;
                            case nameof(IsPesticideUsed):
                                IsPesticideUsed = Response.No;
                                break;
                        }
                        return;
                    }

                    switch (prop)
                    {
                        case nameof(IsIrrigated):
                            IsIrrigated = (Response)Enum.Parse(typeof(Response), value, true);
                            break;
                        case nameof(IsPesticideUsed):
                            IsPesticideUsed = (Response)Enum.Parse(typeof(Response), value, true);
                            break;
                        default:
                            throw new FormatException(String.Format(Properties.Resources.InvalidResponse, value, row + 1, col + 1, fileName));
                    }
                    return;
                }

                catch (Exception)
                {
                    throw new FormatException(String.Format(Properties.Resources.InvalidResponse, value, row + 1, col + 1, fileName));
                }
            }

            if (propertyInfo.PropertyType == typeof(ManureLocationSourceType))
            {
                try
                {
                    if (_inputHelper.IsNotApplicableInput(value))
                    {
                        ManureLocationSourceType = ManureLocationSourceType.NotSelected;
                        return;
                    }

                    ManureLocationSourceType = (ManureLocationSourceType)Enum.Parse(typeof(ManureLocationSourceType), value, true);
                    return;
                }

                catch (Exception)
                {
                    throw new FormatException(String.Format(Properties.Resources.InvalidManureLocationSourceType, value, row + 1, col + 1, fileName));
                }
            }

            if (propertyInfo.PropertyType == typeof(ManureStateType))
            {
                try
                {
                    if (_inputHelper.IsNotApplicableInput(value))
                    {
                        ManureStateType = ManureStateType.NotSelected;
                        return;
                    }

                    ManureStateType = (ManureStateType)Enum.Parse(typeof(ManureStateType), value, true);
                    return;
                }

                catch (Exception)
                {
                    throw new FormatException(String.Format(Properties.Resources.InvalidManureStateType, value, row + 1, col + 1, fileName));
                }
            }

            if (propertyInfo.PropertyType == typeof(ManureApplicationTypes))
            {

                try
                {
                    if (_inputHelper.IsNotApplicableInput(value))
                    {
                        ManureApplicationType = ManureApplicationTypes.NotSelected;
                        return;
                    }

                    ManureApplicationType = (ManureApplicationTypes)Enum.Parse(typeof(ManureApplicationTypes), value, true);
                    return;
                }

                catch (Exception)
                {
                    throw new FormatException(String.Format(Properties.Resources.InvalidManureApplicationType, value, row + 1, col + 1, fileName));
                }
            }

            if (propertyInfo.PropertyType == typeof(CoverCropTerminationType))
            {

                try
                {
                    if (_inputHelper.IsNotApplicableInput(value))
                    {
                        // Set to default value
                        CoverCropTerminationType = CoverCropTerminationType.OptionA;
                        return;
                    }

                    CoverCropTerminationType = (CoverCropTerminationType)Enum.Parse(typeof(CoverCropTerminationType), value, true);
                    return;
                }

                catch (Exception)
                {
                    throw new FormatException(String.Format(Properties.Resources.InvalidCoverCropTerminationType, value, row + 1, col + 1, fileName));
                }
            }

            if (propertyInfo.PropertyType == typeof(CoverCropTypes))
            {
                try
                {
                    if (_inputHelper.IsNotApplicableInput(value))
                    {
                        CoverCropType = CoverCropTypes.OptionA;
                        return;
                    }

                    CoverCropType = (CoverCropTypes)Enum.Parse(typeof(CoverCropTypes), value, true);
                    return;
                }

                catch (Exception)
                {
                    throw new FormatException(String.Format(Properties.Resources.InvalidCoverCropType, value, row + 1, col + 1, fileName));
                }
            }

            if (propertyInfo.PropertyType == typeof(ManureAnimalSourceTypes))
            {

                try
                {
                    if (_inputHelper.IsNotApplicableInput(value))
                    {
                        ManureAnimalSourceType = ManureAnimalSourceTypes.NotSelected;
                        return;
                    }

                    ManureAnimalSourceType = (ManureAnimalSourceTypes)Enum.Parse(typeof(ManureAnimalSourceTypes), value, true);
                    return;
                }

                catch (Exception)
                {
                    throw new FormatException(String.Format(Properties.Resources.InvalidManureAnimalSourceType, value, row + 1, col + 1, fileName));
                }
            }

            if (propertyInfo.PropertyType == typeof(IrrigationType))
            {

                try
                {
                    if (_inputHelper.IsNotApplicableInput(value))
                    {
                        IrrigationType = IrrigationType.Irrigated;
                        return;
                    }

                    IrrigationType = (IrrigationType)Enum.Parse(typeof(IrrigationType), value, true);
                    return;
                }

                catch (Exception)
                {
                    throw new FormatException(String.Format(Properties.Resources.InvalidIrrigationType, value, row + 1, col + 1, fileName));
                }
            }

            if (propertyInfo.PropertyType == typeof(CropType))
            {
                try
                {
                    if (_inputHelper.IsNotApplicableInput(value))
                    {
                        CropType = CropType.NotSelected;
                        return;
                    }

                    CropType = (CropType)Enum.Parse(typeof(CropType), value, true);
                    return;
                }

                catch (Exception)
                {
                    throw new FormatException(String.Format(Properties.Resources.InvalidCropType, value, row + 1, col + 1, fileName));
                }
            }

            if (propertyInfo.PropertyType == typeof(HarvestMethods))
            {

                try
                {
                    if (_inputHelper.IsNotApplicableInput(value))
                    {
                        HarvestMethod = HarvestMethods.Silage;
                        return;
                    }

                    HarvestMethod = (HarvestMethods)Enum.Parse(typeof(HarvestMethods), value, true);
                    return;
                }

                catch (Exception)
                {
                    throw new FormatException(String.Format(Properties.Resources.InvalidHarvestMethod, value, row + 1, col + 1, fileName));
                }
            }

            if (propertyInfo.PropertyType == typeof(TillageType))
            {
                try
                {
                    if (_inputHelper.IsNotApplicableInput(value))
                    {
                        TillageType = TillageType.NotSelected;
                        return;
                    }

                    TillageType = (TillageType)Enum.Parse(typeof(TillageType), value);
                    return;
                }

                catch (Exception)
                {
                    throw new FormatException(String.Format(Properties.Resources.InvalidTillageType, value, row + 1, col + 1, fileName));
                }
            }

            else
                propertyInfo.SetValue(this, Convert.ChangeType(value, propertyInfo.PropertyType, CLILanguageConstants.culture), null);
        }

        // Look for the missing headers and apply the default values for the associated property
        public void FinalSettings(IComponentKeys componentKeys)
        {
            if (componentKeys.MissingHeaders.ContainsKey(Properties.Resources.Key_NitrogenFixation) && componentKeys.MissingHeaders[Properties.Resources.Key_NitrogenFixation])
            {
                var nitrogenFixationProvider = new NitogenFixationProvider();
                this.NitrogenFixation = nitrogenFixationProvider.GetNitrogenFixationResult(this.CropType).Fixation;
            }
            if (componentKeys.MissingHeaders.ContainsKey(Properties.Resources.Key_NitrogenDeposit) && componentKeys.MissingHeaders[Properties.Resources.Key_NitrogenDeposit])
            {
                var defaultDeposit = 5;
                this.NitrogenDeposit = defaultDeposit;
            }
        }
        #endregion

        //List of Properties. When adding a new property, follow the format: NewProperty. If you add a new property here
        //Make sure to add the appropriate key to the list of keys in the FieldKeys class.
        #region Properties
        public string Name { get; set; }
        //The Guid refers to the Perennial Stand Group GUID
        public Guid Guid { get; set; }
        public Guid PerennialStandID { get; set; }
        public Guid FieldSystemComponentGuid { get; set; }
        //RowId refers to the Perennial Stand Group ID
        public int GroupId { get; set; }
        public int PhaseNumber { get; set; }
        public int YearInPerennialStand { get; set; }
        public int PerennialStandLength { get; set; }
        public int CurrentYear { get; set; }
        public int CropYear { get; set; }
        public int NumberOfPesticidePasses { get; set; }
        public bool ManureApplied { get; set; }
        public bool CoverCropsUsed { get; set; }
        public bool UnderSownCropsUsed { get; set; }
        public bool CropIsGrazed { get; set; }
        public double Area { get; set; }
        public double Yield { get; set; }
        public double MoistureContentOfCrop { get; set; }
        public double AmountOfIrrigation { get; set; }
        public double AmountOfManureApplied { get; set; }
        public double NitrogenFertilizerRate { get; set; }
        public double PhosphorousFertilizerRate { get; set; }
        public double PercentageOfStrawReturnedToSoil { get; set; }
        public double PercentageOfRootsReturnedToSoil { get; set; }
        public double PercentageOfProductYieldReturnedToSoil { get; set; }
        public double BiomassCoefficientProduct { get; set; }
        public double BiomassCoefficientStraw { get; set; }
        public double BiomassCoefficientRoots { get; set; }
        public double BiomassCoefficientExtraroot { get; set; }
        public double NitrogenContentInProduct { get; set; }
        public double NitrogenContentInStraw { get; set; }
        public double NitrogenContentInRoots { get; set; }
        public double NitrogenContentInExtraroot { get; set; }
        public double NitrogenFixation { get; set; }
        public double NitrogenDeposit { get; set; }
        public double CarbonConcentration { get; set; }
        public Response IsIrrigated { get; set; }
        public Response IsPesticideUsed { get; set; }
        public ManureLocationSourceType ManureLocationSourceType { get; set; }
        public ManureStateType ManureStateType { get; set; }
        public ManureApplicationTypes ManureApplicationType { get; set; }
        public CoverCropTerminationType CoverCropTerminationType { get; set; }
        public CoverCropTypes CoverCropType { get; set; }
        public ManureAnimalSourceTypes ManureAnimalSourceType { get; set; }
        public IrrigationType IrrigationType { get; set; }
        public CropType CropType { get; set; }
        public HarvestMethods HarvestMethod { get; set; }
        public TillageType TillageType { get; set; }
        public string GroupName { get; set; }
        public AnimalType GroupType { get; set; }
        public string TimePeriodCategoryString { get; set; }
        public double ClimateParameter { get; set; }
        public double TillageFactor { get; set; }
        public double ManagementFactor { get; set; }
        public double PlantCarbonInAgriculturalProduct { get; set; }
        public double CarbonInputFromProduct { get; set; }
        public double CarbonInputFromStraw { get; set; }
        public double CarbonInputFromRoots { get; set; }
        public double CarbonInputFromExtraroots { get; set; }
        public int SizeOfFirstRotationForField { get; set; }
        public double AboveGroundCarbonInput { get; set; }
        public double BelowGroundCarbonInput { get; set; }

        #endregion
    }
}
