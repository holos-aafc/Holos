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
    /// Table 3
    /// </summary>
    public class LumCMaxAndkValueForPerennialsAndGrasslandProvider_Table_3
    {
        #region Fields

        private readonly EcozoneStringConverter _ecozoneStringConverter = new EcozoneStringConverter();
        private readonly SoilTextureStringConverter _soilTextureStringConverter = new SoilTextureStringConverter();
        private readonly PerennialCroppingChangeTypeStringConverter _perennialCroppingChangeTypeStringConverter = new PerennialCroppingChangeTypeStringConverter();

        #endregion

        #region Constructors

        public LumCMaxAndkValueForPerennialsAndGrasslandProvider_Table_3()
        {
            HTraceListener.AddTraceListener();
            this.Data = this.ReadData();
        }

        #endregion

        #region Properties

        private List<LumCMaxAndkValuesForPerennialCroppingChangeData> Data { get; set; }

        #endregion

        #region Public Methods

        public double GetLumCMax(Ecozone ecozone, SoilTexture soilTexture, PerennialCroppingChangeType changeType)
        {
            const double defaultValue = 0;
            var result = this.Data.SingleOrDefault(x => x.Ecozone == ecozone && x.SoilTexture == soilTexture && x.PerennialCroppingChangeType == changeType);
            if (result != null)
            {
                return result.LumCMax;
            }
            else
            {
                Trace.TraceError($"{nameof(LumCMaxAndkValueForPerennialsAndGrasslandProvider_Table_3.GetLumCMax)} unable to get value for {ecozone.GetDescription()}, {soilTexture.GetDescription()}, and {changeType.GetDescription()}. Returning default value of {defaultValue}.");

                return defaultValue;
            }
        }

        public double GetKValue(Ecozone ecozone, SoilTexture soilTexture, PerennialCroppingChangeType changeType)
        {
            const double defaultValue = 0;
            var result = this.Data.SingleOrDefault(x => x.Ecozone == ecozone && x.SoilTexture == soilTexture && x.PerennialCroppingChangeType == changeType);
            if (result != null)
            {
                return result.kValue;
            }
            else
            {
                Trace.TraceError($"{nameof(LumCMaxAndkValueForPerennialsAndGrasslandProvider_Table_3.GetKValue)} unable to get value for {ecozone.GetDescription()}, {soilTexture.GetDescription()}, and {changeType.GetDescription()}. Returning default value of {defaultValue}.");

                return defaultValue;
            }
        }

        #endregion

        #region Private Methods

        private List<LumCMaxAndkValuesForPerennialCroppingChangeData> ReadData()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.LumCMaxAndKValuesForPerennialCroppingChange;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<LumCMaxAndkValuesForPerennialCroppingChangeData>();

            foreach (var line in filelines.Skip(1))
            {
                var entry = new LumCMaxAndkValuesForPerennialCroppingChangeData();
                var ecozone = _ecozoneStringConverter.Convert(line[0]);
                var soilTexture = _soilTextureStringConverter.Convert(line[1]);
                var perennialCroppingChange = _perennialCroppingChangeTypeStringConverter.Convert(line[2]);
                var lumCMax = double.Parse(line[3], cultureInfo);
                var kValue = double.Parse(line[4], cultureInfo);

                entry.Ecozone = ecozone;
                entry.SoilTexture = soilTexture;
                entry.PerennialCroppingChangeType = perennialCroppingChange;
                entry.LumCMax = lumCMax;
                entry.kValue = kValue;
                result.Add(entry);
            }
            return result;
        } 

        #endregion
    }
}
