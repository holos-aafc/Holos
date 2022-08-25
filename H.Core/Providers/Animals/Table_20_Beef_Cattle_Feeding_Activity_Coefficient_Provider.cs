#region Imports

using H.Core.Enumerations;
using H.Core.Tools;

#endregion

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 20. Feeding activity coefficients (Ca) for beef cattle
    /// Source: IPCC (2019, Table 10.5).
    /// </summary>
    public class Table_20_Beef_Cattle_Feeding_Activity_Coefficient_Provider : IFeedingActivityCoefficientProvider
    {
        public Table_20_Beef_Cattle_Feeding_Activity_Coefficient_Provider()
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
                    return new Table_20_Cattle_Feeding_Activity_Coefficient_Data()
                    {
                        FeedingActivityCoefficient = 0,
                    };
                }

                case HousingType.Pasture:
                case HousingType.EnclosedPasture:
                {
                    return new Table_20_Cattle_Feeding_Activity_Coefficient_Data()
                    {
                        FeedingActivityCoefficient = 0.17,
                    };
                }

                case HousingType.OpenRangeOrHills:
                {
                    return new Table_20_Cattle_Feeding_Activity_Coefficient_Data()
                    {
                        FeedingActivityCoefficient = 0.36,
                    };
                }

                default:
                {
                    var defaultValue = new Table_20_Cattle_Feeding_Activity_Coefficient_Data()
                    {
                        FeedingActivityCoefficient = 0,
                    };
                    System.Diagnostics.Trace.TraceError($"{nameof(Table_20_Beef_Cattle_Feeding_Activity_Coefficient_Provider)}.{nameof(Table_20_Beef_Cattle_Feeding_Activity_Coefficient_Provider.GetByHousing)}" +
                        $" unable to get data for housing type: {housingType}." +
                        $" Returning default value of {defaultValue}.");
                    return defaultValue;
                }
            }
        }
    }
}