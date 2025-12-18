using System.Collections.Generic;
using H.Core.Enumerations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>         
    /// Table 6
    /// 
    /// Manure types and default composition included in the Holos model
    /// </summary>
    public class Table_6_Manure_Types_Default_Composition_Provider
    {
        #region Fields

        private readonly AnimalTypeStringConverter _animalTypeStringConverter;
        private readonly ManureStateTypeStringConverter _manureStateTypeStringConverter; 

        #endregion

        #region Constructors

        public Table_6_Manure_Types_Default_Composition_Provider()
        {
            HTraceListener.AddTraceListener();

            _animalTypeStringConverter = new AnimalTypeStringConverter();
            _manureStateTypeStringConverter = new ManureStateTypeStringConverter();

            this.ManureCompositionData = new List<DefaultManureCompositionData>();
            this.ManureCompositionData = ReadFile();
        }

        #endregion

        #region Properties

        public List<DefaultManureCompositionData> ManureCompositionData { get; }

        #endregion

        #region Public Methods

        public DefaultManureCompositionData GetManureCompositionDataByType(AnimalType animalType, ManureStateType manureStateType)
        {
            var data = ManureCompositionData.Find(x =>
                                                x.AnimalType == animalType &&
                                                x.ManureStateType == manureStateType);

            if (data != null)
            {
                return data;
            }

            data = ManureCompositionData.Find(x => x.AnimalType == animalType);

            if (data != null)
            {
                Trace.TraceError($"{nameof(Table_6_Manure_Types_Default_Composition_Provider)}.{nameof(GetManureCompositionDataByType)} " +
                                 $"could not find ManureStateType: {manureStateType} for AnimalType: {animalType} in the csv file. Returning 0 for all values.");
            }
            else
            {
                Trace.TraceError($"{nameof(Table_6_Manure_Types_Default_Composition_Provider)}.{nameof(GetManureCompositionDataByType)} " +
                                 $"could not find AnimalType: {animalType} in the csv file. Returning 0 for all values.");
            }

            return new DefaultManureCompositionData();
        }

        #endregion

        #region Private Methods
        private List<DefaultManureCompositionData> ReadFile()
        {
            var fileData = new List<DefaultManureCompositionData>();
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.ManureTypesDefaultComposition);
            var numberStyle = NumberStyles.Float;
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;

            foreach (var line in fileLines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line[0]))
                {
                    break;
                }

                var cellValue = 0d;

                AnimalType animalType = _animalTypeStringConverter.Convert(line[0].ParseUntilOrDefault());
                ManureStateType manureStateType = _manureStateTypeStringConverter.Convert(line[1].ParseUntilOrDefault());
                var moistureContent = double.TryParse(line[2].ParseUntilOrDefault(), numberStyle, cultureInfo, out cellValue) ? cellValue : CoreConstants.ValueNotDetermined;
                var nfraction = double.TryParse(line[3].ParseUntilOrDefault(), numberStyle, cultureInfo, out cellValue) ? cellValue : CoreConstants.ValueNotDetermined;
                var cfraction = double.TryParse(line[4].ParseUntilOrDefault(), numberStyle, cultureInfo, out cellValue) ? cellValue : CoreConstants.ValueNotDetermined;
                var pfraction = double.TryParse(line[5].ParseUntilOrDefault(), numberStyle, cultureInfo, out cellValue) ? cellValue : CoreConstants.ValueNotDetermined;
                var cToNRatio = double.TryParse(line[6].ParseUntilOrDefault(), numberStyle, cultureInfo, out cellValue) ? cellValue : CoreConstants.ValueNotDetermined;
                var volatileSolidsContent = double.TryParse(line[7].ParseUntilOrDefault(), numberStyle, cultureInfo, out cellValue) ? cellValue : CoreConstants.ValueNotDetermined;

                fileData.Add(new DefaultManureCompositionData
                {
                    AnimalType = animalType,
                    ManureStateType = manureStateType,
                    MoistureContent = moistureContent,
                    NitrogenFraction = nfraction,   // Note this value is a percentage in the file
                    CarbonFraction = cfraction,     // Note this value is a percentage in the file
                    PhosphorusFraction = pfraction, // Note this value is a percentage in the file
                    CarbonToNitrogenRatio = cToNRatio,
                    VolatileSolidsFraction = volatileSolidsContent, // Note this value is a percentage in the file
                });
            }

            return fileData;
        }

        #endregion
    }
}