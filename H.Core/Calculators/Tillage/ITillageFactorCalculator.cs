using H.Core.Enumerations;

namespace H.Core.Calculators.Tillage
{
    public interface ITillageFactorCalculator
    {
        double CalculateTillageFactor(Province province, SoilFunctionalCategory soilFunctionalCategory,
                                      TillageType tillageType, CropType cropType);
    }
}