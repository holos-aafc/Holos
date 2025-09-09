namespace H.Core.Services.LandManagement
{
    public class MonthlyManureSpreadingEmissions : MonthlyManureSpreadingData
    {
        #region Properties

        /// <summary>
        ///     (kg CO2)
        /// </summary>
        public double TotalEmissions { get; set; }

        #endregion

        #region Constructors

        public MonthlyManureSpreadingEmissions()
        {
        }

        public MonthlyManureSpreadingEmissions(MonthlyManureSpreadingData monthlyManureSpreadingData)
        {
            Year = monthlyManureSpreadingData.Year;
            Month = monthlyManureSpreadingData.Month;
        }

        #endregion
    }
}