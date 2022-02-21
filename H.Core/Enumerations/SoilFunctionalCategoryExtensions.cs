using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Core.Enumerations
{
    public static class SoilFunctionalCategoryExtensions
    {
        /// <summary>
        /// General rule of soil zones:  Brown <=> Dark Brown <=> Black.
        /// As in pull from the direct neighbor, and jump if the neighbor doesnt have the info you are looking for.
        /// Likely used for just the economics module
        ///
        /// this function will return the next soil category from above or the next best 
        /// </summary>
        public static SoilFunctionalCategory GetNeighbouringCategory(this SoilFunctionalCategory soilFunctionalCategory)
        {
            if (soilFunctionalCategory.IsBlack()) return SoilFunctionalCategory.Brown;

            if (soilFunctionalCategory.IsDarkBrown()) return SoilFunctionalCategory.Black;

            return soilFunctionalCategory.IsBrown() ? SoilFunctionalCategory.DarkBrown : SoilFunctionalCategory.NotApplicable;
        }

        public static bool IsBlack(this SoilFunctionalCategory soilFunctionalCategory)
        {
            return soilFunctionalCategory == SoilFunctionalCategory.Black ||
                   soilFunctionalCategory == SoilFunctionalCategory.BlackGrayChernozem;
        }

        public static bool IsBrown(this SoilFunctionalCategory soilFunctionalCategory)
        {
            return soilFunctionalCategory == SoilFunctionalCategory.Brown ||
                   soilFunctionalCategory == SoilFunctionalCategory.BrownChernozem ||
                   soilFunctionalCategory == SoilFunctionalCategory.DarkBrown ||
                   soilFunctionalCategory == SoilFunctionalCategory.DarkBrownChernozem;
        }

        public static bool IsDarkBrown(this SoilFunctionalCategory soilFunctionalCategory)
        {
            return soilFunctionalCategory == SoilFunctionalCategory.DarkBrown ||
                   soilFunctionalCategory == SoilFunctionalCategory.DarkBrownChernozem;
        }
    }
}
