using H.Core.Models.Results;

namespace H.Core.Models
{
    public class SoilCarbonEmissionResult : ResultsViewItemBase
    {
        #region Properties

        public LandUseCarbonChangeSource CarbonChangeSource { get; set; }

        /// <summary>
        /// (kg C year^-1)
        /// </summary>
        public double CarbonChangeForSoil { get; set; }

        /// <summary>
        /// (kg CO2 year^-1)
        /// </summary>
        public double CarbonDioxideChangeForSoil { get; set; }

        #endregion
    }
}