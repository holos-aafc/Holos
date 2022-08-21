using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class DefaultAmmoniaEmissionFactorsForPoultryManureStorageProvider
    {
        public double GetDefaultAmmoniaEmissionFactorForStorage(AnimalType animalType)
        {
            if (animalType.IsChickenType())
            {
                if (animalType == AnimalType.ChickenHens || animalType == AnimalType.Layers)
                {
                    return 0.24;
                }
                else
                {
                    return 0.25;
                }
            }
            else
            {
                // Turkeys
                return 0.24;
            }
        }
    }
}