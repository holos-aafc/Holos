namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 39.
    ///
    /// Default values for protein retained for gain by growth stage for growing pigs.
    /// </summary>
    public class DefaultNitrogenGainByGrowthStageProvider
    {
        /// <summary>
        /// Returns the protein required for gain for the given weight of the animal.
        /// </summary>
        /// <param name="averageWeightOfAnimal">Average weight of the animal (kg)</param>
        /// <returns>Protein required for gain (kg head^-1 day^-1)</returns>
        public double GetNitrogenRequiredForGain(double averageWeightOfAnimal)
        {
            if (averageWeightOfAnimal < 4)
            {
                // This has to be zero so that animals that are smaller than this do not use Equation 4.2.1-23, but 4.2.1-22 instead. The effect of returning 0 here
                // is that 4.2.1-23 will return 0 as well.
                return 0;
            }
            else if (averageWeightOfAnimal >= 4 && averageWeightOfAnimal < 7)
            {
                // Nursery
                return 0.031;
            }
            else if (averageWeightOfAnimal >= 7 && averageWeightOfAnimal < 20)
            {
                // Nursery
                return 0.028;
            }
            else if (averageWeightOfAnimal >= 20 && averageWeightOfAnimal < 40)
            {
                // Grower
                return 0.025;
            }
            else if (averageWeightOfAnimal >= 40 && averageWeightOfAnimal < 80)
            {
                // Grower
                return 0.024;
            }
            else if(averageWeightOfAnimal >= 80)
            {
                // Grower
                return 0.021;
            }

            return 0;
        }
    }
}