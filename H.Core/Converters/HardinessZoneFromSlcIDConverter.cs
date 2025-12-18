using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Providers.Shelterbelt;
using H.Infrastructure;

namespace H.Core.Converters
{
    /// <summary>
    /// Return object for the HardinessZoneFromSlcIDConverter
    /// </summary>
    public class WeightedHardinessZone
    {
        public HardinessZone HardinessZone { get; set; }

        public string HardinessZoneString
        {
            get { return this.HardinessZone.GetDescription(); }
        }

        public double HardinessProportion { get; set; } //proportion out of 1
    }

    /// <summary>
    /// Convert from SLC to Hardiness Zone
    /// </summary>
    public class HardinessZoneFromSlcIDConverter
    {
        private SlcToHardinessZoneProvider provider = new SlcToHardinessZoneProvider();

        /// <summary>
        /// Takes an SLC Polygon ID and returns a list, in descending order of overlap which describes the proportion
        /// of that polygon which overlaps each Hardiness Zone represented in the list.
        /// If there are no hardiness zones that overlap with the SLC, the list returned will be empty.
        /// </summary>
        public List<WeightedHardinessZone> Convert(int SlcID)
        {
            var table = provider.GetSlcToHardinessZone();
            var entry = table.Find(x => x.SLCID == SlcID);
            List<WeightedHardinessZone> result = new List<WeightedHardinessZone>();
            if (entry == null)
            {
                return result;
            }
            if (entry.H0bProportion > 0)
            {
                WeightedHardinessZone r = new WeightedHardinessZone();
                r.HardinessZone = HardinessZone.H0b;
                r.HardinessProportion = entry.H0bProportion;
                result.Add(r);
            }
            if (entry.H1aProportion > 0)
            {
                WeightedHardinessZone r = new WeightedHardinessZone();
                r.HardinessZone = HardinessZone.H1a;
                r.HardinessProportion = entry.H1aProportion;
                result.Add(r);
            }
            if (entry.H1bProportion > 0)
            {
                WeightedHardinessZone r = new WeightedHardinessZone();
                r.HardinessZone = HardinessZone.H1b;
                r.HardinessProportion = entry.H1bProportion;
                result.Add(r);
            }
            if (entry.H2aProportion > 0)
            {
                WeightedHardinessZone r = new WeightedHardinessZone();
                r.HardinessZone = HardinessZone.H2a;
                r.HardinessProportion = entry.H2aProportion;
                result.Add(r);
            }
            if (entry.H2bProportion > 0)
            {
                WeightedHardinessZone r = new WeightedHardinessZone();
                r.HardinessZone = HardinessZone.H2b;
                r.HardinessProportion = entry.H2bProportion;
                result.Add(r);
            }
            if (entry.H3aProportion > 0)
            {
                WeightedHardinessZone r = new WeightedHardinessZone();
                r.HardinessZone = HardinessZone.H3a;
                r.HardinessProportion = entry.H3aProportion;
                result.Add(r);
            }
            if (entry.H3bProportion > 0)
            {
                WeightedHardinessZone r = new WeightedHardinessZone();
                r.HardinessZone = HardinessZone.H3b;
                r.HardinessProportion = entry.H3bProportion;
                result.Add(r);
            }
            if (entry.H4aProportion > 0)
            {
                WeightedHardinessZone r = new WeightedHardinessZone();
                r.HardinessZone = HardinessZone.H4a;
                r.HardinessProportion = entry.H4aProportion;
                result.Add(r);
            }
            if (entry.H4bProportion > 0)
            {
                WeightedHardinessZone r = new WeightedHardinessZone();
                r.HardinessZone = HardinessZone.H4b;
                r.HardinessProportion = entry.H4bProportion;
                result.Add(r);
            }
            if (entry.H5aProportion > 0)
            {
                WeightedHardinessZone r = new WeightedHardinessZone();
                r.HardinessZone = HardinessZone.H5a;
                r.HardinessProportion = entry.H5aProportion;
                result.Add(r);
            }
            if (entry.H5bProportion > 0)
            {
                WeightedHardinessZone r = new WeightedHardinessZone();
                r.HardinessZone = HardinessZone.H5b;
                r.HardinessProportion = entry.H5bProportion;
                result.Add(r);
            }
            if (entry.H6aProportion > 0)
            {
                WeightedHardinessZone r = new WeightedHardinessZone();
                r.HardinessZone = HardinessZone.H6a;
                r.HardinessProportion = entry.H6aProportion;
                result.Add(r);
            }
            if (entry.H6bProportion > 0)
            {
                WeightedHardinessZone r = new WeightedHardinessZone();
                r.HardinessZone = HardinessZone.H6b;
                r.HardinessProportion = entry.H6bProportion;
                result.Add(r);
            }
            if (entry.H7aProportion > 0)
            {
                WeightedHardinessZone r = new WeightedHardinessZone();
                r.HardinessZone = HardinessZone.H7a;
                r.HardinessProportion = entry.H7aProportion;
                result.Add(r);
            }
            if (entry.H7bProportion > 0)
            {
                WeightedHardinessZone r = new WeightedHardinessZone();
                r.HardinessZone = HardinessZone.H7b;
                r.HardinessProportion = entry.H7bProportion;
                result.Add(r);
            }
            if (entry.H8aProportion > 0)
            {
                WeightedHardinessZone r = new WeightedHardinessZone();
                r.HardinessZone = HardinessZone.H8a;
                r.HardinessProportion = entry.H8aProportion;
                result.Add(r);
            }
            if (entry.H8bProportion > 0)
            {
                WeightedHardinessZone r = new WeightedHardinessZone();
                r.HardinessZone = HardinessZone.H8b;
                r.HardinessProportion = entry.H8bProportion;
                result.Add(r);
            }
            if (entry.H9aProportion > 0)
            {
                WeightedHardinessZone r = new WeightedHardinessZone();
                r.HardinessZone = HardinessZone.H9a;
                r.HardinessProportion = entry.H9aProportion;
                result.Add(r);
            }

            return result.OrderByDescending(x => x.HardinessProportion).ToList();
        }
    }
}
