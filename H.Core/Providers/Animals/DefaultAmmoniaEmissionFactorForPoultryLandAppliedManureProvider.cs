using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class DefaultAmmoniaEmissionFactorForPoultryLandAppliedManureProvider
    {
        public double GetDefaultAmmoniaEmissionFactorForLandAppliedManure(AnimalType animalType)
        {
            if (animalType.IsChickenType())
            {
                if (animalType == AnimalType.ChickenHens || animalType == AnimalType.Layers)
                {
                    return 0.025;
                }
                else
                {
                    return 0.027;
                }
            }
            else
            {
                // Turkeys
                return 0.026;
            }
        }
    }
}