#region Imports

using H.Core.Enumerations;

#endregion

namespace H.Core.Providers.Animals
{
    public class FeedingActivityCoefficientData : IFeedingActivityCoeffientData
    {
        #region Properties

        public HousingType HousingType { get; set; }
        public double FeedingActivityCoefficient { get; set; }

        #endregion
    }
}