using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Infrastructure;
using H.Core.Converters;
using System.CodeDom;

namespace H.Core.Providers.Shelterbelt
{
    /// <summary>
    /// If speed is needed, try using the same provider object everywhere.
    /// It will cache the table and load it only once, after which things can be accessed in constant time
    /// instead of O(n) time. However, the table for this one is small enough that it probably does not matter.
    /// </summary>
    public static class ShelterbeltAllometricTableProvider
    {
        private static List<ShelterbeltAllometricTableData> _table;

        static ShelterbeltAllometricTableProvider()
        {
            _table = CacheTable();
        }

        public static List<ShelterbeltAllometricTableData> GetShelterbeltAllometricTable()
        {
            return _table;
        }

        private static List<ShelterbeltAllometricTableData> CacheTable()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.ShelterbeltAllometricTable;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<ShelterbeltAllometricTableData>();
            TreeSpeciesStringConverter converter = new TreeSpeciesStringConverter();
            foreach (var line in filelines.Skip(1))
            {
                var entry = new ShelterbeltAllometricTableData();
                entry.TreeSpecies = converter.Convert(line[0]);
                entry.A = double.Parse(line[1], cultureInfo);
                entry.B = double.Parse(line[2], cultureInfo);
                result.Add(entry);
            }
            return result;
        }

    }
}
