using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class DefaultAmmoniaEmissionFactorsForPoultryManureStorageProvider
    {
        public double GetAmmoniaEmissionFactorForStorage(AnimalType animalType)
        {
            if (animalType.IsChickenType())
            {
                if (animalType == AnimalType.ChickenHens || animalType == AnimalType.Layers) return 0.087;

                return 0.087;
            }

            // Turkeys
            return 0.087;
        }
    }
}