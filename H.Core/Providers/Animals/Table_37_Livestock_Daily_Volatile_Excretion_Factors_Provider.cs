using System;
using System.Collections.Generic;
using System.Diagnostics;
using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 37. Daily volatile solid excretion factors for chickens, goats, llamas and alpacas, horses, mules and bison.
    /// </summary>
    public class Table_37_Livestock_Daily_Volatile_Excretion_Factors_Provider
    {
        public Table_37_Livestock_Daily_Volatile_Excretion_Factors_Provider()
        {
        }

        public double GetVolatileSolids(AnimalType animalType)
        {
            return this.GetDefaultVolatileSolidExcretionData(animalType).VolatileSolids;
        }

        public Table_37_Livestock_Daily_Volatile_Excretion_Factors_Data GetDefaultVolatileSolidExcretionData(AnimalType animalType)
        {
            // Read Footnote 1 for data source information.


            if (animalType == AnimalType.ChickenPullets || animalType == AnimalType.ChickenCockerels)
            {
                // Used for pullets from algorithm document
                return new Table_37_Livestock_Daily_Volatile_Excretion_Factors_Data()
                {
                    VolatileSolidExcretionRate = 5.9,
                    VolatileSolids = 0.00417,
                };
            }

            if (animalType == AnimalType.ChickenRoosters || animalType == AnimalType.Broilers)
            {
                // Used for broilers from algorithm document
                return new Table_37_Livestock_Daily_Volatile_Excretion_Factors_Data()
                {
                    VolatileSolidExcretionRate = 16.8,
                    VolatileSolids = 0.0152,
                };
            }

            if (animalType == AnimalType.ChickenHens || animalType == AnimalType.Layers)
            {
                // Used for layers (wet/dry) from algorithm document
                return new Table_37_Livestock_Daily_Volatile_Excretion_Factors_Data()
                {
                    VolatileSolidExcretionRate = 9.4,
                    VolatileSolids = 0.01692,
                };
            }

            if (animalType == AnimalType.Goats)
            {
                return new Table_37_Livestock_Daily_Volatile_Excretion_Factors_Data()
                {
                    VolatileSolidExcretionRate = 9,
                    VolatileSolids = 0.576,
                };
            }
            // Footnote 2
            if (animalType == AnimalType.Llamas || animalType == AnimalType.Alpacas)
            {
                return new Table_37_Livestock_Daily_Volatile_Excretion_Factors_Data()
                {
                    VolatileSolidExcretionRate = 8.2,
                    VolatileSolids = 0.9184,
                };
            }

            if (animalType == AnimalType.Horses)
            {
                return new Table_37_Livestock_Daily_Volatile_Excretion_Factors_Data()
                {
                    VolatileSolidExcretionRate = 5.65,
                    VolatileSolids = 2.5425,
                };
            }

            if (animalType == AnimalType.Mules)
            {
                return new Table_37_Livestock_Daily_Volatile_Excretion_Factors_Data()
                {
                    VolatileSolidExcretionRate = 7.2,
                    VolatileSolids = 1.764,
                };
            }

            // Footnote 3
            if (animalType == AnimalType.Bison)
            {
                return new Table_37_Livestock_Daily_Volatile_Excretion_Factors_Data()
                {
                    VolatileSolidExcretionRate = 7.6,
                    VolatileSolids = 4.4080,
                };
            }

            Trace.TraceError($"{nameof(Table_37_Livestock_Daily_Volatile_Excretion_Factors_Provider)}.{nameof(GetDefaultVolatileSolidExcretionData)}" +
                             $" unable to get data for animal type: {animalType}." +
                             $" Returning default value of 0.");

            return new Table_37_Livestock_Daily_Volatile_Excretion_Factors_Data();
        }


        #region Footnotes

        /*
         * Footnote 1: Daily VS excretion rates were estimated using default rates from IPCC (2019), Table 10.13a
            (kg VS (1,000 kg animal mass)-1 day-1). Average weights for each animal group (except pullets) derived 
            from ECCC (2022), Tables A3.4-14 and A3.4-25. Average weight for pullets is based on 
            Pelletier (2017): initial weight (0.043 kg) + (final weight (1.37 kg) – initial weight (0.043 kg))/2 = 0.7065 kg.
         * Footnote 2: VS excretion rate for sheep from IPCC (2019), Table 10.13a was used for llamas and alpacas.
         * Footnote 3: VS excretion rate for “other cattle” from IPCC (2019), Table 10.13a was used for bison.

         */

        #endregion
    }
}