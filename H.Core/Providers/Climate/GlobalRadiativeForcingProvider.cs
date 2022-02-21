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

namespace H.Core.Providers.Climate
{
    public class GlobalRadiativeForcingProvider
    {
        #region Fields
        private readonly EmissionTypeStringConverter _emissionTypeStringConverter;

        #endregion

        #region Constructors

        /// <summary>
        /// Sets the string converter and reads the CSV file.
        /// </summary>
        public GlobalRadiativeForcingProvider()
        {
            _emissionTypeStringConverter = new EmissionTypeStringConverter();
            this.Data = this.ReadFile();
        }

        #endregion

        #region Properties

        /// <summary>
        /// A list that stores each global radiative forcing value as an instance with a corresonding year and emission type.
        /// </summary>
        List<GlobalRadiativeForcingData> Data { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets an instance of global radiative forcing given a year and emission type. 
        /// </summary>
        /// <param name="year">The year for which we need the global radiative forcing value</param>
        /// <param name="emissionType">The emission for which we need the global radiative forcing value</param>
        /// <returns>An instance of GlobalRadiativeForcingData based on the year and emission type. Returns null if nothing found.
        ///  Unit of measurement for the global radiative forcing value = W m-2</returns>
        public GlobalRadiativeForcingData GetGlobalRadiativeForcingInstance(int year, EmissionTypes emissionType)
        {
            GlobalRadiativeForcingData data = this.Data.Find(x => (x.Year == year) && (x.EmissionType == emissionType));

            if (data != null)
            {
                return data;
            }

            data = this.Data.Find(x => (x.Year == year));
            if (data != null)
            {
                Trace.TraceError($"{nameof(GlobalRadiativeForcingProvider)}.{nameof(GlobalRadiativeForcingProvider.GetGlobalRadiativeForcingInstance)}" +
                                 $" the EmissionType: {emissionType} was not found in the available data. Returning null");
            }
            else
            {
                Trace.TraceError($"{nameof(GlobalRadiativeForcingProvider)}.{nameof(GlobalRadiativeForcingProvider.GetGlobalRadiativeForcingInstance)} " +
                                 $"the Year: {year} was not found in the available data. Returning null");
            }

            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the data from the csv file. An instance of every cell is created and stored in the returning list.
        /// </summary>
        /// <returns>Returns a list of GlobalRadiativeForcingData instances corresponding to each cell in the csv </returns>
        private List<GlobalRadiativeForcingData> ReadFile()
        {
            var results = new List<GlobalRadiativeForcingData>();
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            const int NumberOfHeaders = 1;

            IEnumerable<string[]> fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.GlobalRadiativeForcing);

            // Dictionary Key = Column number where the data is located.
            // Dictionary Value = The value of the particular emission type as read from csv file.
            Dictionary<int, EmissionTypes> dataLocation = GetDataLocation(fileLines);

            foreach (string[] line in fileLines.Skip(NumberOfHeaders))
            {
                foreach (KeyValuePair<int, EmissionTypes> item in dataLocation)
                {
                    var year = int.Parse(line[0]);
                    // Gets the value from a specific column in the line. Column is based on dictionary key.
                    var radiativeForcingValue = double.Parse(line[item.Key], cultureInfo);

                    results.Add(new GlobalRadiativeForcingData 
                    {
                        Year = year,
                        EmissionType = item.Value,
                        RadiativeForcingValue = radiativeForcingValue,
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
        private Dictionary<int, EmissionTypes> GetDataLocation(IEnumerable<string[]> fileLines)
        {
            int columnNumber = 1;
            string[] firstLine = fileLines.ElementAt(0);
            var dataLocation = new Dictionary<int, EmissionTypes>();

            // Skip the number of header rows containing headings
            foreach (string item in firstLine.Skip(1))
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
