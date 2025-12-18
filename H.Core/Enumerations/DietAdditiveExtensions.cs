namespace H.Core.Enumerations
{
    public static class DietAdditiveExtensions
    {
        /// <summary>
        /// Returns the fractional value of the diet additive that is fat.
        /// </summary>
        /// <param name="dietAdditiveType">The type of additive</param>
        /// <returns>Fraction of diet additive that is fat</returns>
        public static double GetFatFromAdditive(this DietAdditiveType dietAdditiveType)
        {
            switch (dietAdditiveType)
            {
                case DietAdditiveType.TwoPercentFat:
                case DietAdditiveType.InonophorePlusTwoPercentFat:
                    return 0.02;

                case DietAdditiveType.FourPercentFat:
                case DietAdditiveType.InonophorePlusFourPercentFat:
                    return 0.04;

                case DietAdditiveType.FivePercentFat:
                case DietAdditiveType.IonophorePlusFivePercentFat:
                    return 0.05;

                default:
                    return 0;
            }
        }
    }
}