using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// No table number
    /// </summary>
    public class DefaultYearlyTanExcretionRatesForPoultry
    {
        public double GetYearlyTanExcretion(AnimalType animalType)
        {
            if (animalType.IsChickenType())
            {
                if (animalType == AnimalType.ChickenHens)
                {
                    return 0.4;
                }
                else
                {
                    return 0.18;
                }
            }
            else
            {
                // Turkeys
                return 0.83;
            }
        }
    }
}