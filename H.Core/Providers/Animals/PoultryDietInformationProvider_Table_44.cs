using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    public class PoultryDietInformationProvider_Table_44
    {
        public PoultryDietInformationProviderData Get(AnimalType animalType)
        {
            switch (animalType)
            {
                case AnimalType.ChickenPullets:
                {
                    return new PoultryDietInformationProviderData()
                    {
                        DailyMeanIntake = 0.0447,
                        CrudeProtein = 16.3,
                        InitialWeight = 0.043,
                        FinalWeight = 1.37,
                        ProductionPeriod = 133,
                    };
                }

                case AnimalType.ChickenCockerels:
                case AnimalType.ChickenRoosters:
                case AnimalType.Broilers:
                {
                    return new PoultryDietInformationProviderData()
                    {
                        DailyMeanIntake = 0.095,
                        CrudeProtein = 19,
                        InitialWeight = 0.043,
                        FinalWeight = 2,
                        ProductionPeriod = 42,
                    };
                }

                case AnimalType.ChickenHens:
                case AnimalType.LayersDryPoultry:
                case AnimalType.LayersWetPoultry:
                {
                    return new PoultryDietInformationProviderData()
                    {
                        DailyMeanIntake = 0.0978,
                        CrudeProtein = 17.5,
                        ProteinLiveWeight = 0.175,
                        WeightGain = 0.0015,
                        ProteinContentEgg = 0.12,
                        EggProduction = 48.5,
                    };
                }

                default:
                { 
                    Trace.TraceError($"{nameof(PoultryDietInformationProvider_Table_44)}.{nameof(Get)}: No data for '{animalType.GetDescription()}'");

                    return new PoultryDietInformationProviderData();
                }
            }
        }
    }
}
