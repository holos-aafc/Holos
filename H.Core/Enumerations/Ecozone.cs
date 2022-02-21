using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum Ecozone
    {
        [LocalizedDescription("AtlanticMaritimes", typeof(Resources))]
        AtlanticMaritimes,

        [LocalizedDescription("BorealPlains", typeof(Resources))]
        BorealPlains,

        [LocalizedDescription("BorealShieldEast", typeof(Resources))]
        BorealShieldEast,

        [LocalizedDescription("BorealShieldWest", typeof(Resources))]
        BorealShieldWest,

        [LocalizedDescription("MixedwoodPlains", typeof(Resources))]
        MixedwoodPlains,

        [LocalizedDescription("MontaneCordillera", typeof(Resources))]
        MontaneCordillera,

        [LocalizedDescription("PacificMaritime", typeof(Resources))]
        PacificMaritime,

        [LocalizedDescription("SemiaridPrairies", typeof(Resources))]
        SemiaridPrairies,

        [LocalizedDescription("SubhumidPrairies", typeof(Resources))]
        SubhumidPrairies
    }
}