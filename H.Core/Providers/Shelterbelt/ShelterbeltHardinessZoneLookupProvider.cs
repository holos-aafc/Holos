using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Infrastructure;
using H.Core.Converters;
using H.Core.Enumerations;

namespace H.Core.Providers.Shelterbelt
{
    /// <summary>
    /// Allows for the lookup of total ecosystem carbon, living biomass carbon, and dead organic matter carbon values by hardiness zone.
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
                entry.HardinessZone = hardinessConverter.Convert(line[0]);
                entry.TreeSpecies = speciesConverter.Convert(line[1]);
                entry.PercentMortality = double.Parse(line[2], cultureInfo);
                entry.Age = double.Parse(line[3], cultureInfo);
                entry.AvgTecMgCkm = double.Parse(line[9], cultureInfo);
                entry.AvgBiomMgKmCYr = double.Parse(line[15], cultureInfo);
                entry.AvgDomMgKmCYr = double.Parse(line[21], cultureInfo);

                result.Add(entry);
            }
             return result;
        }

        public static List<ShelterbeltHardinessZoneLookupData> GetShelterbeltHardinessZoneLookup()
        {
            return _table;
        }

        /// <summary>
        /// If we are not in Saskatchewan, we can't lookup biomass values by cluster id (ecodistrict to cluster id mappings only exist for Saskatchewan), instead we lookup by hardiness zone.
        ///
        /// If we are in Saskatchewan, we use <see cref="H.Core.Providers.Shelterbelt.ShelterbeltCarbonDataProvider.GetLookupValue"/> instead.
        /// </summary>
        public static double GetLookupValue(
            TreeSpecies treeSpecies,
            HardinessZone hardinessZone,
            double percentMortality,
            double mortalityLow,
            double mortalityHigh,
            double age,
            ShelterbeltCarbonDataProviderColumns column)
        {
            if (age > 60)
            {
                age = 60;
            }

            var tableLookupLow = _table.SingleOrDefault(
                x => x.TreeSpecies == treeSpecies &&
                     Math.Abs(x.PercentMortality - mortalityLow) < double.Epsilon &&
                     Math.Abs(x.Age - age) < double.Epsilon &&
                     x.HardinessZone == hardinessZone);

            var tableLookupHigh = _table.SingleOrDefault(
                x => x.TreeSpecies == treeSpecies &&
                     Math.Abs(x.PercentMortality - mortalityHigh) < double.Epsilon &&
                     Math.Abs(x.Age - age) < double.Epsilon &&
                     x.HardinessZone == hardinessZone);

            if (tableLookupHigh != null && tableLookupLow != null)
            {
                var targetLow = 0d;
                var targetHigh = 0d;

                if (column == ShelterbeltCarbonDataProviderColumns.Dom_Mg_C_km)
                {
                    targetLow = tableLookupLow.AvgDomMgKmCYr;
                    targetHigh = tableLookupHigh.AvgDomMgKmCYr;
                }
                else if (column == ShelterbeltCarbonDataProviderColumns.Biom_Mg_C_km)
                {
                    targetLow = tableLookupLow.AvgBiomMgKmCYr;
                    targetHigh = tableLookupHigh.AvgBiomMgKmCYr;
                }

                else if (column == ShelterbeltCarbonDataProviderColumns.Tec_Mg_C_km)
                {
                    targetLow = tableLookupLow.AvgTecMgCkm;
                    targetHigh = tableLookupHigh.AvgTecMgCkm;
                }

                var ratio = (targetLow - targetHigh) / (mortalityHigh - mortalityLow);
                var product = (percentMortality - mortalityLow) * ratio;
                var result = targetLow - product;

                return result;
            }
            else
            {
                Trace.TraceError((nameof(ShelterbeltCarbonDataProvider) + " cannot find value in lookup table."));

                return 0;
            }
        }
    }
}
