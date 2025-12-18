using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Infrastructure;

namespace H.Core.Providers.Plants
{
    public class ShelterbeltCarbonAccumulationCoefficientsProvider
    {
        #region Fields
        private readonly List<ShelterbeltCarbonAccumulationCoefficientsData> _cache;
        #endregion

        #region Constructors
        public ShelterbeltCarbonAccumulationCoefficientsProvider()
        {
            _cache = BuildCache();
        }
        #endregion

        public List<ShelterbeltCarbonAccumulationCoefficientsData> GetShelterbeltCarbonAccumulationCoefficients()
        {
            return _cache;
        }

        private List<ShelterbeltCarbonAccumulationCoefficientsData> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.ShelterbeltCarbonAccumulationCoefficients;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<ShelterbeltCarbonAccumulationCoefficientsData>();
            foreach (var line in filelines.Skip(1))
            {
                var entry = new ShelterbeltCarbonAccumulationCoefficientsData();
                var brownChernozemA = double.Parse(line[1], cultureInfo);
                var brownChernozemB = double.Parse(line[2], cultureInfo);
                var darkBrownA = double.Parse(line[3], cultureInfo);
                var darkBrownB = double.Parse(line[4], cultureInfo);
                var blackChernozemA = double.Parse(line[5], cultureInfo);
                var blackChernozemB = double.Parse(line[6], cultureInfo);
                var plantingSpace = double.Parse(line[7], cultureInfo);
                entry.BrownChernozemA = brownChernozemA;
                entry.BrownChernozemB = brownChernozemB;
                entry.DarkBrownA = darkBrownA;
                entry.DarkBrownB = darkBrownB;
                entry.BlackChernozemA = blackChernozemA;
                entry.BlackChernozemB = blackChernozemB;
                entry.PlantingSpace = plantingSpace;
                result.Add(entry);
            }

            return result;
        }
    }
}