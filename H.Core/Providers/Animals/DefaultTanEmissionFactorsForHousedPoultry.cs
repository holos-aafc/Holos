using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class DefaultTanEmissionFactorsForHousedPoultry
    {
        public double GetDefaultAmmoniaEmissionFactorForHousing(AnimalType animalType)
        {
            if (animalType.IsChickenType())
            {
                if (animalType == AnimalType.ChickenHens || animalType == AnimalType.Layers)
                {
                    return 0.23;
                }
                else
                {
                    return 0.21;
                }
            }
            else
            {
                // Turkeys
                return 0.23;
            }
        }
    }
}
