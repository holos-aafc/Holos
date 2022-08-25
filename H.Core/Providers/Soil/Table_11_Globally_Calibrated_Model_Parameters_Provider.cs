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


namespace H.Core.Providers.Soil
{
    /// <summary>
    /// Table 11: Globally calibrated model parameters to be used to estimate SOC changes from mineral soils with the IPCC (2019) Tier 2 steady-state method
    /// </summary>
    public class Table_11_Globally_Calibrated_Model_Parameters_Provider
    {
        #region Fields
        private readonly ModelParameterStringConverter _modelParameterStringConverter;
        private readonly TillageTypeStringConverter _tillageTypeStringConverter;
        #endregion

        #region Constructors
        /// <summary>
        /// Initiializes the string converters and reads the csv data file.
        /// </summary>
        public Table_11_Globally_Calibrated_Model_Parameters_Provider()
        {
            _modelParameterStringConverter = new ModelParameterStringConverter();
            _tillageTypeStringConverter = new TillageTypeStringConverter();

            this.Data = this.ReadFile();
        }
        #endregion

        #region Properties

        /// <summary>
        /// List of Table_11_Globally_Calibrated_Model_Parameters_Data instances. Each parameter has its own data instance in the list.
        /// </summary>
        List<Table_11_Globally_Calibrated_Model_Parameters_Data> Data { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds and returns a single instance of a parameter given its tillage type used. Tillage type is needed as multiple instances of a given parameter can exist.
        /// </summary>
        /// <param name="parameter">The parameter whose instance we need to find.</param>
        /// <param name="tillageType">The tillage type for that given parameter. Either Conventional, No-Till, Reduced or All.</param>
        /// <returns>Returns a single data instance of Table_11_Globally_Calibrated_Model_Parameters_Data that corresponds to a parameter in the data file.</returns>
        public Table_11_Globally_Calibrated_Model_Parameters_Data GetGloballyCalibratedModelParametersInstance(ModelParameters parameter, TillageType tillageType)
        {
            Table_11_Globally_Calibrated_Model_Parameters_Data data = this.Data.Find(x => (x.Parameter == parameter) && (x.TillageType == tillageType));

            if (data != null)
            {
                return data;
            }

            data = this.Data.Find(x => x.Parameter == parameter);

            if (data != null)
            {
                Trace.TraceError($"{nameof(Table_11_Globally_Calibrated_Model_Parameters_Provider)}.{nameof(Table_11_Globally_Calibrated_Model_Parameters_Provider.GetGloballyCalibratedModelParametersInstance)}" +
                    $"could not find Tillage Type: {tillageType} in the specified data. Returning null");
            }
            else
            {
                Trace.TraceError($"{nameof(Table_11_Globally_Calibrated_Model_Parameters_Provider)}.{nameof(Table_11_Globally_Calibrated_Model_Parameters_Provider.GetGloballyCalibratedModelParametersInstance)}" +
                    $" could not find Model Parameter: {parameter} in the specified data. Returning null");
            }

            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the csv data file and returns a List of instances corresponding to each line in the file.
        /// </summary>
        /// <returns>A list containing instances of Table_11_Globally_Calibrated_Model_Parameters_Data. Each instance is a single line in the csv file</returns>
        private List<Table_11_Globally_Calibrated_Model_Parameters_Data> ReadFile()
        {
            var results = new List<Table_11_Globally_Calibrated_Model_Parameters_Data>();
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;

            IEnumerable<string[]> fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.CalibratedModelParameters);

            foreach (string[] line in fileLines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line[0]))
                {
                    Trace.Write($"{nameof(Table_11_Globally_Calibrated_Model_Parameters_Provider)}.{nameof(ReadFile)}" +
                                     $" : {nameof(CsvResourceNames.CalibratedModelParameters)} : first cell of the line is empty. Exiting loop to stop reading more lines inside .csv file.");
                    break;
                }

                ModelParameters modelParameter = _modelParameterStringConverter.Convert(line[0]);
                TillageType practice = _tillageTypeStringConverter.Convert(line[1]);
                var value = double.Parse(line[2], cultureInfo);
                var minValue = double.Parse(line[3], cultureInfo);
                var maxValue = double.Parse(line[4], cultureInfo);
                var standardDeviation = double.Parse(line[5], cultureInfo);
                var description = line[6];


                results.Add(new Table_11_Globally_Calibrated_Model_Parameters_Data 
                {
                    Parameter = modelParameter,
                    TillageType = practice,
                    Value = value,
                    MinValue = minValue,
                    MaxValue = maxValue,
                    StandardDeviation = standardDeviation,
                    Description = description,
                });      
            }

            return results;
        }

        #endregion
    }
}
