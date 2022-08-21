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
using H.Core.Providers.Fertilizer;

namespace H.Core.Providers.Energy
{
    /// <summary>
    /// Table 53. Fuel energy requirement (Efuel) estimates for various crops in different regions of Canada for specific soils and tillage operations1 (GJ ha-1).
    /// </summary>
    public class Table_52_Electricity_Conversion_Defaults_Provider
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
        public Table_52_Electricity_Conversion_Defaults_Provider()
        {
            _provinceStringConverter = new ProvinceStringConverter();
            this.Data = this.ReadFile();
        }

        #endregion

        #region Properties
        /// <summary>
        /// List containing objects that store the year, province and the corresponding electricity conversion value
        /// </summary>
        private List<Table_52_Electricity_Conversion_Defaults_Data> Data { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Takes a year and province and returns the Data instance corresponding to the parameters.
        /// </summary>
        /// <param name="year"> The year for which we need the electricity conversion value</param>
        /// <param name="province">The province for which we need the electricity conversion value</param>
        /// <returns>Returns an instance of Table_52_Electricity_Conversion_Defaults_Data based on the year and province. If nothing found, returns null. 
        /// Unit of measurement of the electricity value of the data instance is kg CO2 kWh-1</returns>
        public Table_52_Electricity_Conversion_Defaults_Data GetElectricityConversionData(int year, Province province)
        {
            Table_52_Electricity_Conversion_Defaults_Data data = this.Data.Find(x => (x.Year == year) && (x.Province == province));

            if (data != null)
            {
                return data;
            }

            data = this.Data.Find(x => (x.Year == year));
            if (data != null)
            {
                Trace.TraceError($"{ nameof(Table_52_Electricity_Conversion_Defaults_Provider)}.{nameof(Table_52_Electricity_Conversion_Defaults_Provider.GetElectricityConversionData)}" +
                                 $" unable to find province: {province} in the available province data." +
                                 $" Returning empty instance of {nameof(Table_52_Electricity_Conversion_Defaults_Data)}.");
                return new Table_52_Electricity_Conversion_Defaults_Data();
            }
            else
            {
                Trace.TraceError($"{ nameof(Table_52_Electricity_Conversion_Defaults_Provider)}.{nameof(Table_52_Electricity_Conversion_Defaults_Provider.GetElectricityConversionData)}" +
                                 $" unable to find Year: {year} in the available year data." +
                                 $" Returning empty instance of {nameof(Table_52_Electricity_Conversion_Defaults_Data)}.");
                return new Table_52_Electricity_Conversion_Defaults_Data();
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
                Table_52_Electricity_Conversion_Defaults_Data data = this.Data.Find(x => (x.Year == year) && (x.Province == province));
                return data.ElectricityValue;
            }

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the electricity conversion file and puts the data from the file into a List of Table_52_Electricity_Conversion_Defaults_Data instances.
        /// </summary>
        /// <returns>Returns a list of Table_52_Electricity_Conversion_Defaults_Data objects based on the data read from the file</returns>
        private List<Table_52_Electricity_Conversion_Defaults_Data> ReadFile()
        {
            var results = new List<Table_52_Electricity_Conversion_Defaults_Data>();

            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            IEnumerable<string[]> fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.ElectricityConversionValues);

            // Read the province names from the file and create a dictionary with the provinces and corresponding indices.
            string[] provinceNames = fileLines.ElementAt(0);
            var provinceFileMap = new Dictionary<Province, int>();
            MapDataFromFile(provinceNames, provinceFileMap);

            // Read each line in the file and store the data into a new Table_52_Electricity_Conversion_Defaults_Data instance.
            // Province data for each column is read using the corresponding dictionary index.
            foreach (string[] line in fileLines.Skip(1))
            {

                if (string.IsNullOrWhiteSpace(line[0]))
                {
                    Trace.Write($"{nameof(Table_52_Electricity_Conversion_Defaults_Provider)}.{nameof(ReadFile)}" +
                                $" - File: {nameof(CsvResourceNames.ElectricityConversionValues)} : first cell of the line is empty. Exiting loop to stop reading more lines inside .csv file.");
                    break;
                }

                foreach (KeyValuePair<Province, int> province in provinceFileMap)
                {
                    var year = int.Parse(line[0]);
                    Province currentProvince = province.Key;
                    var electricityValue = double.Parse(line[province.Value], cultureInfo);

                    results.Add(new Table_52_Electricity_Conversion_Defaults_Data
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
        /// Takes an array of strings containing province names and stores them in a dictionary based on an index.
        /// Each province is assigned a integer based on their position in the table column header.
        /// </summary>
        /// <param name="provinceNames">A string array containing the provinces for which we need to create an index</param>
        /// <param name="dataLocation">A dictionary that stores a province enumeration and a corresponding index. The index refers to the column number location of that province in the csv.</param>
        private void MapDataFromFile(string[] provinceNames, Dictionary<Province, int> dataLocation)
        {
            // Column number of the first province in the .csv file.
            int columnNumber = 1;

            foreach (string province in provinceNames.Skip(1))
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
            Table_52_Electricity_Conversion_Defaults_Data data;
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
