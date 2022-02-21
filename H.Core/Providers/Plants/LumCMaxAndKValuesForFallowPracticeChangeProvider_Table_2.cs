using H.Content;
using H.Core.Converters;
using H.Infrastructure;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Tools;

namespace H.Core.Providers.Plants
{
    /// <summary>
    /// Table 2
    /// </summary>
    public class LumCMaxAndKValuesForFallowPracticeChangeProvider_Table_2
    {
        #region Fields

        private readonly EcozoneStringConverter _ecozoneStringConverter = new EcozoneStringConverter();
        private readonly SoilTextureStringConverter _soilTextureStringConverter = new SoilTextureStringConverter();
        private readonly FallowPracticeChangeTypeStringConverter _fallowPracticeChangeTypeStringConverter = new FallowPracticeChangeTypeStringConverter();

        #endregion

        #region Constructors

        public LumCMaxAndKValuesForFallowPracticeChangeProvider_Table_2()
        {
            HTraceListener.AddTraceListener();
            this.Data = this.ReadData();
        }

        #endregion

        #region Properties

        private List<LumCMaxAndKValuesForFallowPracticeChangeData> Data { get; set; }

        #endregion

        #region Public Methods

        public double GetLumCMax(Ecozone ecozone, SoilTexture soilTexture, FallowPracticeChangeType fallowPracticeChangeType)
        {
            const double defaultValue = 0;
            var result = this.Data.SingleOrDefault(x => x.Ecozone == ecozone && x.SoilTexture == soilTexture && x.FallowPracticeChangeType == fallowPracticeChangeType);
            if (result != null)
            {
                return result.LumCMax;
            }
            else
            {
                Trace.TraceError($"{nameof(LumCMaxAndKValuesForFallowPracticeChangeProvider_Table_2.GetLumCMax)} unable to get value for {ecozone.GetDescription()}, {soilTexture.GetDescription()}, and {fallowPracticeChangeType.GetDescription()}. Returning default value of {defaultValue}.");

                return defaultValue;
            }
        }

        public double GetKValue(Ecozone ecozone, SoilTexture soilTexture, FallowPracticeChangeType fallowPracticeChangeType)
        {
            const double defaultValue = 0;
            var result = this.Data.SingleOrDefault(x => x.Ecozone == ecozone && x.SoilTexture == soilTexture && x.FallowPracticeChangeType == fallowPracticeChangeType);
            if (result != null)
            {
                return result.kValue;
            }
            else
            {
                Trace.TraceError($"{nameof(LumCMaxAndKValuesForFallowPracticeChangeProvider_Table_2.GetKValue)} unable to get value for {ecozone.GetDescription()}, {soilTexture.GetDescription()}, and {fallowPracticeChangeType.GetDescription()}. Returning default value of {defaultValue}.");

                return defaultValue;
            }
        }

        #endregion

        #region Private Methods

        private List<LumCMaxAndKValuesForFallowPracticeChangeData> ReadData()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.LumCMaxAndKValuesForFallowPracticeChange;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<LumCMaxAndKValuesForFallowPracticeChangeData>();

            foreach (var line in filelines.Skip(1))
            {
                var entry = new LumCMaxAndKValuesForFallowPracticeChangeData();
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
    }
}
