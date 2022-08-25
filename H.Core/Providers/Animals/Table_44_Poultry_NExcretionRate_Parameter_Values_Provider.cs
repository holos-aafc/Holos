using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Providers.Climate;
using H.Core.Providers.Plants;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 44. Parameter values for pullets, broilers (incl. roasters) and layers for the estimation of Nexcretion_rate
    /// </summary>
    public class Table_44_Poultry_NExcretionRate_Parameter_Values_Provider
    {
        #region Fields

        private readonly AnimalTypeStringConverter _animalTypeStringConverter;

        #endregion

        #region Properties

        /// <summary>
        /// A list containing all the data read from the .csv file for Table_44.
        /// </summary>
        public List<Table_44_Poultry_NExcretionRate_Parameter_Values_Data> PoultryParameterValueData { get;  }

        #endregion

        #region Constructor

        public Table_44_Poultry_NExcretionRate_Parameter_Values_Provider()
        {
            _animalTypeStringConverter = new AnimalTypeStringConverter();
            PoultryParameterValueData = ReadFile();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Looks through the available data for Nitrogen Excretion Rate Parameter Values based on the animal type specified.
        /// </summary>
        /// <param name="animalType">The type of animal (poultry) for which data values are required.</param>
        /// <returns>Returns an instance of <see cref="Table_44_Poultry_NExcretionRate_Parameter_Values_Data"/> based on the animal specified.
        /// If nothing is found, returns an empty instance.</returns>
        public Table_44_Poultry_NExcretionRate_Parameter_Values_Data GetParameterValues(AnimalType animalType)
        {

            var data = PoultryParameterValueData.Find(x => x.AnimalType == animalType);

            if (data != null)
            {
                return data;
            }

            Trace.TraceError($"{nameof(Table_44_Poultry_NExcretionRate_Parameter_Values_Provider)}.{nameof(GetParameterValues)}: No data for '{animalType.GetDescription()}'");

            return new Table_44_Poultry_NExcretionRate_Parameter_Values_Data();
        }

        #endregion

        #region Private Methods

        private List<Table_44_Poultry_NExcretionRate_Parameter_Values_Data> ReadFile()
        {
            var fileData = new List<Table_44_Poultry_NExcretionRate_Parameter_Values_Data>();
            IEnumerable<string[]> fileLines =
                CsvResourceReader.GetFileLines(CsvResourceNames.PoultryNExcretionParameterValues);

            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;

            foreach (var line in fileLines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line[0]))
                {
                    Trace.Write($"{nameof(Table_44_Poultry_NExcretionRate_Parameter_Values_Provider)}.{nameof(ReadFile)}" +
                                $" - File: {nameof(CsvResourceNames.PoultryNExcretionParameterValues)} : first cell of the line is empty. Exiting loop to stop reading more lines inside .csv file.");
                    break;
                }

                var animalType = _animalTypeStringConverter.Convert(line[0]);
                var dailyMeanIntake = double.Parse(line[1].ParseUntilOrDefault(), cultureInfo);
                var crudeProtein = double.Parse(line[2].ParseUntilOrDefault(), cultureInfo);
                var averageProteinContentLive = double.Parse(line[3].ParseUntilOrDefault(), cultureInfo);
                var weightGain = double.Parse(line[4].ParseUntilOrDefault(), cultureInfo);
                var averageProteinEgg = double.Parse(line[5].ParseUntilOrDefault(), cultureInfo);
                var eggProduction = double.Parse(line[6].ParseUntilOrDefault(), cultureInfo);
                var finalWeight = double.Parse(line[7].ParseUntilOrDefault(), cultureInfo);
                var initialWeight = double.Parse(line[8].ParseUntilOrDefault(), cultureInfo);
                var productionPeriod = double.Parse(line[9].ParseUntilOrDefault(), cultureInfo);

                fileData.Add(new Table_44_Poultry_NExcretionRate_Parameter_Values_Data
                {
                    AnimalType = animalType,
                    DailyMeanIntake = dailyMeanIntake,
                    CrudeProtein = crudeProtein,
                    ProteinLiveWeight = averageProteinContentLive,
                    WeightGain = weightGain,
                    ProteinContentEgg = averageProteinEgg,
                    EggProduction = eggProduction,
                    FinalWeight = finalWeight,
                    InitialWeight = initialWeight,
                    ProductionPeriod = productionPeriod,
                });
            }

            return fileData;
        }

        #endregion
    }
}
