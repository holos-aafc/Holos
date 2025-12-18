using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Core.Models
{
    public static class ComponentTypeExtensions
    {
        public static AnimalType GetAnimalTypeFromAnimalComponentType(this ComponentType componentType)
        {
            switch(componentType)
            {
                case ComponentType.Backgrounding:
                case ComponentType.Finishing:
                case ComponentType.CowCalf:
                    return AnimalType.Beef;

                case ComponentType.Dairy:
                    return AnimalType.Dairy;

                case ComponentType.SwineGrowers:
                case ComponentType.FarrowToWean:
                case ComponentType.IsoWean:
                case ComponentType.FarrowToFinish:
                    return AnimalType.Swine;

                case ComponentType.SheepFeedlot:
                case ComponentType.Rams:
                case ComponentType.LambsAndEwes:
                    return AnimalType.Sheep;

                case ComponentType.Goats:
                    return AnimalType.Goats;

                case ComponentType.Deer:
                    return AnimalType.Deer;

                case ComponentType.Horses:
                    return AnimalType.Horses;

                case ComponentType.Mules:
                    return AnimalType.Mules;

                case ComponentType.Bison:
                    return AnimalType.Bison;

                case ComponentType.Llamas:
                    return AnimalType.Llamas;

                case ComponentType.ChickenPulletFarm:
                case ComponentType.ChickenMultiplierBreeder:
                case ComponentType.ChickenMeatProduction:
                case ComponentType.ChickenEggProduction:
                case ComponentType.ChickenMultiplierHatchery:
                    return AnimalType.Chicken;

                case ComponentType.TurkeyMultiplierBreeder:
                case ComponentType.TurkeyMeatProduction:
                    return AnimalType.Turkeys;

                default:
                    return AnimalType.NotSelected;

            }

        }
    }
}
