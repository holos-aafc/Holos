using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Markup;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Providers.Climate;
using H.Core.Providers.Plants;
using H.Infrastructure;
using Telerik.Windows.Documents.Model;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Implements two tables:
    /// <para>Table 21: Diet coefficients by livestock group and diet for beef cattle and dairy cattle.</para>
    /// <para>Table 29: Diet coefficients for sheep.</para>
    /// </summary>
    public class Table_21_29_Diet_Coefficients_Beef_Dairy_Sheep_Provider
    {
        #region Fields

        private readonly DietTypeStringConverter _dietConverter = new DietTypeStringConverter();
        private readonly AnimalTypeStringConverter _animalConverter = new AnimalTypeStringConverter();
        private readonly List<Table_21_29_Diet_Coefficients_Beef_Dairy_Sheep_Data> _fileData;

        #endregion

        #region Constructors

        public Table_21_29_Diet_Coefficients_Beef_Dairy_Sheep_Provider()
        {
            _fileData = BuildCache();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns an instance of <see cref="Table_21_29_Diet_Coefficients_Beef_Dairy_Sheep_Data"/> given an AnimalType and DietType.
        /// </summary>
        /// <param name="animalType">The type of animal for which data is needed.</param>
        /// <param name="dietType">The type of diet corresponding to the animal.</param>
        /// <returns>Returns a corresponding instance of <see cref="Table_21_29_Diet_Coefficients_Beef_Dairy_Sheep_Data"/> if data is found, otherwise
        /// returns null.</returns>
        public Table_21_29_Diet_Coefficients_Beef_Dairy_Sheep_Data GetDietCoefficientsDataInstance(AnimalType animalType, 
                                                                                                   DietType dietType)
        {
            Table_21_29_Diet_Coefficients_Beef_Dairy_Sheep_Data dataInstance = _fileData.FirstOrDefault(x => 
                                                                                                        x.AnimalType == animalType && 
                                                                                                        x.DietType == dietType);
            if (dataInstance != null)
            {
                return dataInstance;
            }
            
            dataInstance = _fileData.Find(x => x.AnimalType == animalType);

            if (dataInstance == null)
            {
                Trace.TraceError($"{nameof(Table_21_29_Diet_Coefficients_Beef_Dairy_Sheep_Provider)}.{nameof(GetDietCoefficientsDataInstance)}" +
                                 $" the AnimalType: {animalType} was not found in the available data. Returning null.");
            }
            else
            {
                Trace.TraceError($"{nameof(Table_21_29_Diet_Coefficients_Beef_Dairy_Sheep_Provider)}.{nameof(GetDietCoefficientsDataInstance)}" +
                                 $" the DietType: {dietType} was not found in the available data. Returning null.");
            }

            return null;
        }
        #endregion

        #region Private Methods
        private List<Table_21_29_Diet_Coefficients_Beef_Dairy_Sheep_Data> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.DietCoefficientsForDairyBeefSheep;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<Table_21_29_Diet_Coefficients_Beef_Dairy_Sheep_Data>();


            foreach (var line in filelines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line[0]))
                {
                    Trace.Write($"{nameof(Table_21_29_Diet_Coefficients_Beef_Dairy_Sheep_Provider)}.{nameof(BuildCache)}" +
                                $" - File: {nameof(CsvResourceNames.DietCoefficientsForDairyBeefSheep)} : first cell of the line is empty. Exiting loop to stop reading more lines inside .csv file.");
                    break;
                }

                var animalType = _animalConverter.Convert(line[0]);
                var dietType = _dietConverter.Convert(line[1]);
                var forage = double.Parse(line[2], cultureInfo);
                var crudeProteinContent = double.Parse(line[3], cultureInfo);
                var methaneConversionFactor = double.Parse(line[4], cultureInfo);
                var starch = double.Parse(line[5], cultureInfo);
                var ndf = double.Parse(line[6], cultureInfo);
                var adf = double.Parse(line[7], cultureInfo);
                var tdn = double.Parse(line[8], cultureInfo);
                var me = double.Parse(line[9], cultureInfo);
                var ee = double.Parse(line[10], cultureInfo);
                var nel3X = double.Parse(line[11], cultureInfo);
                

                var entry = new Table_21_29_Diet_Coefficients_Beef_Dairy_Sheep_Data()
                {
                    AnimalType = animalType,
                    DietType = dietType,
                    ForageContent = forage,
                    CrudeProteinContent = crudeProteinContent,
                    MethaneConversionFactor = methaneConversionFactor,
                    StarchContent = starch,
                    NeutralDetergentFiber = ndf,
                    AcidDetergentFiber = adf,
                    TotalDigestibleNutrients = tdn,
                    MetabolizableEnergy = me,
                    EtherExtract = ee,
                    NetEnergyLactation = nel3X,
                };
                result.Add(entry);

            }

            return result;
        }
        #endregion
    }
}