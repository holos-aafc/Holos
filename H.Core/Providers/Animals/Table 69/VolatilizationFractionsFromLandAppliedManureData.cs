using H.Core.Enumerations;

namespace H.Core.Providers.Animals.Table_69
{
    public class VolatilizationFractionsFromLandAppliedManureData
    {
        #region Properties

        public Province Province { get; set; }
        public int Year { get; set; }

        /// <summary>
        ///     (kg NH3-N volatilized kg^-1 manure N applied)
        /// </summary>
        public double ImpliedEmissionFactor { get; set; }

        #endregion
    }
}