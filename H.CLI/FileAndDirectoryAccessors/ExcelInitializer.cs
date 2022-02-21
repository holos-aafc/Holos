using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using H.CLI.UserInput;
using H.Content;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.CLI.FileAndDirectoryAccessors
{
    public class ExcelInitializer
    {
        #region Fields
        private KeyConverter.KeyConverter _converter = new KeyConverter.KeyConverter();
        private UnitsOfMeasurementCalculator unitsOfMeasurementCalculator = new UnitsOfMeasurementCalculator();
        #endregion

        #region Constructors
        public ExcelInitializer() { }
        #endregion

        #region Public Methods
        /// <summary>
        /// Takes in a filePath that corresponds with each component file in a Component's Directory (ie, Shelterbelt1.csv, Shelterbelt2.csv in
        /// the Shelterbelts directory). The file is split into lines and read to the end and the result is stored in a list of string[]
        /// </summary>
        public IEnumerable<string[]> ReadExcelFile(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                        var filelines = CsvResourceReader.SplitFileIntoLines(reader.ReadToEnd(), CLILanguageConstants.Delimiter[0]);
                        return filelines;
                }
            }
        }

        /// <summary>
        /// Takes in the path to the component's directory that does not have a template file and creates a new csv file based
        /// on the component's keys.
        /// </summary>

        public void SetTemplateFile(string componentDirectoryPath, string componentType, Dictionary<string, ImperialUnitsOfMeasurement?> componentKeys)
        {
            string fileName = componentType + "_Example";
            string filePath = componentDirectoryPath + @"\" + fileName + CLILanguageConstants.OutputLanguageAddOn;
            var stringBuilder = new StringBuilder();
            foreach (var key in componentKeys)
            {
                string convertedKey;
                if (CLIUnitsOfMeasurementConstants.measurementSystem == MeasurementSystemType.Metric && key.Value != null)
                {
                    convertedKey = key.Key + "(" + unitsOfMeasurementCalculator.GetMetricUnitsOfMeasurementString(key.Value.Value) + ")";
                    stringBuilder.Append(convertedKey + CLILanguageConstants.Delimiter);
                }

                if (CLIUnitsOfMeasurementConstants.measurementSystem == MeasurementSystemType.Imperial && key.Value != null)
                {
                    convertedKey = key.Key + "(" + key.Value.Value.GetDescription() + ")";
                    stringBuilder.Append(convertedKey + CLILanguageConstants.Delimiter);
                }

                else if (key.Value == null)
                {
                    //convertedKey = _converter.ConvertTemplateKey(key.Key);
                    stringBuilder.Append(key.Key + CLILanguageConstants.Delimiter);
                }
            }

            File.WriteAllText(filePath, stringBuilder.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Used to create a template csv file that corresponds to a Shelterbelt for testing purposes that contains the headers for a Shelterbelt
        /// and two rows of corresponding data and writes it to the filePath. Creates a stringBuilder and appends each added key or element with a comma
        /// For example: "Hardiness Zone, Ecodistrict Id, Year Of Observation, ... ,". After the headers (keys) have been built, then we append a new line and
        /// create another comma delimited string for the second row in our file.
        /// </summary>
    
        public void SetTemplateCSVFileForTesting(string testPath, Dictionary<string, ImperialUnitsOfMeasurement?> componentKeys)
        {
            string filePath = testPath + @"\Shelterbelt1.csv";
            var stringBuilder = new StringBuilder();
            foreach (var keyValuePair in componentKeys)
            {
                var convertedKey = keyValuePair.Key.Trim();
                stringBuilder.Append(convertedKey + ",");
            }

            stringBuilder.Append(Environment.NewLine);

            string[] templateValues1 = new string[]
            {
               "H4b",
                "754",
               "1996",
               "Shelterbelt1",
               "Caragana",
               "1",
               "100",
               "1996",
               "2019",
               "Caragana",
               "138",
               "0",
               "113",
               "34.508"
            };


            foreach (var templateValue in templateValues1)
            {
                stringBuilder.Append(templateValue + ",");

            }

            string[] templateValues2 = new string[]
            {
               "H4b",
                "754",
               "1996",
               "Shelterbelt1",
               "Caragana",
               "2",
               "100",
               "1996",
               "2019",
               "Caragana",
               "138",
               "0",
               "113",
               "34.508"
            };

            stringBuilder.Append(Environment.NewLine);

            foreach (var templateValue in templateValues2)
            {
                stringBuilder.Append(templateValue + ",");
            }

            string[] templateValues3 = new string[]
            {
               "H4b",
                "754",
               "1996",
               "Shelterbelt1",
               "Caragana",
               "1",
               "100",
               "1996",
               "2019",
               "Caragana",
               "138",
               "0",
               "113",
               "34.508"
            };

            stringBuilder.Append(Environment.NewLine);

            foreach (var templateValue in templateValues3)
            {
                stringBuilder.Append(templateValue + ",");

            }

            File.WriteAllText(filePath, stringBuilder.ToString());
        } 
        #endregion

    }
}

