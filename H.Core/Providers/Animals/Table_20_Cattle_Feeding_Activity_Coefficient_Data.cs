#region Imports

using H.Core.Enumerations;

#endregion

namespace H.Core.Providers.Animals
{
    public class Table_20_Cattle_Feeding_Activity_Coefficient_Data : IFeedingActivityCoeffientData
    {
        #region Properties

        public HousingType HousingType { get; set; }
        public double FeedingActivityCoefficient { get; set; }

        #endregion
    }
}