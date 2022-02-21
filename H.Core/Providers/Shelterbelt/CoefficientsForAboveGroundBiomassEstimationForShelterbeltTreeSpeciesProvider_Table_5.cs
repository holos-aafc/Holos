using H.Content;
using H.Core.Converters;
using H.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace H.Core.Providers.Shelterbelt
{
    /// <summary>
    /// Table 5
    /// </summary>
    public class CoefficientsForAboveGroundBiomassEstimationForShelterbeltTreeSpeciesProvider_Table_5
    {
        private readonly TreeSpeciesStringConverter _treeSpeciesStringConverter = new TreeSpeciesStringConverter();
        private readonly List<CoefficientsForAboveGroundBiomassEstimationForShelterbeltTreeSpeciesTableData> _cache;

        public CoefficientsForAboveGroundBiomassEstimationForShelterbeltTreeSpeciesProvider_Table_5()
        {
            _cache = BuildCache();
        }

        public List<CoefficientsForAboveGroundBiomassEstimationForShelterbeltTreeSpeciesTableData> GetData()
        {
            return _cache;
        }

        private List<CoefficientsForAboveGroundBiomassEstimationForShelterbeltTreeSpeciesTableData> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.CoefficientsForAboveGroundBiomassEstimationForShelterbeltTreeSpecies;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<CoefficientsForAboveGroundBiomassEstimationForShelterbeltTreeSpeciesTableData>();

            foreach (var line in filelines.Skip(1))
            {
                var entry = new CoefficientsForAboveGroundBiomassEstimationForShelterbeltTreeSpeciesTableData
                {
                    TreeSpecies = _treeSpeciesStringConverter.Convert(line[0]),
                    A = double.Parse(line[1], cultureInfo),
                    B = double.Parse(line[2], cultureInfo),
                    MinDiameter = double.Parse(line[3], cultureInfo),
                    MaxDiameter = double.Parse(line[4], cultureInfo),
                    MinHeight = double.Parse(line[5], cultureInfo),
                    MaxHeight = double.Parse(line[6], cultureInfo),
                    MinAge = double.Parse(line[7], cultureInfo),
                    MaxAge = double.Parse(line[8], cultureInfo),
                    MinSpacing = double.Parse(line[9], cultureInfo),
                    MaxSpacing = double.Parse(line[10], cultureInfo),
                    MinMortality = double.Parse(line[11], cultureInfo),
                    MaxMortality = double.Parse(line[12], cultureInfo)
                };

                result.Add(entry);
            }

            return result;
        }
    }
}
