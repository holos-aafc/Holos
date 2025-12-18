namespace H.Core.Providers.Animals
{
    public class FractionOfOrganicNitrogenMineralizedData
    {
        /// <summary>
        /// Mineralization of organic N (fecal N and bedding N)
        ///
        /// (unitless)
        /// </summary>
        public double FractionMineralized { get; set; }

        /// <summary>
        /// (unitless)
        /// </summary>
        public double FractionImmobilized { get; set; }

        /// <summary>
        /// (unitless)
        /// </summary>
        public double FractionNitrified { get; set; }

        /// <summary>
        /// (unitless)
        /// </summary>
        public double FractionDenitrified { get; set; }

        public double N2O_N { get; set; }
        public double NO_N { get; set; }
        public double N2_N { get; set; }
        public double N_Leached { get; set; }
    }
}