using H.Core.Enumerations;

namespace H.Core.Providers.Shelterbelt
{
    public class ShelterbeltHardinessZoneLookupData
    {
        public TreeSpecies TreeSpecies { get; set; }
        public HardinessZone HardinessZone { get; set; } = HardinessZone.NotAvailable;
        public double PercentMortality { get; set; }
        public double Age { get; set; }
        public double DiameterCMMin { get; set; }
        public double DiameterCMMax { get; set; }
        public double DiameterCMWeightedMean { get; set; }
        public double AvgTecMgCkm { get; set; }
        public double AvgBiomMgKmCYr { get; set; }
        public double AvgDomMgKmCYr { get; set; }
        public double RootsKgBiomassPerTreeMin { get; set; }
        public double RootsKgBiomassPerTreeMax { get; set; }
        public double RootsKgBiomassPerTreeWeightedMean { get; set; }
        public double FinerootsBiomassKgPerTreeMin { get; set; }
        public double FinerootsKgBiomassPerTreeMax { get; set; }
        public double FinerootsKgBiomassPerTreeWeightedMean { get; set; }

        public bool IsValid()
        {
            return HardinessZone != HardinessZone.NotAvailable;
        }
    }
}
