using H.Core.Enumerations;

namespace H.Core.Providers.Shelterbelt
{
    public class ShelterbeltEcodistrictLookupTableData
    {
        public TreeSpecies TreeSpecies { get; set; }
        public int EcodistrictID { get; set; } = -1; //deliberate nonsense value
        public double PercentMortality { get; set; }
        public double Age { get; set; }
        public double DiameterCM { get; set; }
        public double RootsBiomassKgPerTree { get; set; }
        public double FinerootsKgCperTree { get; set; }
        public string ClusterId { get; set; }

        public bool IsValid()
        {
            return EcodistrictID != -1;
        }
    }
}
