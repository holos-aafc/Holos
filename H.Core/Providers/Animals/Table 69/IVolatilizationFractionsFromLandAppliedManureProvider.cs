using H.Core.Enumerations;

namespace H.Core.Providers.Animals.Table_69
{
    public interface IVolatilizationFractionsFromLandAppliedManureProvider
    {
        VolatilizationFractionsFromLandAppliedManureData GetData(AnimalType animalType, Province province, int year);
    }
}