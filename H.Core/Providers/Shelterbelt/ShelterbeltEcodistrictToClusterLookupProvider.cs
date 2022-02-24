using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using H.Content;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Shelterbelt
{
    public static class ShelterbeltEcodistrictToClusterLookupProvider
    {
        #region Constructors

         static ShelterbeltEcodistrictToClusterLookupProvider()
        {
            HTraceListener.AddTraceListener();
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.LookupEcodistrictClusters;
            var fileLines = CsvResourceReader.GetFileLines(filename);

            foreach (var line in fileLines.Skip(1))
            {
                var entry = new EcodistrictToClusterData();

                entry.EcodistrictId = int.Parse(line[1], cultureInfo);
                entry.ClusterId = line[2];
                entry.SoilZone = line[3];

                Data.Add(entry);
            }
        }

        #endregion

        #region Properties

        private static List<EcodistrictToClusterData> Data { get; } = new List<EcodistrictToClusterData>();

        #endregion

        public static EcodistrictToClusterData GetClusterData(int ecodistrictId)
        {
            var clusterData = Data.SingleOrDefault(x => x.EcodistrictId == ecodistrictId);
            if (clusterData == null)
            {
                var defaultValue = new EcodistrictToClusterData();
                Trace.TraceError($"{nameof(ShelterbeltEcodistrictToClusterLookupProvider)}.{nameof(GetClusterData)}" +
                    $" unable to get data for the ecodistrict id: {ecodistrictId}." +
                    $" Returning default value of {defaultValue}.");
                return defaultValue;
            }
            return clusterData;
        }

        public static bool CanLookupByEcodistrict(int ecodistrict)
        {
            var clusterData = GetClusterData(ecodistrict);
            if (string.IsNullOrWhiteSpace(clusterData.ClusterId))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}