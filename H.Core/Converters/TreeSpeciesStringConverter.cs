using System;
using H.Core.Enumerations;
using H.Core.Properties;

namespace H.Core.Converters
{
    public class TreeSpeciesStringConverter : ConverterBase
    {
        public TreeSpecies Convert(string input)
        {
            switch (this.GetLettersAsLowerCase(input))
            {
                case "hybridpoplar":
                    return TreeSpecies.HybridPoplar;
                case "whitespruce":
                    return TreeSpecies.WhiteSpruce;
                case "scotspine":
                    return TreeSpecies.ScotsPine;
                case "manitobamaple":
                    return TreeSpecies.ManitobaMaple;
                case "greenash":
                    return TreeSpecies.GreenAsh;
                case "caragana":
                    return TreeSpecies.Caragana;
                case "averagedeciduous":
                    return TreeSpecies.AverageDeciduous;
                case "averageconifer":
                    return TreeSpecies.AverageConifer;
                default:
                    throw new Exception(string.Format(Resources.ExceptionUnknownTreeSpecies, input));
            }
        }
    }
}