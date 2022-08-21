using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Models.Animals.Sheep;
using H.Core.Providers.Plants;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 25. Livestock coefficients for sheep.
    /// </summary>
    public class Table_25_Livestock_Coefficients_Sheep_Provider : ISheepCoefficientDataProvider
    {
        #region Fields

        private readonly AnimalTypeStringConverter _converter = new AnimalTypeStringConverter();
        private readonly List<Table_25_Livestock_Coefficients_Sheep_Data> _cache;

        #endregion

        #region Constructors

        public Table_25_Livestock_Coefficients_Sheep_Provider()
        {
            HTraceListener.AddTraceListener();
            _cache = BuildCache();
        }

        #endregion

        #region Public Methods

        public AnimalCoefficientData GetCoefficientsByAnimalType(AnimalType animalType)
        {
            var lookupType = animalType;
            if (animalType == AnimalType.SheepFeedlot)
            {
                lookupType = AnimalType.Ram;
            }

            var result = this.GetSheepCoefficients().SingleOrDefault(x => x.AnimalType == lookupType);
            if (result == null)
            {
                var defaultValue = this.GetSheepCoefficients().Single(x => x.AnimalType == AnimalType.Lambs);
                Trace.TraceError($"{nameof(Table_25_Livestock_Coefficients_Sheep_Provider)}.{nameof(Table_25_Livestock_Coefficients_Sheep_Provider.GetCoefficientsByAnimalType)}" +
                    $" unable to get data for animal type: {animalType}." +
                    $" Returning default value of {defaultValue}.");

                return defaultValue;
            }

            return result;
        }
        #endregion

        #region Private Methods

        private List<Table_25_Livestock_Coefficients_Sheep_Data> GetSheepCoefficients()
        {
            return _cache;
        }

        private List<Table_25_Livestock_Coefficients_Sheep_Data> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.SheepCoefficients;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<Table_25_Livestock_Coefficients_Sheep_Data>();
            foreach (var line in filelines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line[0]))
                {
                    Trace.Write($"{nameof(Table_25_Livestock_Coefficients_Sheep_Provider)}.{nameof(BuildCache)}" +
                                $" - File: {nameof(CsvResourceNames.SheepCoefficients)} : first cell of the line is empty. Exiting loop to stop reading more lines inside .csv file.");
                    break;
                }

                var entry = new Table_25_Livestock_Coefficients_Sheep_Data();
                var sheepGroup = _converter.Convert(line[0]);
                var maintenanceCoefficient = double.Parse(line[1].ParseUntilOrDefault(), cultureInfo);
                var coefficientA = double.Parse(line[2].ParseUntilOrDefault(), cultureInfo);
                var coefficientB = double.Parse(line[3].ParseUntilOrDefault(), cultureInfo);
                var initialWeight = double.Parse(line[4].ParseUntilOrDefault(), cultureInfo);
                var finalWeight = double.Parse(line[5].ParseUntilOrDefault(), cultureInfo);
                var woolProduction = double.Parse(line[6].ParseUntilOrDefault(), cultureInfo);
                entry.AnimalType = sheepGroup;
                entry.BaselineMaintenanceCoefficient = maintenanceCoefficient;
                entry.CoefficientA = coefficientA;
                entry.CoefficientB = coefficientB;
                entry.DefaultInitialWeight = initialWeight;
                entry.DefaultFinalWeight = finalWeight;
                entry.WoolProduction = woolProduction;
                result.Add(entry);
            }

            return result;
        }
        #endregion

    }
}