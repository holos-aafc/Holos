#region Imports

using System.Diagnostics;
using H.Core.Enumerations;
using H.Core.Tools;

#endregion

namespace H.Core.Providers.Animals
{
    /// <summary>
    ///     Table 17. Feeding activity coefficients (Ca) for beef cattle
    ///     Source: IPCC (2019, Table 10.5).
    /// </summary>
    public class Table_17_Beef_Dairy_Cattle_Feeding_Activity_Coefficient_Provider : IFeedingActivityCoefficientProvider
    {
        public Table_17_Beef_Dairy_Cattle_Feeding_Activity_Coefficient_Provider()
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
                    return new Table_17_Cattle_Feeding_Activity_Coefficient_Data
                    {
                        FeedingActivityCoefficient = 0
                    };
                }

                case HousingType.Pasture:
                case HousingType.FlatPasture:
                case HousingType.EnclosedPasture:
                {
                    return new Table_17_Cattle_Feeding_Activity_Coefficient_Data
                    {
                        FeedingActivityCoefficient = 0.17
                    };
                }

                case HousingType.OpenRangeOrHills:
                {
                    return new Table_17_Cattle_Feeding_Activity_Coefficient_Data
                    {
                        FeedingActivityCoefficient = 0.36
                    };
                }

                default:
                {
                    var defaultValue = new Table_17_Cattle_Feeding_Activity_Coefficient_Data
                    {
                        FeedingActivityCoefficient = 0
                    };
                    Trace.TraceError(
                        $"{nameof(Table_17_Beef_Dairy_Cattle_Feeding_Activity_Coefficient_Provider)}.{nameof(GetByHousing)}" +
                        $" unable to get data for housing type: {housingType}." +
                        $" Returning default value of {defaultValue}.");
                    return defaultValue;
                }
            }
        }
    }
}