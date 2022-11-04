using System;
using H.Core.Enumerations;
using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models
{
    public class DisplayUnitStrings : ModelBase
    {
        #region Fields

        private string _fertilizerKilogramsPerHectareString;
        private string _kilogramsPerDayString;
        private string _kilogramsPerHectareString;
        private string _hectaresString;
        private string _kilogramsString;
        private string _dollarsPerKilogramString;
        private string _dollars;
        private string _dollarsPerHectare;
        private string _millimetersString;
        private string _millimetersPerHectareString;
        private string _milliEquivalentsPer100g;
        private string _millimetersPerYearString;
        private string _percentageString;
        private string _degreesCelsiusString;
        private string _percentageDryMatterString;
        private string _oneTimesPercentageString;
        private string _percentageCrudeProteinString;
        private string _percentageAfString;
        private string _mCalKgString;
        private string _kCalKgString;
        private string _kilogramPerKilogramString;
        private string _kilogramsPerHeadPerYearString;
        private string _kilogramsPerHeadPerDayString;
        private string _kilogramsPerKilogramProteinIntakeString;
        private string _kilogramsN2OnPerKilogramNString;
        private string _kilogramsN20NPerYearString;
        private string _kilogramsN20PerYearString;
        private string _kilogramsMethanePerHeadPerDayString;
        private string _kilogramsN20NString;
        private string _kilogramsN20String;
        private string _kilogramsN2OPerHectareString;
        private string _kilogramsN2OPerFieldString;
        private string _kilogramsN2ONPerFieldString;
        private string _kilogramsNONPerFieldString;
        private string _kilogramsNONPerHectareString;
        private string _kilogramsNO3NPerHectareString;
        private string _kilogramsNH4NPerHectareString;
        private string _kilogramsN2NPerHectareString;
        private string _daysString;
        private string _cO2Equivalents;
        private string _megagramsCo2E;
        private string _kilogramsGhgs;
        private string _megagramsGhgs;
        private string _kilogramsNitrogenString;
        private string _kilogramsNitrogenPerYearString;
        private string _kilogramsNitrogenPerMonthPerAnimalString;
        private string _kilogramsNH3NString;
        private string _kilogramsNH3NPerKilogramNitrogenString;
        private string _kilogramsNPerKilogramNitrogenString;
        private string _kilogramsNH3String;
        private string _kilogramsCarbonString;
        private string _kilogramsCarbonPerYearString;
        private string _kilogramsNitrogenPerHectareString;
        private string _kilogramsNitrogenPerHectarePerYearString;
        private string _kilogramsPhosphorusPerHectareString;
        private string _kilogramsPotassiumPerHectareString;
        private string _kilogramsSulphurPerHectareString;
        private string _cubicMetersMethanePerKilogramVolatileSolidsString;
        private string _kilogramsMethaneString;
        private string _kilogramsNitrousOxideString;
        private string _kilogramsMethanePerKilogramsMethaneString;
        private string _kilogramsMethanePerYearString;
        private string _megaJoulesPerDayPerKilogramString;
        private string _megaJoulesPerKilogramSquaredString;
        private string _kilogramsNitrogenPerKilogramString;
        private string _gigajoulesPerHectareString;
        private string _kilogramsPerYearString;
        private string _monthsString;
        private string _yearsString;
        private string _kilogramsCarbonPerHectareString;
        private string _kilogramsCarbonPerTreeString;
        private string _kilogramsCarbonPerPlantingString;
        private string _kilogramsCarbonDioxidePerShelterbeltString;
        private string _kilogramsVolatileSolidsPerKilogramFeedString;
        private string _megaJoulesPerKilogramString;
        private string _gramsPerSquaredMeterString;
        private string _partsPerMillionString;
        private string _microgramsPerKilogramString;
        private string _percentageEtherExtractString;
        private string _kilogramsTanString;
        private string _kilogramsTanPerAnimalPerMonthString;
        private string _wetWeight;
        private string _percentageWetWeight;
        private string _percentageHString;
        private string _percentageNdfString;
        private string _milligramsPerKilogramString;
        private string _internationalUnitsPerGramString;
        private string _kilogramsCO2PerKilogWattHour;
        private string _kilogramsCO2PerGJ;
        private string _kilogramsCO2PerKilogramN;
        private string _kilogramsCO2PerKilogramP2O5;
        private string _kilogramsCO2PerHectare;
        private string _kilowattHourPerAnimal;
        private string _kilowattHourPerPoultryPlacement;
        private string _gigajoulesPer1000Litres;
        private string _kilogramsNPer1000Litres;
        private string _kilogramsDryMatterPerHectareString;
        private string _kilogramsNitrogenPerKilogramDryMatter;
        private string _kilogramsCarbonPerKilogramDryMatter;
        private string _kilogramsPhosphorusPerKilogramDryMatter;
        private string _bushelString;
        private string _kilogramsPerGroupPerTimePeriodString;
        private string _kilogramsCarbonPerTreeTypeString;
        private string _fractionString;

        #endregion

        #region Constructors

        public DisplayUnitStrings()
        {
        }

        #endregion

        #region Properties

        private string _kilogramsCO2;

        public string KilogramsCO2
        {
            get => _kilogramsCO2;
            set => SetProperty(ref _kilogramsCO2, value);
        }

        public string KilogramsPerDayString
        {
            get { return _kilogramsPerDayString; }
            set { SetProperty(ref _kilogramsPerDayString, value); }
        }

        public string KilogramsPerHectareString
        {
            get { return _kilogramsPerHectareString; }
            set { SetProperty(ref _kilogramsPerHectareString, value); }
        }

        public string FertilizerKilogramsPerHectareString
        {
            get => _fertilizerKilogramsPerHectareString;
            set => SetProperty(ref _fertilizerKilogramsPerHectareString, value);
        }

        public string HectaresString
        {
            get { return _hectaresString; }
            set { SetProperty(ref _hectaresString, value); }
        }

        public string MillimetersString
        {
            get { return _millimetersString; }
            set { SetProperty(ref _millimetersString, value); }
        }

        public string KilogramsString
        {
            get { return _kilogramsString; }
            set { SetProperty(ref _kilogramsString, value); }
        }

        public string MillimetersPerYearString
        {
            get { return _millimetersPerYearString; }
            set { SetProperty(ref _millimetersPerYearString, value); }
        }

        public string PercentageString
        {
            get { return _percentageString; }
            set { SetProperty(ref _percentageString, value); }
        }

        public string DegreesCelsiusString
        {
            get { return _degreesCelsiusString; }
            set { SetProperty(ref _degreesCelsiusString, value); }
        }

        public string PercentageDryMatterString
        {
            get { return _percentageDryMatterString; }
            set { SetProperty(ref _percentageDryMatterString, value); }
        }

        public string OneTimesPercentageString
        {
            get { return _oneTimesPercentageString; }
            set { SetProperty(ref _oneTimesPercentageString, value); }
        }

        public string PercentageCrudeProteinString
        {
            get { return _percentageCrudeProteinString; }
            set { SetProperty(ref _percentageCrudeProteinString, value); }
        }

        public string PercentageAFString
        {
            get { return _percentageAfString; }
            set { SetProperty(ref _percentageAfString, value); }
        }

        public string McalKgString
        {
            get { return _mCalKgString; }
            set { SetProperty(ref _mCalKgString, value); }
        }

        public string KilogramPerKilogramString
        {
            get { return _kilogramPerKilogramString; }
            set { SetProperty(ref _kilogramPerKilogramString, value); }
        }

        public string KilogramPerHeadPerYearString
        {
            get { return _kilogramsPerHeadPerYearString; }
            set { SetProperty(ref _kilogramsPerHeadPerYearString, value); }
        }

        public string KilogramPerHeadPerDayString
        {
            get { return _kilogramsPerHeadPerDayString; }
            set { SetProperty(ref _kilogramsPerHeadPerDayString, value); }
        }

        public string KilogramsPerKilogramProteinIntakeString
        {
            get { return _kilogramsPerKilogramProteinIntakeString; }
            set { SetProperty(ref _kilogramsPerKilogramProteinIntakeString, value); }
        }

        public string KilogramsN2ONPerKilogramNString
        {
            get { return _kilogramsN2OnPerKilogramNString; }
            set { SetProperty(ref _kilogramsN2OnPerKilogramNString, value); }
        }

        public string KilogramsN20NPerYearString
        {
            get { return _kilogramsN20NPerYearString; }
            set { SetProperty(ref _kilogramsN20NPerYearString, value); }
        }

        public string KilogramsN20PerYearString
        {
            get { return _kilogramsN20PerYearString; }
            set { SetProperty(ref _kilogramsN20PerYearString, value); }
        }

        public string KilogramsN20NString
        {
            get { return _kilogramsN20NString; }
            set { SetProperty(ref _kilogramsN20NString, value); }
        }

        public string KilogramsN2OPerHectareString
        {
            get { return _kilogramsN2OPerHectareString; }
            set { SetProperty(ref _kilogramsN2OPerHectareString, value); }
        }

        public string KilogramsNONPerHectareString
        {
            get { return _kilogramsNONPerHectareString; }
            set { SetProperty(ref _kilogramsNONPerHectareString, value); }
        }

        public string KilogramsNO3NPerHectareString
        {
            get { return _kilogramsNO3NPerHectareString; }
            set { SetProperty(ref _kilogramsNO3NPerHectareString, value); }
        }

        public string KilogramsNH4NPerHectareString
        {
            get { return _kilogramsNH4NPerHectareString; }
            set { SetProperty(ref _kilogramsNH4NPerHectareString, value); }
        }

        public string KilogramsN2NPerHectareString
        {
            get { return _kilogramsN2NPerHectareString; }
            set { SetProperty(ref _kilogramsN2NPerHectareString, value); }
        }

        public string DaysString
        {
            get { return _daysString; }
            set { SetProperty(ref _daysString, value); }
        }

        public string KilogramsCO2Equivalents
        {
            get { return _cO2Equivalents; }
            set { SetProperty(ref _cO2Equivalents, value); }
        }

        public string MegagramsCO2e
        {
            get { return _megagramsCo2E; }
            set { SetProperty(ref _megagramsCo2E, value); }

        }

        public string KilogramsGhgs
        {
            get { return _kilogramsGhgs; }
            set { SetProperty(ref _kilogramsGhgs, value); }
        }

        public string KilogramsNitrogenString
        {
            get { return _kilogramsNitrogenString; }
            set { SetProperty(ref _kilogramsNitrogenString, value); }
        }

        public string KilogramsNitrogenPerYearString
        {
            get { return _kilogramsNitrogenPerYearString; }
            set { SetProperty(ref _kilogramsNitrogenPerYearString, value); }
        }

        public string KilogramsNitrogenPerMonthPerAnimalString
        {
            get { return _kilogramsNitrogenPerMonthPerAnimalString; }
            set { SetProperty(ref _kilogramsNitrogenPerMonthPerAnimalString, value); }
        }

        public string KilogramsNH3NString
        {
            get { return _kilogramsNH3NString; }
            set { SetProperty(ref _kilogramsNH3NString, value); }
        }

        public string KilogramsNH3String
        {
            get { return _kilogramsNH3String; }
            set { SetProperty(ref _kilogramsNH3String, value); }
        }

        public string KilogramsCarbonString
        {
            get { return _kilogramsCarbonString; }
            set { SetProperty(ref _kilogramsCarbonString, value); }
        }

        public string KilogramsCarbonPerYearString
        {
            get { return _kilogramsCarbonPerYearString; }
            set { SetProperty(ref _kilogramsCarbonPerYearString, value); }
        }

        public string KilogramsNitrogenPerHectareString
        {
            get { return _kilogramsNitrogenPerHectareString; }
            set { SetProperty(ref _kilogramsNitrogenPerHectareString, value); }
        }

        public string KilogramsNitrogenPerHectarePerYearString
        {
            get { return _kilogramsNitrogenPerHectarePerYearString; }
            set { SetProperty(ref _kilogramsNitrogenPerHectarePerYearString, value); }
        }

        public string KilogramsPhosphorusPerHectareString
        {
            get { return _kilogramsPhosphorusPerHectareString; }
            set { SetProperty(ref _kilogramsPhosphorusPerHectareString, value); }
        }

        public string CubicMetersMethanePerKilogramVolatileSolidsString
        {
            get { return _cubicMetersMethanePerKilogramVolatileSolidsString; }
            set { SetProperty(ref _cubicMetersMethanePerKilogramVolatileSolidsString, value); }
        }

        public string KilogramsMethaneString
        {
            get { return _kilogramsMethaneString; }
            set { SetProperty(ref _kilogramsMethaneString, value); }
        }

        public string KilogramsNitrousOxideString
        {
            get { return _kilogramsNitrousOxideString; }
            set { SetProperty(ref _kilogramsNitrousOxideString, value); }
        }

        public string KilogramsMethanePerKilogramsMethaneString
        {
            get { return _kilogramsMethanePerKilogramsMethaneString; }
            set { SetProperty(ref _kilogramsMethanePerKilogramsMethaneString, value); }
        }

        public string KilogramsMethanePerYearString
        {
            get { return _kilogramsMethanePerYearString; }
            set { SetProperty(ref _kilogramsMethanePerYearString, value); }
        }

        public string MegaJoulesPerDayPerKilogramString
        {
            get { return _megaJoulesPerDayPerKilogramString; }
            set { SetProperty(ref _megaJoulesPerDayPerKilogramString, value); }
        }

        public string MegaJoulesPerKilogramSquaredString
        {
            get { return _megaJoulesPerKilogramSquaredString; }
            set { SetProperty(ref _megaJoulesPerKilogramSquaredString, value); }
        }

        public string KilogramsNitrogenPerKilogramString
        {
            get { return _kilogramsNitrogenPerKilogramString; }
            set { SetProperty(ref _kilogramsNitrogenPerKilogramString, value); }
        }

        public string GigajoulesPerHectareString
        {
            get { return _gigajoulesPerHectareString; }
            set { SetProperty(ref _gigajoulesPerHectareString, value); }
        }

        public string KilogramsPerYearString
        {
            get { return _kilogramsPerYearString; }
            set { SetProperty(ref _kilogramsPerYearString, value); }
        }

        public string MonthsString
        {
            get { return _monthsString; }
            set { SetProperty(ref _monthsString, value); }
        }

        public string YearsString
        {
            get { return _yearsString; }
            set { SetProperty(ref _yearsString, value); }
        }

        public string KilogramsCarbonPerHectareString
        {
            get { return _kilogramsCarbonPerHectareString; }
            set { SetProperty(ref _kilogramsCarbonPerHectareString, value); }
        }

        public string KilogramsCarbonPerTreeString
        {
            get { return _kilogramsCarbonPerTreeString; }
            set { SetProperty(ref _kilogramsCarbonPerTreeString, value); }
        }

        public string KilogramsCarbonPerPlantingString
        {
            get { return _kilogramsCarbonPerPlantingString; }
            set { SetProperty(ref _kilogramsCarbonPerPlantingString, value); }
        }

        public string KilogramsCarbonDioxidePerShelterbeltString
        {
            get { return _kilogramsCarbonDioxidePerShelterbeltString; }
            set { SetProperty(ref _kilogramsCarbonDioxidePerShelterbeltString, value); }
        }

        public string KilogramsVolatileSolidsPerKilogramFeedString
        {
            get { return _kilogramsVolatileSolidsPerKilogramFeedString; }
            set { SetProperty(ref _kilogramsVolatileSolidsPerKilogramFeedString, value); }
        }

        public string MegaJoulesPerKilogramString
        {
            get { return _megaJoulesPerKilogramString; }
            set { SetProperty(ref _megaJoulesPerKilogramString, value); }
        }

        public string GramsPerSquaredMeterString
        {
            get { return _gramsPerSquaredMeterString; }
            set { SetProperty(ref _gramsPerSquaredMeterString, value); }
        }

        public string PartsPerMillionString
        {
            get { return _partsPerMillionString; }
            set { SetProperty(ref _partsPerMillionString, value); }
        }

        public string MicrogramsPerKilogramString
        {
            get { return _microgramsPerKilogramString; }
            set { SetProperty(ref _microgramsPerKilogramString, value); }
        }

        public string PercentageEtherExtractString
        {
            get { return _percentageEtherExtractString; }
            set { SetProperty(ref _percentageEtherExtractString, value); }
        }

        public string KilogramsTanString
        {
            get { return _kilogramsTanString; }
            set { SetProperty(ref _kilogramsTanString, value); }
        }

        public string KilogramsTanPerAnimalPerMonthString
        {
            get { return _kilogramsTanPerAnimalPerMonthString; }
            set { SetProperty(ref _kilogramsTanPerAnimalPerMonthString, value); }
        }

        public string WetWeight
        {
            get { return _wetWeight; }
            set { SetProperty(ref _wetWeight, value); }
        }

        public string PercentageHString
        {
            get { return _percentageHString; }
            set { SetProperty(ref _percentageHString, value); }
        }

        public string PercentageNdfString
        {
            get { return _percentageNdfString; }
            set { SetProperty(ref _percentageNdfString, value); }
        }

        public string MilligramsPerKilogramString
        {
            get { return _milligramsPerKilogramString; }
            set { SetProperty(ref _milligramsPerKilogramString, value); }
        }

        public string InternationalUnitsPerGramString
        {
            get { return _internationalUnitsPerGramString; }
            set { SetProperty(ref _internationalUnitsPerGramString, value); }
        }

        public string MegagramsGhgs
        {
            get { return _megagramsGhgs; }
            set { SetProperty(ref _megagramsGhgs, value); }
        }

        public string KilogramsCo2PerKilogWattHour
        {
            get { return _kilogramsCO2PerKilogWattHour; }
            set { SetProperty(ref _kilogramsCO2PerKilogWattHour, value); }
        }

        public string KilogramsCo2PerGj
        {
            get { return _kilogramsCO2PerGJ; }
            set { SetProperty(ref _kilogramsCO2PerGJ, value); }
        }

        public string KilogramsCo2PerKilogramN
        {
            get { return _kilogramsCO2PerKilogramN; }
            set { SetProperty(ref _kilogramsCO2PerKilogramN, value); }
        }

        public string KilogramsCo2PerKilogramP2O5
        {
            get { return _kilogramsCO2PerKilogramP2O5; }
            set { SetProperty(ref _kilogramsCO2PerKilogramP2O5, value); }
        }

        public string KilogramsCo2PerHectare
        {
            get { return _kilogramsCO2PerHectare; }
            set { SetProperty(ref _kilogramsCO2PerHectare, value); }
        }

        public string KilowattHourPerAnimal
        {
            get { return _kilowattHourPerAnimal; }
            set { SetProperty(ref _kilowattHourPerAnimal, value); }
        }

        public string KilowattHourPerPoultryPlacement
        {
            get { return _kilowattHourPerPoultryPlacement; }
            set { SetProperty(ref _kilowattHourPerPoultryPlacement, value); }
        }

        public string GigajoulesPer1000Litres
        {
            get { return _gigajoulesPer1000Litres; }
            set { SetProperty(ref _gigajoulesPer1000Litres, value); }
        }

        public string KilogramsNPer1000Litres
        {
            get { return _kilogramsNPer1000Litres; }
            set { SetProperty(ref _kilogramsNPer1000Litres, value); }
        }

        public string KilogramsNitrogenPerKilogramDryMatter
        {
            get { return _kilogramsNitrogenPerKilogramDryMatter; }
            set { SetProperty(ref _kilogramsNitrogenPerKilogramDryMatter, value); }
        }

        public string KilogramsCarbonPerKilogramDryMatter
        {
            get { return _kilogramsCarbonPerKilogramDryMatter; }
            set { SetProperty(ref _kilogramsCarbonPerKilogramDryMatter, value); }
        }

        public string KilogramsPhosphorusPerKilogramDryMatter
        {
            get { return _kilogramsPhosphorusPerKilogramDryMatter; }
            set { SetProperty(ref _kilogramsPhosphorusPerKilogramDryMatter, value); }
        }

        public string PercentageWetWeight
        {
            get { return _percentageWetWeight; }
            set { SetProperty(ref _percentageWetWeight, value); }
        }

        public string KCalKgString
        {
            get => _kCalKgString;
            set => SetProperty(ref _kCalKgString, value);
        }

        public string DollarsPerKilogramString
        {
            get => _dollarsPerKilogramString;
            set => SetProperty(ref _dollarsPerKilogramString, value);
        }

        public string Dollars
        {
            get => _dollars;
            set => SetProperty(ref _dollars, value);
        }

        public string DollarsPerHectare
        {
            get => _dollarsPerHectare;
            set => SetProperty(ref _dollarsPerHectare, value);
        }

        public string BushelString
        {
            get => _bushelString;
            set => SetProperty(ref _bushelString, value);
        }

        public string MilliEquivalentsPer100G
        {
            get => _milliEquivalentsPer100g;
            set => SetProperty(ref _milliEquivalentsPer100g, value);
        }

        public string KilogramsPotassiumPerHectareString
        {
            get => _kilogramsPotassiumPerHectareString;
            set => SetProperty(ref _kilogramsPotassiumPerHectareString, value);
        }

        public string KilogramsSulphurPerHectareString
        {
            get => _kilogramsSulphurPerHectareString;
            set => SetProperty(ref _kilogramsSulphurPerHectareString, value);
        }

        public string KilogramsDryMatterPerHectareString
        {
            get => _kilogramsDryMatterPerHectareString;
            set => SetProperty(ref _kilogramsDryMatterPerHectareString, value);
        }

        public string KilogramsNh3NPerKilogramNitrogenString
        {
            get => _kilogramsNH3NPerKilogramNitrogenString;
            set => SetProperty(ref _kilogramsNH3NPerKilogramNitrogenString, value);
        }

        public string KilogramsMethanePerHeadPerDayString
        {
            get => _kilogramsMethanePerHeadPerDayString;
            set => SetProperty(ref _kilogramsMethanePerHeadPerDayString, value);
        }
        public string KilogramsPerGroupPerTimePeriodString 
        { 
            get => _kilogramsPerGroupPerTimePeriodString; 
            set => SetProperty(ref _kilogramsPerGroupPerTimePeriodString, value);
        }

        public string KilogramsCarbonPerTreeTypeString 
        { 
            get => _kilogramsCarbonPerTreeTypeString; 
            set => SetProperty(ref _kilogramsCarbonPerTreeTypeString, value); 
        }

        public string FractionString
        {
            get => _fractionString;
            set => SetProperty(ref _fractionString, value);
        }

        public string KilogramsNPerKilogramNitrogenString
        {
            get => _kilogramsNPerKilogramNitrogenString;
            set => SetProperty(ref _kilogramsNPerKilogramNitrogenString, value);
        }

        public string KilogramsN20String
        {
            get => _kilogramsN20String;
            set => SetProperty(ref _kilogramsN20String, value);
        }

        public string KilogramsN2OPerFieldString
        {
            get => _kilogramsN2OPerFieldString;
            set => SetProperty(ref _kilogramsN2OPerFieldString, value);
        }

        public string KilogramsN2OnPerFieldString
        {
            get => _kilogramsN2ONPerFieldString;
            set => SetProperty(ref _kilogramsN2ONPerFieldString, value);
        }

        public string KilogramsNONPerFieldString
        {
            get => _kilogramsNONPerFieldString;
            set => SetProperty(ref _kilogramsNONPerFieldString, value);
        }

        public string MillimetersPerHectareString
        {
            get => _millimetersPerHectareString;
            set => SetProperty(ref _millimetersPerHectareString, value);
        }

        #endregion

        #region Public Methods

        public string WrapString(string input)
        {
            return "(" + input + ")";
        }

        /// <summary>
        /// Sets the units of measurement strings for display in both the CLI and GUI. This call should be only made when using the XAML designer
        /// or once when the CLI initializes a new farm and/or once when the GUI initializes a new farm (or opens an existing farm).
        /// </summary>
        /// <param name="measurementSystemType"></param>
        public void SetStrings(MeasurementSystemType measurementSystemType)
        {
            if (measurementSystemType == MeasurementSystemType.Metric)
            {
                MegagramsGhgs = WrapString(Resources.MegagramsGhgs);
                KilogramsPerHectareString = WrapString(Resources.KilogramsPerHectare);
                HectaresString = WrapString(Resources.Hectares);
                MillimetersString = WrapString(Resources.Millimeters);
                MillimetersPerYearString = WrapString(Resources.MillimetersPerYear);
                PercentageString = WrapString("%");
                DegreesCelsiusString = WrapString(Resources.DegreesCelsius);
                PercentageDryMatterString = WrapString(Resources.PercentageDryMatter);
                KilogramsDryMatterPerHectareString = WrapString(Resources.KilogramsDryMatterPerHectareString);
                PercentageCrudeProteinString = WrapString(Resources.PercentageCrudeProtein);
                PercentageAFString = WrapString(Resources.PercentageAF);
                McalKgString = WrapString(Resources.MegaCaloriesPerKilogram);
                PercentageHString = WrapString(Resources.PercentageH);
                PercentageNdfString = WrapString(Resources.PercentageNdf);
                MilligramsPerKilogramString = WrapString(Resources.MilligramsPerKilogram);
                InternationalUnitsPerGramString = WrapString(Resources.InternationalUnitsPerGram);
                KilogramsString = WrapString(Resources.Kilograms);
                KilogramPerKilogramString = WrapString(Resources.KilogramPerKilogram);
                DaysString = WrapString(Resources.Days);
                KilogramPerHeadPerYearString = WrapString(Resources.KilogramPerHeadPerYear);
                KilogramPerHeadPerDayString = WrapString(Resources.KilogramPerHeadPerDay);
                KilogramsN2ONPerKilogramNString = WrapString(Resources.KilogramsN2ONPerKilogramN);
                KilogramsN2OPerHectareString = WrapString(Resources.KilogramsN2OPerHectareString);
                KilogramsNONPerHectareString = WrapString(Resources.KilogramsNONPerHectareString);
                KilogramsNO3NPerHectareString = WrapString(Resources.KilogramsNO3NPerHectareString);
                KilogramsNH4NPerHectareString = WrapString(Resources.KilogramsNH4NPerHectareString);
                KilogramsN2NPerHectareString = WrapString(Resources.KilogramsN2NPerHectareString);
                KilogramsCO2Equivalents = WrapString(Resources.KilogramsCO2Equivalents);
                MegagramsCO2e = WrapString(Resources.EnumMgC02e);
                KilogramsGhgs = WrapString(Resources.EnumKgGhg);
                KilogramsNitrogenPerHectareString = WrapString(Resources.KilogramsNitrogenPerHectare);
                KilogramsNitrogenPerHectarePerYearString = WrapString(Resources.KilogramsNitrogenPerHectarePerYear);
                KilogramsPotassiumPerHectareString = WrapString(Resources.KilogramsPotassiumPerHectare);
                KilogramsSulphurPerHectareString = WrapString(Resources.KilogramsSulphurPerHectare);
                KilogramsPhosphorusPerHectareString = WrapString(Resources.KilogramsPhosphorousPerHectare);
                CubicMetersMethanePerKilogramVolatileSolidsString = WrapString(Resources.CubicMetersMethanePerKilogramVolatileSolids);
                KilogramsMethanePerKilogramsMethaneString = WrapString(Resources.KilogramsMethanePerKilogramMethane);
                MegaJoulesPerDayPerKilogramString = WrapString(Resources.MegaJoulesPerDayPerKilogram);
                KilogramsNitrogenPerKilogramString = WrapString(Resources.KilogramsNitrogenPerKilogram);
                GigajoulesPerHectareString = WrapString(Resources.GigajoulesPerHectare);
                KilogramsPerDayString = WrapString(Resources.KilogramsPerDay);
                MonthsString = WrapString(Resources.Months);
                YearsString = WrapString(Resources.Years);
                KilogramsCarbonPerHectareString = WrapString(Resources.KilogramsCarbonPerHectare);
                KilogramsCarbonPerTreeString = WrapString(Resources.KilogramsCarbonPerTree);
                KilogramsCarbonPerPlantingString = WrapString(Resources.KilogramsCarbonPerPlanting);
                KilogramsCarbonDioxidePerShelterbeltString = WrapString(Resources.KilogramsCarbonDioxidePerShelterbelt);
                KilogramsMethanePerYearString = WrapString(Resources.KilogramsMethanePerYear);
                KilogramsMethaneString = WrapString(Resources.KilogramsMethane);
                KilogramsN20NPerYearString = WrapString(Resources.KilogramsN2ONPerYear);
                KilogramsN20PerYearString = WrapString(Resources.KilogramsN2OPerYear);
                KilogramsN20NString = WrapString(Resources.KilogramsN2ON);
                KilogramsNitrousOxideString = WrapString(Resources.KilogramsNitrousOxideString);
                KilogramsNitrogenString = WrapString(Resources.KilogramsNitrogen);
                KilogramsNitrogenPerYearString = WrapString(Resources.LabelKilogramsNitrogenPerYear);
                KilogramsNitrogenPerMonthPerAnimalString = WrapString(Resources.LabelKilogramsNitrogenPerHeadPerMonth);
                KilogramsNH3NString = WrapString(Resources.LabelKilogramsNH3N);
                KilogramsNH3String = WrapString(Resources.LabelKilogramsNH3);
                KilogramsCarbonString = WrapString(Resources.LabelKilogramsCarbon);
                KilogramsCarbonPerYearString = WrapString(Resources.LabelKilogramsCarbonPerYear);
                KilogramsPerYearString = WrapString(Resources.KilogramsPerYear);
                MegaJoulesPerKilogramSquaredString = WrapString(Resources.MegaJoulesPerKilogramSquared);
                KilogramsPerKilogramProteinIntakeString = WrapString(Resources.KilogramsPerKilogramProteinIntake);
                KilogramsVolatileSolidsPerKilogramFeedString = WrapString(Resources.KilogramsVolatileSolidsPerKilogramFeed);
                MegaJoulesPerKilogramString = WrapString(Resources.MegaJoulesPerKilogram);
                OneTimesPercentageString = WrapString("1x, %");
                GramsPerSquaredMeterString = WrapString(Resources.LabelGramsPerMeterSquared);
                PartsPerMillionString = WrapString(Resources.PartsPerMillion);
                MicrogramsPerKilogramString = WrapString(Resources.MicrogramsPerKilogram);
                PercentageEtherExtractString = WrapString(Resources.PercentageEE);
                KilogramsTanString = WrapString(Resources.KilogramsTan);
                KilogramsTanPerAnimalPerMonthString = WrapString(Resources.KilogramsTanPerAnimalPerMonth);
                WetWeight = WrapString(Resources.WetWeight);
                KilogramsCo2PerKilogWattHour = WrapString(Resources.KilogramCO2PerKiloWattHour);
                KilogramsCo2PerGj = WrapString(Resources.KilogramCO2PerGJ);
                KilogramsCo2PerKilogramN = WrapString(Resources.KilogramCO2PerKilogramsN);
                KilogramsCo2PerKilogramP2O5 = WrapString(Resources.KilogramCO2PerKilogramsP2O5);
                KilogramsCo2PerHectare = WrapString(Resources.KilogramCO2PerHectare);
                KilowattHourPerAnimal = WrapString(Resources.KilowattPerAnimal);
                KilowattHourPerPoultryPlacement = WrapString(Resources.KilowattPerPoultryPlacement);
                GigajoulesPer1000Litres = WrapString(Resources.GigaJoulesPer1000Litres);
                KilogramsNPer1000Litres = WrapString(Resources.KilogramsNPer1000Litres);
                KilogramsNitrogenPerKilogramDryMatter = WrapString(Resources.KilogramsNitrogenPerKilogramDryMatter);
                KilogramsCarbonPerKilogramDryMatter = WrapString(Resources.KilogramsCarbonPerKilogramDryMatter);
                KilogramsPhosphorusPerKilogramDryMatter = WrapString(Resources.KilogramsPhosphorusPerKilogramDryMatter);
                PercentageWetWeight = WrapString(Resources.PercentageWetWeight);
                KCalKgString = WrapString(Resources.KiloCaloriesPerKilogram);
                DollarsPerKilogramString = WrapString(Resources.DollarsPerKilogram);
                Dollars = WrapString(Resources.Dollars);
                DollarsPerHectare = WrapString(Resources.DollarsPerHectare);
                MilliEquivalentsPer100G = WrapString(Resources.MilliEquivalentsPer100g);
                KilogramsCO2 = WrapString(Resources.EnumKgC02);
                FertilizerKilogramsPerHectareString = WrapString(Resources.KilogramsPerHectare);
                KilogramsNh3NPerKilogramNitrogenString = WrapString(Resources.KilogramsNH3NPerKilogramN);
                KilogramsMethanePerHeadPerDayString = WrapString(Resources.KilogramsMethanePerHeadPerDay);
                KilogramsPerGroupPerTimePeriodString = WrapString(Resources.KilogramsPerGroupPerTimePeriod);
                KilogramsCarbonPerTreeTypeString = WrapString(Resources.KilogramsCarbonPerTreeType);
                FractionString = WrapString(Resources.FractionString);
                KilogramsNPerKilogramNitrogenString = WrapString(Resources.KilogramsNPerKilogramNitrogenString);
                KilogramsN20String = WrapString(Resources.KilogramsN2O);
                KilogramsN2OPerFieldString = WrapString(Resources.KilogramsN2OPerField);
                KilogramsN2OnPerFieldString = WrapString(Resources.KilogramsN2ONPerField);
                KilogramsN2OnPerFieldString = WrapString(Resources.KilogramsNONPerField);
                MillimetersPerHectareString = WrapString(Resources.MillimetersPerHectare);
            }
            else
            {
                KilogramsGhgs = WrapString(Resources.EnumLbsGhg);
                MegagramsGhgs = WrapString(Resources.EnumLbsGhg);
                MegagramsCO2e = WrapString(Resources.PoundsCO2Equivalents);
                KilogramsPerHectareString = WrapString(Resources.BushelsPerAcre);
                HectaresString = WrapString(Resources.Acres);
                MillimetersString = WrapString(Resources.Inches);
                MillimetersPerYearString = WrapString(Resources.InchesPerYear);
                PercentageString = WrapString("%");
                DegreesCelsiusString = WrapString(Resources.DegreesFahrenheit);
                PercentageDryMatterString = WrapString(Resources.PercentageDryMatter);
                KilogramsDryMatterPerHectareString = WrapString(Resources.PoundsDryMatterPerAcreString);
                PercentageCrudeProteinString = WrapString(Resources.PercentageCrudeProtein);
                PercentageAFString = WrapString(Resources.PercentageAF);
                McalKgString = WrapString(Resources.BritishThermalUnitPerPound);
                PercentageHString = WrapString(Resources.PercentageH);
                PercentageNdfString = WrapString(Resources.PercentageNdf);
                MilligramsPerKilogramString = WrapString(Resources.GrainsPerPound);
                InternationalUnitsPerGramString = WrapString(Resources.InternationalUnitsPerGrain);
                KilogramsString = WrapString(Resources.Pounds);
                KilogramPerKilogramString = WrapString(Resources.PoundsPerPound);
                DaysString = WrapString(Resources.Days);
                KilogramPerHeadPerYearString = WrapString(Resources.PoundPerHeadPerYear);
                KilogramPerHeadPerDayString = WrapString(Resources.PoundPerHeadPerDay);
                KilogramsN2ONPerKilogramNString = WrapString(Resources.PoundsN2ONPerPoundN);
                KilogramsN2OPerHectareString = WrapString(Resources.PoundsN2OPerAcreString);
                KilogramsNONPerHectareString = WrapString(Resources.PoundsNONPerAcreString);
                KilogramsNO3NPerHectareString = WrapString(Resources.PoundsNO3NPerAcreString);
                KilogramsNH4NPerHectareString = WrapString(Resources.PoundsNH4NPerAcreString);
                KilogramsN2NPerHectareString = WrapString(Resources.PoundsN2NPerAcreString);
                KilogramsCO2Equivalents = WrapString(Resources.KilogramsCO2Equivalents);
                KilogramsNitrogenPerHectareString = WrapString(Resources.PoundsNitrogenPerAcre);
                KilogramsNitrogenPerHectarePerYearString = WrapString(Resources.PoundsNitrogenPerAcrePerYear);
                KilogramsPhosphorusPerHectareString = WrapString(Resources.PoundsPhosphorousPerAcre);
                KilogramsPotassiumPerHectareString = WrapString(Resources.PoundsPotassiumPerAcre);
                KilogramsSulphurPerHectareString = WrapString(Resources.PoundsSulphurPerAcre);
                CubicMetersMethanePerKilogramVolatileSolidsString = WrapString(Resources.CubicYardsMethanePerPoundVolatileSolids);
                KilogramsMethanePerKilogramsMethaneString = WrapString(Resources.PoundsMethanePerPoundMethane);
                MegaJoulesPerDayPerKilogramString = WrapString(Resources.BritishThermalUnitPerDayPerPound);
                KilogramsNitrogenPerKilogramString = WrapString(Resources.PoundsNitrogenPerPound);
                //? not sure about calories to joules
                GigajoulesPerHectareString = WrapString(Resources.BritishThermalUnitsPerAcre);
                KilogramsPerDayString = WrapString(Resources.PoundsPerDay);
                MonthsString = WrapString(Resources.Months);
                YearsString = WrapString(Resources.Years);
                KilogramsCarbonPerHectareString = WrapString(Resources.PoundsCarbonPerAcre);
                KilogramsCarbonPerTreeString = WrapString(Resources.PoundsCarbonPerTree);
                KilogramsCarbonPerPlantingString = WrapString(Resources.PoundsCarbonPerPlanting);
                KilogramsCarbonDioxidePerShelterbeltString = WrapString(Resources.PoundsCarbonDioxidePerShelterbelt);
                KilogramsMethanePerYearString = WrapString(Resources.PoundsMethanePerYear);
                KilogramsMethaneString = WrapString(Resources.PoundsMethane);
                KilogramsN20NPerYearString = WrapString(Resources.PoundsN2ONPerYear);
                KilogramsN20PerYearString = WrapString(Resources.PoundsN2OPerYear);
                KilogramsN20NString = WrapString(Resources.PoundsN2ONPerPoundN);
                KilogramsNitrousOxideString = WrapString(Resources.EnumLbsN2O);
                KilogramsNitrogenString = WrapString(Resources.PoundsNitrogen);
                KilogramsNitrogenPerYearString = WrapString(Resources.PoundsNitrogenPerYear);
                KilogramsNitrogenPerMonthPerAnimalString = WrapString(Resources.PoundsNitrogenPerMonthPerAnimalString);
                KilogramsNH3NString = WrapString(Resources.PoundsNH3N);
                KilogramsNH3String = WrapString(Resources.PoundsNH3);
                KilogramsCarbonString = WrapString(Resources.PoundsCarbon);
                KilogramsCarbonPerYearString = WrapString(Resources.PoundsCarbonPerYear);
                KilogramsPerYearString = WrapString(Resources.PoundsPerYear);
                MegaJoulesPerKilogramSquaredString = WrapString(Resources.BritishThermalUnitPerPoundSquared);
                KilogramsPerKilogramProteinIntakeString = WrapString(Resources.PoundsPerPoundProteinIntake);
                KilogramsVolatileSolidsPerKilogramFeedString = WrapString(Resources.PoundsVolatileSolidsPerPoundFeed);
                MegaJoulesPerKilogramString = WrapString(Resources.BritishThermalUnitPerPound);
                OneTimesPercentageString = WrapString("1x, %");
                GramsPerSquaredMeterString = WrapString(Resources.OuncesPerYardSquared);
                PartsPerMillionString = WrapString(Resources.PartsPerMillion);
                MicrogramsPerKilogramString = WrapString(Resources.GrainsPerPound);
                PercentageEtherExtractString = WrapString(Resources.PercentageEE);
                KilogramsTanString = WrapString(Resources.PoundsTan);
                KilogramsTanPerAnimalPerMonthString = WrapString(Resources.PoundsTanPerAnimalPerMonth);
                WetWeight = WrapString(Resources.WetWeight);
                KilogramsCo2PerKilogWattHour = WrapString(Resources.PoundsCO2PerBTU);
                KilogramsCo2PerGj = WrapString(Resources.PoundsCO2PerBTU);
                KilogramsCo2PerKilogramN = WrapString(Resources.PoundsCO2PerPoundN);
                KilogramsCo2PerKilogramP2O5 = WrapString(Resources.PoundCO2PerPoundP2O5);
                KilogramsCo2PerHectare = WrapString(Resources.PoundCO2PerAcre);
                KilowattHourPerAnimal = WrapString(Resources.BTUPerAnimal);
                KilowattHourPerPoultryPlacement = WrapString(Resources.BTUPerPoultryPlacement);
                GigajoulesPer1000Litres = WrapString(Resources.BTUPer1000Quarts);
                KilogramsNPer1000Litres = WrapString(Resources.PoundsNitrogenPer1000Quarts);
                KilogramsNitrogenPerKilogramDryMatter = WrapString(Resources.PoundsNitrogenPerPoundDryMatter);
                KilogramsCarbonPerKilogramDryMatter = WrapString(Resources.PoundsCarbonPerPoundDryMatter);
                KilogramsPhosphorusPerKilogramDryMatter = WrapString(Resources.PoundsPhosphorusPerPoundDryMatter);
                PercentageWetWeight = WrapString(Resources.PercentageWetWeight);
                KCalKgString = WrapString(Resources.BritishThermalUnitPerPound);
                DollarsPerKilogramString = WrapString(Resources.DollarsPerBushel);
                Dollars = WrapString(Resources.Dollars);
                DollarsPerHectare = WrapString(Resources.DollarsPerAcre);
                BushelString = WrapString(Resources.Bushel);
                MilliEquivalentsPer100G = WrapString(Resources.MilliEquivalentsPer100g); //US uses this unit same as us
                FertilizerKilogramsPerHectareString = WrapString(Resources.PoundsPerAcre);
                KilogramsCO2 = WrapString(Resources.EnumLbsCO2);
                KilogramsNh3NPerKilogramNitrogenString = WrapString(Resources.KilogramsNH3NPerKilogramN);
                KilogramsMethanePerHeadPerDayString = WrapString(Resources.PoundsMethanePerHeadPerDay);
                KilogramsPerGroupPerTimePeriodString = WrapString(Resources.PoundsPerGroupPerTimePeriod);
                KilogramsCarbonPerTreeTypeString = WrapString(Resources.PoundsCarbonPerTreeType);
                FractionString = WrapString(Resources.FractionString);
                KilogramsNPerKilogramNitrogenString = WrapString(Resources.KilogramsNPerKilogramNitrogenString);
                KilogramsN20String = WrapString(Resources.PoundsN2O);
                KilogramsN2OPerFieldString = WrapString(Resources.PoundsN2OPerField);
                KilogramsN2OnPerFieldString = WrapString(Resources.PoundsN2ONPerField);
                KilogramsNONPerFieldString = WrapString(Resources.PoundsNONPerField);
                MillimetersPerHectareString = WrapString(Resources.InchesPerAcre);
            }
        }

        #endregion
    }
}