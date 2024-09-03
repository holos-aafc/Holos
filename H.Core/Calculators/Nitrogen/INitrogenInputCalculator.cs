namespace H.Core.Calculators.Nitrogen
{
    public interface INitrogenInputCalculator
    {
        double CalculateFractionOfNitrogenLostByLeachingAndRunoff(
            double growingSeasonPrecipitation,
            double growingSeasonEvapotranspiration);
    }
}