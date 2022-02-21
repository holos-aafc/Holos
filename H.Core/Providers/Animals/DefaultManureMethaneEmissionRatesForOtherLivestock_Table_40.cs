using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    public class DefaultManureMethaneEmissionRatesForOtherLivestock_Table_40
    {
        public double GetDailyManureMethaneEmissionRate(AnimalType animalType)
        {
            if (animalType.IsTurkeyType())
            {
                return 0.000247;
            }

            if (animalType == AnimalType.Ducks || animalType == AnimalType.Geese)
            {
                return 0.000055;
            }

            if (animalType == AnimalType.Llamas || animalType == AnimalType.Alpacas)
            {
                return 0.000521;
            }

            if (animalType == AnimalType.Deer || animalType == AnimalType.Elk)
            {
                return 0.000603;
            }

            System.Diagnostics.Trace.TraceError(
                $"{nameof(DefaultManureMethaneEmissionRatesForOtherLivestock_Table_40)}.{nameof(GetDailyManureMethaneEmissionRate)}" +
                $": Unable to get data for animal type: {animalType.GetDescription()}." +
                $" Returning default value of 0.");

            return 0;
        }
    }
}