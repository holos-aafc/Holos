using System;
using System.Collections.Generic;
using System.Diagnostics;
using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class DefaultVolatileSolidExcretionProvider_Table_36
    {
        public DefaultVolatileSolidExcretionProvider_Table_36()
        {
        }

        public double GetVolatileSolids(AnimalType animalType)
        {
            return this.GetDefaultVolatileSolidExcretionData(animalType).VolatileSolids;
        }

        public DefaultVolatileSolidExcretionData GetDefaultVolatileSolidExcretionData(AnimalType animalType)
        {
            if (animalType == AnimalType.ChickenPullets || animalType == AnimalType.ChickenCockerels)
            {
                // Used for pullets from algorithm document
                return new DefaultVolatileSolidExcretionData()
                {
                    VolatileSolidExcretionRate = 5.9,
                    VolatileSolids = 0.0043,
                };
            }

            if (animalType == AnimalType.ChickenRoosters)
            {
                // Used for broilers from algorithm document
                return new DefaultVolatileSolidExcretionData()
                {
                    VolatileSolidExcretionRate = 16.8,
                    VolatileSolids = 0.0169,
                };
            }

            if (animalType == AnimalType.ChickenHens)
            {
                // Used for layers (wet/dry) from algorithm document
                return new DefaultVolatileSolidExcretionData()
                {
                    VolatileSolidExcretionRate = 9.4,
                    VolatileSolids = 0.0169,
                };
            }

            if (animalType == AnimalType.Goats)
            {
                return new DefaultVolatileSolidExcretionData()
                {
                    VolatileSolidExcretionRate = 9,
                    VolatileSolids = 0.5760,
                };
            }

            if (animalType == AnimalType.Llamas || animalType == AnimalType.Alpacas)
            {
                return new DefaultVolatileSolidExcretionData()
                {
                    VolatileSolidExcretionRate = 8.2,
                    VolatileSolids = 0.9184,
                };
            }

            if (animalType == AnimalType.Horses)
            {
                return new DefaultVolatileSolidExcretionData()
                {
                    VolatileSolidExcretionRate = 5.65,
                    VolatileSolids = 2.5425,
                };
            }

            if (animalType == AnimalType.Mules)
            {
                return new DefaultVolatileSolidExcretionData()
                {
                    VolatileSolidExcretionRate = 7.2,
                    VolatileSolids = 1.764,
                };
            }

            if (animalType == AnimalType.Bison)
            {
                return new DefaultVolatileSolidExcretionData()
                {
                    VolatileSolidExcretionRate = 7.6,
                    VolatileSolids = 4.4080,
                };
            }

            Trace.TraceError($"{nameof(DefaultVolatileSolidExcretionProvider_Table_36)}.{nameof(GetDefaultVolatileSolidExcretionData)}" +
                             $" unable to get data for animal type: {animalType}." +
                             $" Returning default value of 0.");

            return new DefaultVolatileSolidExcretionData();
        }
    }
}