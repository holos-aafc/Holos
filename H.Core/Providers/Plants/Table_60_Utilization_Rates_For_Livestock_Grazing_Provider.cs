using H.Core.Enumerations;

namespace H.Core.Providers.Plants
{
    /// <summary>
    ///     Table 60: Utilization rate lookup table for livestock grazing.
    ///     A revised utilization rate table for grazed systems, where the utilization rate depends on the type of grazed
    ///     perennial
    ///     forage (rather than the grazing regime or stocking density), and this in turn can be used to back-calculate above
    ///     ground pasture biomass.
    ///     <para>Source: Bailey et al. (2010) (for native rangeland utilization rate); Expert opinion</para>
    /// </summary>
    public class Table_60_Utilization_Rates_For_Livestock_Grazing_Provider
    {
        public double GetUtilizationRate(CropType cropType)
        {
            switch (cropType)
            {
                case CropType.RangelandNative:
                    return 40;

                case CropType.SeededGrassland:
                    return 50;

                case CropType.Forage:
                case CropType.TameGrass:
                case CropType.TameLegume:
                case CropType.TameMixed:
                case CropType.PerennialForages:
                case CropType.ForageForSeed:
                    return 60;

                // Annuals
                default:
                {
                    return 70;
                }
            }
        }
    }
}