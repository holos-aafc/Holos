namespace H.Core.Models
{
    public enum LandUseCarbonChangeSource
    {
        All, // Used when totaling for entire farm
        Tillage,
        Fallow,
        Perennials,
        SeededAndBrokenGrassland,
        BrokenGrassland,
        SeededGrassland,
        PastPerennials,     // Includes broken grassland
        CurrentPerennials,  // Includes seeded grasslands
    }
}