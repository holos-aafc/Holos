using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;
using H.Content;
using H.Core.Providers.Energy;

namespace H.Core.Providers.Climate
{
    /// <summary>
    /// Table 65. Global warming potential of emissions.
    /// </summary>
    public class Table_65_Global_Warming_Emissions_Potential_Provider
    {
        #region Fields

        private readonly EmissionTypeStringConverter _emissionTypeStringConverter;

        #endregion

        #region Constructors

        /// <summary>
        /// Sets the string converter and reads the CSV file.
        /// </summary>
        public Table_65_Global_Warming_Emissions_Potential_Provider()
        {
            _emissionTypeStringConverter = new EmissionTypeStringConverter();
            this.Data = this.ReadFile();
        }

        #endregion

        #region Properties

        /// <summary>
        /// A list that stores each global radiative forcing value as an instance with a corresonding year and emission type.
        /// </summary>
        private List<Table_65_Global_Warming_Emissions_Potential_Data> Data { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets an instance of global warming emissions given a year and emission type. 
        /// </summary>
        /// <param name="year">The year for which we need the global warming emissions value</param>
        /// <param name="emissionType">The emission for which we need the global warming emissions value</param>
        /// <returns>An instance of Table_65_Global_Warming_Emissions_Potential_Data based on the year and emission type. Returns empty instance of <see cref="Table_65_Global_Warming_Emissions_Potential_Data"/> otherwise.
        /// Unit of measurement of instances's values = Global Warming Potential</returns>
        public Table_65_Global_Warming_Emissions_Potential_Data GetGlobalWarmingEmissionsInstance (int year, EmissionTypes emissionType)
        {
            Table_65_Global_Warming_Emissions_Potential_Data data = this.Data.Find(x => (x.Year == year) && (x.EmissionType == emissionType));

            if (data != null)
            {
                return data;
            }

            data = this.Data.Find(x => (x.Year == year));

            if (data != null)
            {
                Trace.TraceError($"{nameof(Table_65_Global_Warming_Emissions_Potential_Provider)}.{nameof(Table_65_Global_Warming_Emissions_Potential_Provider.GetGlobalWarmingEmissionsInstance)}" +
                                 $" the EmissionType: {emissionType} was not found in the available data. Returning null");
            }
            else
            {
                Trace.TraceError($"{nameof(Table_65_Global_Warming_Emissions_Potential_Provider)}.{nameof(Table_65_Global_Warming_Emissions_Potential_Provider.GetGlobalWarmingEmissionsInstance)} " +
                                 $"the Year: {year} was not found in the available data. Returning null");
            }

            return new Table_65_Global_Warming_Emissions_Potential_Data();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the data from the csv file. An instance of every cell is created and stored in the returning list.
        /// </summary>
        /// <returns>Returns a list of Table_65_Global_Warming_Emissions_Potential_Data instances corresponding to each cell in the csv </returns>
        private List<Table_65_Global_Warming_Emissions_Potential_Data> ReadFile()
        {
            var results = new List<Table_65_Global_Warming_Emissions_Potential_Data>();
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            const int NumberOfHeaders = 1;

            IEnumerable<string[]> fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.GlobalWarmingPotential);

            // Dictionary Key = Column number where the data is located.
            // Dictionary Value = The value of the particular emission type as read from csv file.
            Dictionary<int, EmissionTypes> dataLocation = MapDataLocationFromFile(fileLines);


            foreach (string[] line in fileLines.Skip(NumberOfHeaders))
            {
                if (string.IsNullOrWhiteSpace(line[0]))
                {
                    Trace.Write($"{nameof(Table_65_Global_Warming_Emissions_Potential_Provider)}.{nameof(ReadFile)}" +
                                $" - File: {nameof(CsvResourceNames.GlobalWarmingPotential)} : first cell of the line is empty. Exiting loop to stop reading more lines inside .csv file.");
                    break;
                }

                foreach (KeyValuePair<int, EmissionTypes> item in dataLocation)
                {
                    var year = int.Parse(line[0]);
                    string source = line[1];
                    // Gets the value from a specific column in the line. Column is based on dictionary key.
                    var globalWarmingPotential = double.Parse(line[item.Key], cultureInfo);

                    results.Add(new Table_65_Global_Warming_Emissions_Potential_Data
                    {
                        Year = year,
                        Source = source,
                        EmissionType = item.Value,
                        GlobalWarmingPotentialValue = globalWarmingPotential,
                    });
                }
            }


            return results;
        }

        /// <summary>
        /// Get the data location of each cell and store it in a dictionary.
        /// </summary>
        /// <param name="fileLines">A colllection of string arrays representing each line in the file.</param>
        /// <returns>A dictionary containing the column number and corresponding data of that column.
        /// Dictionary Key = Column number of data.
        /// Dictionary Value = Value of the emission type.</returns>
        private Dictionary<int, EmissionTypes> MapDataLocationFromFile(IEnumerable<string[]> fileLines)
        {
            int columnNumber = 2;
            string[] firstLine = fileLines.ElementAt(0);
            var dataLocation = new Dictionary<int, EmissionTypes>();

            foreach (string item in firstLine.Skip(2))
            {
                if (!string.IsNullOrEmpty(item))
                {
                    dataLocation.Add(columnNumber, _emissionTypeStringConverter.Convert(item));
                }
                columnNumber++;
            }

            return dataLocation;
        }
        #endregion

    }
}
