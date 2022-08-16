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
    /// Table 5. LumCmax and k values for perennial cropping change.
    /// </summary>
    public class Table_5_LumCMax_KValues_Perennial_Cropping_Change_Provider
    {
        #region Fields

        private readonly EcozoneStringConverter _ecozoneStringConverter = new EcozoneStringConverter();
        private readonly SoilTextureStringConverter _soilTextureStringConverter = new SoilTextureStringConverter();
        private readonly PerennialCroppingChangeTypeStringConverter _perennialCroppingChangeTypeStringConverter = new PerennialCroppingChangeTypeStringConverter();

        #endregion

        #region Constructors

        public Table_5_LumCMax_KValues_Perennial_Cropping_Change_Provider()
        {
            HTraceListener.AddTraceListener();
            this.Data = this.ReadData();
        }

        #endregion

        #region Properties

        private List<Table_5_LumCMax_KValues_Perennial_Cropping_Change_Data> Data { get; set; }

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
                Trace.TraceError($"{nameof(Table_5_LumCMax_KValues_Perennial_Cropping_Change_Provider.GetLumCMax)} unable to get value for {ecozone.GetDescription()}, {soilTexture.GetDescription()}, and {changeType.GetDescription()}. Returning default value of {defaultValue}.");

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
                Trace.TraceError($"{nameof(Table_5_LumCMax_KValues_Perennial_Cropping_Change_Provider.GetKValue)} unable to get value for {ecozone.GetDescription()}, {soilTexture.GetDescription()}, and {changeType.GetDescription()}. Returning default value of {defaultValue}.");

                return defaultValue;
            }
        }

        #endregion

        #region Private Methods

        private List<Table_5_LumCMax_KValues_Perennial_Cropping_Change_Data> ReadData()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.LumCMaxAndKValuesForPerennialCroppingChange;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<Table_5_LumCMax_KValues_Perennial_Cropping_Change_Data>();

            foreach (var line in filelines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line[0]))
                {
                    break;
                }

                var entry = new Table_5_LumCMax_KValues_Perennial_Cropping_Change_Data();
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
