using System.Runtime.CompilerServices;
using System.Windows.Markup;
using H.Core.Models;

namespace H.Core.Enumerations
{
    public static class AnimalTypeExtensions
    {
        public static bool IsBeefCattleType(this AnimalType animalType)
        {
            switch (animalType)
            {
                case AnimalType.Beef:
                case AnimalType.BeefBackgrounder:
                case AnimalType.BeefBulls:
                case AnimalType.BeefBackgrounderHeifer:
                case AnimalType.BeefFinishingSteer:
                case AnimalType.BeefFinishingHeifer:
                case AnimalType.BeefReplacementHeifers:
                case AnimalType.BeefFinisher:
                case AnimalType.BeefBackgrounderSteer:
                case AnimalType.BeefCalf:
                case AnimalType.Stockers:
                case AnimalType.StockerHeifers:
                case AnimalType.StockerSteers:
                case AnimalType.BeefCowLactating:
                case AnimalType.BeefCow:
                case AnimalType.BeefCowDry:
                    return true;

                default:
                        return false;
            }
        }

        public static bool IsDairyCattleType(this AnimalType animalType)
        {
            switch (animalType)
            {
                case AnimalType.Dairy:
                case AnimalType.DairyLactatingCow:
                case AnimalType.DairyBulls:
                case AnimalType.DairyCalves:
                case AnimalType.DairyDryCow:
                case AnimalType.DairyHeifers:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsSwineType(this AnimalType animalType)
        {
            switch (animalType)
            {
                case AnimalType.Swine:
                case AnimalType.SwineFinisher:
                case AnimalType.SwineStarter:
                case AnimalType.SwineLactatingSow:
                case AnimalType.SwineDrySow:
                case AnimalType.SwineGrower:
                case AnimalType.SwineSows:
                case AnimalType.SwineBoar:
                case AnimalType.SwineGilts:
                case AnimalType.SwinePiglets:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsSheepType(this AnimalType animalType)
        {
            switch (animalType)
            {
                case AnimalType.Sheep:
                case AnimalType.LambsAndEwes:
                case AnimalType.Ram:
                case AnimalType.Lambs:
                case AnimalType.Ewes:
                case AnimalType.SheepFeedlot:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsPoultryType(this AnimalType animalType)
        {
            switch (animalType)
            {
                case AnimalType.Poultry:
                case AnimalType.LayersWetPoultry:
                case AnimalType.LayersDryPoultry:
                case AnimalType.Layers:
                case AnimalType.Broilers:
                case AnimalType.Turkeys:
                case AnimalType.Ducks:
                case AnimalType.Geese:
                case AnimalType.ChickenPullets:
                case AnimalType.ChickenCockerels:
                case AnimalType.ChickenRoosters:
                case AnimalType.ChickenHens:
                case AnimalType.YoungTom:
                case AnimalType.Tom:
                case AnimalType.YoungTurkeyHen:
                case AnimalType.TurkeyHen:
                case AnimalType.ChickenEggs:
                case AnimalType.TurkeyEggs:
                case AnimalType.Chicks:
                case AnimalType.Poults:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsOtherAnimalType(this AnimalType animalType)
        {
            switch (animalType)
            {
                case AnimalType.OtherLivestock:
                case AnimalType.Goats:
                case AnimalType.Alpacas:
                case AnimalType.Deer:
                case AnimalType.Elk:
                case AnimalType.Llamas:
                case AnimalType.Horses:
                case AnimalType.Mules:
                case AnimalType.Bison:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsChickenType(this AnimalType animalType)
        {
            switch (animalType)
            {
                case AnimalType.Chicken:
                case AnimalType.ChickenHens:
                case AnimalType.Layers:
                case AnimalType.Broilers:
                case AnimalType.ChickenRoosters:
                case AnimalType.ChickenPullets:
                case AnimalType.ChickenCockerels:
                case AnimalType.ChickenEggs:
                case AnimalType.Chicks:
                    return true;
                
                default:
                    return false;
            }
        }

        public static bool IsTurkeyType(this AnimalType animalType)
        {
            switch (animalType)
            {
                case AnimalType.TurkeyHen:
                case AnimalType.YoungTurkeyHen:
                case AnimalType.Tom:
                case AnimalType.TurkeyEggs:
                case AnimalType.YoungTom:
                case AnimalType.Poults:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsLactatingType(this AnimalType animalType)
        {
            switch (animalType)
            {
                case AnimalType.BeefCowLactating:
                case AnimalType.BeefCow:
                case AnimalType.DairyLactatingCow:
                case AnimalType.Ewes:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsEggs(this AnimalType animalType)
        {
            return animalType == AnimalType.ChickenEggs || animalType == AnimalType.TurkeyEggs;
        }

        public static bool IsNewlyHatchedEggs(this AnimalType animalType)
        {
            return animalType == AnimalType.Poults || animalType == AnimalType.Chicks;
        }

        public static bool IsPregnantType(this AnimalType animalType)
        {
            switch (animalType)
            {
                case AnimalType.BeefCow:
                case AnimalType.DairyLactatingCow:
                case AnimalType.Ewes:
                    return true;

                default:
                    return false;
            }
        }

        public static AnimalType GetCategory(this AnimalType animalType)
        {
            if (animalType.IsOtherAnimalType())
            {
                return AnimalType.OtherLivestock;
            }

            if (animalType.IsPoultryType())
            {
                return AnimalType.Poultry;
            }

            if (animalType.IsSheepType())
            {
                return AnimalType.Sheep;
            }

            if (animalType.IsSwineType())
            {
                return AnimalType.Swine;
            }

            if (animalType.IsDairyCattleType())
            {
                return AnimalType.Dairy;
            }

            return AnimalType.Beef;
        }

        public static ComponentCategory GetComponentCategoryFromAnimalType(this AnimalType animalType)
        {
            if (animalType.IsBeefCattleType())
            {
                return ComponentCategory.BeefProduction;
            }
            else if (animalType.IsDairyCattleType())
            {
                return ComponentCategory.Dairy;
            } 
            else if (animalType.IsSwineType())
            {
                return ComponentCategory.Swine;
            }
            else if (animalType.IsPoultryType())
            {
                return ComponentCategory.Poultry;
            }
            else if (animalType.IsSheepType())
            {
                return ComponentCategory.Sheep;
            }
            else
            {
                return ComponentCategory.OtherLivestock;
            }
        }

        public static ManureAnimalSourceTypes GetManureAnimalSource(this AnimalType animalType)
        {
            if (animalType.IsBeefCattleType())
            {
                return ManureAnimalSourceTypes.BeefManure;
            }
            else if (animalType.IsDairyCattleType())
            {
                return ManureAnimalSourceTypes.DairyManure;
            }
            else if (animalType.IsSwineType())
            {
                return ManureAnimalSourceTypes.SwineManure;
            }
            else if (animalType.IsPoultryType())
            {
                return ManureAnimalSourceTypes.PoultryManure;
            }
            else if (animalType.IsSheepType())
            {
                return ManureAnimalSourceTypes.SheepManure;
            }
            else
            {
                return ManureAnimalSourceTypes.OtherLivestockManure;
            }
        }
    }
}