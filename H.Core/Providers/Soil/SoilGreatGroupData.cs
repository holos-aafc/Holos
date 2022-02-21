#region Imports

using H.Core.Enumerations;

#endregion

namespace H.Core.Providers.Soil
{
    internal class SoilGreatGroupData
    {
        public SoilGreatGroupType SoilGreatGroup { get; set; }
        public SoilFunctionalCategory SoilFunctionalCategory { get; set; }
        public Region Region { get; set; }
    }
}