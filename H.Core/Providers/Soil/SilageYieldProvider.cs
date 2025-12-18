using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Soil
{
    public class SilageYieldProvider
    {
        #region Constructors

        /// <summary>
        ///     Sets the province string converter and reads the csv file.
        /// </summary>
        public SilageYieldProvider()
        {
            _provinceStringConverter = new ProvinceStringConverter();
            _cropTypeStringConverter = new CropTypeStringConverter();

            Data = ReadFile();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     List containing instances of SmallAreaYieldData. Each instance corresponds to a single cell in the csv file.
        /// </summary>
        private List<SmallAreaYieldData> Data { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Given a year and a province, returns a single instance of SmallAreaYieldData.
        /// </summary>
        /// <param name="year">The year for which the yield value is needed.</param>
        /// <param name="province">The province for which the yield value is needed. </param>
        /// <returns>
        ///     Returns a single instance of SmallAreaYieldData given the year and province. If nothing is found, returns
        ///     null.
        /// </returns>
        public SmallAreaYieldData GetDataInstance(int year, Province province)
        {
            var data = Data.Find(x => x.Year == year && x.Province == province);

            if (data != null) return data;

            // Check if the year is a valid. If yes, then the province specified was wrong.
            data = Data.Find(x => x.Year == year);

            if (data != null)
                Trace.TraceError(
                    $"{nameof(SilageYieldProvider)}.{nameof(GetDataInstance)} cannot find the Province: {province} " +
                    " in the available data. Returning null");
            else
                Trace.TraceError(
                    $"{nameof(SilageYieldProvider)}.{nameof(GetDataInstance)} cannot find the Year: {year} " +
                    " in the available data. Returning null");

            return null;
        }

        #endregion

        #region Fields

        private readonly ProvinceStringConverter _provinceStringConverter;
        private readonly CropTypeStringConverter _cropTypeStringConverter;

        #endregion

        #region Private Methods

        /// <summary>
        ///     Reads the .csv file for data and stores the result in a List of SmallAreaYieldData instances. Each instance
        ///     corresponds to a single cell in the csv.
        /// </summary>
        /// <returns>A List containing SmallAreaYieldData instances.</returns>
        private List<SmallAreaYieldData> ReadFile()
        {
            var cornSillageInstances = new List<SmallAreaYieldData>();
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;

            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.SilageYields);

            // Get the location of each province inside the csv file.
            // Dictionary Key = Column Number of the province.
            // Dictionary Value = The province corresponding to the column number.
            var yieldDataLocation = GetDataLocation(fileLines.ElementAt(0));

            // The second line in the csv contains the crop names.
            var cropNames = fileLines.ElementAt(1);

            foreach (var line in fileLines.Skip(2))
            foreach (var item in yieldDataLocation)
            {
                var columnNumber = item.Key;
                var year = int.Parse(line[0]);
                var cropType = _cropTypeStringConverter.Convert(cropNames[columnNumber]);
                var yieldValue = string.IsNullOrEmpty(line[columnNumber])
                    ? 0
                    : int.Parse(line[columnNumber], cultureInfo);
                var provinceName = item.Value;

                cornSillageInstances.Add(new SmallAreaYieldData
                {
                    Year = year,
                    Yield = yieldValue,
                    Province = provinceName,
                    CropType = cropType,
                    Id = 0,
                    Polygon = 0
                });
            }

            return cornSillageInstances;
        }


        /// <summary>
        ///     Takes a string array containing province names and returns a dictionary mapping the location of the province to a
        ///     column number.
        /// </summary>
        /// <param name="provinceNames">A string array containing the names of the provinces.</param>
        /// <returns>A dictionary where: Key = column number of province . Value = Province name corresponding to dictionary key.</returns>
        private Dictionary<int, Province> GetDataLocation(string[] provinceNames)
        {
            var yieldDataLocation = new Dictionary<int, Province>();
            var columnNumber = 1;

            // Skip 1 name in the dictionary as its a non-province entry.
            foreach (var province in provinceNames.Skip(1))
            {
                if (!string.IsNullOrEmpty(province))
                    yieldDataLocation.Add(columnNumber, _provinceStringConverter.Convert(province));
                columnNumber++;
            }

            return yieldDataLocation;
        }

        #endregion
    }
}