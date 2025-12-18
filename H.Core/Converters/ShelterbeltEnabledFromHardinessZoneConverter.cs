using H.Core.Enumerations;
using H.Core.Providers.Shelterbelt;

namespace H.Core.Converters
{
    public class ShelterbeltEnabledFromHardinessZoneConverter
    {
        /// <summary>
        /// Converts a hardiness zone to a bool, indicating whether the Shelterbelt Component should be enabled.
        /// </summary>
        public bool Convert(HardinessZone plantHardinessZone)
        {
            //this hard coded swtich should reflect which hardiness zones we have data for in the lookup table: shelterbelt_hardiness_lookup.csv
            switch (plantHardinessZone)
            {
                case HardinessZone.H2a:
                    return true;
                case HardinessZone.H2b:
                    return true;
                case HardinessZone.H3a:
                    return true;
                case HardinessZone.H3b:
                    return true;
                case HardinessZone.H4a:
                    return true;
                case HardinessZone.H4b:
                    return true;
                default:
                    return false;
            }
        }
    }
}
