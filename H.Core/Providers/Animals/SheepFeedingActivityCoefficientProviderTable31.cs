using H.Core.Enumerations;
using H.Core.Tools;
using System.Diagnostics;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 31
    /// </summary>
    public class SheepFeedingActivityCoefficientProviderTable31 : IFeedingActivityCoefficientProvider
    {
        public SheepFeedingActivityCoefficientProviderTable31()
        {
            HTraceListener.AddTraceListener(); 
        }
        public IFeedingActivityCoeffientData GetByHousing(HousingType housingType)
        {
            switch (housingType)
            {

               case HousingType.HousedEwes:
                    {
                        return new FeedingActivityCoefficientData()
                        {
                            FeedingActivityCoefficient = 0.0096,
                        };
                    }
                case HousingType.Confined:
                {
                    return new FeedingActivityCoefficientData()
                    {
                        FeedingActivityCoefficient = 0.0067,
                    };
                }

                case HousingType.FlatPasture:
                {
                    return new FeedingActivityCoefficientData()
                    {
                        FeedingActivityCoefficient = 0.0107,
                    };
                }

                case HousingType.HillyPastureOrOpenRange:
                {
                    return new FeedingActivityCoefficientData()
                    {
                        FeedingActivityCoefficient = 0.024
                    };
                }

                default:
                {
                    var defaultValue = new FeedingActivityCoefficientData()
                    {
                        FeedingActivityCoefficient = 0
                    };

                    Trace.TraceError($"{nameof(SheepFeedingActivityCoefficientProviderTable31.GetByHousing)}" +
                    $" unable to get data for housing type: {housingType}." +
                    $" Returning default value of {defaultValue}.");
                    return defaultValue;
                }
            }
        }
    }
}
