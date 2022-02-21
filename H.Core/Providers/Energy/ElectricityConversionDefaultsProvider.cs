using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Energy
{
    /// <summary>
    /// Helps set the default values for electricity conversion.
    /// </summary>
    public class ElectricityConversionDefaultsProvider_Table_47
    {

        #region Fields

        private readonly ProvinceStringConverter _provinceStringConverter;
        private const int FirstYear = 1990;
        private const int LastYear = 2018;
        private const int TimePeriod = 5;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor. Initializes class and reads the required file.
        /// </summary>
        public ElectricityConversionDefaultsProvider_Table_47()
        {
            _provinceStringConverter = new ProvinceStringConverter();
            this.Data = this.ReadFile();
        }

        #endregion

        #region Properties
        /// <summary>
        /// List containing objects that store the year, province and the corresponding electricity conversion value
        /// </summary>
        private List<ElectricityConversionDefaultsData> Data { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Takes a year and province and returns the Data instance corresponding to the parameters.
        /// </summary>
        /// <param name="year"> The year for which we need the electricity conversion value</param>
        /// <param name="province">The province for which we need the electricity conversion value</param>
        /// <returns>Returns an instance of ElectricityConversionDefaultsData based on the year and province. If nothing found, returns null. 
        /// Unit of measurement of the electricity value of the data instance is kg CO2 kWh-1</returns>
        public ElectricityConversionDefaultsData GetElectricityConversionData(int year, Province province)
        {
            ElectricityConversionDefaultsData data = this.Data.Find(x => (x.Year == year) && (x.Province == province));

            if (data != null)
            {
                return data;
            }

            data = this.Data.Find(x => (x.Year == year));
            if (data != null)
            {
                Trace.TraceError($"{ nameof(ElectricityConversionDefaultsProvider_Table_47)}.{nameof(ElectricityConversionDefaultsProvider_Table_47.GetElectricityConversionData)}" +
                                 $" unable to find province: {province} in the available province data." +
                                 $" Returning 0.");
                return new ElectricityConversionDefaultsData();
            }
            else
            {
                Trace.TraceError($"{ nameof(ElectricityConversionDefaultsProvider_Table_47)}.{nameof(ElectricityConversionDefaultsProvider_Table_47.GetElectricityConversionData)}" +
                                 $" unable to find Year: {year} in the available year data." +
                                 $" Returning 0.");
                return new ElectricityConversionDefaultsData();
            }
        }

        /// <summary>
        /// Returns the electricity conversion value given a year and province.
        /// </summary>
        /// <param name="year">The year for which the electricity conversion value is needed.</param>
        /// <param name="province">The province for which the electricity conversion value is needed.</param>
        /// <returns>Returns a electricity conversion value based on the year and province. If nothing found, returns a multi-year average value(kg CO2 kWh-1)</returns>
        public double GetElectricityConversionValue (int year, Province province)
        {
            
            if (year > LastYear)
            {
                // Add 1 to include the LastYear in average calculation
                int startYear = LastYear - TimePeriod + 1;
                int endYear = LastYear;

                return CalculateAverageElectricityValue(province, startYear, endYear);
            }

            else if (year < FirstYear)
            {
                int startYear = FirstYear;
                // Subtract 1 to include the FirstYear in average calculation
                int endYear = FirstYear + TimePeriod - 1;

                return CalculateAverageElectricityValue(province, startYear, endYear);
            }

            else
            {
                ElectricityConversionDefaultsData data = this.Data.Find(x => (x.Year == year) && (x.Province == province));
                return data.ElectricityValue;
            }

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the electricity conversion file and puts the data from the file into a List of ElectricityConversionDefaultsData instances.
        /// </summary>
        /// <returns>Returns a list of ElectricityConversionDefaultsData objects based on the data read from the file</returns>
        private List<ElectricityConversionDefaultsData> ReadFile()
        {
            var results = new List<ElectricityConversionDefaultsData>();

            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            IEnumerable<string[]> fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.ElectricityConversionValues);

            // Read the province names from the file and create a dictionary with the provinces and corresponding indices.
            string[] provinceNames = fileLines.ElementAt(0);
            var provinceLocation = new Dictionary<Province, int>();
            GetDataLocation(provinceNames, provinceLocation);

            // Read each line in the file and store the data into a new ElectricityConversionDefaultsData instance.
            // Province data for each column is read using the corresponding dictionary index.
            foreach (string[] line in fileLines.Skip(1))
            {
                foreach (KeyValuePair<Province, int> province in provinceLocation)
                {
                    var year = int.Parse(line[0]);
                    Province currentProvince = province.Key;
                    var electricityValue = double.Parse(line[province.Value], cultureInfo);

                    results.Add(new ElectricityConversionDefaultsData
                    {
                        Year = year,
                        Province = currentProvince,
                        ElectricityValue = electricityValue,
                    });
                } 
            }
            return results;
        }

        /// <summary>
        /// Takes an array of strings containing province names and storing them in a dictionary based on an index.
        /// Each province is assigned a integer based on their position in the table column header.
        /// </summary>
        /// <param name="provinceNames">A string array containing the provinces for which we need to create an index</param>
        /// <param name="dataLocation">A dictionary that stores a province enumeration and a corresponding index. The index refers to the column number location of that province in the csv.</param>
        private void GetDataLocation(string[] provinceNames, Dictionary<Province, int> dataLocation)
        {
            int columnNumber = 0;

            foreach (string province in provinceNames)
            {
                if (!string.IsNullOrEmpty(province))
                {
                    dataLocation.Add(_provinceStringConverter.Convert(province), columnNumber);
                }
                columnNumber++;
            }
        }

        /// <summary>
        /// Calculates the average electricity value. This method is used when an input is given that is outside the bounds of available data
        /// </summary>
        /// <param name="province">The province for which we need to calculate the average electricity value</param>
        /// <param name="startYear">The starting year for the average calculation</param>
        /// <param name="endYear">The ending year for the average calculation</param>
        /// <returns>Returns the average electricity conversion value. The average is calculated based on the startYear, endYear and TimePeriod.</returns>
        private double CalculateAverageElectricityValue(Province province, int startYear, int endYear)
        {
            double sumElectricityValues = 0.0;
            ElectricityConversionDefaultsData data;
            for (int currentYear = startYear; currentYear <= endYear; currentYear++)
            {
                data = GetElectricityConversionData(currentYear, province);
                sumElectricityValues += data.ElectricityValue;
            }

            return sumElectricityValues / TimePeriod;
        }

        #endregion
    }
}
