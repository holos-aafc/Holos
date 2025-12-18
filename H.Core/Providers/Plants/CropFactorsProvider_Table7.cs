using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Infrastructure;

namespace H.Core.Providers.Plants
{
    /// <summary>
    /// Table 7
    /// </summary>
    public class CropFactorsProvider_Table7
    {
        #region Fields
        
        private readonly CropTypeStringConverter _cropTypeStringConverter = new CropTypeStringConverter();
        private readonly List<CropFactorsData> _cache;
        
        #endregion
        
        #region Constructors
        public CropFactorsProvider_Table7()
        {
            _cache = BuildCache();
        }
        #endregion

        #region Public Methods
        public List<CropFactorsData> GetCropFactors()
        {
            return _cache;
        }
        #endregion

        #region Private Methods
        private List<CropFactorsData> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.CropFactors;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<CropFactorsData>();
            foreach (var line in filelines.Skip(1))
            {
                var entry = new CropFactorsData();
                var cropType = _cropTypeStringConverter.Convert(line[0]);
                var moistureContent = double.Parse(line[1], cultureInfo);
                var aboveGroundResidueNitrogenConcentration = double.Parse(line[2], cultureInfo);
                var belowGroundResidueNitrogenConcentration = double.Parse(line[3], cultureInfo);
                var yieldRatio = double.Parse(line[4], cultureInfo);
                var aboveGroundResidueRatio = double.Parse(line[5], cultureInfo);
                var belowGroundResidueRatio = double.Parse(line[5], cultureInfo);
                var yield = double.Parse(line[6], cultureInfo);
                entry.CropType = cropType;
                entry.MoistureContent = moistureContent;
                entry.AboveGroundResidueNitrogenConcentration = aboveGroundResidueNitrogenConcentration;
                entry.BelowGroundResidueNitrogenConcentration = belowGroundResidueNitrogenConcentration;
                entry.YieldRatio = yieldRatio;
                entry.AboveGroundResidueRatio = aboveGroundResidueRatio;
                entry.BelowGroundResidueRatio = belowGroundResidueRatio;
                entry.Yield = yield;
                result.Add(entry);
            }

            return result;
        }
        #endregion
    }
}