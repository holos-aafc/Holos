﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Soil
{
    /// <summary>
    ///     Table 8: Globally calibrated model parameters to be used to estimate SOC changes from mineral soils with the IPCC
    ///     (2019) Tier 2 steady-state method
    /// </summary>
    public class Table_8_Globally_Calibrated_Model_Parameters_Provider
    {
        #region Constructors

        /// <summary>
        ///     Initiializes the string converters and reads the csv data file.
        /// </summary>
        public Table_8_Globally_Calibrated_Model_Parameters_Provider()
        {
            _modelParameterStringConverter = new ModelParameterStringConverter();
            _tillageTypeStringConverter = new TillageTypeStringConverter();

            Data = ReadFile();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     List of Table_8_Globally_Calibrated_Model_Parameters_Data instances. Each parameter has its own data instance in
        ///     the list.
        /// </summary>
        private List<Table_8_Globally_Calibrated_Model_Parameters_Data> Data { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Finds and returns a single instance of a parameter given its tillage type used. Tillage type is needed as multiple
        ///     instances of a given parameter can exist.
        /// </summary>
        /// <param name="parameter">The parameter whose instance we need to find.</param>
        /// <param name="tillageType">The tillage type for that given parameter. Either Conventional, No-Till, Reduced or All.</param>
        /// <returns>
        ///     Returns a single data instance of Table_8_Globally_Calibrated_Model_Parameters_Data that corresponds to a
        ///     parameter in the data file.
        /// </returns>
        public Table_8_Globally_Calibrated_Model_Parameters_Data GetGloballyCalibratedModelParametersInstance(
            ModelParameters parameter, TillageType tillageType)
        {
            var data = Data.Find(x => x.Parameter == parameter && x.TillageType == tillageType);

            if (data != null) return data;

            data = Data.Find(x => x.Parameter == parameter);

            if (data != null)
                Trace.TraceError(
                    $"{nameof(Table_8_Globally_Calibrated_Model_Parameters_Provider)}.{nameof(GetGloballyCalibratedModelParametersInstance)}" +
                    $"could not find Tillage Type: {tillageType} in the specified data. Returning null");
            else
                Trace.TraceError(
                    $"{nameof(Table_8_Globally_Calibrated_Model_Parameters_Provider)}.{nameof(GetGloballyCalibratedModelParametersInstance)}" +
                    $" could not find Model Parameter: {parameter} in the specified data. Returning null");

            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Reads the csv data file and returns a List of instances corresponding to each line in the file.
        /// </summary>
        /// <returns>
        ///     A list containing instances of Table_8_Globally_Calibrated_Model_Parameters_Data. Each instance is a single
        ///     line in the csv file
        /// </returns>
        private List<Table_8_Globally_Calibrated_Model_Parameters_Data> ReadFile()
        {
            var results = new List<Table_8_Globally_Calibrated_Model_Parameters_Data>();
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;

            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.CalibratedModelParameters);

            foreach (var line in fileLines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line[0]))
                {
                    Trace.Write($"{nameof(Table_8_Globally_Calibrated_Model_Parameters_Provider)}.{nameof(ReadFile)}" +
                                $" : {nameof(CsvResourceNames.CalibratedModelParameters)} : first cell of the line is empty. Exiting loop to stop reading more lines inside .csv file.");
                    break;
                }

                var modelParameter = _modelParameterStringConverter.Convert(line[0]);
                var practice = _tillageTypeStringConverter.Convert(line[1]);
                var value = double.Parse(line[2], cultureInfo);
                var minValue = double.Parse(line[3], cultureInfo);
                var maxValue = double.Parse(line[4], cultureInfo);
                var standardDeviation = double.Parse(line[5], cultureInfo);
                var description = line[6];


                results.Add(new Table_8_Globally_Calibrated_Model_Parameters_Data
                {
                    Parameter = modelParameter,
                    TillageType = practice,
                    Value = value,
                    MinValue = minValue,
                    MaxValue = maxValue,
                    StandardDeviation = standardDeviation,
                    Description = description
                });
            }

            return results;
        }

        #endregion

        #region Fields

        private readonly ModelParameterStringConverter _modelParameterStringConverter;
        private readonly TillageTypeStringConverter _tillageTypeStringConverter;

        #endregion
    }
}