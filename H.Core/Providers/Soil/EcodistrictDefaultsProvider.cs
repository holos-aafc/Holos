using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Soil
{
    /// <summary>
    /// Used to assist in the lookup of values in Table 1.
    /// </summary>
    public class EcodistrictDefaultsProvider
    {
        #region Fields

        private readonly EcozoneStringConverter _ecozoneStringConverter;
        private readonly ProvinceStringConverter _provinceStringConverter;
        private readonly SoilFunctionalCategoryStringConverter _soilFunctionalCategoryStringConverter;
        private readonly SoilTextureStringConverter _soilTextureStringConverter;

        #endregion

        #region Constructors

        public EcodistrictDefaultsProvider()
        {
            HTraceListener.AddTraceListener();

            _ecozoneStringConverter = new EcozoneStringConverter();
            _provinceStringConverter = new ProvinceStringConverter();
            _soilFunctionalCategoryStringConverter = new SoilFunctionalCategoryStringConverter();
            _soilTextureStringConverter = new SoilTextureStringConverter();

            this.Data = this.ReadFile();
        }

        #endregion

        #region Properties

        private List<EcodistrictDefaultsData> Data { get; set; }

        #endregion

        #region Public Methods       

        /// <summary>
        /// Multiple ecodistricts can exist in a single ecozone.
        /// </summary>
        public Ecozone GetEcozone(int ecodistrictId)
        {
            var result = this.Data.FirstOrDefault(x => x.EcodistrictId == ecodistrictId);
            if (result != null)
            {
                return result.Ecozone;
            }

            else
            {
                Trace.TraceError( $"{nameof(EcodistrictDefaultsProvider)}.{nameof(EcodistrictDefaultsProvider.GetEcozone)} unable to get ecozone for ecodistrict: {ecodistrictId}. Returning default value of {Ecozone.AtlanticMaritimes.GetDescription()}.");

                return Ecozone.AtlanticMaritimes;
            }
        }

        public double GetFractionOfLandOccupiedByPortionsOfLandscape(int ecodistrictId, Province province)
        {
            const double defaultValue = 0;

            var result = this.Data.FirstOrDefault(x => x.EcodistrictId == ecodistrictId && 
                                                       x.Province == province);
            if (result != null)
            {
                // Convert value to a fraction not a percentage (i.e. 0.20 not 20)
                return result.FTopo / 100;
            }
            else
            {
                Trace.TraceError($"{nameof(EcodistrictDefaultsProvider)}.{nameof(EcodistrictDefaultsProvider.GetEcozone)} unable to get FTopo value for ecodistrict: {ecodistrictId}. Returning default value of {defaultValue}.");

                return defaultValue;
            }
        }

        #endregion

        #region Private Methods

        private List<EcodistrictDefaultsData> ReadFile()
        {
            var results = new List<EcodistrictDefaultsData>();

            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.EcodistrictToEcozoneMapping);
            foreach (var line in fileLines.Skip(1))
            {
                var ecodistrictId = int.Parse(line[0], cultureInfo);
                var ecozone = _ecozoneStringConverter.Convert(line[1]);
                var province = _provinceStringConverter.Convert(line[2]);
                var pMayToOct = int.Parse(line[3], cultureInfo);
                var peMayToOct = int.Parse(line[4], cultureInfo);
                var fTopo = double.Parse(line[5], cultureInfo);
                var soilType = _soilFunctionalCategoryStringConverter.Convert(line[6]);
                var soilTexture = _soilTextureStringConverter.Convert(line[7]);

                results.Add(new EcodistrictDefaultsData()
                {
                    EcodistrictId = ecodistrictId,
                    Ecozone = ecozone,
                    Province = province,
                    PMayToOct = pMayToOct,
                    PEMayToOct = peMayToOct,
                    FTopo = fTopo,
                    SoilFunctionalCategory = soilType,
                    SoilTexture = soilTexture,
                });
            }

            return results;
        }

        #endregion
    }
}