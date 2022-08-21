using H.Content;
using H.Core.Converters;
using H.Core.Providers.Plants;
using H.Infrastructure;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace H.Core.Providers.Shelterbelt
{
    /// <summary>
    /// Table 14
    /// Coefficients for above-ground biomass estimation for shelterbelt tree species
    /// </summary>
    public class Table_14_Coefficients_For_AGB_Estimation_Shelterbelt_Trees_Provider
    {
        private readonly TreeSpeciesStringConverter _treeSpeciesStringConverter = new TreeSpeciesStringConverter();
        private readonly List<Table_14_Coefficients_For_AGB_Estimation_Shelterbelt_Trees_Data> _cache;

        public Table_14_Coefficients_For_AGB_Estimation_Shelterbelt_Trees_Provider()
        {
            _cache = BuildCache();
        }

        public List<Table_14_Coefficients_For_AGB_Estimation_Shelterbelt_Trees_Data> GetData()
        {
            return _cache;
        }

        private List<Table_14_Coefficients_For_AGB_Estimation_Shelterbelt_Trees_Data> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.CoefficientsForAboveGroundBiomassEstimationForShelterbeltTreeSpecies;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<Table_14_Coefficients_For_AGB_Estimation_Shelterbelt_Trees_Data>();

            foreach (var line in filelines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line[0]))
                {
                    Trace.Write($"{nameof(Table_14_Coefficients_For_AGB_Estimation_Shelterbelt_Trees_Provider)}.{nameof(BuildCache)}" +
                                $" - File: {nameof(CsvResourceNames.CoefficientsForAboveGroundBiomassEstimationForShelterbeltTreeSpecies)} : first cell of the line is empty. Exiting loop to stop reading more lines inside .csv file.");
                    break;
                }

                var entry = new Table_14_Coefficients_For_AGB_Estimation_Shelterbelt_Trees_Data
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
