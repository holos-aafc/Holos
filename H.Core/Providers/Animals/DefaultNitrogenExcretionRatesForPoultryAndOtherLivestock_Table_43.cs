using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class DefaultNitrogenExcretionRatesForPoultryAndOtherLivestock_Table_43
    {
        public double GetNitrogenExcretionRate(AnimalType animalType)
        {
            var result = this.GetByAnimalType(animalType);
            if (result != null)
            {
                return result.NitrogenExcretionRate;
            }
            else
            {
                return 0;
            }
        }

        public DefaultNitrogenExcretionRatesForPoultryAndOtherLivestockData GetByAnimalType(AnimalType animalType)
        {
            if (animalType == AnimalType.ChickenPullets || animalType == AnimalType.ChickenCockerels)
            {
                return new DefaultNitrogenExcretionRatesForPoultryAndOtherLivestockData()
                {
                    AverageLiveAnimalWeight = 0.725,
                    NitrogenExcretionRate = 0.0009,
                };
            }

            if (animalType == AnimalType.ChickenHens)
            {
                return new DefaultNitrogenExcretionRatesForPoultryAndOtherLivestockData()
                {
                    AverageLiveAnimalWeight = 1.8,
                    NitrogenExcretionRate = 0.0017,
                };
            }

            if (animalType == AnimalType.ChickenRoosters)
            {
                return new DefaultNitrogenExcretionRatesForPoultryAndOtherLivestockData()
                {
                    AverageLiveAnimalWeight = 0.9,
                    NitrogenExcretionRate = 0.0022,
                };
            }

            if (animalType.IsTurkeyType())
            {
                return new DefaultNitrogenExcretionRatesForPoultryAndOtherLivestockData()
                {
                    AverageLiveAnimalWeight = 6.8,
                    NitrogenExcretionRate = 0.005,
                };
            }

            if (animalType == AnimalType.Ducks)
            {
                return new DefaultNitrogenExcretionRatesForPoultryAndOtherLivestockData()
                {
                    AverageLiveAnimalWeight = 2.7,
                    NitrogenExcretionRate = 0.0022,
                };
            }

            if (animalType == AnimalType.Geese)
            {
                return new DefaultNitrogenExcretionRatesForPoultryAndOtherLivestockData()
                {
                    AverageLiveAnimalWeight = 4,
                    NitrogenExcretionRate = 0.0033,
                };
            }

            if (animalType == AnimalType.Goats)
            {
                return new DefaultNitrogenExcretionRatesForPoultryAndOtherLivestockData()
                {
                    AverageLiveAnimalWeight = 64,
                    NitrogenExcretionRate = 0.0189,
                };
            }

            if (animalType == AnimalType.Llamas || animalType == AnimalType.Alpacas)
            {
                return new DefaultNitrogenExcretionRatesForPoultryAndOtherLivestockData()
                {
                    AverageLiveAnimalWeight = 112,
                    NitrogenExcretionRate = 0.0392,
                };
            }

            if (animalType == AnimalType.Deer || animalType == AnimalType.Elk)
            {
                return new DefaultNitrogenExcretionRatesForPoultryAndOtherLivestockData()
                {
                    AverageLiveAnimalWeight = 120,
                    NitrogenExcretionRate = 0.0804,
                };
            }

            if (animalType == AnimalType.Horses)
            {
                return new DefaultNitrogenExcretionRatesForPoultryAndOtherLivestockData()
                {
                    AverageLiveAnimalWeight = 450,
                    NitrogenExcretionRate = 0.1131,
                };
            }

            if (animalType == AnimalType.Mules)
            {
                return new DefaultNitrogenExcretionRatesForPoultryAndOtherLivestockData()
                {
                    AverageLiveAnimalWeight = 245,
                    NitrogenExcretionRate = 0.0390,
                };
            }

            if (animalType == AnimalType.Bison)
            {
                return new DefaultNitrogenExcretionRatesForPoultryAndOtherLivestockData()
                {
                    AverageLiveAnimalWeight = 580,
                    NitrogenExcretionRate = 0.2320,
                };
            }

            Trace.TraceError($"{nameof(DefaultNitrogenExcretionRatesForPoultryAndOtherLivestock_Table_43)}.{nameof(this.GetByAnimalType)}" +
                             $" unable to get data for animal type: {animalType}." +
                             $" Returning default value of 0.");

            return new DefaultNitrogenExcretionRatesForPoultryAndOtherLivestockData();
        }
    }
}
