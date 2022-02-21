using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Models.Animals.Sheep;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 24
    /// </summary>
    public class SheepCoefficientsProvider_Table_24 : ISheepCoefficientDataProvider
    {
        #region Fields

        private readonly AnimalTypeStringConverter _converter = new AnimalTypeStringConverter();
        private readonly List<SheepCoefficientsData> _cache;

        #endregion

        #region Constructors

        public SheepCoefficientsProvider_Table_24()
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
                Trace.TraceError($"{nameof(SheepCoefficientsProvider_Table_24)}.{nameof(SheepCoefficientsProvider_Table_24.GetCoefficientsByAnimalType)}" +
                    $" unable to get data for animal type: {animalType}." +
                    $" Returning default value of {defaultValue}.");

                return defaultValue;
            }

            return result;
        }
        #endregion

        #region Private Methods

        private List<SheepCoefficientsData> GetSheepCoefficients()
        {
            return _cache;
        }

        private List<SheepCoefficientsData> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.SheepCoefficients;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<SheepCoefficientsData>();
            foreach (var line in filelines.Skip(1))
            {
                var entry = new SheepCoefficientsData();
                var sheepGroup = _converter.Convert(line[0]);
                var maintenanceCoefficient = double.Parse(line[1], cultureInfo);
                var coefficientA = double.Parse(line[2], cultureInfo);
                var coefficientB = double.Parse(line[3], cultureInfo);
                var initialWeight = double.Parse(line[4], cultureInfo);
                var finalWeight = double.Parse(line[5], cultureInfo);
                var woolProduction = double.Parse(line[6], cultureInfo);
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