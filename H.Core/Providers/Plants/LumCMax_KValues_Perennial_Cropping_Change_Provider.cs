﻿using System.Collections.Generic;
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
    ///     Table 5. LumCmax and k values for perennial cropping change.
    /// </summary>
    public class LumCMax_KValues_Perennial_Cropping_Change_Provider
    {
        #region Constructors

        public LumCMax_KValues_Perennial_Cropping_Change_Provider()
        {
            HTraceListener.AddTraceListener();
            Data = ReadData();
        }

        #endregion

        #region Properties

        private List<LumCMax_KValues_Perennial_Cropping_Change_Data> Data { get; }

        #endregion

        #region Private Methods

        private List<LumCMax_KValues_Perennial_Cropping_Change_Data> ReadData()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.LumCMaxAndKValuesForPerennialCroppingChange;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<LumCMax_KValues_Perennial_Cropping_Change_Data>();

            foreach (var line in filelines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line[0])) break;

                var entry = new LumCMax_KValues_Perennial_Cropping_Change_Data();
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

        #region Fields

        private readonly EcozoneStringConverter _ecozoneStringConverter = new EcozoneStringConverter();
        private readonly SoilTextureStringConverter _soilTextureStringConverter = new SoilTextureStringConverter();

        private readonly PerennialCroppingChangeTypeStringConverter _perennialCroppingChangeTypeStringConverter =
            new PerennialCroppingChangeTypeStringConverter();

        #endregion

        #region Public Methods

        public double GetLumCMax(Ecozone ecozone, SoilTexture soilTexture, PerennialCroppingChangeType changeType)
        {
            const double defaultValue = 0;
            var result = Data.SingleOrDefault(x =>
                x.Ecozone == ecozone && x.SoilTexture == soilTexture && x.PerennialCroppingChangeType == changeType);
            if (result != null) return result.LumCMax;

            Trace.TraceError(
                $"{nameof(GetLumCMax)} unable to get value for {ecozone.GetDescription()}, {soilTexture.GetDescription()}, and {changeType.GetDescription()}. Returning default value of {defaultValue}.");

            return defaultValue;
        }

        public double GetKValue(Ecozone ecozone, SoilTexture soilTexture, PerennialCroppingChangeType changeType)
        {
            const double defaultValue = 0;
            var result = Data.SingleOrDefault(x =>
                x.Ecozone == ecozone && x.SoilTexture == soilTexture && x.PerennialCroppingChangeType == changeType);
            if (result != null) return result.kValue;

            Trace.TraceError(
                $"{nameof(GetKValue)} unable to get value for {ecozone.GetDescription()}, {soilTexture.GetDescription()}, and {changeType.GetDescription()}. Returning default value of {defaultValue}.");

            return defaultValue;
        }

        #endregion
    }
}