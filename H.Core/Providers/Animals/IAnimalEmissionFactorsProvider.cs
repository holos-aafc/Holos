using H.Core.Enumerations;
using H.Core.Models;

namespace H.Core.Providers.Animals
{
    public interface IAnimalEmissionFactorsProvider
    {
        IEmissionData GetFactors(
            ManureStateType manureStateType, 
            ComponentCategory componentCategory,
            double meanAnnualPrecipitation, 
            double meanAnnualTemperature,
            double meanAnnualEvapotranspiration, 
            double beddingRate,
            AnimalType animalType, 
            Farm farm, 
            int year);

        IEmissionData GetLandApplicationFactors(
            Farm farm,
            double meanAnnualPrecipitation,
            double meanAnnualEvapotranspiration,
            AnimalType animalType,
            int year);

        double GetEmissionFactorForVolatilizationBasedOnClimate(double precipitation, double evapotranspiration);
        double GetVolatilizationFractionForLandApplication(AnimalType animalType, Province province, int year);
    }
}