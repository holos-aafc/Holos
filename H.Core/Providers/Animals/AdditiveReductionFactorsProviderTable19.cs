using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 19.
    ///
    /// Additive reduction factors for beef cattle and dairy cattle.
    /// </summary>
    public class AdditiveReductionFactorsProviderTable19 : IAdditiveReductionFactorsProvider
    {
        /// <param name="additiveType">Type of diet additive</param>
        /// <param name="numberOfDays">Number of days animals are fed the additive</param>
        /// <param name="fat">The % fat for beef diets, and the % EE for dairy diets</param>
        public double GetAdditiveReductionFactor(
            DietAdditiveType additiveType, 
            double numberOfDays, 
            double fat)
        {
            if (numberOfDays <= 0)
            {
                return 0;
            }

            switch (additiveType)
            {
                case DietAdditiveType.TwoPercentFat:                
                    return 10;

                case DietAdditiveType.FourPercentFat:
                    return 20;

                case DietAdditiveType.Inonophore:
                    return 20 * 30 / numberOfDays;

                case DietAdditiveType.InonophorePlusTwoPercentFat:
                    return 10 + 0.5 * 20 * 30 / numberOfDays;

                case DietAdditiveType.InonophorePlusFourPercentFat:
                    return 20 + 0.5 * 20 * 30 / numberOfDays;

                case DietAdditiveType.FivePercentFat:
                    return 5 * (fat <= 6 ? fat : 6); // Up to 6 % added fat is possible

                case DietAdditiveType.IonophorePlusFivePercentFat:
                    return 5 * (fat <= 6 ? fat : 6) + 0.5 * (20 * 30 / numberOfDays); // Up to 6 % added fat is possible

                default:
                    return 0;
            }
        }
    }
}