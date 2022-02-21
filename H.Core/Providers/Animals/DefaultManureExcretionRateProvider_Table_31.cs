using System.Diagnostics;
using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class DefaultManureExcretionRateProvider_Table_31
    {
        public double GetManureExcretionRate(AnimalType animalType)
        {
            if (animalType.IsSheepType())
            {
                return 1.8;
            }

            if (animalType == AnimalType.ChickenPullets ||
                animalType == AnimalType.Broilers)
            {
                return 0.08;
            }

            if (animalType == AnimalType.LayersDryPoultry ||
                animalType == AnimalType.LayersWetPoultry)
            {
                return 0.12;
            }

            if (animalType == AnimalType.TurkeyHen ||
                animalType == AnimalType.YoungTurkeyHen ||
                animalType == AnimalType.Turkeys)
            {
                return 0.32;
            }

            if (animalType == AnimalType.Ducks)
            {
                return 0.2;
            }

            if (animalType == AnimalType.Geese)
            {
                return 0.2;
            }

            if (animalType == AnimalType.Llamas ||
                animalType == AnimalType.Alpacas)
            {
                return 2;
            }

            if (animalType == AnimalType.Deer ||
                animalType == AnimalType.Elk)
            {
                return 0;
            }

            if (animalType == AnimalType.Goats)
            {
                return 3;
            }

            if (animalType == AnimalType.Horses)
            {
                return 23;
            }

            if (animalType == AnimalType.Mules)
            {
                return 23;
            }

            if (animalType == AnimalType.Bison)
            {
                return 62;
            }

            Trace.TraceError($"{nameof(DefaultManureExcretionRateProvider_Table_31)}.{nameof(GetManureExcretionRate)}" +
                             $" unable to get data for manure excretion rate for animal type: {animalType}." +
                             $" Returning default value of 0.");

            return 0;
        }
    }
}