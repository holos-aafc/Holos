using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// No table number
    /// </summary>
    public class DefaultDailyTanExcretionRatesForPoultry
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>kg TAN head^-1 day^-1</returns>
        public double GetDailyTanExcretionRate(AnimalType animalType)
        {
            if (animalType.IsChickenType())
            {
                if (animalType == AnimalType.ChickenHens)
                {
                    return 0.0007;
                }
                else
                {
                    return 0.0011;
                }
            }
            else
            {
                // Turkeys
                return 0.0026;
            }
        }
    }
}