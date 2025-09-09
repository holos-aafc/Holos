using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Irrigation
{
    /// <summary>
    ///     Table 4. Percentage of total annual irrigation water applied by month for each province/region in Canada (average
    ///     values across 2010 and 2012).
    /// </summary>
    public class Table_4_Monthly_Irrigation_Water_Application_Provider
    {
        #region Fields

        private readonly ProvinceStringConverter _provinceStringConverter;

        #endregion

        #region Constructors

        public Table_4_Monthly_Irrigation_Water_Application_Provider()
        {
            _provinceStringConverter = new ProvinceStringConverter();

            Data = ReadFile();
        }

        #endregion

        #region Properties

        private List<Table_4_Monthly_Irrigation_Water_Application_Data> Data { get; }

        #endregion

        #region Public Methods

        public double GetTotalGrowingSeasonIrrigation(Province province)
        {
            var months = new List<Months>
                { Months.May, Months.June, Months.July, Months.August, Months.September, Months.October };
            var result = 0d;

            foreach (var month in months)
            {
                var irrigationForMonth = GetMonthlyAverageIrrigationDataInstance(month, province);
                var amount = irrigationForMonth.IrrigationVolume;

                result += amount;
            }

            return result;
        }

        /// <summary>
        ///     Takes a month and province and finds the corresponding monthly average data instance specific to that input.
        /// </summary>
        /// <param name="month">The month for which we need the data instance</param>
        /// <param name="province">The year for which we need the data instance</param>
        /// <returns>Returns the data instance based on the month and year. Returns null when nothing is found</returns>
        public Table_4_Monthly_Irrigation_Water_Application_Data GetMonthlyAverageIrrigationDataInstance(Months month,
            Province province)
        {
            var irrigationWaterApplicationData = Data.Find(x => x.Month == month && x.Province == province);

            if (irrigationWaterApplicationData != null) return irrigationWaterApplicationData;

            irrigationWaterApplicationData = Data.Find(x => x.Month == month);

            if (irrigationWaterApplicationData != null)
            {
                Trace.TraceError(
                    $"{nameof(Table_4_Monthly_Irrigation_Water_Application_Provider)}.{nameof(GetMonthlyAverageIrrigationDataInstance)}" +
                    $" unable to find Province: {province} in the available province data." +
                    " Returning null.");

                return null;
            }

            //Trace.TraceError($"{nameof(Table_4_Monthly_Irrigation_Water_Application_Provider)}.{nameof(Table_4_Monthly_Irrigation_Water_Application_Provider.GetMonthlyAverageIrrigationDataInstance)}" +
            //                 $" unable to find Month: {month} in the available month data." +
            //                 $" Returning null.");
            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Reads the file containing the average monthly irrigation data and stores the results in a list of objects.
        /// </summary>
        /// <returns>Returns a list of Table_49_Electricity_Conversion_Defaults_Data objects based on the data read from the file</returns>
        private List<Table_4_Monthly_Irrigation_Water_Application_Data> ReadFile()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.IrrigationByMonth);

            var results = new List<Table_4_Monthly_Irrigation_Water_Application_Data>();

            // Store location of data into a dictionary based on column number.
            // Key = Name of Province
            // Value = Its index in the csv file
            var dataLocation = GetDataLocation(fileLines);

            foreach (var line in fileLines.Skip(1))
            foreach (var province in dataLocation)
            {
                var month = (Months)Enum.Parse(typeof(Months), line[0]);
                var currentProvince = province.Key;
                var irrigationVolume = double.Parse(line[province.Value], cultureInfo);

                results.Add(new Table_4_Monthly_Irrigation_Water_Application_Data
                {
                    Month = month,
                    Province = currentProvince,
                    IrrigationVolume = irrigationVolume
                });
            }

            return results;
        }

        /// <summary>
        ///     Sets the index of each province based on the input province names and a starting index value.
        /// </summary>
        /// <param name="provinceNames">A string array containing the provinces for which we need to create an index</param>
        /// <param name="provinces">
        ///     A <Province, int> dictionary that stores a province enumeration and a corresponding index
        /// </param>
        private Dictionary<Province, int> GetDataLocation(IEnumerable<string[]> fileLines)
        {
            var columnNumber = 1;
            var provinceNames = fileLines.ElementAt(0);
            var dataLocation = new Dictionary<Province, int>();

            foreach (var province in provinceNames.Skip(1))
            {
                if (!string.IsNullOrEmpty(province))
                    dataLocation.Add(_provinceStringConverter.Convert(province), columnNumber);
                columnNumber++;
            }

            return dataLocation;
        }

        #endregion
    }
}