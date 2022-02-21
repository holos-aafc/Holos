using System.Diagnostics;
using H.Core.Models;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public static class ManureAnimalSourceTypeExtension
    {
        public static ComponentCategory GetComponentCategory(this ManureAnimalSourceTypes manureAnimalSourceType)
        {
            switch (manureAnimalSourceType)
            {
                case ManureAnimalSourceTypes.BeefManure:
                    return ComponentCategory.BeefProduction;

                case ManureAnimalSourceTypes.DairyManure:
                    return ComponentCategory.Dairy;

                case ManureAnimalSourceTypes.SwineManure:
                    return ComponentCategory.Swine;

                case ManureAnimalSourceTypes.SheepManure:
                    return ComponentCategory.Sheep;

                case ManureAnimalSourceTypes.PoultryManure:
                    return ComponentCategory.Poultry;

                case ManureAnimalSourceTypes.OtherLivestockManure:
                    return ComponentCategory.OtherLivestock;

                default:
                {
                    Trace.TraceError($"Unknown manure animal source type: {manureAnimalSourceType.GetDescription()}. Returning {ComponentCategory.BeefProduction.GetDescription()}");
                    
                    return ComponentCategory.BeefProduction;
                }
            }
        }
    }
}