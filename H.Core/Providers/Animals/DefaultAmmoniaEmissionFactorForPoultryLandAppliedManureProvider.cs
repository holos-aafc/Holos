﻿using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class DefaultAmmoniaEmissionFactorForPoultryLandAppliedManureProvider
    {
        public double GetAmmoniaEmissionFactorForLandAppliedManure(AnimalType animalType)
        {
            if (animalType.IsChickenType())
            {
                if (animalType == AnimalType.ChickenHens || animalType == AnimalType.Layers) return 0.025;

                return 0.027;
            }

            // Turkeys
            return 0.025;
        }
    }
}