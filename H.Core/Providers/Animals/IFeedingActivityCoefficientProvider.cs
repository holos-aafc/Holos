using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public interface IFeedingActivityCoefficientProvider
    {
        IFeedingActivityCoeffientData GetByHousing(HousingType housingType);
    }
}