using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public interface IFeedingActivityCoeffientData
    {
        HousingType HousingType { get; set; }
        double FeedingActivityCoefficient { get; set; }
    }
}