﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Energy
{
    /// <summary>
    ///     Table 50: Fuel energy requirement (Efuel) estimates for various crops in different regions of Canada for specific
    ///     soils and tillage operations1 (GJ ha-1)
    /// </summary>
    public class Table_50_Fuel_Energy_Estimates_Provider
    {
        #region Constructors

        public Table_50_Fuel_Energy_Estimates_Provider()
        {
            _provinceStringConverter = new ProvinceStringConverter();
            _soilFunctionalCategoryStringConverter = new SoilFunctionalCategoryStringConverter();
            _tillageTypeStringConverter = new TillageTypeStringConverter();
            _cropTypeStringConverter = new CropTypeStringConverter();

            Data = ReadFile();
        }

        #endregion

        #region Properties

        // List that stores all instances of FuelEnergyData. Each instance corresponds to a given province, soil category, tillage type and crop.
        private List<Table_50_Fuel_Energy_Estimates_Data> Data { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     This method takes in a set of characteristics and finds the fuel energy estimate given those characteristics.
        /// </summary>
        /// <param name="province">The province for which fuel energy estimate data is required</param>
        /// <param name="soilCategory">The functional soil category for the province and crop</param>
        /// <param name="tillageType">The tillage type used for the crop</param>
        /// <param name="cropType">The type of crop for which fuel energy estimate data is required</param>
        /// <returns>
        ///     The method returns an instance of FuelEnergyEstimateData based on the characteristics in the parameters. Returns an
        ///     empty instance of <see cref="Table_50_Fuel_Energy_Estimates_Data" /> if nothing found
        ///     Unit of measurement of fuel energy estimate value = GJ ha-1
        /// </returns>
        public Table_50_Fuel_Energy_Estimates_Data GetFuelEnergyEstimatesDataInstance(Province province,
            SoilFunctionalCategory soilCategory, TillageType tillageType, CropType cropType)
        {
            var soilLookupType = soilCategory.GetSimplifiedSoilCategory();

            // No summer fallow in table
            if (cropType.IsFallow()) cropType = CropType.Fallow;

            var data = Data.Find(x => x.Province == province && x.SoilFunctionalCategory == soilLookupType
                                                             && x.TillageType == tillageType && x.CropType == cropType);

            // If instance is found return the instance
            if (data != null) return data;

            // If instance is not found, we do a few searches to help trace which input parameter was wrong.
            // Lookup three of the four paramters and see if anything is found or not.
            data = Data.Find(x =>
                x.Province == province && x.SoilFunctionalCategory == soilLookupType && x.TillageType == tillageType);

            // The specified crop type was wrong
            if (data != null)
            {
                Trace.TraceError(
                    $"{nameof(Table_50_Fuel_Energy_Estimates_Provider)}.{nameof(GetFuelEnergyEstimatesDataInstance)}" +
                    $" unable to find Crop: {cropType} in the available crop type data. Returning empty {nameof(Table_50_Fuel_Energy_Estimates_Data)}");
                return new Table_50_Fuel_Energy_Estimates_Data();
            }

            data = Data.Find(x => x.Province == province && x.TillageType == tillageType && x.CropType == cropType);

            // The specified soil category was wrong
            if (data != null)
                Trace.TraceError(
                    $"{nameof(Table_50_Fuel_Energy_Estimates_Provider)}.{nameof(GetFuelEnergyEstimatesDataInstance)}" +
                    $" unable to find Soil Category: {soilLookupType} in the available soil data. Returning empty {nameof(Table_50_Fuel_Energy_Estimates_Data)}");

            // The specified province type was wrong
            else
                Trace.TraceError(
                    $"{nameof(Table_50_Fuel_Energy_Estimates_Provider)}.{nameof(GetFuelEnergyEstimatesDataInstance)}" +
                    $" unable to find Province: {province} in the available province data. Returning empty {nameof(Table_50_Fuel_Energy_Estimates_Data)}");

            return new Table_50_Fuel_Energy_Estimates_Data();
        }

        #endregion

        #region Fields

        private readonly ProvinceStringConverter _provinceStringConverter;
        private readonly SoilFunctionalCategoryStringConverter _soilFunctionalCategoryStringConverter;
        private readonly TillageTypeStringConverter _tillageTypeStringConverter;
        private readonly CropTypeStringConverter _cropTypeStringConverter;

        // Sets the default value for when no fuel energy estimate value is available for a cell
        private const double DefaultValue = 0.0;

        #endregion

        #region Private Methods

        /// <summary>
        ///     Reads the Comma Separated Value (.csv) file containing the required data.
        /// </summary>
        /// <returns>
        ///     Returns a List of Table_50_Fuel_Energy_Estimates_Data instances. Each instance represents a single cell and
        ///     contains properties that are read from the csv file.
        /// </returns>
        private List<Table_50_Fuel_Energy_Estimates_Data> ReadFile()
        {
            var results = new List<Table_50_Fuel_Energy_Estimates_Data>();

            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.FuelEnergyEstimates);

            // A new dictionary is created to store the column headers of the file.
            // Key = Cell location of an entry
            // Value = A custom class containing characteristics read from given file.
            var dataLocation = GetDataLocation(fileLines);


            // We skip over the first three lines as they are already read above. We then move to each cell
            // and extract data from that cell. If a cell does not contain any data the value of DefaultValue is stored in the instance.
            foreach (var line in fileLines.Skip(3))
            {
                if (string.IsNullOrWhiteSpace(line[0]))
                {
                    Trace.Write($"{nameof(Table_50_Fuel_Energy_Estimates_Provider)}.{nameof(ReadFile)}" +
                                $" - File: {nameof(CsvResourceNames.FuelEnergyEstimates)} : first cell of the line is empty. Exiting loop to stop reading more lines inside .csv file.");
                    break;
                }

                foreach (var item in dataLocation)
                {
                    var columnNumber = item.Key;
                    var fuelEnergyEstimate = CheckCellValue(line, columnNumber);

                    var provinceName = item.Value.Province;
                    var soilFunctionalCategory = item.Value.SoilFunctionalCategory;
                    var tillageType = item.Value.TillageType;
                    var cropType = _cropTypeStringConverter.Convert(line[0]);


                    results.Add(new Table_50_Fuel_Energy_Estimates_Data
                    {
                        FuelEstimate = fuelEnergyEstimate,
                        Province = provinceName,
                        SoilFunctionalCategory = soilFunctionalCategory,
                        TillageType = tillageType,
                        CropType = cropType
                    });
                }
            }

            return results;
        }

        /// <summary>
        ///     Reads the header columns of the csv file and stores the results in a dictionary. The key of the dictionary is the
        ///     column number
        ///     while the value is an instance of <see cref="EnergyDataCharacteristics" /> which contains various information from
        ///     that cell.
        /// </summary>
        /// <param name="fileLines">The collection of string arrays denoting each line in the csv file.</param>
        /// <param name="dataLocation">
        ///     A dictionary that stores the location of a fuel energy estimate given a column. The key is the column number
        ///     and the value is a custom class that stores the header rows (characteristics).
        /// </param>
        /// <returns>A dictionary that contains the column number as key and Province, SoilCategory and TillageType as value.</returns>
        private Dictionary<int, EnergyDataCharacteristics> GetDataLocation(IEnumerable<string[]> fileLines)
        {
            var numberOfColumns = fileLines.First().Length;
            var dataLocation = new Dictionary<int, EnergyDataCharacteristics>();

            // Each string array denotes a row in the column. Is row is taken from the string array collection of the file's lines.
            var provinceNames = fileLines.ElementAt(0);
            var soilFunctionalCategory = fileLines.ElementAt(1);
            var tillageTypes = fileLines.ElementAt(2);

            // Iterate over each column and store the information in the rows into the custom class. This custom class is the dictionary key value.
            for (var columnNumber = 1; columnNumber < numberOfColumns; columnNumber++)
            {
                var energyCharacteristics = new EnergyDataCharacteristics();

                energyCharacteristics.Province = _provinceStringConverter.Convert(provinceNames[columnNumber]);
                energyCharacteristics.SoilFunctionalCategory =
                    _soilFunctionalCategoryStringConverter.Convert(soilFunctionalCategory[columnNumber]);
                energyCharacteristics.TillageType = _tillageTypeStringConverter.Convert(tillageTypes[columnNumber]);

                dataLocation.Add(columnNumber, energyCharacteristics);
            }

            return dataLocation;
        }

        /// <summary>
        ///     Checks a given cell in the file to see if it contains a valid value.
        /// </summary>
        /// <param name="line">The line (row) in which the cell is located.</param>
        /// <param name="columnNumber">The column number in which the cell is located.</param>
        /// <returns>Returns the value of the cell. If the cell is empty, the default value of a cell is returned..</returns>
        private double CheckCellValue(string[] line, int columnNumber)
        {
            var value = DefaultValue;
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;

            if (!string.IsNullOrEmpty(line[columnNumber])) value = double.Parse(line[columnNumber], cultureInfo);

            return value;
        }

        #endregion
    }
}