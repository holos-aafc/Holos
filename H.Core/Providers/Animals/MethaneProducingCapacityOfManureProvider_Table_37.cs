using System.Diagnostics;
using System.Security.Cryptography.Pkcs;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 37.
    ///
    /// Default values for maximum methane producing capacity of manure.
    /// </summary>
    public class MethaneProducingCapacityOfManureProvider_Table_37
    {
        public double GetMethaneProducingCapacityOfManure(AnimalType animalType)
        {
            if (animalType.IsBeefCattleType())
            {
                return 0.19;
            }

            if (animalType.IsDairyCattleType())
            {
                return 0.24;
            }

            if (animalType.IsSwineType())
            {
                return 0.48;
            }

            if (animalType.IsSheepType())
            {
                return 0.19;
            }

            if (animalType == AnimalType.ChickenRoosters)
            {
                // Used for broilers from algorithm document
                return 0.36;
            }

            if (animalType == AnimalType.ChickenHens || animalType == AnimalType.ChickenPullets || animalType == AnimalType.ChickenCockerels)
            {
                // Used for layers (wet/dry) from algorithm document
                return 0.39;
            }

            if (animalType == AnimalType.Goats)
            {
                return 0.18;
            }

            if (animalType == AnimalType.Horses)
            {
                return 0.30;
            }

            if (animalType == AnimalType.Mules)
            {
                return 0.33;
            }

            if (animalType == AnimalType.Llamas || animalType == AnimalType.Alpacas)
            {
                return 0.19;
            }

            if (animalType == AnimalType.Bison)
            {
                return 0.10;
            }

            Trace.TraceError($"{nameof(MethaneProducingCapacityOfManureProvider_Table_37)}.{nameof(GetMethaneProducingCapacityOfManure)}: no result found for '{animalType.GetDescription()}'");

            return 0;
        }
    }
}