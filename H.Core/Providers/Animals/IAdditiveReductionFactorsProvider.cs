using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public interface IAdditiveReductionFactorsProvider
    {
        double GetAdditiveReductionFactor(DietAdditiveType additiveType, double numberOfDays, double fat);
    }
}