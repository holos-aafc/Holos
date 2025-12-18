using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Infrastructure;
using H.Core.Converters;

namespace H.Core.Providers.Shelterbelt
{
    /// <summary>
    /// If speed is needed, try using the same provider object everywhere.
    /// It will cache the table and load it only once, after which things can be accessed in constant time
    /// instead of O(n) time where n may be large enough to take a second.
    /// </summary>
    public static class ShelterbeltEcodistrictLookupTableProvider
    {
        private static readonly List<ShelterbeltEcodistrictLookupTableData> _table;

        static ShelterbeltEcodistrictLookupTableProvider()
        {
            _table = CacheTable();
        }

        private static List<ShelterbeltEcodistrictLookupTableData> CacheTable()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.ShelterbeltEcodistrictLookupTable;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<ShelterbeltEcodistrictLookupTableData>();
            TreeSpeciesStringConverter converter = new TreeSpeciesStringConverter();
            foreach (var line in filelines.Skip(1))
            {
                var entry = new ShelterbeltEcodistrictLookupTableData();
                entry.TreeSpecies = converter.Convert(line[0]);
                entry.EcodistrictID = int.Parse(line[1], cultureInfo);
                entry.PercentMortality = double.Parse(line[2], cultureInfo);
                entry.Age = double.Parse(line[3], cultureInfo);
                entry.DiameterCM = double.Parse(line[4], cultureInfo);
                entry.RootsBiomassKgPerTree = double.Parse(line[5], cultureInfo);
                entry.FinerootsKgCperTree = double.Parse(line[6], cultureInfo);
                result.Add(entry);
            }
            return result;
        }

        public static List<ShelterbeltEcodistrictLookupTableData> GetShelterbeltEcodistrictLookupTable()
        {
            return _table;
        }
    }
}
