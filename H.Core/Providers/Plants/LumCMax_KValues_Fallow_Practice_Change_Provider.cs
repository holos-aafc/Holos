using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Plants
{
    /// <summary>
    ///     Table 4. LumCmax and k values for fallow practice change.
    /// </summary>
    public class LumCMax_KValues_Fallow_Practice_Change_Provider
    {
        #region Constructors

        public LumCMax_KValues_Fallow_Practice_Change_Provider()
        {
            HTraceListener.AddTraceListener();
            Data = ReadData();
        }

        #endregion

        #region Properties

        private List<LumCMax_KValues_Fallow_Practice_Change_Data> Data { get; }

        #endregion

        #region Private Methods

        private List<LumCMax_KValues_Fallow_Practice_Change_Data> ReadData()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.LumCMaxAndKValuesForFallowPracticeChange;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<LumCMax_KValues_Fallow_Practice_Change_Data>();

            foreach (var line in filelines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line[0])) break;

                var entry = new LumCMax_KValues_Fallow_Practice_Change_Data();
                var ecozone = _ecozoneStringConverter.Convert(line[0]);
                var soilTexture = _soilTextureStringConverter.Convert(line[1]);
                var fallowPracticeChange = _fallowPracticeChangeTypeStringConverter.Convert(line[2]);
                var lumCMax = double.Parse(line[3], cultureInfo);
                var kValue = double.Parse(line[4], cultureInfo);

                entry.Ecozone = ecozone;
                entry.SoilTexture = soilTexture;
                entry.FallowPracticeChangeType = fallowPracticeChange;
                entry.LumCMax = lumCMax;
                entry.kValue = kValue;

                result.Add(entry);
            }

            return result;
        }

        #endregion

        #region Fields

        private readonly EcozoneStringConverter _ecozoneStringConverter = new EcozoneStringConverter();
        private readonly SoilTextureStringConverter _soilTextureStringConverter = new SoilTextureStringConverter();

        private readonly FallowPracticeChangeTypeStringConverter _fallowPracticeChangeTypeStringConverter =
            new FallowPracticeChangeTypeStringConverter();

        #endregion

        #region Public Methods

        public double GetLumCMax(Ecozone ecozone, SoilTexture soilTexture,
            FallowPracticeChangeType fallowPracticeChangeType)
        {
            const double defaultValue = 0;
            var result = Data.SingleOrDefault(x =>
                x.Ecozone == ecozone && x.SoilTexture == soilTexture &&
                x.FallowPracticeChangeType == fallowPracticeChangeType);
            if (result != null) return result.LumCMax;

            Trace.TraceError(
                $"{nameof(GetLumCMax)} unable to get value for {ecozone.GetDescription()}, {soilTexture.GetDescription()}, and {fallowPracticeChangeType.GetDescription()}. Returning default value of {defaultValue}.");

            return defaultValue;
        }

        public double GetKValue(Ecozone ecozone, SoilTexture soilTexture,
            FallowPracticeChangeType fallowPracticeChangeType)
        {
            const double defaultValue = 0;
            var result = Data.SingleOrDefault(x =>
                x.Ecozone == ecozone && x.SoilTexture == soilTexture &&
                x.FallowPracticeChangeType == fallowPracticeChangeType);
            if (result != null) return result.kValue;

            Trace.TraceError(
                $"{nameof(GetKValue)} unable to get value for {ecozone.GetDescription()}, {soilTexture.GetDescription()}, and {fallowPracticeChangeType.GetDescription()}. Returning default value of {defaultValue}.");

            return defaultValue;
        }

        #endregion
    }
}