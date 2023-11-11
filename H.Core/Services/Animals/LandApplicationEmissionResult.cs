using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services.Animals
{
    public class LandApplicationEmissionResult
    {
        public CropViewItem CropViewItem { get; set; }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double TotalIndirectN2OEmissions { get; set; }

        /// <summary>
        /// (kg N2O-N)
        /// </summary>
        public double TotalIndirectN2ONEmissions { get; set; }

        /// <summary>
        /// (kg NO3-N)
        /// </summary>
        public double TotalNitrateLeached { get; set; }

        /// <summary>
        /// (kg NH3-N)
        /// </summary>
        public double AdjustedAmmoniacalLoss { get; set; }

        /// <summary>
        /// (kg NH3)
        /// </summary>
        public double AdjustedAmmoniaLoss { get; set; }

        /// <summary>
        /// (kg N2O-N)
        /// </summary>
        public double TotalN2ONFromManureVolatilized;

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double TotalN2OFromManureVolatilized { get; set; }

        /// <summary>
        /// (kg N2O-N)
        /// </summary>
        public double TotalN2ONFromDigestateVolatilized;

        /// <summary>
        /// (kg N2O-N)
        /// </summary>
        public double TotalN2ONFromManureLeaching;

        /// <summary>
        /// (kg N2O-N)
        /// </summary>
        public double TotalN2ONFromDigestateLeaching;

        /// <summary>
        /// (kg / L)
        /// </summary>
        public double TotalVolumeOfManureUsedDuringApplication { get; set; }

        /// <summary>
        /// (kg NH3-N)
        /// </summary>
        public double AmmoniacalLoss { get; set; }

        /// <summary>
        /// (kg NH3)
        /// </summary>
        public double AmmoniaLoss { get; set; }

        /// <summary>
        /// (kg)
        /// </summary>
        public double TotalTANApplied { get; set; }

        /// <summary>
        /// The actual amount of N that was applied from the manure application
        /// 
        /// (kg N)
        /// </summary>
        public double ActualAmountOfNitrogenAppliedFromLandApplication { get; set; }
    }
}