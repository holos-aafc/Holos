using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Shelterbelt
{
    public static class ShelterbeltEcodistrictFutureLookupTableProvider
    {
        private static List<ShelterbeltEcodistrictLookupTableData> _data;

        static ShelterbeltEcodistrictFutureLookupTableProvider()
        {
            _data = CacheTable();
        }

        public static List<ShelterbeltEcodistrictLookupTableData> GetShelterbeltEcodistrictLookupTable()
        {
            return _data;
        }

        private static List<ShelterbeltEcodistrictLookupTableData> CacheTable()
        {            
            var result = new List<ShelterbeltEcodistrictLookupTableData>();

            result.AddRange(GetLines(TreeSpecies.Caragana, CsvResourceNames.CaraganaAllClustersFuture));
            result.AddRange(GetLines(TreeSpecies.GreenAsh, CsvResourceNames.GreenAshAllClustersFuture));
            result.AddRange(GetLines(TreeSpecies.HybridPoplar, CsvResourceNames.HybridPoplarAllClustersFuture));
            result.AddRange(GetLines(TreeSpecies.ManitobaMaple, CsvResourceNames.ManitobaMapleAllClustersFuture));
            result.AddRange(GetLines(TreeSpecies.ScotsPine, CsvResourceNames.ScotsPineAllClustersFuture));
            result.AddRange(GetLines(TreeSpecies.WhiteSpruce, CsvResourceNames.WhiteSpruceAllClustersFuture));

            return result;
        }

        private static List<ShelterbeltEcodistrictLookupTableData> GetLines(
            TreeSpecies species, 
            CsvResourceNames resourceName)
        {
            var result = new List<ShelterbeltEcodistrictLookupTableData>();
            var filelines = CsvResourceReader.GetFileLines(resourceName);
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;

            foreach (var line in filelines.Skip(1))
            {
                var entry = new ShelterbeltEcodistrictLookupTableData();
                entry.TreeSpecies = species;
                entry.ClusterId = line[1];
                entry.PercentMortality = double.Parse(line[2], cultureInfo);
                entry.Age = double.Parse(line[4], cultureInfo);
                entry.DiameterCM = double.Parse(line[6], cultureInfo);
                entry.RootsBiomassKgPerTree = double.Parse(line[13], cultureInfo);
                entry.FinerootsKgCperTree = double.Parse(line[14], cultureInfo);
                result.Add(entry);
            }

            return result;
        }

    }
}