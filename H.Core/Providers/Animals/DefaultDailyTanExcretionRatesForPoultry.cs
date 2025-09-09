﻿using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    /// <summary>
    ///     No table number
    /// </summary>
    public class DefaultDailyTanExcretionRatesForPoultry
    {
        /// <summary>
        /// </summary>
        /// <returns>kg TAN head^-1 day^-1</returns>
        public double GetDailyTanExcretionRate(AnimalType animalType)
        {
            if (animalType.IsChickenType())
            {
                if (animalType == AnimalType.ChickenHens || animalType == AnimalType.Layers) return 0.0011;

                return 0.0007;
            }

            // Turkeys
            return 0.0026;
        }

        public double GetAmmoniaEmissionFactorForHousing(AnimalType animalType)
        {
            if (animalType.IsChickenType())
            {
                if (animalType == AnimalType.ChickenHens || animalType == AnimalType.Layers) return 0.25;

                return 0.26;
            }

            // Turkeys
            return 0.25;
        }
    }
}