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
    public static class ShelterbeltHardinessZoneLookupProvider
    {
        private static List<ShelterbeltHardinessZoneLookupData> _table;

        static ShelterbeltHardinessZoneLookupProvider()
        {
            _table = CacheTable();
        }
      
        private static List<ShelterbeltHardinessZoneLookupData> CacheTable()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.ShelterbeltHardinessZoneLookup;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<ShelterbeltHardinessZoneLookupData>();
            TreeSpeciesStringConverter speciesConverter = new TreeSpeciesStringConverter();
            HardinessZoneStringConverter hardinessConverter = new HardinessZoneStringConverter();
            foreach (var line in filelines.Skip(1))
            {
                var entry = new ShelterbeltHardinessZoneLookupData();
                entry.TreeSpecies = speciesConverter.Convert(line[0]);
                entry.HardinessZone = hardinessConverter.Convert(line[1]);
                entry.PercentMortality = double.Parse(line[2], cultureInfo);
                entry.Age = double.Parse(line[3], cultureInfo);
                entry.DiameterCMMin = double.Parse(line[4], cultureInfo);
                entry.DiameterCMMax = double.Parse(line[5], cultureInfo);
                entry.DiameterCMWeightedMean = double.Parse(line[6], cultureInfo);
                entry.RootsKgBiomassPerTreeMin = double.Parse(line[7], cultureInfo);
                entry.RootsKgBiomassPerTreeMax = double.Parse(line[8], cultureInfo);
                entry.RootsKgBiomassPerTreeWeightedMean = double.Parse(line[9], cultureInfo);
                entry.FinerootsBiomassKgPerTreeMin = double.Parse(line[10], cultureInfo);
                entry.FinerootsKgBiomassPerTreeMax = double.Parse(line[11], cultureInfo);
                entry.FinerootsKgBiomassPerTreeWeightedMean = double.Parse(line[12], cultureInfo);
                result.Add(entry);
            }
             return result;
        }

        public static List<ShelterbeltHardinessZoneLookupData> GetShelterbeltHardinessZoneLookup()
        {
            return _table;
        }
    }
}
