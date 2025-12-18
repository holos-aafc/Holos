using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum DairyFeedClassType
    {
       
        [LocalizedDescription("Forage", typeof(Resources))]
        Forage,

        [LocalizedDescription("Conc", typeof(Resources))]
        Conc,

        [LocalizedDescription("Animal", typeof(Resources))]
        Animal,

        [LocalizedDescription("Fat", typeof(Resources))]
        Fat,

        [LocalizedDescription("FatG", typeof(Resources))]
        FatG,



    }
}
