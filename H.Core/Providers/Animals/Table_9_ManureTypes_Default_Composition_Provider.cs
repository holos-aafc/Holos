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
    /// Table 9
    /// 
    /// Manure types and default composition included in the Holos model
    /// </summary>
    public class Table_9_ManureTypes_Default_Composition_Provider
    {
        #region Fields

        private readonly AnimalTypeStringConverter _animalTypeStringConverter;
        private readonly ManureStateTypeStringConverter _manureStateTypeStringConverter; 

        #endregion

        #region Constructors

        public Table_9_ManureTypes_Default_Composition_Provider()
        {
            HTraceListener.AddTraceListener();

            _animalTypeStringConverter = new AnimalTypeStringConverter();
            _manureStateTypeStringConverter = new ManureStateTypeStringConverter();

            ManureCompositionData = ReadFile();

            #region Manually Reading Provider
            //#region Beef

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.Pasture,
            //    AnimalType = AnimalType.Beef,
            //    MoistureContent = 50,
            //    NitrogenFraction = 1,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = 0.24,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.DeepBedding,
            //    AnimalType = AnimalType.Beef,
            //    MoistureContent = 60.08,
            //    NitrogenFraction = 0.715,
            //    CarbonFraction = 12.63,
            //    PhosphorusFraction = 0.236,
            //    CarbonToNitrogenRatio = 18.79,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.SolidStorage,
            //    AnimalType = AnimalType.Beef,
            //    MoistureContent = 60.43,
            //    NitrogenFraction = 0.722,
            //    CarbonFraction = 8.58,
            //    PhosphorusFraction = 0.254,
            //    CarbonToNitrogenRatio = 16.9,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.CompostPassive,
            //    AnimalType = AnimalType.Beef,
            //    MoistureContent = 62.35,
            //    NitrogenFraction = 0.659,
            //    CarbonFraction = 9.16,
            //    PhosphorusFraction = 0.255,
            //    CarbonToNitrogenRatio = 14.52,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.CompostIntensive,
            //    AnimalType = AnimalType.Beef,
            //    MoistureContent = 36.54,
            //    NitrogenFraction = 1.041,
            //    CarbonFraction = 14.48,
            //    PhosphorusFraction = 0.398,
            //    CarbonToNitrogenRatio = 14.23,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.Liquid,
            //    AnimalType = AnimalType.Beef,
            //    MoistureContent = 89.9,
            //    NitrogenFraction = 0.37,
            //    CarbonFraction = 0,
            //    PhosphorusFraction = 0.080,
            //    CarbonToNitrogenRatio = 21.5,
            //});

            //#endregion

            //#region Dairy

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.Pasture,
            //    AnimalType = AnimalType.Dairy,
            //    MoistureContent = 50,
            //    NitrogenFraction = 1,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = 0.24,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.DeepBedding,
            //    AnimalType = AnimalType.Dairy,
            //    MoistureContent = 60.08,
            //    NitrogenFraction = 0.715,
            //    CarbonFraction = 12.63,
            //    PhosphorusFraction = 0.236,
            //    CarbonToNitrogenRatio = 18.79,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.SolidStorage,
            //    AnimalType = AnimalType.Dairy,
            //    MoistureContent = 77.3,
            //    NitrogenFraction = 0.392,
            //    CarbonFraction = 2.99,
            //    PhosphorusFraction = 0.118,
            //    CarbonToNitrogenRatio = 21.95,
            //    NitrogenConcentrationOfManure = 5,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.Composted,
            //    AnimalType = AnimalType.Dairy,
            //    MoistureContent = 78.11,
            //    NitrogenFraction = 0.265,
            //    CarbonFraction = 4.72,
            //    PhosphorusFraction = CoreConstants.ValueNotDetermined,
            //    CarbonToNitrogenRatio = 17.11,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.Slurry,
            //    AnimalType = AnimalType.Dairy,
            //    MoistureContent = 94.41,
            //    NitrogenFraction = 0.209,
            //    CarbonFraction = 2.19,
            //    PhosphorusFraction = 0.06,
            //    CarbonToNitrogenRatio = 10.58,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.LiquidSeparated,
            //    AnimalType = AnimalType.Dairy,
            //    MoistureContent = 95.5,
            //    NitrogenFraction = 0.013,
            //    CarbonFraction = 0.125,
            //    PhosphorusFraction = 0.002,
            //    CarbonToNitrogenRatio = 6.53,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.AnaerobicDigester,
            //    AnimalType = AnimalType.Dairy,
            //    MoistureContent = 95,
            //    NitrogenFraction = 0.233,
            //    CarbonFraction = 2.39,
            //    PhosphorusFraction = CoreConstants.ValueNotDetermined,
            //    CarbonToNitrogenRatio = 10.42,
            //});

            //#endregion

            //#region Swine

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.CompostIntensive,
            //    AnimalType = AnimalType.Swine,
            //    MoistureContent = 79.23,
            //    NitrogenFraction = 0.279,
            //    CarbonFraction = 9.13,
            //    PhosphorusFraction = CoreConstants.ValueNotDetermined,
            //    CarbonToNitrogenRatio = 30.88,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.CompostPassive,
            //    AnimalType = AnimalType.Swine,
            //    MoistureContent = 79.23,
            //    NitrogenFraction = 0.279,
            //    CarbonFraction = 9.13,
            //    PhosphorusFraction = CoreConstants.ValueNotDetermined,
            //    CarbonToNitrogenRatio = 30.88,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.DeepPit,
            //    AnimalType = AnimalType.Swine,
            //    MoistureContent = CoreConstants.ValueNotDetermined,
            //    NitrogenFraction = CoreConstants.ValueNotDetermined,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = CoreConstants.ValueNotDetermined,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.LiquidWithNaturalCrust,
            //    AnimalType = AnimalType.Swine,
            //    MoistureContent = 95.16,
            //    NitrogenFraction = 0.325,
            //    CarbonFraction = 1.29,
            //    PhosphorusFraction = 0.118,
            //    CarbonToNitrogenRatio = 3.25,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.LiquidNoCrust,
            //    AnimalType = AnimalType.Swine,
            //    MoistureContent = 95.16,
            //    NitrogenFraction = 0.325,
            //    CarbonFraction = 1.29,
            //    PhosphorusFraction = 0.118,
            //    CarbonToNitrogenRatio = 3.25,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.LiquidWithSolidCover,
            //    AnimalType = AnimalType.Swine,
            //    MoistureContent = 95.16,
            //    NitrogenFraction = 0.325,
            //    CarbonFraction = 1.29,
            //    PhosphorusFraction = 0.118,
            //    CarbonToNitrogenRatio = 3.25,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.AnaerobicDigester,
            //    AnimalType = AnimalType.Swine,
            //    MoistureContent = CoreConstants.ValueNotDetermined,
            //    NitrogenFraction = CoreConstants.ValueNotDetermined,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = CoreConstants.ValueNotDetermined,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //#endregion

            //#region Poultry

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.SolidStorage,
            //    AnimalType = AnimalType.Poultry,
            //    MoistureContent = 44.83,
            //    NitrogenFraction = 2.427,
            //    CarbonFraction = 10.12,
            //    PhosphorusFraction = 1.06,
            //    CarbonToNitrogenRatio = 4.36,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.Liquid,
            //    AnimalType = AnimalType.Poultry,
            //    MoistureContent = 89,
            //    NitrogenFraction = 8.95,
            //    CarbonFraction = 2.92,
            //    PhosphorusFraction = 0.26,
            //    CarbonToNitrogenRatio = 3.27,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.Slurry,
            //    AnimalType = AnimalType.Poultry,
            //    MoistureContent = 89,
            //    NitrogenFraction = 0.895,
            //    CarbonFraction = 2.92,
            //    PhosphorusFraction = 0.26,
            //    CarbonToNitrogenRatio = 3.57,
            //});

            //#endregion

            //#region Sheep

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.Pasture,
            //    AnimalType = AnimalType.Sheep,
            //    MoistureContent = 25,
            //    NitrogenFraction = 1,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = 0.231,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.SolidStorage,
            //    AnimalType = AnimalType.Sheep,
            //    MoistureContent = 67.8,
            //    NitrogenFraction = 0.87,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = 0.34,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //    NitrogenConcentrationOfManure = 10,
            //});

            //#endregion

            //#region Other Animals

            //// Pasture
            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.Pasture,
            //    AnimalType = AnimalType.Llamas,
            //    MoistureContent = 25,
            //    NitrogenFraction = CoreConstants.ValueNotDetermined,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = 0.231,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.Pasture,
            //    AnimalType = AnimalType.Alpacas,
            //    MoistureContent = 25,
            //    NitrogenFraction = CoreConstants.ValueNotDetermined,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = 0.231,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.Pasture,
            //    AnimalType = AnimalType.Deer,
            //    MoistureContent = CoreConstants.ValueNotDetermined,
            //    NitrogenFraction = CoreConstants.ValueNotDetermined,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = CoreConstants.ValueNotDetermined,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.Pasture,
            //    AnimalType = AnimalType.Elk,
            //    MoistureContent = CoreConstants.ValueNotDetermined,
            //    NitrogenFraction = CoreConstants.ValueNotDetermined,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = CoreConstants.ValueNotDetermined,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.Pasture,
            //    AnimalType = AnimalType.Goats,
            //    MoistureContent = CoreConstants.ValueNotDetermined,
            //    NitrogenFraction = 0.63,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = CoreConstants.ValueNotDetermined,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.Pasture,
            //    AnimalType = AnimalType.Horses,
            //    MoistureContent = 75,
            //    NitrogenFraction = 0.6,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = 0.131,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.Pasture,
            //    AnimalType = AnimalType.Mules,
            //    MoistureContent = 75,
            //    NitrogenFraction = 0.6,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = 0.131,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.Pasture,
            //    AnimalType = AnimalType.Bison,
            //    MoistureContent = CoreConstants.ValueNotDetermined,
            //    NitrogenFraction = CoreConstants.ValueNotDetermined,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = CoreConstants.ValueNotDetermined,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //// Solid storage
            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{
            //    ManureStateType = ManureStateType.SolidStorage,
            //    AnimalType = AnimalType.Llamas,
            //    MoistureContent = 67.8,
            //    NitrogenFraction = 0.87,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = 0.34,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.SolidStorage,
            //    AnimalType = AnimalType.Alpacas,
            //    MoistureContent = 67.8,
            //    NitrogenFraction = 0.87,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = 0.34,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.SolidStorage,
            //    AnimalType = AnimalType.Deer,
            //    MoistureContent = 35,
            //    NitrogenFraction = 0.65,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = 0.21,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.SolidStorage,
            //    AnimalType = AnimalType.Elk,
            //    MoistureContent = 35,
            //    NitrogenFraction = 0.65,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = 0.21,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.SolidStorage,
            //    AnimalType = AnimalType.Goats,
            //    MoistureContent = 64.3,
            //    NitrogenFraction = 0.104,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = 0.28,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.SolidStorage,
            //    AnimalType = AnimalType.Horses,
            //    MoistureContent = 62.6,
            //    NitrogenFraction = 0.5,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = 0.15,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.SolidStorage,
            //    AnimalType = AnimalType.Mules,
            //    MoistureContent = 62.6,
            //    NitrogenFraction = 0.5,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = 0.15,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //this.ManureCompositionData.Add(new DefaultManureCompositionData()
            //{

            //    ManureStateType = ManureStateType.SolidStorage,
            //    AnimalType = AnimalType.Bison,
            //    MoistureContent = 35,
            //    NitrogenFraction = 0.65,
            //    CarbonFraction = CoreConstants.ValueNotDetermined,
            //    PhosphorusFraction = 0.21,
            //    CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            //});

            //#endregion

            #endregion
        }
        #endregion

        #region Properties

        public List<DefaultManureCompositionData> ManureCompositionData { get; } = new List<DefaultManureCompositionData>();

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
                Trace.TraceError($"{nameof(Table_9_ManureTypes_Default_Composition_Provider)}.{nameof(GetManureCompositionDataByType)} " +
                                 $"could not find ManureStateType: {manureStateType} for AnimalType: {animalType} in the csv file. Returning null.");
            }
            else
            {
                Trace.TraceError($"{nameof(Table_9_ManureTypes_Default_Composition_Provider)}.{nameof(GetManureCompositionDataByType)} " +
                                 $"could not find AnimalType: {animalType} in the csv file. Returning null.");
            }

            return null;
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

                fileData.Add(new DefaultManureCompositionData
                {
                    AnimalType = animalType,
                    ManureStateType = manureStateType,
                    MoistureContent = moistureContent,
                    NitrogenFraction = nfraction,
                    CarbonFraction = cfraction,
                    PhosphorusFraction = pfraction,
                    CarbonToNitrogenRatio = cToNRatio,
                });
            }

            return fileData;
        }

        #endregion
    }
}