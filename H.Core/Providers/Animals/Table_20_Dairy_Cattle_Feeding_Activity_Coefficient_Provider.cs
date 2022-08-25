using H.Core.Enumerations;
using H.Core.Tools;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 20. Feeding activity coefficients (Ca) for dairy cattle.
    /// </summary>
    public class Table_20_Dairy_Cattle_Feeding_Activity_Coefficient_Provider : IFeedingActivityCoefficientProvider
    {
        public Table_20_Dairy_Cattle_Feeding_Activity_Coefficient_Provider()
        {
            HTraceListener.AddTraceListener();
        }

        public IFeedingActivityCoeffientData GetByHousing(HousingType housingType)
        {
            if (housingType.IsFreeStall() || housingType == HousingType.Confined)
            {
                return new Table_20_Cattle_Feeding_Activity_Coefficient_Data()
                {
                    FeedingActivityCoefficient = 0,
                };
            }
            else
            {
                // Pasture
                return new Table_20_Cattle_Feeding_Activity_Coefficient_Data()
                {
                    FeedingActivityCoefficient = 0.17,
                };
            }
        }
    }
}