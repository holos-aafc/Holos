using H.Core.Enumerations;
using H.Core.Tools;
using System.Diagnostics;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 26. Feeding activity coefficients for sheep.
    /// </summary>
    public class Table_26_Feeding_Activity_Coefficient_Sheep_Provider : IFeedingActivityCoefficientProvider
    {
        public Table_26_Feeding_Activity_Coefficient_Sheep_Provider()
        {
            HTraceListener.AddTraceListener(); 
        }
        public IFeedingActivityCoeffientData GetByHousing(HousingType housingType)
        {
            switch (housingType)
            {
                
                case HousingType.HousedEwes: // Footnote 1
                    {
                        return new Table_20_Cattle_Feeding_Activity_Coefficient_Data()
                        {
                            FeedingActivityCoefficient = 0.0096,
                        };
                    }
                
                case HousingType.Confined: // Footnote 2
                    {
                    return new Table_20_Cattle_Feeding_Activity_Coefficient_Data()
                    {
                        FeedingActivityCoefficient = 0.0067,
                    };
                }

                case HousingType.FlatPasture:
                {
                    return new Table_20_Cattle_Feeding_Activity_Coefficient_Data()
                    {
                        FeedingActivityCoefficient = 0.0107,
                    };
                }

                case HousingType.HillyPastureOrOpenRange:
                {
                    return new Table_20_Cattle_Feeding_Activity_Coefficient_Data()
                    {
                        FeedingActivityCoefficient = 0.024
                    };
                }

                default:
                {
                    var defaultValue = new Table_20_Cattle_Feeding_Activity_Coefficient_Data()
                    {
                        FeedingActivityCoefficient = 0
                    };

                    Trace.TraceError($"{nameof(Table_26_Feeding_Activity_Coefficient_Sheep_Provider.GetByHousing)}" +
                    $" unable to get data for housing type: {housingType}." +
                    $" Returning default value of {defaultValue}.");
                    return defaultValue;
                }
            }
        }

        #region Footnotes

        // Footnote 1: Animals are confined due to pregnancy in final trimester (50 days) (IPCC, 2019)
        // Footnote 2: Animals housed for fattening
        #endregion
    }
}
