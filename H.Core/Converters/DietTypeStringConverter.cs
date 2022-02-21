using System;
using H.Core.Enumerations;
using H.Core.Properties;

namespace H.Core.Converters
{
    public class DietTypeStringConverter : ConverterBase
    {
        public DietType Convert(string input)
        {
            switch (this.GetLettersAsLowerCase(input))
            {
                case "barley":
                    return DietType.Barley;
                case "closeup":
                    return DietType.CloseUp;
                case "corn":
                    return DietType.Corn;
                case "faroffdry":
                    return DietType.FarOffDry;
                case "highenergy":
                    return DietType.HighEnergy;
                case "highenergyptn":
                    return DietType.HighEnergyAndProtein;
                case "lowenergy":
                    return DietType.LowEnergy;
                case "lowenergyptn":
                    return DietType.LowEnergyAndProtein;
                case "mediumenergy":
                    return DietType.MediumEnergy;
                case "mediumenergyptn":
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
                default:
                    throw new Exception(string.Format(Resources.ExceptionUnknownDietType, input));
            }
        }
    }
}