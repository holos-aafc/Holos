#region Imports

using H.Core.Enumerations;
using H.Core.Tools;

#endregion

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 17
    /// </summary>
    public class BeefCattleFeedingActivityCoefficientDataProvider_Table_17 : IFeedingActivityCoefficientProvider
    {
        public BeefCattleFeedingActivityCoefficientDataProvider_Table_17()
        {
            HTraceListener.AddTraceListener();
        }
        public IFeedingActivityCoeffientData GetByHousing(HousingType housingType)
        {
            switch (housingType)
            {
                case HousingType.HousedInBarn:
                case HousingType.Confined:
                case HousingType.ConfinedNoBarn:
                {
                    return new FeedingActivityCoefficientData()
                    {
                        FeedingActivityCoefficient = 0,
                    };
                }

                case HousingType.Pasture:
                case HousingType.EnclosedPasture:
                {
                    return new FeedingActivityCoefficientData()
                    {
                        FeedingActivityCoefficient = 0.17,
                    };
                }

                case HousingType.OpenRangeOrHills:
                {
                    return new FeedingActivityCoefficientData()
                    {
                        FeedingActivityCoefficient = 0.36,
                    };
                }

                default:
                {
                    var defaultValue = new FeedingActivityCoefficientData()
                    {
                        FeedingActivityCoefficient = 0,
                    };
                    System.Diagnostics.Trace.TraceError($"{nameof(BeefCattleFeedingActivityCoefficientDataProvider_Table_17)}.{nameof(BeefCattleFeedingActivityCoefficientDataProvider_Table_17.GetByHousing)}" +
                        $" unable to get data for housing type: {housingType}." +
                        $" Returning default value of {defaultValue}.");
                    return defaultValue;
                }
            }
        }
    }
}