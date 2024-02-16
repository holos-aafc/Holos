namespace H.Core.Services
{
    public class MonthlyManureSpreadingData
    {
        #region Properties
        
        public int Month { get; set; }
        public int Year { get; set; }

        /// <summary>
        /// (kg)
        /// </summary>
        public double TotalVolume { get; set; }

        #endregion
    }
}