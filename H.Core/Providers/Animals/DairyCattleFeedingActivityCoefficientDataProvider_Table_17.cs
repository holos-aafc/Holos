using H.Core.Enumerations;
using H.Core.Tools;

namespace H.Core.Providers.Animals
{   
    /// <summary>
    /// Table 17
    /// </summary>
    public class DairyCattleFeedingActivityCoefficientDataProvider_Table_17 : IFeedingActivityCoefficientProvider
    {
        public DairyCattleFeedingActivityCoefficientDataProvider_Table_17()
        {
            HTraceListener.AddTraceListener();
        }

        public IFeedingActivityCoeffientData GetByHousing(HousingType housingType)
        {
            if (housingType.IsFreeStall() || housingType == HousingType.Confined)
            {
                return new FeedingActivityCoefficientData()
                {
                    FeedingActivityCoefficient = 0,
                };
            }
            else
            {
                // Pasture
                return new FeedingActivityCoefficientData()
                {
                    FeedingActivityCoefficient = 0.17,
                };
            }
        }
    }
}