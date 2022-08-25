using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Globalization;
using H.Core.Enumerations;
using H.Core.Converters;
using H.Infrastructure;
using H.Content;


namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 24. Average milk production for dairy cows from 1990 to 2020 by province. Source: ECCC (2022)
    /// </summary>
    public class Table_24_Average_Milk_Production_Dairy_Cows_Provider
    {
        #region Fields

        private readonly ProvinceStringConverter _provinceStringConverter;

        #endregion

        #region Constructors

        public Table_24_Average_Milk_Production_Dairy_Cows_Provider()
        {

            _provinceStringConverter = new ProvinceStringConverter();

            this.AvgMilkProductionData = this.ReadFile();

        }

        #endregion

        #region Properties

        /// <summary>
        /// A list of <see cref="Table_24_Average_Milk_Production_Dairy_Cows_Data"/> where each entry in the list represents a single cell in the provider's csv file.
        /// </summary>
        private List<Table_24_Average_Milk_Production_Dairy_Cows_Data> AvgMilkProductionData { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// This methods takes in a province and a year and returns the average milk production value. If either of the province or year are incorrect, the method returns zero.
        /// Unit of Measurement: kg head-1 day-1
        /// </summary>
        /// <param name="year">The year for which we need the milk production data.</param>
        /// <param name="province">The province for which we need the milk production data.</param>
        /// <returns>Returns a value of milk production (kg head-1 day-1). If value isn't found, the method returns 0.</returns>
        public double GetAverageMilkProductionForDairyCowsValue(int year, Province province)
        {
            // Maximum year in table is 2020
            if (year > 2020)
            {
                year = 2020;
            }

            // Minimum year in table is 1990
            if (year < 1990)
            {
                year = 1990;
            }

            Table_24_Average_Milk_Production_Dairy_Cows_Data data = this.AvgMilkProductionData.Find(x => (x.Year == year) && (x.Province == province));

            if (data != null)
            {
                return data.AverageMilkProduction;
            }

            data = this.AvgMilkProductionData.Find(x => (x.Year == year));
            if (data != null)
            {
                Trace.TraceError($"{ nameof(Table_24_Average_Milk_Production_Dairy_Cows_Provider)}.{nameof(Table_24_Average_Milk_Production_Dairy_Cows_Provider.GetAverageMilkProductionForDairyCowsValue)}" +
                                 $" unable to find province: {province} in the available province data." +
                                 $" Returning 0.");
                return 0;
            }
            else
            {
                Trace.TraceError($"{ nameof(Table_24_Average_Milk_Production_Dairy_Cows_Provider)}.{nameof(Table_24_Average_Milk_Production_Dairy_Cows_Provider.GetAverageMilkProductionForDairyCowsValue)}" +
                                 $" unable to find Year: {year} in the available year data." +
                                 $" Returning 0.");
                return 0;
            }

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the provider's .csv file and stores the data in a list. Each element in the list represents a single cell in the .csv file.
        /// </summary>
        /// <returns>Returns a list containing <see cref="Table_24_Average_Milk_Production_Dairy_Cows_Data"/></returns>
        public List<Table_24_Average_Milk_Production_Dairy_Cows_Data> ReadFile()
        {
            var milkProductionData = new List<Table_24_Average_Milk_Production_Dairy_Cows_Data>();

            CultureInfo cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            IEnumerable<string[]> fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.AverageMilkProductionDairyCows);

            // Get the first line of the collection as that contains province names
            string[] provinceNames = fileLines.ElementAt(0);

            // Store the province names in a dictionary.
            // Key = Province = Province Name
            // Value = int = index of province in the provider .csv file.
            Dictionary<Province, int> dataLocation = GetDataLocation(provinceNames);

            // Loop through each row
            foreach (string[] line in fileLines.Skip(1))
            {
                // For each row, go to each column and store the relevant cell as a Table_24_Average_Milk_Production_Dairy_Cows_Data instance in a list.
                foreach (KeyValuePair<Province, int> province in dataLocation)
                {
                    var year = int.Parse(line[0]);
                    Province currentProvince = province.Key;
                    var averageMilkProduction = double.Parse(line[province.Value], cultureInfo);

                    milkProductionData.Add(new Table_24_Average_Milk_Production_Dairy_Cows_Data 
                    {
                        Year = year,
                        Province = currentProvince,
                        AverageMilkProduction = averageMilkProduction,
                    });
                }
            }
            return milkProductionData;
        }

        /// <summary>
        /// Helps find the index of provinces in a string array. The string array is the first element (first line) in the collection of lines read from the .csv file.
        /// </summary>
        /// <param name="provinceNames">An array of strings with each element representing a single province.</param>
        /// <returns>A dictionary containing the province and its index in the array.</returns>
        private Dictionary<Province, int> GetDataLocation(string[] provinceNames)
        {
            var dataLocation = new Dictionary<Province,int>();
            // The first column at index 0 does not contain a province name so we index from 1.
            int columnNumber = 1;

            // Skip the first element in the string array as it doesn't contain  a province name.
            foreach (string provinceName in provinceNames.Skip(1))
            {
                if (!string.IsNullOrEmpty(provinceName))
                {
                    dataLocation.Add(_provinceStringConverter.Convert(provinceName), columnNumber);
                }
                columnNumber++;
            }

            return dataLocation;
        }

        #endregion
    }
}
