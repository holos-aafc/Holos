using System;
using H.Core.Enumerations;
using H.Core.Properties;
using H.Core.Providers.Feed;

namespace H.Core.Converters
{
    public class DietTypeStringConverter : ConverterBase
    {
        public DietType Convert(string input)
        {
            switch (this.GetLettersAsLowerCase(input))
            {
                case "barley":
                case "barleybased":
                    return DietType.Barley;
                case "closeup":
                    return DietType.CloseUp;
                case "corn":
                case "cornbased":
                    return DietType.Corn;
                case "faroffdry":
                    return DietType.FarOffDry;
                case "highenergy":
                    return DietType.HighEnergy;
                case "highenergyptn":
                case "highenergyprotein":
                    return DietType.HighEnergyAndProtein;
                case "lowenergy":
                    return DietType.LowEnergy;
                case "lowenergyptn":
                case "lowenergyprotein":
                    return DietType.LowEnergyAndProtein;
                case "mediumenergy":
                    return DietType.MediumEnergy;
                case "mediumenergyptn":
                case "mediumenergyprotein":
                    return DietType.MediumEnergyAndProtein;
                case "mediumgrowth":
                    return DietType.MediumGrowth;
                case "slowgrowth":
                    return DietType.SlowGrowth;
                case "standard":
                    return DietType.Standard;
                case "reducedprotein":
                    return DietType.ReducedProtein;
                case "highlydigestiblefeed":
                    return DietType.HighlyDigestibleFeed;
                case "legumeforagebased":
                    return DietType.LegumeForageBased;
                case "barleysilagebased":
                    return DietType.BarleySilageBased;
                case "cornsilagebased":
                    return DietType.CornSilageBased;
                case "highfiber":
                    return DietType.HighFiber;
                case "lowfiber":
                    return DietType.LowFiber;
                case "all":
                    return DietType.All;
                case "foragebased":
                case "forage":
                    return DietType.Forage;
                default:
                    throw new Exception(string.Format(Resources.ExceptionUnknownDietType, input));
            }
        }
    }
}