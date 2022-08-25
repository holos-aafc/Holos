using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Plants
{
    /// <summary>
    /// Table 13. Default values for carbon to nitrogen ratios, nitrogen, and lignin contents in livestock manure for the steady-state method (from IPCC (2019), Table 5.5C)
    /// </summary>
    public class Table_13_Default_Values_For_Steady_State_Method_Provider
    {
        #region Fields

        private readonly AnimalTypeStringConverter _animalStringConverter;

        #endregion

        #region Properties
        /// <summary>
        /// Contains all the <see cref="Table_13_Default_Values_For_Steady_State_Method_Data"/> instances from the .csv file.
        /// </summary>
        public List<Table_13_Default_Values_For_Steady_State_Method_Data> DefaultValuesData { get; private set; }
        #endregion

        #region Constructors

        public Table_13_Default_Values_For_Steady_State_Method_Provider()
        {
            _animalStringConverter = new AnimalTypeStringConverter();
            DefaultValuesData = ReadFile();
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Finds the default values steady state method data for a specific animal type.
        /// </summary>
        /// <param name="animalType">The AnimalType for which data is needed</param>
        /// <returns>Returns an instance of <see cref="Table_13_Default_Values_For_Steady_State_Method_Data"/> with values for the specified animal type.
        /// If no data is found, returns null</returns>
        public Table_13_Default_Values_For_Steady_State_Method_Data GetSteadyStateMethodValueForAnimalGroup(AnimalType animalType)
        {
            var data = DefaultValuesData.Find(x => x.AnimalType == animalType);

            if (data == null)
            {
                Trace.TraceError($"{nameof(Table_13_Default_Values_For_Steady_State_Method_Provider)}.{nameof(GetSteadyStateMethodValueForAnimalGroup)} " +
                                 $": AnimalType {animalType} not found in the provider data. Returning null");
            }

            return data;
        }

        #endregion

        #region Private Methods

        private List<Table_13_Default_Values_For_Steady_State_Method_Data> ReadFile()
        {
            var fileData = new List<Table_13_Default_Values_For_Steady_State_Method_Data>();
            IEnumerable<string[]> fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.SteadyStateMethodDefaultNValues);
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;

            foreach (string[] line in fileLines.Skip(1))
            {
                if (string.IsNullOrEmpty(line[0]))
                {
                    break;
                }

                var animalType = _animalStringConverter.Convert(line[0].ParseUntilOrDefault());
                var carbonToNitrogenRatio = double.Parse(line[1].ParseUntilOrDefault(), cultureInfo);
                var nitrogenContent = double.Parse(line[2].ParseUntilOrDefault(), cultureInfo);
                var ligninContent = double.Parse(line[3].ParseUntilOrDefault(), cultureInfo);

                fileData.Add(new Table_13_Default_Values_For_Steady_State_Method_Data
                {
                    AnimalType = animalType,
                    CarbonToNitrogenRatio = carbonToNitrogenRatio,
                    NitrogenContentManure = nitrogenContent,
                    LigninContentManure = ligninContent,
                });
            }

            return fileData;
        }


        #endregion
    }
}
