using H.Core.Models.Results;

namespace H.Core.Services.LandManagement
{
    public class MonthlyManureSpreadingResults
    {
        public int Month { get; set; }
        public int Year { get; set; }

        /// <summary>
        /// (kg CO2)
        /// </summary>
        public double TotalEmissions { get; set; }
    }
}