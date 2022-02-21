using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    /// <summary>
    /// Hardiness Zones are classified using a number followed by a letter
    /// - this violates a naming requirement of C#.
    /// Therefore it was necessary to place something in front of the intended enumeration value.
    /// H was chosen as it can represent the first letter of "Hardiness"
    /// </summary>
    public enum HardinessZone
    {
        [LocalizedDescription("EnumNotAvailable", typeof(Resources))]
        NotAvailable,

        [LocalizedDescription("H0a", typeof(Resources))]
        H0a,

        [LocalizedDescription("H0b", typeof(Resources))]
        H0b,

        [LocalizedDescription("H1a", typeof(Resources))]
        H1a,

        [LocalizedDescription("H1b", typeof(Resources))]
        H1b,

        [LocalizedDescription("H2a", typeof(Resources))]
        H2a,

        [LocalizedDescription("H2b", typeof(Resources))]
        H2b,

        [LocalizedDescription("H3a", typeof(Resources))]
        H3a,

        [LocalizedDescription("H3b", typeof(Resources))]
        H3b,

        [LocalizedDescription("H4a", typeof(Resources))]
        H4a,

        [LocalizedDescription("H4b", typeof(Resources))]
        H4b,

        [LocalizedDescription("H5a", typeof(Resources))]
        H5a,

        [LocalizedDescription("H5b", typeof(Resources))]
        H5b,

        [LocalizedDescription("H6a", typeof(Resources))]
        H6a,

        [LocalizedDescription("H6b", typeof(Resources))]
        H6b,

        [LocalizedDescription("H7a", typeof(Resources))]
        H7a,

        [LocalizedDescription("H7b", typeof(Resources))]
        H7b,

        [LocalizedDescription("H8a", typeof(Resources))]
        H8a,

        [LocalizedDescription("H8b", typeof(Resources))]
        H8b,

        [LocalizedDescription("H9a", typeof(Resources))]
        H9a
    }
}