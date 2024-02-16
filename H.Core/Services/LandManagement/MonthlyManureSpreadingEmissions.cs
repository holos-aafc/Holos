using H.Core.Models.Results;

namespace H.Core.Services.LandManagement
{
    public class MonthlyManureSpreadingEmissions : MonthlyManureSpreadingData
    {
        #region Constructors

        public MonthlyManureSpreadingEmissions()
        {
        }

        public MonthlyManureSpreadingEmissions(MonthlyManureSpreadingData monthlyManureSpreadingData)
        {
            base.Year = monthlyManureSpreadingData.Year;
            base.Month = monthlyManureSpreadingData.Month;
        }

        #endregion

        #region Properties

        /// <summary>
        /// (kg CO2)
        /// </summary>
        public double TotalEmissions { get; set; } 

        #endregion
    }
}