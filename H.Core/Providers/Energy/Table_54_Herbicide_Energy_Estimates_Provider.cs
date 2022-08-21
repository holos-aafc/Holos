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

namespace H.Core.Providers.Energy
{
    /// <summary>
    /// Table 54. Herbicide energy requirement (Eherbicide) estimates for various crops in different regions of Canada for specific soils and tillage operations1 (GJ ha-1).
    /// </summary>
    public class Table_54_Herbicide_Energy_Estimates_Provider
    {

        #region Fields
        private readonly ProvinceStringConverter _provinceStringConverter;
        private readonly SoilFunctionalCategoryStringConverter _soilFunctionalCategoryStringConverter;
        private readonly TillageTypeStringConverter _tillageTypeStringConverter;
        private readonly CropTypeStringConverter _cropTypeStringConverter;

        private const double DefaultValue = 0.0;
        #endregion

        #region Constructors

        public Table_54_Herbicide_Energy_Estimates_Provider()
        {
            _provinceStringConverter = new ProvinceStringConverter();
            _soilFunctionalCategoryStringConverter = new SoilFunctionalCategoryStringConverter();
            _tillageTypeStringConverter = new TillageTypeStringConverter();
            _cropTypeStringConverter = new CropTypeStringConverter();

            this.Data = this.ReadFile();
        }
        #endregion


        #region Properties
        // List that stores all instances of HerbicideEnergyData. Each instance corresponds to a given province, soil category, tillage type and crop.
        List<Table_54_Herbicide_Energy_Estimates_Data> Data { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method takes in a set of characteristics and finds the Herbicide energy estimate based on those characteristics.
        /// </summary>
        /// <param name="province">The province for which Herbicide energy estimate data is required</param>
        /// <param name="soilCategory">The functional soil category for the province and crop</param>
        /// <param name="tillageType">The tillage type used for the crop</param>
        /// <param name="cropType">The type of crop for which Herbicide energy estimate data is required</param>
        /// <returns> The method returns an instance of Table_54_Herbicide_Energy_Estimates_Data based on the characteristics in the parameters. Returns null if nothing found.
        ///  Unit of measurement of herbicide energy estimates is GJ ha-1</returns>
        public Table_54_Herbicide_Energy_Estimates_Data GetHerbicideEnergyDataInstance(Province provinceName, SoilFunctionalCategory soilCategory, TillageType tillageType, CropType cropType)
        {
            var soilLookupType = soilCategory.GetSimplifiedSoilCategory();

            Table_54_Herbicide_Energy_Estimates_Data data = Data.Find(x => (x.Province == provinceName) && (x.SoilFunctionalCategory == soilLookupType) 
                                                            && (x.TillageType == tillageType) && (x.CropType == cropType));

            // If instance is found return the instance
            if (data != null)
            {
                return data;
            }

            // If instance is not found, we do a few searches to help trace which input parameter was wrong.
            data = this.Data.Find(x => (x.Province == provinceName) && (x.SoilFunctionalCategory == soilLookupType) && (x.TillageType == tillageType));

            // If we found an instance that contained our province, tillage and soil then the specified crop type was wrong
            if (data != null)
            {
                Trace.TraceError($"{nameof(Table_54_Herbicide_Energy_Estimates_Provider)}.{nameof(Table_54_Herbicide_Energy_Estimates_Provider.GetHerbicideEnergyDataInstance)}" +
                                 $" unable to find Crop: {cropType} in the available crop type data. Returning empty instance of {nameof(Table_54_Herbicide_Energy_Estimates_Data)}");
                return new Table_54_Herbicide_Energy_Estimates_Data();
            }

            data = this.Data.Find(x => (x.Province == provinceName) && (x.TillageType == tillageType) && (x.CropType == cropType));

            // If we found an instance that contained the province, tillage and crop then the specified soil category was wrong
            if (data != null)
            {
                Trace.TraceError($"{nameof(Table_54_Herbicide_Energy_Estimates_Provider)}.{nameof(Table_54_Herbicide_Energy_Estimates_Provider.GetHerbicideEnergyDataInstance)}" +
                                 $" unable to find Soil Category: {soilLookupType} in the available soil data. Returning {nameof(Table_54_Herbicide_Energy_Estimates_Data)}");
            }

            // The specified province type was wrong
            else
            {
                Trace.TraceError($"{nameof(Table_54_Herbicide_Energy_Estimates_Provider)}.{nameof(Table_54_Herbicide_Energy_Estimates_Provider.GetHerbicideEnergyDataInstance)}" +
                                 $" unable to find Province: {provinceName} in the available province data. Returning {nameof(Table_54_Herbicide_Energy_Estimates_Data)}");
            }

            return new Table_54_Herbicide_Energy_Estimates_Data();
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Reads the Comma Separated Value (.csv) file containing the required data. 
        /// </summary>
        /// <returns>Returns a List of Table_54_Herbicide_Energy_Estimates_Data instances. Each instance presents a single cell and contains properties that represent information regarding that cell.</returns>
        private List<Table_54_Herbicide_Energy_Estimates_Data> ReadFile()
        {
            var results = new List<Table_54_Herbicide_Energy_Estimates_Data>();

            IEnumerable<string[]> fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.HerbicideEnergyEstimates);

            // A new dictionary is created to store the column headers of the file.
            // Key = Cell location of an entry
            // Value = A custom class containing characteristics read from given file. These are the column headings.
            Dictionary<int, EnergyDataCharacteristics> dataLocation = GetDataLocation(fileLines);


            // We skip over the first three lines as they are already read above. We then move to each cell
            // and extract data from that cell. If a cell does not contain any data the value of DefaultValue is stored in the instance.
            foreach (string[] line in fileLines.Skip(3))
            {
                if (string.IsNullOrWhiteSpace(line[0]))
                {
                    Trace.Write($"{nameof(Table_54_Herbicide_Energy_Estimates_Provider)}.{nameof(ReadFile)}" +
                                $" - File: {nameof(CsvResourceNames.HerbicideEnergyEstimates)} : first cell of the line is empty. Exiting loop to stop reading more lines inside .csv file.");
                    break;
                }

                foreach (KeyValuePair<int, EnergyDataCharacteristics> item in dataLocation)
                {
                    int columnNumber = item.Key;
                    double herbicideValue = CheckCellValue(line, columnNumber);

                    Province provinceName = item.Value.Province;
                    SoilFunctionalCategory soilCategory = item.Value.SoilFunctionalCategory;
                    TillageType tillageType = item.Value.TillageType;
                    CropType cropType = _cropTypeStringConverter.Convert(line[0]);

                    results.Add(new Table_54_Herbicide_Energy_Estimates_Data
                    {
                        HerbicideEstimate = herbicideValue,
                        Province = provinceName,
                        SoilFunctionalCategory = soilCategory,
                        TillageType = tillageType,
                        CropType = cropType,
                    });
                }
                   
            }
            
            return results;
        }

        /// <summary>
        /// Reads the header columns of the csv file and stores the results in a dictionary where the key represents a column number and value is an instance
        /// of <see cref="EnergyDataCharacteristics"/>
        /// </summary>
        /// <param name="fileLines">The collection of string arrays denoting each line in the csv file.</param>
        /// <param name="dataLocation">A dictionary that stores the location of a Herbicide energy estimate given a column. The key is the column number
        /// and the value is a custom class that stores the header rows (characteristics).</param>
        private Dictionary<int, EnergyDataCharacteristics> GetDataLocation(IEnumerable<string[]> fileLines)
        {
            int numberOfColumns = fileLines.First().Length;
            var dataLocation = new Dictionary<int, EnergyDataCharacteristics>();

            // Each string array denotes a row in the column. Is row is taken from the string array collection of the file's lines.
            string[] provinceName = fileLines.ElementAt(0);
            string[] soilFunctionalCategory = fileLines.ElementAt(1);
            string[] tillageType = fileLines.ElementAt(2);

            // Iterate over each column and store the information in the rows into the custom class. This custom class is the dictionary key value.
            for (int columnNumber = 1; columnNumber < numberOfColumns; columnNumber++)
            {
                EnergyDataCharacteristics energyCharacteristics = new EnergyDataCharacteristics();

                energyCharacteristics.Province = _provinceStringConverter.Convert(provinceName[columnNumber]);
                energyCharacteristics.SoilFunctionalCategory = _soilFunctionalCategoryStringConverter.Convert(soilFunctionalCategory[columnNumber]);
                energyCharacteristics.TillageType = _tillageTypeStringConverter.Convert(tillageType[columnNumber]);

                dataLocation.Add(columnNumber, energyCharacteristics);
            }

            return dataLocation;
        }

        /// <summary>
        /// Checks a given cell in the file to see if it contains a valid value. Uses a DefaultValue constant to specify a value when a cell is empty.
        /// </summary>
        /// <param name="line">The line (row) in which the cell is located.</param>
        /// <param name="columnNumber">The column number in which the cell is located.</param>
        /// <returns>Returns the value of the cell. If the cell is empty, the default value of a cell is returned..</returns>
        private double CheckCellValue(string[] line, int columnNumber)
        {
            double value = DefaultValue;
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;

            if (!string.IsNullOrEmpty(line[columnNumber]))
            {
                value = double.Parse(line[columnNumber], cultureInfo);
            }

            return value;
        }

        #endregion
    }
}
