using System.Diagnostics;
using System.Security.Cryptography.Pkcs;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 38. Default values for maximum methane producing capacity (Bo).
    /// <para>Source: IPCC (2019), Table 10.16</para>
    /// </summary>
    public class Table_38_Methane_Producing_Capacity_Default_Values_Provider
    {
        public double GetMethaneProducingCapacityOfManure(AnimalType animalType)
        {
            // Footnote 3 : For Methane producing capacity (B0) value reference.


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

            if (animalType == AnimalType.ChickenRoosters || animalType == AnimalType.Broilers)
            {
                // Used for broilers from algorithm document
                return 0.36;
            }

            if (animalType == AnimalType.ChickenHens || animalType == AnimalType.ChickenPullets || animalType == AnimalType.ChickenCockerels || animalType == AnimalType.Layers)
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

            // Footnote 2
            if (animalType == AnimalType.Llamas || animalType == AnimalType.Alpacas)
            {
                return 0.19;
            }

            // Footnote 1
            if (animalType == AnimalType.Bison)
            {
                return 0.10;
            }

            Trace.TraceError($"{nameof(Table_38_Methane_Producing_Capacity_Default_Values_Provider)}.{nameof(GetMethaneProducingCapacityOfManure)}: no result found for '{animalType.GetDescription()}'");

            return 0;
        }

        #region Footnotes

        /*
            Footnote 1: Value for non-dairy cattle used
            Footnote 2: Value for sheep used
            Footnote 3: For all animals on pasture, range or paddock, the Bo should be set to 0.19
         */

        #endregion
    }
}