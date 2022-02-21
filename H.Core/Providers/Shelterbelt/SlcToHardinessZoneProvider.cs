using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using H.Content;

namespace H.Core.Providers.Shelterbelt
{
    /// <summary>
    /// If speed is needed, try using the same provider object everywhere.
    /// It will cache the table and load it only once, after which things can be accessed in constant time
    /// instead of O(n) time where n may be large enough to take a second.
    /// </summary>
	public class SlcToHardinessZoneProvider
    {
        private List<SlcToHardinessZoneData> _table;

        public SlcToHardinessZoneProvider()
        {
            _table = CacheTable();
        }
        public List<SlcToHardinessZoneData> GetSlcToHardinessZone()
        {
            return _table;
        }

        private List<SlcToHardinessZoneData> CacheTable()
        {
            var cultureInfo = H.Infrastructure.InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.SlcToHardinessZone;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<SlcToHardinessZoneData>();
            foreach (var line in filelines.Skip(1))
            {
                var entry = new SlcToHardinessZoneData();
                var sLCID = int.Parse(line[0], cultureInfo);
                var h0bProportion = double.Parse(line[1], cultureInfo);
                var h1aProportion = double.Parse(line[2], cultureInfo);
                var h1bProportion = double.Parse(line[3], cultureInfo);
                var h2aProportion = double.Parse(line[4], cultureInfo);
                var h2bProportion = double.Parse(line[5], cultureInfo);
                var h3aProportion = double.Parse(line[6], cultureInfo);
                var h3bProportion = double.Parse(line[7], cultureInfo);
                var h4aProportion = double.Parse(line[8], cultureInfo);
                var h4bProportion = double.Parse(line[9], cultureInfo);
                var h5aProportion = double.Parse(line[10], cultureInfo);
                var h5bProportion = double.Parse(line[11], cultureInfo);
                var h6aProportion = double.Parse(line[12], cultureInfo);
                var h6bProportion = double.Parse(line[13], cultureInfo);
                var h7aProportion = double.Parse(line[14], cultureInfo);
                var h7bProportion = double.Parse(line[15], cultureInfo);
                var h8aProportion = double.Parse(line[16], cultureInfo);
                var h8bProportion = double.Parse(line[17], cultureInfo);
                var h9aProportion = double.Parse(line[18], cultureInfo);
                var hTotalProportion = double.Parse(line[19], cultureInfo);
                entry.SLCID = sLCID;
                entry.H0bProportion = h0bProportion;
                entry.H1aProportion = h1aProportion;
                entry.H1bProportion = h1bProportion;
                entry.H2aProportion = h2aProportion;
                entry.H2bProportion = h2bProportion;
                entry.H3aProportion = h3aProportion;
                entry.H3bProportion = h3bProportion;
                entry.H4aProportion = h4aProportion;
                entry.H4bProportion = h4bProportion;
                entry.H5aProportion = h5aProportion;
                entry.H5bProportion = h5bProportion;
                entry.H6aProportion = h6aProportion;
                entry.H6bProportion = h6bProportion;
                entry.H7aProportion = h7aProportion;
                entry.H7bProportion = h7bProportion;
                entry.H8aProportion = h8aProportion;
                entry.H8bProportion = h8bProportion;
                entry.H9aProportion = h9aProportion;
                entry.HTotalProportion = hTotalProportion;
                result.Add(entry);
            }
            return result;
        }

    }
}